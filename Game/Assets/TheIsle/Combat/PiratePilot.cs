using UnityEngine;

namespace TheIsle
{
    /// <summary>The pirate steers the real motor and fires the same finite-reload weapons.</summary>
    public sealed class PiratePilot : MonoBehaviour
    {
        private ShipMotor motor;
        private ShipMotor target;
        private ShipVitals vitals;
        private BroadsideCombat combat;
        private bool retreating;
        private bool engaged;
        private float awayFor;
        private float repairDecisionAt;
        private float orbitSign;
        public bool Escaped { get; private set; }

        public void Initialize(ShipMotor shipMotor, ShipVitals shipVitals, BroadsideCombat weapons, ShipMotor victim)
        {
            motor = shipMotor;
            vitals = shipVitals;
            combat = weapons;
            target = victim;
            retreating = engaged = Escaped = false;
            awayFor = 0f;
            repairDecisionAt = Time.time;
            Vector3 toward = FlightPlane.Flat(victim.transform.position - transform.position);
            orbitSign = Vector3.Dot(Vector3.Cross(Vector3.up, toward), transform.forward) >= 0f ? 1f : -1f;
        }

        private void FixedUpdate()
        {
            if (motor == null || vitals == null || combat == null || target == null || vitals.IsDestroyed || Escaped)
                return;
            if (motor.Layer != target.Layer || !target.ControlsEnabled)
            {
                motor.SetCommand(0f, 0f, true);
                awayFor = 0f;
                return;
            }
            Vector3 toward = FlightPlane.Flat(target.transform.position - transform.position);
            float distance = toward.magnitude;
            if (distance < 110f || vitals.Hull < vitals.MaxHull) engaged = true;
            if (engaged && distance > 140f) awayFor += Time.fixedDeltaTime;
            else awayFor = 0f;
            if (awayFor >= 12f)
            {
                Escaped = true;
                motor.SetCommand(0f, 0f, true);
                return;
            }
            if (vitals.Integrity(ShipModule.Hull) < .25f) retreating = true;
            if (distance < .01f) toward = transform.forward;
            else toward /= distance;
            if (!engaged)
            {
                motor.SetCommand(0f, 0f, true);
                return;
            }

            Vector3 aim = InterceptPoint(target.transform.position, target.Velocity - motor.Velocity);
            Vector3 heading;
            float throttle;
            if (retreating)
            {
                heading = -toward;
                throttle = 1f;
            }
            else
            {
                Vector3 tangent = Vector3.Cross(Vector3.up, toward) * orbitSign;
                // Approach from range, then circle so the victim crosses a broadside, never a bow turret.
                float radial = Mathf.Clamp((distance - 54f) / 20f, -1.2f, 1.2f);
                Broadside orbitBank = orbitSign > 0f ? Broadside.Port : Broadside.Starboard;
                bool loading = combat.Reload01(orbitBank) < .65f;
                if (loading && distance < 65f) radial -= .3f;
                // Outside gun range, close directly instead of spiralling farther from the target.
                heading = distance > 85f ? toward : (tangent + toward * radial).normalized;
                throttle = distance > 85f ? 1f : Mathf.Clamp(.48f + (distance - 54f) / 90f, .35f, .85f);
                if (combat.InArc(Broadside.Port, aim)) combat.TryFire(Broadside.Port, aim);
                if (combat.InArc(Broadside.Starboard, aim)) combat.TryFire(Broadside.Starboard, aim);
            }
            float yaw = Vector3.SignedAngle(FlightPlane.Flat(transform.forward), heading, Vector3.up);
            motor.SetCommand(throttle, Mathf.Clamp(yaw / 35f, -1f, 1f), false);
            ChooseRepair();
        }

        private Vector3 InterceptPoint(Vector3 targetPosition, Vector3 relativeVelocity)
        {
            Vector3 offset = FlightPlane.Flat(targetPosition - transform.position);
            relativeVelocity = FlightPlane.Flat(relativeVelocity);
            float speed = BroadsideCombat.ShotSpeed;
            float a = relativeVelocity.sqrMagnitude - speed * speed;
            float b = 2f * Vector3.Dot(offset, relativeVelocity);
            float c = offset.sqrMagnitude;
            float time = offset.magnitude / speed;
            if (Mathf.Abs(a) < .0001f)
            {
                if (Mathf.Abs(b) > .0001f && -c / b > 0f) time = -c / b;
            }
            else
            {
                float discriminant = b * b - 4f * a * c;
                if (discriminant >= 0f)
                {
                    float root = Mathf.Sqrt(discriminant);
                    float t1 = (-b - root) / (2f * a);
                    float t2 = (-b + root) / (2f * a);
                    if (t1 > 0f && t2 > 0f) time = Mathf.Min(t1, t2);
                    else if (t1 > 0f) time = t1;
                    else if (t2 > 0f) time = t2;
                }
            }
            return targetPosition + relativeVelocity * Mathf.Clamp(time, 0f, 3f);
        }

        private void ChooseRepair()
        {
            if (Time.time < repairDecisionAt || vitals.RepairBudget <= 0f || vitals.RepairParts <= 0f) return;
            repairDecisionAt = Time.time + 3f;
            ShipModule wanted = ShipModule.Hull;
            if (vitals.Integrity(ShipModule.Engine) < .45f) wanted = ShipModule.Engine;
            else if (vitals.Integrity(ShipModule.Hull) > .6f)
            {
                if (vitals.Integrity(ShipModule.PortGuns) < .5f) wanted = ShipModule.PortGuns;
                else if (vitals.Integrity(ShipModule.StarboardGuns) < .5f) wanted = ShipModule.StarboardGuns;
            }
            if (!vitals.IsRepairing || wanted != vitals.RepairTarget) vitals.StartRepair(wanted);
        }
    }
}
