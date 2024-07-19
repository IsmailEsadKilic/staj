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

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = System.Text.Encoding.UTF8.GetString(body);
            var visit = System.Text.Json.JsonSerializer.Deserialize<Visit>(message) ?? new Visit();
            Console.WriteLine($"Received message: {visit.Id} {visit.DoctorId} {visit.Description} {visit.PatientId}");
        };

        channel.BasicConsume(
            queue: "visitQueue",
            autoAck: true,
            consumer: consumer
        );

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
        // consumer.Received += (model, ea) =>
        // {
        //     var body = ea.Body.ToArray();
        //     var message = Encoding.UTF8.GetString(body);
        //     Console.WriteLine(" Consumer Received {0}", message);
        // };
    }
}