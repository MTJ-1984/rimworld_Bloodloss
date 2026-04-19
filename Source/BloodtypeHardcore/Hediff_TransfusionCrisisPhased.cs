using Verse;

namespace BloodtypeHardcore
{
    public class Hediff_TransfusionCrisisPhased : HediffWithComps
    {
        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (pawn == null || pawn.Dead || delta <= 0)
            {
                return;
            }

            float severityPerDay = BloodtypeMod.Settings?.crisisSeverityPerDay ?? 10.66f;
            Severity += severityPerDay * (delta / 60000f);
        }
    }
}
