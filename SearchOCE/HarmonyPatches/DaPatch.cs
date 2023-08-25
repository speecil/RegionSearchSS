using HarmonyLib;
using System;
using System.Reflection;

namespace SearchOCE.HarmonyPatches
{
    [HarmonyPatch]
    internal class DaPatch
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            Type leaderboardServiceType = typeof(ScoreSaber.Plugin).Assembly.GetType("ScoreSaber.Core.Services.LeaderboardService");
            return leaderboardServiceType.GetMethod("GetLeaderboardUrl", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(IDifficultyBeatmap), typeof(PlatformLeaderboardsModel.ScoresScope), typeof(int), typeof(bool) }, null);
        }


        [HarmonyPrefix]
        public static bool Prefix(ref string __result, IDifficultyBeatmap difficultyBeatmap, PlatformLeaderboardsModel.ScoresScope scope, int page, bool filterAroundCountry)
        {
            Plugin.Log.Info("FORTNITE PREFIX");
            string difficulty = BeatmapDifficultyMethods.DefaultRating(difficultyBeatmap.difficulty).ToString();
            if (filterAroundCountry)
            {
                __result = $"leaderboard/by-hash/{difficultyBeatmap.level.levelID.Split('_')[2]}/scores?difficulty={difficulty}&countries=AU,NZ&page={page}";
                return false;
            }
            return true;
        }
    }

    //[HarmonyPatch]
    //internal class DaPatch2
    //{
    //    [HarmonyTargetMethod]
    //    public static MethodBase TargetMethod()
    //    {
    //        Type leaderboardServiceType = typeof(ScoreSaber.Plugin).Assembly.GetType("ScoreSaber.Core.Services.LeaderboardService");
    //        return leaderboardServiceType.GetMethod("GetLeaderboardData", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(IDifficultyBeatmap), typeof(PlatformLeaderboardsModel.ScoresScope), typeof(PlayerSpecificSettings), typeof(bool) }, null);
    //    }


    //    [HarmonyPrefix]
    //    public static bool Prefix(object __instance, IDifficultyBeatmap difficultyBeatmap, PlatformLeaderboardsModel.ScoresScope scope, PlayerSpecificSettings playerSpecificSettings, bool filterAroundCountry)
    //    {
    //        if (!filterAroundCountry) return false;

    //        Type instanceType = __instance.GetType();
    //        FieldInfo leaderboardRawDataField = instanceType.GetField("leaderboardRawData", BindingFlags.NonPublic | BindingFlags.Instance);

    //        if (leaderboardRawDataField != null)
    //        {
    //            string leaderboardRawData = (string)leaderboardRawDataField.GetValue(__instance);

    //            // magic

    //            leaderboardRawDataField.SetValue(__instance, leaderboardRawData);
    //        }

    //        return true;
    //    }
    //}

}


