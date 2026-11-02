# File Sorter
## Beskrivning
Det här programmet läser in `Inställningar.xml` och hanterar sedan automatiska filförflyttningar baserade på innehållet i inställningsfilen. En loggfil skapas som loggar alla förflyttningar som gjorts.

Några nyckelfunktioner:
- Inställningsfilen kan redigeras medans programmet körs. Inställningsfilen kommer att läsas in igen, och automatiska filförflyttningar kommer att uppdateras baserat på ändringarna.
- Filförflyttningar sker både på existerande filer och på filer som skapas eller ändras under programmets körning.
- Ett obegränsat antal regler kan konfigureras.
> [!IMPORTANT]
> Programmet kommer att stängas ner med felkod 1 om fel upptäcks i inställningsfilen. Alla element måste existera, alla sökvägar måste vara giltiga, och minst en \<Type> måste finnas för varje \<Directory>.
## Inställningar.xml
Standardfil som följer med programmet:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<Settings>
	<Log>log.txt</Log>
	<Directory>
		<Name>Bilder</Name>
		<Input>C:\Exempel\Downloads</Input>
		<Output>C:\Exempel\Bilder</Output>
		<Type>.jpg</Type>
		<Type>.jpeg</Type>
		<Type>.png</Type>
	</Directory>
	<Directory>
		<Name>Textfiler</Name>
		<Input>C:\Exempel\Downloads</Input>
		<Output>C:\Exempel\Textfiler</Output>
		<Type>.txt</Type>
	</Directory>
</Settings>
```
### \<Settings> ... \</Settings>
- Rot-elementet \<Settings> måste finnas.
- Om rotelementet inte hittas så avslutas programmet med felkod 1.
### \<Log>log.txt\</Log>
- Filnamnet `log.txt` kan bytas ut mot valfritt namn.
- Om ett filnamn anges utan sökväg så kommer programmets mapp att användas.
- Om ett filnamn anges med sökväg så kommer den sökvägen att användas.
- Elementet \<Log> måste finnas under rot-elementet \<Settings>.
- Om elementet \<Log> inte hittas så avslutas programmet med felkod 1.
- Om en ogiltig sökväg anges så avslutas programmet med felkod 1.
### \<Directory> ... \</Directory>
- Minst ett \<Directory> element måste finnas under rot-elementet \<Settings>.
- Om minst ett \<Directory> element inte hittas så avslutas programmet med felkod 1.
- Ett obegränsat antal \<Directory> element får finnas.
- Varje \<Directory> element måste innehålla:
  - Ett \<Name> element,
  - Ett \<Input> element,
  - Ett \<Output> element,
  - Minst ett \<Type> element
- Ett obegränsat antal \<Type> element får finnas under varje \<Directory> element.
- Om varje \<Directory> element inte innehåller ett \<Name> element, ett \<Input> element, ett \<Output> element och minst ett \<Type> element så avslutas programmet med felkod 1.
### \<Name>Namn\</Name>
\<Name> elementet används för loggfilen. Det rekommenderas att använda beskrivande och unika namn för att underlätta eventuell felsökning eller filtrering av data. Exempel på en rad i loggfilen:
```
2026-10-30 08:52:29 [Textfiler] File moved from "C:\Exempel\Downloads\Test.txt" to "C:\Exempel\Textfiler\Test.txt"
```
### \<Input>Sökväg\</Input>
- \<Input> elementet anger varifrån filer ska hämtas.
- \<Input> elementet måste ange en giltig sökväg.
- Om \<Input> elementet inte anger en giltig sökväg så avslutas programmet med felkod 1.
### \<Output>Sökväg\</Output>
- \<Output> elementet anger vart filer ska flyttas till.
- \<Output> elementet måste ange en giltig sökväg.
- Om \<Output> elementet inte anger en giltig sökväg så avslutas programmet med felkod 1.
### \<Type>Filter\</Type>
- \<Type> elementet anger filter för de filer som ska flyttas.
- Filändelser kan anges med eller utan asterisk, exempelvis så funger både `.txt` och `*.txt`.