using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class FriendUserComponent : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _userNameText;
        [SerializeField] TextMeshProUGUI _userLevelText;
        [SerializeField] Image _userImage;
        [SerializeField] TextMeshProUGUI _lastLoginDateTimeText;
        [SerializeField] TextMeshProUGUI _characterName;
        [SerializeField] TextMeshProUGUI _characterLevel;
        [SerializeField] Image _characterImage;

        [SerializeField] Button _button;

        [SerializeField] FriendSceneController _controller;

        private FriendUser FriendUser { get; set; }

        /// <summary>
        /// 初期化する。
        /// </summary>
        /// <param name="friendUser"></param>
        public void Initialize(FriendUser friendUser)
        {
            FriendUser = friendUser;

            _userNameText.text = friendUser.Name;
            _userLevelText.text = $"Lv.{friendUser.Level}";
            _lastLoginDateTimeText.text = friendUser.LastLoginDateTime.ToString();

            var character = TitleDataManager.CharacterMaster[friendUser.CharacterId];
            _characterName.text = character.Name;
            _characterLevel.text = $"Lv.{friendUser.CharacterLevel}";
            _characterImage.sprite = _controller.CharacterSprites[friendUser.CharacterId];

            _button.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => RemoveFriendAsync().Forget());
        }

        /// <summary>
        /// フレンドを削除する。
        /// </summary>
        /// <returns></returns>
        private async UniTaskVoid RemoveFriendAsync()
        {
            var confirm = await ConfirmDialog.ShowAsync($"{FriendUser.Name} をフレンドから削除しますか？");
            if (confirm == DialogResult.Cancel)
                return;

            var (isSuccess, message) = await FriendManager.RemoveFriendAsync(FriendUser.PlayFabId);

            var displayMessage = isSuccess
                ? $"{FriendUser.Name} をフレンドから削除しました。"
                : message;

            await UniTask.WhenAll(
                MessageDialog.ShowAsync(displayMessage),
                FriendManager.SyncPlayFabToClientAsync());

            SceneManager.LoadScene("FriendScene");

        }
    }
}
