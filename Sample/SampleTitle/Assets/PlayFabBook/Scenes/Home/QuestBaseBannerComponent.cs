using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class QuestBaseBannerComponent : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _nameText;
        [SerializeField] TextMeshProUGUI _levelText;

        [SerializeField] Button _button;

        [SerializeField] Transform _questDetailBannerAria;
        [SerializeField] GameObject _questDetailBannerPrefab;

        private bool displayChildren;

        public void Initialize(IGrouping<(int questBaseId, QuestBaseMaster questBaseMaster), QuestDetailMaster> quest)
        {
            _nameText.text = quest.Key.questBaseMaster.Name;
            _levelText.text = $"推奨 Lv {quest.Key.questBaseMaster.Level}";

            foreach (var questDetail in quest)
            {
                var questDetailBanner = Instantiate(_questDetailBannerPrefab, _questDetailBannerAria).GetComponent<QuestDetailBannerComponent>();
                questDetailBanner.Initialize(questDetail);
            }

            _button.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ =>
            {
                displayChildren = !displayChildren;
                foreach (Transform questDetail in _questDetailBannerAria)
                {
                    questDetail.gameObject.SetActive(displayChildren);
                }
            });
        }
    }
}

