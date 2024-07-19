using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;

var receiver = "Analytics";
Console.WriteLine($"{receiver} is running");


var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "myTopicExchange", type: ExchangeType.Topic);

var queueName = channel.QueueDeclare().QueueName;

var consumer = new EventingBasicConsumer(channel);

channel.QueueBind(queue: queueName,
                exchange: "myTopicExchange",
                routingKey: "*.europe.*");

var random = new Random();

consumer.Received += (model, ea) =>
{

    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine("{0} Received {1}", receiver, message);


    var delay = random.Next(1000, 5000);

    if(int.TryParse(message.Split('|')[0], out int number) && number % 2 == 0)
    {
        delay -= 999;
    }
    Task.Delay(delay).Wait();
    
    message = $"{message} - Processed in {delay} ms";

    Console.WriteLine(" {0} Processed {1}", receiver, message);
};

channel.BasicConsume(queue: queueName,
                    autoAck: true,
                    consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
