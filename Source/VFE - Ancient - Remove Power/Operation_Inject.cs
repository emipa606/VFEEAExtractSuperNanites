using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using VFEAncients;

namespace VFEE_Ancient_ExtractPower;

public class Operation_Inject(CompGeneTailoringPod pod) : Operation(pod), IExposable
{
    private readonly float failChance =
        LoadedModManager.GetMod<VFEEA_Mod>().GetSettings<VFEEA_ModSettings>().injectFailChance;

    public PowerDef selectedPowers;

    public override string Label => "VFEEA_ExtractPWR.Inject".Translate();

    protected virtual int MaxPowerLevel
    {
        get
        {
            return (from comp in Pod.parent.TryGetComp<CompAffectedByFacilities>().LinkedFacilitiesListForReading
                    .OfType<ThingWithComps>().SelectMany(t => t.AllComps)
                select comp.props).OfType<CompProperties_Facility_PowerUnlock>().Append(
                new CompProperties_Facility_PowerUnlock
                {
                    unlockedLevels = 3
                }).Sum(props => props.unlockedLevels);
        }
    }

    public new void ExposeData()
    {
        Scribe_Defs.Look(ref selectedPowers, "VFEEA_selected_IJ_Powers");
    }

    public override float FailChanceOnPawn(Pawn pawn)
    {
        return base.FailChanceOnPawn(pawn) + failChance;
    }

    public override string FailChanceExplainOnPawn(Pawn pawn)
    {
        return string.Concat(base.FailChanceExplainOnPawn(pawn), "\n", Label, ": +",
            ((int)(failChance * 100f)).ToString(), "%");
    }

    public override int StartOnPawnGetDuration()
    {
        _ = Pod.Occupant.GetPowerTracker();
        var list = (from t in Pod.parent.GetComp<CompAffectedByFacilities>().LinkedFacilitiesListForReading
            where t.def == VFEEA_DefOf.VFEEA_PowerInjector
            select t).Cast<Building_PowerInjector>().ToList();

        var list2 = new List<Tuple<PowerDef, Building_PowerInjector>>();
        foreach (var building_PowerInjector in list)
        {
            if (building_PowerInjector.isEmpty() ||
                building_PowerInjector.InjectorState != Enum_InjectorState.Inactive)
            {
                continue;
            }

            var vfeea_powerDef = building_PowerInjector.ActiveSN.TryGetComp<ThingComp_powerDef>().VFEEA_powerDef;
            list2.Add(new Tuple<PowerDef, Building_PowerInjector>(DefDatabase<PowerDef>.GetNamed(vfeea_powerDef),
                building_PowerInjector));
        }

        Find.WindowStack.Add(new Dialog_InjectPowers(list2, Pod.Occupant, OnChosen));
        return TicksRequired;

        void OnChosen(Tuple<PowerDef, Building_PowerInjector> powers_and_injector)
        {
            selectedPowers = powers_and_injector.Item1;
            powers_and_injector.Item2.InjectorState = Enum_InjectorState.Injecting;
        }
    }

    public override bool CanRunOnPawn(Pawn pawn)
    {
        var injectorList = (from t in Pod.parent.GetComp<CompAffectedByFacilities>().LinkedFacilitiesListForReading
            where t.def == VFEEA_DefOf.VFEEA_PowerInjector
            select t).Cast<Building_PowerInjector>().ToList();
        var hasInactive = false;
        if (base.CanRunOnPawn(pawn))
        {
            if (Pod.parent.GetComp<CompAffectedByFacilities>().LinkedFacilitiesListForReading
                    .Any(t => t.def == VFEEA_DefOf.VFEEA_PowerInjector) && isAnySNInserted(injectorList))
            {
                hasInactive = isAnyInactivate(injectorList);
            }
        }

        bool result;
        if (hasInactive)
        {
            var powerTracker = pawn.GetPowerTracker();
            int? num;
            if (powerTracker == null)
            {
                num = null;
            }
            else
            {
                num = powerTracker.AllPowers.Count(power => power.powerType == 0);
            }

            var num2 = num;
            var maxPowerLevel = MaxPowerLevel;
            result = (num2.GetValueOrDefault() < maxPowerLevel) & (num2 != null);
        }
        else
        {
            result = false;
        }

        return result;
    }

    public override void Success()
    {
        var powerTracker = Pod.Occupant.GetPowerTracker();
        InjectSucess();
        if (powerTracker != null)
        {
            Find.LetterStack.ReceiveLetter("VFEEA_ExtractPWR.Inject.Label".Translate(powerTracker.Pawn.LabelShortCap),
                "VFEEA_ExtractPWR.Inject.Text".Translate(powerTracker.Pawn.NameShortColored, selectedPowers.LabelCap),
                LetterDefOf.PositiveEvent, powerTracker.Pawn);
        }
        else
        {
            Log.Message("[VFEEA - Extract Power] - An error as occured during the injection process id:311");
        }
    }

    public override void Failure()
    {
        var powerInjectors = (from t in Pod.parent.GetComp<CompAffectedByFacilities>().LinkedFacilitiesListForReading
            where t.def == VFEEA_DefOf.VFEEA_PowerInjector
            select t).Cast<Building_PowerInjector>().ToList();
        var occupant = Pod.Occupant;
        Log.Message($"Failchance={FailChanceOnPawn(occupant)}");
        if (Rand.Chance(FailChanceOnPawn(occupant)))
        {
            var powerTracker = occupant.GetPowerTracker();
            Pod.EjectContents();
            if (occupant != null && powerTracker != null && selectedPowers != null)
            {
                var fail_Inject =
                    (Fail_Inject)Activator.CreateInstance(
                        typeof(Fail_Inject).AllSubclassesNonAbstract().RandomElement());
                if (!TryConsumeActiveSN(powerInjectors))
                {
                    Log.Message(
                        "[VFEEA - Extract Power] - The selected power was ejected before the operation could finish");
                }

                fail_Inject.RunOnPawn(occupant, selectedPowers);
            }
            else
            {
                Log.Message("[VFEEA - Extract Power] - An error as occured during the injection process id:312");
            }
        }
        else
        {
            if (occupant != null && selectedPowers != null)
            {
                InjectSucess();
                var middle_Fail_Inject =
                    (Middle_Fail_Inject)Activator.CreateInstance(typeof(Middle_Fail_Inject).AllSubclassesNonAbstract()
                        .RandomElement());
                middle_Fail_Inject.RunOnPawn(occupant, selectedPowers);
            }
            else
            {
                Log.Message("[VFEEA - Extract Power] - An error as occured during the injection process id:313");
            }
        }
    }

    private void InjectSucess()
    {
        var list = (from t in Pod.parent.GetComp<CompAffectedByFacilities>().LinkedFacilitiesListForReading
            where t.def == VFEEA_DefOf.VFEEA_PowerInjector
            select t).Cast<Building_PowerInjector>().ToList();
        var powerTracker = Pod.Occupant.GetPowerTracker();
        if (powerTracker != null && selectedPowers != null)
        {
            if (TryConsumeActiveSN(list))
            {
                powerTracker.AddPower(selectedPowers);
            }
            else
            {
                Messages.Message("VFEEA_ExtractPWR.InjectorMissing".Translate(), MessageTypeDefOf.CautionInput);
            }
        }

        Log.Message("[VFEEA - Extract Power] - An error as occured during the injection process id:314");
        Pod.EjectContents();
    }

    private bool TryConsumeActiveSN(List<Building_PowerInjector> powerInjectors)
    {
        foreach (var building_PowerInjector in powerInjectors)
        {
            if (building_PowerInjector.isEmpty())
            {
                continue;
            }

            if (building_PowerInjector.InjectorState != Enum_InjectorState.Injecting ||
                building_PowerInjector.ActiveSN.TryGetComp<ThingComp_powerDef>().VFEEA_powerDef !=
                selectedPowers.defName)
            {
                continue;
            }

            building_PowerInjector.ConsumeActiveSN();
            building_PowerInjector.InjectorState = Enum_InjectorState.Inactive;
            GenPlace.TryPlaceThing(ThingMaker.MakeThing(ThingDef.Named("VFEA_SuperNanites")),
                building_PowerInjector.Position, building_PowerInjector.Map, ThingPlaceMode.Near);
            return true;
        }

        return false;
    }

    private bool isAnySNInserted(List<Building_PowerInjector> injectorList)
    {
        foreach (var building_PowerInjector in injectorList)
        {
            if (!building_PowerInjector.isEmpty())
            {
                return true;
            }
        }

        return false;
    }

    private bool isAnyInactivate(List<Building_PowerInjector> injectorList)
    {
        foreach (var building_PowerInjector in injectorList)
        {
            if (building_PowerInjector.InjectorState == Enum_InjectorState.Inactive &&
                !building_PowerInjector.isEmpty())
            {
                return true;
            }
        }

        return false;
    }
}