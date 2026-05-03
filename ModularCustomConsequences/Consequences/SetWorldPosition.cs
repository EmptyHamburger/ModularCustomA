using ModularSkillScripts;
using System;
using BattleUI;
using UnityEngine;
using DG.Tweening;
using System.Text.RegularExpressions;
using MTCustomScripts;

namespace MTCustomScripts.Consequences;

public class ConsequenceSetWorldPosition: IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
        Il2CppSystem.Collections.Generic.List<BattleUnitModel> units = modular.GetTargetModelList(circles[0]);
        if (units.Count < 1) return;
        // MTCustomScripts.Main.Logger.LogFatal(circles[0]);
        // MTCustomScripts.Main.Logger.LogFatal(circles[1]);
        
        // MTCustomScripts.Main.Logger.LogFatal(section);
        // MTCustomScripts.Main.Logger.LogFatal(circledSection);

        // MTCustomScripts.Main.Logger.LogFatal(circles);

        string circle1 = circles[1].Replace(" ", "");

        if (circle1.StartsWith("{"))
        {
            // @"\{\s*(-?\d+)\s*;\s*(-?\d+)\s*;\s*(-?\d+)\s*;\s*(\d+)\s*;\s*([^\}\s]+)\s*\}"
            var newTween = Regex.Matches(circle1, @"\{([^;}]+);([^;}]+);([^;}]+);([^;}]+);([^;}]+)\}");
            System.Collections.Generic.Dictionary<IntPtr, Sequence> sequences = new();

            foreach(Match match in newTween)
            {
                // MTCustomScripts.Main.Logger.LogFatal(match.Value);
                // MTCustomScripts.Main.Logger.LogFatal(match.Groups[1].Value);
                // MTCustomScripts.Main.Logger.LogFatal(match.Groups[2].Value);
                // MTCustomScripts.Main.Logger.LogFatal(match.Groups[3].Value);
                // MTCustomScripts.Main.Logger.LogFatal(match.Groups[4].Value);
                if (match.Groups[5].Success) MTCustomScripts.Main.Logger.LogFatal(match.Groups[5].Value);

                // float x = int.Parse(match.Groups[1].Value) / 100f;
                // float y = int.Parse(match.Groups[2].Value) / 100f;
                // float z = int.Parse(match.Groups[3].Value) / 100f;
                // float duration = int.Parse(match.Groups[4].Value) / 100f;
                float x = modular.GetNumFromParamString(match.Groups[1].Value) / 100f;
                float y = modular.GetNumFromParamString(match.Groups[2].Value) / 100f;
                float z = modular.GetNumFromParamString(match.Groups[3].Value) / 100f;
                float duration = modular.GetNumFromParamString(match.Groups[4].Value) / 100f;

                Ease easingStyle = Ease.Linear;
                if (match.Groups[5].Success) Enum.TryParse(match.Groups[5].Value, true, out easingStyle);

                foreach (BattleUnitModel bum in units)
                {
                    if (!sequences.TryGetValue(bum.Pointer, out var _))
                    {
                        Sequence sequence = DOTween.Sequence();
                        sequence.Pause();
                        sequence.SetLink(SingletonBehavior<BattleObjectManager>.Instance.GetView(bum).gameObject);
                        sequences.Add(bum.Pointer, sequence);
                    }

                    BattleUnitView view = SingletonBehavior<BattleObjectManager>.Instance.GetView(bum);
                    if (view != null) 
                    {
                        sequences[bum.Pointer].Append(view.transform.DOMove(new Vector3(x, y, z), duration).SetEase(easingStyle));
                    }
                }
            }

            if (circles.Length > 2)
            {
                int loopCount = modular.GetNumFromParamString(circles[2]);
                LoopType loopType = LoopType.Restart;
                if (circles.Length > 3) Enum.TryParse(circles[3], true, out loopType);

                foreach(Sequence sequence in sequences.Values)
                {
                    sequence.SetLoops(loopCount, loopType);
                }
            }

            foreach(Sequence sequence in sequences.Values)
            {
                sequence.Play();
            }
        }
        else
        {
            if (circles.Length < 4) return;

            foreach (BattleUnitModel bum in units)
            {
                BattleUnitView view = SingletonBehavior<BattleObjectManager>.Instance.GetView(bum);
                if (view != null)
                {
                    int x = modular.GetNumFromParamString(circles[1]);
                    int y = modular.GetNumFromParamString(circles[2]);
                    int z = modular.GetNumFromParamString(circles[3]);

                    view.WorldPosition = new Vector3(
                        x / 100f,
                        y / 100f,
                        z / 100f
                    );
                }
            }
        }
	}
}