using System;
using System.Collections.Generic;
using System.Linq;

namespace DevExpress.CRUD.Northwind {
    public class Category {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
