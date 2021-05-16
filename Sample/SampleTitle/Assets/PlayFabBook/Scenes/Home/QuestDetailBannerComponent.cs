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
    public class QuestDetailBannerComponent : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _nameText;
        [SerializeField] TextMeshProUGUI _staminaText;

        [SerializeField] Button _button;

        /// <summary>
        /// 初期化する。
        /// </summary>
        /// <param name="questDetail"></param>
        public void Initialize(QuestDetailMaster questDetail)
        {
            this.gameObject.SetActive(false);

            _nameText.text = questDetail.Name;
            _staminaText.text = $"消費スタミナ {questDetail.Stamina}";

            _button.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(async _ =>
            {
                if (VirtualCurrencyManager.Stamina < questDetail.Stamina)
                {
                    if (InventoryManager.StaminaRecovery.Num > 0)
                    {
                        var confirm1 = await ConfirmDialog.ShowAsync("スタミナが不足しています。スタミナ回復薬を使用しますか？");
                        if (confirm1 == DialogResult.Cancel)
                            return;

                        await UniTask.WhenAll(
                            MessageDialog.ShowAsync("スタミナを回復しました。クエストを開始します。"),
                            InventoryManager.ConsumeStaminaRecoveryItemAsync());
                    }
                    else
                    {
                        await MessageDialog.ShowAsync("スタミナが不足しています。");
                        return;
                    }
                }
                else
                {

                    var confirm = await ConfirmDialog.ShowAsync("クエストを開始してよろしいですか？");
                    if (confirm == DialogResult.Cancel)
                        return;
                }

                await VirtualCurrencyManager.SubtractStaminaAsync(questDetail.Stamina);

                if (questDetail.BeforeStories?.Any() ?? false)
                {
                    StorySceneController.CurrentQuestAndStoryType = (questDetail, StoryType.Before);
                    SceneManager.LoadScene("StoryScene");
                    return;
                }

                SceneManager.LoadScene("InGameScene");
            });
        }
    }
}

