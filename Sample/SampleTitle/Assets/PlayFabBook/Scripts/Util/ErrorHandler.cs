using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorHandler : MonoBehaviour
{

    public static void HandleException(string logText, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
        {
            UniTask.Void(() => OpenErrorDialogAsync());
        }
    }

    /// <summary>
    /// 共通のエラーダイアログを表示する。
    /// 今のところエラーメッセージは「不明なエラー」で固定。
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private static async UniTaskVoid OpenErrorDialogAsync(string message = null)
    {
        // 別スレッドから呼ばれることがあるかもしれないのでメインスレッドに戻しておく
        await UniTask.SwitchToMainThread();

        if (!string.IsNullOrEmpty(message))
            Debug.Log($"ErrorDialogOpenMessage : {message}");

        var dialog = await Resources.LoadAsync("Prefabs/Dialogs/ErrorDialog/ErrorDialogPrefab");
        Instantiate(dialog);
    }
}
