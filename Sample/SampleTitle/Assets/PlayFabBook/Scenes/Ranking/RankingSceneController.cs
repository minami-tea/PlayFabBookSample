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
    public class RankingSceneController : MonoBehaviour
    {
        [SerializeField] Button _backButton;

        [SerializeField] Transform _rankingUserAria;
        [SerializeField] GameObject _rankingUserPrefab;

        public Dictionary<string, Sprite> CharacterSprites { get; private set; }

        private async UniTaskVoid Start()
        {
            await UniTask.WaitUntil(() => ApplicationEntryPoint.Initialized, cancellationToken: this.GetCancellationTokenOnDestroy());

            // キャラクター画像をまとめてロードしておく
            CharacterSprites = (await UniTask.WhenAll(RankingManager.UserLevelRanking
                .Select(x => x.CharacterId)
                .Distinct()
                .Select(async x => (key: x, sprite: await Resources.LoadAsync<Sprite>($"Textures/Characters/{x}")))))
                .ToDictionary(x => x.key, x => x.sprite as Sprite);

            foreach (var rankingUser in RankingManager.UserLevelRanking)
            {
                var obj = Instantiate(_rankingUserPrefab, _rankingUserAria);
                obj.GetComponent<RankingUserComponent>().Initialize(rankingUser);
            }

            _backButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("HomeScene"));
        }
    }

}