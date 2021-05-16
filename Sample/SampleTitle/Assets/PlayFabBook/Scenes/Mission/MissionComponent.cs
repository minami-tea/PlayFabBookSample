using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class MissionComponent : MonoBehaviour
    {
        [SerializeField] Image _image;
        [SerializeField] TextMeshProUGUI _titleText;
        [SerializeField] TextMeshProUGUI _descriptionText;
        [SerializeField] Button _button;
        [SerializeField] TextMeshProUGUI _buttonText;

        [SerializeField] MissionSceneController _controller;

        public void Initialize(MissionInfo info)
        {
            var missionName = info.Master.Type == MissionType.Daily ? $"[Daily] {info.Master.Text}" : info.Master.Text;
            var currentNum = info.CurrentNum > info.Master.Num ? info.Master.Num : info.CurrentNum;
            _titleText.text = $"{missionName} ({currentNum}/{info.Master.Num})";
            _descriptionText.text = info.RewardName;

            if (info.IsCompleted && !info.ReceivedReward)
            {
                _buttonText.text = "受取";
                _button.enabled = true;
            }
            else if (info.IsCompleted && info.ReceivedReward)
            {
                _buttonText.text = "受取済";
                _button.enabled = false;
            }
            else
            {
                _button.gameObject.SetActive(false);
            }

            _button.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => _controller.ReceiveRewardAsync(info).Forget());
        }
    }
}
