// See https://aka.ms/new-console-template for more information

using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Newtonsoft.Json;
using WorkingWithMessages.Config;

ServiceBusClient SBClient;
WriteLine("Receiver Console", ConsoleColor.White);

SBClient = new ServiceBusClient(Settings.ConnectionString);

//await RecreateQueueAsync();


//Comment in the appropriate method

//await ReceiveAndProcessText(1);

//await ReceiveAndProcessPizzaOrdes(1);
//await ReceiveAndProcessPizzaOrdes(5);
//await ReceiveAndProcessPizzaOrdes(100);

await ReceiveAndProcessControlMessage(1);

//await ReceiveAndProcessCharacters(1);

//await ReceiveAndProcessCharacters(16);


//WriteLine("Receiving, hit enter to exit", ConsoleColor.White);
//Console.ReadLine();
//StopReceivingAsync().Wait();
async Task ReceiveAndProcessPizzaOrdes(int threads)
{
    WriteLine($"ReceiveAndProcessPizzaOrdes({threads})", ConsoleColor.Cyan);

    // Set the oprions for processing messages
    var options = new ServiceBusProcessorOptions
    {
        AutoCompleteMessages = false,
        MaxConcurrentCalls = threads,
        MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(10)
    };

    // Create a message processor
    var processor = SBClient.CreateProcessor(Settings.QueueName, options);

    // Add handler to process messages
    processor.ProcessMessageAsync += ProcessPizzaMessageAsync;


    // Add handler to process any errors
    processor.ProcessErrorAsync += ErrorHandler;

    // Start the message processor
    await processor.StartProcessingAsync();


    WriteLine("Receiving, hit enter to exit", ConsoleColor.White);
    Console.ReadLine();

    // Stop and close the message processor
    await processor.StopProcessingAsync();
    await processor.CloseAsync();
}
async Task ProcessPizzaMessageAsync(ProcessMessageEventArgs args)
{

    // Deserialize the message body.           
    var pizzaOrder = JsonConvert.DeserializeObject<PizzaOrder>(args.Message.Body.ToString());

    // Process the message
    CookPizza(pizzaOrder);

    // Complete the message receive operation
    await args.CompleteMessageAsync(args.Message);



}
void CookPizza(PizzaOrder order)
{
    WriteLine($"Cooking {order.Type} for {order.CustomerName}.", ConsoleColor.Yellow);
    Thread.Sleep(5000);
    WriteLine($"    {order.Type} pizza for {order.CustomerName} is ready!", ConsoleColor.Green);
}
async Task ReceiveAndProcessText(int threads)
{
    WriteLine($"ReceiveAndProcessText({threads})", ConsoleColor.Cyan);


    // Set the oprions for processing messages
    var options = new ServiceBusProcessorOptions
    {
        AutoCompleteMessages = false,
        MaxConcurrentCalls = threads,
        MaxAutoLockRenewalDuration = TimeSpan.FromSeconds(30)
    };


    // Create a message processor
    var processor = SBClient.CreateProcessor(Settings.QueueName, options);

    // add handler to process messages
    processor.ProcessMessageAsync += ProcessTextMessageAsync;


    // add handler to process any errors
    processor.ProcessErrorAsync += ErrorHandler;


    // Start the message processor
    await processor.StartProcessingAsync();


    WriteLine("Receiving, hit enter to exit", ConsoleColor.White);
    Console.ReadLine();

    await processor.StopProcessingAsync();
    await processor.CloseAsync();

    //StopReceivingAsync().Wait();
}

async Task RecreateQueueAsync()
{
    // Create a management client to manage artifacts
    var serviceBusAdministrationClient = new ServiceBusAdministrationClient(Settings.ConnectionString);

    if (await serviceBusAdministrationClient.QueueExistsAsync(Settings.QueueName))
    {
        WriteLine($"Deleting queue: {Settings.QueueName}...", ConsoleColor.Magenta);
        await serviceBusAdministrationClient.DeleteQueueAsync(Settings.QueueName);
        WriteLine("Done!", ConsoleColor.Magenta);
    }

    WriteLine($"Creating queue: {Settings.QueueName}...", ConsoleColor.Magenta);
    await serviceBusAdministrationClient.CreateQueueAsync(Settings.QueueName);
    WriteLine("Done!", ConsoleColor.Magenta);
}

void WriteLine(string text, ConsoleColor color)
{
    var tempColor = Console.ForegroundColor;
    Console.ForegroundColor = color;
    Console.WriteLine(text);
    Console.ForegroundColor = tempColor;
}
async Task ReceiveAndProcessCharacters(int threads)
{
    WriteLine($"ReceiveAndProcessCharacters({threads})", ConsoleColor.Cyan);

    // Set the options for processing messages
    var options = new ServiceBusProcessorOptions
    {
        AutoCompleteMessages = false,
        MaxConcurrentCalls = threads,
        MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(10),
    };

    // Create a message processor
    var processor = SBClient.CreateProcessor(Settings.QueueName, options);

    // Add handler to process messages
    processor.ProcessMessageAsync += ProcessCharacterMessageAsync;


    // Add handler to process any errors
    processor.ProcessErrorAsync += ErrorHandler;

    // Start the message processor
    await processor.StartProcessingAsync();


    WriteLine("Receiving, hit enter to exit", ConsoleColor.White);
    Console.ReadLine();

    // Stop and close the message processor
    await processor.StopProcessingAsync();
    await processor.CloseAsync();
}

async Task ReceiveAndProcessControlMessage(int threads)
{
    WriteLine($"ReceiveAndProcessControlMessage({threads})", ConsoleColor.Cyan);




    // Set the oprions for processing messages
    var options = new ServiceBusProcessorOptions
    {
        AutoCompleteMessages = false,
        MaxConcurrentCalls = threads,
        MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(10)
    };

    // Create a message processor
    var processor = SBClient.CreateProcessor(Settings.QueueName, options);

    // add handler to process messages
    processor.ProcessMessageAsync += ProcessControlMessageAsync;


    // add handler to process any errors
    processor.ProcessErrorAsync += ErrorHandler;

    // Start the message processor
    await processor.StartProcessingAsync();

    WriteLine("Receiving, hit enter to exit", ConsoleColor.White);
    Console.ReadLine();

    // Stop and close the message processor
    await processor.StopProcessingAsync();
    await processor.CloseAsync();

    //StopReceivingAsync().Wait();

}


async Task ProcessCharacterMessageAsync(ProcessMessageEventArgs args)
{
    Write(args.Message.Subject, ConsoleColor.Green);

    // Complete the message receive operation
    await args.CompleteMessageAsync(args.Message);
}
void Write(string text, ConsoleColor color)
{
    var tempColor = Console.ForegroundColor;
    Console.ForegroundColor = color;
    Console.Write(text);
    Console.ForegroundColor = tempColor;
}

async Task ProcessTextMessageAsync(ProcessMessageEventArgs args)
{

    // Deserialize the message body.
    var messageBodyText = args.Message.Body.ToString();

    WriteLine($"Received: {messageBodyText}", ConsoleColor.Green);

    // Complete the message receive operation
    await args.CompleteMessageAsync(args.Message);

}

async Task ProcessControlMessageAsync(ProcessMessageEventArgs args)
{
    WriteLine($"Received: {args.Message.Subject}", ConsoleColor.Green);

    WriteLine("Application properties...", ConsoleColor.Yellow);
    foreach (var property in args.Message.ApplicationProperties)
    {
        WriteLine($"    {property.Key} - {property.Value}", ConsoleColor.Cyan);
    }

    // Complete the message receive operation
    await args.CompleteMessageAsync(args.Message);

}

async Task ErrorHandler(ProcessErrorEventArgs args)
{
    WriteLine(args.Exception.Message, ConsoleColor.Red);
}

