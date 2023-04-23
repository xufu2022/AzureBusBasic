// See https://aka.ms/new-console-template for more information
using Azure.Messaging.ServiceBus;

string ConnectionString = "Endpoint=sb://fu04002.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=9I6WicTmWqSQNh3uHjLxlKfLMcCtp1H02+ASbC7GNHQ=";
string QueueName = "demoqueue";


var client = new ServiceBusClient(ConnectionString);

// Create a service bus receiver
var receiver = client.CreateReceiver(QueueName);


// Receive the messages
Console.WriteLine("Receive messages...");
while (true)
{
    var message = await receiver.ReceiveMessageAsync();

    if (message != null)
    {
        Console.Write(message.Body.ToString());

        // Complete the message
        await receiver.CompleteMessageAsync(message);
    }
    else
    {
        Console.WriteLine();
        Console.WriteLine("All messages received.");
        break;
    }
}

// Close the receiver
await receiver.CloseAsync();