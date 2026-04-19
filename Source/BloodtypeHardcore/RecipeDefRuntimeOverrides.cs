using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BloodtypeHardcore
{
    [StaticConstructorOnStartup]
    public static class RecipeDefRuntimeOverrides
    {
        static RecipeDefRuntimeOverrides()
        {
            RecipeDef extract = DefDatabase<RecipeDef>.GetNamedSilentFail("ExtractHemogenPack");
            if (extract != null)
            {
                extract.workerClass = typeof(Recipe_ExtractBloodPack);
                ConfigureExtractionIngredients(extract);
                extract.dontShowIfAnyIngredientMissing = false;
            }

            RecipeDef transfusion = DefDatabase<RecipeDef>.GetNamedSilentFail("BloodTransfusion");
            if (transfusion != null)
            {
                transfusion.workerClass = typeof(Recipe_TransfuseBloodPack);
                ConfigureTransfusionIngredients(transfusion);
                transfusion.dontShowIfAnyIngredientMissing = false;
                LogTransfusionRecipeState(transfusion);
            }

            RecipeDef stabilize = DefDatabase<RecipeDef>.GetNamedSilentFail("StabilizeTransfusionCrisis");
            if (stabilize != null)
            {
                stabilize.dontShowIfAnyIngredientMissing = false;
                EnsureStabilizeRecipeUsers(stabilize);
            }
        }

        private static void EnsureStabilizeRecipeUsers(RecipeDef stabilize)
        {
            if (stabilize == null)
            {
                return;
            }

            int added = 0;
            List<ThingDef> allDefs = DefDatabase<ThingDef>.AllDefsListForReading;
            for (int i = 0; i < allDefs.Count; i++)
            {
                ThingDef raceDef = allDefs[i];
                if (raceDef?.race == null || raceDef.recipes == null || raceDef.recipes.Count == 0)
                {
                    continue;
                }

                bool hasBloodTransfusion = false;
                for (int j = 0; j < raceDef.recipes.Count; j++)
                {
                    RecipeDef existing = raceDef.recipes[j];
                    if (existing?.defName == "BloodTransfusion")
                    {
                        hasBloodTransfusion = true;
                        break;
                    }
                }

                if (!hasBloodTransfusion || raceDef.recipes.Contains(stabilize))
                {
                    continue;
                }

                raceDef.recipes.Add(stabilize);
                added++;
            }

            if (added > 0)
            {
                Log.Message("[BloodtypeHardcore] Added StabilizeTransfusionCrisis to " + added + " race recipe list(s).");
            }
        }

        private static void ConfigureExtractionIngredients(RecipeDef extract)
        {
            extract.ingredients = new List<IngredientCount>();
            extract.fixedIngredientFilter = new ThingFilter();
            extract.defaultIngredientFilter = new ThingFilter();
        }

        private static void ConfigureTransfusionIngredients(RecipeDef transfusion)
        {
            IngredientCount bloodPackIngredient = new IngredientCount();
            bloodPackIngredient.SetBaseCount(1f);
            ThingFilter bloodFilter = BuildBloodPackFilter();
            bloodPackIngredient.filter.CopyAllowancesFrom(bloodFilter);

            transfusion.ingredients = new List<IngredientCount> { bloodPackIngredient };

            ThingFilter fixedFilter = new ThingFilter();
            fixedFilter.CopyAllowancesFrom(bloodFilter);
            transfusion.fixedIngredientFilter = fixedFilter;

            ThingFilter defaultFilter = new ThingFilter();
            defaultFilter.CopyAllowancesFrom(bloodFilter);
            transfusion.defaultIngredientFilter = defaultFilter;
        }

        private static ThingFilter BuildBloodPackFilter()
        {
            ThingFilter filter = new ThingFilter();
            List<ThingDef> defs = new List<ThingDef>
            {
                DefDatabase<ThingDef>.GetNamedSilentFail("BloodPack_O_Positive"),
                DefDatabase<ThingDef>.GetNamedSilentFail("BloodPack_O_Negative"),
                DefDatabase<ThingDef>.GetNamedSilentFail("BloodPack_A_Positive"),
                DefDatabase<ThingDef>.GetNamedSilentFail("BloodPack_A_Negative"),
                DefDatabase<ThingDef>.GetNamedSilentFail("BloodPack_B_Positive"),
                DefDatabase<ThingDef>.GetNamedSilentFail("BloodPack_B_Negative"),
                DefDatabase<ThingDef>.GetNamedSilentFail("BloodPack_AB_Positive"),
                DefDatabase<ThingDef>.GetNamedSilentFail("BloodPack_AB_Negative")
            };

            int added = 0;
            for (int i = 0; i < defs.Count; i++)
            {
                ThingDef def = defs[i];
                if (def == null)
                {
                    continue;
                }

                filter.SetAllow(def, true);
                added++;
            }

            if (added == 0)
            {
                Log.Warning("[BloodtypeHardcore] No typed blood pack defs found while building transfusion filter.");
            }

            return filter;
        }

        private static void LogTransfusionRecipeState(RecipeDef transfusion)
        {
            ThingFilter ingredientFilter = transfusion.ingredients != null && transfusion.ingredients.Count > 0
                ? transfusion.ingredients[0].filter
                : null;
            if (ingredientFilter == null)
            {
                Log.Warning("[BloodtypeHardcore] BloodTransfusion override: ingredient filter is null.");
                return;
            }

            List<string> allowedDefs = DefDatabase<ThingDef>.AllDefsListForReading
                .Where(def => ingredientFilter.Allows(def))
                .Select(def => def.defName)
                .OrderBy(name => name)
                .ToList();

            Log.Message("[BloodtypeHardcore] BloodTransfusion override active. Allowed first-ingredient defs: " + string.Join(", ", allowedDefs));
            Log.Message("[BloodtypeHardcore] BloodTransfusion ingredient-count=" + (transfusion.ingredients?.Count ?? 0)
                + ", fixedFilterAllowsTypedO-=" + transfusion.fixedIngredientFilter?.Allows(DefDatabase<ThingDef>.GetNamedSilentFail("BloodPack_O_Negative"))
                + ", defaultFilterAllowsTypedO-=" + transfusion.defaultIngredientFilter?.Allows(DefDatabase<ThingDef>.GetNamedSilentFail("BloodPack_O_Negative")));
        }
    }
}
