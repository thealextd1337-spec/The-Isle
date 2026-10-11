using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace TheIsle
{
    /// <summary>Runtime uGUI dashboard. Chart markers exist only for discovered sites.</summary>
    public sealed class DemoHud : MonoBehaviour
    {
        static readonly Color Cream = new Color(.96f, .91f, .79f);
        static readonly Color Gold = new Color(.94f, .73f, .39f);
        static readonly Color Teal = new Color(.12f, .24f, .26f, .94f);
        static readonly Color Muted = new Color(.65f, .78f, .75f);
        static readonly Color Red = new Color(.95f, .49f, .37f);
        readonly Dictionary<int, RectTransform> mapMarkers = new Dictionary<int, RectTransform>();
        readonly Dictionary<GoodsKind, Text> goodsLabels = new Dictionary<GoodsKind, Text>();
        readonly Dictionary<UpgradeKind, Button> upgradeButtons = new Dictionary<UpgradeKind, Button>();
        readonly Dictionary<CrewStation, Text> crewLabels = new Dictionary<CrewStation, Text>();
        readonly List<Button> portButtons = new List<Button>();
        DemoSession session;
        ShipMotor motor;
        ShipVitals vitals;
        CrewRoster crew;
        WorldMap world;
        BroadsideCombat combat;
        FlightCameraRig cameraRig;
        Font font;
        Sprite solidSprite;
        RectTransform canvasRoot;
        RectTransform chart;
        RectTransform playerMarker;
        GameObject portPanel;
        GameObject helpPanel;
        Text title;
        Text objective;
        Text values;
        Text message;
        Text context;
        Text repairInfo;
        Text crewInfo;
        Text portTitle;
        Text serviceLabel;
        Text chartCaption;
        Text danger;
        Text deliveryLabel;
        Text muteLabel;
        Button dockButton;
        Image hullBar;
        Image energyBar;
        Image portReload;
        Image starboardReload;
        float nextRefresh;
        bool initialized;
        float timeBeforeHelp=1;
        bool audioBeforeHelp;
        public bool HelpOpen => helpPanel != null && helpPanel.activeSelf;
        public event Action UiActivated;

        public void Initialize(DemoSession voyage, ShipMotor ship, ShipVitals health, CrewRoster roster, WorldMap map, BroadsideCombat guns)
        {
            session = voyage; motor = ship; vitals = health; crew = roster; world = map; combat = guns;
            cameraRig = Camera.main.GetComponent<FlightCameraRig>();
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            solidSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height), new Vector2(.5f, .5f));
            CreateCanvas();
            BuildTop();
            BuildCrew();
            BuildChart();
            BuildPort();
            BuildHelp();
            context = MakeText("Kontext", canvasRoot, new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(0, 25), new Vector2(650, 55), 18, Gold, TextAnchor.MiddleCenter);
            initialized = true;
            Refresh();
        }

        void CreateCanvas()
        {
            var canvasObject = new GameObject("Voyage HUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 1f;
            canvas.sortingOrder = 20;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600, 900);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = .5f;
            canvasRoot = canvasObject.GetComponent<RectTransform>();
            if (EventSystem.current == null)
            {
                var events = new GameObject("UI Event System", typeof(EventSystem), typeof(InputSystemUIInputModule));
                events.transform.SetParent(transform, false);
                events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
            }
        }

        void BuildTop()
        {
            var panel = Panel("Reiseübersicht", canvasRoot, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(0, -16), new Vector2(1568, 115));
            title = MakeText("Titel", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(14, -9), new Vector2(680, 24), 19, Gold);
            objective = MakeText("Auftrag", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(14, -34), new Vector2(1240, 24), 17, Cream);
            values = MakeText("Flugdaten", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(14, -62), new Vector2(1170, 22), 15, Cream);
            message = MakeText("Meldung", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(14, -88), new Vector2(1535, 20), 14, Muted);
            hullBar = Bar(panel, "Rumpf", new Vector2(1236, -40), new Vector2(145, 7), Gold);
            energyBar = Bar(panel, "Energie", new Vector2(1392, -40), new Vector2(145, 7), Muted);
            portReload = Bar(panel, "Backbord", new Vector2(1236, -64), new Vector2(145, 7), Gold);
            starboardReload = Bar(panel, "Steuerbord", new Vector2(1392, -64), new Vector2(145, 7), Gold);
            MakeText("Banken", panel, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-14, -74), new Vector2(300, 15), 11, Muted, TextAnchor.UpperRight).text = "Q Backbord                     R Steuerbord";
            MakeButton("Neue Reise", panel, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-202, -9), new Vector2(112, 26), session.RequestNewVoyage);
            MakeButton("Duell", panel, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-112, -9), new Vector2(82, 26), session.RequestDuel);
            MakeButton("Hilfe", panel, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-14, -9), new Vector2(90, 26), ToggleHelp);
            MakeText("Ton Titel", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(775, -11), new Vector2(58, 22), 13, Muted).text = "Sound";
            BuildVolumeSlider(panel, new Vector2(835, -9));
            muteLabel = MakeButton("Stumm", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(974, -9), new Vector2(112, 26), DemoAudio.ToggleMute, 13).GetComponentInChildren<Text>();
            danger = MakeText("Gefahr", canvasRoot, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(0, -145), new Vector2(780, 35), 22, Red, TextAnchor.MiddleCenter);
        }

        void BuildCrew()
        {
            var panel = Panel("Besatzung", canvasRoot, new Vector2(0, 0), new Vector2(0, 0), new Vector2(16, 16), new Vector2(346, 266));
            MakeText("Crew Titel", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(14, -12), new Vector2(316, 24), 18, Gold).text = "Besatzung & Reparatur";
            var stations = new[] { CrewStation.Navigation, CrewStation.Engineering, CrewStation.Gunnery };
            for (int index = 0; index < stations.Length; index++)
            {
                CrewStation from = stations[index];
                float y = -45 - index * 32;
                crewLabels[from] = MakeText("Crew " + from, panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(14, y), new Vector2(165, 27), 13, Cream);
                int targetIndex = 0;
                foreach (CrewStation destination in stations)
                {
                    if (destination == from) continue;
                    CrewStation to = destination;
                    MakeButton("→ " + StationShort(to), panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(177 + targetIndex * 78, y), new Vector2(72, 25), () =>
                    {
                        session.Notify(crew.Transfer(from, to) ? "Unterbesatzung wechselt Station: 5 Sekunden ohne Arbeitskraft." : "Versetzen nicht möglich: Ziel voll, keine verfügbare Person oder Wechselpause.");
                    }, 12);
                    targetIndex++;
                }
            }
            crewInfo = MakeText("Transit", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(14, -144), new Vector2(318, 20), 12, Muted);
            var modules = new[] { ShipModule.Hull, ShipModule.Engine, ShipModule.PortGuns, ShipModule.StarboardGuns };
            for (int i = 0; i < modules.Length; i++)
            {
                ShipModule target = modules[i];
                MakeButton(ModuleShort(target), panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(14 + i * 65, -174), new Vector2(59, 27), () =>
                {
                    session.Notify(vitals.StartRepair(target) ? "Feldreparatur: " + ModuleShort(target) + ". Mehr Maschinisten beschleunigen die Arbeit." : "Keine Reparatur möglich: Ziel intakt, Budget oder Teile fehlen.");
                }, 12);
            }
            MakeButton("Stopp", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(274, -174), new Vector2(59, 27), vitals.StopRepair, 12);
            repairInfo = MakeText("Reparaturvorrat", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(14, -209), new Vector2(320, 49), 12, Muted);
        }

        void BuildChart()
        {
            var panel = Panel("Seekarte", canvasRoot, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-16, -147), new Vector2(252, 255));
            MakeText("Karten Titel", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(12, -10), new Vector2(230, 22), 17, Gold).text = "Entdeckte Wolkenkarte";
            chart = Rect("Kartenfläche", panel, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(0, -39), new Vector2(226, 170));
            var backing = chart.gameObject.AddComponent<Image>();
            backing.color = new Color(.055f, .13f, .15f, .9f);
            chart.gameObject.AddComponent<RectMask2D>();
            var vertical = Rect("Meridian", chart, new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(1, 170));
            vertical.gameObject.AddComponent<Image>().color = new Color(.4f, .6f, .6f, .15f);
            var horizontal = Rect("Parallel", chart, new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(226, 1));
            horizontal.gameObject.AddComponent<Image>().color = new Color(.4f, .6f, .6f, .15f);
            playerMarker = MakeText("Spieler", chart, new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(22, 22), 21, Gold, TextAnchor.MiddleCenter).rectTransform;
            playerMarker.GetComponent<Text>().text = "^";
            chartCaption = MakeText("Kartenlegende", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(12, -215), new Vector2(230, 32), 12, Muted);
        }

        void BuildPort()
        {
            var panel = Panel("Hafen", canvasRoot, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-16, 16), new Vector2(362, 361));
            portPanel = panel.gameObject;
            portTitle = MakeText("Hafenname", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(14, -12), new Vector2(334, 25), 18, Gold);
            MakeText("Markt", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(14, -42), new Vector2(332, 20), 12, Muted).text = "Ware / an Bord          Kauf           Verkauf";
            for (int i = 0; i < 3; i++)
            {
                GoodsKind kind = (GoodsKind)i;
                float y = -66 - i * 36;
                goodsLabels[kind] = MakeText("Ware " + kind, panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(14, y), new Vector2(149, 28), 14, Cream);
                portButtons.Add(MakeButton("Kauf", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(163, y), new Vector2(84, 28), () => session.Buy(kind), 13));
                portButtons.Add(MakeButton("Verkauf", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(255, y), new Vector2(92, 28), () => session.Sell(kind), 13));
            }
            var delivery = MakeButton("Auftrag abgeben · +350", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(14, -177), new Vector2(333, 30), () => session.Deliver(), 14);
            deliveryLabel = delivery.GetComponentInChildren<Text>();
            var service = MakeButton("Werftservice", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(14, -214), new Vector2(333, 29), () => session.HarborService(), 14);
            serviceLabel = service.GetComponentInChildren<Text>();
            var names = new[] { "Laderaum", "Motor", "Winde" };
            for (int i = 0; i < 3; i++)
            {
                UpgradeKind upgrade = (UpgradeKind)i;
                upgradeButtons[upgrade] = MakeButton(names[i], panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(14 + i * 113, -252), new Vector2(107, 42), () => session.PurchaseUpgrade(upgrade), 12);
            }
            MakeText("Werfthinweis", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(14, -300), new Vector2(334, 23), 11, Muted).text = "Ausbau: obere Werft · Service füllt Teile & Energie.";
            MakeButton("E · Ablegen", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(14, -327), new Vector2(333, 25), session.ToggleDock, 13);
            portPanel.SetActive(false);
            dockButton = MakeButton("Anlegen", canvasRoot, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-16, 16), new Vector2(205, 36), session.ToggleDock, 15);
        }

        void BuildHelp()
        {
            var panel = Panel("Hilfe", canvasRoot, new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(720, 510));
            helpPanel = panel.gameObject;
            MakeText("Hilfe Titel", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(24, -22), new Vector2(650, 30), 26, Gold).text = "Dein erstes Wolkenabenteuer";
            MakeText("Hilfe Inhalt", panel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(24, -68), new Vector2(672, 375), 15, Cream).text =
                "FLIEGEN · NUR TASTATUR\nW/S oder ↑/↓: Schub · A/D oder ←/→: Drehen · Leertaste: Bremsen\nE: Hafen, Durchgang oder Wrack. Zum Anlegen auf ≤ 3 m/s bremsen.\n\nKAMERA\nBlick voraus, ruhiger Hafenanflug und automatische Kampfübersicht.\nMausrad oder +/−: eigener Zoom · C: Automatik zurücksetzen\nZ/X: Ansicht drehen · H/Esc: Hilfe und Pause · M: Ton an/aus\n\nKÄMPFEN\nQ/R: linke/rechte Breitseite · linke/rechte Maustaste: gezielt feuern\n60° zur Seite; Bug und Heck sind blind. Vorhalt und Wind nutzen,\nbeim Nachladen ausweichen oder bei Gefahr zurückziehen.\n\nBESETZUNG & REPARATUR\nGeschützleute zur Maschine versetzen: schneller reparieren, langsamer laden.\nWechsel: 5 s ohne Arbeit; erneut nach 10 s ab Start möglich.\nReparatur braucht Budget und Teile. Hauptpersonen sterben nur bei Hull 0.\n\nDEINE REISE\nPassage entdecken, 4 Auftragsteile unten liefern, über die Windtreppe\nzurückkehren und ein Modul ausbauen. Die Winde ermöglicht Bergung.";
            MakeButton("Weiterfliegen · H", panel, new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(0, 22), new Vector2(280, 36), ToggleHelp, 17);
            helpPanel.SetActive(false);
        }

        void Update()
        {
            if (!initialized) return;
            if (Keyboard.current != null && (Keyboard.current.hKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)) ToggleHelp();
            if (Time.unscaledTime >= nextRefresh) { nextRefresh = Time.unscaledTime + .12f; Refresh(); }
        }

        void Refresh()
        {
            title.text = "THE ISLE  ·  " + (motor.Layer == 0 ? "Obere Wolkenwelt" : "Untere Wolkenwelt") + "  ·  Saat " + world.Seed + "  ·  Kamera " + (cameraRig.Automatic ? "Auto" : "Manuell (C)");
            objective.text = session.Objective;
            Vector3 wind = world.WindAt(motor.transform.position, motor.Layer);
            float headwind = Vector3.Dot(wind, motor.transform.forward);
            string windDirection = Mathf.Abs(headwind) < 1.5f ? "Seitenwind" : headwind > 0 ? "Rückenwind" : "Gegenwind";
            values.text = string.Format("Rumpf {0:0}/{1:0}  ·  Energie {2:0}%  ·  Fahrt {3:0.0} m/s  ·  {4} {5:0.0}  ·  Ladung {6}/{7}  ·  {8} Kredit", vitals.Hull, vitals.MaxHull, motor.Energy, motor.Speed, windDirection, wind.magnitude, session.CargoCount, session.CargoCapacity, session.Credits);
            message.text = session.Message;
            muteLabel.text = DemoAudio.Muted ? "Ton an · M" : "Stumm · M";
            context.text = session.ContextHint;
            hullBar.fillAmount = Mathf.Clamp01(vitals.Hull / vitals.MaxHull);
            hullBar.color = hullBar.fillAmount < .25f ? Red : Gold;
            energyBar.fillAmount = motor.Energy / 100f;
            portReload.fillAmount = combat.Reload01(Broadside.Port);
            starboardReload.fillAmount = combat.Reload01(Broadside.Starboard);
            danger.text = vitals.IsDestroyed ? "SCHIFF ZERSTÖRT · Neue Reise oder Duell starten" : vitals.Hull / vitals.MaxHull < .25f ? "Rumpf kritisch · Absetzen oder reparieren" : world.StormActive ? "Sturm: Hauptroute gesperrt" : string.Empty;
            foreach (var entry in crewLabels) entry.Value.text = StationName(entry.Key) + " · " + crew.Count(entry.Key) + "  (" + crew.WorkRate(entry.Key).ToString("0.0") + "×)";
            crewInfo.text = crew.InTransit > 0 ? crew.InTransit + " im Wechsel · 5 s Transit / 10 s Wechselpause" : "6 Unterbesatzung · Hauptpersonen führen ihre Station";
            repairInfo.text = (vitals.IsRepairing ? "Arbeit: " + ModuleShort(vitals.RepairTarget) : "Keine Feldreparatur") + " · Budget " + vitals.RepairBudget.ToString("0.0") + " HP\nTeile " + vitals.RepairParts.ToString("0.0") + " · Motor " + Mathf.RoundToInt(vitals.Integrity(ShipModule.Engine) * 100) + "%\nKanonen links " + Mathf.RoundToInt(vitals.Integrity(ShipModule.PortGuns) * 100) + "% · rechts " + Mathf.RoundToInt(vitals.Integrity(ShipModule.StarboardGuns) * 100) + "%";
            portPanel.SetActive(session.IsDocked);
            dockButton.gameObject.SetActive(!session.IsDocked && session.CanDock);
            if (session.IsDocked) RefreshPort();
            RefreshMap();
        }

        void RefreshPort()
        {
            portTitle.text = session.DockedPort.Name + " · " + (session.DockedPort.Layer == 0 ? "Himmelsbund" : "Freie Händler");
            for (int i = 0; i < 3; i++)
            {
                GoodsKind kind = (GoodsKind)i;
                goodsLabels[kind].text = DemoSession.GoodsName(kind) + " · " + session.GoodsCount(kind);
                portButtons[i * 2].GetComponentInChildren<Text>().text = "Kauf " + session.BuyPrice(kind);
                portButtons[i * 2 + 1].GetComponentInChildren<Text>().text = "Verk. " + session.SellPrice(kind);
                portButtons[i * 2].interactable = session.Credits >= session.BuyPrice(kind) && session.CargoCount < session.CargoCapacity;
                portButtons[i * 2 + 1].interactable = session.GoodsCount(kind) > 0;
            }
            deliveryLabel.text = session.DeliveryComplete ? "Auftrag abgeschlossen ✓" : session.DockedPort.Layer == 1 ? "4 Auftragsteile abgeben · +350" : "Lieferziel: Hafen der unteren Ebene";
            deliveryLabel.GetComponentInParent<Button>().interactable = !session.DeliveryComplete && session.DockedPort.Layer == 1;
            serviceLabel.text = "Werftservice · " + session.ServicePrice + " Kredit";
            serviceLabel.GetComponentInParent<Button>().interactable = session.Credits >= session.ServicePrice;
            foreach (var entry in upgradeButtons)
            {
                string name = entry.Key == UpgradeKind.Cargo ? "Laderaum +6" : entry.Key == UpgradeKind.Engine ? "Motor +Schub" : "Bergungswinde";
                entry.Value.GetComponentInChildren<Text>().text = name + "\n" + (session.HasUpgrade(entry.Key) ? "Eingebaut" : session.UpgradePrice(entry.Key) + " Kredit");
                entry.Value.interactable = session.DockedPort.Layer == 0 && !session.HasUpgrade(entry.Key) && session.Credits >= session.UpgradePrice(entry.Key);
            }
        }

        void RefreshMap()
        {
            foreach (var marker in mapMarkers.Values) marker.gameObject.SetActive(false);
            foreach (var site in world.Sites)
            {
                if (!site.Discovered || (site.Layer != motor.Layer && site.Layer != -1)) continue;
                if (!mapMarkers.TryGetValue(site.Id, out RectTransform marker))
                {
                    var label = MakeText("Entdeckt " + site.Id, chart, new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(25, 23), 16, Cream, TextAnchor.MiddleCenter);
                    marker = label.rectTransform;
                    mapMarkers.Add(site.Id, marker);
                }
                Vector3 delta = FlightPlane.Flat(site.Position - motor.transform.position);
                marker.anchoredPosition = new Vector2(delta.x * .75f, delta.z * .75f);
                var text = marker.GetComponent<Text>();
                text.text = site.IsPort ? "H" : site.IsGate ? "G" : site.IsSalvage ? "*" : "•";
                text.color = site.Blocked ? Red : site.IsGate ? Gold : Cream;
                marker.gameObject.SetActive(true);
            }
            playerMarker.SetAsLastSibling();
            playerMarker.localRotation = Quaternion.Euler(0, 0, -motor.transform.eulerAngles.y);
            chartCaption.text = "H Hafen · G Durchgang · * Wrack\n" + (motor.Layer == 0 ? "Oben" : "Unten") + " · Nord ↑ · Radius etwa 150 m";
        }

        public void ToggleHelp()
        {
            if(!HelpOpen)
            {
                timeBeforeHelp=Time.timeScale;audioBeforeHelp=AudioListener.pause;
                helpPanel.SetActive(true);Time.timeScale=0;AudioListener.pause=true;
            }
            else
            {
                helpPanel.SetActive(false);Time.timeScale=timeBeforeHelp;AudioListener.pause=audioBeforeHelp;
            }
        }

        void OnDisable()
        {
            if(HelpOpen) { Time.timeScale=timeBeforeHelp;AudioListener.pause=audioBeforeHelp; }
        }

        void OnDestroy()
        {
            if (solidSprite != null) Destroy(solidSprite);
        }

        void BuildVolumeSlider(Transform parent, Vector2 position)
        {
            var rect = Rect("Lautstärke", parent, new Vector2(0, 1), new Vector2(0, 1), position, new Vector2(125, 26));
            var slider = rect.gameObject.AddComponent<Slider>();
            var track = Rect("Ton Spur", rect, new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(125, 5));
            track.gameObject.AddComponent<Image>().color = new Color(.32f, .47f, .45f);
            var handleArea = Rect("Ton Griffbereich", rect, new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(113, 26));
            var handle = Rect("Ton Griff", handleArea, new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(12, 20));
            var image = handle.gameObject.AddComponent<Image>();
            image.color = Gold;
            slider.targetGraphic = image;
            slider.handleRect = handle;
            slider.minValue = 0;
            slider.maxValue = 1;
            slider.value = DemoAudio.Volume;
            slider.onValueChanged.AddListener(DemoAudio.SetVolume);
        }

        RectTransform Panel(string name, Transform parent, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
        {
            var rect = Rect(name, parent, anchor, pivot, position, size);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = Teal;
            var outline = rect.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(.77f, .65f, .43f, .55f);
            outline.effectDistance = new Vector2(1, -1);
            return rect;
        }

        RectTransform Rect(string name, Transform parent, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
        {
            var obj = new GameObject(name, typeof(RectTransform));
            obj.transform.SetParent(parent, false);
            var rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        Text MakeText(string name, Transform parent, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size, int fontSize, Color color, TextAnchor alignment = TextAnchor.UpperLeft)
        {
            var rect = Rect(name, parent, anchor, pivot, position, size);
            var text = rect.gameObject.AddComponent<Text>();
            text.font = font;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            return text;
        }

        Button MakeButton(string label, Transform parent, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size, Action action, int fontSize = 14)
        {
            var rect = Rect(label, parent, anchor, pivot, position, size);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = new Color(.22f, .36f, .35f);
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.13f, 1.12f, .93f);
            colors.pressedColor = new Color(.71f, .85f, .78f);
            colors.disabledColor = new Color(.48f, .54f, .51f, .7f);
            button.colors = colors;
            MakeText("Beschriftung", rect, new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, size - new Vector2(6, 2), fontSize, Cream, TextAnchor.MiddleCenter).text = label;
            button.onClick.AddListener(() => { UiActivated?.Invoke(); action(); });
            return button;
        }

        Image Bar(Transform parent, string name, Vector2 position, Vector2 size, Color color)
        {
            var backing = Rect(name + " Hintergrund", parent, new Vector2(0, 1), new Vector2(0, 1), position, size);
            backing.gameObject.AddComponent<Image>().color = new Color(.04f, .12f, .14f);
            var fill = Rect(name + " Füllung", backing, new Vector2(0, 1), new Vector2(0, 1), Vector2.zero, size);
            var image = fill.gameObject.AddComponent<Image>();
            image.color = color;
            image.sprite = solidSprite;
            image.raycastTarget = false;
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillOrigin = (int)Image.OriginHorizontal.Left;
            return image;
        }

        static string StationName(CrewStation station) => station == CrewStation.Navigation ? "Navigator" : station == CrewStation.Engineering ? "Maschinist" : "Geschützmeister";
        static string StationShort(CrewStation station) => station == CrewStation.Navigation ? "Nav" : station == CrewStation.Engineering ? "Masch" : "Gesch";
        static string ModuleShort(ShipModule module) => module == ShipModule.Hull ? "Rumpf" : module == ShipModule.Engine ? "Motor" : module == ShipModule.PortGuns ? "Links" : "Rechts";
    }
}
