using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayFabBook
{
    public class MissionMaster
    {
        public int MissionId { get; set; }
        public string Text { get; set; }
        public MissionType Type { get; set; }
        public MissionAction Action { get; set; }
        public int Num { get; set; }
        public string RewardItemId { get; set; }
        public int PrerequisiteMissionId { get; set; }
    }
}
