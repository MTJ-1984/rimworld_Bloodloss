using RimWorld;
using Verse;

namespace BloodtypeHardcore
{
    [DefOf]
    public static class BloodtypeHediffDefOf
    {
        public static HediffDef Bloodtype_TransfusionReaction;
        public static HediffDef Bloodtype_TransfusionCrisis;
        public static HediffDef Bloodtype_ClinicalComa;
        public static HediffDef Bloodtype_PostCrisisInjury;

        static BloodtypeHediffDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BloodtypeHediffDefOf));
        }
    }
}
