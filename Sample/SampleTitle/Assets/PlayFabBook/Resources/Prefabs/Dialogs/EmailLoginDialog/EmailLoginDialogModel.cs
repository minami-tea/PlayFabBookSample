using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayFabBook
{
    public class EmailLoginDialogModel
    {
        public static EmailLoginDialogModel Instance { get; } = new EmailLoginDialogModel();

        /// <summary>
        /// インプットフィールドを検証する。
        /// </summary>
        /// <returns></returns>
        public (bool isSuccess, string errorMessage) InputValidation(string mail, string password)
        {
            if (string.IsNullOrWhiteSpace(mail) || string.IsNullOrWhiteSpace(password))
                return (false, "入力されていない箇所があります。");

            return (true, string.Empty);
        }

        /// <summary>
        /// Email と Password でログインする。
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async UniTask<(bool isSuccess, string errorMessage)> LoginEmailAndPasswordAsync(string email, string password)
        {
            var request = new LoginWithEmailAddressRequest
            {
                Email = email,
                Password = password,
                InfoRequestParameters = LoginManager.CombinedInfoRequestParams
            };

            var response = await PlayFabClientAPI.LoginWithEmailAddressAsync(request);
            if (response.Error != null)
            {
                // ドキュメントを参考にエラーハンドリングする。
                // https://docs.microsoft.com/en-us/rest/api/playfab/client/authentication/loginwithemailaddress?view=playfab-rest
                switch (response.Error.Error)
                {
                    case PlayFabErrorCode.InvalidParams:
                    case PlayFabErrorCode.InvalidEmailOrPassword:
                    case PlayFabErrorCode.AccountNotFound:
                        return (false, "メールアドレスかパスワードが正しくありません。");

                    // 想定外のエラーなので例外として処理する。
                    default:
                        throw new PlayFabErrorException(response.Error);
                }
            }

            // PlayerPregs を初期化して引き継いだ UserId をセットする。
            PlayerPrefs.DeleteAll();
            PlayerPrefsManager.UserId = response.Result.InfoResultPayload.AccountInfo.CustomIdInfo.CustomId;
            PlayerPrefsManager.IsLoginEmailAddress = true;

            // ログインボーナスがあれば獲得する
            await LoginManager.UpdateLocalCacheAsync(response.Result);
    
        return (true, string.Empty);
        }
    }

}