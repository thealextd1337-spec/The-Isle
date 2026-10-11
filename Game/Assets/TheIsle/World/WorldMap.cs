using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheIsle
{
    public sealed class WorldMap : MonoBehaviour
    {
        readonly List<WorldSite> sites = new List<WorldSite>();
        ShipMotor player;
        CrewRoster crew;
        Vector3 prevailingWind;
        float weatherPhase;
        bool initialized;

        public int Seed { get; private set; }
        public IReadOnlyList<WorldSite> Sites => sites;
        public Vector3 Wind => WindAt(player != null ? player.transform.position : Vector3.zero, player != null ? player.Layer : 0);
        public bool StormActive { get; private set; }
        public float WindMultiplier { get; set; } = 1f;
        public float RevealRadius => 25f + 11f * (crew != null ? crew.WorkRate(CrewStation.Navigation) : 1f);
        public event Action<string> Notice;

        public void Initialize(int seed, ShipMotor playerMotor, CrewRoster roster)
        {
            Seed = seed;
            player = playerMotor;
            crew = roster;
            StormActive = false;
            WindMultiplier = 1f;
            sites.Clear();
            var random = new System.Random(seed);
            float rotation = (float)(random.NextDouble() * 150.0 - 75.0);
            Quaternion layoutRotation = Quaternion.Euler(0f, rotation, 0f);
            weatherPhase = (float)random.NextDouble() * Mathf.PI * 2f;
            prevailingWind = Quaternion.Euler(0f, (float)random.NextDouble() * 360f, 0f) * new Vector3(2.8f, 0f, 0f);

            AddSite(0, "Hafen Morgenanker", Vector3.zero, 0, true, false, false);
            AddSite(1, "Hafen Tiefenlicht", Place(160f, 190f), 1, true, false, false);
            AddSite(2, "Sonnengarten", Place(-60f, 52f), 0, false, false, false);
            AddSite(3, "Kupferkrone", Place(105f, -55f), 0, false, false, false);
            AddSite(4, "Kristallhort", Place(-95f, 210f), 1, false, false, false);
            AddSite(5, "Hauptpassage", Place(83f, 90f), -1, false, true, false);
            AddSite(6, "Windtreppe", Place(-105f, 110f), -1, false, true, false);
            AddSite(7, "Wrack der Abendstern", Place(25f, 175f), 1, false, false, true);
            sites[0].Discovered = true;
            initialized = true;
            RevealAt(player != null ? player.transform.position : Vector3.zero, 0, RevealRadius);

            Vector3 Place(float x, float z)
            {
                float jitterX = (float)(random.NextDouble() * 20.0 - 10.0);
                float jitterZ = (float)(random.NextDouble() * 20.0 - 10.0);
                return layoutRotation * new Vector3(x + jitterX, 0f, z + jitterZ);
            }
        }

        void AddSite(int id, string name, Vector3 position, int layer, bool port, bool gate, bool salvage)
        {
            sites.Add(new WorldSite
            {
                Id = id, Name = name, Position = position, Layer = layer,
                IsPort = port, IsGate = gate, IsSalvage = salvage
            });
        }

        void Update()
        {
            if (initialized && player != null)
                RevealAt(player.transform.position, player.Layer, RevealRadius);
        }

        public WorldSite NearestSite(float range)
        {
            if (player == null || range < 0f) return null;
            WorldSite nearest = null;
            float distance = range;
            foreach (WorldSite site in sites)
            {
                if (site.Layer != -1 && site.Layer != player.Layer) continue;
                float candidate = FlightPlane.Distance(player.transform.position, site.Position);
                if (candidate <= distance)
                {
                    distance = candidate;
                    nearest = site;
                }
            }
            return nearest;
        }

        public Vector3 WindAt(Vector3 position, int layer)
        {
            if (!initialized || WindMultiplier <= 0f) return Vector3.zero;
            float gust = 1f + Mathf.Sin(Time.time * 0.17f + weatherPhase + position.x * 0.006f) * 0.17f;
            Vector3 wind = prevailingWind * (layer == 1 ? 1.35f : 1f) * gust * (StormActive ? 1.85f : 1f) * WindMultiplier;
            if (layer == 1) wind = Quaternion.Euler(0f, 28f, 0f) * wind;
            Vector3 direction = wind.normalized;
            float shadow = 1f;
            foreach (WorldSite site in sites)
            {
                if (site.IsGate || site.IsSalvage || site.Layer != layer) continue;
                Vector3 offset = FlightPlane.Flat(position - site.Position);
                float downwind = Vector3.Dot(offset, direction);
                if (downwind < 8f || downwind > 70f) continue;
                float lateral = (offset - direction * downwind).magnitude;
                float width = 17f + downwind * 0.18f;
                if (lateral >= width) continue;
                float strength = (1f - lateral / width) * (1f - downwind / 85f);
                shadow = Mathf.Min(shadow, 1f - strength * 0.8f);
            }
            return wind * shadow;
        }

        public bool TryTransition()
        {
            if (player == null) return false;
            ShipVitals vitals = player.GetComponent<ShipVitals>();
            if (vitals != null && vitals.IsDestroyed) return false;
            WorldSite gate = null;
            float nearest = 22f;
            foreach (WorldSite site in sites)
            {
                if (!site.IsGate) continue;
                float distance = FlightPlane.Distance(player.transform.position, site.Position);
                if (distance <= nearest) { gate = site; nearest = distance; }
            }
            if (gate == null)
            {
                Notice?.Invoke("Ebenenwechsel ist nur an einer Passage möglich.");
                return false;
            }
            if (gate.Blocked)
            {
                Notice?.Invoke("Der Sturm sperrt diese Passage. Suche die Windtreppe.");
                return false;
            }
            gate.Discovered = true;
            player.SetLayer(1 - player.Layer);
            RevealAt(player.transform.position, player.Layer, RevealRadius);
            Notice?.Invoke(player.Layer == 0 ? "Obere Wolkenebene erreicht." : "Untere Wolkenebene erreicht: stärkerer Wind und seltene Rohstoffe.");
            return true;
        }

        public void TriggerStorm()
        {
            if (StormActive) return;
            StormActive = true;
            foreach (WorldSite site in sites)
                if (site.IsGate && site.Id == 5) site.Blocked = true;
            // A route clue prevents an unlucky fog configuration from stranding the delivery voyage.
            WorldSite alternate = sites.Find(site => site.Id == 6);
            if (alternate != null) alternate.Discovered = true;
            Notice?.Invoke("Sturmwarnung: Hauptpassage gesperrt! Die Windtreppe bleibt offen und wurde auf der Karte markiert.");
        }

        public void RevealAt(Vector3 position, int layer, float radius)
        {
            if (radius < 0f) return;
            foreach (WorldSite site in sites)
            {
                if (site.Discovered || (site.Layer != -1 && site.Layer != layer)) continue;
                if (FlightPlane.Distance(position, site.Position) > radius) continue;
                site.Discovered = true;
                Notice?.Invoke("Entdeckt: " + site.Name);
            }
        }
    }
}
