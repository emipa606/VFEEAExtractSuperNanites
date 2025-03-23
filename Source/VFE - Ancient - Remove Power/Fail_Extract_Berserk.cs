using Verse;
using VFEAncients;

namespace VFEE_Ancient_ExtractPower;

internal class Fail_Extract_Berserk : Fail_Extract
{
    public override string Label => "VFEAncients.Berserk".Translate();

    public override void RunOnPawn(Pawn pawn, PowerDef power)
    {
        VFEA_DefOf.Berserk.Worker.TryStart(pawn, "VFEAncients.ExperimentFailed".Translate(), false);
        SendLetter("VFEEA_ExtractPWR.ExperimentFailed.Berserk".Translate(pawn.LabelShortCap, power.label), pawn);
    }
}