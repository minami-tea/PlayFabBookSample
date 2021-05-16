using System;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class HomeSceneController : MonoBehaviour
    {
        [SerializeField] Button _presentBoxButton;
        [SerializeField] Button _missionButton;
        [SerializeField] Button _deckButton;
        [SerializeField] Button _syntheticButton;
        [SerializeField] Button _gachaButton;
        [SerializeField] Button _storeButton;
        [SerializeField] Button _friendButton;
        [SerializeField] Button _myPageButton;

        [SerializeField] TextMeshProUGUI _userDisplayName;
        [SerializeField] TextMeshProUGUI _level;
        [SerializeField] TextMeshProUGUI _stamina;

        [SerializeField] Transform _questBannerAria;
        [SerializeField] GameObject _questBaseBannerPrefab;

        private async UniTaskVoid Start()
        {
            await UniTask.WaitUntil(() => ApplicationEntryPoint.Initialized, cancellationToken: this.GetCancellationTokenOnDestroy());

            await InitializeAsync();

            _presentBoxButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("PresentBoxScene"));
            _missionButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("MissionScene"));
            _deckButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("DeckScene"));
            _syntheticButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("SyntheticScene"));
            _gachaButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("GachaScene"));
            _storeButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("StoreScene"));
            _friendButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("FriendScene"));
            _myPageButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("MyPageScene"));
        }

        /// <summary>
        /// 初期化する。
        /// </summary>
        /// <returns></returns>
        private async UniTask InitializeAsync()
        {
            _userDisplayName.text = PlayerProfileManager.UserDisplayName;
            _level.text = UserDataManager.Level.ToString();
            _stamina.text = $"{VirtualCurrencyManager.Stamina}/{UserDataManager.MaxStamina}";

            foreach (var quest in TitleDataManager.QuestMaster)
            {
                var questBaseBanner = Instantiate(_questBaseBannerPrefab, _questBannerAria).GetComponent<QuestBaseBannerComponent>();
                questBaseBanner.Initialize(quest);
            }

            await CheckLoginBonusAsync();
        }

        /// <summary>
        /// ログインボーナスがあれば獲得する。
        /// </summary>
        /// <returns></returns>
        private static async UniTask CheckLoginBonusAsync()
        {
            if (PlayerPrefsManager.HasLoginBonus)
            {
                await UniTask.WhenAll(
                    MessageDialog.ShowAsync("ログインボーナスを獲得しました。ログインボーナスはプレゼントボックスに送られます。"),
                    LoginManager.LoginAndUpdateLocalCacheAsync());

                PlayerPrefsManager.HasLoginBonus = false;
            }
        }
    }
}