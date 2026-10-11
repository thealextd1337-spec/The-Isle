# The Isle

Ein Spielkonzept über die Erkundung schwebender Inseln mit einem selbst gesteuerten, modular ausgebauten Zeppelin. Handel, Wind, Besatzung, Piraten und Fraktionen prägen die Reisen durch mehrere Höhenebenen.

Gewählter Grafikstil: **B — Gemaltes Abenteuer**, mit gemalten Wolken, warmer Beleuchtung und sanften Materialtexturen in einer isometrischen Ansicht.

Jeder neue Durchlauf beginnt mit einer anderen Welt. Fog of War verbirgt unbekannte Regionen; Weltereignisse können Routen, Inseln, Versorgung und Machtverhältnisse im Verlauf einer Partie verändern.

## Planungsdokumentation

- [Spielplanung und Vorschläge für Weltereignisse](docs/spielplanung.md)
- [Grafikstil: vier gezeichnete Designrichtungen](docs/grafikstil.md)
- [Inhalt und Ablauf der ersten Demo](docs/erste-demo.md)
- [Technische Prüfung und Einschränkungen](docs/demo-validierung.md)

## Erste Demo starten

Unity-Projekt `Game/` mit **Unity 6000.6.4f1** öffnen. Startszene: `Assets/TheIsle/Scenes/FirstVoyage.unity`. Play drücken. Windows-Build lokal: `Game/Builds/Windows/TheIsle.exe`.

WASD/Pfeile fliegen, Leertaste bremsen, E interagieren, Q/R seitlich feuern, Maus zielen, H/Esc Hilfe und Pause, M stummschalten. Kamera: C Automatik, Z/X drehen, +/− oder Mausrad zoomen. Flugsteuerung ausschließlich per Tastatur. **Duell** startet den separaten Kampf; **Neue Reise** erzeugt eine neue Welt. Hafenservice ist bezahlt; Auftragsteile sind vor Verkauf geschützt.

Enthalten: Physik und Wind, Erkundung, zwei Ebenen mit festen Passagen, Handel, Auftrag, Sturm, drei Ausbauten, Besatzungszuweisung, begrenzte Reparaturen, Piraten und Breitseitenkampf sowie Sound. Die gemalte Grafik ist ein erster spielbarer Prototyp; Kampfdauer und Fluggefühl brauchen menschliche Abstimmung. Spielstände sind noch nicht implementiert.

Details, reproduzierbare Prüfungen und Buildanleitung stehen in [Demo-Validierung](docs/demo-validierung.md). Stand: 11. Oktober 2026.
