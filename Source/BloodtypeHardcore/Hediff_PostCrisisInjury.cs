using Verse;

namespace BloodtypeHardcore
{
    public class Hediff_PostCrisisInjury : HediffWithComps
    {
        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (pawn == null || pawn.Dead || delta <= 0)
            {
                return;
            }

            float severityPerDay = GetRecoverySeverityPerDay();
            Severity -= severityPerDay * (delta / 60000f);
            if (Severity <= 0f)
            {
                pawn.health.RemoveHediff(this);
            }
        }

        private static float GetRecoverySeverityPerDay()
        {
            string preset = BloodtypeMod.Settings?.selectedPreset ?? "Normal";
            if (preset == "Easy")
            {
                return 1f / 1.5f;
            }

            if (preset == "Hard")
            {
                return 1f / 2.5f;
            }

            if (preset == "UltraBrutal")
            {
                return 1f / 3.5f;
            }

            return 1f / 2f;
        }
    }
}
