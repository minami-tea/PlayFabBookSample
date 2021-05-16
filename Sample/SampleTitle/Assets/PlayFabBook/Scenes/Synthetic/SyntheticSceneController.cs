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
    public class SyntheticSceneController : MonoBehaviour
    {
        [SerializeField] SyntheticTargetCharacterComponent[] _targetCharacters;
        [SerializeField] SyntheticAfterCharacterComponent _afterCharacter;
        [SerializeField] Button _backButton;
        [SerializeField] Button _submitButton;

        [SerializeField] Transform _characterAria;
        [SerializeField] GameObject _characterPrefab;

        public Dictionary<string, Sprite> CharacterSprites { get; private set; }
        private Dictionary<string, GameObject> CharacterObjects { get; set; } = new Dictionary<string, GameObject>();

        private (string instanceId, int? position) CurrentSelectCharacter { get; set; }

        async UniTaskVoid Start()
        {
            await UniTask.WaitUntil(() => ApplicationEntryPoint.Initialized, cancellationToken: this.GetCancellationTokenOnDestroy());

            // キャラクター画像をまとめてロードしておく
            CharacterSprites = (await UniTask.WhenAll(UserDataManager.User.Characters
                .Select(x => x.Value.CharacterId)
                .Distinct()
                .Select(async x => (key: x, sprite: await Resources.LoadAsync<Sprite>($"Textures/Characters/{x}")))))
                .ToDictionary(x => x.key, x => x.sprite as Sprite);

            // デッキのチュートリアルが完了していなければデッキのチュートリアルを先に進行させる。
            if (!UserDataManager.User.TutorialFlag.HasFlag(TutorialFlag.CreateDeck))
            {
                // チュートリアルを表示しつつ並列で初期デッキを作成してローカルと PlayFab に保存する。
                await MessageDialog.ShowAsync("強化・合成を行う前にデッキの作成が必要です。デッキ編集画面へ遷移します。");
                SceneManager.LoadScene("DeckScene");
                return;
            }

            // 初めてシーンを開いたらチュートリアルを表示する。
            if (!UserDataManager.User.TutorialFlag.HasFlag(TutorialFlag.Synthetic))
            {
                // チュートリアルを表示しつつ、チュートリアルを表示したフラグを PlayFab に保存する。
                await UniTask.WhenAll(
                    MessageDialog.ShowAsync("キャラクターを強化・合成するチュートリアルを表示します。"),
                    UserDataManager.User.UpdateTutorialFlagAsync(TutorialFlag.Synthetic));
            }

            // キャラクター表示を初期化する
            foreach (var character in UserDataManager.User.Characters)
            {
                var obj = Instantiate(_characterPrefab, _characterAria);
                obj.GetComponent<SyntheticCharacterComponent>().Initialize(character.Key, true);
                CharacterObjects.Add(character.Key, obj);
            }

            _backButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("HomeScene"));
            _submitButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SynthesizeAsync().Forget());
        }

        /// <summary>
        /// 強化・合成する。
        /// </summary>
        /// <returns></returns>
        public async UniTask SynthesizeAsync()
        {
            var target = _targetCharacters[0];
            var material = _targetCharacters[1];

            if (UserDataManager.Deck.InstanceIds.Any(x => x.Value == material.InstanceId))
            {
                await MessageDialog.ShowAsync("デッキに含まれているキャラクターを素材にすることはできません。");
                return;
            }

            if (string.IsNullOrEmpty(target.InstanceId) || string.IsNullOrEmpty(material.InstanceId))
            {
                await MessageDialog.ShowAsync("強化・合成対象のキャラクターが選択されていません。");
                return;
            }

            var msg = $"{UserDataManager.User.Characters[target.InstanceId].Master.Name} に {UserDataManager.User.Characters[material.InstanceId].Master.Name} を合成してよろしいですか？";
            var confirmResult = await ConfirmDialog.ShowAsync(msg);
            if (confirmResult == DialogResult.Cancel)
                return;

            await UniTask.WhenAll(
                MessageDialog.ShowAsync("キャラクターを合成して強化しました。"),
                UserDataManager.User.SyntheticCharacterAsync(_targetCharacters[0].InstanceId, _targetCharacters[1].InstanceId));

            // 合成したらシーンを開きなおす。
            SceneManager.LoadScene("SyntheticScene");
        }

        /// <summary>
        /// 強化・合成画面でキャラクターをタップしたときの挙動。
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public void SelectCharacter(string instanceId, int? position)
        {
            // 入れ替え元のキャラクターがいないとき
            if (string.IsNullOrEmpty(CurrentSelectCharacter.instanceId))
            {
                CurrentSelectCharacter = (instanceId, position);
                return;
            }

            // 入れ替え元と入れ替え先が同じとき
            if (instanceId == CurrentSelectCharacter.instanceId)
            {
                CurrentSelectCharacter = (null, null);
                return;
            }

            // 入れ替え元も入れ替え先も強化・合成対象に選択されていないキャラクターのとき
            if (CurrentSelectCharacter.position is null && position is null)
            {
                CurrentSelectCharacter = (null, null);
                return;
            }

            // 強化対象キャラクターと強化素材キャラクターを入れ替えるとき
            if (CurrentSelectCharacter.position != null && position != null)
            {
                _targetCharacters[(int)position].Initialize(CurrentSelectCharacter.instanceId);
                _targetCharacters[(int)CurrentSelectCharacter.position].Initialize(instanceId);
                _afterCharacter.Initialize(_targetCharacters[0].InstanceId, _targetCharacters[1].InstanceId);
                CurrentSelectCharacter = (null, null);
                return;
            }

            // 強化対象 or 強化素材キャラクターとその他のキャラクターを入れ替えるとき
            var (inDeckInstanceId, outDeckInstanceId) = position is null
                ? (instanceId, CurrentSelectCharacter.instanceId)
                : (CurrentSelectCharacter.instanceId, instanceId);
            var newDeckPosition = position ?? CurrentSelectCharacter.position;
            _targetCharacters[(int)newDeckPosition].Initialize(inDeckInstanceId);
            CharacterObjects[inDeckInstanceId].SetActive(false);

            // 強化対象 or 強化素材に選択されていたキャラクターの選択を解除したとき
            if (!string.IsNullOrEmpty(outDeckInstanceId))
            {
                CharacterObjects[outDeckInstanceId].SetActive(true);
            }

            // 強化対象と素材がセットされているときは結果表示を更新する
            if (!string.IsNullOrEmpty(_targetCharacters[0]?.InstanceId) && !string.IsNullOrEmpty(_targetCharacters[1]?.InstanceId))
            {
                _afterCharacter.Initialize(_targetCharacters[0].InstanceId, _targetCharacters[1].InstanceId);
            }

            CurrentSelectCharacter = (null, null);
        }
    }
}

