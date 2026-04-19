using RimWorld;
using Verse;

namespace BloodtypeHardcore
{
    [DefOf]
    public static class BloodtypeThingDefOf
    {
        public static ThingDef BloodPack_O_Positive;
        public static ThingDef BloodPack_O_Negative;
        public static ThingDef BloodPack_A_Positive;
        public static ThingDef BloodPack_A_Negative;
        public static ThingDef BloodPack_B_Positive;
        public static ThingDef BloodPack_B_Negative;
        public static ThingDef BloodPack_AB_Positive;
        public static ThingDef BloodPack_AB_Negative;

        static BloodtypeThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BloodtypeThingDefOf));
        }
    }
}
