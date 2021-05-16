using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayFabBook
{
    public class FriendUser
    {
        public string PlayFabId { get; private set; }
        public string Name { get; private set; }
        public int Level { get; private set; }
        public DateTime LastLoginDateTime { get; private set; }
        public string CharacterId { get; private set; }
        public int CharacterLevel { get; private set; }

        public static FriendUser CreateFromFriendInfo(FriendInfo friendInfo)
        {
            var friendUser = new FriendUser
            {
                PlayFabId = friendInfo.FriendPlayFabId,
                Name = friendInfo.Profile.DisplayName,
                Level = friendInfo.Profile.Statistics?.FirstOrDefault(x => x.Name == "Level")?.Value ?? 1,
                LastLoginDateTime = friendInfo.Profile.LastLogin ?? DateTime.Now,
                CharacterId = $"character-{string.Format("{0:D8}", friendInfo.Profile.Statistics?.FirstOrDefault(x => x.Name == "CharacterId")?.Value ?? 1)}",
                CharacterLevel = friendInfo.Profile.Statistics?.FirstOrDefault(x => x.Name == "CharacterLevel")?.Value ?? 1
            };

            return friendUser;
        }
    }
}
