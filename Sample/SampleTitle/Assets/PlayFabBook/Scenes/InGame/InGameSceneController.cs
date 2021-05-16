using System;
using Cysharp.Threading.Tasks;
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
    public class InGameSceneController : MonoBehaviour
    {
        public static QuestDetailMaster CurrentQuest { get; set; }

        [SerializeField] TextMeshProUGUI _expText;
        [SerializeField] TextMeshProUGUI _itemText;
        [SerializeField] Button _nextButton;

        private async UniTaskVoid Start()
        {
            await UniTask.WaitUntil(() => ApplicationEntryPoint.Initialized, cancellationToken: this.GetCancellationTokenOnDestroy());

            // 今回のサンプルではインゲームは実装しないつもりなのでいきなりクエストクリア画面へ遷移する。

            var quest = CurrentQuest;

            // 報酬のアイテムと経験値を獲得する。
            var (rewardResult, isLevelUp) = await UniTask.WhenAll(
                StoreManager.PurchaseItemAsync(StoreId.DummyStore, quest.Reward.ItemId, VirtualCurrencyNames.MS.Code, false),
                VirtualCurrencyManager.AddExpAsync(quest.Reward.Exp, false));

            // レベルアップしていればレベルアップ処理を行う。
            if (isLevelUp)
                await UserDataManager.User.LevelUpAsync(false);

            // ミッション情報を更新する。
            await UserDataManager.User.Mission.AddMissionActionCount(MissionAction.Quest, 1, true);

            // PlayFab へログインしなおしてローカルのデータを最新の状態に更新する。
            await LoginManager.LoginAndUpdateLocalCacheAsync();

            _expText.text = isLevelUp
                ? $"経験値 {quest.Reward.Exp} を獲得しました。レベルが上がりました。"
                : $"経験値 {quest.Reward.Exp} を獲得しました。";

            var item = rewardResult.Items.FirstOrDefault(x => x.ItemId != quest.Reward.ItemId);
            var itemMaster = CatalogManager.CatalogItems[item.ItemId];
            _itemText.text = $"{itemMaster.DisplayName} を獲得しました。";

            if (quest.AfterStories.Any())
            {
                StorySceneController.CurrentQuestAndStoryType = (quest, StoryType.After);
                _nextButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("StoryScene"));
                return;
            }

            _nextButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("HomeScene"));
        }
    }
}

