using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRI.DISS.Libs.Helpers
{
    public class CsvWriter
    {
        public string Delimiter { get; set; } = ";";
        public bool IncludeHeaderToEmptyFile { get; set; } = true;

        public List<CsvColumn> Columns { get; set; } = new List<CsvColumn>();

        public void Write(FileInfo file, object data)
        {
            using (var writer = new StreamWriter(file.FullName, true, Encoding.UTF8))
            {
                // Write header if file is empty or IncludeHeaderToEmptyFile is true
                if (file.Length == 0 && IncludeHeaderToEmptyFile)
                {
                    var header = Columns.Select(c => c.Title).ToArray();
                    writer.WriteLine(string.Join(Delimiter, header));
                }

                // Write data
                var values = Columns.Select(c => c.Converter(data)).ToArray();
                writer.WriteLine(string.Join(Delimiter, values));
            }
        }

    }

    public class CsvColumn
    {
        public string Title { get; set; } = "";
        public Func<object, string> Converter { get; set; } = (x) => x.ToString()!;
    }
}
