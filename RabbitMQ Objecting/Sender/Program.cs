using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

public class Visit
{
    public long Id { get; set; }
    public long DoctorId { get; set; }
    public string? Description { get; set; }
    public long PatientId { get; set; }
}
class Program
{
    private static readonly string _url = "amqps://mswaottz:SVECk7VkYXbpms7jfEEmth8BE5OrUQcf@cow.rmq2.cloudamqp.com/mswaottz";
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { Uri = new Uri(_url) };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        var visit = new Visit
        {
            Id = 9,
            DoctorId = 9,
            Description = "Description",
            PatientId = 9
        };


        channel.ExchangeDeclare(
            exchange: "visitExchange",
            type: ExchangeType.Direct
        );

        channel.QueueDeclare(
            queue: "visitQueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        channel.QueueBind(
            queue: "visitQueue",
            exchange: "visitExchange",
            routingKey: ""
        );

        var message = System.Text.Json.JsonSerializer.Serialize(visit);
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
            exchange: "visitExchange",
            routingKey: "visitQueue",
            basicProperties: null,
            body: body
        );

        Console.WriteLine("Sent message: {0}", message);

        Console.WriteLine(" Press [enter] to exit.");

        Console.ReadLine();


    }
}