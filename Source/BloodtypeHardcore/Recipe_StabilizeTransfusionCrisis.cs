using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace BloodtypeHardcore
{
    public class Recipe_StabilizeTransfusionCrisis : Recipe_RemoveHediff
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            Hediff crisis = pawn.health?.hediffSet?.GetFirstHediffOfDef(BloodtypeHediffDefOf.Bloodtype_TransfusionCrisis);
            if (crisis == null)
            {
                Messages.Message("BloodtypeHardcore.Transfusion.UltraBrutal.NoActiveCrisis".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
                return;
            }

            bool hasMedicine = ingredients != null && ingredients.Exists(t =>
                t?.def == ThingDefOf.MedicineHerbal ||
                t?.def == ThingDefOf.MedicineIndustrial ||
                t?.def == ThingDefOf.MedicineUltratech);
            if (!hasMedicine)
            {
                Messages.Message("BloodtypeHardcore.Transfusion.UltraBrutal.RequiresMedicine".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
                return;
            }

            float rescueChance = CalculateRescueChance(pawn, billDoer, ingredients, bill?.recipe);
            if (Rand.Chance(rescueChance))
            {
                crisis.Severity -= GetSuccessSeverityDrop();
                if (crisis.Severity < 0.25f)
                {
                    pawn.health.RemoveHediff(crisis);
                    ApplyPostCrisisInjury(pawn);
                    ApplyPostCrisisBloodLoss(pawn);
                    Messages.Message(
                        "BloodtypeHardcore.Transfusion.UltraBrutal.Rescued".Translate(
                            (rescueChance * 100f).ToString("0"),
                            pawn.Named("PAWN")),
                        pawn,
                        MessageTypeDefOf.PositiveEvent,
                        false);
                    return;
                }

                Messages.Message(
                    "BloodtypeHardcore.Transfusion.UltraBrutal.PartialStabilization".Translate(
                        (rescueChance * 100f).ToString("0"),
                        pawn.Named("PAWN")),
                    pawn,
                    MessageTypeDefOf.CautionInput,
                    false);
                QueueRetryBill(pawn, bill);
                return;
            }

            crisis.Severity += GetFailureSeverityGain();
            Messages.Message(
                "BloodtypeHardcore.Transfusion.UltraBrutal.FailedRescue".Translate(
                    (rescueChance * 100f).ToString("0"),
                    pawn.Named("PAWN")),
                pawn,
                MessageTypeDefOf.CautionInput,
                false);

            QueueRetryBill(pawn, bill);
        }

        private static void QueueRetryBill(Pawn pawn, Bill sourceBill)
        {
            if (pawn?.BillStack == null || sourceBill == null || sourceBill.recipe == null)
            {
                return;
            }

            RecipeDef recipe = sourceBill.recipe;
            bool hasPendingSameRecipe = false;
            List<Bill> existingBills = pawn.BillStack.Bills;
            for (int i = 0; i < existingBills.Count; i++)
            {
                Bill existing = existingBills[i];
                if (existing == null || existing.deleted || existing == sourceBill)
                {
                    continue;
                }

                if (existing.recipe == recipe)
                {
                    hasPendingSameRecipe = true;
                    break;
                }
            }

            if (hasPendingSameRecipe)
            {
                return;
            }

            Bill_Medical retryBill = new Bill_Medical(recipe, new List<Thing>());
            Bill_Medical sourceMedical = sourceBill as Bill_Medical;
            if (sourceMedical != null)
            {
                retryBill.Part = sourceMedical.Part;
            }
            retryBill.suspended = false;
            pawn.BillStack.AddBill(retryBill);
        }

        private static float CalculateRescueChance(Pawn patient, Pawn surgeon, List<Thing> ingredients, RecipeDef recipe)
        {
            if (surgeon == null)
            {
                return 0f;
            }

            float surgeonChance = surgeon.GetStatValue(StatDefOf.MedicalSurgerySuccessChance);
            float bedFactor = GetBedSurgeryFactor(patient);
            float medicinePotency = GetMedicinePotency(ingredients);
            float operationFactor = recipe?.surgerySuccessChanceFactor ?? 1f;
            float raw = surgeonChance * bedFactor * medicinePotency * operationFactor;
            float operationSuccessChance = Mathf.Min(raw, 0.98f);
            float scale = BloodtypeMod.Settings?.ultraBrutalRescueScale ?? 0.41f;
            float resilienceBonus = GetResilienceBonus(patient);
            return Mathf.Clamp01((operationSuccessChance * scale) + resilienceBonus);
        }

        private static float GetBedSurgeryFactor(Pawn patient)
        {
            Building_Bed bed = patient.CurrentBed();
            if (bed == null)
            {
                return 0.6f;
            }

            return bed.GetStatValue(StatDefOf.SurgerySuccessChanceFactor);
        }

        private static float GetMedicinePotency(List<Thing> ingredients)
        {
            if (ingredients == null || ingredients.Count == 0)
            {
                return 0f;
            }

            float best = 0f;
            for (int i = 0; i < ingredients.Count; i++)
            {
                Thing ingredient = ingredients[i];
                if (ingredient == null)
                {
                    continue;
                }

                if (ingredient.def != ThingDefOf.MedicineHerbal &&
                    ingredient.def != ThingDefOf.MedicineIndustrial &&
                    ingredient.def != ThingDefOf.MedicineUltratech)
                {
                    continue;
                }

                float potency = ingredient.GetStatValue(StatDefOf.MedicalPotency, true);
                if (potency > best)
                {
                    best = potency;
                }
            }

            return best;
        }

        private static float GetResilienceBonus(Pawn patient)
        {
            if (patient?.health?.capacities == null)
            {
                return 0f;
            }

            float filtration = patient.health.capacities.GetLevel(PawnCapacityDefOf.BloodFiltration);
            float pumping = patient.health.capacities.GetLevel(PawnCapacityDefOf.BloodPumping);
            float immunityGain = patient.GetStatValue(StatDefOf.ImmunityGainSpeed);

            float filtrationBonus = Mathf.Clamp((filtration - 1f) * 0.12f, -0.08f, 0.12f);
            float pumpingBonus = Mathf.Clamp((pumping - 1f) * 0.10f, -0.08f, 0.10f);
            float immunityBonus = Mathf.Clamp((immunityGain - 1f) * 0.10f, -0.05f, 0.08f);
            return filtrationBonus + pumpingBonus + immunityBonus;
        }

        private static float GetSuccessSeverityDrop()
        {
            return BloodtypeMod.Settings?.stabilizeSuccessSeverityDrop ?? 0.22f;
        }

        private static float GetFailureSeverityGain()
        {
            return BloodtypeMod.Settings?.stabilizeFailureSeverityGain ?? 0.09f;
        }

        private static float GetPostCrisisBloodLoss()
        {
            return BloodtypeMod.Settings?.postCrisisBloodLoss ?? 0.55f;
        }

        private static void ApplyPostCrisisBloodLoss(Pawn pawn)
        {
            float amount = GetPostCrisisBloodLoss();
            if (amount <= 0f)
            {
                return;
            }

            HealthUtility.AdjustSeverity(pawn, HediffDefOf.BloodLoss, amount);
        }

        private static void ApplyPostCrisisInjury(Pawn pawn)
        {
            Hediff injury = pawn.health.hediffSet.GetFirstHediffOfDef(BloodtypeHediffDefOf.Bloodtype_PostCrisisInjury);
            if (injury == null)
            {
                injury = HediffMaker.MakeHediff(BloodtypeHediffDefOf.Bloodtype_PostCrisisInjury, pawn);
                pawn.health.AddHediff(injury);
            }

            injury.Severity = 1f;
        }
    }
}
