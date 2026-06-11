using ModularSkillScripts;
using Lethe.Patches;
using System;

namespace MTCustomScripts.Acquirers;

public class AcquirerHasBuffKeyword : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        /*
        * var_1: single-target
        * var_2: current/buffKeyword
        * var_3: main/sub/maub/category
        * var_4: unique_buff/category_keyword
        * opt_5: store as string
        */

        BattleUnitModel bum = modular.GetTargetModel(circles[0]);
        if (bum == null) return -1;

        BuffModel selectedBuff = null;

        if (circles[1] != "current")
        {
            BUFF_UNIQUE_KEYWORD var1Keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);
            if (bum._buffDetail.HasBuff(var1Keyword) == true) selectedBuff = bum._buffDetail.FindActivatedBuff(var1Keyword, false);
        }
        if (selectedBuff == null) selectedBuff = modular.modsa_buffModel;


        bool flag = true;
        string keywordPrint = string.Empty;
        switch (circles[2])
        {
            case "main":
                BUFF_UNIQUE_KEYWORD resultUniqueKeyword = CustomBuffs.ParseBuffUniqueKeyword(circles[3]);
                if ((resultUniqueKeyword == 0) && (resultUniqueKeyword.ToString() != circles[3])) return -1;
                flag = selectedBuff.IsMainKeyword(resultUniqueKeyword);
                keywordPrint = selectedBuff.GetMainKeyword().ToString();
                break;
            case "sub":
                BUFF_UNIQUE_KEYWORD resultSubUniqueKeyword = CustomBuffs.ParseBuffUniqueKeyword(circles[3]);
                if ((resultSubUniqueKeyword == 0) && (resultSubUniqueKeyword.ToString() != circles[3])) return -1;
                flag = selectedBuff.GetSubKeywordList().Contains(resultSubUniqueKeyword);
                keywordPrint = string.Join("|", selectedBuff.GetSubKeywordList().ToArray());
                break;
            case "maub":
            case "mainsub":
                BUFF_UNIQUE_KEYWORD resultMaubUniqueKeyword = CustomBuffs.ParseBuffUniqueKeyword(circles[3]);
                if ((resultMaubUniqueKeyword == 0) && (resultMaubUniqueKeyword.ToString() != circles[3])) return -1;
                flag = selectedBuff.IsKeyword(resultMaubUniqueKeyword);
                keywordPrint = string.Join("|", selectedBuff.GetKeywordList().ToArray());
                break;
            case "category":
                if (!Enum.TryParse<BUFF_CATEGORY_KEYWORD>(circles[3], true, out var resultCategoryKeyword)) return -1;
                flag = selectedBuff.HasCategoryKeyword(resultCategoryKeyword);
                keywordPrint = string.Join("|", selectedBuff.GetBuffCategoryKeywords().ToArray());
                break;
            default:
                return -1;
        }

        if (flag == true && circles.Length > 4 && circles[4] != null && circles[4] == "print")
            Main.SetCustomMTData(modular.modsa_unitModel.Pointer.ToInt64(), "BuffKeyword_" + circles[2], keywordPrint, "HasBuffKeyword");

        return flag ? 1 : 0;
    }
}