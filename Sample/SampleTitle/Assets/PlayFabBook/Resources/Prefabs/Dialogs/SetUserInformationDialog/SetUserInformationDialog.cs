using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace PlayFabBook
{
    public class SetUserInformationDialog : DialogBaseMonoBehaviour
    {
        [SerializeField] Button _cancelButton;
        [SerializeField] Button _okButton;
        [SerializeField] Toggle _maleToggle;
        [SerializeField] Toggle _femaleToggle;
        [SerializeField] TMP_InputField _nameInputField;

        private readonly SetUserInformationDialogModel _model = SetUserInformationDialogModel.Instance;

        private void Start()
        {
            _cancelButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => taskCompletion.TrySetResult(DialogResult.Cancel));
            _okButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SubmitAsync().Forget());
        }

        private async UniTaskVoid SubmitAsync()
        {
            // 入力項目を検証して NG ならダイアログを表示する。
            var userDisplayName = _nameInputField.text;
            var (isSuccess, errorMessage, gender) = _model.ValidateInputAndGetGender(userDisplayName, _maleToggle.isOn, _femaleToggle.isOn);
            if (!isSuccess)
            {
                await MessageDialog.ShowAsync(errorMessage);
                return;
            }

            // ユーザーの表示名と性別を登録する。
            (isSuccess, errorMessage) = await _model.SetUserInformationAsync(userDisplayName, gender);
            if (!isSuccess)
            {
                await MessageDialog.ShowAsync(errorMessage);
                return;
            }

            await MessageDialog.ShowAsync("登録が完了しました。");
            taskCompletion.TrySetResult(DialogResult.OK);
        }

        public static async UniTask<DialogResult> ShowAsync()
        {
            var prefab = await Resources.LoadAsync("Prefabs/Dialogs/SetUserInformationDialog/SetUserInformationDialogPrefab");
            var obj = Instantiate(prefab) as GameObject;
            var dialog = obj.GetComponent<SetUserInformationDialog>();
            dialog.enabled = true;
            var result = await dialog.ClickResult;
            Destroy(obj);
            return result;
        }

    }
}