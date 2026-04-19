using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace BloodtypeHardcore
{
    public class CompBloodPackUsable : ThingComp
    {
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption option in base.CompFloatMenuOptions(selPawn))
            {
                yield return option;
            }

            if (selPawn == null ||
                parent.MapHeld == null ||
                parent.Destroyed ||
                parent.stackCount <= 0 ||
                !selPawn.IsColonistPlayerControlled ||
                selPawn.MapHeld != parent.MapHeld)
            {
                yield break;
            }

            if (selPawn.WorkTagIsDisabled(WorkTags.Caring) || !selPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                yield break;
            }

            if (!BloodPackUtility.TryGetBloodTypeFromThingDef(parent.def, out _))
            {
                yield return new FloatMenuOption("BloodtypeHardcore.BloodPack.Use.InvalidType".Translate(), null);
                yield break;
            }

            if (!selPawn.CanReserveAndReach(parent, PathEndMode.ClosestTouch, Danger.Deadly))
            {
                yield return new FloatMenuOption("BloodtypeHardcore.BloodPack.Use.NoPath".Translate(), null);
                yield break;
            }

            yield return new FloatMenuOption(
                "BloodtypeHardcore.BloodPack.Use.Menu".Translate(),
                () => BeginTargeting(selPawn));
        }

        private void BeginTargeting(Pawn doctor)
        {
            TargetingParameters parameters = new TargetingParameters
            {
                canTargetPawns = true,
                canTargetAnimals = true,
                canTargetHumans = true,
                mapObjectTargetsMustBeAutoAttackable = false,
                validator = target => target.Thing is Pawn pawn && !pawn.Dead && pawn.MapHeld == doctor.MapHeld
            };

            Find.Targeter.BeginTargeting(parameters, target =>
            {
                Pawn recipient = target.Thing as Pawn;
                if (recipient == null)
                {
                    return;
                }

                if (recipient.health?.hediffSet?.GetFirstHediffOfDef(BloodtypeHediffDefOf.Bloodtype_TransfusionCrisis) != null)
                {
                    Messages.Message("BloodtypeHardcore.BloodPack.Use.CrisisBlocked".Translate(recipient.Named("PAWN")), recipient, MessageTypeDefOf.RejectInput, false);
                    return;
                }

                if (!doctor.CanReserveAndReach(parent, PathEndMode.ClosestTouch, Danger.Deadly))
                {
                    Messages.Message("BloodtypeHardcore.BloodPack.Use.NoPath".Translate(), parent, MessageTypeDefOf.RejectInput, false);
                    return;
                }

                if (!doctor.CanReserveAndReach(recipient, PathEndMode.Touch, Danger.Deadly))
                {
                    Messages.Message("BloodtypeHardcore.BloodPack.Use.CannotReachTarget".Translate(recipient.Named("PAWN")), recipient, MessageTypeDefOf.RejectInput, false);
                    return;
                }

                Job job = JobMaker.MakeJob(BloodtypeJobDefOf.Bloodtype_AdministerBloodPack, parent, recipient);
                job.count = 1;
                job.playerForced = true;
                doctor.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            });
        }
    }
}
