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
    /// フレンドを管理する。
    /// </summary>
    public static class FriendManager
    {
        /// <summary>
        /// フレンドを登録できる最大人数。
        /// </summary>
        const int FriendLimitNum = 100;

        /// <summary>
        /// フレンド。
        /// </summary>
        public static Dictionary<string, FriendUser> Friends { get; private set; }

        public static async UniTask SyncPlayFabToClientAsync()
        {
            var request = new GetFriendsListRequest
            {
                ProfileConstraints = new PlayerProfileViewConstraints
                {
                    ShowDisplayName = true,
                    ShowLastLogin = true,
                    ShowStatistics = true,
                }
            };

            var response = await PlayFabClientAPI.GetFriendsListAsync(request);
            if (response.Error != null)
                throw new PlayFabErrorException(response.Error);

            Friends = response.Result.Friends.Select(x => FriendUser.CreateFromFriendInfo(x)).ToDictionary(y => y.PlayFabId);
        }

        /// <summary>
        /// フレンドを登録する。
        /// </summary>
        /// <param name="playFabId"></param>
        /// <returns></returns>
        public static async UniTask<(bool isSuccess, string message)> AddFriendAsync(string playFabId)
        {
            if (Friends.Count >= FriendLimitNum)
                return (false, $"フレンドの人数が上限に達しています。");

            var request = new AddFriendRequest
            {
                FriendPlayFabId = playFabId
            };

            var response = await PlayFabClientAPI.AddFriendAsync(request);
            if (response.Error != null)
            {

                switch (response.Error.Error)
                {
                    case PlayFabErrorCode.AccountNotFound:
                    case PlayFabErrorCode.InvalidParams:
                        return (false, "指定されたユーザーが見つかりませんでした。");

                    case PlayFabErrorCode.UsersAlreadyFriends:
                        return (false, "既にフレンドになっているユーザーです。");

                    default:
                        throw new PlayFabErrorException(response.Error);
                }
            }

            return (true, "フレンドを追加しました。");
        }

        /// <summary>
        /// フレンドを削除する。
        /// </summary>
        /// <param name="playFabId"></param>
        /// <returns></returns>
        public static async UniTask<(bool isSuccess, string message)> RemoveFriendAsync(string playFabId)
        {
            var request = new RemoveFriendRequest
            {
                FriendPlayFabId = playFabId
            };

            var response = await PlayFabClientAPI.RemoveFriendAsync(request);
            if (response.Error != null)
            {
                switch (response.Error.Error)
                {
                    case PlayFabErrorCode.AccountNotFound:
                    case PlayFabErrorCode.InvalidParams:
                        return (false, "指定されたユーザーが見つかりませんでした。");

                    default:
                        throw new PlayFabErrorException(response.Error);
                }
            }

            return (true, "フレンドを解除しました。");
        }
    }
}
