using CommonData.RabbitQueue;

namespace ProfilesService.Worker
{
    public class Worker : BackgroundService
    {
        private readonly IBus _busControl;
        public Worker()
        {
            _busControl = RabbitHutch.CreateBus("localhost");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //await _busControl.ReceiveAsync<IEnumerable<WeatherForecastRequest>>(Queue.Processing, x =>
            //{
            //    Task.Run(() => { DidJob(x); }, stoppingToken);
            //});
        }
    }
}
