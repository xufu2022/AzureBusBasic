// See https://aka.ms/new-console-template for more information
using Azure.Messaging.ServiceBus;

string ConnectionString = "Endpoint=sb://fu04002.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=9I6WicTmWqSQNh3uHjLxlKfLMcCtp1H02+ASbC7GNHQ=";
string QueueName = "demoqueue";

string Sentance = "The quick brown fox jumps over the lazy dog";

var client = new ServiceBusClient(ConnectionString);

// Create a service bus sender
var sender = client.CreateSender(QueueName);

// Send some messages
Console.WriteLine("Sending messages...");
foreach (var character in Sentance)
{
    var message = new ServiceBusMessage(character.ToString());
    await sender.SendMessageAsync(message);
    Console.WriteLine($"    Sent: {character}");
}

// Close the sender
await sender.CloseAsync();
Console.WriteLine("Sent messages.");
Console.ReadLine();
