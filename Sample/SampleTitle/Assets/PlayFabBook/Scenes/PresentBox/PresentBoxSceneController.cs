using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class PresentBoxSceneController : MonoBehaviour
    {
        [SerializeField] Button _backButton;
        [SerializeField] Button _openALlButton;

        [SerializeField] Transform _presentBoxAria;
        [SerializeField] GameObject _presentPrefab;

        private Dictionary<string, GameObject> Presents { get; set; } = new Dictionary<string, GameObject>();
        private async UniTaskVoid Start()
        {
            await UniTask.WaitUntil(() => ApplicationEntryPoint.Initialized, cancellationToken: this.GetCancellationTokenOnDestroy());

            foreach (var present in InventoryManager.Presents)
            {
                var obj = Instantiate(_presentPrefab, _presentBoxAria);
                obj.GetComponent<PresentComponent>().Initialize(present.Value);
                Presents.Add(present.Key, obj);
            }

            _backButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("HomeScene"));
            _openALlButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => OpenAllPresentAsync().Forget());
        }

        /// <summary>
        /// プレゼントを1件受け取る。
        /// </summary>
        /// <param name="present"></param>
        /// <returns></returns>
        public async UniTaskVoid OpenPresentAsync(Present present)
        {
            Destroy(Presents[present.InstanceId]);
            Presents.Remove(present.InstanceId);

            await UniTask.WhenAll(
                MessageDialog.ShowAsync($"{present.DisplayName} を受け取りました。"),
                InventoryManager.OpenPresentAsync(present.InstanceId));
        }

        /// <summary>
        /// プレゼントをすべて受け取る。
        /// </summary>
        /// <returns></returns>
        public async UniTaskVoid OpenAllPresentAsync()
        {
            foreach (var present in Presents)
            {
                Destroy(present.Value);
            }

            Presents = new Dictionary<string, GameObject>();

            await UniTask.WhenAll(
                MessageDialog.ShowAsync($"{InventoryManager.Presents.Count} 件のプレゼントを受け取りました。"),
                InventoryManager.OpenAllPresentAsync());
        }
    }
}