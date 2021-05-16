using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class NewsComponent : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _timestampText;
        [SerializeField] TextMeshProUGUI _titleText;

        [SerializeField] Button _button;

        [SerializeField] NewsSceneController _controller;

        /// <summary>
        /// 初期化する。
        /// </summary>
        /// <param name="news"></param>
        public void Initialize(TitleNewsItem news)
        {
            _timestampText.text = (news.Timestamp + TimeSpan.FromHours(9)).ToString("g");
            _titleText.text = news.Title;
            _button.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => MessageDialog.ShowAsync(news.Body).Forget());
        }
    }
}
