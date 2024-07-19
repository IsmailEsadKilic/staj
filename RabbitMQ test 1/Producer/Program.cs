using System;
using System.Text;
using RabbitMQ.Client;

Console.WriteLine("Producer is running");

var sender = "Producer";

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: "myRoutingExchange", type: ExchangeType.Direct);

var messageId = 1;

while (true)
{
    Console.WriteLine("Enter your message (or 'exit' to close):");
    string message = "default message";
    message = Console.ReadLine() ?? string.Empty;
    message = $"{messageId}| {message}";
    if (message == "exit" || message == "Exit" || message == "EXIT" || message == "e" || message == "E" || message == null || message == "" || message == " ")
    {
        break;
    }
    string routingKey = "user.asia.payments";
    if (messageId % 2 == 0)
    {
        routingKey = "business.europe.order";
    } else if (messageId % 3 == 0)
    {
        routingKey = "user.europe.payments";
    } else if (messageId % 5 == 0)
    {
        routingKey = "user.asia.order";
    }
    if (routingKey == "exit" || routingKey == "Exit" || routingKey == "EXIT" || routingKey == "e" || routingKey == "E" || routingKey == null || routingKey == "" || routingKey == " ")
    {
        break;
    }
    var encodedMessage = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish(exchange: "myTopicExchange",
                        routingKey: routingKey,
                        basicProperties: null,
                        body: encodedMessage);
    Console.WriteLine(" {0} Sent {1} to {2}", sender, message, routingKey);

    messageId++;
}