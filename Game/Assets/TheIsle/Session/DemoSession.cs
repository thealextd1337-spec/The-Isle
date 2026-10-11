using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheIsle
{
    /// <summary>The voyage economy and contextual interactions; mission freight cannot be sold.</summary>
    public sealed class DemoSession : MonoBehaviour
    {
        const float DockRange = 22f;
        const float DockSpeed = 3f;
        static readonly string[] Compass = { "Nord", "Nordost", "Ost", "Südost", "Süd", "Südwest", "West", "Nordwest" };
        readonly int[] goods = new int[3];
        readonly bool[] upgrades = new bool[3];
        ShipMotor player;
        ShipVitals vitals;
        CrewRoster crew;
        WorldMap world;
        int missionParts;
        bool initialized;
        bool salvaged;

        public int Credits { get; private set; }
        public int CargoCount => missionParts + goods[0] + goods[1] + goods[2];
        public int CargoCapacity => upgrades[(int)UpgradeKind.Cargo] ? 14 : 8;
        public bool HasWinch => upgrades[(int)UpgradeKind.Winch];
        public bool IsDuel { get; private set; }
        public string Objective
        {
            get
            {
                if (IsDuel) return "Breitseite manövrieren, feuern oder zurückziehen. Neue Reise startet das Abenteuer.";
                if (DemoComplete) return "Reise geschafft. Handel und Erkundung gehen weiter.";
                if (DeliveryComplete)
                    return "Zur oberen Werft und ein Modul ausbauen." + RouteHint(player.Layer == 1 ? 6 : 0, true);
                return "Liefere 4 Auftragsteile zum unteren Hafen." + (player.Layer == 0 ? RouteHint(5, false) : " Erkunde die untere Wolkenwelt.");
            }
        }
        public string Message { get; private set; }
        public bool DeliveryComplete { get; private set; }
        public bool DemoComplete { get; private set; }
        public WorldSite DockedPort { get; private set; }
        public bool IsDocked => DockedPort != null;
        public bool CanDock => initialized && !IsDuel && !vitals.IsDestroyed && player.Speed <= DockSpeed && NearbyPort() != null;
        public event Action NewVoyageRequested;
        public event Action DuelRequested;
        public event Action<string> MessageChanged;

        public void Initialize(ShipMotor ship, ShipVitals health, CrewRoster roster, WorldMap map, BroadsideCombat combat)
        {
            if (world != null) world.Notice -= Notify;
            player = ship;
            vitals = health;
            crew = roster;
            world = map;
            Credits = 500;
            missionParts = 4;
            Array.Clear(goods, 0, goods.Length);
            Array.Clear(upgrades, 0, upgrades.Length);
            DeliveryComplete = false;
            DemoComplete = false;
            IsDuel = false;
            DockedPort = null;
            salvaged = false;
            initialized = true;
            world.Notice += Notify;
            RefreshCargo();
            Notify("WASD fliegt · Leertaste bremst · E interagiert. Hilfe oben rechts.");
        }

        void OnDestroy()
        {
            if (world != null) world.Notice -= Notify;
        }

        void Update()
        {
            if (Time.timeScale <= 0 || !initialized || vitals.IsDestroyed) return;
            if (IsDocked) player.Stop();
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                Interact();
        }

        public int GoodsCount(GoodsKind kind) => goods[(int)kind];
        public bool HasUpgrade(UpgradeKind kind) => upgrades[(int)kind];
        public int MissionParts => missionParts;
        string RouteHint(int siteId, bool distance)
        {
            if (world == null || player == null) return string.Empty;
            foreach (var site in world.Sites)
            {
                if (site.Id != siteId) continue;
                Vector3 delta = FlightPlane.Flat(site.Position - player.transform.position);
                int compassIndex = (Mathf.RoundToInt(Mathf.Atan2(delta.x, delta.z) * Mathf.Rad2Deg / 45f) + 8) % 8;
                string label = site.IsGate ? "Passage" : "Werft";
                return " " + label + ": " + Compass[compassIndex] + (distance ? ", " + Mathf.RoundToInt(delta.magnitude) + " m" : " (Hinweis vom Hafen)") + ".";
            }
            return string.Empty;
        }
        public void SetDuelMode()
        {
            IsDuel = true;
            missionParts = 0;
            RefreshCargo();
            Notify("Übungsduell: gleiche Schiffsklasse. Q / R für Breitseiten; nachladen, manövrieren und Schäden reparieren.");
        }
        public string ContextHint
        {
            get
            {
                if (!initialized) return string.Empty;
                if (vitals.IsDestroyed) return "Schiff zerstört · Neue Reise oder Duell starten";
                if (IsDocked) return "E · Ablegen";
                var port = NearbyPort();
                if (port != null) return player.Speed <= DockSpeed ? "E · Anlegen bei " + port.Name : "Leertaste · Zum Anlegen abbremsen";
                var site = world.NearestSite(22f);
                if (site != null && site.IsGate) return site.Blocked ? "Durchgang vom Sturm gesperrt" : "E · Ebene wechseln";
                if (site != null && site.IsSalvage) return salvaged ? "Wrack geborgen" : HasWinch ? "E · Wrack bergen" : "Wrack · Benötigt eine Bergungswinde";
                return "Q / R · Backbord / Steuerbord feuern";
            }
        }

        void Interact()
        {
            if (IsDocked || NearbyPort() != null) { ToggleDock(); return; }
            var site = world.NearestSite(22f);
            if (site != null && site.IsGate)
            {
                if (world.TryTransition()) Notify(player.Layer == 0 ? "Obere Wolkenwelt erreicht." : "Untere Wolkenwelt erreicht. Vorsicht: Piratengebiet.");
                else Notify(site.Blocked ? "Der Sturm blockiert diesen Durchgang. Suche die zweite Route." : "Näher an den Durchgang fliegen und abbremsen.");
                return;
            }
            if (site != null && site.IsSalvage) Salvage();
        }

        WorldSite NearbyPort()
        {
            if (IsDuel || world == null || player == null) return null;
            WorldSite nearest = null;
            float distance = DockRange;
            foreach (var site in world.Sites)
            {
                if (!site.IsPort || site.Layer != player.Layer || !site.Discovered) continue;
                float candidate = FlightPlane.Distance(player.transform.position, site.Position);
                if (candidate <= distance) { nearest = site; distance = candidate; }
            }
            return nearest;
        }

        public void ToggleDock()
        {
            if (!initialized || vitals.IsDestroyed) return;
            if (IsDocked)
            {
                DockedPort = null;
                player.ControlsEnabled = true;
                Notify("Leinen los. Gute Fahrt!");
                return;
            }
            var port = NearbyPort();
            if (port == null) { Notify("Kein entdeckter Hafen in Anlegereichweite."); return; }
            if (player.Speed > DockSpeed) { Notify("Zum Anlegen auf höchstens 3 m/s abbremsen."); return; }
            DockedPort = port;
            if (DeliveryComplete && port.Layer == 0 &&
                (HasUpgrade(UpgradeKind.Cargo) || HasUpgrade(UpgradeKind.Engine) || HasUpgrade(UpgradeKind.Winch)))
                DemoComplete = true;
            player.ControlsEnabled = false;
            player.Stop();
            Notify("Angelegt bei " + port.Name + ". Handel, Werkstatt und Auftrag sind verfügbar.");
        }

        public int BuyPrice(GoodsKind kind)
        {
            bool lower = IsDocked && DockedPort.Layer == 1;
            switch (kind)
            {
                case GoodsKind.Parts: return lower ? (world.StormActive ? 64 : 54) : 32;
                case GoodsKind.Supplies: return lower ? 18 : 26;
                default: return lower ? 36 : 60;
            }
        }
        public int SellPrice(GoodsKind kind) => Mathf.Max(1, Mathf.FloorToInt(BuyPrice(kind) * .8f));
        public int UpgradePrice(UpgradeKind kind) => kind == UpgradeKind.Cargo ? 220 : kind == UpgradeKind.Engine ? 300 : 180;

        public bool Buy(GoodsKind kind)
        {
            if (!RequirePort()) return false;
            if (CargoCount >= CargoCapacity) { Notify("Laderaum voll. Verkaufe Waren oder baue ihn aus."); return false; }
            int price = BuyPrice(kind);
            if (Credits < price) { Notify("Nicht genug Kredit für diese Ware."); return false; }
            Credits -= price;
            goods[(int)kind]++;
            RefreshCargo();
            Notify(GoodsName(kind) + " gekauft (" + price + " Kredit).");
            return true;
        }

        public bool Sell(GoodsKind kind)
        {
            if (!RequirePort()) return false;
            if (goods[(int)kind] <= 0) { Notify("Keine handelbare Ware vorhanden. Auftragsteile bleiben geschützt."); return false; }
            goods[(int)kind]--;
            int price = SellPrice(kind);
            Credits += price;
            RefreshCargo();
            Notify(GoodsName(kind) + " verkauft (+" + price + " Kredit).");
            return true;
        }

        public bool Deliver()
        {
            if (!RequirePort()) return false;
            if (DeliveryComplete) { Notify("Der Lieferauftrag ist bereits abgeschlossen."); return false; }
            if (DockedPort.Layer != 1) { Notify("Der Lieferhafen liegt auf der unteren Ebene."); return false; }
            if (missionParts != 4) { Notify("Die vier Auftragsteile fehlen."); return false; }
            missionParts = 0;
            Credits += 350;
            DeliveryComplete = true;
            RefreshCargo();
            world.TriggerStorm();
            Notify("Auftrag erfüllt: +350 Kredit. Ein Sturm sperrt die Hauptroute! Finde den zweiten Durchgang zurück zur Werft.");
            return true;
        }

        public bool PurchaseUpgrade(UpgradeKind kind)
        {
            if (!RequirePort()) return false;
            if (DockedPort.Layer != 0) { Notify("Schiffsausbau gibt es an der oberen Werft."); return false; }
            if (HasUpgrade(kind)) { Notify("Dieses Modul ist schon eingebaut."); return false; }
            int price = UpgradePrice(kind);
            if (Credits < price) { Notify("Nicht genug Kredit für den Ausbau."); return false; }
            Credits -= price;
            upgrades[(int)kind] = true;
            if (kind == UpgradeKind.Engine) player.EngineUpgrade = true;
            RefreshCargo();
            if (DeliveryComplete) DemoComplete = true;
            Notify(kind == UpgradeKind.Cargo ? "Laderaum ausgebaut: 14 Plätze, zusätzlicher Rumpfballast."
                : kind == UpgradeKind.Engine ? "Stärkerer Motor eingebaut: mehr Schub, höherer Energiebedarf."
                : "Bergungswinde eingebaut: Du kannst das entdeckte Wrack bergen.");
            return true;
        }

        public int ServicePrice => Mathf.CeilToInt(60 + Mathf.Max(0, vitals.MaxHull - vitals.Hull) * .3f);
        public bool HarborService()
        {
            if (!RequirePort()) return false;
            int price = ServicePrice;
            if (Credits < price) { Notify("Für den Werftservice fehlen " + (price - Credits) + " Kredit."); return false; }
            Credits -= price;
            vitals.RestoreAtHarbor();
            player.Refuel();
            Notify("Werftservice: Schiff repariert, Energie und begrenzter Reparaturvorrat aufgefüllt (" + price + " Kredit).");
            return true;
        }

        public bool Salvage()
        {
            if (!initialized || vitals.IsDestroyed || IsDocked) return false;
            var site = world.NearestSite(22f);
            if (site == null || !site.IsSalvage) { Notify("Kein Wrack in Bergungsreichweite."); return false; }
            if (!HasWinch) { Notify("Baue zuerst eine Bergungswinde an der oberen Werft ein."); return false; }
            if (salvaged) { Notify("Dieses Wrack wurde bereits geborgen."); return false; }
            if (player.Speed > DockSpeed) { Notify("Zur Bergung zuerst abbremsen."); return false; }
            if (CargoCapacity - CargoCount < 2) { Notify("Für die Bergung werden zwei freie Ladeplätze benötigt."); return false; }
            salvaged = true;
            goods[(int)GoodsKind.Crystal] += 2;
            vitals.AddRepairParts(1f);
            RefreshCargo();
            Notify("Wrack geborgen: 2 Kristalle und 1 Reparaturteil gefunden.");
            return true;
        }

        bool RequirePort()
        {
            if (initialized && IsDocked && !vitals.IsDestroyed) return true;
            Notify("Lege zuerst an einem Hafen an.");
            return false;
        }
        void RefreshCargo() => player.CargoMass = CargoCount * 15f + (HasUpgrade(UpgradeKind.Cargo) ? 20f : 0f) + (HasWinch ? 12f : 0f);
        public static string GoodsName(GoodsKind kind) => kind == GoodsKind.Parts ? "Bauteile" : kind == GoodsKind.Supplies ? "Vorräte" : "Kristalle";
        public void Notify(string message) { Message = message; MessageChanged?.Invoke(message); }
        public void RequestNewVoyage() => NewVoyageRequested?.Invoke();
        public void RequestDuel() => DuelRequested?.Invoke();
    }
}
