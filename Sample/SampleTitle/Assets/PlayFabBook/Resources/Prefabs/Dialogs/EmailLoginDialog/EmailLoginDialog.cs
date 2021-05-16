using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class EmailLoginDialog : DialogBaseMonoBehaviour
    {
        [SerializeField] Button _okButton;
        [SerializeField] Button _cancelButton;
        [SerializeField] TMP_InputField _mailInputField;
        [SerializeField] TMP_InputField _passwordInputField;

        private readonly EmailLoginDialogModel _model = EmailLoginDialogModel.Instance;

        public static async UniTask<DialogResult> ShowAsync()
        {
            var prefab = await Resources.LoadAsync("Prefabs/Dialogs/EmailLoginDialog/EmailLoginDialogPrefab");
            var obj = Instantiate(prefab) as GameObject;
            var dialog = obj.GetComponent<EmailLoginDialog>();
            dialog.enabled = true;
            var result = await dialog.ClickResult;
            Destroy(obj);
            return result;
        }

        private void Start()
        {
            _okButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => OnClickSubmitAsync().Forget());
            _cancelButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => taskCompletion.TrySetResult(DialogResult.Cancel));
        }

        private async UniTask OnClickSubmitAsync()
        {
            // 入力項目を検証して NG ならダイアログを表示する。
            var (isSuccess, errorMessage) = _model.InputValidation(_mailInputField.text, _passwordInputField.text);
            if (!isSuccess)
            {
                await MessageDialog.ShowAsync(errorMessage);
                return;
            }

            // メールアドレスとパスワードでログインを試みて NG ならダイアログを表示する。
            (isSuccess, errorMessage) = await _model.LoginEmailAndPasswordAsync(_mailInputField.text, _passwordInputField.text);
            if (!isSuccess)
            {
                await MessageDialog.ShowAsync(errorMessage);
                return;
            }

            // PlayerPrefs にメールアドレスによってログインしたフラグを立ててタイトルに戻る
            PlayerPrefsManager.IsLoginEmailAddress = true;
            await MessageDialog.ShowAsync("ログインしました。\nタイトル画面に戻ります。");

            taskCompletion.TrySetResult(DialogResult.OK);
        }
    }
}