using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;

namespace PlayFabBook
{
    /// <summary>
    /// カタログを管理する。
    /// </summary>
    public static class CatalogManager
    {
        /// <summary>
        /// カタログアイテム。
        /// </summary>
        public static Dictionary<string, CatalogItem> CatalogItems { get; private set; }

        /// <summary>
        /// PlayFab から Client へデータを同期する。
        /// </summary>
        /// <returns></returns>
        public static async UniTask SyncPlayFabToClientAsync()
        {
            var response = await PlayFabClientAPI.GetCatalogItemsAsync(new GetCatalogItemsRequest());
            if (response.Error != null)
                throw new PlayFabErrorException(response.Error);

            CatalogItems = response.Result.Catalog.ToDictionary(x => x.ItemId);
        }
    }
}