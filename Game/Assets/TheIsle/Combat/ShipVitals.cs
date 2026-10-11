using System;
using UnityEngine;

namespace TheIsle
{
    public sealed class ShipVitals : MonoBehaviour
    {
        private const float ModuleMax = 100f;
        private readonly float[] modules = { ModuleMax, ModuleMax, ModuleMax, ModuleMax };
        private CrewRoster crew;
        public ShipTeam Team { get; private set; }
        public float Hull { get; private set; }
        public float MaxHull => 280f;
        public bool IsDestroyed => Hull <= 0f;
        public float RepairParts { get; private set; }
        public float RepairBudget { get; private set; }
        public ShipModule RepairTarget { get; private set; }
        public bool IsRepairing { get; private set; }
        public event Action<ShipModule, float> Damaged;
        public event Action Destroyed;

        public void Initialize(CrewRoster roster, ShipTeam team)
        {
            crew = roster;
            Team = team;
            Hull = MaxHull;
            for (int i = 0; i < modules.Length; i++) modules[i] = ModuleMax;
            RepairParts = 6f;
            RepairBudget = 35f;
            RepairTarget = ShipModule.Hull;
            IsRepairing = false;
        }

        public float Integrity(ShipModule module)
        {
            if (module == ShipModule.Hull) return Mathf.Clamp01(Hull / MaxHull);
            int index = (int)module;
            return index >= 0 && index < modules.Length ? Mathf.Clamp01(modules[index] / ModuleMax) : 0f;
        }

        public void Damage(float amount, ShipModule module = ShipModule.Hull)
        {
            if (IsDestroyed || amount <= 0f || float.IsNaN(amount) || float.IsInfinity(amount)) return;
            float actual = Mathf.Min(Hull, amount);
            Hull -= actual;
            int index = (int)module;
            if (module != ShipModule.Hull && index >= 0 && index < modules.Length)
                modules[index] = Mathf.Max(0f, modules[index] - amount * .5f);
            Damaged?.Invoke(module, actual);
            if (!IsDestroyed) return;
            IsRepairing = false;
            crew?.MarkShipDestroyed();
            Destroyed?.Invoke();
        }

        public bool StartRepair(ShipModule module)
        {
            if (IsDestroyed || RepairParts <= 0f || RepairBudget <= 0f || Integrity(module) >= .99999f ||
                (int)module < 0 || (int)module >= modules.Length) return false;
            RepairTarget = module;
            IsRepairing = true;
            return true;
        }

        public void StopRepair() => IsRepairing = false;

        private void Update()
        {
            if (!IsRepairing || IsDestroyed || crew == null) return;
            float missing = RepairTarget == ShipModule.Hull ? MaxHull - Hull : ModuleMax - modules[(int)RepairTarget];
            float repair = Mathf.Min(Mathf.Min(missing, RepairBudget), RepairParts * 10f);
            repair = Mathf.Min(repair, Mathf.Min(1.3f, .8f * crew.WorkRate(CrewStation.Engineering)) * Time.deltaTime);
            if (repair > 0f)
            {
                if (RepairTarget == ShipModule.Hull) Hull += repair;
                else modules[(int)RepairTarget] += repair;
                RepairParts = Mathf.Max(0f, RepairParts - repair * .1f);
                RepairBudget = Mathf.Max(0f, RepairBudget - repair);
            }
            if (Integrity(RepairTarget) >= .99999f || RepairParts <= .00001f || RepairBudget <= .00001f)
                IsRepairing = false;
        }

        public void RestoreAtHarbor()
        {
            // Harbor service restores a surviving vessel, never resurrects a lost ship or officers.
            if (IsDestroyed) return;
            Hull = MaxHull;
            for (int i = 0; i < modules.Length; i++) modules[i] = ModuleMax;
            RepairParts = 6f;
            RepairBudget = 35f;
            IsRepairing = false;
        }

        public void AddRepairParts(float amount)
        {
            if (!IsDestroyed && amount > 0f && !float.IsNaN(amount) && !float.IsInfinity(amount))
                RepairParts += amount;
        }
    }
}
