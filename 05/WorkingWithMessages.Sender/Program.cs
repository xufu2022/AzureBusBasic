// See https://aka.ms/new-console-template for more information

using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Diagnostics;
using WorkingWithMessages.Config;

ServiceBusClient SBClient;
string Sentance = "The quick brown fox jumps over the lazy dog.";

WriteLine("Sender Console - Hit enter", ConsoleColor.White);
Console.ReadLine();

SBClient = new ServiceBusClient(Settings.ConnectionString);

//ToDo: Comment in the appropriate method

//await SendTextString(Sentance);

//await SendPizzaOrderAsync();

//await SendControlMessageAsync();

//await SendPizzaOrderListAsMessagesAsync();

//await SendPizzaOrderListAsBatchAsync();

//for (int i = 0; i < 20; i++)
//{
//    await SendPizzaOrderListAsBatchAsync();
//}

await SendTextStringAsBatchAsync(Sentance);


WriteLine("Sender Console - Complete", ConsoleColor.White);
Console.ReadLine();

async Task SendTextStringAsBatchAsync(string text)
{
    WriteLine("SendTextStringAsBatchAsync", ConsoleColor.Cyan);

    // Create a message sender
    var sender = SBClient.CreateSender(Settings.QueueName);

    Write("Sending:", ConsoleColor.Green);
    using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

    foreach (var letter in text.ToCharArray())
    {
        // Create an empty message and set the subject.
        var message = new ServiceBusMessage
        {
            Subject = letter.ToString(),
            SessionId = Guid.NewGuid().ToString()
        };

        // try adding a message to the batch
        if (!messageBatch.TryAddMessage(message))
        {
            // if it is too large for the batch
            throw new Exception("The message is too large to fit in the batch.");
        }
    }

    await sender.SendMessagesAsync(messageBatch);
    Console.WriteLine();

    // Always close the message sender
    await sender.CloseAsync();
}
async Task SendTextStringAsMessagesAsync(string text)
{
    WriteLine("SendTextStringAsMessagesAsync", ConsoleColor.Cyan);

    // Create a service bus sender
    var sender = SBClient.CreateSender(Settings.QueueName);

    Write("Sending:", ConsoleColor.Green);


    foreach (var letter in text.ToCharArray())
    {
        // Create an empty message and set the label.
        var message = new ServiceBusMessage();
        message.Subject = letter.ToString();

        // Send the message
        await sender.SendMessageAsync(message);
        Write(message.Subject, ConsoleColor.Green);
    }


    Console.WriteLine();
    Console.WriteLine();

    // Always close the message sender
    await sender.CloseAsync();
}
async Task SendTextString(string text)
{
    WriteLine("SendTextStringAsMessagesAsync", ConsoleColor.Cyan);

    // Create a service bus sender
    var sender = SBClient.CreateSender(Settings.QueueName);

    Write("Sending...", ConsoleColor.Green);

    // Create and send a text message.
    var message = new ServiceBusMessage(text);
    message.SessionId = Guid.NewGuid().ToString();
    await sender.SendMessageAsync(message);

    WriteLine("Done!", ConsoleColor.Green);

    // Always close the sender
    await sender.CloseAsync();
}

async Task SendPizzaOrderAsync()
{
    WriteLine("SendPizzaOrderAsync", ConsoleColor.Cyan);

    var order = new PizzaOrder()
    {
        CustomerName = "Alan Smith",
        Type = "Hawaiian",
        Size = "Large"
    };

    // Serialize the order object
    var jsonPizzaOrder = JsonConvert.SerializeObject(order);

    // Create a Message
    var message = new ServiceBusMessage(jsonPizzaOrder)
    {
        Subject = "PizzaOrder",
        ContentType = "application/json",
        SessionId = Guid.NewGuid().ToString()
    };


    // Send the message...
    var sender = SBClient.CreateSender(Settings.QueueName);
    Write("Sending order...", ConsoleColor.Green);
    await sender.SendMessageAsync(message);
    WriteLine("Done!", ConsoleColor.Green);
    Console.WriteLine();
    await sender.CloseAsync();

}
async Task SendControlMessageAsync()
{
    WriteLine("SendControlMessageAsync", ConsoleColor.Cyan);

    // Create a message with no body.
    var message = new ServiceBusMessage
    {
        Subject = "Control",
        SessionId = Guid.NewGuid().ToString()
    };

    // Add some application properties to the property collection
    message.ApplicationProperties.Add("SystemId", 1462);
    message.ApplicationProperties.Add("Command", "Pending Restart");
    message.ApplicationProperties.Add("ActionTime", DateTime.UtcNow.AddHours(2));

    // Send the message
    var sender = SBClient.CreateSender(Settings.QueueName);
    Write("Sending control message...", ConsoleColor.Green);
    await sender.SendMessageAsync(message);
    WriteLine("Done!", ConsoleColor.Green);
    Console.WriteLine();
    await sender.CloseAsync();

}
async Task SendPizzaOrderListAsMessagesAsync()
{
    WriteLine("SendPizzaOrderListAsMessagesAsync", ConsoleColor.Cyan);

    var pizzaOrderList = GetPizzaOrderList();

    // Create a message sender
    var sender = SBClient.CreateSender(Settings.QueueName);

    WriteLine("Sending...", ConsoleColor.Yellow);
    var watch = Stopwatch.StartNew();

    foreach (var pizzaOrder in pizzaOrderList)
    {
        // Create a JSON serialized pizza order
        var jsonPizzaOrder = JsonConvert.SerializeObject(pizzaOrder);
        var message = new ServiceBusMessage(jsonPizzaOrder)
        {
            Subject = "PizzaOrder",
            ContentType = "application/json",
            SessionId = Guid.NewGuid().ToString(),
        };

        // Send the order message
        await sender.SendMessageAsync(message);
    }

    WriteLine($"Sent {pizzaOrderList.Count} orders! - Time: {watch.ElapsedMilliseconds} milliseconds, that's {pizzaOrderList.Count / watch.Elapsed.TotalSeconds} messages per second.", ConsoleColor.Green);
    Console.WriteLine();
    Console.WriteLine();

    // Always close the message sender
    await sender.CloseAsync();
}

async Task SendPizzaOrderListAsBatchAsync()
{
    WriteLine("SendPizzaOrderListAsBatchAsync", ConsoleColor.Cyan);

    var pizzaOrderList = GetPizzaOrderList();

    // Create a message sender
    var sender = SBClient.CreateSender(Settings.QueueName);

    var watch = Stopwatch.StartNew();
    // Create a message batch
    using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

    foreach (var pizzaOrder in pizzaOrderList)
    {
        // Create a JSON serialized pizza order
        var jsonPizzaOrder = JsonConvert.SerializeObject(pizzaOrder);
        var message = new ServiceBusMessage(jsonPizzaOrder)
        {
            Subject = "PizzaOrder",
            ContentType = "application/json",
            SessionId = Guid.NewGuid().ToString()
        };

        // Add the message to the batch
        if (!messageBatch.TryAddMessage(message))
        {
            // If it is too large for the batch
            throw new Exception("The message is too large to fit in the batch.");
        }
    }

    WriteLine("Sending...", ConsoleColor.Yellow);

    // Send the message batch
    await sender.SendMessagesAsync(messageBatch);

    WriteLine($"Sent {pizzaOrderList.Count} orders! - Time: {watch.ElapsedMilliseconds} milliseconds, that's {pizzaOrderList.Count / watch.Elapsed.TotalSeconds} messages per second.", ConsoleColor.Green);
    Console.WriteLine();
    Console.WriteLine();

    // Always close the message sender
    await sender.CloseAsync();
}


List<PizzaOrder> GetPizzaOrderList()
{
    // Create some data
    string[] names = { "Alan", "Jennifer", "James" };
    string[] pizzas = { "Hawaiian", "Vegetarian", "Capricciosa", "Napolitana" };

    var pizzaOrderList = new List<PizzaOrder>();
    for (int pizza = 0; pizza < pizzas.Length; pizza++)
    {
        for (int name = 0; name < names.Length; name++)
        {

            PizzaOrder order = new PizzaOrder()
            {
                CustomerName = names[name],
                Type = pizzas[pizza],
                Size = "Large"
            };
            pizzaOrderList.Add(order);
        }
    }
    return pizzaOrderList;
}

void WriteLine(string text, ConsoleColor color)
{
    var tempColor = Console.ForegroundColor;
    Console.ForegroundColor = color;
    Console.WriteLine(text);
    Console.ForegroundColor = tempColor;
}

  void Write(string text, ConsoleColor color)
{
    var tempColor = Console.ForegroundColor;
    Console.ForegroundColor = color;
    Console.Write(text);
    Console.ForegroundColor = tempColor;
}
