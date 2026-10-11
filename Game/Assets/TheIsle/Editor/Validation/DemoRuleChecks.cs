using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TheIsle.Editor
{
    /// <summary>Synchronous public-API checks, runnable without entering Play mode.</summary>
    public static class DemoRuleChecks
    {
        [Serializable]
        sealed class Report
        {
            public string checkedAtUtc;
            public bool succeeded;
            public int assertions;
            public int seedsChecked;
            public List<string> passed = new List<string>();
            public List<string> limitations = new List<string>();
            public string error;
        }

        readonly struct SiteSnapshot
        {
            public readonly int Id;
            public readonly Vector3 Position;
            public readonly int Layer;
            public readonly bool IsPort;
            public readonly bool IsGate;
            public readonly bool IsSalvage;

            public SiteSnapshot(WorldSite site)
            {
                Id = site.Id;
                Position = site.Position;
                Layer = site.Layer;
                IsPort = site.IsPort;
                IsGate = site.IsGate;
                IsSalvage = site.IsSalvage;
            }
        }

        public static string Run()
        {
            var report = new Report { checkedAtUtc = DateTime.UtcNow.ToString("O") };
            report.limitations.Add("Synchronous checks do not advance Unity time or physics.");
            report.limitations.Add("Crew arrival/cooldown, repair-budget depletion, reload sharing, AI, projectile impacts and flight tuning require Play-mode validation.");
            report.limitations.Add("Repair switching checks that selecting targets preserves the current budget and parts; it does not claim to exhaust the budget over time.");
            GameObject isolated = null;
            try
            {
                isolated = new GameObject("__TheIsleRuleChecks") { hideFlags = HideFlags.HideAndDontSave };
                var crew = isolated.AddComponent<CrewRoster>();
                var vitals = isolated.AddComponent<ShipVitals>();
                var motor = isolated.AddComponent<ShipMotor>();
                var world = isolated.AddComponent<WorldMap>();
                var combat = isolated.AddComponent<BroadsideCombat>();
                crew.enabled = vitals.enabled = motor.enabled = world.enabled = combat.enabled = false;
                crew.Initialize();
                vitals.Initialize(crew, ShipTeam.Player);
                motor.Initialize(vitals, crew, world, false);
                combat.Initialize(motor, vitals, crew);

                CheckWorldSeeds(report, world, motor, crew);
                CheckWorldInteraction(report, world, motor, crew);
                CheckCrew(report, crew);
                CheckVitals(report, crew, vitals);
                CheckArcs(report, combat);
                report.succeeded = true;
                return JsonUtility.ToJson(report, true);
            }
            catch (Exception exception)
            {
                report.error = exception.Message;
                throw new InvalidOperationException("The Isle rule checks failed: " + exception.Message, exception);
            }
            finally
            {
                if (isolated != null) UnityEngine.Object.DestroyImmediate(isolated);
                string destination = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", "artifacts", "rule-checks.json"));
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
                File.WriteAllText(destination, JsonUtility.ToJson(report, true));
            }
        }

        static void CheckWorldSeeds(Report report, WorldMap world, ShipMotor motor, CrewRoster crew)
        {
            SiteSnapshot[] previous = null;
            for (int seed = 0; seed < 200; seed++)
            {
                motor.transform.position = Vector3.zero;
                motor.SetLayer(0);
                world.Initialize(seed, motor, crew);
                Require(report, world.Seed == seed, "World seed is retained.");
                Require(report, world.Sites.Count == 8, "Five islands, two gates and one wreck must exist.");
                int islands = 0, gates = 0, wrecks = 0, upperPorts = 0, lowerPorts = 0;
                foreach (WorldSite site in world.Sites)
                {
                    if (site.IsGate)
                    {
                        gates++;
                        Require(report, site.Layer == -1 && !site.Blocked, "Both gates must initially connect both layers.");
                    }
                    else if (site.IsSalvage) wrecks++;
                    else islands++;
                    if (site.IsPort && site.Layer == 0) upperPorts++;
                    if (site.IsPort && site.Layer == 1) lowerPorts++;
                }
                Require(report, islands == 5 && gates == 2 && wrecks == 1, "Site roles match the demo scope.");
                Require(report, upperPorts == 1 && lowerPorts == 1, "Each layer has its harbor.");
                for (int i = 0; i < world.Sites.Count; i++)
                    for (int j = i + 1; j < world.Sites.Count; j++)
                        Require(report, FlightPlane.Distance(world.Sites[i].Position, world.Sites[j].Position) > 40f,
                            "World sites must retain maneuvering room; seed " + seed + ", sites " + i + "/" + j + ".");

                SiteSnapshot[] first = Snapshot(world);
                world.Initialize(seed, motor, crew);
                SiteSnapshot[] second = Snapshot(world);
                Require(report, SameLayout(first, second), "Repeating a seed must reproduce the layout.");
                if (previous != null)
                    Require(report, !SameLayout(previous, second), "Adjacent seeds must generate distinct layouts.");
                previous = second;

                world.TriggerStorm();
                Require(report, world.StormActive, "Storm activation must be retained.");
                Require(report, Find(world, 5).Blocked, "The storm blocks the main passage.");
                WorldSite alternate = Find(world, 6);
                Require(report, alternate.IsGate && !alternate.Blocked && alternate.Discovered,
                    "Storm leaves a known alternate passage available.");
                world.TriggerStorm();
                Require(report, !alternate.Blocked, "Repeated storm activation cannot close the alternate.");
                report.seedsChecked++;
            }
            report.passed.Add("200 seeds: repeatability, distinct layouts, minimum spacing, five islands, ports on both layers, two shared gates and storm-safe alternate.");
        }

        static void CheckWorldInteraction(Report report, WorldMap world, ShipMotor motor, CrewRoster crew)
        {
            motor.transform.position = Vector3.zero;
            motor.SetLayer(0);
            world.Initialize(37, motor, crew);
            Require(report, !world.StormActive && !Find(world, 5).Blocked, "New voyages clear storm state.");
            motor.transform.position = new Vector3(10000f, 0f, 10000f);
            Require(report, !world.TryTransition() && motor.Layer == 0, "Open air cannot change the layer.");
            WorldSite main = Find(world, 5);
            motor.transform.position = main.Position + Vector3.right * 22.1f;
            Require(report, !world.TryTransition(), "The passage cannot be used outside its interaction radius.");
            motor.transform.position = main.Position + Vector3.right * 21.9f;
            Require(report, world.TryTransition() && motor.Layer == 1, "Nearby main passage reaches the lower layer.");
            Require(report, world.TryTransition() && motor.Layer == 0, "Shared passages work in both directions.");
            world.TriggerStorm();
            motor.transform.position = main.Position;
            Require(report, !world.TryTransition() && motor.Layer == 0, "Blocked passage cannot change the layer.");
            motor.transform.position = Find(world, 6).Position;
            Require(report, world.TryTransition() && motor.Layer == 1, "Alternate passage remains usable during the storm.");
            Require(report, world.TryTransition() && motor.Layer == 0, "The alternate also provides the return route.");
            report.passed.Add("Transitions require an open nearby gate; storm-blocked main gate and alternate return route behave correctly.");

            motor.transform.position = Vector3.zero;
            world.Initialize(37, motor, crew);
            WorldSite lowerPort = Find(world, 1);
            Require(report, !lowerPort.Discovered, "Distant lower harbor starts hidden.");
            world.RevealAt(lowerPort.Position, 0, 1f);
            Require(report, !lowerPort.Discovered, "Upper-layer scans cannot discover lower-layer islands.");
            world.RevealAt(lowerPort.Position, 1, 1f);
            Require(report, lowerPort.Discovered, "Matching-layer proximity discovers the harbor.");
            motor.transform.position = lowerPort.Position;
            Require(report, world.NearestSite(2f) == null, "Nearest-site lookup excludes islands on another layer.");
            motor.SetLayer(1);
            Require(report, world.NearestSite(2f) == lowerPort, "Nearest-site lookup finds the matching-layer harbor.");
            world.WindMultiplier = 0f;
            Require(report, world.WindAt(motor.transform.position, 1) == Vector3.zero, "Calm duel mode suppresses wind.");
            report.passed.Add("Fog and nearby-site lookup respect layers; calm duel wind override works.");
        }

        static void CheckCrew(Report report, CrewRoster crew)
        {
            crew.Initialize();
            Require(report, crew.Count(CrewStation.Navigation) == 2 && crew.Count(CrewStation.Engineering) == 2 &&
                crew.Count(CrewStation.Gunnery) == 2 && crew.InTransit == 0, "The initial crew consists of exactly six stationed sailors.");
            Require(report, Mathf.Approximately(crew.WorkRate(CrewStation.Navigation), 1f), "Two sailors produce normal work rate.");
            Require(report, !crew.Transfer(CrewStation.Navigation, CrewStation.Navigation), "Self-transfer must be rejected.");
            Require(report, !crew.Transfer((CrewStation)(-1), CrewStation.Engineering), "Invalid source station must be rejected.");
            Require(report, !crew.Transfer(CrewStation.Engineering, (CrewStation)3), "Invalid destination station must be rejected.");
            Require(report, crew.Transfer(CrewStation.Navigation, CrewStation.Engineering), "First engineering transfer is accepted.");
            Require(report, crew.Transfer(CrewStation.Navigation, CrewStation.Engineering), "Second engineering transfer is accepted.");
            Require(report, crew.Count(CrewStation.Navigation) == 0 && crew.Count(CrewStation.Engineering) == 2 && crew.InTransit == 2,
                "Traveling sailors work at neither station and do not arrive immediately.");
            Require(report, Mathf.Approximately(crew.WorkRate(CrewStation.Navigation), .4f), "Officer-only station retains its baseline work rate.");
            Require(report, !crew.Transfer(CrewStation.Gunnery, CrewStation.Engineering), "Four reserved engineering slots forbid another incoming sailor.");
            Require(report, !crew.Transfer(CrewStation.Navigation, CrewStation.Gunnery), "A depleted station cannot duplicate traveling sailors.");
            for (int attempt = 0; attempt < 30; attempt++)
            {
                crew.Transfer((CrewStation)(attempt % 3), (CrewStation)((attempt + 1) % 3));
                int total = crew.InTransit;
                foreach (CrewStation station in Enum.GetValues(typeof(CrewStation)))
                {
                    int count = crew.Count(station);
                    Require(report, count >= 0 && count <= 4, "Station workforce stays within its capacity.");
                    total += count;
                }
                Require(report, total == 6, "Transfers conserve all six sailors.");
            }
            report.passed.Add("Crew initial population, transfer exclusion, reserved capacity, invalid transfers and closed population remain valid.");
        }

        static void CheckVitals(Report report, CrewRoster crew, ShipVitals vitals)
        {
            crew.Initialize();
            vitals.Initialize(crew, ShipTeam.Player);
            vitals.Damage(8f, ShipModule.Engine);
            Require(report, Mathf.Approximately(vitals.Hull, vitals.MaxHull - 8f), "Module hits apply hull damage once.");
            Require(report, Mathf.Approximately(vitals.Integrity(ShipModule.Engine), .96f), "Eight hull damage also applies four component damage.");
            vitals.Damage(192f, ShipModule.Engine);
            Require(report, vitals.Integrity(ShipModule.Engine) == 0f && vitals.Hull > 0f && crew.LeadersAlive,
                "A destroyed engine cannot kill the officers of a surviving ship.");
            int destructionEvents = 0;
            Action destroyed = () => destructionEvents++;
            vitals.Destroyed += destroyed;
            try
            {
                vitals.Damage(vitals.Hull);
                Require(report, vitals.IsDestroyed && !crew.LeadersAlive && destructionEvents == 1,
                    "Only complete ship destruction loses the officers.");
                vitals.Damage(100f);
                vitals.RestoreAtHarbor();
                Require(report, vitals.Hull == 0f && vitals.IsDestroyed && !crew.LeadersAlive && destructionEvents == 1,
                    "Repeated damage and harbor service cannot resurrect a destroyed ship or repeat its destruction event.");
                Require(report, !vitals.StartRepair(ShipModule.Hull), "Destroyed ships cannot start field repairs.");
            }
            finally { vitals.Destroyed -= destroyed; }
            report.passed.Add("Component hits preserve officers until full destruction; destroyed ships cannot resurrect at a harbor.");

            crew.Initialize();
            vitals.Initialize(crew, ShipTeam.Player);
            vitals.Damage(40f, ShipModule.Engine);
            vitals.Damage(40f, ShipModule.PortGuns);
            float budget = vitals.RepairBudget;
            float parts = vitals.RepairParts;
            for (int i = 0; i < 200; i++)
            {
                ShipModule target = i % 3 == 0 ? ShipModule.Hull : i % 3 == 1 ? ShipModule.Engine : ShipModule.PortGuns;
                Require(report, vitals.StartRepair(target) && vitals.RepairTarget == target, "Damaged repair targets may be selected.");
                Require(report, Mathf.Approximately(vitals.RepairBudget, budget) && Mathf.Approximately(vitals.RepairParts, parts),
                    "Selecting another repair target cannot refill budget or parts.");
                vitals.StopRepair();
                Require(report, !vitals.IsRepairing, "Stopping repair clears its active state.");
            }
            vitals.RestoreAtHarbor();
            Require(report, Mathf.Approximately(vitals.Hull, vitals.MaxHull) && vitals.Integrity(ShipModule.Engine) == 1f,
                "Harbor repair restores surviving ships.");
            Require(report, !vitals.StartRepair(ShipModule.Hull), "Full hull cannot consume field repair work.");
            report.passed.Add("200 repair-target switches preserve current budget and parts; repair stop and surviving harbor restore work.");
        }

        static void CheckArcs(Report report, BroadsideCombat combat)
        {
            combat.transform.position = new Vector3(17f, 0f, -34f);
            foreach (float heading in new[] { 0f, 37f, 179f })
            {
                combat.transform.rotation = Quaternion.Euler(0f, heading, 0f);
                foreach (Broadside side in Enum.GetValues(typeof(Broadside)))
                {
                    Vector3 center = combat.transform.right * (side == Broadside.Port ? -1f : 1f);
                    Require(report, combat.InArc(side, combat.transform.position + center * 50f), "Bank center belongs to the firing arc.");
                    foreach (float angle in new[] { -29.99f, 29.99f })
                        Require(report, combat.InArc(side, combat.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * center * 50f),
                            "The firing arc includes angles immediately inside plus/minus thirty degrees.");
                    foreach (float angle in new[] { -30.01f, 30.01f })
                        Require(report, !combat.InArc(side, combat.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * center * 50f),
                            "The firing arc excludes angles immediately outside plus/minus thirty degrees.");
                    Require(report, !combat.InArc(side, combat.transform.position + combat.transform.forward * 50f), "The bow is outside a broadside firing arc.");
                    Require(report, !combat.InArc(side, combat.transform.position - combat.transform.forward * 50f), "The stern is outside a broadside firing arc.");
                    Require(report, !combat.InArc(side, combat.transform.position - center * 50f), "The opposite bank is outside this bank's arc.");
                    Require(report, !combat.InArc(side, combat.transform.position), "A coincident target has no firing direction.");
                }
            }
            report.passed.Add("Both broadside arcs respect plus/minus thirty degrees, bow/stern blind zones and ship rotation.");
        }

        static SiteSnapshot[] Snapshot(WorldMap world)
        {
            var snapshot = new SiteSnapshot[world.Sites.Count];
            for (int i = 0; i < snapshot.Length; i++) snapshot[i] = new SiteSnapshot(world.Sites[i]);
            return snapshot;
        }

        static bool SameLayout(SiteSnapshot[] a, SiteSnapshot[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i].Id != b[i].Id || a[i].Layer != b[i].Layer || a[i].IsPort != b[i].IsPort ||
                    a[i].IsGate != b[i].IsGate || a[i].IsSalvage != b[i].IsSalvage ||
                    (a[i].Position - b[i].Position).sqrMagnitude > .000001f) return false;
            return true;
        }

        static WorldSite Find(WorldMap world, int id)
        {
            foreach (WorldSite site in world.Sites) if (site.Id == id) return site;
            throw new InvalidOperationException("Missing world site " + id + ".");
        }

        static void Require(Report report, bool condition, string message)
        {
            report.assertions++;
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
