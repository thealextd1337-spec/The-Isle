# The Isle — Spielplanung

Stand: 11. Oktober 2026

Dieses Dokument hält die bisher gemeinsam entwickelte Spielidee fest. **Festgelegt** bezeichnet ausdrückliche Wünsche des Auftraggebers. **Vorschlag** bezeichnet aus diesen Wünschen abgeleitete Mechaniken, die noch abgestimmt und erprobt werden sollen. **Offen** bezeichnet Entscheidungen, die den weiteren Entwurf beeinflussen.

## 1. Spielidee und festgelegte Anforderungen

Der Spieler erkundet eine Welt aus Inseln über den Wolken. Sein Zeppelin ist Fahrzeug, Transportmittel und ausbaubares Zuhause. Als Inspirationsrichtung wurden Elite Dangerous, Handel, Piraten und Fraktionen genannt. Die Darstellung soll einen isometrischen Comic-Grafikstil nutzen.

Festgelegt sind:

- Gewählter Grafikstil: **B — Gemaltes Abenteuer** in isometrischer Darstellung.
- Erkundung und Abenteuer bilden einen Schwerpunkt.
- Der Spieler fliegt den Zeppelin selbst.
- Das Fliegen besitzt etwas Physik und spürbare Trägheit.
- Große Schiffe sind langsamer und benötigen mehr Energie.
- Gegenwind und Rückenwind beeinflussen das Fliegen; Wind ist Teil der Welt.
- Der Zeppelin wird modular ausgebaut.
- Die Besatzung spielt eine Rolle im Gameplay.
- Hauptbesatzung besitzt Fähigkeiten und kann ausschließlich bei kompletter Schiffzerstörung sterben. Jede Hauptrolle führt Unterbesatzung, deren Zuweisung das Tätigkeitstempo beeinflusst.
- Bereits die Demo enthält Breitseitenkampf. Schiffe schießen hauptsächlich seitlich; gleichwertige Kämpfe sollen mehrere Strategien erlauben und einige Zeit dauern.
- Sound gehört zur Demo.
- Handel, Piraten und Fraktionen gehören zum Konzept.
- Fog of War verbirgt die Welt, bis sie erkundet wird.
- Die Ausgangswelt soll bei jedem Spielstart anders sein.
- Es gibt mehrere Höhenebenen mit unterschiedlichen Gefahren und Rohstoffen.
- Der Wechsel zwischen Ebenen erfolgt an bestimmten Punkten der Welt. Ein Ebenenwechsel ist nicht an jedem beliebigen Ort möglich.
- Weltereignisse dürfen die Spielwelt während einer Partie verändern; konkrete Ereignisse sind zunächst Vorschläge.

Arbeitsannahme für diese Planung: Ein neu gestarteter Durchlauf erhält eine neue Ausgangswelt. Die genaue Bedeutung von „Spielstart“ beim Laden oder Fortsetzen eines bestehenden Spielstands bleibt offen. Es ist noch keine Entscheidung für ein Roguelike, dauerhafte Fortschritte zwischen Durchläufen oder Permadeath gefallen.

## 2. Angestrebtes Spielerlebnis — Vorschlag

Der Spieler soll sich als Kapitän seines eigenen Luftschiffs fühlen. Er liest Wind und Gelände, plant Expeditionen, entdeckt Verbindungen zwischen Ebenen und verändert seinen Zeppelin für die nächste Reise.

Vier Leitgedanken:

1. **Fliegen lohnt sich als Tätigkeit.** Trägheit, Wind und Beladung erzeugen Situationen, in denen Können zählt.
2. **Wissen erweitert die Möglichkeiten.** Eine entdeckte Passage oder günstige Windroute kann genauso wertvoll sein wie ein neues Modul.
3. **Ausbau verlangt Entscheidungen.** Frachtraum, Reichweite, Schutz, Erkundungsausrüstung und Mannschaft konkurrieren um Platz, Tragfähigkeit und Energie.
4. **Die Welt reagiert und verändert sich.** Ereignisse schaffen neue Aufgaben und lassen bekannte Routen unter anderen Bedingungen erscheinen.

Eine typische Entscheidung: Ein geborgenes Bauteil ist wertvoll, macht das Schiff aber schwerer. Der Rückweg führt durch Gegenwind und einen kontrollierten Ebenenübergang. Der Spieler muss zwischen zusätzlicher Beute, Energiereserve und einer anderen Route abwägen.

## 3. Gameplayloop — Vorschlag

### 3.1 Eine Expedition

1. **Vorbereiten:** Ziel oder Auftrag wählen, Waren kaufen, Module und Besatzung zuweisen, Energiereserve und mögliche Rückwege planen.
2. **Fliegen:** Wind nutzen, gegensteuern, Hindernisse umgehen und Energie sowie Zustand des Schiffs beobachten.
3. **Entdecken:** Inseln, Häfen, Rohstoffe, Wracks und Ebenenübergänge finden und kartieren.
4. **Entscheiden:** Eine Bergung wagen, auf Piraten reagieren, einen Übergang nutzen oder rechtzeitig umkehren.
5. **Anlegen:** Handeln, Aufträge abgeben, reparieren, Informationen beschaffen und Beziehungen pflegen.
6. **Ausbauen:** Module kaufen oder verbessern, Mannschaft ergänzen und das nächste Ziel festlegen.

Eine reine Handelsfahrt kann bekannte Häfen verbinden. Eine Erkundungsfahrt muss keinen Kampf enthalten. Chancen und Zwischenfälle können mehrmals auf derselben Reise auftreten.

### 3.2 Langfristige Entwicklung

Neue Kartenkenntnisse, Kontakte, Mannschaftsfähigkeiten und Schiffsausrüstung eröffnen weitere Wege. Der Spieler entwickelt eigene Handelsrouten und erreicht anspruchsvollere Regionen.

Entdeckungsbelohnungen können sein:

- Ein neuer Hafen oder sicherer Ankerplatz.
- Eine Verbindung in eine bisher unerreichbare Ebene.
- Wertvolle Rohstoffe, Bergungsgut oder besondere Bauteile.
- Eine energiesparende Windroute.
- Ein Kontakt, Bauplan oder neuer Auftrag.
- Verkaufbare Karteninformationen.

## 4. Fliegen, Wind und Energie

### Festgelegte Grundlage

Der Spieler steuert direkt. Das Schiff besitzt Trägheit. Große Schiffe sind langsamer und energiehungriger. Gegenwind und Rückenwind müssen wahrnehmbar sein.

### Ausgestaltung — Vorschlag

- Motorschub baut Geschwindigkeit allmählich auf. Nach dem Wegnehmen des Schubs gleitet das Schiff weiter; Gegenschub ermöglicht stärkeres Bremsen.
- Masse beeinflusst Beschleunigung, Bremsweg und Drehverhalten. Fracht verändert das Fluggefühl während der Reise.
- Rückenwind hilft beim Vorankommen und erlaubt geringeren Motorschub. Gegenwind verlangt mehr Schub; bei unzureichender Leistung sinkt die Geschwindigkeit relativ zu den Inseln.
- Seitenwind verursacht Drift. Böen und Windschatten machen Passagen und Hafenanflüge abwechslungsreich.
- Schwere Schiffe reagieren weniger abrupt auf Böen, korrigieren ihren Kurs anschließend aber langsamer.
- Die Bedingungen unterscheiden sich zwischen Regionen und Höhenebenen. Der Wechsel in eine andere Ebene kann eine günstigere Route eröffnen.
- Hauptverbraucher sind Motoren und ausgewählte aktive Systeme. Die genaue Energiequelle und Versorgung werden noch festgelegt.
- Ballon- und Auftriebsmodule bestimmen die Tragfähigkeit. Lokale Höhenbewegungen können zum Fluggefühl gehören; sie eröffnen keinen beliebigen Wechsel zwischen Welt-Ebenen.

Wind soll anhand von Wolkenbewegung, Fahnen, Partikeln und Instrumenten lesbar sein. Energiereserve, Schäden und Überlastung brauchen verständliche Rückmeldungen. Eine Fahrt soll mit einfachen Eingaben möglich sein, während saubere Manöver Übung belohnen.

## 5. Höhenebenen und feste Übergänge

### Ebenen — vorläufiger Vorschlag

- **Sonneninseln:** Gute Sicht, belebte Häfen und erste Handelsrouten. Häufige Güter könnten Holz, Nahrung und Fasern sein.
- **Sturmgürtel:** Dichte Wolken, starke Strömungen und versteckte Häfen. Erze und leitfähige Kristalle bieten neue Ausbauchancen.
- **Wolkentiefe:** Schwierige Navigation, wenige sichere Ankerplätze und alte Ruinen. Seltene Legierungen und verlorene Technik belohnen Vorbereitung.

Namen, Anzahl und genaue Rohstoffe sind offen. Der Entwurf nimmt vorläufig an, dass die Gefahr in tieferen Ebenen zunimmt.

### Übergangspunkte — festgelegt, Ausgestaltung als Vorschlag

Nur bestimmte Orte verbinden die Ebenen. Mögliche Formen sind Wolkenschächte, starke vertikale Strömungen oder Anlagen aus einer früheren Epoche.

- Ein Übergang wird entdeckt und auf der Karte vermerkt.
- Der Spieler erreicht ihn durch reguläres Fliegen und steuert die Passage selbst.
- Ein- und Ausgänge haben feste Positionen. Eine Passage kann zwei Ebenen verbinden oder Teil einer längeren Verbindungskette sein.
- Wind, Sicht und Anforderungen an das Schiff unterscheiden die Passagen.
- Fraktionen können Gebühren erheben, kontrollieren oder Schutz anbieten. Piraten können die Zu- und Abflugrouten bedrohen.
- Mehrere Verbindungen zwischen denselben Ebenen ermöglichen Umwege und geheime Handelsrouten.
- Wetter oder Weltereignisse können einzelne Verbindungen zeitweise verändern. Das ist ein zusätzlicher Vorschlag, keine bereits festgelegte Pflicht für jeden Übergang.
- Neu entstehende Übergänge bleiben an konkrete Orte gebunden.

Die Weltgenerierung und Ereignislogik sollen erreichbare Versorgung und eine mögliche Rückkehr berücksichtigen. Eine Passage darf nicht ohne ausreichende Hinweise eine praktisch ausweglose Reise erzeugen.

## 6. Fog of War, Karten und neue Ausgangswelten

### Erkundung — Vorschlag

- **Unbekannt:** Die Region ist verborgen. Gerüchte können eine grobe Richtung nennen, ohne genaue Ziele preiszugeben.
- **In Sichtweite:** Inseln und aktuelle Schiffe werden sichtbar. Wetter, Instrumente und Navigator beeinflussen die Sichtweite.
- **Kartiert:** Entdeckte Orte bleiben als Wissen erhalten. Aktuelle Preise, Wind, Piraten und Kontrollverhältnisse müssen durch Beobachtung oder Nachrichten aktualisiert werden.

Jede Ebene besitzt ihren eigenen Erkundungsstand. Das Auffinden eines Übergangs deckt die Zielregion noch nicht automatisch auf.

Wenn Ereignisse bekannte Orte verändern, zeigt die Karte den zuletzt bekannten Zustand und gegebenenfalls einen Hinweis auf veraltete Informationen. Veränderungen werden durch eigene Beobachtung oder glaubwürdige Meldungen bestätigt. Entdecktes Wissen soll nachvollziehbar bleiben.

### Prozedurale Ausgangswelt — Vorschlag zur Umsetzung des festgelegten Wunschs

Eine neue Partie variiert Inselanordnung, Höhenlagen, Rohstoffvorkommen, Häfen, Übergänge, Fraktionsgebiete, Piratenstützpunkte und vorherrschende Windströmungen.

Die Generierung behandelt Flugräume und Ebenenverbindungen gemeinsam. Versorgungshäfen, erste Handelsmöglichkeiten und lohnende Entdeckungen sollen zur Reichweite des Startschiffs passen. Handgestaltete besondere Inseln können an unterschiedlichen Stellen in der erzeugten Welt auftauchen.

Für fortsetzbare Partien ist vorgeschlagen, Ausgangswelt, Ereignisfolgen und Entdeckungsstand zu speichern. Ob bereits das erneute Laden eine neue Welt erzeugen soll, wird vor der Umsetzung ausdrücklich entschieden.

## 7. Zeppelin, Module und Besatzung — Vorschläge

### Module

Mögliche Module sind Frachträume, Motoren, Auftriebskörper, Energiereserven, Panzerung, Geschütze, Wolkenscanner, Bergungswinden, Werkstätten und Mannschaftsquartiere.

Jedes Modul erhält verständliche Eigenschaften: Funktion, Gewicht, Platzbedarf, Energiebedarf und gegebenenfalls Personalbedarf. Ausbauten verändern die sichtbare Silhouette des Zeppelins.

Handels-, Expeditions- und bewaffnete Schiffe entstehen durch die Kombination von Modulen. Kleine Schiffe bleiben durch niedrige Betriebskosten, Wendigkeit und die Erreichbarkeit enger Ankerplätze nützlich.

### Besatzung

Besatzung ist festgelegt. Folgende Rollen sind Vorschläge:

- **Maschinist:** Wartung, Reparatur und kurzzeitiger zusätzlicher Schub.
- **Navigator:** Wetterwarnungen, Hinweise auf Windrouten und bessere Erkundung.
- **Geschützbesatzung:** Waffen bedienen, während der Spieler manövriert.
- **Quartiermeister:** Vorräte, Fracht, Handel und Bergung organisieren.

Umzuweisungen schaffen konkrete Entscheidungen. Wird eine Person zur Reparatur gebraucht, steht sie vorübergehend für ihre andere Aufgabe nicht zur Verfügung.

Erfahrung und Spezialisierungen könnten die Mannschaft langfristig entwickeln. Namen, Eigenheiten, Beziehungen, Löhne und Versorgungsbedarf sind noch abzustimmen. Eine umfangreiche Simulation des Bordlebens ist bisher nicht beschlossen.

## 8. Handel, Fraktionen und Piraten — Vorschläge

### Handel

Inseln produzieren bestimmte Waren und benötigen andere. Preise sollen durch nachvollziehbare lokale Versorgung und Nachfrage entstehen. Ereignisse können diese Bedingungen verändern. Hinweise auf die Ursache einer Knappheit helfen beim Planen.

Handelsgewinn muss Reiseaufwand, Energie, mögliche Gebühren und Reparaturen berücksichtigen. Eine bekannte Route darf verlässlich sein; bessere Informationen und veränderte Bedingungen können zusätzliche Chancen eröffnen.

Weitere Einnahmen könnten Aufträge, Bergung, Erkundungsprämien und der Verkauf von Kartenwissen sein. Wer Informationen verkauft, erschließt möglicherweise Konkurrenz die eigene Route.

### Fraktionen

Vorläufige Beispiele sind eine Handelsliga, eine Luftflotte und freie Inselgemeinschaften. Ruf könnte Preise, Hafenrechte, Aufträge, Passagegebühren und Unterstützung beeinflussen.

Fraktionen verfolgen lokale Interessen. Ihr Einfluss wird an Häfen, Patrouillen und Übergangskontrollen sichtbar. Ein Auftrag kann einer Fraktion helfen und die Interessen einer anderen beeinträchtigen.

### Piraten

Piraten bedrohen wertvolle Fracht, schwach geschützte Routen und Übergänge. Der Spieler kann ausweichen, fliehen, verhandeln, Fracht abwerfen oder kämpfen.

Ob der Spieler selbst Piraterie betreiben kann, ist offen. Falls diese Rolle gewünscht ist, braucht sie passende Abnehmer, Rufänderungen und Folgen für Hafen- und Passagerechte.

## 9. Weltereignisse — konkrete Vorschläge

Alle Ereignisse in diesem Abschnitt sind Vorschläge. Sie sollen neue Entscheidungen erzeugen und den Zustand der Welt tatsächlich beeinflussen. Dauer, Häufigkeit und Schwere werden erst durch Erprobung festgelegt.

### 9.1 Wandernde Sturmfront

**Vorzeichen:** Wetterstationen melden fallenden Luftdruck; Wolken verdichten sich und Windrichtung sowie Böen verändern sich.

**Weltänderung:** Eine Front wandert durch mehrere Regionen. Flugrouten werden schwieriger, ein Übergang kann vorübergehend gefährlich oder gesperrt sein. Betroffene Häfen benötigen anschließend Reparaturmaterial.

**Spielmöglichkeiten:** Eine geschützte Route nehmen, günstig mit der Front reisen, einen alternativen Übergang suchen, Hilfsgüter liefern oder beschädigte Schiffe bergen.

**Dauer und Folgen:** Der Sturm zieht weiter; beschädigte Infrastruktur und neue Nachfrage können länger bestehen. Die Welt wird über Bedingungen und lokale Schäden verändert.

### 9.2 Inselwanderung

**Vorzeichen:** Bewohner melden veränderte Ankerkräfte; Messgeräte zeigen, dass sich eine Insel langsam verschiebt.

**Weltänderung:** Eine Insel wandert innerhalb ihrer Ebene. Entfernungen, Windschatten und Handelswege verändern sich. Zugehörige Gebäude und ein angedocktes Schiff bewegen sich mit der Insel.

**Spielmöglichkeiten:** Die Bewegung vermessen, aktualisierte Karten verkaufen, Vorräte liefern oder frühzeitig eine neue Handelsverbindung nutzen.

**Dauer und Folgen:** Die Verschiebung geschieht sichtbar über Zeit und kann in einer dauerhaften neuen Position enden. Ein bestehender Übergang bleibt zunächst an seinem eigenen Ort, sofern seine Bindung an die Insel nicht ausdrücklich Teil des Ereignisses ist.

### 9.3 Neuer Wolkenschacht

**Vorzeichen:** Ungewöhnliche vertikale Strömungen, Wolkenrisse und widersprüchliche Berichte aus einem abgelegenen Gebiet.

**Weltänderung:** An einem bestimmten Ort entsteht eine neue Verbindung zwischen zwei Ebenen. Eine bisher abgelegene Region erhält einen kürzeren Zugang.

**Spielmöglichkeiten:** Die Passage zuerst erkunden, ihre Anforderungen vermessen, Informationen verkaufen oder eine neue Handelsroute etablieren.

**Dauer und Folgen:** Eine natürliche Passage kann zeitlich begrenzt sein. Eine freigelegte alte Anlage könnte nach Reparatur dauerhaft nutzbar werden. Für den ersten Entwurf sollte eine Variante gewählt werden.

### 9.4 Fraktionsblockade

**Vorzeichen:** Ein Streit um Abgaben oder Rohstoffe eskaliert; Patrouillen sammeln sich und Häfen verbreiten Warnungen.

**Weltänderung:** Eine Fraktion kontrolliert einen Hafen oder Übergang stärker. Bestimmte Güter werden zurückgehalten, die Versorgung verschlechtert sich und Preise steigen.

**Spielmöglichkeiten:** Erlaubte Waren liefern, eine Ausnahmegenehmigung beschaffen, Hilfslieferungen übernehmen, eine alternative Passage nutzen oder einer Seite helfen.

**Dauer und Folgen:** Verhandlungen, Nachschub oder ein Machtwechsel können die Blockade beenden. Der Ausgang beeinflusst Ruf, Kontrolle und künftige Gebühren.

### 9.5 Piratenkoalition

**Vorzeichen:** Mehrere Schiffe bleiben aus; Überlebende berichten von gemeinsam operierenden Piraten.

**Weltänderung:** Überfälle konzentrieren sich auf einen wichtigen Übergang. Händler wählen Umwege; entlegene Häfen werden schlechter versorgt. Ein versteckter Piratenstützpunkt gewinnt Bedeutung.

**Spielmöglichkeiten:** Konvois begleiten, Informationen sammeln, eine sichere Umfahrung entdecken, Schutz aushandeln oder an der Bekämpfung des Stützpunkts teilnehmen.

**Dauer und Folgen:** Die Koalition kann zerfallen, verdrängt oder gestärkt werden. Sichere Routen und lokale Machtverhältnisse ändern sich entsprechend.

### 9.6 Rohstofffund und Goldrausch

**Vorzeichen:** Ein Sturm legt eine Lagerstätte frei, ein Prospektor kehrt mit einer Probe zurück oder ungewöhnliche Fracht taucht in einem Hafen auf.

**Weltänderung:** Ein bisher unbedeutender Ort zieht Händler und Fraktionen an. Ausrüstung und Vorräte werden knapp; später kann das Angebot des neuen Rohstoffs stark wachsen.

**Spielmöglichkeiten:** Den Fund erkunden, Versorgung liefern, Rohstoffe transportieren, Informationen zurückhalten oder Fraktionen beim Aufbau einer Förderstelle unterstützen.

**Dauer und Folgen:** Ausbeutung, Erschöpfung oder Kontrolle verändern die Bedeutung des Orts. Ein Versorgungsposten kann dauerhaft entstehen. Die konkrete Rohstoffgewinnung ist noch zu entwerfen.

### 9.7 Gründung eines neuen Hafens

**Vorzeichen:** Siedler suchen Transporthilfe, eine Fraktion sucht einen Außenposten oder Händler benötigen einen sicheren Zwischenhalt.

**Weltänderung:** Durch mehrere Lieferungen entsteht ein Hafen mit Versorgung, Reparatur und Handel. Ein bisher zu langer Reiseabschnitt wird für kleinere Schiffe praktikabel.

**Spielmöglichkeiten:** Bauwaren und Personal liefern, die Versorgung organisieren, den Standort vermessen oder das Vorhaben gegen Überfälle unterstützen.

**Dauer und Folgen:** Der Hafen bleibt bestehen. Unterstützer beeinflussen seine Ausrichtung und Beziehungen. Spielerhandlungen können den Ausgang sichtbar verändern.

### 9.8 Inselbruch und Evakuierung

**Vorzeichen:** Risse, kleinere Abbrüche und Hilferufe kündigen die Gefahr an.

**Weltänderung:** Teile einer Insel brechen auseinander. Ein Hafen muss evakuiert werden; neue Trümmer, Wracks und kleinere Inselteile entstehen.

**Spielmöglichkeiten:** Menschen und Waren transportieren, kritische Ausrüstung retten, Bergung durchführen oder beim Aufbau eines Ersatzhafens helfen.

**Dauer und Folgen:** Die Geografie verändert sich dauerhaft. Dieses aufwendige Ereignis ist ein Kandidat für eine spätere Ausbaustufe. Der Spieler braucht rechtzeitig erkennbare Fluchtmöglichkeiten.

### 9.9 Aufstand und Machtwechsel

**Vorzeichen:** Versorgungsprobleme, hohe Abgaben und lokale Unruhen führen zu Hilfs- und Propagandaaufträgen.

**Weltänderung:** Die Kontrolle über einen Hafen oder Übergang wechselt. Warenregeln, Gebühren und Beziehungen ändern sich. Bestehende Aufträge brauchen nachvollziehbare neue Bedingungen.

**Spielmöglichkeiten:** Versorgung sichern, vermitteln, eine Seite unterstützen oder während der Unruhe andere Routen nutzen.

**Dauer und Folgen:** Der Ausgang bleibt als politischer Zustand bestehen. Die Rolle des Spielers kann Unterstützung, Misstrauen oder Zugang zu neuen Aufträgen erzeugen.

## 10. Gemeinsame Regeln für Ereignisse — Vorschlag

### Ablauf

Ein Ereignis durchläuft **Vorzeichen**, **aktive Phase**, **Auflösung** und gegebenenfalls **bleibende Folgen**. Nachrichten an Häfen, sichtbare Veränderungen und Besatzungshinweise machen den Ablauf verständlich.

Ereignisse sind regional verankert. Sie dürfen zufällig beginnen, aus einem Weltzustand entstehen oder durch Spielerhandlungen ausgelöst werden. Voraussetzungen müssen zum Ort passen: Eine Blockade benötigt eine kontrollierbare Verbindung; eine Hafengründung benötigt einen geeigneten Standort.

### Wirkung auf den Spielzustand

Ein Ereignis verändert konkret benannte Zustände: Wind, Position eines Orts, Erreichbarkeit einer Passage, Besitzer eines Hafens, Versorgung, Nachfrage oder vorhandene Infrastruktur. Nach dem Ende werden vorübergehende Effekte aufgehoben und dauerhafte Folgen beibehalten.

Ereignisse können zusammenwirken. Eine Sturmfront verursacht Schäden, Reparaturbedarf schafft einen Auftrag und eine erfolgreiche Lieferung hilft einem Hafen. Die Zahl gleichzeitig wirkender Ereignisse wird begrenzt, damit Ursachen und Handlungsmöglichkeiten lesbar bleiben.

### Erkundung und Fairness

- Ereignisse in unentdeckten Regionen werden über Gerüchte oder Meldungen angekündigt, ohne die gesamte Region aufzudecken.
- Bereits kartierte Orte behalten ihre Entdeckungshistorie. Aktuelle Änderungen werden als unsicher oder bestätigt unterschieden.
- Ein Ereignis soll mehrere sinnvolle Reaktionen zulassen, etwa Umweg, Vorbereitung, Hilfeleistung oder Rückzug.
- Einschränkungen wichtiger Verbindungen benötigen erkennbare Vorzeichen und praktikable Alternativen oder eine lokale Versorgungslösung.
- Gleichzeitige Ereignisse sollen nicht sämtliche lebenswichtigen Rückwege und Versorgungspunkte einer Region unvorhersehbar ausschalten.
- Belohnung und tatsächliche Gefahr müssen zusammenpassen. Wetter und Blockaden sollen verständliche Entscheidungen ermöglichen.

Bei fortsetzbaren Partien werden der aktuelle Ereigniszustand und seine Folgen gespeichert. Zeitfortschritt bei pausiertem oder geschlossenem Spiel ist noch nicht festgelegt.

## 11. Grafik und Lesbarkeit — Vorschlag

Festgelegt ist der isometrische Comic-Grafikstil **B — Gemaltes Abenteuer**. Die [Designrichtungen](grafikstil.md) und ihre Originalbilder sind Bestandteil der Planung; B ist die gewählte Referenz. Gemalte Wolken, warme Beleuchtung und sanfte Materialtexturen prägen die weitere visuelle Entwicklung.

Markante Silhouetten sollen Schiffsgrößen, Modularten und Fraktionszugehörigkeit lesbar machen. Inselkulturen erhalten eigene Bauformen und Farben.

Wolken vermitteln Höhe und verbergen unbekannte Regionen. Sichthindernisse, Gefahr und rein dekorative Wolken müssen unterscheidbar sein. Ein Ebenenwechsel sollte visuell verständlich bleiben, auch wenn die Kamera anschließend eine andere Ebene zeigt.

Wichtige Informationen sind Windrichtung, Energiereserve, Zustand, Zuladung, Mannschaftsaufgaben, aktuelles Reiseziel und bekannte Übergänge. Die konkrete Benutzeroberfläche wird erst mit dem Flugprototyp entworfen.

## 12. Fehlschläge und Erholung — Vorschlag

Notlandungen, beschädigte Module, verlorene Fracht und Abschleppkosten können eine misslungene Reise in neue Aufgaben überführen. Rettungspunkte oder Hilferufe sollen eine erkennbare Chance zur Erholung geben.

Hauptbesatzung kann ausschließlich bei vollständiger Schiffzerstörung sterben. Treffer oder Modulverlust töten keine Hauptperson. Rettungswahrscheinlichkeit, Unterbesatzungsverluste und langfristige Folgen bleiben offen. Der erste Teststand verliert Hauptpersonen bei Rumpf-HP 0 und erlaubt danach einen neuen Durchlauf. Ob Fortschritte über Durchläufe hinweg bestehen bleiben, ist noch offen.

## 13. Erste Demo — erste Unity-Umsetzung

Ziel ist, eine vollständige Reise in etwa 20–30 Minuten erlebbar zu machen: Waren kaufen, selbst fliegen, Wind nutzen, eine Passage entdecken, in die zweite Ebene gelangen, eine Gelegenheit wahrnehmen, einen Hafen erreichen und den Erlös in einen Ausbau investieren. Der genaue Ablauf und die Abnahmekriterien stehen in [Inhalt und Ablauf der ersten Demo](erste-demo.md).

### Begrenzter Inhalt

- Zwei Höhenebenen mit fünf Inseln und zwei Versorgungshäfen.
- Zwei feste Ebenenübergänge als Haupt- und Alternativroute.
- Eine einfache neue Insel- und Routenanordnung pro neuem Durchlauf.
- Fog of War mit dauerhaft kartierten Orten innerhalb der laufenden Partie.
- Ein Startzeppelin, direkte Steuerung, Trägheit, Wind und Energieverbrauch.
- Wenige Handelswaren und drei austauschbare Module, beispielsweise Frachtraum, zusätzlicher Motor und Bergungswinde.
- Navigator, Maschinist und Geschützmeister mit sechs verteilbaren Unterbesatzungsmitgliedern.
- Eine Piratenbegegnung mit Breitseitenkampf und Rückzug, plus separat wiederholbares gleichwertiges Duell. Verhandlungen folgen später.
- Zwei Hafenfraktionen mit unterschiedlichen Preisen; Ruf- und Zugangsentscheidungen folgen später.
- Begrenzte Feldreparatur mit gemeinsamem Budget sowie bezahlter Hafenservice.
- Neun originale synthetisierte Sounddateien, Lautstärke und Stummschaltung.

Die konkrete Implementierung und ihre Grenzen stehen in [Erste Demo](erste-demo.md), technische Evidenz in [Validierung](demo-validierung.md). Unity 6000.6.4f1 mit URP 17.6.0 wird verwendet; Windows-Einzelspieler ist die Arbeitsannahme für diesen ersten Stand. Die gemalte Darstellung ist ein funktionaler Grafikprototyp und noch nicht auf dem Qualitätsniveau der ausgewählten Konzeptzeichnung.

### Weltereignis und spätere Erweiterung

Die erste Demo enthält **eine Sturmfront**. Sie verändert den Wind, beeinflusst eine Passage und erzeugt Reparaturbedarf an einem Hafen. Eine erreichbare Alternative ermöglicht die Rückreise.

Weitere Ausbaustufen können Rohstofffunde und Fraktionsblockaden ergänzen. Inselwanderung, Inselbruch und dauerhafte Hafengründung folgen später. Die erste Demo prüft zunächst den vollständigen Reiseablauf mit einem verständlichen Ereignis.

### Fragen, die der Prototyp beantworten soll

- Ist das Fliegen schon ohne Auftrag angenehm und beherrschbar?
- Ändern Wind und Beladung die Entscheidungen tatsächlich?
- Ist ein erreichter Ebenenübergang eine interessante Entdeckung?
- Kann der Spieler Energiebedarf und Rückweg ausreichend einschätzen?
- Entstehen verschiedene sinnvolle Routen statt einer immer besten Lösung?
- Ist der Beitrag der Besatzung während der Reise sichtbar?
- Verändert ein Ausbau die nächste Reise spürbar?
- Schafft ein Ereignis neue Möglichkeiten und verständliche Konsequenzen?

## 14. Offene Entscheidungen

1. **Spielstart und Fortsetzen:** Neue Welt nur beim Beginn eines neuen Durchlaufs oder auch beim erneuten Laden? Welche Fortschritte bleiben erhalten?
2. **Spielmodus:** Einzelspieler oder Mehrspieler; langfristige Kampagne oder kürzere Durchläufe?
3. **Weitere Plattformen:** Die Demo verwendet Unity und Windows mit Tastatur/Maus; zusätzliche Plattformen und Controller sind offen.
4. **Besatzungstiefe:** Persönliche Figuren mit Beziehungen oder zunächst Rollen, Fähigkeiten und Aufgaben?
5. **Weitere Piraterie:** Breitseitenkampf ist Teil der Demo. Entern, Verhandeln und selbst als Pirat handeln bleiben offen.
6. **Schadensfolgen:** Rettung, Wiederbeschaffung, Verlust von Schiff und Mannschaft, mögliche dauerhafte Konsequenzen?
7. **Versorgung:** Energiequelle, Nachladen oder Betanken, Reparaturmaterial und laufende Mannschaftskosten?
8. **Erkundungsinteraktion:** Was tut der Spieler konkret an Inseln, Ruinen und Lagerstätten — etwa scannen, bergen, handeln oder besondere Flugmanöver ausführen?

Der Grafikstil B, die Besatzungshierarchie und seitlicher Kampf sind festgelegt. Der erste Unity-Stand setzt den Reiseablauf einschließlich Kampf und Sound um. Neue Reise bedeutet neue Seed-Welt; Speichern/Fortsetzen, individuelle Besatzungsentwicklung und langfristige Verlustregeln werden später entschieden.
