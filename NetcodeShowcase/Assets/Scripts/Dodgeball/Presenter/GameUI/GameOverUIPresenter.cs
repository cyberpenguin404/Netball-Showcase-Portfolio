using Assets.Scripts.Backend;
using Dodgeball.Model;
using Dodgeball.Presenter;
using MVP.Presenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Dodgeball.Presenter
{
    public class GameOverUIPresenter : PresenterMonobehaviour<MatchModel>
    {
        [SerializeField]
        private GamePresenter _gamePresenter;

        [SerializeField] private TextMeshProUGUI _hostStatsText;
        [SerializeField] private TextMeshProUGUI _opponentStatsText;
        private async void OnEnable()
        {
            //find model
            Model = _gamePresenter.Model.CurrentMatch;

            Debug.Log("displaying match summary for match id: " + Model.MatchId);

            await DisplayMatchSummaryAsync(Model.MatchId);
        }
        public async Task DisplayMatchSummaryAsync(int completedMatchId)
        {
            if (completedMatchId <= 0) return;

            var summaryDto = await BackendConnection.GetMatchSummaryAsync(completedMatchId);

            if (summaryDto != null && summaryDto.Players != null)
            {
                foreach (var player in summaryDto.Players)
                {
                    string statsFormat = $"{player.DisplayName}\nPoints: {player.PointsScored}\nTimes Hit: {player.TimesHit}";

                    if (player.IsHost)
                        _hostStatsText.text = $"[Host]\n{statsFormat}";
                    else
                        _opponentStatsText.text = $"[Opponent]\n{statsFormat}";
                }
            }
        }
    }
}
