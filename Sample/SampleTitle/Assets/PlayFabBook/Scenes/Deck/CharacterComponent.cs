using PlayFabBook;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CharacterComponent : MonoBehaviour
{
    [SerializeField] GameObject _characterPrefab;

    [SerializeField] Image _image;
    [SerializeField] TextMeshProUGUI _rarityText;
    [SerializeField] TextMeshProUGUI _levelText;
    [SerializeField] Button _button;

    [SerializeField] DeckSceneController _controller;

    private string InstanceId { get; set; }

    public void Initialize(string instanceId, bool isEnable)
    {
        InstanceId = instanceId;

        var character = UserDataManager.User.Characters[instanceId];
        _rarityText.text = character.Master.Rarity.ToString();
        _levelText.text = $"Lv.{character.Level}";
        _image.sprite = _controller.CharacterSprites[character.CharacterId];

        _characterPrefab.SetActive(isEnable);
    }

    private void Start()
    {
        _button.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => _controller.SelectCharacter(InstanceId, null));
    }
}
