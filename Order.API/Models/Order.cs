using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string BuyerId { get; set; }

        public Address Address { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public OrderStatus Status { get; set; }

        public string FailMessage { get; set; }

        public CourierCompany CourierCompany { get; set; }
    }

    public enum OrderStatus
    {
        Suspend,
        Complete,
        Fail
    }

    public class CourierCompany
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Order Order { get; set; }
        public int OrderId { get; set; }
    }
}