# Ankern, Mannschaft und sichtbarer Erkundungsnebel

Stand: 11. Oktober 2026. Folgekonzept nach dem ersten menschlichen Flugtest. Das Fluggefühl wurde vom Spieler positiv beurteilt und bildet die Grundlage. Neue Zahlen sind Abstimmungswerte für den nächsten Prototyp, keine bereits gemessene Balance.

## Festgelegt und vorgeschlagen

**Festgelegt:** Die bisherige Unterbesatzung heißt **Mannschaft**. Die Hauptbesatzung führt weiterhin ihre Stationen und kann nur bei kompletter Schiffzerstörung sterben. Mannschaft wird links unten zugewiesen. Viel Navigation soll Reisen lohnender machen; im Kampf soll eine Verlagerung zu den Geschützen sinnvoll sein. Ankern ist überall in der aktuellen Wolkenebene als energieverbrauchender Haltemodus möglich. Das Lichten darf Zeit kosten. Sichtbarer Nebel über der Weltkarte soll sich bei Erkundung langsam lichten.

**Vorgeschlagen:** Die folgenden Zeiten, Wirkungskurven, Presets und Eingaben. Dieses Dokument beschreibt den nächsten Ausbau; Anker und flächiger Erkundungsnebel sind noch keine Funktionen der bestehenden Demo. Die bisherige Demo besitzt Stationswechsel und Sichtweiten, aber hauptsächlich ausgeblendete Orte statt einer sichtbaren Nebeldecke.

## 1. Ankern: anhalten und eine Entscheidung treffen

Der Kapitän bremst ab und aktiviert mit **F** oder der Schaltfläche **Ankern** einen Haltemodus. Das Luftschiff arbeitet mit den Triebwerken gegen Wind und Drift. Eine aufspannende Stabilisierungsvorrichtung und eine laufende Winde vermitteln den Begriff Anker; es wird keine Kette zu einem unsichtbaren Meeresboden benötigt.

Der Modus hält eine Position und die Ausrichtung beim Setzen. Er navigiert nicht zu einem Ziel. Schiff und Geschosse bleiben physikalisch: ein Stoß oder übermäßiger Wind kann das Schiff versetzen. Das Halten bietet keinen Schutz vor Piraten, keine zusätzlichen Reparaturen und keinen Schadensbonus.

### Ablauf für das Demoschiff

| Zustand | Verhalten | Rückmeldung |
|---|---|---|
| Frei | Normales Fliegen. Ankern erst bei höchstens 3 m/s. | Ankern / Erst abbremsen |
| Anker setzen | Bei zwei Maschinenleuten 4 s. Antrieb bremst weiter und stabilisiert. | Fortschritt und verbleibende Sekunden |
| Geankert | Position und Ausrichtung werden mit begrenzter Kraft gehalten. Schub und Rudereingaben bewegen das Schiff nicht. | Halteenergie, Windlast, Anker lichten |
| Anker lichten | Bei zwei Maschinenleuten 8 s. Schiff wird bis zum Abschluss weiter gehalten. Geschütze bleiben nutzbar. | Fortschritt und Freigabe in … s |
| Frei | Haltesystem wird gelöst. Danach sofort normale Tastatursteuerung. | Anker frei |

F oder Schub während des Haltens fordert das Lichten an; Schub ist keine sofortige Flucht. Während des Setzens darf F abbrechen: bereits ausgeführte Arbeit muss zurückgenommen werden und kann nicht als sofortige Bremse missbraucht werden. Während des Lichtens lässt sich der Ablauf nicht durch wiederholtes Setzen zurücksetzen. Das Schiff nimmt danach nur aktuell gehaltene Flugtasten an, keine gespeicherten Schubbefehle.

### Mannschaft und Energie

- Setzen und Lichten nutzen die Arbeitsleistung der Maschinenstation: `Dauer = Basisdauer / Arbeitsfaktor`. Der Arbeitsfaktor beträgt 0,4 / 0,7 / 1,0 / 1,3 / 1,6 bei null bis vier Mannschaftsmitgliedern. Hauptbesatzung kann die Arbeit auch allein abschließen.
- Setzen und Lichten beanspruchen die Maschinenarbeitskapazität. Feldreparatur pausiert in diesen kurzen Phasen und läuft danach mit verbleibenden Vorräten weiter. Bloßes Halten erlaubt normale Feldreparatur.
- Stationswechsel während eines Vorgangs verändert die Geschwindigkeit der verbleibenden Arbeit; der Fortschritt wird nicht gelöscht und seine Gesamtdauer nicht rückwirkend neu gestartet.
- Bei zwei Maschinenleuten entsprechen 4 s Setzen und 8 s Lichten der Basis. Bei vier sind es 2,5 / 5 s; ohne Mannschaft 10 / 20 s. Die Hauptperson verhindert eine unlösbare Situation.
- Startwert für Halteenergie: 0,05 Energie/s Grundbedarf plus 0,01 Energie/s je m/s lokalem Wind, skaliert mit Schiffmasse und Motorzustand. Dadurch kostet Halten bei leichtem Wind ungefähr 4–6 Energie/min. Die Kosten ersetzen normalen Schubverbrauch während des Haltens und werden nicht doppelt berechnet.
- Starker Wind und ein beschädigter Motor können die verfügbare Haltekraft übersteigen. Eine Lastanzeige warnt zuerst; das Schiff driftet dann trotz Haltemodus. Es wird nicht teleportiert oder unverwundbar fixiert.
- Bei leerer Energie beginnt automatisch das Lichten. Das Halten fällt aus, Winddrift setzt wieder ein, aber die Hauptperson kann den Mechanismus lösen. Anschließend bleibt der bestehende langsame Notantrieb verfügbar.
- Andocken, Bergung und Ebenenwechsel bleiben eigene Tätigkeiten. Ein Ebenenwechsel braucht einen freien Anker. Andocken kann einen gesetzten Anker in die Hafenbindung übernehmen; beim Ablegen entsteht keine zweite, unsichtbare Sperre.

**Taktischer Preis:** Der Spieler kann sich zum Lesen der Karte oder zur Reparatur stabilisieren, verliert aber Beweglichkeit. Wer sämtliche Maschinenleute zu den Kanonen schickt, kann schneller feuern und benötigt bis zu 20 s zum Lichten. Verändert sich das Wetter, muss diese Entscheidung rechtzeitig korrigiert werden.

## 2. Mannschaft: sechs Menschen, drei Stationen

Die drei Hauptpersonen sind Navigatorin, Maschinist und Geschützmeisterin. Sie bleiben an ihren Stationen. Die **sechs Mannschaftsmitglieder** werden verteilt; höchstens vier können an einer Station arbeiten. Hauptperson und Mannschaft zählen getrennt, damit aus einer Anzeige wie „Navigation 4“ nicht fünf frei zuweisbare Menschen entstehen.

Die Mannschaft verändert tatsächliche Tätigkeiten. Zwei pro Station bleiben der Referenzzustand des guten Fluggefühls. Mehr Navigation hilft auch im Kampf beim Manövrieren; mehr Geschützleute nützen auf Reisen bei einem unerwarteten Angriff. Es gibt keinen automatisch angewandten Kampfbonus und keinen versteckten Wechsel des Mannschaftsplans.

### Navigation: Orientierung und Handhabung auf Reisen

| Mannschaft | Ruderkraft gegenüber heute mit 2 | Erkundungsradius | Schubenergie durch Trimmen |
|---:|---:|---:|---:|
| 0 | 75 % | 28 m | +8 % |
| 1 | 88 % | 32 m | +4 % |
| 2 | 100 % | 36 m | unverändert |
| 3 | 108 % | 42 m | −4 % |
| 4 | 115 % | 48 m | −7 % |

Zusätzliche Hände stellen Ruder, Trimmflächen und Ausguck. Sie verbessern die Reaktion auf manuelles Steuern, entdecken einen breiteren Korridor und sparen auf längeren Reisen etwas Energie. Wind, Trägheit, Frachtmasse und Tastaturkurs bleiben erhalten. Die Besetzung erhöht weder die Schiffshöchstgeschwindigkeit noch steuert sie automatisch um Hindernisse. Das Lichtungsgebiet des Nebels wächst mit dem Erkundungsradius; im Stand wird kein unbegrenzt großer Bereich aufgedeckt.

### Maschine: Fahrt, Reparatur und Ankerarbeit

Motorleistung als sanfte Kurve gegenüber dem heutigen Referenzflug: 85 / 93 / 100 / 106 / 110 Prozent bei null bis vier Mannschaftsmitgliedern. So bleibt Reise mit geringer Besetzung möglich, während schwere Fracht und schlechte Bedingungen zusätzliche Maschinenleute rechtfertigen.

Feldreparatur und Ankerarbeit verwenden den Arbeitsfaktor 0,4 / 0,7 / 1,0 / 1,3 / 1,6. Das bestehende Reparaturbudget, Teilekosten und Schadensregeln bleiben maßgeblich. Zusätzliche Maschinenleute reparieren schneller, erzeugen aber keine kostenlosen Vorräte. Der Stand mit zwei Personen behält 0,8 Rumpf-HP/s.

### Geschütze: mehr Salven, kein Gratis-Doppelwechsel

| Mannschaft | Arbeitsfaktor | Eine vollständig leere Bank | Beide Banken vollständig leer |
|---:|---:|---:|---:|
| 0 | 0,4 | 25 s | 50 s |
| 1 | 0,7 | 14,3 s | 28,6 s |
| 2 | 1,0 | 10 s | 20 s |
| 3 | 1,3 | 7,7 s | 15,4 s |
| 4 | 1,6 | 6,25 s | 12,5 s |

Die Geschützstation versorgt beide Breitseiten gemeinsam. Drehen und mit der anderen Bank feuern bleibt taktisch nützlich, verdoppelt aber nicht die Mannschaftsleistung. Die Tabelle zeigt unbeschädigte Geschütze und konstant besetzte Stationen; Treffer und laufende Versetzungen verändern die verbleibende Nachladearbeit. Schaden je Kugel und Feuerbogen bleiben unverändert.

### Erste Presets

Reihenfolge: Navigation / Maschine / Geschütze. Ein Preset ist ein Mannschaftsbefehl, keine Teleportation.

| Plan | Verteilung | Vorteil und Kosten |
|---|---|---|
| Ausgewogen | 2 / 2 / 2 | Bisheriges Grundverhalten |
| Erkunden | 4 / 2 / 0 | 48 m Sicht, gutes Ruder, sparsamere Fahrt; eine Waffenbank lädt 25 s |
| Gefechtsstationen | 1 / 1 / 4 | Eine Bank lädt 6,25 s; schwächeres Ruder, kürzere Sicht, Lichten etwa 11,4 s |
| Sturmfahrt | 3 / 3 / 0 | Bessere Handhabung, Maschinenleistung und Haltearbeit; langsame Waffen |
| Schadenskontrolle | 1 / 4 / 1 | 1,28 HP/s Reparatur und Lichten in 5 s; eine Bank lädt 14,3 s |

## 3. Bedienung links unten

Das vorhandene Panel wird **Mannschaft & Reparatur**. Jede Stationszeile zeigt die feste Hauptperson, vier mögliche Mannschaftsplätze, aktuelle Wirkung und gegebenenfalls eintreffende Leute mit Restzeit. Die Anzeige unterscheidet zum Beispiel `2 an Station · +1 in 3 s` von drei bereits arbeitenden Menschen.

- Eine Person über die Stationszeile auswählen und eine Zielstation anklicken. Im Auftrag stehen Quelle, Ziel und Wirkung, beispielsweise „Navigation → Geschütze: Sicht 42 → 36 m; Nachladen schneller“.
- Die bestehende Versetzungsdauer bleibt zunächst 5 s; währenddessen arbeitet die Person an keiner Station. Ein erneuter Wechsel derselben Person ist frühestens 10 s nach dem ersten Auftrag möglich.
- Die vier Plätze einer Station umfassen bereits reservierte Ankünfte. Mehrfachklicken kann keine zusätzlichen Mannschaftsmitglieder oder überfüllten Stationen erzeugen.
- Presets ordnen die ganze Verteilung mit einem Klick an. Alle berechtigten Personen wechseln gleichzeitig; gesperrte Personen bleiben mit sichtbarer Warteanzeige im Auftrag. Es gibt nur einen aktuellen Zielplan. Ein neuer Plan ersetzt den alten, ohne Personen im Transit zu duplizieren oder deren laufenden Weg kostenlos abzubrechen.
- Das Menü zeigt unmittelbar das **Ziel**, die Statistiken richten sich weiterhin nach den **tatsächlich arbeitenden** Personen. Gegebenenfalls zuerst ankern, dann Mannschaft umverteilen.
- Bei Schiffzerstörung werden Mannschaftsbefehle beendet. Hauptbesatzung stirbt weiterhin ausschließlich in diesem Fall. Verletzungen und individuelle Mannschaftstode werden in diesem Ausbau nicht neu eingeführt.

Die kleine HUD-Fläche zeigt drei Zeilen und wenige Presets; eine aufklappbare Detailansicht erklärt Effekte. F und die Anker-Schaltfläche funktionieren unabhängig von diesem Panel. Hilfe pausiert auch Ankerarbeit und Mannschaftstransit.

## 4. Sichtbarer Nebel über der Karte

Eine weiche, gemalte Wolkendecke liegt über unbekanntem Terrain. Sie bewegt sich langsam, hat sanfte Ränder und öffnet sich während des Fliegens um das Schiff. Bisher unbekannte Inseln treten aus der Wolke hervor, statt plötzlich ohne sichtbare Grenze aufzutauchen. Der Zeppelin, Feuerbogen und HUD bleiben lesbar.

Weltansicht und Minikarte verwenden dieselbe Erkundungsmaske. Jede Ebene hat einen eigenen Stand; ein neuer Seed setzt beide zurück. Ein bekannter Übergang verrät nicht automatisch das Terrain auf seiner anderen Seite.

| Gebiet | Weltansicht | Minikarte und Wissen |
|---|---|---|
| Unbekannt | Dichte gemalte Wolken verbergen Terrain und Ortsnamen. | Verhüllt; keine genauen Insel-, Hafen- oder Wrackpositionen. |
| Wird erkundet | Nebelrand öffnet sich innerhalb etwa 1,5–2,5 s. Direkt um das Schiff wird sofort ausreichend freie Sicht sichergestellt. | Nebel lichtet sich im selben Korridor; Orte werden beim tatsächlichen Entdecken kartiert. |
| Bereits erkundet | Dauerhaft lichter; gewöhnliche Wetterwolken bleiben als Atmosphäre. | Gelände und bekannte Orte bleiben kartiert. |

**Kartiertes Gelände ist keine permanente Feindaufklärung.** Piraten und andere bewegliche Kontakte benötigen aktuelle Sicht. Ein Sturm kann die aktuelle Sicht reduzieren, löscht aber keine Kartenerkenntnisse. Aus Kartenwissen darf kein Gegner außerhalb der Sensorreichweite sichtbar bleiben.

Navigation erweitert den Erkundungsradius und damit die pro Strecke gelichtete Fläche. Gute Besetzung findet Abzweige und Rohstoffe früher; eine kampforientierte Besetzung muss näher heranfliegen. Nur bisher bekannte Orte behalten ihre Namen. Gerüchte und Funkhinweise dürfen grobe Richtungen nennen, ohne unerkundetes Gelände vollständig aufzudecken.

Für die erste Umsetzung genügt eine weich überblendete, bemalte Nebelfläche mit Erkundungsmaske; echte volumetrische Simulation ist nicht erforderlich. Bewegung und Erkundung steuern verschiedene Aspekte: die Pinselwolken bewegen sich mit dem Wind, die bekannte Landkarte wandert nicht mit. Gelände- und Schiffscollider, Windschatten und KI bestehen unabhängig vom Sichtbarkeitszustand. Verborgene Inseln dürfen deshalb weder durchfliegbar werden noch nachträglich in ein Schiff hinein erscheinen.

## 5. Umsetzung und Abnahme des nächsten Ausbaus

1. Mannschaftsbegriff und HUD, nachvollziehbare aktuelle/Zielbesetzung sowie die Wirkungskurven. Die Verteilung 2 / 2 / 2 erhält die bisherige Flugabstimmung.
2. Haltemodus mit zeitabhängiger Arbeit, begrenzter Gegenkraft, Energieverbrauch, Windüberlastung und sauberem Ende vor dem Ebenenwechsel.
3. Gemeinsame Erkundungsmaske für Welt und Minikarte, weiche Lichtung, getrennte Ebenen und unabhängig aktive Physik.
4. Menschlich testen: Reise mit 4 / 2 / 0 gegen 2 / 2 / 2; beweglicher Kampf mit 1 / 1 / 4 gegen 2 / 2 / 2; geankert unter Beschuss lichten; Erkundung vor und nach Ebenenwechsel.

Zu prüfen: überall ankern ohne freie Höhenwechsel; kein Einfrieren gegnerischer Kräfte; mehrfach F drücken verkürzt keine Wartezeit; Crewwechsel erneuert keine Nachladearbeit und kein Reparaturbudget; sechs Personen bleiben sechs; Presets reservieren höchstens vier Plätze; Pause und Neustart räumen Ankerzustand auf; Nebel wirkt nie als Kollisionsschalter; kartierte Orte bleiben erhalten, versteckte Kontakte werden nicht verraten.

Die KI benötigt dieselben Stationsregeln und Sichtbeschränkungen. Die bestehenden Kampfzahlen werden erst nach dem Mannschaftstest weiter abgestimmt: Mehr Geschützleute ändern bereits die mögliche Kampfdauer. Ankern soll keine allgemein beste Kampfhaltung werden, und vier Navigatoren sollen auf langen Erkundungsreisen einen sichtbaren Vorteil gegenüber einer reinen Kampfverteilung bieten.
