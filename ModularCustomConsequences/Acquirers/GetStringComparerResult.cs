using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerGetStringComparerResult : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        /*
        var_1: input
        var_2: operator
        var_3: dictionnary key
        */

        string key = string.Format("{0}{1}", modular.ptr_intlong, circles[2]);

        if (MTCustomScripts.Main.TestStuffStorage.stringDict.TryGetValue(key, out string storedValue))
        {
            if (circles[1] == "=") return circles[0] == storedValue ? 1 : 0;
            else if (circles[1] == "!") return circles[0].Equals(storedValue) ? 0 : 1;
            else if (circles[1] == ">") return circles[0].Contains(storedValue) ? 1 : 0;
            else if (circles[1] == "<") return circles[0].Contains(storedValue) ? 0 : 1;

            else return 0;
        }
        return -1;
    }
}