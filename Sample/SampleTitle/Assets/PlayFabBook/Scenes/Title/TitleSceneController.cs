using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class TitleSceneController : MonoBehaviour
    {
        [SerializeField] Button _screenButton;
        [SerializeField] Button _loginButton;
        [SerializeField] Button _cacheClearButton;

        async UniTaskVoid Start()
        {
            await UniTask.WaitUntil(() => ApplicationEntryPoint.Initialized, cancellationToken: this.GetCancellationTokenOnDestroy());

            _screenButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => MoveHomeSceneAsync().Forget());
            _loginButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => LoginAsync().Forget());
            _cacheClearButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => CacheClearAsync().Forget());
        }

        /// <summary>
        /// ホームへ遷移する。
        /// </summary>
        public async UniTaskVoid MoveHomeSceneAsync()
        {
            // チュートリアルの進行状況によってはユーザーの情報を登録するダイアログを表示する。
            if (!UserDataManager.User.TutorialFlag.HasFlag(TutorialFlag.SetNameAndGender))
            {
                var result = await SetUserInformationDialog.ShowAsync();
                if (result == DialogResult.Cancel)
                    return;
            }

            // チュートリアルの進行状況によってはオープニングのイベントを表示する。
            // という予定だったが今回のサンプルでは特に見せたいものがないので、何も表示せずに初期アイテムとキャラクターの付与を行っておく。
            if (!UserDataManager.User.TutorialFlag.HasFlag(TutorialFlag.ShowOpeningEvent))
            {
                UserDataManager.User.TutorialFlag |= TutorialFlag.ShowOpeningEvent;
                await StoreManager.PurchaseItemAsync(StoreId.DummyStore, "bundle-initial-present-0001", VirtualCurrencyNames.MS.Code);
                await UserDataManager.UpdatePlayFab();
            }

            // シーン遷移
            SceneManager.LoadScene("HomeScene");
        }

        /// <summary>
        /// ログインする。
        /// </summary>
        /// <returns></returns>
        public async UniTaskVoid LoginAsync()
        {
            if (PlayerPrefsManager.IsLoginEmailAddress)
            {
                await MessageDialog.ShowAsync("すでにログイン済みです。");
                return;
            }

            var result = await EmailLoginDialog.ShowAsync();
            if (result == DialogResult.OK)
                SceneManager.LoadScene("TitleScene");
        }

        /// <summary>
        /// PlayerPrefs をクリアして TitleScene を再読み込みする。
        /// </summary>
        public async UniTaskVoid CacheClearAsync()
        {
            PlayerPrefs.DeleteAll();
            await UniTask.WhenAll(
                LoginManager.LoginAndUpdateLocalCacheAsync(),
                MessageDialog.ShowAsync("キャッシュをクリアしました。\n再起動します。"));
            
            SceneManager.LoadScene("TitleScene");
        }
    }
}