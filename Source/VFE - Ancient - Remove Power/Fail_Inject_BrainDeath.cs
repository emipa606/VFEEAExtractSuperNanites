using Verse;
using VFEAncients;

namespace VFEE_Ancient_ExtractPower;

internal class Fail_Inject_BrainDeath : Fail_Inject
{
    public override string Label => "VFEAncients.BrainDeath".Translate();

    public override void RunOnPawn(Pawn pawn, PowerDef power)
    {
        var brain = pawn.health.hediffSet.GetBrain();
        var hediff = HediffMaker.MakeHediff(VFEA_DefOf.ChemicalBurn, pawn, brain);
        hediff.Severity = pawn.health.hediffSet.GetPartHealth(brain) - 1f;
        hediff.TryGetComp<HediffComp_GetsPermanent>().IsPermanent = true;
        pawn.health.AddHediff(hediff, brain);
        SendLetter("VFEEA_ExtractPWR.InjectFailed.BrainDeath".Translate(power.label), pawn.Corpse);
    }
}