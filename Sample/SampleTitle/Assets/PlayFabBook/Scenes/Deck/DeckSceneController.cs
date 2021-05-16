using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class DeckSceneController : MonoBehaviour
    {
        [SerializeField] DeckCharacterComponent[] _deckCharacters;
        [SerializeField] Button _backButton;
        [SerializeField] Button _submitButton;

        [SerializeField] Transform _characterAria;
        [SerializeField] GameObject _characterPrefab;

        public Dictionary<string, Sprite> CharacterSprites { get; private set; }
        private Dictionary<string, GameObject> CharacterObjects { get; } = new Dictionary<string, GameObject>();

        private (string instanceId, int? deckPosition) CurrentSelectCharacter { get; set; }

        private bool DeckChangeFlag { get; set; }

        async UniTaskVoid Start()
        {
            await UniTask.WaitUntil(() => ApplicationEntryPoint.Initialized, cancellationToken: this.GetCancellationTokenOnDestroy());

            // キャラクター画像をまとめてロードしておく
            CharacterSprites = (await UniTask.WhenAll(UserDataManager.User.Characters
                .Select(x => x.Value.CharacterId)
                .Distinct()
                .Select(async x => (key: x, sprite: await Resources.LoadAsync<Sprite>($"Textures/Characters/{x}")))))
                .ToDictionary(x => x.key, x => x.sprite as Sprite);

            // デッキを作成したフラグがなければチュートリアルの表示と初期デッキの作成処理を実行する。
            if (!UserDataManager.User.TutorialFlag.HasFlag(TutorialFlag.CreateDeck))
            {
                // チュートリアルを表示しつつ並列で初期デッキを作成してローカルと PlayFab に保存する。
                await UniTask.WhenAll(
                    MessageDialog.ShowAsync("デッキを編集するチュートリアルを表示します。"),
                    UserDataManager.User.CreateNewDeckAsync());
            }

            // デッキ表示を初期化する
            InitializeDeckCharacters();

            // キャラクター表示を初期化する
            var deck = UserDataManager.User.Deck;
            foreach (var character in UserDataManager.User.Characters)
            {
                var obj = Instantiate(_characterPrefab, _characterAria);
                var useDeck = deck.InstanceIds.ContainsValue(character.Key);
                obj.GetComponent<CharacterComponent>().Initialize(character.Key, !useDeck);
                CharacterObjects.Add(character.Key, obj);
            }

            _backButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(async _ =>
            {
                if (!DeckChangeFlag)
                {
                    SceneManager.LoadScene("HomeScene");
                    return;
                }

                var confirm = await ConfirmDialog.ShowAsync("デッキが保存されていませんがよろしいですか？");
                if (confirm == DialogResult.OK)
                    SceneManager.LoadScene("HomeScene");
            });

            _submitButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(async _ =>
            {
                if (!DeckChangeFlag)
                {
                    await MessageDialog.ShowAsync("デッキが変更されていません。");
                    return;
                }

                var newDeckCharacters = _deckCharacters
                    .Select((x, i) => (key: i, id: x.InstanceId))
                    .ToDictionary(x => x.key, x => x.id);

                DeckChangeFlag = false;

                await (MessageDialog.ShowAsync("デッキを保存しました。"), UserDataManager.User.UpdateDeckAsync(newDeckCharacters));
            });
        }

        /// <summary>
        /// デッキ表示を初期化する。
        /// </summary>
        public void InitializeDeckCharacters()
        {
            // デッキ表示を初期化する
            var deck = UserDataManager.User.Deck;
            foreach (var character in deck.InstanceIds)
            {
                // Key は DeckPosition（0-2） Value は Character の InstanceId
                _deckCharacters[character.Key].Initialize(character.Value);
            }
        }

        /// <summary>
        /// デッキ編集画面でキャラクターをタップしたときの挙動。
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="deckPosition"></param>
        /// <returns></returns>
        public void SelectCharacter(string instanceId, int? deckPosition)
        {
            // 入れ替え元のキャラクターがいないとき
            if (string.IsNullOrEmpty(CurrentSelectCharacter.instanceId))
            {
                CurrentSelectCharacter = (instanceId, deckPosition);
                return;
            }

            // 入れ替え元と入れ替え先が同じとき
            if (instanceId == CurrentSelectCharacter.instanceId)
            {
                CurrentSelectCharacter = (null, null);
                return;
            }

            // 入れ替え元も入れ替え先もデッキにセットしていないキャラクターのとき
            if (CurrentSelectCharacter.deckPosition is null && deckPosition is null)
            {
                CurrentSelectCharacter = (null, null);
                return;
            }

            // 入れ替え元も入れ替え先もデッキにセットしているキャラクターのとき
            if (deckPosition != null && CurrentSelectCharacter.deckPosition != null)
            {
                _deckCharacters[(int)deckPosition].Initialize(CurrentSelectCharacter.instanceId);
                _deckCharacters[(int)CurrentSelectCharacter.deckPosition].Initialize(instanceId);
                CurrentSelectCharacter = (null, null);
                DeckChangeFlag = true;
                return;
            }

            var (inDeckInstanceId, outDeckInstanceId) = deckPosition is null
                ? (instanceId, CurrentSelectCharacter.instanceId)
                : (CurrentSelectCharacter.instanceId, instanceId);

            // デッキ内とデッキ外のキャラクターを入れ替えるとき
            var newDeckPosition = deckPosition ?? CurrentSelectCharacter.deckPosition;
            _deckCharacters[(int)newDeckPosition].Initialize(inDeckInstanceId);
            CharacterObjects[inDeckInstanceId].SetActive(false);
            CharacterObjects[outDeckInstanceId].SetActive(true);
            CurrentSelectCharacter = (null, null);
            DeckChangeFlag = true;
        }
    }
}

