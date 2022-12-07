using ICZ.Products.DB;
using System;
using System.Runtime.Serialization;

namespace ICZ.Products.Models
{
    [DataContract]
    public class OrderItemCreate
    {
        [DataMember, DataField("IdProduct")]
        public Guid IdProduct { get; set; }
        [DataMember, DataField("Quantity")]
        public int Quantity { get; set; }
    }
}