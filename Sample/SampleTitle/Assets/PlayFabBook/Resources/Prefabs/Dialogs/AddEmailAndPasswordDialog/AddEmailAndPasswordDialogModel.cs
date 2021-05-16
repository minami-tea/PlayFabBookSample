using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AddEmailAndPasswordDialogModel
{
    public static AddEmailAndPasswordDialogModel Instance { get; } = new AddEmailAndPasswordDialogModel();

    /// <summary>
    /// インプットフィールドを検証する。
    /// </summary>
    /// <returns></returns>
    public (bool isSuccess, string errorMessage) InputValidation(string mail, string password, string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(mail) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            return (false, "入力されていない箇所があります。");

        if (password != confirmPassword)
            return (false, "パスワードと確認用パスワードが一致していません。");

        return (true, string.Empty);
    }

    /// <summary>
    /// アカウントに Email と Password をセットして回復可能なログインアカウントにする。
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async UniTask<(bool isSuccess, string errorMessage)> SetEmailAndPasswordAsync(string email, string password)
    {
        var request = new AddUsernamePasswordRequest
        {
            Username = PlayerPrefsManager.UserId,
            Email = email,
            Password = password,
        };

        var response = await PlayFabClientAPI.AddUsernamePasswordAsync(request);
        if (response.Error != null)
        {
            // ドキュメントを参考にエラーハンドリングする。
            // https://docs.microsoft.com/en-us/rest/api/playfab/client/account-management/addusernamepassword?view=playfab-rest#addusernamepasswordresult
            switch (response.Error.Error)
            {
                case PlayFabErrorCode.InvalidParams:
                    return (false, "有効なメールアドレスと6～100文字以内のパスワードを入力してください。");

                case PlayFabErrorCode.EmailAddressNotAvailable:
                    return (false, "このメールアドレスは既に使用されています。");

                case PlayFabErrorCode.InvalidEmailAddress:
                    return (false, "このメールアドレスは使用できません。");

                case PlayFabErrorCode.InvalidPassword:
                    return (false, "このパスワードは無効です。");

                // 想定外のエラーなので例外として処理する。
                default:
                    throw new PlayFabErrorException(response.Error);
            }
        }

        return (true, string.Empty);
    }
}
