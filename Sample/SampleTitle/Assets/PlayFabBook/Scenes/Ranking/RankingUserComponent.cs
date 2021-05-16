using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class RankingUserComponent : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _positionText;
        [SerializeField] TextMeshProUGUI _userNameText;
        [SerializeField] TextMeshProUGUI _userLevelText;
        [SerializeField] Image _userImage;
        [SerializeField] TextMeshProUGUI _characterName;
        [SerializeField] TextMeshProUGUI _characterLevel;
        [SerializeField] Image _characterImage;

        [SerializeField] RankingSceneController _controller;

        /// <summary>
        /// 初期化する。
        /// </summary>
        /// <param name="user"></param>
        public void Initialize(RankingUser user)
        {
            _positionText.text = user.Position.ToString();

            _userNameText.text = user.Name;
            _userLevelText.text = $"Lv.{user.Level}";

            var character = TitleDataManager.CharacterMaster[user.CharacterId];
            _characterName.text = character.Name;
            _characterLevel.text = $"Lv.{user.CharacterLevel}";
            _characterImage.sprite = _controller.CharacterSprites[user.CharacterId];
        }
    }
}
