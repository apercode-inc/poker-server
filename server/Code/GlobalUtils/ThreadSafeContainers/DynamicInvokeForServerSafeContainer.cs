using NetFrame;

namespace server.Code.GlobalUtils.ThreadSafeContainers
{
    public class DynamicInvokeForServerSafeContainer
    {
        public List<Delegate> Handlers;
        public INetworkDataframe Dataframe;
        public int Id;
    }
}