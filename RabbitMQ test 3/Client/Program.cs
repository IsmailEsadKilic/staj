using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var receiver = "Client";
Console.WriteLine($"{receiver} is running. Press ESC to exit.");
var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
var messageId = 1;

/////////////////////////

channel.ExchangeDeclare(
    exchange: "secondExchange",
    type: ExchangeType.Fanout
);

var consumer = new EventingBasicConsumer(channel);

channel.QueueDeclare(
    queue: "bruh",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null
);

channel.QueueBind(
    queue: "bruh",
    exchange: "secondExchange",
    routingKey: ""
);

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Message received: {message}");
};

channel.BasicConsume(
    queue: "bruh",
    autoAck: true,
    consumer: consumer
);

/////////////////////////

while (true)
{
    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(intercept: true);
        if (key.Key == ConsoleKey.Escape)
        {
            break;
        }
    }
}
Console.ReadKey();
