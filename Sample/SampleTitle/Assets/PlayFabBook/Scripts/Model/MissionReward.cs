namespace PlayFabBook
{
    public class MissionInfo
    {
        public int MissionId { get; set; }
        public MissionMaster Master { get; set; }
        public string RewardName { get; set; }
        public int CurrentNum { get; set; }
        public bool IsCompleted { get; set; }
        public bool ReceivedReward { get; set; }
    }
}