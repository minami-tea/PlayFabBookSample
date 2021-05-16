using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayFabBook
{
    public class Mission
    {
        public Dictionary<int, MissionInfo> MissionInformations => MissionActionCount is null ? null :
            TitleDataManager.MissionMaster.Select(x =>
            {
                var info = new MissionInfo
                {
                    MissionId = x.Key,
                    Master = TitleDataManager.MissionMaster[x.Key]
                };

                info.RewardName = CatalogManager.CatalogItems[info.Master.RewardItemId].DisplayName;
                info.CurrentNum = info.Master.Type == MissionType.Normal
                    ? MissionActionCount.TryGetValue(info.Master.Action, out var v1) ? v1 : 0
                    : DailyMissionActionCount.TryGetValue(info.Master.Action, out var v2) ? v2 : 0;
                (info.IsCompleted, info.ReceivedReward) = info.Master.Type == MissionType.Normal
                    ? CompleteMissionReceiveRewards.TryGetValue(x.Key, out var v3) ? (true, v3) : (false, false)
                    : CompleteDailyMissionReceiveRewards.TryGetValue(x.Key, out var v4) ? (true, v4) : (false, false);

                return info;
            })
            .Where(x => x.Master.PrerequisiteMissionId == 0
                || CompleteMissionReceiveRewards.ContainsKey(x.MissionId)
                || CompleteDailyMissionReceiveRewards.ContainsKey(x.MissionId))
            .ToDictionary(y => y.MissionId);

        public Dictionary<MissionAction, int> MissionActionCount { get; set; }
        public Dictionary<MissionAction, int> DailyMissionActionCount { get; set; }
        public Dictionary<int, bool> CompleteMissionReceiveRewards { get; set; }
        public Dictionary<int, bool> CompleteDailyMissionReceiveRewards { get; set; }

        /// <summary>
        /// 初期データを作成する。
        /// </summary>
        /// <returns></returns>
        public static Mission Create()
        {
            // 初期データの作成時点でログイン回数とレベルの MisionAction を登録しておく
            var mission = new Mission
            {
                MissionActionCount = new Dictionary<MissionAction, int> { { MissionAction.Login, 1 }, { MissionAction.Level, 1 } },
                DailyMissionActionCount = new Dictionary<MissionAction, int> { { MissionAction.Login, 1 } },
                CompleteMissionReceiveRewards = new Dictionary<int, bool>(),
                CompleteDailyMissionReceiveRewards = new Dictionary<int, bool>()
            };

            return mission;
        }

        /// <summary>
        /// ミッションの進行状況を更新する。
        /// </summary>
        /// <param name="action"></param>
        /// <param name="num"></param>
        /// <param name="updatePlayFab"></param>
        public async UniTask AddMissionActionCount(MissionAction action, int num, bool updatePlayFab = true)
        {
            // 通常ミッション
            if (MissionActionCount.TryGetValue(action, out var v1))
            {
                MissionActionCount[action] = v1 + num;
            }
            else
            {
                MissionActionCount.Add(action, num);
            }
            var normalActionNum = MissionActionCount[action];

            // デイリーミッション
            if (DailyMissionActionCount.TryGetValue(action, out var v2))
            {
                DailyMissionActionCount[action] = v2 + num;
            }
            else
            {
                DailyMissionActionCount.Add(action, num);
            }
            var dailyActionNum = MissionActionCount[action];

            foreach (var mission in TitleDataManager.MissionMaster.Where(x => x.Value.Action == action))
            {
                switch (mission.Value.Type)
                {
                    case MissionType.Normal:
                        if (mission.Value.Num > normalActionNum)
                            continue;

                        if (!CompleteMissionReceiveRewards.ContainsKey(mission.Key))
                            CompleteMissionReceiveRewards.Add(mission.Key, false);
                        break;

                    case MissionType.Daily:
                        if (mission.Value.Num > dailyActionNum)
                            continue;

                        if (!CompleteDailyMissionReceiveRewards.ContainsKey(mission.Key))
                            CompleteDailyMissionReceiveRewards.Add(mission.Key, false);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            // PlayFab のデータを更新する。
            if (updatePlayFab)
                await UserDataManager.UpdatePlayFab();
        }

        /// <summary>
        /// 達成済みのミッションの報酬を受け取る。
        /// </summary>
        /// <param name="missionId"></param>
        /// <param name="syncLocalToPlayFab"></param>
        /// <returns></returns>
        public async UniTask ReceiveRewardAsync(int missionId, bool syncLocalToPlayFab)
        {
            var mission = TitleDataManager.MissionMaster[missionId];

            switch (mission.Type)
            {
                case MissionType.Normal:
                    if (CompleteMissionReceiveRewards.ContainsKey(missionId))
                        CompleteMissionReceiveRewards[missionId] = true;
                    break;

                case MissionType.Daily:
                    if (CompleteDailyMissionReceiveRewards.ContainsKey(missionId))
                        CompleteDailyMissionReceiveRewards[missionId] = true;
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (syncLocalToPlayFab)
            {
                await UniTask.WhenAll(
                    StoreManager.PurchaseItemAsync(StoreId.DummyStore, mission.RewardItemId, VirtualCurrencyNames.MS.Code, false),
                    UserDataManager.UpdatePlayFab());

                await LoginManager.LoginAndUpdateLocalCacheAsync();
            }
            else
            {
                await StoreManager.PurchaseItemAsync(StoreId.DummyStore, mission.RewardItemId, VirtualCurrencyNames.MS.Code, false);
            }
        }

        /// <summary>
        /// 複数のミッションの報酬を受け取る。
        /// </summary>
        /// <param name="missionId"></param>
        /// <returns></returns>
        public static async UniTask ReceiveRewardAsync(IEnumerable<int> missionIds)
        {
            foreach (var middionId in missionIds)
            {
                await UserDataManager.User.Mission.ReceiveRewardAsync(middionId, false);
            }

            await UserDataManager.UpdatePlayFab();
            await LoginManager.LoginAndUpdateLocalCacheAsync();
        }

        /// <summary>
        /// デイリーミッションの進行状況をリセットする。
        /// </summary>
        /// <param name="updatePlayFab"></param>
        /// <returns></returns>
        public async UniTask ResetDailyMissionAsync(bool updatePlayFab = true)
        {
            DailyMissionActionCount = new Dictionary<MissionAction, int>();
            CompleteDailyMissionReceiveRewards = new Dictionary<int, bool>();

            // PlayFab のデータを更新する。
            if (updatePlayFab)
                await UserDataManager.UpdatePlayFab();
        }
    }
}
