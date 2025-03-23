using RimWorld;
using Verse;
using VFEAncients;

namespace VFEE_Ancient_ExtractPower;

public abstract class Fail_Inject
{
    public abstract string Label { get; }

    public abstract void RunOnPawn(Pawn pawn, PowerDef power);

    public virtual void SendLetter(string letterText, LookTargets targets = null)
    {
        Find.LetterStack.ReceiveLetter("VFEEA_ExtractPWR.ExperimentCriticalFailed".Translate() + ": " + Label,
            letterText, LetterDefOf.NegativeEvent, targets);
    }
}