using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class ConfirmDialog : DialogBaseMonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _message;
        [SerializeField] Button _cancelButton;
        [SerializeField] Button _okButton;

        private void Start()
        {
            _cancelButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => taskCompletion.TrySetResult(DialogResult.Cancel));
            _okButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => taskCompletion.TrySetResult(DialogResult.OK));
        }

        public static async UniTask<DialogResult> ShowAsync(string message)
        {
            var prefab = await Resources.LoadAsync("Prefabs/Dialogs/ConfirmDialog/ConfirmDialogPrefab");
            var obj = Instantiate(prefab) as GameObject;
            var dialog = obj.GetComponent<ConfirmDialog>();
            dialog._message.text = message;
            dialog.enabled = true;
            var result = await dialog.ClickResult;
            Destroy(obj);
            return result;
        }
    }
}