using UnityEngine;
using Random = System.Random;

namespace GalacticScale.Scripts {
    public static class MathUtils {
        public static float RangePlusMinusOne(Random mainSeed) {
            // will return a number between -1 and 1 
            return Mathf.Sin((float) (mainSeed.NextDouble() * (2 * Mathf.PI)));
        }

		public static int RoundToNextPowerOf2(int from)
		{
			int result = from;
			result--;
			result |= result >> 1;
			result |= result >> 2;
			result |= result >> 4;
			result |= result >> 8;
			result |= result >> 16;
			result++;
			return result;
		}
	}
}