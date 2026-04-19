using RimWorld;
using Verse;

namespace BloodtypeHardcore
{
    [DefOf]
    public static class BloodtypeMarkerHediffDefOf
    {
        public static HediffDef BloodtypeMarker_O_Positive;
        public static HediffDef BloodtypeMarker_O_Negative;
        public static HediffDef BloodtypeMarker_A_Positive;
        public static HediffDef BloodtypeMarker_A_Negative;
        public static HediffDef BloodtypeMarker_B_Positive;
        public static HediffDef BloodtypeMarker_B_Negative;
        public static HediffDef BloodtypeMarker_AB_Positive;
        public static HediffDef BloodtypeMarker_AB_Negative;

        static BloodtypeMarkerHediffDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BloodtypeMarkerHediffDefOf));
        }
    }
}
