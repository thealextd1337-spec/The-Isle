# Demo — technische Prüfung

Stand: 11. Oktober 2026. Ausgangspunkt: reines Planungsrepository, Commit `1d42d85`. Der erste Unity-Stand wird lokal unter Windows geprüft. Keine bestehenden Unity-Szenen wurden übernommen; die Demo besitzt ihre eigene Startszene.

## Umgebung

- Unity 6000.6.4f1, URP 17.6.0, Input System 1.20.0, uGUI 2.6.0.
- Unity CLI 1.0.0-beta.12 und projectlokales Pipeline-Paket 0.8.0-exp.1.
- Projektpfad: `Game/`. Startszene: `Assets/TheIsle/Scenes/FirstVoyage.unity`.
- Ziel: Windows x86_64, Mono, Fenster 1600×900. Kein Cloud-Projekt verknüpft.
- Szene, Material- und Audioassets werden durch `TheIsle.Editor.DemoBuilder.CreateScene()` über Editor-APIs erzeugt. Alle Metadaten stammen aus Unity.

## Ausgeführte Prüfungen

| Kriterium | Evidenz | Ergebnis |
|---|---|---|
| Seedwelt, Abstand, Häfen, offene Rückroute | 200 Seeds, wiederholte Erzeugung; 8.806 Assertions in sieben Kategorien | bestanden |
| Passagen und Fog of War | Ebenenfilter, außerhalb Gate abgewiesen, Sturm sperrt Hauptgate, Alternative bleibt nutzbar | bestanden |
| Hauptbesatzung und Mannschaft | Gesamtzahl, reservierte Stationsplätze, keine Arbeit im Transit; Hauptcrew lebt bei Modulschäden, stirbt erst bei Hull 0 | bestanden |
| Reparaturgrenzen | Zielwechsel erneuert keine Vorräte; tatsächliche zeitabhängige Reparatur verbraucht Teile und Budget; Hafen kostet Geld | bestanden |
| Reiseablauf | Im Play Mode: Startdock, Kauf, Winde, Bergung, unterer Hafen, einmalige Lieferung, Sturm, Rückroute, Ausbau | bestanden |
| Flugphysik | echte FixedUpdates: schweres Schiff beschleunigt langsamer und verbraucht mehr, Trägheit, Bremse, Drehmoment, Gegensteuern, abklingende Drehbewegung, Winddrift | bestanden |
| Breitseiten und Geschosse | Winkelgrenzen, falsche Seite abgewiesen, zwei echte Kugeln treffen für 16 Rumpfschaden | bestanden |
| Gemeinsames Nachladen | zwei leere Bänke nach 10 Sekunden ungefähr halb fertig, nach 20 Sekunden beide fertig | bestanden |
| Kamera und Pause | Beide Duellschiffe innerhalb der HUD-Ränder, manueller Zoom bleibt erhalten, Automatik zurücksetzbar; Hilfe pausiert Simulation und Audio, verhindert Salven und E-Interaktionen; auch Neustart räumt Pause auf | bestanden |
| Reset | ein HUD, ein AudioListener, neuer Seed, alte Schiffe/Kugeln/Audio entfernt | bestanden |
| Soundverdrahtung | neun WAVs, laufende AudioSources, nichtleere PCM-Motorsamples, Lautstärke/Mute im HUD | technisch bestanden; Lautsprecherklang nicht beurteilt |
| Assets | `unity projects verify Game --strict`: keine fehlenden/orphan Metas, doppelten GUIDs oder ungültigen Manifeste | bestanden |
| Grafik | Spielaufnahme mit Kamera, gemalten Wolken, sichtbarem Schiff und uGUI betrachtet | Grafikprototyp geprüft; Konzeptqualität noch auszuarbeiten |

Die Play-Mode-Prüfung umfasst **53 Verhaltenstests**. Sie steuert Komponenten direkt und versetzt das Schiff zwischen Testorten; sie behauptet keine menschlich geflogene vollständige Reise. Flugkräfte und Geschosskollisionen werden gesondert in realen Physics-Updates geprüft. Eine bereits vor der Lieferung gekaufte Verbesserung wird bei Rückkehr als Ausbau anerkannt, damit frühzeitiger Handel den Abschluss nicht blockiert.

Nach Korrektur der KI-Annäherung dauerte ein beschleunigter KI-gegen-KI-Vergleich **281,88 simulierte Sekunden (4:42)**. Die Schiffe feuerten 16 beziehungsweise 15 Salven und verursachten jeweils 256 Rumpfschaden; Feldreparaturen verwendeten begrenzte Vorräte. Die Begegnung endete durch Trennung beim Rückzug. Dieser eine Vergleich ist eine technische Funktionsprobe, keine Aussage über die Spieldauer mit Menschen.

Die erste Kompilierung fand zwei neue API-Probleme (obsolete GetInstanceID, fehlender Offset bei AudioClip.SetData), die korrigiert wurden. Der Spieltest fand zusätzlich eine vom Rigidbody überschriebene Startposition. Das ist durch gemeinsames Setzen von Rigidbody- und Transformposition behoben. UI-Klicks unterdrücken Mausfeuer; Q/R bleiben unabhängig davon verfügbar. Beim Ebenenwechsel werden alte Projektile entfernt, bevor Inselcollider der neuen Ebene aktiv werden.

## Reproduzieren

Im Projekt-Editor die Startszene öffnen und kompilieren lassen. Aufruf über Unity CLI, immer mit explizitem Zielprojekt:

```powershell
unity command eval 'return TheIsle.Editor.DemoRuleChecks.Run();' --project-path Game --caller plugin --skill unity-build-validation --format json
unity command editor_play --project-path Game --caller plugin --skill unity-build-validation --format json
unity command eval 'TheIsle.Editor.RuntimeSmoke.Begin(); return 1;' --project-path Game --caller plugin --skill unity-build-validation --format json
unity command console --level warn --project-path Game --caller plugin --skill unity-build-validation --format json
unity command editor_stop --project-path Game --caller plugin --skill unity-build-validation --format json
unity projects verify Game --strict --format json
```

Regel- und Laufzeitberichte entstehen lokal unter `artifacts/`. Die Laufzeitprüfung dauert ungefähr 45 Sekunden. Testharness-Code ist mit UNITY_EDITOR geschützt und wird nicht in den Windows-Player aufgenommen. Es ist kein NUnit-Testlauf; die Berichte nennen die tatsächlich ausgeführten Assertions und Verhaltenstests.

Optionaler beschleunigter KI-Vergleich: `TheIsle.Editor.BattleSmoke.Begin()` im Play Mode. Er verwendet identische Schiffe und dieselbe KI auf beiden Seiten. Das ist eine Funktionsprobe für beidseitige Salven und Schäden, keine Messung menschlicher Kampfdauer.

Windows-Build im Editor: `TheIsle.Editor.DemoBuilder.BuildWindows()`. Bei geschlossenem Editor alternativ:

```powershell
unity build Game --target StandaloneWindows64 --execute-method TheIsle.Editor.DemoBuilder.BuildWindows --output-path Game/Builds/Windows/TheIsle.exe --allow-dirty-build --timeout 600 --format json
```

Die ursprünglichen URP-Template-Settings bleiben Grundlage. Gezielte Änderungen betreffen Product/Company, Fensterauflösung, Mono-Backend, Hintergrundbetrieb und die eine Buildszene; Library, Temp, Logs, UserSettings und Builds sind ignoriert.

## Praktische Grenzen

Noch offen: menschliches Fluggefühl, 3–5 Minuten Kampfdauer, Preisbalance, Lautsprechermischung, weitere Auflösungen und Geräte. Keine Performance-Budgets oder Multiplayer-Anforderungen wurden vorgegeben. Fertige gemalte Assets, Spielstände, Diplomatie/Ruf, Verhandlung, Entern, individuelle Crew-Entwicklung und Rettung nach Zerstörung sind spätere Schritte. Die Demo ist ein spielbarer erster Prototyp.

## Windows-Artefakt

Der abschließende Build `build_4fbeab0b195a` ist **Succeeded**, mit 0 Fehlern und 106.933.760 Byte. Eine Warnung meldet die im Player deaktivierte Pipeline-Editorbrücke; die Demo benötigt diese Brücke zur Laufzeit nicht. Die EXE wurde separat gestartet: Direct3D 11 und PhysX initialisierten, der Prozess blieb aktiv, das Player-Log enthielt keine Exceptions oder Fehler. Dieser Starttest ersetzt keine menschliche Bedienungsprüfung.

Prüfberichte liegen unter `docs/validation/`. Die lokale Windows-ZIP befindet sich in `artifacts/TheIsle-FirstVoyage-Windows.zip` (etwa 38 MiB). Zum Spielen die vollständige ZIP entpacken und `TheIsle.exe` starten; Data- und Laufzeitordner müssen neben der EXE bleiben. Build und ZIP sind absichtlich nicht im Quellcode-Repository enthalten.

Spielaufnahmen: [Erste Reise](design/demo/first-voyage.png), [Breitseiten-Duell](design/demo/broadside-duel.png). Sie zeigen den aktuellen Grafikprototyp im Editor, nicht die spätere Qualität fertiger gemalter Assets. Unity-serialisierte Template-Dateien enthalten generatorbedingte Leerzeichen; die selbst geschriebenen Scripts und Dokumente wurden separat auf Diff-Fehler geprüft.
