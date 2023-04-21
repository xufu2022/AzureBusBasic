// See https://aka.ms/new-console-template for more information
using Azure.Messaging.ServiceBus;
using System.Diagnostics;

string connectionString = "Endpoint=sb://.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=TiDLl7VuGJjJoGuTZW2w7SChndhpicz+b+ASbO4b2Gg=";
string queueName = "queue001";
//await Process();
await Receive();

async Task Process()
{
    await using (ServiceBusClient client=new ServiceBusClient(connectionString))
    {
        var sender = client.CreateSender(queueName);
        var message = new ServiceBusMessage("Hello World");
        await sender.SendMessageAsync(message);
        Console.WriteLine($"send a message to the queue : {queueName}");
    }
}

async Task Receive()
{
    await using var client = new ServiceBusClient(connectionString);
    var receiver=client.CreateReceiver(queueName);
    while (true)
    {
        ServiceBusReceivedMessage message=await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(100));
        if(message != null)
        {
            Console.WriteLine($"Receive: {message.Body}");
            Console.WriteLine($"Message Id: {message.MessageId}");
            Console.WriteLine($"Sequence Number: {message.SequenceNumber}");
            await receiver.CompleteMessageAsync(message);
        }
        else
        {
            break;
        }
        await Console.Out.WriteLineAsync("No more Message in the queue");
    }
}