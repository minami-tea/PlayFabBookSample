using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;

namespace PlayFabBook
{
    public static class VirtualCurrencyManager
    {
        /// <summary>
        /// スタミナ
        /// </summary>
        public static int Stamina => UserDataManager.MaxStamina > StaminaRaw ? StaminaRaw : UserDataManager.MaxStamina;
        private static int StaminaRaw { get; set; }

        /// <summary>
        /// 経験値
        /// </summary>
        public static int Exp { get; private set; }

        /// <summary>
        /// 魔法石
        /// </summary>
        public static int MagicStone { get; private set; }

        /// <summary>
        /// PlayFab から最新のデータを取得してローカルにキャッシュする。
        /// </summary>
        /// <param name="currency"></param>
        public static void SyncPlayFabToClient(Dictionary<string, int> currency)
        {
            StaminaRaw = currency.TryGetValue("ST", out var st) ? st : 0;
            Exp = currency.TryGetValue("EP", out var ep) ? ep : 0;
            MagicStone = currency.TryGetValue("MS", out var ms) ? ms : 0;
        }

        /// <summary>
        /// スタミナを消費する。
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static async UniTask SubtractStaminaAsync(int amount)
        {
            // PlayFab 内部ではスタミナがユーザーごとのスタミナ最大値を超えていることがあるのでその分もここで考慮して減算する。
            var overMaxStaminaNum = StaminaRaw - UserDataManager.MaxStamina;
            if (overMaxStaminaNum > 0)
                amount = overMaxStaminaNum + amount;

            var request = new SubtractUserVirtualCurrencyRequest
            {
                VirtualCurrency = "ST",
                Amount = amount
            };

            var response = await PlayFabClientAPI.SubtractUserVirtualCurrencyAsync(request);
            if (response.Error != null)
                throw new PlayFabErrorException(response.Error);

            StaminaRaw = response.Result.Balance;
        }

        /// <summary>
        /// スタミナを回復する。
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static async UniTask AddStaminaAsync(int amount)
        {
            var request = new AddUserVirtualCurrencyRequest
            {
                VirtualCurrency = "ST",
                Amount = amount,
            };

            var response = await PlayFabClientAPI.AddUserVirtualCurrencyAsync(request);
            if (response.Error != null)
                throw new PlayFabErrorException(response.Error);

            StaminaRaw = response.Result.Balance;
        }

        /// <summary>
        /// 経験値を増加する。
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static async UniTask<bool> AddExpAsync(int amount, bool login = true)
        {
            var request = new AddUserVirtualCurrencyRequest
            {
                VirtualCurrency = "EP",
                Amount = amount,
            };

            var response = await PlayFabClientAPI.AddUserVirtualCurrencyAsync(request);
            if (response.Error != null)
                throw new PlayFabErrorException(response.Error);

            Exp = response.Result.Balance;
            var isLevelUp = Exp >= UserDataManager.NextLevelInfo.Exp;
            if (isLevelUp)
                await AddStaminaAsync(UserDataManager.NextLevelInfo.MaxStamina);
            
            if (login)
                await LoginManager.LoginAndUpdateLocalCacheAsync();

            return isLevelUp;
        }
    }
}