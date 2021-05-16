using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerPrefs を管理するラッパー。
/// </summary>
public static class PlayerPrefsManager
{
    /// <summary>
    /// PlayFab の UserId と CustomId
    /// </summary>
    public static string UserId
    {
        get => PlayerPrefs.GetString("UserId");
        set
        {
            PlayerPrefs.SetString("UserId", value);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// メールアドレスを使ってログイン済みなら true
    /// </summary>
    public static bool IsLoginEmailAddress
    {
        get => bool.TryParse(PlayerPrefs.GetString("IsLoginEmailAddress"), out var result) && result;
        set
        {
            PlayerPrefs.SetString("IsLoginEmailAddress", value.ToString());
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// ログインボーナスを獲得したときに true をセットする。
    /// ログインボーナス演出を表示したら false に戻す。
    /// </summary>
    public static bool HasLoginBonus
    {
        get => bool.TryParse(PlayerPrefs.GetString("HasLoginBonus"), out var result) && result;
        set
        {
            PlayerPrefs.SetString("HasLoginBonus", value.ToString());
            PlayerPrefs.Save();
        }
    }
}
