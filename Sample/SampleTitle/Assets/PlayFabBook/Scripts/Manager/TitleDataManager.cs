using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayFabBook
{
    static class TitleDataManager
    {
        public static Dictionary<int, UserLevelMaster> UserLevelMaster { get; private set; }
        public static Dictionary<string, CharacterMaster> CharacterMaster { get; private set; }
        public static Dictionary<string, EnemyMaster> EnemyMaster { get; private set; }
        public static ILookup<(int queseBaseId, QuestBaseMaster questBaseMaster), QuestDetailMaster> QuestMaster { get; private set; }
        public static Dictionary<int, MissionMaster> MissionMaster { get; private set; }

        /// <summary>
        /// PlayFab から最新のデータを取得してローカルにキャッシュする。
        /// </summary>
        /// <param name="titleData"></param>
        public static void SyncPlayFabToClient(Dictionary<string, string> titleData)
        {
            // ユーザーレベル
            UserLevelMaster = JsonConvert.DeserializeObject<UserLevelMaster[]>(titleData["UserLevelMaster"]).ToDictionary(x => x.Level);

            // 味方キャラクター
            CharacterMaster = JsonConvert.DeserializeObject<CharacterMaster[]>(titleData["CharacterMaster"]).ToDictionary(x => x.CharacterId);

            // 敵キャラクター
            EnemyMaster = JsonConvert.DeserializeObject<EnemyMaster[]>(titleData["EnemyMaster"]).ToDictionary(x => x.EnemyId);
            
            // クエスト
            var questBaseMaster = JsonConvert.DeserializeObject<QuestBaseMaster[]>(titleData["QuestBaseMaster"]).ToDictionary(x => x.QuestBaseId);
            var questDetailMaster = JsonConvert.DeserializeObject<QuestDetailMaster[]>(titleData["QuestDetailMaster"]);
            QuestMaster = questDetailMaster.ToLookup(x => (x.QuestBaseId, questBaseMaster[x.QuestBaseId]));

            // ミッション
            MissionMaster = JsonConvert.DeserializeObject<MissionMaster[]>(titleData["MissionMaster"]).ToDictionary(x => x.MissionId);
        }
    }
}