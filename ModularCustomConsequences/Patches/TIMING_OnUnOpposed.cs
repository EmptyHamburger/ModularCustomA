using System;
using System.Collections.Generic;
using HarmonyLib;
using ModularSkillScripts;
using ModularSkillScripts.Patches;

namespace MTCustomScripts.Patches
{
    internal static class OnUnOpposedState
    {
        public sealed class Accum
        {
            public BattleUnitModel target;
            public BattleActionModel action;
            public int realDmg;
            public int hpDmg;
        }

        public static readonly Dictionary<long, Accum> _accum = new Dictionary<long, Accum>();
        public static readonly Dictionary<long, bool> _wasDuel = new Dictionary<long, bool>();
        public static readonly Dictionary<int, Queue<long>> _queue = new Dictionary<int, Queue<long>>();

        public static void ClearAll()
        {
            _accum.Clear();
            _wasDuel.Clear();
            _queue.Clear();
        }
    }

    internal class OnUnOpposed
    {
        [HarmonyPatch(typeof(BattleActionModelManager), nameof(BattleActionModelManager.Run), new Type[] { })]
        [HarmonyPrefix, HarmonyPriority(Priority.VeryHigh)]
        public static void Prefix_BattleActionModelManager_RunAll()
        {
            OnUnOpposedState.ClearAll();
        }

        [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnTakeAttackDamage))]
        [HarmonyPostfix, HarmonyPriority(Priority.VeryHigh)]
        public static void Postfix_BattleUnitModel_OnTakeAttackDamage(BattleActionModel action, int realDmg, int hpDamage, BattleUnitModel __instance)
        {
            if (__instance == null || action == null) return;
            long key = action.Pointer.ToInt64();
            if (!OnUnOpposedState._accum.TryGetValue(key, out var a))
            {
                a = new OnUnOpposedState.Accum { target = __instance, action = action };
                OnUnOpposedState._accum[key] = a;
            }
            if (a.target == null) a.target = __instance;
            if (a.action == null) a.action = action;
            a.realDmg += realDmg;
            a.hpDmg += hpDamage;
        }

        [HarmonyPatch(typeof(BattleActionModelManager), nameof(BattleActionModelManager.Run), new Type[] { typeof(BattleActionModel) })]
        [HarmonyPrefix, HarmonyPriority(Priority.VeryHigh)]
        public static void Prefix_BattleActionModelManager_RunAction(BattleActionModel action, BattleActionModelManager __instance)
        {
            if (__instance == null || action == null) return;

            bool wasDuel = __instance.GetDuelModel(action) != null;
            long key = action.Pointer.ToInt64();
            OnUnOpposedState._wasDuel[key] = wasDuel;

            if (!OnUnOpposedState._accum.TryGetValue(key, out var a))
            {
                a = new OnUnOpposedState.Accum { action = action };
                OnUnOpposedState._accum[key] = a;
            }
            else if (a.action == null)
            {
                a.action = action;
            }

            BattleUnitModel attacker = action.Model;
            if (attacker == null) return;
            int attackerId = attacker.InstanceID;

            if (!OnUnOpposedState._queue.TryGetValue(attackerId, out var q))
            {
                q = new Queue<long>();
                OnUnOpposedState._queue[attackerId] = q;
            }
            q.Enqueue(key);
        }

        [HarmonyPatch(typeof(BattleUnitView), "EndBehaviourAction")]
        [HarmonyPostfix, HarmonyPriority(Priority.VeryHigh)]
        public static void Postfix_BattleUnitView_EndBehaviourAction(BattleUnitView __instance)
        {
            if (__instance == null) return;
            BattleUnitModel attacker = __instance._unitModel;
            if (attacker == null) return;
            int attackerId = attacker.InstanceID;

            if (!OnUnOpposedState._queue.TryGetValue(attackerId, out var q) || q.Count == 0) return;
            long key = q.Dequeue();

            OnUnOpposedState._accum.TryGetValue(key, out var a);
            OnUnOpposedState._wasDuel.TryGetValue(key, out bool wasDuel);

            OnUnOpposedState._accum.Remove(key);
            OnUnOpposedState._wasDuel.Remove(key);

            if (wasDuel) return;
            if (a == null) return;

            BattleActionModel action = a.action;
            if (action == null || !action.IsAttack()) return;

            BattleUnitModel target = a.target ?? action.GetMainTarget();
            if (target == null) return;

            int actevent = MainClass.timingDict["OnUnOpposed"];

            foreach (PassiveModel passiveModel in target._passiveDetail.PassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    modpa.modsa_passiveModel = passiveModel;
                    modpa.modsa_killerModel = attacker;
                    modpa.lastFinalDmg = a.realDmg;
                    modpa.lastHpDmg = a.hpDmg;
                    modpa.modsa_target_list.Clear();
                    modpa.modsa_target_list.Add(target);
                    modpa.Enact(target, null, null, action, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                }
            }

            foreach (PassiveModel passiveModel in target._passiveDetail.EgoPassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    modpa.modsa_passiveModel = passiveModel;
                    modpa.modsa_killerModel = attacker;
                    modpa.lastFinalDmg = a.realDmg;
                    modpa.lastHpDmg = a.hpDmg;
                    modpa.modsa_target_list.Clear();
                    modpa.modsa_target_list.Add(target);
                    modpa.Enact(target, null, null, action, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                }
            }

            foreach (BuffModel buffModel in target._buffDetail.GetActivatedBuffModelAll())
            {
                long buffmodel_intlong = buffModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modbaDict.ContainsKey(buffmodel_intlong)) continue;

                foreach (ModularSA modba in SkillScriptInitPatch.modbaDict[buffmodel_intlong])
                {
                    modba.modsa_buffModel = buffModel;
                    modba.modsa_killerModel = attacker;
                    modba.lastFinalDmg = a.realDmg;
                    modba.lastHpDmg = a.hpDmg;
                    modba.modsa_target_list.Clear();
                    modba.modsa_target_list.Add(target);
                    modba.Enact(target, null, null, action, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                }
            }
        }
    }
}
