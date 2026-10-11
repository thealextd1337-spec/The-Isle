using UnityEngine;

namespace TheIsle
{
    /// <summary>Six people are moved, never copied. Main officers survive until the ship is lost.</summary>
    public sealed class CrewRoster : MonoBehaviour
    {
        private sealed class Sailor
        {
            public CrewStation Station;
            public CrewStation Destination;
            public float ArrivesAt;
            public float CanMoveAt;
            public bool Travelling;
        }

        private readonly Sailor[] sailors = new Sailor[6];
        public bool LeadersAlive { get; private set; } = true;
        public int InTransit
        {
            get
            {
                int count = 0;
                foreach (Sailor sailor in sailors)
                    if (sailor != null && sailor.Travelling) count++;
                return count;
            }
        }

        public void Initialize()
        {
            LeadersAlive = true;
            for (int i = 0; i < sailors.Length; i++)
                sailors[i] = new Sailor { Station = (CrewStation)(i / 2), CanMoveAt = float.NegativeInfinity };
        }

        public int Count(CrewStation station)
        {
            int count = 0;
            foreach (Sailor sailor in sailors)
                if (sailor != null && !sailor.Travelling && sailor.Station == station) count++;
            return count;
        }

        public float WorkRate(CrewStation station) => LeadersAlive ? .4f + .3f * Count(station) : 0f;

        public bool Transfer(CrewStation from, CrewStation to)
        {
            if (!LeadersAlive || from == to || (int)from < 0 || (int)from > 2 || (int)to < 0 || (int)to > 2)
                return false;
            int reserved = Count(to);
            foreach (Sailor sailor in sailors)
                if (sailor != null && sailor.Travelling && sailor.Destination == to) reserved++;
            if (reserved >= 4) return false;
            foreach (Sailor sailor in sailors)
            {
                if (sailor == null || sailor.Travelling || sailor.Station != from || Time.time < sailor.CanMoveAt)
                    continue;
                sailor.Travelling = true;
                sailor.Destination = to;
                sailor.ArrivesAt = Time.time + 5f;
                sailor.CanMoveAt = Time.time + 10f;
                return true;
            }
            return false;
        }

        private void Update()
        {
            foreach (Sailor sailor in sailors)
            {
                if (sailor == null || !sailor.Travelling || Time.time < sailor.ArrivesAt) continue;
                sailor.Station = sailor.Destination;
                sailor.Travelling = false;
            }
        }

        public void MarkShipDestroyed() => LeadersAlive = false;

        public string Ability(CrewStation station)
        {
            switch (station)
            {
                case CrewStation.Navigation: return "Navigatorin: Wolken lesen und Inseln aufspüren";
                case CrewStation.Engineering: return "Maschinist: Triebwerk pflegen und Feldreparaturen leiten";
                default: return "Geschützmeisterin: Breitseiten koordinieren und Kanonen nachladen";
            }
        }
    }
}
