# The Isle — Erste Demo

Stand: 11. Oktober 2026. Unity-Projekt: Game/. Startszene: Assets/TheIsle/Scenes/FirstVoyage.unity. Grafikrichtung **B — Gemaltes Abenteuer** bleibt verbindlich. Die Umsetzung verwendet eigene Pinseltexturen, weiche Wolken und einfache 3D-Modelle als spielbaren Grafikprototyp. Die Konzeptzeichnung bleibt das Qualitätsziel für spätere fertige Assets.

## Ablauf

Eine vollständige Reise verbindet Erkundung, Fluggefühl, Handel, feste Ebenenwechsel, Kampf, einen veränderten Rückweg und Schiffsausbau. Die langfristige Zielzeit von 20–30 Minuten ist noch keine gemessene Spieldauer. Die kleine Testwelt ermöglicht kürzere Wiederholungen.

1. Vier geschützte Auftragsteile liegen im Zeppelin. Im oberen Hafen Morgenanker handeln, ausbauen und Service nutzen.
2. Selbst fliegen, Wind und Trägheit kennenlernen. Der Navigator gibt eine Richtung zur Hauptpassage; die Karte zeigt nur entdeckte Orte.
3. Die Passage entdecken und mit E in die untere Ebene wechseln. Ein Wechsel außerhalb einer Passage ist ausgeschlossen.
4. Tiefenlicht entdecken, anlegen und die Teile für 350 Kredit liefern. Eine Piratenbegegnung erprobt Breitseite, Vorhalt, Ausweichen und Rückzug.
5. Die Lieferung löst eine Sturmwarnung aus: stärkerer Wind, gesperrte Hauptpassage und höhere Teilepreise. Die Windtreppe bleibt offen; ihr Funkhinweis kartiert den Rettungsweg.
6. Zur oberen Werft zurückkehren und einen Ausbau kaufen. Freies Fliegen, Handel und optionale Wrackbergung bleiben möglich.

**Duell** startet ein wiederholbares Kampfszenario mit gleichwertigen Schiffen und ruhigem Wind. **Neue Reise** erzeugt eine andere Ausgangswelt und setzt Auftrag, Geld, Besatzung und Erkundung zurück. Spielstände und dauerhafter Fortschritt sind noch nicht umgesetzt.

## Welt und Flug

- Fünf Inseln, zwei Häfen, zwei Höhenebenen, zwei feste Übergänge und ein Wrack.
- Seedabhängige Rotation und Positionsabweichungen in einer kleinen, zusammenhängenden Welt.
- Entdeckte Orte bleiben während der Partie kartiert. Die Karte zeigt nur aktuelle Ebene und bekannte Passagen; die Weltansicht enthüllt Inseln im Besatzungs-Sichtradius.
- Physikalischer Schub, Drehmoment, Bremse und Trägheit auf XZ. Orthografische isometrische Kamera.
- Gegen-, Rücken- und Seitenwind, Böen und Windschatten hinter Inseln.
- Fracht erhöht Masse, Verbrauch und Bremsweg. Zusätzliche Masse mindert Beschleunigung und Drehfähigkeit.
- Bezahlter Hafenservice liefert Energie; bei leerer Energie bleibt langsamer Notantrieb.
- Zwei Hafenfraktionen mit verschiedenen Preisen. Ruf, Diplomatie und Verhandlungen sind spätere Erweiterungen.

## Breitseitenkampf

Jede Seite hat zwei Kanonen und einen **60°-Feuerbogen**, ±30° um die Querrichtung. Bug und Heck besitzen keine Geschütze. Die Maus zielt innerhalb dieser Bögen; Q/R können eine direkte seitliche Salve abgeben. Geschosse fliegen tatsächlich, brauchen Vorhalt und werden durch Inseln abgefangen.

Beide Waffenbänke teilen dieselbe Geschützmannschaft. Eine Bank benötigt bei zwei Geschützleuten zehn Sekunden Nachladearbeit; zwei leere Bänke teilen die Kapazität und brauchen zusammen zwanzig Sekunden. Drehen verdoppelt daher nicht die Nachladeleistung.

Abstimmungswerte: 280 Rumpf-HP, 8 Rumpfschaden pro Kugel, 16 pro vollständig treffender Salve. Treffer in Modulzonen verursachen zusätzlich vier Modulschaden, jedoch keinen zusätzlichen Rumpfschaden. Beschädigte Waffen laden langsamer; beschädigte Motoren verlieren Leistung. Komponenten behalten auch bei 0 HP begrenzte Restfunktion.

**Ziel: etwa 3–5 Minuten für einen ebenbürtigen Kampf einschließlich Manövern.** Es gibt keinen Mindesttimer. Dauer und Siegquote müssen mit menschlichen Spieltests gemessen werden; alle Zahlen sind Startwerte.

Sinnvolle Strategien: Wind und Abstand nutzen, blinde Bug-/Heckbereiche kreuzen, beim Nachladen ausweichen, Deckung und Vorhalt einsetzen, Motor-/Waffenbereiche treffen, Mannschaft zu Reparaturen abziehen oder zurückziehen. Nach mehr als 140 Metern Trennung für zwölf Sekunden endet eine begonnene Begegnung. Die KI sucht seitliche Feuerpositionen, weicht beim Laden aus, nutzt begrenzte Reparaturen und zieht sich bei schwerem Schaden zurück. Sie verwendet dieselben Regeln und Kräfte wie das Spielerschiff.

Verhandlungen, Frachtabgabe, Entern und Kapitulation sind noch nicht umgesetzt.

## Hauptbesatzung und Mannschaft

Verbindliche Nutzerregel: **Hauptbesatzung kann ausschließlich bei kompletter Schiffzerstörung sterben.** Treffer oder beschädigte Komponenten töten keine Hauptperson. Im Teststand bedeutet Rumpf-HP 0 komplette Zerstörung und Verlust der Hauptbesatzung. Eine spätere Rettungs- oder Todeswahrscheinlichkeit ist offen.

| Station | Hauptfähigkeit | Wirkung der Mannschaft |
|---|---|---|
| Navigation | Orientierung und Wetterhinweise | Sichtweite und Ruderleistung |
| Maschine | Feldreparatur und Maschinenbetrieb | Reparaturtempo und Motorleistung |
| Geschütze | Betrieb beider Breitseiten | gemeinsames Nachladetempo |

Sechs Mannschaftsmitglieder beginnen mit zwei pro Station. Maximal vier arbeiten an einer Station. Versetzen dauert fünf Sekunden ohne Arbeitsleistung; dieselbe Person darf zehn Sekunden nach Beginn erneut versetzt werden. Die Gesamtmenge bleibt sechs. Ohne Mannschaft arbeitet die Hauptperson langsam weiter. Arbeitsfaktor: 0,4 / 1,0 / 1,6 bei null / zwei / vier Personen. Geschützleute zur Maschine versetzen beschleunigt Reparatur und verlangsamt Nachladen.

Individuelle Persönlichkeiten, Erfahrung und Beziehungen sowie Verwundung oder Tod der Mannschaft sind noch offen.

## Reparaturen

- Unterwegs ein Ziel wählen: Rumpf, Motor oder Waffenbank. Nur ein Ziel wird gleichzeitig bearbeitet.
- Etwa 0,8 HP/s bei zwei Maschinisten, maximal etwa 1,3 HP/s. Fertige Ziele oder leere Vorräte beenden die Arbeit.
- **Gemeinsames Budget von 35 reparierten HP je Hafenversorgung**, für alle Komponenten zusammen. Zielwechsel setzt es nicht zurück.
- Jeder reparierte HP verbraucht 0,1 Reparaturteile. Bergung erhöht den Vorrat, erneuert aber nicht das Budget.
- Gesundheit überschreitet niemals Maximalwerte. Hafenservice kann komplett zerstörte Schiffe nicht wiederbeleben.
- Bezahlter Werftservice stellt Integrität, Energie, Teile und Budget wieder her; der Preis steigt mit Rumpfschaden.

Dauerhafte Schäden, Ersatz ganzer Module und längere Werftarbeiten können später folgen.

## Module, Handel und Sound

Laderaum: sechs zusätzliche Plätze und mehr Gewicht. Motor: mehr Schub bei höherem Verbrauch. Winde: Wrackbergung bei zusätzlicher Masse. Alle Ausbauten erscheinen am Schiff. Bauteile, Vorräte und Kristalle haben verschiedene Hafenpreise. Auftragsteile können nicht verkauft werden.

Eigene synthetisierte WAVs liefern Wind, Motor, Kanonen, Treffer, Reparatur, Hafen, Ebenenübergang, Erfolg und UI-Rückmeldung. Lautstärke und Stummschaltung bleiben als lokale Einstellungen erhalten. Klang und Mischung sind Prototypwerte; die Lautsprecherausgabe muss der Spieler beurteilen.

## Technik und Bedienung

Unity **6000.6.4f1**, URP **17.6.0**, C#, Input System, uGUI. Die bereits installierte Version ermöglicht sofortige Entwicklung. Arbeitsannahme: Windows-Einzelspieler. Cloud, Mehrspieler und Monetarisierung gehören nicht zur Demo. CLI/Pipeline steuert nur dieses Projekt; Startszene und Assets entstehen über die Editor-API.

| Eingabe | Aktion |
|---|---|
| W/S oder hoch/runter | Schub vor/zurück |
| A/D oder links/rechts | Drehen |
| Leertaste | Bremsen |
| E | Hafen, Passage oder Bergung |
| Q/R | Backbord-/Steuerbordsalve |
| linke/rechte Maustaste | jeweilige Breitseite zum Mausziel |
| Mausrad oder +/− | Manueller Kamerazoom |
| C | Automatische Kamera zurücksetzen |
| Z/X | Kamera in 45°-Schritten drehen |
| H/Esc | Hilfe und Pause |
| M | Stummschaltung |

Zum Anlegen auf höchstens 3 m/s abbremsen. Crewwechsel, Reparatur, Handel, Service und Ausbau erfolgen über HUD-Schaltflächen. Die Hilfe pausiert Simulation und Sound. Die Kamera blickt in Flugrichtung voraus, beruhigt sich beim Hafenanflug und hält im Kampf beide Schiffe im Bild. Manueller Zoom bleibt erhalten, bis C die Automatik zurücksetzt. Flugsteuerung ausschließlich per Tastatur; es gibt keine Mauskurs-Hilfe.

## Prüfung und weitere Arbeit

Nach dem ersten menschlichen Flugtest ist das [Folgekonzept für Ankern, Mannschaft und sichtbaren Erkundungsnebel](ankern-mannschaft-nebel.md) ausgearbeitet. Die Bezeichnung im HUD lautet bereits Mannschaft. Haltemodus, Presets, neue Wirkungskurven und flächiger Nebel sind der nächste geplante Ausbau; die folgenden Prüfberichte beziehen sich auf die bisherige Mechanik.

Die Prüfevidenz steht in [Validierung](demo-validierung.md). Automatisierte Regelprüfungen ersetzen keine menschlichen Flug-, Klang- oder Balancingtests. B benötigt nach dem funktionalen Prototyp ausgearbeitete Modelle, Effekte, Beleuchtung und hochwertigere gemalte Texturen.

Später: größere Schiffsklassen, dritte Ebene, weitere Weltereignisse, Ruf/Diplomatie, Verhandlungen, individuelle Hauptpersonen, Speichern/Fortsetzen, Rettung und hochwertige Grafik-/Soundassets.
