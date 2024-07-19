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
