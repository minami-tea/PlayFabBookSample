using PlayFabBook;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SyntheticAfterCharacterComponent : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _rarityText;
    [SerializeField] Image _image;
    [SerializeField] TextMeshProUGUI _levelText;
    [SerializeField] TextMeshProUGUI _hpText;
    [SerializeField] TextMeshProUGUI _atkText;
    [SerializeField] TextMeshProUGUI _costText;

    [SerializeField] SyntheticSceneController _controller;

    public void Initialize(string targetInstanceId, string materialInstanceId)
    {
        var target = UserDataManager.User.Characters[targetInstanceId];
        var material = UserDataManager.User.Characters[materialInstanceId];

        var afterCharacter = Character.Create(targetInstanceId, target.CharacterId);
        afterCharacter.Level = target.Level + material.Level;

        _rarityText.text = afterCharacter.Master.Rarity.ToString();
        _image.sprite = _controller.CharacterSprites[afterCharacter.CharacterId];
        _levelText.text = afterCharacter.Level.ToString();
        _hpText.text = afterCharacter.Hp.ToString();
        _atkText.text = afterCharacter.Atk.ToString();
        _costText.text = afterCharacter.Master.Cost.ToString();
    }
}
