using NetFrame;

namespace server.Code.GlobalUtils.ThreadSafeContainers
{
    public class DynamicInvokeForClientSafeContainer
    {
        public List<Delegate> Handlers;
        public INetworkDataframe Dataframe;
    }
}