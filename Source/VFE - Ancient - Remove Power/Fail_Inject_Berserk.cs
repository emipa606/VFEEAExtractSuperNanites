using Verse;
using VFEAncients;

namespace VFEE_Ancient_ExtractPower;

internal class Fail_Inject_Berserk : Fail_Inject
{
    public override string Label => "VFEAncients.Berserk".Translate();

    public override void RunOnPawn(Pawn pawn, PowerDef power)
    {
        VFEA_DefOf.Berserk.Worker.TryStart(pawn, "VFEAncients.ExperimentFailed".Translate(), false);
        SendLetter("VFEEA_ExtractPWR.InjectFailed.Berserk".Translate(power.label), pawn);
    }
}