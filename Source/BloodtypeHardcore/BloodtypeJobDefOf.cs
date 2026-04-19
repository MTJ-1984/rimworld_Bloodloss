using RimWorld;
using Verse;

namespace BloodtypeHardcore
{
    [DefOf]
    public static class BloodtypeJobDefOf
    {
        public static JobDef Bloodtype_AdministerBloodPack;

        static BloodtypeJobDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BloodtypeJobDefOf));
        }
    }
}
