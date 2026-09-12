using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Backend
{

    #region DTOs
    [Serializable]
    public class PlayerDTO
    {
        public string PlayFabId;
        public string DisplayName;
    }

    [Serializable]
    public class CreateMatchDTO
    {
        public string HostPlayFabId;
        public string OpponentPlayFabId;
    }

    [Serializable]
    public class MatchCreatedDTO
    {
        public int MatchID;
    }

    [Serializable]
    public class RecordHitDTO
    {
        public string AttackerPlayfabId;
        public string VictimPlayfabId;
    }

    [Serializable]
    public class MatchSummaryDTO
    {
        public int MatchId;
        public List<MatchParticipantDTO> Players;
    }

    [Serializable]
    public class MatchParticipantDTO
    {
        public string PlayfabID;
        public string DisplayName;
        public bool IsHost;
        public int PointsScored;
        public int TimesHit;
    }
    #endregion
}
