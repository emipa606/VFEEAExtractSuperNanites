using RimWorld;
using Verse;
using Verse.AI;

namespace VFEE_Ancient_ExtractPower;

public class WorkGiver_ExtractSN : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(VFEEA_DefOf.VFEEA_PowerInjector);

    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Building_PowerInjector building_PowerInjector)
        {
            return false;
        }

        if (building_PowerInjector.activeSNToBeProcessed == null && building_PowerInjector.ActiveSN == null)
        {
            return false;
        }

        if (t.IsForbidden(pawn) || t.IsBurning())
        {
            return false;
        }

        LocalTargetInfo target = t;
        if (!pawn.CanReserve(target, 1, 1, null, forced))
        {
            return false;
        }

        if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
        {
            return false;
        }

        return building_PowerInjector.removeActiveSN && building_PowerInjector.ActiveSN != null &&
               building_PowerInjector.InjectorState == Enum_InjectorState.Inactive;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var t2 = t as Building_PowerInjector;
        return new Job(VFEEA_DefOf.VFEEA_ExtractActiveSN, t2);
    }
}