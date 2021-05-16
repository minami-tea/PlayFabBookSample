namespace PlayFabBook
{
    public class Present
    {
        public string InstanceId { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public PresentCustomData CustomData { get; set; }
    }

    public class PresentCustomData
    {
        // 今のところ使っていないプレゼントごとに固有のメッセージが必要なら入れても良いかも。
    }
}