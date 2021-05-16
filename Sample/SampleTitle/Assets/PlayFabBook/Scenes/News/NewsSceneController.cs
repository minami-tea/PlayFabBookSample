using System;
using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class NewsSceneController : MonoBehaviour
    {
        [SerializeField] Button _backButton;
        [SerializeField] Transform _newsAria;
        [SerializeField] GameObject _newsPrefab;

        private async UniTaskVoid Start()
        {
            await UniTask.WaitUntil(() => ApplicationEntryPoint.Initialized, cancellationToken: this.GetCancellationTokenOnDestroy());

            foreach (var news in NewsManager.NewsItems)
            {
                var obj = Instantiate(_newsPrefab, _newsAria);
                obj.GetComponent<NewsComponent>().Initialize(news);
            }

            _backButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("HomeScene"));
        }
    }

}