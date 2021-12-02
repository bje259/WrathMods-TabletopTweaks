using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Properties;
using System;
using System.Linq;
using TabletopTweaks.Config;
using TabletopTweaks.Extensions;
using TabletopTweaks.Utilities;

namespace TabletopTweaks.Bugfixes.General {
    class ShadowMagicFix {
        [HarmonyPatch(typeof(AutoMetamagic), nameof(AutoMetamagic.ShouldApplyTo), new Type[] { 
            typeof(AutoMetamagic),
            typeof(BlueprintAbility),
            typeof(AbilityData)
        })]
        static class AutoMetamagic_ShouldApplyTo_Shadow_Patch {
            static void Postfix(AutoMetamagic c, BlueprintAbility ability, AbilityData data, ref bool __result) {
                if (ModSettings.Fixes.BaseFixes.IsDisabled("FixShadowSpells")) { return; }
                if (data?.ConvertedFrom?.Blueprint?.GetComponent<AbilityShadowSpell>() != null) {
                    __result |= AutoMetamagic.ShouldApplyTo(c, data.ConvertedFrom.Blueprint, data.ConvertedFrom);
                }
            }
        }
        [HarmonyPatch(typeof(IncreaseSpellDescriptorDC), "OnEventAboutToTrigger", new Type[] { typeof(RuleCalculateAbilityParams) })]
        static class IncreaseSpellDescriptorDC_OnEventAboutToTrigger_Shadow_Patch {
            static bool Prefix(IncreaseSpellDescriptorDC __instance, RuleCalculateAbilityParams evt) {
                if (ModSettings.Fixes.BaseFixes.IsDisabled("FixShadowSpells")) { return true; }
                SpellDescriptorComponent component = evt.Spell.GetComponent<SpellDescriptorComponent>();

                var ParentAbility = evt.AbilityData?.ConvertedFrom;
                if (ParentAbility?.Blueprint?.GetComponent<AbilityShadowSpell>() != null) {
                    component = ParentAbility.Blueprint.GetComponent<SpellDescriptorComponent>();
                }

                if (component != null && component.Descriptor.HasAnyFlag(__instance.Descriptor)) {
                    evt.AddBonusDC(__instance.BonusDC);
                }
                return false;
            }
        }
        [HarmonyPatch(typeof(IncreaseSpellSchoolDC), "OnEventAboutToTrigger", new Type[] { typeof(RuleCalculateAbilityParams) })]
        static class IncreaseSpellSchoolDC_OnEventAboutToTrigger_Shadow_Patch {

            static bool Prefix(IncreaseSpellSchoolDC __instance, RuleCalculateAbilityParams evt) {
                if (ModSettings.Fixes.BaseFixes.IsDisabled("FixShadowSpells")) { return true; }
                var spell = evt.Spell;
                var ParentAbility = evt.AbilityData?.ConvertedFrom;
                if (ParentAbility?.Blueprint?.GetComponent<AbilityShadowSpell>() != null) {
                    spell = ParentAbility.Blueprint;
                }

                bool isSchool = __instance.School == SpellSchool.None;
                if (!isSchool) {
                    foreach (SpellComponent spellComponent in spell.GetComponents<SpellComponent>()) {
                        isSchool = (spellComponent.School == __instance.School);
                    }
                }
                if (isSchool) {
                    evt.AddBonusDC(__instance.BonusDC);
                }
                return false;
            }
        }

        /*[HarmonyPatch(typeof(AutoMetamagic), "OnEventAboutToTrigger", new Type[] { typeof(RuleCalculateAbilityParams) })]

        static class AutoMetamagic_OnEventAboutToTrigger_Shadow_Patch {

            static bool Prefix(AutoMetamagic __instance, RuleCalculateAbilityParams evt) {
                if (ModSettings.Fixes.BaseFixes.IsDisabled("FixShadowSpells")) { return true; }
                var spell = evt.Spell;
                var ParentAbility = evt.AbilityData?.ConvertedFrom;
                if (ParentAbility?.Blueprint?.GetComponent<AbilityShadowSpell>() != null) {
                    spell = ParentAbility.Blueprint;
                    //empower
                    if (__instance.Owner.Buffs.GetBuff(Resources.GetBlueprint<BlueprintBuff>("a0e8e970756146c99cbe1c611e6deecd")) != null) {
                        var IllusionMetaEmpower = Resources.GetBlueprint<BlueprintBuff>("a0e8e970756146c99cbe1c611e6deecd");
                        IllusionMetaEmpower.GetComponent<AutoMetamagic>().School = spell.School;
                        evt.AddMetamagic(Metamagic.Empower);
                        //evt.AddMetamagic(Kingmaker.UnitLogic.Abilities.Metamagic.Empower);
                        //Main.LogDebug($"Tried to add Metamagic1");
                    }
                    //extend
                    if (__instance.Owner.Buffs.GetBuff(Resources.GetBlueprint<BlueprintBuff>("ad00aeea70a548aaa4b213038c9d8963")) != null) {
                        var IllusionMetaEmpower = Resources.GetBlueprint<BlueprintBuff>("ad00aeea70a548aaa4b213038c9d8963");
                        IllusionMetaEmpower.GetComponent<AutoMetamagic>().School = spell.School;
                        evt.AddMetamagic(Metamagic.Extend);
                        //evt.AddMetamagic(Kingmaker.UnitLogic.Abilities.Metamagic.Empower);
                        //Main.LogDebug($"Tried to add Metamagic1");
                    }
                    //selective
                    if (__instance.Owner.Buffs.GetBuff(Resources.GetBlueprint<BlueprintBuff>("fac1de8143b945aaa1d48d30d25066c2")) != null) {
                        var IllusionMetaEmpower = Resources.GetBlueprint<BlueprintBuff>("fac1de8143b945aaa1d48d30d25066c2");
                        IllusionMetaEmpower.GetComponent<AutoMetamagic>().School = spell.School;
                        evt.AddMetamagic(Metamagic.Selective);
                        //evt.AddMetamagic(Kingmaker.UnitLogic.Abilities.Metamagic.Empower);
                        //Main.LogDebug($"Tried to add Metamagic1");
                    }
                    //reach
                    if (__instance.Owner.Buffs.GetBuff(Resources.GetBlueprint<BlueprintBuff>("985da6a662924b22aaf02cf059315aa1")) != null) {
                        var IllusionMetaEmpower = Resources.GetBlueprint<BlueprintBuff>("985da6a662924b22aaf02cf059315aa1");
                        IllusionMetaEmpower.GetComponent<AutoMetamagic>().School = spell.School;
                        evt.AddMetamagic(Metamagic.Reach);
                        //evt.AddMetamagic(Kingmaker.UnitLogic.Abilities.Metamagic.Empower);
                        //Main.LogDebug($"Tried to add Metamagic1");
                    }
                    //quickened
                    if (__instance.Owner.Buffs.GetBuff(Resources.GetBlueprint<BlueprintBuff>("f44bbc18206641099c08913c8663b614")) != null) {
                        var IllusionMetaEmpower = Resources.GetBlueprint<BlueprintBuff>("f44bbc18206641099c08913c8663b614");
                        IllusionMetaEmpower.GetComponent<AutoMetamagic>().School = spell.School;
                        evt.AddMetamagic(Metamagic.Quicken);
                        //evt.AddMetamagic(Kingmaker.UnitLogic.Abilities.Metamagic.Empower);
                        //Main.LogDebug($"Tried to add Metamagic1");
                    }
                    //max
                    if (__instance.Owner.Buffs.GetBuff(Resources.GetBlueprint<BlueprintBuff>("dfade9d3bd314bf788ab58dd7716e0f3")) != null) {
                        var IllusionMetaEmpower = Resources.GetBlueprint<BlueprintBuff>("dfade9d3bd314bf788ab58dd7716e0f3");
                        IllusionMetaEmpower.GetComponent<AutoMetamagic>().School = spell.School;
                        evt.AddMetamagic(Metamagic.Maximize);
                        //evt.AddMetamagic(Kingmaker.UnitLogic.Abilities.Metamagic.Empower);
                        //Main.LogDebug($"Tried to add Metamagic1");
                    }
                    //persistent
                    if (__instance.Owner.Buffs.GetBuff(Resources.GetBlueprint<BlueprintBuff>("6319ef758d444b0fa7bdaf0482fddf02")) != null) {
                        var IllusionMetaEmpower = Resources.GetBlueprint<BlueprintBuff>("6319ef758d444b0fa7bdaf0482fddf02");
                        IllusionMetaEmpower.GetComponent<AutoMetamagic>().School = spell.School;
                        evt.AddMetamagic(Metamagic.Persistent);
                        //evt.AddMetamagic(Kingmaker.UnitLogic.Abilities.Metamagic.Empower);
                        //Main.LogDebug($"Tried to add Metamagic1");
                    }

                }
                return false;
            }
        }*/

        [HarmonyPatch(typeof(SpellFocusParametrized), "OnEventAboutToTrigger", new Type[] { typeof(RuleCalculateAbilityParams) })]
        static class SpellFocusParametrized_OnEventAboutToTrigger_Shadow_Patch {

            static bool Prefix(SpellFocusParametrized __instance, RuleCalculateAbilityParams evt) {
                if (ModSettings.Fixes.BaseFixes.IsDisabled("FixShadowSpells")) { return true; }
                var spell = evt.Spell;
                var ParentAbility = evt.AbilityData?.ConvertedFrom;
                if (ParentAbility?.Blueprint?.GetComponent<AbilityShadowSpell>() != null) {
                    spell = ParentAbility.Blueprint;
                }
                SpellSchool school = spell?.GetComponent<SpellComponent>()?.School ?? SpellSchool.None;
                //SpellSchool ? nullable = spell != null ? spell.GetComponent<SpellComponent>()?.School : new SpellSchool?();
                //SpellSchool school = nullable.HasValue ? nullable.GetValueOrDefault() : SpellSchool.None;
                int num1 = school == __instance.Param ? 1 : 0;
                int num2;
                if (!__instance.Owner.Progression.Features.Enumerable.Any(p => p.Blueprint == __instance.Fact.Blueprint && p.Param == school)) {
                    UnitPartExpandedArsenal partExpandedArsenal = __instance.Owner.Get<UnitPartExpandedArsenal>();
                    num2 = partExpandedArsenal != null ? (partExpandedArsenal.HasSpellSchoolEntry(school) ? 1 : 0) : 0;
                } else
                    num2 = 0;
                bool flag = num2 != 0;
                int num3 = evt.Initiator.Progression.Features.Enumerable.Any(p => p.Param == __instance.Param && p.Blueprint == __instance.MythicFocus) ? 2 : 1;
                int num4 = flag ? 1 : 0;
                if ((num1 | num4) == 0) {
                    return false;
                }
                evt.AddBonusDC(__instance.BonusDC * num3);
                return false;
            }
        }
    }
}
