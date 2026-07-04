using ModularSkillScripts;
using System;
using BattleUI;
using UnityEngine;
using DG.Tweening;
using System.Text.RegularExpressions;
using MTCustomScripts;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace MTCustomScripts.Consequences;

public class ConsequenceSetWorldPosition: IModularConsequence
{
    private class SequenceData
    {
        public Vector3 Pos;
        public float Duration;
        public Ease EaseType;
    }
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
        Il2CppSystem.Collections.Generic.List<BattleUnitModel> units = modular.GetTargetModelList(circles[0]);
        if (units.Count < 1) return;

        string circle1 = circles[1].Replace(" ", "");

        if (circle1.StartsWith("{"))
        {
            // @"\{\s*(-?\d+)\s*;\s*(-?\d+)\s*;\s*(-?\d+)\s*;\s*(\d+)\s*;\s*([^\}\s]+)\s*\}"
            //                                    @"\{([^;}]+);([^;}]+);([^;}]+);([^;}]+);([^;}]+)\}"
            var newTween = Regex.Matches(circle1, @"\{([^;}]+);([^;}]+);([^;}]+);([^;}]+);([^;}]+)\}");
            int matchCount = newTween.Count;
            if (matchCount == 0) return;

            System.Collections.Generic.Dictionary<IntPtr, Sequence> sequences = new System.Collections.Generic.Dictionary<IntPtr, Sequence>(units.Count);

            System.Collections.Generic.List<SequenceData> sequenceData = new System.Collections.Generic.List<SequenceData>(newTween.Count);

            for (int i = 0; i < matchCount; i++)
            {
                Match match = newTween[i];
                SequenceData data = new();
                
                float x = modular.GetNumFromParamString(match.Groups[1].Value) / 100f;
                float y = modular.GetNumFromParamString(match.Groups[2].Value) / 100f;
                float z = modular.GetNumFromParamString(match.Groups[3].Value) / 100f;
                data.Pos = new Vector3(x, y ,z);

                data.Duration = modular.GetNumFromParamString(match.Groups[4].Value) / 100f;
                data.EaseType = Ease.Linear;
                
                if (match.Groups[5].Success)
                Enum.TryParse(match.Groups[5].Value, true, out data.EaseType);

                sequenceData.Add(data);
            }

            foreach (BattleUnitModel bum in units)
            {
                BattleUnitView view = SingletonBehavior<BattleObjectManager>.Instance.GetView(bum);
                if (view == null || view.gameObject == null) continue;

                Sequence sequence = DOTween.Sequence();
                sequence.Pause();
                sequence.SetLink(view.gameObject);

                for (int i = 0; i < sequenceData.Count; i ++)
                {
                    SequenceData data = sequenceData[i];
                    sequence.Append(view.transform.DOMove(data.Pos, data.Duration).SetEase(data.EaseType));
                }

                sequences.Add(bum.Pointer, sequence);
            }

            if (circles.Length > 2)
            {
                int loopCount = modular.GetNumFromParamString(circles[2]);
                LoopType loopType = LoopType.Restart;
                if (circles.Length > 3) Enum.TryParse(circles[3], true, out loopType);

                foreach(Sequence sequence in sequences.Values)
                sequence.SetLoops(loopCount, loopType);
            }

            foreach(Sequence sequence in sequences.Values)
            sequence.Play();
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

// foreach(Match match in newTween)
// {
//     if (match.Groups[5].Success) MTCustomScripts.Main.Logger.LogFatal(match.Groups[5].Value);

//     float x = modular.GetNumFromParamString(match.Groups[1].Value) / 100f;
//     float y = modular.GetNumFromParamString(match.Groups[2].Value) / 100f;
//     float z = modular.GetNumFromParamString(match.Groups[3].Value) / 100f;
//     float duration = modular.GetNumFromParamString(match.Groups[4].Value) / 100f;

//     Ease easingStyle = Ease.Linear;
//     if (match.Groups[5].Success) Enum.TryParse(match.Groups[5].Value, true, out easingStyle);

//     foreach (BattleUnitModel bum in units)
//     {
//         if (!sequences.TryGetValue(bum.Pointer, out var _))
//         {
//             Sequence sequence = DOTween.Sequence();
//             sequence.Pause();
//             sequence.SetLink(SingletonBehavior<BattleObjectManager>.Instance.GetView(bum).gameObject);
//             sequences.Add(bum.Pointer, sequence);
//         }

//         BattleUnitView view = SingletonBehavior<BattleObjectManager>.Instance.GetView(bum);
//         if (view != null) 
//         {
//             sequences[bum.Pointer].Append(view.transform.DOMove(new Vector3(x, y, z), duration).SetEase(easingStyle));
//         }
//     }
// }