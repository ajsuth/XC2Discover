using CsvHelper;
using System.Globalization;
using System.IO;
using System.Text;

namespace Ajsuth.Sample.Discover.Engine.Service
{
    public class FeedService
    {
        public FeedService()
        {
        }

        public static void CreateFeedFile<T>(string filePath)
        {
            using (var writer = new StreamWriter(@filePath))
            {
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csv.WriteHeader<T>();
                csv.NextRecord();
            }
        }

        public static void AppendToFeedFile(string filePath, object entry)
        {
            using (var writer = new StreamWriter(@filePath, true, Encoding.UTF8))
            {
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csv.WriteRecord(entry);
                csv.NextRecord();
            }
        }
    }
}
