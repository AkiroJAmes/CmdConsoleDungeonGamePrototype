using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using static TextBasedAdventureGame.Program;

namespace TextBasedAdventureGame
{
    class Utility
    {
        public static BattleOptions Next(BattleOptions src)
        {
            BattleOptions[] arr = (BattleOptions[])Enum.GetValues(src.GetType());

            // Get next index based on src index in array
            int j = Array.IndexOf(arr, src) + 1;
            return (arr.Length == j) ? arr[0] : arr[j];
        }

        public static BattleOptions Previous(BattleOptions src)
        {
            BattleOptions[] arr = (BattleOptions[])Enum.GetValues(src.GetType());

            // Get previous index based on src index in array
            int j = Array.IndexOf(arr, src) - 1;
            return (0 <= j) ? arr[j] : arr[arr.Length - 1];
        }

        public static Timer ResetEnemyTimer(Timer timer)
        {
            timer.Dispose();
            return timer = new Timer(TimerCallback, null, 1000, 1000);
        }
    }
}
