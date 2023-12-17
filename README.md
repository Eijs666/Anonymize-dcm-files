# DICOM Anonymizer

## Beskrivelse
DICOM Anonymizer er en kommandolinjeapplikation skrevet i C#, der er designet til at anonymisere DICOM-billeder. Applikationen læser DICOM-filer fra en angivet inputmappe, anonymiserer patientinformationen og gemmer de anonymiserede billeder i en outputmappe.

## Funktioner
- Anonymisering af patientnavne i DICOM-filer.
- Tilfældiggørelse af datofelter (PatientBirthDate og StudyDate).
- Bevaring af originale værdier for udvalgte DICOM-tags.
- Fejlhåndtering og logging af processen.
- 
## Brug
For at køre applikationen, brug følgende kommando i terminalen:
anonymizeDicom --InputFolder <sti_til_input_mappe> --OutputFolder <sti_til_output_mappe>

Erstat `<sti_til_input_mappe>` med stien til mappen, der indeholder DICOM-filerne, du ønsker at anonymisere, og `<sti_til_output_mappe>` med stien til mappen, hvor de anonymiserede filer skal gemmes.

## Udvikling
Applikationen er skrevet i C# og kan udvikles yderligere med OCR/OpenCV, ved at skanne for text der er "brændt inde" i dicom filer.
