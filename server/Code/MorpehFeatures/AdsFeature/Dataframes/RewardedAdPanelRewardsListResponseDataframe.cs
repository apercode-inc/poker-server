using NetFrame;
using NetFrame.WriteAndRead;

namespace server.Code.MorpehFeatures.AdsFeature.Dataframes;

public struct RewardedAdPanelRewardsListResponseDataframe : INetworkDataframe
{
    public string PanelId;
    public List<RewardItemDataframe> Rewards;
        
    public void Write(NetFrameWriter writer)
    {
        writer.WriteString(PanelId);
            
        int count = Rewards != null ? Rewards.Count : 0;
        writer.WriteInt(count);
            
        if (count == 0) return;

        foreach (var reward in Rewards)
        {
            reward.Write(writer);
        }
    }

    public void Read(NetFrameReader reader)
    {
        PanelId = reader.ReadString();

        Rewards = null;
        int count = reader.ReadInt();
        if (count == 0) return;

        Rewards = new List<RewardItemDataframe>();
        for (int i = 0; i < count; i++)
        {
            var item = new RewardItemDataframe();
            item.Read(reader);
            Rewards.Add(item);
        }
    }
}