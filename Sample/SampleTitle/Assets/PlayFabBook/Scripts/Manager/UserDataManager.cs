using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlayFabBook
{
    public static class UserDataManager
    {
        public static int Level => CurrentLevelInfo.Level;
        public static int MaxStamina => CurrentLevelInfo.MaxStamina;
        public static Mission Mission => User.Mission;
        public static Deck Deck => User.Deck;
        public static Dictionary<string, Character> Characters => User.Characters;

        private static UserLevelMaster CurrentLevelInfo { get; set; }
        public static UserLevelMaster NextLevelInfo { get; private set; }

        public static User User { get; private set; }

        /// <summary>
        /// PlayFab から最新のデータを取得してローカルにキャッシュする。
        /// </summary>
        /// <param name="userData"></param>
        public static void SyncPlayFabToClient(Dictionary<string, UserDataRecord> userData)
        {
            var exp = VirtualCurrencyManager.Exp;
            CurrentLevelInfo = TitleDataManager.UserLevelMaster
                .OrderByDescending(x => x.Key)
                .FirstOrDefault(x => x.Value.Exp <= exp).Value;

            NextLevelInfo = TitleDataManager.UserLevelMaster.TryGetValue(CurrentLevelInfo.Level + 1, out var nextLevelInfo)
                ? nextLevelInfo
                : null;

            User = userData.TryGetValue("User", out var user)
                ? JsonConvert.DeserializeObject<User>(user.Value)
                : User.Create();

            User.Characters = InventoryManager.Characters.ToDictionary(
                x => x.InstanceId,
                x =>
                {
                    if (User.Characters is null)
                        return Character.Create(x.InstanceId, x.CharacterId);

                    return User.Characters.TryGetValue(x.InstanceId, out var character)
                        ? character
                        : Character.Create(x.InstanceId, x.CharacterId);
                });
        }

        /// <summary>
        /// PlayFab のユーザーデータを更新する。
        /// </summary>
        /// <returns></returns>
        public static async UniTask<(bool isSuccess, string errorMessage)> UpdatePlayFab()
        {
            var userJson = JsonConvert.SerializeObject(User);
            var request = new UpdateUserDataRequest { Data = new Dictionary<string, string> { { "User", userJson } } };

            var response = await PlayFabClientAPI.UpdateUserDataAsync(request);
            if (response.Error != null)
                throw new PlayFabErrorException(response.Error);

            return (true, string.Empty);
        }

        /// <summary>
        /// ユーザーの性別を登録する。
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public static async UniTask<(bool isSuccess, string errorMessage)> SetGenderAsync(Gender gender)
        {
            User.Gender = gender;
            User.TutorialFlag |= TutorialFlag.SetNameAndGender;
            return await UpdatePlayFab();
        }
    }

}

