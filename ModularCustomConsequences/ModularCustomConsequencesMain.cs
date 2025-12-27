using BepInEx;
using BepInEx.Unity.IL2CPP;
using Lethe;
using ModularSkillScripts;

namespace MyPlugin;

[BepInPlugin(GUID, NAME, VERSION)]
// this is required to make your plugin run AFTER Modular has been loaded.
[BepInDependency("GlitchGames.ModularSkillScripts")]
public class Main : BasePlugin
{
    // Edit the below to your own plugin name, version, etc.
    public const string NAME = "ModularCustomConsequences";
    public const string VERSION = "1.0.0";
    public const string AUTHOR = "LeagueOfNine";
    public const string GUID = $"{AUTHOR}.{NAME}";

    // creates a class to be added to the consequence dict of modular 
    public class MyConsequenceClass : IModularConsequence
    {
        // put all the actual consequence code inside this execute
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            // The following code is taken from the "Break" consequence of Modular, staggering a unit.

            var modelList = modular.GetTargetModelList(circles[0]); // gets target from first param 
            if (modelList.Count < 1) return;
            // if there is no 2nd argument in the param, use "natural"
            string opt2_string = circles.Length >= 2 ? circles[1] : "natural";
            bool force = opt2_string != "natural";
            bool both = opt2_string == "both";
            bool resistancebreak = circles.Length <= 2;

            // loop over every possible target in the list
            foreach (BattleUnitModel targetModel in modelList)
            {
                // code to change the ability source depending on what the script is run on, a Skill or a Passive
                ABILITY_SOURCE_TYPE abilitySourceType = ABILITY_SOURCE_TYPE.SKILL;
                if (modular.abilityMode == 2) abilitySourceType = ABILITY_SOURCE_TYPE.PASSIVE;

                // possible cases based off of previous parameters
                if (force)
                    targetModel.BreakForcely(modular.modsa_unitModel, abilitySourceType, modular.battleTiming,
                        false, modular.modsa_selfAction);
                if (!force || both)
                    targetModel.Break(modular.modsa_unitModel, modular.battleTiming, modular.modsa_selfAction);
                if (resistancebreak) targetModel.ChangeResistOnBreak();
            }
        }
    }
    
   
    // creates a value getter to be added to the consequence dict of modular 
    public class MyAcquirerClass : IModularAcquirer
    {
        public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
        {
            // the value which this acquirer returns, by convention, -1 should be returned in case of an error
            return 42;
        }
    }

    public override void Load()
    {
        // Adds the consequence to the consequence dict in modular
        MainClass.consequenceDict["myConsequence"] = new MyConsequenceClass();
        MainClass.acquirerDict["myAcquirer"] = new MyAcquirerClass();
        // optional logging, feel free to remove.
        LetheHooks.LOG.LogError("THE MODULAR CONSEQUNCE HAS ARRIVED!!!");
    }
}