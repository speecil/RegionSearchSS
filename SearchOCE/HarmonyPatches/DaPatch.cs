using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SearchOCE.HarmonyPatches
{
    [HarmonyPatch]
    internal class DaPatch
    {
        private static readonly BindingFlags[] Flags = { BindingFlags.NonPublic, BindingFlags.Public, BindingFlags.Instance, BindingFlags.Static };

        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            Type leaderboardServiceType = typeof(ScoreSaber.Plugin).Assembly.GetType("ScoreSaber.Core.Services.LeaderboardService");
            var parameters = new[] { typeof(IDifficultyBeatmap), typeof(PlatformLeaderboardsModel.ScoresScope), typeof(int), typeof(PlayerSpecificSettings), typeof(bool) };
            return AccessTools.Method(leaderboardServiceType, "GetLeaderboardData", parameters);
        }

        [HarmonyPrefix]
        public static bool Prefix(object __instance, IDifficultyBeatmap difficultyBeatmap, int page, PlayerSpecificSettings playerSpecificSettings, bool filterAroundCountry, ref object __result)
        {
            if (!filterAroundCountry) return true;
            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
            __result = taskCompletionSource.Task;
            GetLeaderboardData(__instance, difficultyBeatmap, page, playerSpecificSettings, taskCompletionSource);
            return false;
        }

        public static async void GetLeaderboardData(object __instance, IDifficultyBeatmap difficultyBeatmap, int page, PlayerSpecificSettings playerSettings, TaskCompletionSource<object> patchResult)
        {
            string gameMode = $"Solo{difficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName}";
            string difficulty = difficultyBeatmap.difficulty.DefaultRating().ToString();
            string leaderboardId = difficultyBeatmap.level.levelID.Split('_')[2];
            string[] oceUrl = {$"https://scoresaber.com/api/leaderboard/by-hash/{leaderboardId}/scores?difficulty={difficulty}&countries=AU,NZ&page=", $"{page}", $"&gameMode={gameMode}"};
            string normalUrl = $"/game/leaderboard/around-country/{leaderboardId}/mode/{gameMode}/difficulty/{difficulty}?page={page}";
            
            object http = typeof(ScoreSaber.Plugin).GetProperty("HttpInstance", Flags[0] | Flags[3])?.GetValue(null);
            string normalData = await (Task<string>)http.GetType().GetMethod("GetAsync", Flags[0] | Flags[2]).Invoke(http, new object[] { normalUrl });
            
            Type leaderboardType = typeof(ScoreSaber.Plugin).Assembly.GetType("ScoreSaber.Core.Data.Models.Leaderboard");
            object leaderboardData = JsonConvert.DeserializeObject(normalData, leaderboardType);

            object oceScoreData = await GetOceScoreData(oceUrl, page);
            leaderboardData?.GetType().GetProperty("scores", Flags[0] | Flags[2])?.SetValue(leaderboardData, oceScoreData);

            var beatmapData = await difficultyBeatmap.GetBeatmapDataAsync(difficultyBeatmap.GetEnvironmentInfo(), playerSettings);

            Type leaderboardMapType = typeof(ScoreSaber.Plugin).Assembly.GetType("ScoreSaber.Core.Data.Wrappers.LeaderboardMap");
            object leaderboard = leaderboardMapType.GetConstructors(Flags[0] | Flags[2])[0].Invoke(new[] { leaderboardData, difficultyBeatmap, beatmapData });

            __instance.GetType().GetField("currentLoadedLeaderboard", Flags[1] | Flags[2])?.SetValue(__instance, leaderboard);
            patchResult.SetResult(leaderboard);
        }

        public static async Task<object> GetOceScoreData(string[] url, int page)
        {
            HttpClient client = new HttpClient();
            JArray scoreContent = new JArray();
            List<string> responseData = new List<string>();

            int pageNum = page == 1 ? 1 : (int)(page*10 / 12f);
            if (page*10 % 12 != 10 || page == 1) responseData.Add(await client.GetStringAsync(string.Join("", url[0] + pageNum + url[2])));
            if (page*10 % 12 != 0 && page != 1) responseData.Add(await client.GetStringAsync(string.Join("", url[0] + (pageNum + 1) + url[2])));

            foreach (string response in responseData)
            {
                JObject json = JObject.Parse(response);
                if (!json.ContainsKey("scores")) continue;
                JArray scores = json["scores"]?.Value<JArray>();
                for (int i = 0; i < scores?.Count; i++)
                {
                    int rank = scores[i].Value<JObject>()["rank"]?.Value<int>() ?? 0;
                    if (rank <= page * 10 && rank > (page - 1) * 10) continue; 
                    scores.RemoveAt(i); i--;
                }
                foreach (var score in scores) scoreContent.Add(score.Value<JObject>());
            }
            Type scoreType = typeof(ScoreSaber.Plugin).Assembly.GetType("ScoreSaber.Core.Data.Models.Score");
            object scoreData = JsonConvert.DeserializeObject(scoreContent.ToString(), scoreType.MakeArrayType());
            return scoreData;
        }
    }
}
