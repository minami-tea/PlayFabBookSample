using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PlayFabBook
{
    public abstract class DialogBaseMonoBehaviour : MonoBehaviour
    {
        protected readonly UniTaskCompletionSource<DialogResult> taskCompletion = new UniTaskCompletionSource<DialogResult>();

        public UniTask<DialogResult> ClickResult => taskCompletion.Task;

        private void OnDestroy()
        {
            taskCompletion.TrySetResult(DialogResult.Cancel);
        }
    }
}