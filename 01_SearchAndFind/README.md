# Search And Find

## Lernziele
- FUSEE's Szenengraph-Aufbau verstehen (s.a. 
  [Tutorial 05](https://github.com/griestopf/Fusee.Tutorial/blob/master/Tutorial05/_images/SceneHierarchy.png))
  - Szene: Header und Root-Child-Liste
  - Node (Aufbau der Hierarchie): Liste von Komponenten und Liste von Kind-Nodes 
  - Komponente (Container für Nutzdaten): unterschiedliche Typen für unterschiedliche Bestandteile.

- Verwendung der [Such-Funktionalität](https://github.com/FUSEEProjectTeam/Fusee/blob/develop/src/Xene/SceneFinder.cs) in FUSEE's Szenengraphen verstehen.

- Definieren von Match-Kriterien als Lambdas verstehen.

- Implementierung der Suchfunktionalität in FUSEE verstehen und nachvollziehen können.

- [Visitor-Pattern](https://de.wikipedia.org/wiki/Besucher_(Entwurfsmuster)) im allgemeinen verstehen.

- Implementierung des 
  [Visitor-Pattern](https://github.com/FUSEEProjectTeam/Fusee/blob/develop/src/Xene/SceneVisitor.cs) in FUSEE verstehen.
  - Visit-Methoden per `VisitMethodAttribute` ausgezeichnet.
  - Identifizierung von mit `[VisitMethod]` attributierten Methoden durch Reflection (Methode `ScanForVisitors`)
  - Speicherung der VisitMethod-Methoden als Dictionary von 
    [delegate](https://msdn.microsoft.com/de-de/library/ms173171.aspx).
  - Aufruf des passenden Delegates typ-abhängig in `DoVisitNode`

- Erzeugen und Ableiten eigener Visitor verstehen.

- `SceneFinderBase` als Ableitung des Visitor

- Ableitungen von `SceneFinderBase` für Nodes und Components

- Extension-Methods für Listen von Nodes und Components verstehen.

- IEnumerator<T> vs. List<T> verstehen.

## Vorgehen

### Roboterszene vervollständigen

- Code von Search-And-Find herunterladen und zum Laufen bringen
- Szenengraph erweitern, so dass ein Cube-Roboter wie in Tutorial 02 entsteht (aber mit FUSEE-Szenengraph-Bausteinen):
  ![Bild vom Robi](https://github.com/griestopf/Fusee.Tutorial/blob/master/Tutorial03/_images/Robot.png)
  ggf. Verwenden von mehreren Farben.
- Durch Einfügen von leeren Hierarchie-Stufen mit geeigeten Transformationen Rotations-Achsen (Pivot-Points) an sinnvolle Stellen setzen.
- Szenengraph-Bestandteile mit Namen versehen.
- Verwenden der eingebauten Suchfunktionalität, um die richtigen Rotations-Objekte zu finden
- Verknüpfen von Rotationen mit Benutzer-Eingaben zur Steuerung des Roboter.
  
### Notwendige Bestandteile selbst implmentieren

- Eigene Traversierungs-Funktionalität zum Suchen nach Namen, sowie zum Rendern bauen
- Gemeinsamen Basis-Traversierungs-Algorithmus für Suchen nach Namen UND Rendern implementieren -> Das Visitor Pattern
- Suche auf beliebige Kriterien ausdehnen -> Delegates
- Suche als Extension-Method auf allen sinnvollen Ebenen

## Fingerübung

- In der Datei [SimpleMeshes.cs](Core/SimpleMeshes.cs) die bereits angelegten Methoden für andere 3D-Körper implementieren
