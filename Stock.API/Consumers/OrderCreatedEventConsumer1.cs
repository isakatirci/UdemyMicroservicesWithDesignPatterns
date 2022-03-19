using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Interfaces;
using Stock.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer1 : IConsumer<IOrderCreatedEvent>
    {
        private readonly AppDbContext _context;
        private ILogger<OrderCreatedEventConsumer> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer1(AppDbContext context, ILogger<OrderCreatedEventConsumer> logger, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sendEndpointProvider = sendEndpointProvider ?? throw new ArgumentNullException(nameof(sendEndpointProvider));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task Consume(ConsumeContext<IOrderCreatedEvent> context)
        {
            var orderItems = context.Message.OrderItems;
            var orderItemsCount = context.Message.OrderItems.Count;
            var correlationId = context.Message.CorrelationId;
            var stockResult = new List<bool>(orderItemsCount);
            for (int i = 0; i < orderItemsCount; i++)
            {
                var item = orderItems[i];
                stockResult.Add(await _context.Stocks.AnyAsync(x => x.ProductId == item.ProductId && x.Count > item.Count));
            }
            if (stockResult.All(x => x.Equals(true)))
            {
                for (int i = 0; i < orderItemsCount; i++)
                {
                    var item = orderItems[i];
                    var stock = await _context.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);
                    if (stock != null)
                    {
                        stock.Count -= item.Count;
                    }
                    await _context.SaveChangesAsync();
                }
                _logger.LogInformation($"Stock was reserved for CorrelationId Id :{correlationId}");

                StockReservedEvent stockReservedEvent = new(correlationId)
                {
                    OrderItems = orderItems
                };
                await _publishEndpoint.Publish(stockReservedEvent);
                return;
            }
            await _publishEndpoint.Publish(new StockNotReservedEvent(correlationId)
            {
                Reason = "Not enough stock"
            });

            _logger.LogInformation($"Not enough stock for CorrelationId Id :{correlationId}");
        }
    }
}
