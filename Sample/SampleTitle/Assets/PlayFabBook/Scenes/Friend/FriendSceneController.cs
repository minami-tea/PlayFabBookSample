using System;
using Cysharp.Threading.Tasks;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class FriendSceneController : MonoBehaviour
    {
        [SerializeField] Button _backButton;
        [SerializeField] Button _addFriendButton;

        [SerializeField] TextMeshProUGUI _friendCodeText;

        [SerializeField] Transform _friendUserAria;
        [SerializeField] GameObject _friendUserPrefab;

        public Dictionary<string, Sprite> CharacterSprites { get; private set; }

        private async UniTaskVoid Start()
        {
            await UniTask.WaitUntil(() => ApplicationEntryPoint.Initialized, cancellationToken: this.GetCancellationTokenOnDestroy());

            _friendCodeText.text = $"あなたのフレンドコード : {PlayerProfileManager.PlayFabId}";

            // キャラクター画像をまとめてロードしておく
            CharacterSprites = (await UniTask.WhenAll(FriendManager.Friends
                .Select(x => x.Value.CharacterId)
                .Distinct()
                .Select(async x => (key: x, sprite: await Resources.LoadAsync<Sprite>($"Textures/Characters/{x}")))))
                .ToDictionary(x => x.key, x => x.sprite as Sprite);

            foreach (var friend in FriendManager.Friends)
            {
                var obj = Instantiate(_friendUserPrefab, _friendUserAria);
                obj.GetComponent<FriendUserComponent>().Initialize(friend.Value);
            }

            _backButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("HomeScene"));
            _addFriendButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(async _ =>
            {
                // フレンドを追加したらシーンを開きなおす。
                var result = await AddFriendDialog.ShowAsync();
                if (result == DialogResult.OK)
                    SceneManager.LoadScene("FriendScene");
                
            });
        }
    }

}