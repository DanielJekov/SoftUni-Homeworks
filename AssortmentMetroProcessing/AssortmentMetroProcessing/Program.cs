using AssortmentMetroProcessing.ExportModel;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AssortmentMetroProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            const string path = @"../../../ImportModel\RawData.csv";
            const string procesedFilePathToSave = "../../../SplittedData.csv";

            var originalCollectionRead = ReaderOfCsvFile<CsvReadingDataModel>(path);
            ;
            var proccesedCollection = new List<CsvExportModelDataResult>();
            SplittingByWeigh(originalCollectionRead, proccesedCollection);

            CsvWriter<CsvExportModelDataResult>(proccesedCollection, procesedFilePathToSave);

        }

        public static ICollection<T> ReaderOfCsvFile<T>(string path)
        {
            IEnumerable<T> records;
            var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";", Encoding = Encoding.Unicode };

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, config))
            {
                records = csv.GetRecords<T>().ToList();
            }

            return records.ToList();
        }

        public static ICollection<CsvExportModelDataResult> SplittingByWeigh(ICollection<CsvReadingDataModel> inputcollection, ICollection<CsvExportModelDataResult> processedCollection)
        {
            foreach (var record in inputcollection)
            {
                var strinToCheck = record.ART_NAME.Substring(0, 7);

                if (!char.IsDigit(strinToCheck[0]))
                {
                    if (!char.IsDigit(strinToCheck[1]))
                    {
                        var test = new CsvExportModelDataResult()
                        {
                            ArticulName = record.ART_NAME,
                            Weigh = null,
                            Supplyer = record.SUPPL_NAME
                        };
                        processedCollection.Add(test);

                        continue;
                    }
                }

                if (!(strinToCheck.Contains("КГ") ||
                    strinToCheck.Contains("ГР") ||
                    strinToCheck.Contains("Г")))
                {
                    var test = new CsvExportModelDataResult()
                    {
                        ArticulName = record.ART_NAME,
                        Weigh = null,
                        Supplyer = record.SUPPL_NAME
                    };
                    processedCollection.Add(test);
                    continue;
                }

                var separatedElement = new CsvExportModelDataResult();
                var name = string.Empty;

                if (strinToCheck.Contains("КГ"))
                {
                    var index = record.ART_NAME.IndexOf("КГ");
                    name = record.ART_NAME.Insert(index + 2, "|");
                }
                else if (strinToCheck.Contains("ГР"))
                {
                    var index = record.ART_NAME.IndexOf("ГР");
                    name = record.ART_NAME.Insert(index + 2, "|");
                }
                else if (strinToCheck.Contains("Г"))
                {
                    var index = record.ART_NAME.IndexOf("Г");
                    name = record.ART_NAME.Insert(index + 1, "|");
                }

                var splitted = name.Split(new string[] { "| ", "|" }, StringSplitOptions.RemoveEmptyEntries);

                separatedElement.ArticulName = splitted[1];
                separatedElement.Weigh = splitted[0];
                separatedElement.Supplyer = record.SUPPL_NAME;

                processedCollection.Add(separatedElement);
            }

            return processedCollection;
        }

        public static void CsvWriter<T>(ICollection<T> collection, string path)
        {
            var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";", Encoding = Encoding.Unicode };
            using (var writer = new StreamWriter(path, false, Encoding.Unicode))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(collection);
            }
        }
    }
}
