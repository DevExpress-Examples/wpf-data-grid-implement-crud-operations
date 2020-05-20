using System;
using System.Linq;

namespace DevExpress.CRUD.Northwind {
    public class Product {
        public long Id { get; set; }
        public string Name { get; set; }
        public long CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public string QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        public string EAN13 { get; set; }
    }
}
