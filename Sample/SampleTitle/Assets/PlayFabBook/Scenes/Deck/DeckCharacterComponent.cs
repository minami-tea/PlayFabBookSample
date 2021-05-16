using PlayFabBook;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DeckCharacterComponent : MonoBehaviour
{
    [SerializeField] int _deckPosition;
    [SerializeField] TextMeshProUGUI _rarityText;
    [SerializeField] Image _image;
    [SerializeField] TextMeshProUGUI _levelText;
    [SerializeField] TextMeshProUGUI _hpText;
    [SerializeField] TextMeshProUGUI _atkText;
    [SerializeField] TextMeshProUGUI _costText;
    [SerializeField] Button _button;

    [SerializeField] Image _mainCharacterButtonColor;
    [SerializeField] Button _mainCharacterButton;

    [SerializeField] DeckSceneController _controller;

    public string InstanceId { get; private set; }

    private Character Character { get; set; }

    public void Initialize(string instanceId)
    {
        InstanceId = instanceId;

        Character = UserDataManager.User.Characters[instanceId];
        _rarityText.text = Character.Master.Rarity.ToString();
        _levelText.text = Character.Level.ToString();
        _image.sprite = _controller.CharacterSprites[Character.CharacterId];
        _hpText.text = Character.Hp.ToString();
        _atkText.text = Character.Atk.ToString();
        _costText.text = Character.Master.Cost.ToString();

        var leaderCharacterId = $"character-{string.Format("{0:D8}", PlayerProfileManager.Statistics?.FirstOrDefault(x => x.StatisticName == "CharacterId")?.Value ?? 0)}";
        if (leaderCharacterId == Character.CharacterId)
        {
            _mainCharacterButtonColor.color = ColorConst.Green;
            _mainCharacterButton.enabled = false;
        }
        else
        {
            _mainCharacterButtonColor.color = ColorConst.White;
            _mainCharacterButton.enabled = true;
        }
    }

    private void Start()
    {
        _button.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => _controller.SelectCharacter(InstanceId, _deckPosition));
        _mainCharacterButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(async _ =>
        {
            await PlayerProfileManager.UpdateMainCharacterAsync(Character.CharacterId, Character.Level);
            _controller.InitializeDeckCharacters();
        });
    }
}
