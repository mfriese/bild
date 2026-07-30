# AGENTS.md

## Projektüberblick

`bild` ist eine interaktive .NET-Konsolenanwendung zur Organisation von Foto-
und Videodateien. Sie kopiert akzeptierte Mediendateien in eine Bibliothek nach
`<Jahr>/<Monat>` und kann die Quelldateien anschließend auf ausdrückliche
Nutzerbestätigung löschen.

Die Solution `Bild.sln` enthält vier Projekte:

- `Bild/`: ausführbares Startprojekt; erstellt und startet `Bild.Core.Program`.
- `Bild.Core/`: Anwendungslogik, Domänenmodelle, Spectre.Console-Oberfläche,
  Dateioperationen, EXIF-/QuickTime-Metadaten und Konfiguration.
- `Bild.Test/`: xUnit-Tests. Die Test-Mediendateien liegen in
  `Bild.Test/Samples/` und werden ins Testausgabeverzeichnis kopiert.
- `Bild.Nuke/`: NUKE-Builddefinition. Die Root-Skripte `build.sh`, `build.ps1`
  und `build.cmd` rufen sie plattformübergreifend auf.

Ziel-Framework ist überall .NET 9 (`net9.0`). Paketversionen werden direkt in
den jeweiligen `.csproj`-Dateien verwaltet; es gibt kein `global.json`.

## Wichtige Arbeitsregeln

- Halte fachliche Logik in `Bild.Core`; das Projekt `Bild/` soll ein sehr
  schlanker Einstiegspunkt bleiben.
- Lege neue Interactors entsprechend ihrer Aufgabe unter
  `Bild.Core/Interactors/<Bereich>/` ab. Sie folgen üblicherweise dem Muster
  einer öffentlichen Klasse mit einer `Perform(...)`-Methode.
- Lege Domänenobjekte unter `Bild.Core/Features/` ab und verwende Namespaces,
  die der Ordnerstruktur entsprechen.
- Ergänze oder ändere bei Metadaten-/Dateitypverhalten passende xUnit-Tests in
  `Bild.Test/Tests/`. Kleine repräsentative Fixtures gehören nach
  `Bild.Test/Samples/`; keine privaten oder großen Medienbibliotheken einchecken.
- Bewahre bestehende Zeilenenden und den lokalen Stil. `Bild.Core/.editorconfig`
  fordert für C# vier Leerzeichen und `crlf`; `Bild.Nuke` hat eigene Regeln.
- Ändere Paketversionen, Buildskripte und die NUKE-Builddefinition nur, wenn die
  Aufgabe dies erfordert.

## Sicherheitskritische Bereiche

- `NewImportCommand` kopiert Dateien in die konfigurierte Fotobibliothek und
  kann danach Quelldateien mit `MediaFile.Delete()` unwiderruflich löschen.
  Niemals einen echten Import als Test oder zur Verifikation ausführen.
- `MediaDir.GetOrCreateSubdirectory`, `MediaDir.Insert` und
  `MediaFile.Copy` verändern den Dateisystembestand. Für Tests immer temporäre
  Verzeichnisse und eigene Fixtures verwenden.
- Die Konfiguration wird außerhalb des Repositories unter
  `ApplicationData/Bild/settings.json` gespeichert. Interaktive Konfiguration
  nicht bei automatisierten Prüfungen ausführen oder verändern.
- EXIF-/Dateityperkennung arbeitet mit echten Binärdateien und `SharpExifTool`.
  Behalte Fehlerfälle (fehlende, beschädigte oder zu kleine Dateien) bei und
  erweitere sie gezielt mit Tests.

## Build und Tests

Voraussetzung: installiertes .NET-9-SDK. Zuerst aus dem Repository-Stamm
ausführen:

```bash
dotnet test Bild.Test/Bild.Test.csproj
dotnet build Bild.sln
```

Für den vollständigen projektspezifischen Build stehen diese Ziele bereit:

```bash
./build.sh Compile
./build.sh Test
```

`Compile` bereinigt vor dem Build die `bin`- und `obj`-Verzeichnisse (außerhalb
von `Bild.Nuke`). Verwende es daher nicht, wenn lokale Buildartefakte erhalten
bleiben müssen. `Test` baut zuerst und führt anschließend das Testprojekt mit
einem TRX-Logger aus.

Vor Übergabe mindestens die für die Änderung relevanten Tests ausführen; bei
Änderungen in `Bild.Core` bevorzugt zusätzlich den gesamten Testbefehl oben.
Wenn Tests wegen einer externen ExifTool-Abhängigkeit oder fehlender SDKs nicht
laufen, Ursache und nicht ausgeführte Prüfung klar berichten.

## Ausführen und Prüfen

Die Anwendung ist absichtlich interaktiv und lässt sich mit folgendem Befehl
starten:

```bash
dotnet run --project Bild/Bild.csproj
```

Starte sie nur für UI-Änderungen oder auf ausdrücklichen Wunsch, da Menüpfade
Konfigurationen und Dateioperationen auslösen können. Für Änderungen am Import
sind unit- oder temporärverzeichnisbasierte Tests der sichere Standard.
