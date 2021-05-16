using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class PresentComponent : MonoBehaviour
    {
        [SerializeField] Image _image;
        [SerializeField] TextMeshProUGUI _titleText;
        [SerializeField] TextMeshProUGUI _descriptionText;
        [SerializeField] Button _button;

        [SerializeField] PresentBoxSceneController _controller;

        public void Initialize(Present present)
        {
            _titleText.text = present.DisplayName;
            _descriptionText.text = present.Description;
            _button.onClick.AddListener(() =>
            {
                _controller.OpenPresentAsync(present).Forget();
            });
        }
    }
}
