# Search And Find

## Lernziele
- FUSEE's Szenengraph-Aufbau verstehen (s.a. 
  [Tutorial 05](https://github.com/griestopf/Fusee.Tutorial/blob/master/Tutorial05/_images/SceneHierarchy.png))
  - Szene: Header und Root-Child-Liste
  - Node (Aufbau der Hierarchie): Liste von Komponenten und Liste von Kind-Nodes 
  - Komponente (Container für Nutzdaten): unterschiedliche Typen für unterschiedliche 

- [Visitor-Pattern](https://de.wikipedia.org/wiki/Besucher_(Entwurfsmuster)) im allgemeinen verstehen.

- Implementierung des 
  [Visitor-Pattern](https://github.com/FUSEEProjectTeam/Fusee/blob/develop/src/Xene/SceneVisitor.cs) in FUSEE verstehen.
  - Visit-Methoden per `VisitMethodAttribute` ausgezeichnet.
  - Identifizierung von mit `[VisitMethod]` attributierten Methoden durch Reflection (Methode `ScanForVisitors`)
  - Speicherung der VisitMethod-Methoden als Dictionary von 
    [delegate](https://msdn.microsoft.com/de-de/library/ms173171.aspx).
  - Aufruf des passenden Delegates typ-abhängig in `DoVisitNode`

- Erzeugen und Ableiten eigener Visitor verstehen.


## Vorgehen

- Code von Search-And-Find herunterladen und zum Laufen bringen
- Szenengraph erweitern, so dass ein 


## Fingerübungen

- 