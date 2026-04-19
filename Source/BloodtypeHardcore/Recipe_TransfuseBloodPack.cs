using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BloodtypeHardcore
{
    public class Recipe_TransfuseBloodPack : Recipe_Surgery
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            Hediff activeCrisis = pawn.health?.hediffSet?.GetFirstHediffOfDef(BloodtypeHediffDefOf.Bloodtype_TransfusionCrisis);
            if (activeCrisis != null)
            {
                Messages.Message("BloodtypeHardcore.Transfusion.CrisisNeedsStabilization".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
                return;
            }

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

            if (!TryGetDonorType(ingredients, out BloodType donorType))
            {
                Messages.Message("BloodtypeHardcore.Transfusion.MissingPack".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
                return;
            }

            ApplyDonorBloodType(pawn, donorType);
        }

        public static bool CanTransfuseInto(Pawn pawn)
        {
            if (pawn.health?.hediffSet?.GetFirstHediffOfDef(BloodtypeHediffDefOf.Bloodtype_TransfusionCrisis) != null)
            {
                Messages.Message("BloodtypeHardcore.Transfusion.CrisisNeedsStabilization".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
                return false;
            }

            if (BloodTypeService.GetBloodFamily(pawn) == BloodFamily.NoBlood)
            {
                Messages.Message("BloodtypeHardcore.General.NoBlood".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
                return false;
            }

            if (!BloodTypeService.CanUseHumanTypedBlood(pawn))
            {
                Messages.Message("BloodtypeHardcore.General.UnsupportedBloodFamily".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
                return false;
            }

            return true;
        }

        public static void ApplyDonorBloodType(Pawn pawn, BloodType donorType)
        {
            if (!CanTransfuseInto(pawn))
            {
                return;
            }

            BloodType recipientType = BloodTypeService.GetOrAssign(pawn);
            bool exactMatch = BloodTypeCompatibility.IsExactMatch(donorType, recipientType);
            bool compatible = BloodTypeCompatibility.IsCompatible(donorType, recipientType);
            bool allowCompatible = BloodtypeMod.Settings == null || BloodtypeMod.Settings.allowCompatibleNonIdenticalTransfusions;
            float exactRecovery = GetExactMatchRecovery();
            float compatibleRecovery = GetCompatibleRecovery();
            float incompatibleRecovery = GetIncompatibleRecovery();
            float compatibleReaction = GetCompatibleReactionSeverity();
            float incompatibleReaction = GetIncompatibleReactionSeverity();

            if (exactMatch)
            {
                RecoverBloodLoss(pawn, -exactRecovery);
                RestoreHemogen(pawn, 0.20f);
                Messages.Message(
                    "BloodtypeHardcore.Transfusion.ExactMatch".Translate(
                        BloodTypeCompatibility.ToLabel(donorType),
                        pawn.Named("PAWN")),
                    pawn,
                    MessageTypeDefOf.PositiveEvent,
                    false);
                return;
            }

            if (BloodtypeMod.Settings?.ultraBrutalCriticalMode == true)
            {
                AddReaction(pawn, incompatibleReaction);
                ApplyTransfusionCrisis(pawn);
                Messages.Message(
                    "BloodtypeHardcore.Transfusion.UltraBrutal.CrisisStarted".Translate(
                        BloodTypeCompatibility.ToLabel(donorType),
                        BloodTypeCompatibility.ToLabel(recipientType),
                        pawn.Named("PAWN")),
                    pawn,
                    MessageTypeDefOf.NegativeHealthEvent,
                    false);
                return;
            }

            if (compatible && allowCompatible)
            {
                RecoverBloodLoss(pawn, -compatibleRecovery);
                AddReaction(pawn, compatibleReaction);
                RestoreHemogen(pawn, 0.12f);
                Messages.Message(
                    "BloodtypeHardcore.Transfusion.Compatible".Translate(
                        BloodTypeCompatibility.ToLabel(donorType),
                        BloodTypeCompatibility.ToLabel(recipientType),
                        pawn.Named("PAWN")),
                    pawn,
                    MessageTypeDefOf.CautionInput,
                    false);
                return;
            }

            RecoverBloodLoss(pawn, -incompatibleRecovery);
            AddReaction(pawn, incompatibleReaction);
            RestoreHemogen(pawn, 0.03f);
            Messages.Message(
                "BloodtypeHardcore.Transfusion.Incompatible".Translate(
                    BloodTypeCompatibility.ToLabel(donorType),
                    BloodTypeCompatibility.ToLabel(recipientType),
                    pawn.Named("PAWN")),
                pawn,
                MessageTypeDefOf.NegativeHealthEvent,
                false);
        }

        private static void RecoverBloodLoss(Pawn pawn, float amount)
        {
            HealthUtility.AdjustSeverity(pawn, HediffDefOf.BloodLoss, amount);
        }

        private static void AddReaction(Pawn pawn, float baseSeverity)
        {
            float multiplier = BloodtypeMod.Settings?.reactionSeverityMultiplier ?? 1f;
            float severityToAdd = baseSeverity * multiplier;
            if (severityToAdd <= 0f)
            {
                return;
            }

            Hediff reaction = pawn.health.hediffSet.GetFirstHediffOfDef(BloodtypeHediffDefOf.Bloodtype_TransfusionReaction);
            if (reaction == null)
            {
                reaction = HediffMaker.MakeHediff(BloodtypeHediffDefOf.Bloodtype_TransfusionReaction, pawn);
                pawn.health.AddHediff(reaction);
            }

            reaction.Severity += severityToAdd;
        }

        private static void ApplyTransfusionCrisis(Pawn pawn)
        {
            Hediff crisis = pawn.health.hediffSet.GetFirstHediffOfDef(BloodtypeHediffDefOf.Bloodtype_TransfusionCrisis);
            if (crisis == null)
            {
                crisis = HediffMaker.MakeHediff(BloodtypeHediffDefOf.Bloodtype_TransfusionCrisis, pawn);
                pawn.health.AddHediff(crisis);
            }

            crisis.Severity = 0.001f;
        }

        private static bool TryGetDonorType(List<Thing> ingredients, out BloodType donorType)
        {
            donorType = default;
            if (ingredients == null)
            {
                return false;
            }

            for (int i = 0; i < ingredients.Count; i++)
            {
                ThingDef def = ingredients[i]?.def;
                if (def == null)
                {
                    continue;
                }

                if (BloodPackUtility.TryGetBloodTypeFromThingDef(def, out donorType))
                {
                    return true;
                }
            }

            return false;
        }

        private static float GetExactMatchRecovery()
        {
            return BloodtypeMod.Settings?.exactMatchBloodLossRecovery ?? 0.25f;
        }

        private static float GetCompatibleRecovery()
        {
            return BloodtypeMod.Settings?.compatibleBloodLossRecovery ?? 0.15f;
        }

        private static float GetIncompatibleRecovery()
        {
            return BloodtypeMod.Settings?.incompatibleBloodLossRecovery ?? 0.03f;
        }

        private static float GetCompatibleReactionSeverity()
        {
            return BloodtypeMod.Settings?.compatibleReactionSeverity ?? 0.12f;
        }

        private static float GetIncompatibleReactionSeverity()
        {
            return BloodtypeMod.Settings?.incompatibleReactionSeverity ?? 0.45f;
        }

        private static void RestoreHemogen(Pawn pawn, float offset)
        {
            if (offset <= 0f || pawn?.genes == null)
            {
                return;
            }

            GeneUtility.OffsetHemogen(pawn, offset, true);
        }
    }
}
