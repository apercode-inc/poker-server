using NetFrame.Server;
using Scellecs.Morpeh;
using server.Code.Injection;
using server.Code.MorpehFeatures.LocalizationFeature;
using server.Code.MorpehFeatures.LocalizationFeature.Dataframes;
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

        public void Show(Entity playerEntity, string messageKey, NotificationType type, List<LocalizationParameter> parameters = null)
        {
            var dataframe = new NotificationDataframe
            {
                MessageText = messageKey,
                Type = type,
                LocalizationParameters = new LocalizationParametersListDataframe
                {
                    Parameters = parameters
                }
            };

            _server.Send(ref dataframe, playerEntity);
        }

        public void Dispose()
        {
        }
    }
}