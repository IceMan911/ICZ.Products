using ICZ.Products.DB;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ICZ.Products.Models
{
    [DataContract]
    public class OrderCreate
    {
        [DataMember, DataField("Status")]
        public string Status { get; set; }
        [DataMember, DataField("Items")]
        public List<OrderItemCreate> Items { get; set; }
    }
}