using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Backend
{
    public static class BackendConnection
    {
        private static readonly string _apiBaseUrl = "";

        /// <summary>
        /// Registers a new player or safely logs them in via the Upsert database pattern.
        /// POST /Player
        /// </summary>
        public static async Task<PlayerDTO> RegisterOrLoginPlayerAsync(string playFabId, string displayName)
        {
            string url = $"{_apiBaseUrl}/Player";

            PlayerDTO dto = new PlayerDTO { PlayFabId = playFabId, DisplayName = displayName };
            string jsonText = JsonConvert.SerializeObject(dto);

            var request = UnityWebRequest.Post(url, jsonText, "application/json");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                return JsonConvert.DeserializeObject<PlayerDTO>(request.downloadHandler.text);
            }

            Debug.LogError($"[Backend] Register/Login failed: {request.error} | Response: {request.downloadHandler.text}");
            return null;
        }

        /// <summary>
        /// Commands the server to initialize a clean competitive structural layout match.
        /// POST /Match
        /// </summary>
        public static async Task<MatchCreatedDTO> CreateMatchAsync(string hostPlayFabId, string opponentPlayFabId)
        {
            string url = $"{_apiBaseUrl}/Match";

            CreateMatchDTO dto = new CreateMatchDTO { HostPlayFabId = hostPlayFabId, OpponentPlayFabId = opponentPlayFabId };
            string jsonText = JsonConvert.SerializeObject(dto);

            var request = UnityWebRequest.Post(url, jsonText, "application/json");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                return JsonConvert.DeserializeObject<MatchCreatedDTO>(request.downloadHandler.text);
            }

            Debug.LogError($"[Backend] CreateMatch failed: {request.error} | Response: {request.downloadHandler.text}");
            return null;

        }

        /// <summary>
        /// Logs a raw gameplay hit event transaction and recalculates player points dynamically.
        /// POST /Match/{matchId}/PlayerHit
        /// </summary>
        public static async Task<bool> RecordPlayerHitAsync(int matchId, string attackerPlayFabId, string victimPlayFabId)
        {
            string url = $"{_apiBaseUrl}/Match/{matchId}/PlayerHit";

            RecordHitDTO dto = new RecordHitDTO { AttackerPlayfabId = attackerPlayFabId, VictimPlayfabId = victimPlayFabId };
            string jsonText = JsonConvert.SerializeObject(dto);

            UnityWebRequest request = UnityWebRequest.Post(url, jsonText, "application/json");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[Backend] Player hit event posted and recorded successfully.");
                return true;
            }

            Debug.LogError($"[Backend] RecordPlayerHit failed: {request.error} | Response: {request.downloadHandler.text}");
            return false;

        }

        /// <summary>
        /// Fetches the match statistics and performance history array for a specific user profile.
        /// GET /Players/{playFabId}/Matches
        /// </summary>
        public static async Task<List<MatchSummaryDTO>> GetPlayerMatchesAsync(string playFabId)
        {
            string url = $"{_apiBaseUrl}/Players/{playFabId}/Matches";

            UnityWebRequest request = UnityWebRequest.Get(url);

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string rawJson = request.downloadHandler.text;
                return JsonConvert.DeserializeObject<List<MatchSummaryDTO>>(rawJson);
            }

            Debug.LogError($"[Backend] GetPlayerMatches failed: {request.error} | Response: {request.downloadHandler.text}");
            return null;

        }
        /// <summary>
        /// Fetches the compiled 3NF summarized statistics data payload for a completed match.
        /// GET /Matches/{matchId}
        /// </summary>
        public static async Task<MatchSummaryDTO> GetMatchSummaryAsync(int matchId)
        {
            string url = $"{_apiBaseUrl}/Match/{matchId}";

            UnityWebRequest request = UnityWebRequest.Get(url);

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string rawJson = request.downloadHandler.text;
                return JsonConvert.DeserializeObject<MatchSummaryDTO>(rawJson);
            }

            Debug.LogError($"[Backend] GetMatchSummary failed: {request.error} | Response: {request.downloadHandler.text}");
            return null;
        }
    }
}
