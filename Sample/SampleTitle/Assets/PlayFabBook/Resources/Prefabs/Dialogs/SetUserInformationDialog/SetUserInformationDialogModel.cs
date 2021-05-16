using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlayFabBook
{
    public class SetUserInformationDialogModel
    {
        public static SetUserInformationDialogModel Instance { get; } = new SetUserInformationDialogModel();

        /// <summary>
        /// 入力を検証して性別を取得する。
        /// </summary>
        /// <returns></returns>
        public (bool isSuccess, string errorMessage, Gender gener) ValidateInputAndGetGender(string name, bool isMale, bool isFemale)
        {
            if (string.IsNullOrWhiteSpace(name))
                return (false, "入力されていない箇所があります。", default);

            if (name.Length < 1)
                return (false, "名前は1文字以上で入力してください。", default);

            if (name.Length > 16)
                return (false, "名前は16文字以内で入力してください。", default);

            if (!isMale && !isFemale)
                return (false, "性別を選択してください。", default);

            if (isMale && isFemale)
                return (false, "性別を選択してください。", default);

            var gender = isMale ? Gender.Male : Gender.Female;
            return (true, string.Empty, gender);
        }

        /// <summary>
        /// ユーザーの表示名と性別を登録する。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        public async UniTask<(bool isSuccess, string errorMessage)> SetUserInformationAsync(string name, Gender gender)
        {
            // ユーザー名を登録する。
            var (isSuccess, errorMessage) = await PlayerProfileManager.UpdateUserDisplayNameAsync(name);
            if (!isSuccess)
                return (isSuccess, errorMessage);

            // ユーザーの性別を登録する。
            return await UserDataManager.SetGenderAsync(gender);
        }
    }

}