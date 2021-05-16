using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ErrorDialog : MonoBehaviour
{
    [SerializeField] Button okButton;

    private void Start()
    {
        okButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("TitleScene"));
    }

}