using System;
using System.Threading.Tasks;
using ECommerce.Common.Commands;
using ECommerce.Common.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ECommerce.Shipping.Host.Consumers
{
    public class InitiateOrderPackingCommandConsumer : IConsumer<InitiateOrderPackingCommand>
    {
        private readonly ILogger<InitiateOrderPackingCommandConsumer> _logger;

        public InitiateOrderPackingCommandConsumer(ILogger<InitiateOrderPackingCommandConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<InitiateOrderPackingCommand> context)
        {
            _logger.LogDebug($"由顾客 {context.Message.CustomerId} 提交的订单 {context.Message.OrderId} 正在打包");

            await Task.Delay(10000);

            await context.Publish(new OrderPackedEvent()
            {
                CorrelationId = context.Message.CorrelationId,
                OrderId = context.Message.OrderId,
                CustomerId = context.Message.CustomerId
            });
        }
    }
}
