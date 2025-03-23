using Verse;
using VFEAncients;

namespace VFEE_Ancient_ExtractPower;

internal class Fail_Extract_Death : Fail_Extract
{
    public override string Label => "VFEAncients.Death".Translate();

    public override void RunOnPawn(Pawn pawn, PowerDef power)
    {
        pawn.Kill(null);
        SendLetter("VFEEA_ExtractPWR.ExperimentFailed.Death".Translate(pawn.LabelShortCap, power.label), pawn.Corpse);
    }
}