using UnityEngine;
using UnityEngine.InputSystem;

namespace TheIsle
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class ShipMotor : MonoBehaviour
    {
        const float BaseMass = 100f;
        Rigidbody body;
        ShipVitals vitals;
        CrewRoster crew;
        WorldMap world;
        bool isPlayer;
        bool initialized;
        float turnCommand;
        bool brakeCommand;
        float cargoMass;

        public float Energy { get; private set; } = 100f;
        public float Speed => Velocity.magnitude;
        public float AngularSpeedDegrees => body != null ? body.angularVelocity.y * Mathf.Rad2Deg : 0f;
        public float Throttle { get; private set; }
        public int Layer { get; private set; }
        public Vector3 Velocity => body != null ? FlightPlane.Flat(body.linearVelocity) : Vector3.zero;
        public float CargoMass
        {
            get => cargoMass;
            set
            {
                cargoMass = Mathf.Max(0f, value);
                if (body != null) body.mass = BaseMass + cargoMass;
            }
        }
        public bool EngineUpgrade { get; set; }
        public bool ControlsEnabled { get; set; } = true;

        void Awake() => ConfigureBody();

        void ConfigureBody()
        {
            body = GetComponent<Rigidbody>();
            body.useGravity = false;
            body.constraints = RigidbodyConstraints.FreezePositionY |
                               RigidbodyConstraints.FreezeRotationX |
                               RigidbodyConstraints.FreezeRotationZ;
            body.mass = BaseMass + cargoMass;
            body.linearDamping = 0f;
            body.angularDamping = 0.18f;
            body.maxAngularVelocity = 1.3f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        public void Initialize(ShipVitals shipVitals, CrewRoster roster, WorldMap map, bool player)
        {
            ConfigureBody();
            vitals = shipVitals;
            crew = roster;
            world = map;
            isPlayer = player;
            Layer = 0;
            Energy = 100f;
            EngineUpgrade = false;
            ControlsEnabled = true;
            initialized = true;
            Stop();
        }

        void Update()
        {
            if (!initialized || !isPlayer) return;
            Keyboard keyboard = Keyboard.current;
            if (!ControlsEnabled || keyboard == null || (vitals != null && vitals.IsDestroyed))
            {
                SetCommand(0f, 0f, false);
                return;
            }
            float throttle = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1f : 0f) -
                             (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1f : 0f);
            float turn = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1f : 0f) -
                         (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1f : 0f);
            SetCommand(throttle, turn, keyboard.spaceKey.isPressed);
        }

        public void SetCommand(float throttle, float turn, bool brake)
        {
            Throttle = Mathf.Clamp(throttle, -1f, 1f);
            turnCommand = Mathf.Clamp(turn, -1f, 1f);
            brakeCommand = brake;
        }

        void FixedUpdate()
        {
            if (!initialized || body == null) return;
            if (!ControlsEnabled || (vitals != null && vitals.IsDestroyed))
            {
                Stop();
                return;
            }

            float massRatio = body.mass / BaseMass;
            float engine = vitals != null ? 0.4f + 0.6f * vitals.Integrity(ShipModule.Engine) : 1f;
            float engineCrew = crew != null ? Mathf.Lerp(0.8f, 1.1f, Mathf.InverseLerp(0.4f, 1.6f, crew.WorkRate(CrewStation.Engineering))) : 1f;
            float power = engine * engineCrew * (EngineUpgrade ? 1.25f : 1f);
            float fuelPower = Energy > 0f ? 1f : 0.22f;
            float reverse = Throttle < 0f ? 0.55f : 1f;
            body.AddForce(transform.forward * (310f * Throttle * reverse * power * fuelPower), ForceMode.Force);

            Vector3 wind = world != null ? world.WindAt(body.position, Layer) : Vector3.zero;
            // Air drag acts on relative air speed, so tail/head/side wind changes the route physically.
            Vector3 relativeAirVelocity = Velocity - wind;
            body.AddForce(-relativeAirVelocity * 24f, ForceMode.Force);
            if (brakeCommand) body.AddForce(-Velocity * 100f, ForceMode.Force);

            float navigation = crew != null ? Mathf.Clamp(crew.WorkRate(CrewStation.Navigation), 0.4f, 1.6f) : 1f;
            float helm = Mathf.Lerp(0.75f, 1.12f, Mathf.InverseLerp(0.4f, 1.6f, navigation));
            float steeringPower = helm * engine / massRatio;
            float targetYawRate = brakeCommand ? 0f : turnCommand * 42f * Mathf.Deg2Rad * steeringPower;
            // The rudder follows a requested turn rate, not a heading. Bounded torque keeps real inertia.
            float response = brakeCommand ? 8f : Mathf.Abs(turnCommand) > 0.01f ? 4f : 2f;
            float maxYawAcceleration = 3.5f * steeringPower * (brakeCommand ? 2f : 1f);
            float yawAcceleration = Mathf.Clamp((targetYawRate - body.angularVelocity.y) * response,
                -maxYawAcceleration, maxYawAcceleration);
            body.AddTorque(Vector3.up * yawAcceleration, ForceMode.Acceleration);

            if (Energy > 0f)
            {
                float use = Mathf.Abs(Throttle) * 0.13f * massRatio * (EngineUpgrade ? 1.16f : 1f);
                use += Mathf.Abs(turnCommand) * 0.025f;
                Energy = Mathf.Max(0f, Energy - use * Time.fixedDeltaTime);
            }
        }

        public void SetLayer(int layer)
        {
            Layer = Mathf.Clamp(layer, 0, 1);
            Stop();
        }

        public void Refuel() => Energy = 100f;

        public void Stop()
        {
            SetCommand(0f, 0f, false);
            if (body == null) return;
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
    }
}
