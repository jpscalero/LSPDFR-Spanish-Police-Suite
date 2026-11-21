using System;

namespace PoliceLifeSystem.Career
{
    public static class CelebrationScheduleRules
    {
        public static int ClampIntervalDays(int intervalDays)
        {
            if (intervalDays < 1) return 1;
            if (intervalDays > 365) return 365;
            return intervalDays;
        }

        public static bool IsDue(DateTime lastCelebration, DateTime currentGameDate, int intervalDays)
        {
            if (lastCelebration == DateTime.MinValue) return false;
            if (currentGameDate < lastCelebration) return false;
            return currentGameDate - lastCelebration >= TimeSpan.FromDays(ClampIntervalDays(intervalDays));
        }
    }
}
