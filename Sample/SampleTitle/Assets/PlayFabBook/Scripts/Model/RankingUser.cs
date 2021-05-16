using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayFabBook
{
    public class RankingUser
    {
        public string PlayFabId { get; private set; }
        public string Name { get; private set; }
        public int Position { get; private set; }
        public int StatValue { get; private set; }
        public int Level { get; private set; }
        public DateTime LastLoginDateTime { get; private set; }
        public string CharacterId { get; private set; }
        public int CharacterLevel { get; private set; }

        public static RankingUser CreateFromPlayerLeaderboardEntry(PlayerLeaderboardEntry entry)
        {
            var friendUser = new RankingUser
            {
                PlayFabId = entry.PlayFabId,
                Name = entry.DisplayName,
                Position = entry.Position + 1, // ポジションは順位を表すが 0 始まりなので +1 しておく
                StatValue = entry.StatValue,
                Level = entry.Profile.Statistics?.FirstOrDefault(x => x.Name == "Level")?.Value ?? 1,
                LastLoginDateTime = entry.Profile.LastLogin ?? DateTime.Now,
                CharacterId = $"character-{string.Format("{0:D8}", entry.Profile.Statistics?.FirstOrDefault(x => x.Name == "CharacterId")?.Value ?? 1)}",
                CharacterLevel = entry.Profile.Statistics?.FirstOrDefault(x => x.Name == "CharacterLevel")?.Value ?? 1
            };

            return friendUser;
        }
    }
}
