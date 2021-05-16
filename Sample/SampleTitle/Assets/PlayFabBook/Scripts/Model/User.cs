using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace PlayFabBook
{
    /// <summary>
    /// PlayFab の UserData として記録するユーザー情報。
    /// </summary>
    public class User
    {
        public Gender Gender { get; set; }
        public TutorialFlag TutorialFlag { get; set; }
        public Deck Deck { get; set; }
        public Dictionary<string, Character> Characters { get; set; }
        public Mission Mission { get; set; }

        /// <summary>
        /// 新規ユーザーを作成する。
        /// </summary>
        /// <returns></returns>
        public static User Create()
        {
            var user = new User
            {
                Mission = Mission.Create()
            };

            return user;
        }

        /// <summary>
        /// レベルアップする。
        /// </summary>
        /// <returns></returns>
        public async UniTask LevelUpAsync(bool updatePlayFabUserData = true)
        {
            // 統計情報とミッション情報を更新する。
            await UniTask.WhenAll(
                PlayerProfileManager.UpdateUserLevelAsync(UserDataManager.Level + 1),
                UserDataManager.Mission.AddMissionActionCount(MissionAction.Level, 1, updatePlayFabUserData));
        }

        /// <summary>
        /// デッキを新規作成する。
        /// </summary>
        /// <returns></returns>
        public async UniTask CreateNewDeckAsync()
        {
            if (UserDataManager.User.Deck is null)
            {
                var newDeck = new Deck
                {
                    InstanceIds = UserDataManager.User.Characters
                        .Take(3)
                        .Select((x, i) => (key: i, characterId: x.Key))
                        .ToDictionary(x => x.key, x => x.characterId)
                };

                UserDataManager.User.Deck = newDeck;
                await UpdateTutorialFlagAsync(TutorialFlag.CreateDeck);
            }
        }

        /// <summary>
        /// デッキを更新する。
        /// </summary>
        /// <param name="deckCharacters"></param>
        /// <returns></returns>
        public async UniTask UpdateDeckAsync(Dictionary<int, string> deckCharacters)
        {
            UserDataManager.User.Deck.InstanceIds = deckCharacters;
            await UserDataManager.UpdatePlayFab();
        }

        /// <summary>
        /// 強化・合成する。
        /// </summary>
        /// <param name="targetInstanceId"></param>
        /// <param name="materialInstanceId"></param>
        /// <returns></returns>
        public async UniTask SyntheticCharacterAsync(string targetInstanceId, string materialInstanceId)
        {
            Characters[targetInstanceId].Level += Characters[materialInstanceId].Level;
            Characters.Remove(materialInstanceId);

            await InventoryManager.ConsumeSyntheticMaterialCharacter(materialInstanceId);
            await UserDataManager.UpdatePlayFab();
        }

        /// <summary>
        /// チュートリアルフラグを更新する。
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public async UniTask UpdateTutorialFlagAsync(TutorialFlag flag)
        {
            UserDataManager.User.TutorialFlag |= flag;
            await UserDataManager.UpdatePlayFab();
        }
    }
}