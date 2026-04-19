using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace BloodtypeHardcore
{
    public class JobDriver_AdministerBloodPack : JobDriver
    {
        private const TargetIndex BloodPackInd = TargetIndex.A;
        private const TargetIndex RecipientInd = TargetIndex.B;
        private const int InjectionTicks = 150;

        private Thing BloodPack => job.GetTarget(BloodPackInd).Thing;
        private Pawn Recipient => job.GetTarget(RecipientInd).Pawn;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(BloodPack, job, 1, -1, null, errorOnFailed) &&
                   pawn.Reserve(Recipient, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedNullOrForbidden(BloodPackInd);
            this.FailOnDestroyedNullOrForbidden(RecipientInd);
            this.FailOn(() => Recipient == null || Recipient.Dead);

            yield return Toils_Goto.GotoThing(BloodPackInd, PathEndMode.ClosestTouch);

            Toil pickUpPack = ToilMaker.MakeToil("PickUpBloodPack");
            pickUpPack.initAction = () =>
            {
                Thing pack = BloodPack;
                if (pack == null || pack.Destroyed)
                {
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                if (pawn.carryTracker.TryStartCarry(pack, 1, false) <= 0)
                {
                    EndJobWith(JobCondition.Incompletable);
                }
            };
            pickUpPack.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return pickUpPack;

            yield return Toils_Goto.GotoThing(RecipientInd, PathEndMode.Touch);

            Toil wait = Toils_General.Wait(InjectionTicks, RecipientInd);
            wait.WithProgressBarToilDelay(RecipientInd);
            wait.FailOnCannotTouch(RecipientInd, PathEndMode.Touch);
            yield return wait;

            Toil apply = ToilMaker.MakeToil("ApplyBloodPack");
            apply.initAction = () =>
            {
                Thing carriedPack = pawn.carryTracker.CarriedThing;
                if (carriedPack == null || carriedPack.Destroyed)
                {
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                if (!BloodPackUtility.TryGetBloodTypeFromThingDef(carriedPack.def, out BloodType donorType))
                {
                    Messages.Message("BloodtypeHardcore.BloodPack.Use.InvalidType".Translate(), MessageTypeDefOf.RejectInput, false);
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                Pawn recipient = Recipient;
                if (recipient == null || recipient.Dead)
                {
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                if (!Recipe_TransfuseBloodPack.CanTransfuseInto(recipient))
                {
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                Recipe_TransfuseBloodPack.ApplyDonorBloodType(recipient, donorType);
                pawn.carryTracker.innerContainer.Remove(carriedPack);
                if (!carriedPack.Destroyed)
                {
                    carriedPack.Destroy();
                }
            };
            apply.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return apply;
        }
    }
}
