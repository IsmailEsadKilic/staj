using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var sender = "Server";
Console.WriteLine($"{sender} is running. Press ESC to exit.");
var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
var messageId = 1;

/////////////////////////

channel.ExchangeDeclare(
    exchange: "firstExchange",
    type: ExchangeType.Direct
);

channel.ExchangeDeclare(
    exchange: "secondExchange",
    type: ExchangeType.Fanout
);

channel.ExchangeBind(
    destination: "secondExchange",
    source: "firstExchange",
    routingKey: ""
);

void SendMessage()
{
    var message = $"Message {messageId++}";
    var body = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish(
        exchange: "firstExchange",
        routingKey: "",
        basicProperties: null,
        body: body
    );
    Console.WriteLine($"Message sent: {message}");
}

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

        SendMessage();
    }
}
Console.ReadKey();