using Cysharp.Threading.Tasks;
using System;
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
    public class StorySceneController : MonoBehaviour
    {
        public static (QuestDetailMaster Quest, StoryType Type) CurrentQuestAndStoryType { get; set; }

        [SerializeField] TextMeshProUGUI _nameText;
        [SerializeField] TextMeshProUGUI _storyText;

        [SerializeField] Image _leftCharacterImage;
        [SerializeField] Image _centerLeftCharacterImage;
        [SerializeField] Image _centerCharacterImage;
        [SerializeField] Image _centerRightCharacterImage;
        [SerializeField] Image _rightCharacterImage;
        [SerializeField] Button _nextButton;

        private Dictionary<StoryCharacterPosition, Image> CharacterImages { get; set; }
        private Story[] Stories { get; set; }
        private int CurrentStoryNum { get; set; }

        private async UniTaskVoid Start()
        {
            await UniTask.WaitUntil(() => ApplicationEntryPoint.Initialized, cancellationToken: this.GetCancellationTokenOnDestroy());

            Stories = CurrentQuestAndStoryType.Type switch
            {
                StoryType.Before => CurrentQuestAndStoryType.Quest.BeforeStories,
                StoryType.After => CurrentQuestAndStoryType.Quest.AfterStories,
                _ => throw new NotImplementedException(),
            };

            CharacterImages = new Dictionary<StoryCharacterPosition, Image>
            {
                { StoryCharacterPosition.Left, _leftCharacterImage },
                { StoryCharacterPosition.CenterLeft, _leftCharacterImage },
                { StoryCharacterPosition.Center, _centerCharacterImage },
                { StoryCharacterPosition.CenterRight, _rightCharacterImage },
                { StoryCharacterPosition.Right, _leftCharacterImage },
            };

            await UpdateDisplayAsync(0);
            _nextButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => UpdateDisplayAsync(CurrentStoryNum + 1).Forget());
        }

        /// <summary>
        /// ストーリーの画面表示を更新する。
        /// </summary>
        /// <param name="storyNum"></param>
        /// <returns></returns>
        private async UniTask UpdateDisplayAsync(int storyNum)
        {
            // storyNum が最後の番号ならこのストーリーの再生は終わっているので終了処理へ飛ぶ
            if (storyNum == Stories.Length)
            {
                StoryEndAndMoveSceneAsync();
                return;
            }

            CurrentStoryNum = storyNum;
            var story = Stories[CurrentStoryNum];

            // 名前とテキストの更新
            _nameText.text = story.Name;
            _storyText.text = story.Text;

            // キャラクター画像の更新
            if (story.Characters != null)
            {
                await UniTask.WhenAll(CharacterImages.Select(async x =>
                {
                    var updateCharacter = story.Characters.FirstOrDefault(y => y.Position == x.Key);
                    if (updateCharacter is null)
                    {
                        CharacterImages[x.Key].enabled = false;
                        return;
                    }

                    var image = await Resources.LoadAsync<Sprite>($"Textures/StoryCharacters/{updateCharacter.Image}");
                    CharacterImages[x.Key].sprite = image as Sprite;
                    CharacterImages[x.Key].enabled = true;
                }));
            }
        }

        /// <summary>
        /// ストーリーの表示を終わらせて別の Scene へ遷移する。
        /// </summary>
        private void StoryEndAndMoveSceneAsync()
        {
            switch (CurrentQuestAndStoryType.Type)
            {
                case StoryType.Before:
                    InGameSceneController.CurrentQuest = CurrentQuestAndStoryType.Quest;
                    SceneManager.LoadScene("InGameScene");
                    break;
                case StoryType.After:
                    SceneManager.LoadScene("HomeScene");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
