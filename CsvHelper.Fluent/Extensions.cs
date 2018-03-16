using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvCfg = CsvHelper.Configuration.Configuration;
using IEnum = System.Collections.IEnumerable;

namespace CsvHelper.Fluent
{
    public static class Extensions
    {
        public static IList<T> ReadCsv<T>(FileInfo file, CsvCfg config = null)
        {
            config = config ?? Configuration.Default;
            using (var fs = File.OpenRead(file.FullName))
            using (var sr = new StreamReader(fs, config.Encoding))
            {
                var csvReader = new CsvReader(sr, config);
                return csvReader.GetRecords<T>().ToList();
            }
        }

        public static IList<T> ReadCsv<T>(FileInfo file, Func<IReaderRow, T> readFunc) => ReadCsv(null, file, readFunc);
        public static IList<T> ReadCsv<T>(this CsvCfg config, FileInfo file, Func<IReaderRow,T> readFunc)
        {
            config = config ?? Configuration.Default;
            using (var fs = File.OpenRead(file.FullName))
            using (var sr = new StreamReader(fs, config.Encoding))
            {
                var records = new List<T>();
                var csvReader = new CsvReader(sr, config);
                if (config.HasHeaderRecord)
                {
                    csvReader.Read();
                    csvReader.ReadHeader();
                }

                while (csvReader.Read())
                {
                    records.Add(readFunc(csvReader));
                }

                return records;
            }
        }

        public static void WriteCsvTo(this CsvCfg config, IEnum data, Stream stream) => WriteCsvTo(data, stream, config);
        public static void WriteCsvTo(this IEnum data, Stream stream, CsvCfg config = null)
        {
            config = config ?? Configuration.Default;
            using (var sw = new StreamWriter(stream, config.Encoding))
            {
                var csvWriter = new CsvWriter(sw, config);
                csvWriter.WriteRecords(data);
            }
        }
        public static void WriteCsv(this CsvCfg config, IEnum data, FileInfo file) => WriteCsv(data, file, config);
        public static void WriteCsv(this IEnum data, FileInfo file, CsvCfg config = null)
        {
            using (var fs = File.OpenWrite(file.FullName))
            {
                data.WriteCsvTo(fs, config);
            }
        }
        public static byte[] GetCsvBytes(this IEnum data, CsvCfg config = null)
        {
            using (var ms = new MemoryStream())
            {
                data.WriteCsvTo(ms, config);
                return ms.ToArray();
            }
        }

        public static string RemoveControlChars(this string str) => new string(str.Where(c => !char.IsControl(c)).ToArray());
    }
}
