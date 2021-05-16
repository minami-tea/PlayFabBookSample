using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class AddFriendDialog : DialogBaseMonoBehaviour
    {
        [SerializeField] Button _cancelButton;
        [SerializeField] Button _okButton;
        [SerializeField] TMP_InputField _idInputField;

        private void Start()
        {
            _cancelButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => taskCompletion.TrySetResult(DialogResult.Cancel));
            _okButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SubmitAsync().Forget());
        }

        /// <summary>
        /// フレンドを追加する。
        /// </summary>
        /// <returns></returns>
        private async UniTaskVoid SubmitAsync()
        {
            var playFabId = _idInputField.text;
            var (_, message) = await FriendManager.AddFriendAsync(playFabId);

            // 結果を表示しつつフレンド情報を更新する。
            await UniTask.WhenAll(
                MessageDialog.ShowAsync(message),
                FriendManager.SyncPlayFabToClientAsync());

            taskCompletion.TrySetResult(DialogResult.OK);
        }

        public static async UniTask<DialogResult> ShowAsync()
        {
            var prefab = await Resources.LoadAsync("Prefabs/Dialogs/AddFriendDialog/AddFriendDialogPrefab");
            var obj = Instantiate(prefab) as GameObject;
            var dialog = obj.GetComponent<AddFriendDialog>();
            dialog.enabled = true;
            var result = await dialog.ClickResult;
            Destroy(obj);
            return result;
        }

    }
}