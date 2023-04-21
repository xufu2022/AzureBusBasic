using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace ASBQueueExzample
{
    class ReceiveMessage
    {
        // Define connection string, topic name and subscription name
        static string connectionString = "Endpoint=sb://fu04002.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=9I6WicTmWqSQNh3uHjLxlKfLMcCtp1H02+ASbC7GNHQ=";
		static string topicName = "topic001";
        static string subscriptionName = "sub001";

        // Define a method to receive messages from the subscription
        static async Task ReceiveMessagesAsync()
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

        static async Task Main(string[] args)
        {
            Console.WriteLine("Receiving messages from Azure Service Bus Topic...");

            // Call the method to receive messages from the subscription
            await ReceiveMessagesAsync();

            Console.WriteLine("Done.");
        }
    }
}