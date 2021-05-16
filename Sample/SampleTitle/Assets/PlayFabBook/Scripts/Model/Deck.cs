using System.Collections.Generic;
using System.Linq;

namespace PlayFabBook
{
    public class Deck
    {
        public Dictionary<int, string> InstanceIds { get; set; }

        public static Deck CreateNewDeck()
        {
            var deck = new Deck
            {
                InstanceIds = UserDataManager.User.Characters
                    .Take(3)
                    .Select((x, i) => (key: i, instanceId: x.Key))
                    .ToDictionary(x => x.key, x => x.instanceId)
            };

            return deck;
        }
    }
}