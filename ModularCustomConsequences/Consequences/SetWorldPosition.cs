using ModularSkillScripts;
using System;
using BattleUI;
using UnityEngine;
using DG.Tweening;

namespace MTCustomScripts.Consequences;

public class ConsequenceSetWorldPosition: IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
        Il2CppSystem.Collections.Generic.List<BattleUnitModel> unit = modular.GetTargetModelList(circles[0]);
        float x = modular.GetNumFromParamString(circles[1]) / 100f;
        float y = modular.GetNumFromParamString(circles[2]) / 100f;
        float z = modular.GetNumFromParamString(circles[3]) / 100f;

        if (circles.Length > 4)
        {
            float duration = modular.GetNumFromParamString(circles[5]) / 100f;
            string easingStyle = circles[6];

            Ease newEase = (Ease) Enum.Parse(typeof(Ease), easingStyle);

            foreach (BattleUnitModel bum in unit)
            {
                SingletonBehavior<BattleObjectManager>.Instance.GetView(bum).transform.DOMove(new Vector3(x, y, z), duration).SetEase(newEase);
            }
        }
        else
        {
            foreach (BattleUnitModel bum in unit)
            {
                SingletonBehavior<BattleObjectManager>.Instance.GetView(bum).WorldPosition = new Vector3(x, y, z);
            }
        }
	}
}