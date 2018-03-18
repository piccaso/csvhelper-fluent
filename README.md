# Examples
```cs
// read only needed columns
var file = new FileInfo("data.csv");
var csv = file.ReadCsv(row => new {
    col0 = row.GetField<int>(0),
    col2 = row.GetField<int>("col2"),
    col5 = int.Parse(row[5]),
});
```
```cs
// write collection with spesific culture
var file = new FileInfo("products.csv");
var config = Configuration.For(new CultureInfo("de-AT")); // optional, default == CurrentCulture

var products = new List<Product> {
    new Product {Name = "X223-A", Price = 3.90M},
    new Product {Name = "X223-B", Price = 4.95M},
    new Product {Name = "X223-Ö", Price = 8.85M},
};

products.WriteCsv(file, config);
```
