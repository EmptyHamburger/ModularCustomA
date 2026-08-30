using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerGainBuffSource : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        if (modular.modsa_passiveModel == null && modular.modsa_buffModel == null) return -1;

        return MTCustomScripts.Main.Instance.gainbuff_source switch
        {
            ABILITY_SOURCE_TYPE.NONE => 0,
            ABILITY_SOURCE_TYPE.SKILL => 1,
            ABILITY_SOURCE_TYPE.EVENT => 2,
            ABILITY_SOURCE_TYPE.BUFF => 3,
            ABILITY_SOURCE_TYPE.PASSIVE => 4,
            ABILITY_SOURCE_TYPE.SYSTEM_ABILITY => 5,
            ABILITY_SOURCE_TYPE.EGO_GIFT => 6,
            ABILITY_SOURCE_TYPE.PATTERN => 7,
            ABILITY_SOURCE_TYPE.STAGE => 8,
            ABILITY_SOURCE_TYPE.UNIT => 9,
            _ => -1,
        };
    }
}