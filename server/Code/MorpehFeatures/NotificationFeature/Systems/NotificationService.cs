using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.NotificationFeature.Dataframes;
using server.Code.MorpehFeatures.NotificationFeature.Enums;

namespace server.Code.MorpehFeatures.NotificationFeature.Systems
{
    public class NotificationService : IInitializer
    {
        [Injectable] private NetFrameServer _server;

        public World World { get; set; }

        public void OnAwake()
        {
        }

        public void Show(Entity playerEntity, string messageText, NotificationKind kind)
        {
            var dataframe = new NotificationDataframe
            {
                MessageText = messageText,
                Kind = kind
            };

            _server.Send(ref dataframe, playerEntity);
        }

        public void Dispose()
        {
        }
    }
}