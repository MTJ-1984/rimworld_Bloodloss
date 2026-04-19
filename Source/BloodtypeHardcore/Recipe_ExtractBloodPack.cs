using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BloodtypeHardcore
{
    public class Recipe_ExtractBloodPack : Recipe_Surgery
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (BloodTypeService.GetBloodFamily(pawn) == BloodFamily.NoBlood)
            {
                Messages.Message("BloodtypeHardcore.General.NoBlood".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
                return;
            }

            if (!BloodTypeService.CanUseHumanTypedBlood(pawn))
            {
                Messages.Message("BloodtypeHardcore.General.UnsupportedBloodFamily".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
                return;
            }

            if (!CanExtractFrom(pawn))
            {
                Messages.Message("BloodtypeHardcore.Extract.NotEnoughBlood".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
                return;
            }

            BloodType bloodType = BloodTypeService.GetOrAssign(pawn);
            ThingDef bloodPackDef = DefForType(bloodType);
            if (bloodPackDef == null)
            {
                Log.Error($"[BloodtypeHardcore] Missing blood pack def for blood type {bloodType}.");
                return;
            }

            HealthUtility.AdjustSeverity(pawn, HediffDefOf.BloodLoss, GetExtractBloodLossAdded());

            Thing bloodPack = ThingMaker.MakeThing(bloodPackDef);
            GenPlace.TryPlaceThing(bloodPack, pawn.PositionHeld, pawn.MapHeld, ThingPlaceMode.Near);

            Messages.Message(
                "BloodtypeHardcore.Extract.Success".Translate(
                    BloodTypeCompatibility.ToLabel(bloodType),
                    pawn.Named("PAWN")),
                pawn,
                MessageTypeDefOf.PositiveEvent,
                false);
        }

        private static bool CanExtractFrom(Pawn pawn)
        {
            Hediff bloodLoss = pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
            float currentSeverity = bloodLoss?.Severity ?? 0f;
            return currentSeverity <= GetExtractMaxAllowedBloodLoss();
        }

        private static float GetExtractBloodLossAdded()
        {
            return BloodtypeMod.Settings?.extractBloodLossAdded ?? 0.20f;
        }

        private static float GetExtractMaxAllowedBloodLoss()
        {
            return BloodtypeMod.Settings?.extractMaxAllowedBloodLoss ?? 0.45f;
        }

        private static ThingDef DefForType(BloodType bloodType)
        {
            switch (bloodType)
            {
                case BloodType.OPositive:
                    return BloodtypeThingDefOf.BloodPack_O_Positive;
                case BloodType.ONegative:
                    return BloodtypeThingDefOf.BloodPack_O_Negative;
                case BloodType.APositive:
                    return BloodtypeThingDefOf.BloodPack_A_Positive;
                case BloodType.ANegative:
                    return BloodtypeThingDefOf.BloodPack_A_Negative;
                case BloodType.BPositive:
                    return BloodtypeThingDefOf.BloodPack_B_Positive;
                case BloodType.BNegative:
                    return BloodtypeThingDefOf.BloodPack_B_Negative;
                case BloodType.ABPositive:
                    return BloodtypeThingDefOf.BloodPack_AB_Positive;
                case BloodType.ABNegative:
                    return BloodtypeThingDefOf.BloodPack_AB_Negative;
                default:
                    return null;
            }
        }
    }
}
