using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayFabBook
{
    public static class ApplicationEntryPoint
    {
        public static bool Initialized { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static async UniTaskVoid InitializeAsync()
        {
            Application.logMessageReceivedThreaded += ErrorHandler.HandleException;

            Debug.Log("初期化開始");
            await LoginManager.LoginAndUpdateLocalCacheAsync();
            Debug.Log("初期化完了");

            if (string.IsNullOrEmpty(PlayerProfileManager.UserDisplayName))
            {
                Debug.Log("ユーザー名が登録されていないので強制的に TitleScene から起動します。");
                SceneManager.LoadScene("TitleScene");
            }

            Initialized = true;
        }
    }
}