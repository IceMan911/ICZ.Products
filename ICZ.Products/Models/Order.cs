using ICZ.Products.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ICZ.Products.Models
{
    [DataContract]
    public class Order
    {
        [DataMember, DataField("IdOrder")]
        public Guid IdOrder { get; set; }
        [DataMember, DataField("Status")]
        public string Status { get; set; }
        [DataMember, DataField("OrderCreated")]
        public DateTime OrderCreated { get; set; }
        [DataMember, DataField("Items")]
        public List<OrderItem> Items { get; set; }

        public Order()
        {
            Items = new List<OrderItem>();
        }
    }
}