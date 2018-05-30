using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QueueProcessor
{
    class Program
    {
        static async Task Main(string[] args)
        {
#if DEBUG
            Environment.SetEnvironmentVariable("AccountName", "");
            Environment.SetEnvironmentVariable("AccountKey", "");
            Environment.SetEnvironmentVariable("QueueName", "");
#endif

            Console.WriteLine("Container started");
            Console.WriteLine($"Name of hosting VM: {Environment.MachineName}");

            await GenerateMessagesAsync();
            int processedMessages = await StartAsync();

            Console.WriteLine($"Container processed {processedMessages} messages");
        }

        static async Task<int> StartAsync()
        {
            CloudStorageAccount storage = CloudStorageAccount.Parse(Config.ConnectionString);
            CloudQueueClient client = storage.CreateCloudQueueClient();
            CloudQueue queue = client.GetQueueReference(Config.QueueName);

            int processedMessages = 0;
            while (true)
            {
                // in real scenario more messages should be polled in one call
                var messages = await queue.GetMessagesAsync(2, TimeSpan.FromSeconds(30), null, null);
                foreach (CloudQueueMessage message in messages)
                {
                    // do some magic with the message
                    Console.WriteLine(message.AsString);

                    // simulate some work
                    await Task.Delay(500);

                    // do not forget to delete the message from the queue
                    await queue.DeleteMessageAsync(message);

                    processedMessages++;
                }

                if (!messages.Any())
                    return processedMessages;
            }
        }

        static async Task GenerateMessagesAsync()
        {
            CloudStorageAccount storage = CloudStorageAccount.Parse(Config.ConnectionString);
            CloudQueueClient client = storage.CreateCloudQueueClient();
            CloudQueue queue = client.GetQueueReference(Config.QueueName);
            await queue.CreateIfNotExistsAsync();

            for (int i = 0; i < new Random().Next(10, 40); i++)
            {
                await queue.AddMessageAsync(new CloudQueueMessage(Guid.NewGuid().ToString()));
            }
        }
    }

    static class Config
    {
        static string AccountName { get { return Environment.GetEnvironmentVariable("AccountName"); } }
        static string AccountKey { get { return Environment.GetEnvironmentVariable("AccountKey"); } }
        public static string QueueName { get { return Environment.GetEnvironmentVariable("QueueName"); } }
        public static string ConnectionString
        {
            get
            {
                return $"DefaultEndpointsProtocol=https;AccountName={AccountName};AccountKey={AccountKey};EndpointSuffix=core.windows.net";
            }
        }
    }
}
