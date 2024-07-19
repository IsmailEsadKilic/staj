using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var sender = "Server";
Console.WriteLine($"{sender} is running");

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "requestQueue", exclusive: false);

var messageId = 1;

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var receivedMessage = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Message received: {ea.BasicProperties.CorrelationId} | {receivedMessage}");

    Console.WriteLine("Enter your reply (or 'exit' to close):");

    string message = "default message";

    message = Console.ReadLine() ?? string.Empty;

    if (message.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        return;
    }

    var correlationId = Guid.NewGuid().ToString();
    message = $"S{messageId}|{correlationId}|{message}";

    var replyProps = channel.CreateBasicProperties();
    replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

    var replyBody = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(
        exchange: "",
        routingKey: ea.BasicProperties.ReplyTo,
        basicProperties: replyProps,
        body: replyBody
    );
};

channel.BasicConsume(queue: "requestQueue", autoAck: true, consumer: consumer);

while (true)
{
    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(intercept: true);
        if (key.Key == ConsoleKey.Enter)
        {
            break;
        }
    }
}

Console.ReadLine();