
# Async-Example

Dieses Code-Beispiel demonstriert die Notwendigkeit, lange dauernde Prozesse in nebenläufige Aktivitäten
auszulagern, um ein ruckelfreies Rendering zu gewährleisten. Es zeigt zwei der drei verschiedenen
Patterns, die in C#/.NET existieren: 

- Asynchronous Programming Model (APM)
- Event-based Asynchronous Pattern (EAP)
- Task-based Asynchronous Pattern (TAP)

Siehe auch [Asynchronous Programming Patterns](https://msdn.microsoft.com/en-us/library/jj152938.aspx)
im MSDN.


## Bespiel-Applikation

Das Code-Beispiel enthält eine FUSEE-Applikation, die eine Zeichenkette in einer Art Ticker über den
Bildschirm laufen lässt. Gleichzeitig kann ein 3D-Modell interaktiv bewegt werden.

Ein Button in der oberen linken Fensterecke lädt eine Textdatei mit langem Inhalt (Shakespear's gesammelte
Werke) aus dem Web, um diesen, sobald er geladen wurde, im Ticker anzuzeigen.

In der Datei [Main.cs](X04_Async/Desktop/Main.cs#63) des Desktop-Build ist ab Zeile 63 die Implementierung des Ladens
der Textdatei als Reaktion auf den Button-Click in drei Varianten implementiert.


### Synchron

Die erste Implementierung ist ein synchroner Download, d.h. Sobald der Button geklickt wurde, 
wird der Download gestartet und erst wenn der Text vollständig heruntergeladen wurde, fährt die
Anwendung mit dem Rendern der visuellen Inhalte fort. Während des Ladevorgangs scheint die Applikation
eingefroren. Die Klasse `WebClient` erlaubt das synchrone Herunterladen einer Textdatei und
Zurückliefern in Form eines Strings mit der Methode `DownloadString`

```C#
    WebClient client = new WebClient();

    string fileContents = client.DownloadString(new Uri("http://www.fusee3d.org/Examples/Async/SomeText.txt"));
    app.Ticker.CompleteText = fileContents;
```

Wenn ein Benutzer, z.B. mit den 
Pfeiltasten, das Modell dreht und gleichzeitig das Laden des Textes per Button initiiert,
wird die Bewegung des 3D-Modells für die Zeit des Herunterladens angehalten. 

Gerade in stark interaktiven Umgebungen wie Echtzeit 3D-Visualisierungen sollte das fortwährende
Rendern unbedingt gewährleistet sein. Daher sollten zeitintensive Aktionen, wie das Laden von Daten
oder umfangreiche Berechnungen, in nebenläufige Threads ausgelagert werden.

Neben der Möglichkeit, direkt mit den vom Betriebssystem zur Verfügung gestellten Threads
zu arbeiten, bietet C# eine Reihe von Möglichkeiten an, mit programmiertechnsich einfacherer
Herangehensweise Nebenläufigkeit (die hier auch oft _Asynchronizität_ genannt wird), zu erzeugen. 
Die drei Möglichkeiten sind in oben genanntem Artikel beschrieben und bieten Erleichterung
vor allem für das Starten nebenläufiger Aktionen, sowie das Ausführen von Code 
_nach dem Beenden_ nebenläufiger Aktionen (bei denen meist auf das Ergebnis der
nebenläufigen Aktion zugegriffen werden soll). Für klassische Programmier-Beispiele für Nebenläufigkeit,
bei der es oft um eine _Synchronisierung_ der nebenläufigen Aktionen geht, wie z.B. das 
[Dining Philosophers Problem](https://de.wikipedia.org/wiki/Philosophenproblem),
werden von diesen Patterns nicht auf spezielle Art unterstützt. Diese Art der Nebenläufigkeit 
muss auch in C# mit den 
[klassischen Mitteln der Thread-Synchronisierung](https://msdn.microsoft.com/de-de/library/ms228964(v=vs.110).aspx)
 gelöst werden.

Ein Großteil von der .NET-Library zur Verfügung gestellten Funktionalität wird in einem oder 
mehreren der o.g. Patterns bereitgestellt, wenn diese Funktionalität eine lange Auführungszeit
benötigt. Das im Beispiel verwendete Herunterladen einer Textdatei erfolgt mit 
der [WebClient](https://msdn.microsoft.com/de-de/library/system.net.webclient(v=vs.110).aspx)
Klasse.


### Asynchronous Programming Model (APM)

Das APM ist historisch zu betrachten und wird nicht mehr unterstützt, daher ist es in diesem
Code-Beispiel auch nicht implementiert. APM-Methoden sind daran erkennbar, dass für eine
Aktion jeweils ein Paar von Methoden implmentiert ist, das dem Namensschema
`BeginAktion()` und `EndAktion()` folgt, wobei die `Begin...()`-Methode ein Objekt zurückgibt, 
das das Interface `IAsyncResult` implmenentiert.


### Event-based Asynchronous Pattern (EAP)

Bei diesem Pattern gibt es für jede Aktion eine Methode, die dem Namenssschema `AktionAsync()`
folgt. Zudem gibt es für jede Aktion ein `event` mit dem Namensschema `AktionCompleted`.

Die Methode kann vom Anwender zum Starten der asynchronen Operation verwendet werden. Dabei
wird die Methode ohne Verzögerung wieder verlassen und der aufrufende Kontext kann ohne
für Anwender spürbare Verzögerungen mit dem Programmablauf fortfahren, während die Aktion
im Hintergrund _asynchron_ durchgeführt wird.

Code, der ausgeführt werden soll, sobald die Aktion beendet ist (z.B. um ein Rechen- oder
Download-Ergebnis zu verwenden), kann als _Callback-Methode_ bzw. _Event-Listener_ an den
Event angehängt werden. Der Event verlangt eine Methdoe, der als Parameter ein Objekt
vom Typ `AktionCompletedEventArgs` übergeben werden kann. In diesem Objekt kann dann
aus der Eventbearbeitungsmethode auf das Ergebnis der Aktion zugegriffen werden.

Die Klasse `WebClient` erlaubt das
asynchrone Herunterladen einer Textdatei und Zurückliefern in Form eines Strings nach dem 
EA-Pattern mit der Methode `DownloadStringAsync` 

```C#
    WebClient client = new WebClient();

    client.DownloadStringCompleted += delegate(object o, DownloadStringCompletedEventArgs eventArgs)
    {
        app.Ticker.CompleteText = eventArgs.Result;
    };
    client.DownloadStringAsync(new Uri("http://www.fusee3d.org/Examples/Async/SomeText.txt"));
```



### Task-based Asynchronous Pattern (TAP - a.k.a. _async/await_-Pattern)

Aus Sicht von Anwendungsprogrammierern ist das EAP nicht immer einfach zu verwenden:
Im Programmcode, der eine asynchrone Operation aufruft muss _zuerst_ der Event-Handler
angegeben werden, in dem beschrieben ist, was passiert, wenn die Aktion _beendet_ ist.
_Danach_ wird im Programmcode erst die eigentliche Aktion _gestartet_. Der Zugriff auf ein
während der asynchronen Aktion generiertes Ergebnis erfolgt etwas versteckt über ein Feld
`Result` des EventArg-Typs.

Für eine einzelne
Aktion mag dieser Aufwand noch gut handhabbar sein. Schwierig wird es, wenn mehrere
Asynchrone Aktionen als Kette oder in baumartiger Struktur mit verschachtelten 
Abhängigkeiten gestartet (und beendet) werden sollen. 

Aus diesem Grund wurde das TAP eingeführt, das, ähnlich wie `yield return` durch
Spracherweiterungen der C#-Syntax (und anderer .NET-basierter Programmiersprachen)
großen Einfluss auf den Ablauf von Programmcode nimmt, der ansonsnten nur durch komplexe
programmiersprachliche Konstrukte implementierbar wäre.

Aktionen, die mit dem TAP verwendbar sind, erkennt man an Methoden mit dem Namensschema
`AktionTaskAsync()`. Zudem liefern diese Methoden ein Objekt vom Typ `Task` bzw. `Task<T>`
zurück, wobei `T` den Typ des zu erwartenden Ergebnis repräsentiert.

Der Aufruf der Methode kann dann mit dem C#-Schlüsselwort `await` erfolgen. Falls die Aktion
ein Ergebnis liefert, erlaubt es diese erweiterte Syntax, den Aufruf direkt in eine Zuweisung
an eine Variable des Typs `T` zu verpacken. Direkt an den Anschluss des Aufrufs mit `await`
kann dann auf das Ergebnis zugegriffen werden, so dass die Code-Struktur nahezu wie
ein synchroner Aufruf aussieht. Der Compiler separiert allerdings den Code _vor_ und
_hinter_ dem Schlüsselwort `await` derart, dass ein asynchroner Aufruf der Aktion entsteht,
die Methode also direkt beendet wird
und der Code hinter `await` erst nach Beendigung der nebenläufigen Aktion ausgeführt wird.

Die Methode, die den asynchronen Aufruf enthält, kann nun bei der Deklaration mit dem 
als Modifizierer wirkenden Schlüsselwort `async` markiert werden, was dann dazu führt,
dass diese Methode unmittelbar nach dem mittels `await` markierten Aufruf beendet
wird und damit der Kontrollfluss nicht für längere Zeit unterbrochen wird.

Dieser Mechanismus funktioniert auch über mehrere `await`-Aufrufe hinweg, die dann scheinbar
zwar asynchron aufgerufen werden, die Weiterbearbeitung der Ergebnisse allerdings 
automatisch synchronisiert wird. 

Die Klasse `WebClient` erlaubt das
asynchrone Herunterladen einer Textdatei und Zurückliefern in Form eines Strings nach dem 
TA-Pattern mit der Methode `DownloadStringTaskAsync` 

```C#
    app.ButtonDown += async delegate (object sender, EventArgs args)
    {
        WebClient client = new WebClient();

        String fileContents = await client.DownloadStringTaskAsync(new Uri("http://www.fusee3d.org/Examples/Async/SomeText.txt"));
        // Nach dem await - Code der hier steht, wird erst nach dem ENDE des Task aufgerufen
        app.Ticker.CompleteText = fileContents;

    };
```

Während der Code im Methodenrumpf nun nahezu wie ein synchroner Aufruf aussieht, muss 
die Methode selbst mit `async` deklariert werden, damit der Mechanismus der Code-Separation
vor und nach `await` funktioniert. Das klappt auch, wie im Beispiel, mit anonymen Methoden.

