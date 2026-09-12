
using Assets.Scripts.Backend;
using PD4.Singleton;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity.Netcode;

public class PlayfabPlayer : Singleton<PlayfabPlayer>
{
    public static string PlayFabID { get; private set; }

    public string DisplayName { get; private set; }

    public void StorePlayFabID(string id)
    {
        PlayFabID = id;
        FetchDisplayName(async () =>
        {
            await BackendConnection.RegisterOrLoginPlayerAsync(id, DisplayName);
        });
        UnityEngine.Debug.Log($"Stored PlayFabID: {PlayFabID}");
    }
    public void FetchDisplayName(Action finishedCallback = null)
    {
        var request = new GetPlayerProfileRequest()
        {
            PlayFabId = PlayFabID,
        };
        PlayFabClientAPI.GetPlayerProfile(request,
        (result) =>
        {
            if (result.PlayerProfile != null)
            {
                DisplayName = result.PlayerProfile.DisplayName;
            }
            finishedCallback?.Invoke();
        },
        OnError);
    }

    private void OnError(PlayFabError error)
    {
        throw new Exception(error.GenerateErrorReport());
    }
    public void SetHighScore(int score)
    {
        UpdatePlayerStatisticsRequest request = new()
        {
            Statistics = new List<StatisticUpdate>
                {
                     new StatisticUpdate
                    {
                        StatisticName = "Highscore",
                        Value = score
                    }
                }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => { }, OnError);
    }
}
