using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace VFEE_Ancient_ExtractPower;

public class WorkGiver_InsertSN : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(VFEEA_DefOf.VFEEA_PowerInjector);

    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Building_PowerInjector building_PowerInjector ||
            building_PowerInjector.InjectorState == Enum_InjectorState.Injecting)
        {
            return false;
        }

        if (building_PowerInjector.activeSNToBeProcessed == null || t.IsForbidden(pawn) || t.IsBurning())
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

        if (FindSN(pawn, building_PowerInjector.activeSNToBeProcessed) != null)
        {
            return true;
        }

        JobFailReason.Is("VFEEA_ExtractPWR.NoSNFound".Translate());
        return false;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var building_PowerInjector = t as Building_PowerInjector;
        var t2 = FindSN(pawn, building_PowerInjector?.activeSNToBeProcessed);
        return new Job(VFEEA_DefOf.VFEEA_InsertSN, building_PowerInjector, t2);
    }

    private Thing FindSN(Pawn pawn, ThingDef SN)
    {
        var position = pawn.Position;
        var map = pawn.Map;
        var thingReq = ThingRequest.ForDef(SN);
        var peMode = PathEndMode.ClosestTouch;
        var traverseParams = TraverseParms.For(pawn);
        var validator = (Predicate<Thing>)Predicate;
        return GenClosest.ClosestThingReachable(position, map, thingReq, peMode, traverseParams, 9999f, validator);

        bool Predicate(Thing x)
        {
            return !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, 1);
        }
    }
}