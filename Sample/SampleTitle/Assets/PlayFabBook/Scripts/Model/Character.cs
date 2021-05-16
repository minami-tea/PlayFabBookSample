using Newtonsoft.Json;

namespace PlayFabBook
{
    public class Character
    {
        public string InstanceId { get; set; }
        public string CharacterId { get; set; }
        public int Level { get; set; }
        [JsonIgnore] public int Hp => (int)(Master.Hp + Master.Hp * Level * 0.1);
        [JsonIgnore] public int Atk => (int)(Master.Atk + Master.Atk * Level * 0.1);
        [JsonIgnore] public CharacterMaster Master => TitleDataManager.CharacterMaster[CharacterId];

        public static Character Create(string instanceId, string characterId)
        {
            return new Character
            {
                InstanceId = instanceId,
                CharacterId = characterId,
                Level = 1
            };
        }
    }
}