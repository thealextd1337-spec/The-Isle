using UnityEngine;

namespace TheIsle
{
    public sealed class WorldSite
    {
        public string Name;
        public Vector3 Position;
        // -1 means one shared gate exists at this position on both layers.
        public int Layer;
        public bool IsPort;
        public bool IsGate;
        public bool IsSalvage;
        public bool Discovered;
        public bool Blocked;
        public int Id;
    }
}
