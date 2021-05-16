using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;

namespace PlayFabBook
{
    /// <summary>
    /// インベントリを管理する。
    /// </summary>
    public static class InventoryManager
    {
        /// <summary>
        /// 所持キャラクター。
        /// </summary>
        public static List<(string InstanceId, string CharacterId)> Characters { get; private set; }

        /// <summary>
        /// 所持プレゼント。
        /// </summary>
        public static Dictionary<string, Present> Presents { get; private set; }

        /// <summary>
        /// 所持スタミナ回復薬数。
        /// </summary>
        public static (string InstanceId, int Num) StaminaRecovery { get; private set; }

        /// <summary>
        /// PlayFab から Client へデータを同期する。
        /// </summary>
        /// <param name="inventory"></param>
        public static void SyncPlayFabToClient(IEnumerable<ItemInstance> inventory)
        {
            Characters = inventory
                .Where(x => x.ItemClass == ItemClass.Character.ToString())
                .Select(y => (y.ItemInstanceId, y.ItemId)).ToList();

            Presents = inventory
                .Where(x => x.ItemClass == ItemClass.Present.ToString())
                .ToDictionary(y => y.ItemInstanceId, y => y.ToPresent());

            var staminaRecovery = inventory.FirstOrDefault(x => x.ItemId == ItemIds.StaminaRecovery);
            StaminaRecovery = staminaRecovery is null ? (string.Empty, 0) : (staminaRecovery.ItemInstanceId, staminaRecovery.RemainingUses ?? 0);
        }

        /// <summary>
        /// スタミナ回復薬を使用する。
        /// </summary>
        /// <returns></returns>
        public static async UniTask ConsumeStaminaRecoveryItemAsync()
        {
            var consumeItemRequest = new ConsumeItemRequest
            {
                ItemInstanceId = StaminaRecovery.InstanceId,
                ConsumeCount = 1
            };

            var (response, _) = await UniTask.WhenAll(
                PlayFabClientAPI.ConsumeItemAsync(consumeItemRequest).AsUniTask(),
                VirtualCurrencyManager.AddStaminaAsync(UserDataManager.MaxStamina).AsAsyncUnitUniTask());

            if (response.Error != null)
                throw new PlayFabErrorException(response.Error);

            StaminaRecovery = (response.Result.ItemInstanceId, response.Result.RemainingUses);            
        }

        /// <summary>
        /// 強化・合成で素材にしたキャラクターを消費（削除）する
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public static async UniTask ConsumeSyntheticMaterialCharacter(string instanceId)
        {
            var targetCharacter = Characters.Single(x => x.InstanceId == instanceId);
            Characters.Remove(targetCharacter);

            var result = await PlayFabClientAPI.ConsumeItemAsync(new ConsumeItemRequest { ItemInstanceId = instanceId, ConsumeCount = 1});
            if (result.Error != null)
                throw new PlayFabErrorException(result.Error);
        }

        /// <summary>
        /// プレゼントを開封する。
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="reloadPlayFabData"></param>
        /// <returns></returns>
        public static async UniTask OpenPresentAsync(string instanceId, bool reloadPlayFabData = true)
        {
            var request = new UnlockContainerInstanceRequest
            {
                ContainerItemInstanceId = instanceId
            };

            var result = await PlayFabClientAPI.UnlockContainerInstanceAsync(request);
            if (result.Error != null)
                throw new PlayFabErrorException(result.Error);

            if (reloadPlayFabData)
                await LoginManager.LoginAndUpdateLocalCacheAsync();
        }

        /// <summary>
        /// すべてのプレゼントを開封する。
        /// </summary>
        /// <returns></returns>
        public static async UniTask OpenAllPresentAsync()
        {
            await UniTask.WhenAll(Presents.Select(x => OpenPresentAsync(x.Key, false)));
            await LoginManager.LoginAndUpdateLocalCacheAsync();
        }
    }
}