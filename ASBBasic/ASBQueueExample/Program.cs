// See https://aka.ms/new-console-template for more information
using Azure.Messaging.ServiceBus;
using System.Diagnostics;

string connectionString = "Endpoint=sb://fu04002.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=9I6WicTmWqSQNh3uHjLxlKfLMcCtp1H02+ASbC7GNHQ=";
string queueName = "queue001";
var topicName = "topic001";
string subscriptionName = "sub001";
//await BasicProcess();
//await BasicReceive();
//await SendMessageAsync("First Message");
//await SendMessageAsync("second Message");
//await SendMessageAsync("third Message
await ReceiveMessagesAsync();

async Task ReceiveMessagesAsync()
{
    // Create a client that can be used to receive messages
    await using var client = new ServiceBusClient(connectionString);
    var receiver = client.CreateReceiver(topicName, subscriptionName);

    // Receive messages until there are no more messages in the subscription
    while (true)
    {
        // Try to receive a message with a 100-second timeout
        ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(100));

        if (message != null)
        {
            // Process the message here

            // Display the message body and properties
            Console.WriteLine($"Received: {message.Body}");
            Console.WriteLine($"Message ID: {message.MessageId}");
            Console.WriteLine($"Sequence Number: {message.SequenceNumber}");

            // Complete the message so it is not received again
            await receiver.CompleteMessageAsync(message);
        }
        else
        {
            // No more messages in the subscription, exit the loop
            break;
        }
    }

    Console.WriteLine("No more messages in the subscription.");
}
async Task SendMessageAsync(string messageBody)
{
    await using var client = new ServiceBusClient(connectionString);
    var sender = client.CreateSender(topicName);
    var message = new ServiceBusMessage(messageBody);
    await sender.SendMessageAsync(message);
    await Console.Out.WriteLineAsync($"Send: {messageBody}");
}
    async Task BasicProcess()
{
    await using (ServiceBusClient client=new ServiceBusClient(connectionString))
    {
        var sender = client.CreateSender(queueName);
        var message = new ServiceBusMessage("Hello World");
        await sender.SendMessageAsync(message);
        Console.WriteLine($"send a message to the queue : {queueName}");
    }
}

async Task BasicReceive()
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