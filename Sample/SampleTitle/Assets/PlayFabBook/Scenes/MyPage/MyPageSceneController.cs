using System;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class MyPageSceneController : MonoBehaviour
    {
        [SerializeField] Button _backButton;
        [SerializeField] Button _newsButton;
        [SerializeField] Button _rankingButton;
        [SerializeField] Button _linkAccountButton;

        private async UniTaskVoid Start()
        {
            await UniTask.WaitUntil(() => ApplicationEntryPoint.Initialized, cancellationToken: this.GetCancellationTokenOnDestroy());

            _backButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("HomeScene"));
            _newsButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("NewsScene"));
            _rankingButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("RankingScene"));

            _linkAccountButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(async _ =>
            {
                if (PlayerPrefsManager.IsLoginEmailAddress)
                {
                    await MessageDialog.ShowAsync("すでにログイン済みです。");
                    return;
                }

                await AddEmailAndPasswordDialog.ShowAsync();
            });

        }
    }

}