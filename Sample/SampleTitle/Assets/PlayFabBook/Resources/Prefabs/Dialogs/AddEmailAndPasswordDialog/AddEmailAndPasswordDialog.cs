using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class AddEmailAndPasswordDialog : DialogBaseMonoBehaviour
    {
        [SerializeField] Button _okButton;
        [SerializeField] Button _cancelButton;
        [SerializeField] TMP_InputField _mailInputField;
        [SerializeField] TMP_InputField _passwordInputField;
        [SerializeField] TMP_InputField _confirmPasswordInputField;

        private readonly AddEmailAndPasswordDialogModel _model = AddEmailAndPasswordDialogModel.Instance;

        private void Start()
        {
            _okButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SubmitAsync().Forget());
            _cancelButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => taskCompletion.TrySetResult(DialogResult.Cancel));
        }

        private async UniTaskVoid SubmitAsync()
        {
            // 入力項目を検証して NG ならダイアログを表示する。
            var (isSuccess, errorMessage) = _model.InputValidation(_mailInputField.text, _passwordInputField.text, _confirmPasswordInputField.text);
            if (!isSuccess)
            {
                await MessageDialog.ShowAsync(errorMessage);
                return;
            }

            // メールアドレスとパスワードの登録を試みて NG ならダイアログを表示する。
            (isSuccess, errorMessage) = await _model.SetEmailAndPasswordAsync(_mailInputField.text, _passwordInputField.text);
            if (!isSuccess)
            {
                await MessageDialog.ShowAsync(errorMessage);
                return;
            }

            await MessageDialog.ShowAsync("アカウント連携が完了しました。");
            taskCompletion.TrySetResult(DialogResult.OK);
        }

        public static async UniTask<DialogResult> ShowAsync()
        {
            var prefab = await Resources.LoadAsync("Prefabs/Dialogs/AddEmailAndPasswordDialog/AddEmailAndPasswordDialogPrefab");
            var obj = Instantiate(prefab) as GameObject;
            var dialog = obj.GetComponent<AddEmailAndPasswordDialog>();
            dialog.enabled = true;
            var result = await dialog.ClickResult;
            Destroy(obj);
            return result;
        }

    }
}