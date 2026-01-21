using BepInEx.Logging;
using DiscordRPC;
using Lethe;
using ModularSkillScripts;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MTCustomScripts.Consequences
{
    public class ConsequenceModifyRichPresence : IModularConsequence
    {
        // Token: 0x0600004E RID: 78 RVA: 0x00006080 File Offset: 0x00004280
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            if (circles == null)
            {
                Main.Logger.Log(LogLevel.Debug, "ModifyRichPresenceIsNull");
                circles = new string[]
                {
                "Child_Touching_Simulator_Ultimate:_Epstein_Battle_Grounds_3",
                "Active_Match:_Ranked\nTier:_Platinum_V",
                "true_lethe_icon",
                "Kids_Touched:_32 \\ 39",
                "true_lethe_icon",
                "7_Matches_Until_Rank_Up",
                "Competing",
                "1",
                "2",
                "67",
                "Public",
                "68",
                "69"
                };
            }
            GameObject gameObject = GameObject.Find("DiscordRPCUpdater");
            if (gameObject != null && gameObject.active) gameObject.SetActive(false);
            ActivityType type = ActivityType.Watching;
            if (circles.Length >= 7 && !Il2CppSystem.Enum.TryParse<ActivityType>(circles[6], true, out type)) type = ActivityType.Watching;
            circles[0] = Regex.Replace(circles[0], "_", " ");
            circles[1] = Regex.Replace(circles[1], "_", " ");
            circles[3] = Regex.Replace(circles[3], "_", " ");
            circles[5] = Regex.Replace(circles[5], "_", " ");
            LetheMain.discordRpcClient.ClearPresence();
            LetheMain.discordRpcClient = new DiscordRpcClient("1460766292683522120");
            LetheMain.discordRpcClient.RegisterUriScheme();
            Party party = new Party();
            if (circles.Length >= 7)
            {
                party.Size = modular.GetNumFromParamString(circles[7]);
                party.Max = modular.GetNumFromParamString(circles[8]);
                party.ID = circles[9];
                Party.PrivacySetting privacy = Party.PrivacySetting.Private;
                if (!Il2CppSystem.Enum.TryParse<Party.PrivacySetting>(circles[10], true, out privacy)) privacy = Party.PrivacySetting.Private;
                party.Privacy = privacy;
            }
            /*
            Secrets secrets = new Secrets();
            if (circles.Length >= 12)
            {
                secrets.SpectateSecret = circles[11];
                secrets.JoinSecret = circles[12];
            }
            */
            LetheMain.discordRpcClient.SetPresence(new RichPresence
            {
                Type = type,
                Details = circles[0],
                Timestamps = Timestamps.Now,
                State = circles[1],
                Party = party,
                //Secrets = secrets,
                Assets = new Assets
                {
                    SmallImageKey = circles[2],
                    SmallImageText = circles[3],
                    LargeImageKey = circles[4],
                    LargeImageText = circles[5]
                }
            });
        }
    }
}
