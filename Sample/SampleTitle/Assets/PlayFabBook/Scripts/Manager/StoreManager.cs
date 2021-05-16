using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;

namespace PlayFabBook
{
    public static class StoreManager
    {
        public static Dictionary<StoreId, Dictionary<string, StoreItem>> StoreItems { get; private set; }
            = new Dictionary<StoreId, Dictionary<string, StoreItem>>
            {
                { StoreId.MainStore, null },
                { StoreId.GachaStore, null },
                { StoreId.DummyStore, null }
            };

        /// <summary>
        /// PlayFab から最新のデータを取得してローカルにキャッシュする。
        /// </summary>
        /// <returns></returns>
        public static async UniTask SyncPlayFabToClientAsync()
        {
            var (mainStoreResponse, gachaStoreResponse, dummyStoreResponse) = await UniTask.WhenAll(
                PlayFabClientAPI.GetStoreItemsAsync(new GetStoreItemsRequest { StoreId = StoreId.MainStore.ToString() }).AsUniTask(),
                PlayFabClientAPI.GetStoreItemsAsync(new GetStoreItemsRequest { StoreId = StoreId.GachaStore.ToString() }).AsUniTask(),
                PlayFabClientAPI.GetStoreItemsAsync(new GetStoreItemsRequest { StoreId = StoreId.DummyStore.ToString() }).AsUniTask());

            if (mainStoreResponse.Error != null)
                throw new PlayFabErrorException(mainStoreResponse.Error);

            if (gachaStoreResponse.Error != null)
                throw new PlayFabErrorException(gachaStoreResponse.Error);

            if (dummyStoreResponse.Error != null)
                throw new PlayFabErrorException(dummyStoreResponse.Error);

            StoreItems[StoreId.MainStore] = mainStoreResponse.Result.Store.OrderBy(x => x.DisplayPosition).ToDictionary(y => y.ItemId);
            StoreItems[StoreId.GachaStore] = gachaStoreResponse.Result.Store.OrderBy(x => x.DisplayPosition).ToDictionary(y => y.ItemId);
            StoreItems[StoreId.DummyStore] = dummyStoreResponse.Result.Store.OrderBy(x => x.DisplayPosition).ToDictionary(y => y.ItemId);
        }

        /// <summary>
        /// ストアで商品を購入します。
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="itemId"></param>
        /// <param name="vc"></param>
        /// <returns></returns>
        public static async UniTask<PurchaseItemResult> PurchaseItemAsync(StoreId storeId, string itemId, string vc, bool login = true)
        {
            var item = StoreItems[storeId][itemId];
            var price = (int)item.VirtualCurrencyPrices[vc];
            var request = new PurchaseItemRequest { StoreId = storeId.ToString(), ItemId = itemId, VirtualCurrency = vc, Price = price };

            var response = await PlayFabClientAPI.PurchaseItemAsync(request);
            if (response.Error != null)
                throw new PlayFabErrorException(response.Error);

            if (login)
                await LoginManager.LoginAndUpdateLocalCacheAsync();

            return response.Result;
        }
    }
}