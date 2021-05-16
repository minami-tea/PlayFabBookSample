using PlayFabBook;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PlayFabBook
{

    [Serializable]
    public class SyntheticTargetCharacterComponent : MonoBehaviour
    {
        [SerializeField] int _targetType;
        [SerializeField] TextMeshProUGUI _rarityText;
        [SerializeField] Image _image;
        [SerializeField] TextMeshProUGUI _levelText;
        [SerializeField] TextMeshProUGUI _hpText;
        [SerializeField] TextMeshProUGUI _atkText;
        [SerializeField] TextMeshProUGUI _costText;
        [SerializeField] Button _button;

        [SerializeField] SyntheticSceneController _controller;

        public string InstanceId { get; private set; }

        public void Initialize(string instanceId)
        {
            InstanceId = instanceId;

            var character = UserDataManager.User.Characters[instanceId];
            _rarityText.text = character.Master.Rarity.ToString();
            _levelText.text = character.Level.ToString();
            _image.sprite = _controller.CharacterSprites[character.CharacterId];
            _hpText.text = character.Hp.ToString();
            _atkText.text = character.Atk.ToString();
            _costText.text = character.Master.Cost.ToString();
        }

        private void Start()
        {
            _button.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => _controller.SelectCharacter(InstanceId, _targetType));

        }
    }

}