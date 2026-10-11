using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace TheIsle
{
    public sealed class BroadsideCombat : MonoBehaviour
    {
        public const float ShotSpeed = 32f;
        public const float ShotRange = 95f;
        private readonly float[] reload = { 1f, 1f };
        private ShipMotor motor;
        private ShipVitals vitals;
        private CrewRoster crew;
        public event Action<Broadside> Fired;

        public void Initialize(ShipMotor shipMotor, ShipVitals shipVitals, CrewRoster roster)
        {
            motor = shipMotor;
            vitals = shipVitals;
            crew = roster;
            reload[0] = reload[1] = 1f;
        }

        public float Reload01(Broadside side) => reload[(int)side];

        public bool InArc(Broadside side, Vector3 aimPoint)
        {
            Vector3 aim = FlightPlane.Flat(aimPoint - transform.position);
            if (aim.sqrMagnitude < .001f) return false;
            Vector3 bankDirection = FlightPlane.Flat(transform.right) * (side == Broadside.Port ? -1f : 1f);
            return Vector3.Dot(bankDirection.normalized, aim.normalized) >= .8660254f;
        }

        public bool TryFire(Broadside side, Vector3 aimPoint)
        {
            if (Time.timeScale <= 0 || vitals == null || vitals.IsDestroyed || motor == null || !motor.ControlsEnabled ||
                reload[(int)side] < .99999f || !InArc(side, aimPoint))
                return false;
            reload[(int)side] = 0f;
            float sign = side == Broadside.Port ? -1f : 1f;
            for (int gun = 0; gun < 2; gun++)
            {
                Vector3 origin = transform.position + transform.right * (sign * 3.1f) + transform.forward * (gun == 0 ? -1.65f : 1.65f);
                origin.y = 0f;
                Vector3 direction = FlightPlane.Flat(aimPoint - origin).normalized;
                direction = Quaternion.AngleAxis(UnityEngine.Random.Range(-1.1f, 1.1f), Vector3.up) * direction;
                Cannonball.Spawn(origin, direction * ShotSpeed + motor.Velocity, vitals, motor.Layer);
            }
            Fired?.Invoke(side);
            return true;
        }

        private void Update()
        {
            if (Time.timeScale <= 0 || vitals == null || vitals.IsDestroyed || crew == null) return;
            ReloadBanks(Time.deltaTime);
            if (vitals.Team != ShipTeam.Player) return;
            // A UI click must never also launch a volley behind the interface.
            bool overUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;
            bool portKey = keyboard != null && keyboard.qKey.wasPressedThisFrame;
            bool starboardKey = keyboard != null && keyboard.rKey.wasPressedThisFrame;
            bool portMouse = !overUi && mouse != null && mouse.leftButton.wasPressedThisFrame;
            bool starboardMouse = !overUi && mouse != null && mouse.rightButton.wasPressedThisFrame;
            if (!portKey && !starboardKey && !portMouse && !starboardMouse) return;
            Vector3 point;
            bool mouseAim = MouseAim(out point);
            if (portKey || portMouse)
            {
                Vector3 aim = mouseAim ? point : transform.position - transform.right * 60f;
                if (portKey && !InArc(Broadside.Port, aim)) aim = transform.position - transform.right * 60f;
                TryFire(Broadside.Port, aim);
            }
            if (starboardKey || starboardMouse)
            {
                Vector3 aim = mouseAim ? point : transform.position + transform.right * 60f;
                if (starboardKey && !InArc(Broadside.Starboard, aim)) aim = transform.position + transform.right * 60f;
                TryFire(Broadside.Starboard, aim);
            }
        }

        private void ReloadBanks(float deltaTime)
        {
            int active = (reload[0] < 1f ? 1 : 0) + (reload[1] < 1f ? 1 : 0);
            if (active == 0) return;
            // Both banks share the very same gunners. Turning around does not double their work.
            float sharedWork = deltaTime * crew.WorkRate(CrewStation.Gunnery) / (10f * active);
            for (int i = 0; i < 2; i++)
            {
                if (reload[i] >= 1f) continue;
                ShipModule module = i == 0 ? ShipModule.PortGuns : ShipModule.StarboardGuns;
                reload[i] = Mathf.Min(1f, reload[i] + sharedWork * (.4f + .6f * vitals.Integrity(module)));
            }
        }

        private static bool MouseAim(out Vector3 point)
        {
            point = Vector3.zero;
            if (Mouse.current == null || Camera.main == null) return false;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (!plane.Raycast(ray, out float distance)) return false;
            point = ray.GetPoint(distance);
            return true;
        }
    }
}
