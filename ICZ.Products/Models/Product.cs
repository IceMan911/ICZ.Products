using ICZ.Products.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ICZ.Products.Models
{
    [DataContract]
    public partial class Product
    {
        [DataMember, DataField("IdProduct")]
        public Guid IdProduct { get; set; }
        [DataMember, DataField("ProductName")]
        public string ProductName { get; set; }
        [DataMember, DataField("IdCategory")]
        public Guid IdCategory { get; set; }
        [DataMember, DataField("CategoryName")]
        public string CategoryName { get; set; }
        [DataMember, DataField("ProductCost")]
        public Decimal ProductCost { get; set; }

    }
}
