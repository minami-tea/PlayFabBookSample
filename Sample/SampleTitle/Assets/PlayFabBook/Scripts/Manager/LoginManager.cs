using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PlayFabBook
{
    /// <summary>
    /// PlayFab へのログインを管理する。
    /// </summary>
    public static class LoginManager
    {
        /// <summary>
        /// ログインと同時に取得する情報の設定。
        /// </summary>
        public static GetPlayerCombinedInfoRequestParams CombinedInfoRequestParams { get; }
            = new GetPlayerCombinedInfoRequestParams
            {
                GetUserAccountInfo = true,
                GetPlayerProfile = true,
                GetTitleData = true,
                GetUserData = true,
                GetUserInventory = true,
                GetUserVirtualCurrency = true,
                GetPlayerStatistics = true,
            };

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        static LoginManager()
        {
            // PlayFab の TitleId を指定する
            PlayFabSettings.staticSettings.TitleId = "";
        }

        /// <summary>
        /// ユーザーデータとタイトルデータを初期化する。
        /// </summary>
        public static async UniTask LoginAndUpdateLocalCacheAsync()
        {
            // UserId がなければユーザーを新規作成し、UserId があれば既存ユーザーでログインする
            var userId = PlayerPrefsManager.UserId;
            var loginResult = string.IsNullOrEmpty(userId)
                ? await CreateNewUserAsync()
                : await LoadUserAsync(userId);

            await UpdateLocalCacheAsync(loginResult);
        }

        /// <summary>
        /// ログイン時に取得したデータをキャッシュする。
        /// </summary>
        /// <param name="loginResult"></param>
        /// <returns></returns>
        public static async UniTask UpdateLocalCacheAsync(LoginResult loginResult)
        {
            // カタログは他のインスタンスの初期化にも必要なので最初に行うこと
            await UniTask.WhenAll(
                CatalogManager.SyncPlayFabToClientAsync(),
                StoreManager.SyncPlayFabToClientAsync(),
                FriendManager.SyncPlayFabToClientAsync(),
                NewsManager.SyncPlayFabToClientAsync(),
                RankingManager.SyncPlayFabToClientAsync());

            TitleDataManager.SyncPlayFabToClient(loginResult.InfoResultPayload.TitleData);
            PlayerProfileManager.SyncPlayFabToClient(loginResult.InfoResultPayload.PlayerProfile, loginResult.InfoResultPayload.PlayerStatistics);
            InventoryManager.SyncPlayFabToClient(loginResult.InfoResultPayload.UserInventory);
            VirtualCurrencyManager.SyncPlayFabToClient(loginResult.InfoResultPayload.UserVirtualCurrency);
            UserDataManager.SyncPlayFabToClient(loginResult.InfoResultPayload.UserData);

            // ログインボーナス獲得処理
            await CheckAndAddLoginBonusAsync(loginResult);
        }

        /// <summary>
        /// 最終ログイン日時から日付が変わっていればログインボーナスを獲得する。
        /// </summary>
        private static async UniTask CheckAndAddLoginBonusAsync(LoginResult loginResult)
        {
            var loginDateTime = loginResult.InfoResultPayload.AccountInfo.TitleInfo.LastLogin;
            var lastLoginDateTime = loginResult.LastLoginTime;

            if (loginDateTime is null || lastLoginDateTime is null)
                return;

            // PlayFab は UTC なので +9 時間して日本時間に直してから日付を算出する
            var loginDate = (loginDateTime + TimeSpan.FromHours(9))?.Date;
            var lastLoginDate = (lastLoginDateTime + TimeSpan.FromHours(9))?.Date;

            if (loginDate == lastLoginDate)
                return;

            // ログインボーナスを獲得したということはその日は初めてのログインなので
            // デイリーミッションをリセットしたり累積ログイン日数を足したり
            await UserDataManager.Mission.ResetDailyMissionAsync(false);
            await UserDataManager.Mission.AddMissionActionCount(MissionAction.Login, 1);

            // ログインボーナス（今のことろ内容は固定）をプレゼントボックスに付与する
            await StoreManager.PurchaseItemAsync(StoreId.DummyStore, "login-bonus-0001", VirtualCurrencyNames.MS.Code, false);
            
            // PlayerPrefs にログインボーナスを獲得したことを記録しておく
            PlayerPrefsManager.HasLoginBonus = true;
        }

        /// <summary>
        /// 新規ユーザーを作成して UserId を PlayerPrefs に保存する。
        /// </summary>
        private static async UniTask<LoginResult> CreateNewUserAsync()
        {
            while (true)
            {
                // UserId を採番する
                // PlayFab の CustomId として使うなら Guid.NewGuid().Tostring() で十分
                // ただし今回はこれをメールアドレス連携するときの UserId にも使いまわしたいため、記号は使用せず、文字数を20文字以内にしておく
                var newUserId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);

                // ログインリクエストを作成する
                var request = new LoginWithCustomIDRequest
                {
                    CustomId = newUserId,
                    CreateAccount = true,
                    InfoRequestParameters = CombinedInfoRequestParams
                };

                // ログインする
                var response = await PlayFabClientAPI.LoginWithCustomIDAsync(request);
                if (response.Error != null)
                    throw new PlayFabErrorException(response.Error);

                // もし LastLoginTime に値が入っている場合は採番した ID が既存ユーザーと重複しているのでリトライする
                if (response.Result.LastLoginTime.HasValue)
                    continue;

                // PlayerPrefs に UserId を記録する
                PlayerPrefsManager.UserId = newUserId;

                return response.Result;
            }
        }

        /// <summary>
        /// ログインしてユーザーデータをロードする。
        /// </summary>
        /// <param name="userId"></param>
        private static async UniTask<LoginResult> LoadUserAsync(string userId)
        {
            // ログインリクエストを作成する
            var request = new LoginWithCustomIDRequest
            {
                CustomId = userId,
                CreateAccount = false,
                InfoRequestParameters = CombinedInfoRequestParams
            };

            // ログインする
            var response = await PlayFabClientAPI.LoginWithCustomIDAsync(request);
            if (response.Error != null)
                throw new PlayFabErrorException(response.Error);

            return response.Result;
        }
    }
}

