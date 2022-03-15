using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public class OrderCreatedRequestEvent1 : IOrderCreatedRequestEvent1
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; } = new List<OrderItemMessage>();
        public PaymentMessage Payment { get; set; }
    }

    public class OrderCreatedRequestEvent : IOrderCreatedRequestEvent
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; } = new List<OrderItemMessage>();
        public PaymentMessage Payment { get; set; }
    }
}