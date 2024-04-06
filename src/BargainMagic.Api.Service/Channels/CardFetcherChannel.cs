using BargainMagic.Api.Service.Commands;

using System.Threading.Channels;

namespace BargainMagic.Api.Service.Channels
{
    public class CardFetcherChannel
    {
        private readonly Channel<CardFetchCommand> channel;

        public CardFetcherChannel()
        {
            channel = Channel.CreateUnbounded<CardFetchCommand>();
        }

        public ChannelReader<CardFetchCommand> Reader => channel.Reader;

        public ChannelWriter<CardFetchCommand> Writer => channel.Writer;
    }
}
