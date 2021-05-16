using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayFabBook
{
    public class MissionSceneController : MonoBehaviour
    {
        [SerializeField] Button _backButton;
        [SerializeField] Button _receiveAllButton;

        [SerializeField] Transform _missionAria;
        [SerializeField] GameObject _missionPrefab;

        private Dictionary<int, GameObject> Missions { get; set; } = new Dictionary<int, GameObject>();
        private async UniTask Start()
        {
            await UniTask.WaitUntil(() => ApplicationEntryPoint.Initialized, cancellationToken: this.GetCancellationTokenOnDestroy());

            var orderdMissions = UserDataManager.Mission.MissionInformations
                .OrderBy(x => x.Value.IsCompleted && !x.Value.ReceivedReward)
                .ThenBy(y => !y.Value.IsCompleted)
                .ThenBy(z => z.Value.Master.Type).Reverse();

            foreach (var missionInfo in orderdMissions)
            {
                var obj = Instantiate(_missionPrefab, _missionAria);
                obj.GetComponent<MissionComponent>().Initialize(missionInfo.Value);
                Missions.Add(missionInfo.Key, obj);
            }

            _backButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => SceneManager.LoadScene("HomeScene"));
            _receiveAllButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromMilliseconds(500)).Subscribe(_ => ReceiveAllRewardAsync().Forget());
        }

        /// <summary>
        /// ミッションを1件受け取る。
        /// </summary>
        /// <param name="missionInfo"></param>
        /// <returns></returns>
        public async UniTaskVoid ReceiveRewardAsync(MissionInfo missionInfo)
        {
            Destroy(Missions[missionInfo.MissionId]);
            Missions.Remove(missionInfo.MissionId);

            await UniTask.WhenAll(
                MessageDialog.ShowAsync($"{missionInfo.RewardName} を受け取りました。"),
                UserDataManager.Mission.ReceiveRewardAsync(missionInfo.MissionId, true));
        }

        /// <summary>
        /// ミッション報酬をすべて受け取る。
        /// </summary>
        /// <returns></returns>
        public async UniTaskVoid ReceiveAllRewardAsync()
        {
            var targetMissionIds = UserDataManager.Mission.MissionInformations
                .Where(x => x.Value.IsCompleted && !x.Value.ReceivedReward)
                .Select(x => x.Key).ToArray();

            if (targetMissionIds.Length == 0)
            {
                await MessageDialog.ShowAsync($"受け取れる報酬がありません。");
                return;
            }

            foreach (var target in targetMissionIds)
            {
                Destroy(Missions[target]);
                Missions.Remove(target);
            }

            await UniTask.WhenAll(
                MessageDialog.ShowAsync($"{targetMissionIds.Length} 件の報酬を受け取りました。"),
                Mission.ReceiveRewardAsync(targetMissionIds));
        }
    }
}