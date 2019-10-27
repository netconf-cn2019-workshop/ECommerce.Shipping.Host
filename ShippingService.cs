using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Shipping.Host
{
    public class ShippingService : Microsoft.Extensions.Hosting.IHostedService
    {
        private readonly IBusControl _busControl;
        private readonly ILogger<ShippingService> _logger;

        public ShippingService(IBusControl busControl, ILogger<ShippingService> logger)
        {
            this._busControl = busControl;
            this._logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("正在启动服务总线");

            try
            {
                await _busControl.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "启动服务总线时发生错误");
                throw;
            }

            _logger.LogInformation("发货 微服务已启动");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _busControl.StopAsync(cancellationToken);
            _logger.LogInformation("发货 微服务已停止");
        }
    }
}
