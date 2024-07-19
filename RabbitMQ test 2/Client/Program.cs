using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;

var receiver = "Client";
Console.WriteLine($"{receiver} is running");

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

var replyQueue = channel.QueueDeclare(
    queue: "",
    exclusive: true
    );
channel.QueueDeclare(
    queue: "requestQueue",
    exclusive: false
    );

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Repply received: {message}");
};

channel.BasicConsume(queue: replyQueue.QueueName,
                    autoAck: true,
                    consumer: consumer);

var messageId = 1;

while(true)
{
    var correlationId = Guid.NewGuid().ToString();

    Console.WriteLine("Enter your message (or 'exit' to close):");
    string message = "default message";
    message = Console.ReadLine() ?? string.Empty;
    message = $"C{messageId}|{correlationId}|{message}";

    if (message == "exit" || message == "Exit" || message == "EXIT" || message == "e" || message == "E" || message == null || message == "" || message == " ")
    {
        break;
    }

    var body = Encoding.UTF8.GetBytes(message);
    var props = channel.CreateBasicProperties();
    props.ReplyTo = replyQueue.QueueName;
    props.CorrelationId = correlationId;

    channel.BasicPublish(
        exchange: "",
        routingKey: "requestQueue",
        basicProperties: props,
        body: body
    );
}


Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
