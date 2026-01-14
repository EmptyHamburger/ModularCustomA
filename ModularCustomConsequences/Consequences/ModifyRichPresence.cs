using DiscordRPC;
using Lethe;
using ModularSkillScripts;

namespace MTCustomScripts.Consequences
{
    public class ConsequenceModifyRichPresence : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            ActivityType activity = ActivityType.Watching;
            if (circles.Length >= 3 && !Il2CppSystem.Enum.TryParse<ActivityType>(circles[2], true, out activity)) activity = ActivityType.Watching;
            LetheMain.discordRpcClient.ClearPresence();
            LetheMain.discordRpcClient.SetPresence(new RichPresence
            {
                Type = activity,
                Details = circles[0],
                Timestamps = Timestamps.Now,
                State = circles[1],
                Assets = new Assets
                {
                    LargeImageKey = "lethe_icon",
                    LargeImageUrl = "https://lethelc.site/"
                }
            });
        }
    }
}
