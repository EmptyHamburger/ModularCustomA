using ModularSkillScripts;
using Lethe.Patches;
using System;

namespace MTCustomScripts.Acquirers;

public class AcquirerHasBuffKeyword : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        /*
        * var_1: current/buffKeyword
        * var_2: main/sub/maub/category
        * var_3: unique_buff/category_keyword
        * opt_4: store as string
        */

        BuffModel selectedBuff = null;

        if (circles[0] != "current")
        {
            BUFF_UNIQUE_KEYWORD var1Keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[0]);
            if (modular.modsa_unitModel._buffDetail.HasBuff(var1Keyword) == true) selectedBuff = modular.modsa_unitModel._buffDetail.FindActivatedBuff(var1Keyword, true);
        }
        if (selectedBuff == null) selectedBuff = modular.modsa_buffModel;


        bool flag = true;
        string keywordPrint = string.Empty;
        switch (circles[1])
        {
            case "main":
                BUFF_UNIQUE_KEYWORD resultUniqueKeyword = CustomBuffs.ParseBuffUniqueKeyword(circles[2]);
                if ((resultUniqueKeyword == 0) && (resultUniqueKeyword.ToString() != circles[2])) return -1;
                flag = selectedBuff.IsMainKeyword(resultUniqueKeyword);
                keywordPrint = selectedBuff.GetMainKeyword().ToString();
                break;
            case "sub":
                BUFF_UNIQUE_KEYWORD resultSubUniqueKeyword = CustomBuffs.ParseBuffUniqueKeyword(circles[2]);
                if ((resultSubUniqueKeyword == 0) && (resultSubUniqueKeyword.ToString() != circles[2])) return -1;
                flag = selectedBuff.GetSubKeywordList().Contains(resultSubUniqueKeyword);
                keywordPrint = string.Join("|", selectedBuff.GetSubKeywordList().ToArray());
                break;
            case "maub":
            case "mainsub":
                BUFF_UNIQUE_KEYWORD resultMaubUniqueKeyword = CustomBuffs.ParseBuffUniqueKeyword(circles[2]);
                if ((resultMaubUniqueKeyword == 0) && (resultMaubUniqueKeyword.ToString() != circles[2])) return -1;
                flag = selectedBuff.IsKeyword(resultMaubUniqueKeyword);
                keywordPrint = string.Join("|", selectedBuff.GetKeywordList().ToArray());
                break;
            case "category":
                if (!Enum.TryParse<BUFF_CATEGORY_KEYWORD>(circles[2], true, out var resultCategoryKeyword)) return -1;
                flag = selectedBuff.HasCategoryKeyword(resultCategoryKeyword);
                keywordPrint = string.Join("|", selectedBuff.GetBuffCategoryKeywords().ToArray());
                break;
            default:
                return -1;
        }

        if (flag == true && circles.Length > 2 && circles[3] != null && circles[3] == "print")
            MTCustomScripts.Main.TestStuffStorage.stringDict[string.Format("{0}{1}{2}", modular.ptr_intlong, "BuffKeyword_", circles[1])] = keywordPrint;

        return flag ? 1 : 0;
    }
}