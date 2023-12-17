using FellowOakDicom;
using System;
using System.Globalization;
using System.IO;

class Program
{
    //Future work --
    // Use Tesseract/OCR to scan for text "burned in" (independent of metatags) and to delete or edit the detected text.
    
    // PatientBirthDate and StudyDate Random number generator to use for randomizing date.
    private static readonly Random random = new Random();

    //Aonymize Dicom images / metatags
    static void Main(string[] args)
    {
        // Has arguemnts passed?
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: anonymizeDicom --InputFolder <inputFolderPath> --OutputFolder <outputFolderPath>");
            return;
        }

        // Parse command line arguments for input and output folders
        string inputFolder = args[0];
        string outputFolder = args[1];

        // If folder dont exist, create folder
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        // Process each DICOM file - loop over each .dcm file
        foreach (string file in Directory.EnumerateFiles(inputFolder, "*.dcm"))
        {
            try
            {
                // Print the file that is being processed
                Console.WriteLine($"Processing file {file}...");

                var dicomFile = DicomFile.Open(file); //save selected file to variable 

                AnonymizeFields(dicomFile.Dataset); //Call method to Anonymize the metags in selected file

                //join folder and dcm filename for output path
                string outputFile = Path.Combine(outputFolder, Path.GetFileName(file));
                dicomFile.Save(outputFile);

                // Log successful processing of each file
                Console.WriteLine($"Processed and saved {outputFile}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error process {file}: {ex.Message}");
            }
        }
        //Log "Complete" when every file has been ran through
        Console.WriteLine("Processing complete.");
    }

    //Anonymize dicom dataset metadata from input folder
    static void AnonymizeFields(DicomDataset dataset)
    {

        // Check current value of name before updating, by logging it to console
        if (dataset.TryGetSingleValue(DicomTag.PatientName, out string currentName))
        {
            Console.WriteLine($"Current PatientName: {currentName}");
        }
        else { Console.WriteLine("PatientName tag not found or has no value."); }

        // CHange patient name to DEFAULT NAME
        dataset.AddOrUpdate(DicomTag.PatientName, "DEFAULT NAME");

        // Check the name of patient has been updated by logging to console
        if (dataset.TryGetSingleValue(DicomTag.PatientName, out string updatedName))
        {
            Console.WriteLine($"Updated PatientName: {updatedName}");
        }
        else { Console.WriteLine("Cant update PatientName."); }

        //Metatags to keep valueunchanged
        //Ignore tags shown in assigment tables: A.1-A.5.

        KeepOriginalValue(dataset, DicomTag.PatientSex);
        KeepOriginalValue(dataset, DicomTag.StudyInstanceUID);
        KeepOriginalValue(dataset, DicomTag.Modality);
        KeepOriginalValue(dataset, DicomTag.Manufacturer);
        KeepOriginalValue(dataset, DicomTag.ManufacturerModelName);
        KeepOriginalValue(dataset, DicomTag.SoftwareVersions);
        KeepOriginalValue(dataset, DicomTag.ImagerPixelSpacing);
        KeepOriginalValue(dataset, DicomTag.Rows);
        KeepOriginalValue(dataset, DicomTag.Columns);
        KeepOriginalValue(dataset, DicomTag.PixelSpacing);
        KeepOriginalValue(dataset, DicomTag.BitsAllocated);
        KeepOriginalValue(dataset, DicomTag.PixelRepresentation);
        KeepOriginalValue(dataset, DicomTag.SmallestImagePixelValue);
        KeepOriginalValue(dataset, DicomTag.LargestImagePixelValue);
        KeepOriginalValue(dataset, DicomTag.PixelData);

        KeepOriginalValue(dataset, DicomTag.SOPClassUID);
        KeepOriginalValue(dataset, DicomTag.SOPInstanceUID);
        KeepOriginalValue(dataset, DicomTag.Rows);


        // Get random ofsset number
        int randomDaysOffset = random.Next(-365, 365);
        //Call method to change the tags containing any "date"
        AnonymizeDateField(dataset, DicomTag.PatientBirthDate, randomDaysOffset);
        AnonymizeDateField(dataset, DicomTag.StudyDate, randomDaysOffset);
        AnonymizeDateField(dataset, DicomTag.AcquisitionDate, randomDaysOffset);
        AnonymizeDateField(dataset, DicomTag.SeriesDate, randomDaysOffset);
        AnonymizeDateField(dataset, DicomTag.ContentDate, randomDaysOffset);



    }
    //Anonymize date file in metatags in dicom file
    static void AnonymizeDateField(DicomDataset dataset, DicomTag tag, int offsetDays)
    { //Does date field exist? if yes, then get the value
        if (dataset.TryGetSingleValue(tag, out string dateValue))
        {
            // try and put date into date time object, add (random) offset days to date, convert back to same format, update
            if (DateTime.TryParseExact(dateValue, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime originalDate))
            {
                DateTime newDate = originalDate.AddDays(offsetDays);
                string newDateString = newDate.ToString("yyyyMMdd");
                dataset.AddOrUpdate(tag, newDateString);
            }
        }
    }
    static void KeepOriginalValue(DicomDataset dataset, DicomTag tag)
    {
        //Ignore tags shown in assigment tables: A.1-A.5.
        if (!dataset.Contains(tag))
        {
            Console.WriteLine($"Tag {tag} not found in the dataset.");
        }
    }
}
