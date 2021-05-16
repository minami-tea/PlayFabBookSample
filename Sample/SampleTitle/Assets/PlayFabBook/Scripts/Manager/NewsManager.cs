using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;

namespace PlayFabBook
{
    /// <summary>
    /// ニュースを管理する。
    /// </summary>
    public static class NewsManager
    {
        /// <summary>
        /// カタログアイテム。
        /// </summary>
        public static IReadOnlyCollection<TitleNewsItem> NewsItems { get; private set; }

        /// <summary>
        /// PlayFab から Client へデータを同期する。
        /// </summary>
        /// <returns></returns>
        public static async UniTask SyncPlayFabToClientAsync()
        {
            var response = await PlayFabClientAPI.GetTitleNewsAsync(new GetTitleNewsRequest());
            if (response.Error != null)
                throw new PlayFabErrorException(response.Error);

            NewsItems = response.Result.News.AsReadOnly();
        }
    }
}