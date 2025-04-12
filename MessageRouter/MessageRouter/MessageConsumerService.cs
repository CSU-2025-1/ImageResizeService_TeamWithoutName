using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter
{
    public class MessageConsumerService : BackgroundService
    {
        private readonly ILogger<MessageConsumerService> _logger;
        private readonly IMessageConsumer _messageConsumer; // Ваш интерфейс/класс консьюмера
        private readonly IServiceProvider _serviceProvider; // Для получения сервисов из DI внутри цикла

        public MessageConsumerService(ILogger<MessageConsumerService> logger, IMessageConsumer messageConsumer, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _messageConsumer = messageConsumer;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Message Consumer Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Используйте область (scope) для каждого сообщения, если нужно
                    // для DI-сервисов с scoped lifetime.
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var scopedService = scope.ServiceProvider.GetRequiredService<IScopedService>(); // Пример scoped сервиса

                        var message = await _messageConsumer.ConsumeMessageAsync(); // Асинхронное получение сообщения

                        if (message != null)
                        {
                            _logger.LogInformation($"Consumed message: {message}");

                            // Обработка сообщения (ВАЖНО: обработка должна быть быстрой или тоже асинхронной)
                            await ProcessMessageAsync(message, scopedService);
                        }
                        else
                        {
                            // Нет сообщений, ожидаем
                            await Task.Delay(1000, stoppingToken); // Не блокируем поток
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error consuming message.");
                    // Здесь можно реализовать логику повторных попыток (retry policy)
                    await Task.Delay(5000, stoppingToken); // Подождать перед повторной попыткой
                }
            }

            _logger.LogInformation("Message Consumer Service is stopping.");
        }


        private async Task ProcessMessageAsync(string message, IScopedService scopedService)
        {
            // Здесь логика обработки сообщения.
            // ВАЖНО: Если требуется доступ к базе данных или другим ресурсам,
            // используйте асинхронные методы.
            // Идеально, если обработка сообщения выполняется быстро.
            // Если обработка сложная, рассмотрите использование очереди (например, RabbitMQ)
            // для разделения задач консьюмера и обработчика.

            //Пример использования scoped сервиса
            scopedService.DoSomething();

            await Task.CompletedTask; // Заглушка для асинхронной операции
        }
    }

    // Пример интерфейса консьюмера
    public interface IMessageConsumer
    {
        Task<string> ConsumeMessageAsync();
    }

    //Пример scoped сервиса
    public interface IScopedService
    {
        void DoSomething();
    }

    public class ScopedService : IScopedService
    {
        public void DoSomething()
        {
            // Do something here
        }
    }
}
