using UnityEngine;

namespace TheIsle
{
    public enum ShipModule { Hull, Engine, PortGuns, StarboardGuns }
    public enum CrewStation { Navigation, Engineering, Gunnery }
    public enum Broadside { Port, Starboard }
    public enum ShipTeam { Player, Pirate }
    public enum UpgradeKind { Cargo, Engine, Winch }
    public enum GoodsKind { Parts, Supplies, Crystal }

    public static class FlightPlane
    {
        public static Vector3 Flat(Vector3 value) => new Vector3(value.x, 0, value.z);
        public static float Distance(Vector3 a, Vector3 b) => Flat(a - b).magnitude;
    }
}
