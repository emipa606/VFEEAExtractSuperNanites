using System.Linq;
using Verse;
using VFEAncients;

namespace VFEE_Ancient_ExtractPower;

internal class Middle_Fail_Inject_Weaken : Middle_Fail_Inject
{
    protected override string Label => "VFEEA_ExtractPWR.Weaken".Translate();

    public override void RunOnPawn(Pawn pawn, PowerDef power)
    {
        var powerTracker = pawn.GetPowerTracker();
        if (powerTracker.AllPowers.Count() < 5)
        {
            DefDatabase<PowerDef>.AllDefs.Except(powerTracker.AllPowers)
                .Split(out _, out var source, def => def.powerType == 0);
            var powerDef = source.RandomElement();
            powerTracker.AddPower(powerDef);
            SendLetter("VFEEA_ExtractPWR.InjectMiddleFailed.Weaken".Translate(powerDef.label, power.label),
                pawn.Corpse);
        }
        else
        {
            pawn.Kill(null);
            SendLetter("VFEEA_ExtractPWR.InjectMiddleFailed.HighlyWeaken".Translate(power.label), pawn.Corpse);
        }
    }
}