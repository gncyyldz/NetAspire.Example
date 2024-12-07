using MassTransit;
using Shared;

namespace ServiceA.Consumers
{
    public class ServiceBSentMessageConsumer : IConsumer<Message>
    {
        public Task Consume(ConsumeContext<Message> context)
        {
            Console.WriteLine(context.Message.Text);
            return Task.CompletedTask;
        }
    }
}
