using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using VFEAncients;

namespace VFEE_Ancient_ExtractPower;

public class Operation_Extract(CompGeneTailoringPod pod) : Operation(pod), IExposable
{
    private readonly float failChance =
        LoadedModManager.GetMod<VFEEA_Mod>().GetSettings<VFEEA_ModSettings>().extractFailChance;

    public PowerDef selectedPowers;

    public override string Label => "VFEEA_ExtractPWR.Extract".Translate();

    public new void ExposeData()
    {
        Scribe_Defs.Look(ref selectedPowers, "VFEEA_selectedPowers");
    }

    public override float FailChanceOnPawn(Pawn pawn)
    {
        return base.FailChanceOnPawn(pawn) + failChance;
    }

    public override int StartOnPawnGetDuration()
    {
        var powerTracker = Pod.Occupant.GetPowerTracker();
        var powerDef = powerTracker.AllPowers.LastOrDefault(power => power.powerType == 0);
        DefDatabase<PowerDef>.AllDefs.Intersect(powerTracker.AllPowers)
            .Split(out var list, out var source, def => def.powerType == 0);

        if (powerDef == null)
        {
            return TicksRequired;
        }

        var list2 = new List<Tuple<PowerDef, PowerDef>>();
        foreach (var item in list)
        {
            list2.Add(new Tuple<PowerDef, PowerDef>(item, source.RandomElement()));
        }

        Find.WindowStack.Add(new Dialog_ExtractPowers(list2, Pod.Occupant, OnChosen));

        return TicksRequired;

        void OnChosen(Tuple<PowerDef, PowerDef> powers)
        {
            selectedPowers = powers.Item1;
        }
    }

    public override bool CanRunOnPawn(Pawn pawn)
    {
        var hasAttachedRetractor = false;
        if (base.CanRunOnPawn(pawn))
        {
            hasAttachedRetractor = Pod.parent.GetComp<CompAffectedByFacilities>().LinkedFacilitiesListForReading
                .Any(t => t.def == VFEA_DefOf.VFEA_NanotechRetractor);
        }

        return hasAttachedRetractor && pawn.GetPowerTracker().AllPowers.Any(power => power.powerType == 0);
    }

    public override string FailChanceExplainOnPawn(Pawn pawn)
    {
        return string.Concat(base.FailChanceExplainOnPawn(pawn), "\n", Label, ": +",
            ((int)(failChance * 100f)).ToString(), "%");
    }

    public override void Success()
    {
        var powerTracker = Pod.Occupant.GetPowerTracker();
        if (powerTracker != null)
        {
            ExtractSuccess();
            Find.LetterStack.ReceiveLetter("VFEEA_ExtractPWR.Extract.Label".Translate(powerTracker.Pawn.LabelShortCap),
                "VFEEA_ExtractPWR.Extract.Text".Translate(powerTracker.Pawn.NameShortColored, selectedPowers.LabelCap),
                LetterDefOf.PositiveEvent, powerTracker.Pawn);
        }
        else
        {
            Log.Message("[VFEEA - Extract Power] - An error as occured during the extraction process id:411");
        }
    }

    public override void Failure()
    {
        var occupant = Pod.Occupant;
        if (Rand.Chance(FailChanceOnPawn(occupant)))
        {
            var powerTracker = occupant.GetPowerTracker();
            Pod.EjectContents();
            if (occupant != null && powerTracker != null && selectedPowers != null)
            {
                powerTracker.RemovePower(selectedPowers);
                var fail_Extract =
                    (Fail_Extract)Activator.CreateInstance(typeof(Fail_Extract).AllSubclassesNonAbstract()
                        .RandomElement());
                fail_Extract.RunOnPawn(occupant, selectedPowers);
            }
            else
            {
                Log.Message("[VFEEA - Extract Power] - An error as occured during the extraction process id:412");
            }
        }
        else
        {
            if (occupant != null && selectedPowers != null)
            {
                ExtractSuccess();
                var middle_Fail_Extract =
                    (Middle_Fail_Extract)Activator.CreateInstance(typeof(Middle_Fail_Extract).AllSubclassesNonAbstract()
                        .RandomElement());
                middle_Fail_Extract.RunOnPawn(occupant, selectedPowers);
            }
            else
            {
                Log.Message("[VFEEA - Extract Power] - An error as occured during the extraction process id:413");
            }
        }
    }

    private void ExtractSuccess()
    {
        var powerTracker = Pod.Occupant.GetPowerTracker();
        if (powerTracker != null && selectedPowers != null)
        {
            powerTracker.RemovePower(selectedPowers);
            var thing = ThingMaker.MakeThing(ThingDef.Named(
                $"VFEEA_Empowered_SuperNanites_{selectedPowers.defName}"));
            GenPlace.TryPlaceThing(thing, Pod.parent.Position, Pod.parent.Map, ThingPlaceMode.Near);
        }
        else
        {
            Log.Message("[VFEEA - Extract Power] - An error as occured during the extraction process id:414");
        }

        Pod.EjectContents();
    }
}