using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayFabBook
{
    public class QuestDetailMaster
    {
        public int QuestBaseId { get; set; }
        public int QuestDetailId { get; set; }
        public string Name { get; set; }
        public int Stamina { get; set; }
        public Story[] BeforeStories { get; set; }
        public BattleWave[] BattleWaves { get; set; }
        public Story[] AfterStories { get; set; }
        public Reward Reward { get; set; }
    }

    public class Story
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public StoryCharacter[] Characters { get; set; }
    }

    public class StoryCharacter
    {
        public StoryCharacterPosition Position { get; set; }
        public string Image { get; set; }
        public StoryCharacterEffect Effect { get; set; }
    }

    public class BattleWave
    {
        public string[] EnemyId { get; set; }
    }

    public class Reward
    {
        public int Exp { get; set; }
        public string ItemId { get; set; }
    }

    public class Prerequisites
    {
        public PrerequisitesType Type { get; set; }
        public string Value { get; set; }
    }
}
