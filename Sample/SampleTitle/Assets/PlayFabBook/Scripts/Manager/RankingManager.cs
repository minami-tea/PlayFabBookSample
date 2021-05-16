using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayFabBook
{
    /// <summary>
    /// ランキングを管理する。
    /// </summary>
    public static class RankingManager
    {
        /// <summary>
        /// ユーザーレベルランキング。
        /// </summary>
        public static RankingUser[] UserLevelRanking { get; private set; }

        /// <summary>
        /// PlayFab からランキングデータを取得する。
        /// </summary>
        /// <returns></returns>
        public static async UniTask SyncPlayFabToClientAsync()
        {
            var request = new GetLeaderboardRequest
            {
                StatisticName = "Level",
                ProfileConstraints = new PlayerProfileViewConstraints
                {
                    ShowDisplayName = true,
                    ShowStatistics = true
                }
            };

            var response = await PlayFabClientAPI.GetLeaderboardAsync(request);
            if (response.Error != null)
                throw new PlayFabErrorException(response.Error);

            UserLevelRanking = response.Result.Leaderboard.Select(x => RankingUser.CreateFromPlayerLeaderboardEntry(x)).ToArray();
        }
    }
}
