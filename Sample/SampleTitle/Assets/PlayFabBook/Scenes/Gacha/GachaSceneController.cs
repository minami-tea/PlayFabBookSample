using System;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class GachaSceneController : MonoBehaviour
    {
        [SerializeField] Button _backButton;

        [SerializeField] Transform _storeItemAria;
        [SerializeField] GameObject _storeItemPrefab;

        async UniTaskVoid Start()
        {
            await UniTask.WaitUntil(() => ApplicationEntryPoint.Initialized, cancellationToken: this.GetCancellationTokenOnDestroy());

            foreach (var item in StoreManager.StoreItems[StoreId.GachaStore])
            {
                var obj = Instantiate(_storeItemPrefab, _storeItemAria);
                obj.GetComponent<GachaStoreItemComponent>().Initialize(item.Value);
            }

            _backButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("HomeScene"));
        }
    }

}