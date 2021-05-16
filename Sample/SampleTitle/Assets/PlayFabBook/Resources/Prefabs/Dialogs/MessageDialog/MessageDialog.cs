using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class MessageDialog : DialogBaseMonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _message;
        [SerializeField] Button _okButton;

        private void Start()
        {
            _okButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => taskCompletion.TrySetResult(DialogResult.OK));
        }

        public static async UniTask ShowAsync(string message)
        {
            var prefab = await Resources.LoadAsync("Prefabs/Dialogs/MessageDialog/MessageDialogPrefab");
            var obj = (Instantiate(prefab) as GameObject);
            var dialog = obj.GetComponent<MessageDialog>();
            dialog._message.text = message;
            dialog.enabled = true;
            var result = await dialog.ClickResult;
            Destroy(obj);
        }
    }
}