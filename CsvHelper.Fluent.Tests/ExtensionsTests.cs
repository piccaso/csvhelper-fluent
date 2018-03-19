using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using Xunit;
using Xunit.Abstractions;

namespace CsvHelper.Fluent.Tests
{
    public class ExtensionsTests
    {
        private readonly ITestOutputHelper _output;

        public ExtensionsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void AnonTypes()
        {
            var file = new FileInfo("anon.csv");

            var data = new List<dynamic>
            {
                new {col1 = 10, col2 = 20, col3 = 30},
                new {col1 = 11, col2 = 21, col3 = 31},
                new {col1 = 12, col2 = 22, col3 = 32},
            };

            data.WriteCsv(file);

            var csv = file.ReadCsv(row => new
            {
                col1 = row.GetField<int>(0),
                col2 = row.GetField<int>("col2"),
                col3 = int.Parse(row[2]),
            });


            Assert.Equal(data[0].col1, csv[0].col1);
            Assert.Equal(data[1].col2, csv[1].col2);
            Assert.Equal(data[2].col3, csv[2].col3);

            var dynCsv = file.ReadCsv<dynamic>();
            Assert.Equal(data[0].col1.ToString(), dynCsv[0].col1.ToString());
            Assert.Equal(data[1].col2.ToString(), dynCsv[1].col2.ToString());
            Assert.Equal(data[2].col3.ToString(), dynCsv[2].col3.ToString());

        }

        public class Product
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
        }

        [Fact]
        public void StaticType()
        {
            var file = new FileInfo("products.csv");
            var config = Configuration.For(new CultureInfo("de-AT"));

            var products = new List<Product>
            {
                new Product {Name = "X223-A", Price = 3.90M},
                new Product {Name = "X223-B", Price = 4.95M},
                new Product {Name = "X223-Ö", Price = 8.85M},
            };

            products.WriteCsv(file, config);

            var csv = file.ReadCsv<Product>(config);

            Assert.Equal(products[0].Price, csv[0].Price);
            Assert.Equal(products[2].Name, csv[2].Name);
        }

        [Fact]
        public void ZipAsFile()
        {
            if(File.Exists("report.zip")) File.Delete("report.zip");

            var products = new List<Product>
            {
                new Product {Name = "X867-A", Price = 31.90M},
                new Product {Name = "X867-B", Price = 43.95M},
                new Product {Name = "X867-C", Price = 88.85M},
            };

            using (var zipFile = ZipFile.Open("report.zip", ZipArchiveMode.Create))
            {
                var entry = zipFile.CreateEntry("data.csv", CompressionLevel.Optimal);
                using (var stream = entry.Open())
                {
                    products.WriteCsv(stream);
                }
            }
        }

        [Fact]
        public void Binary()
        {
            var collection = new List<dynamic>
            {
                new {a = 1, b = 2, c = 3},
            };

            var bytes = collection.GetCsvBytes();

            var str = Configuration.Default.Encoding.GetString(bytes);

            _output.WriteLine(str);

            Assert.Contains("a", str);
            Assert.Contains("b", str);
            Assert.Contains("c", str);
            Assert.Contains("1", str);
            Assert.Contains("2", str);
            Assert.Contains("3", str);
        }
    }
}
