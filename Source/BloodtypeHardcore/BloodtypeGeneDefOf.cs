using RimWorld;
using Verse;

namespace BloodtypeHardcore
{
    [DefOf]
    public static class BloodtypeGeneDefOf
    {
        public static GeneDef BloodtypeHumanProfile;
        public static GeneDef BloodtypeNoBloodProfile;

        static BloodtypeGeneDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BloodtypeGeneDefOf));
        }
    }
}
