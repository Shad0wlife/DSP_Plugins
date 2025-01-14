﻿using System.Collections.Generic;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale.Scripts {
    public static class PlanetRawDataExtension {
        private static readonly Dictionary<PlanetRawData, float>
            FactoredRadius = new Dictionary<PlanetRawData, float>();

        public static void AddFactoredRadius(this PlanetRawData planetRawData, PlanetData planet) {
            FactoredRadius[planetRawData] = planet.GetScaleFactored();
        }

        public static float GetFactoredScale(this PlanetRawData planetRawData) {
            return FactoredRadius.TryGetValue(planetRawData, out var result) ? result : 1f;
        }

        public static int GetModPlaneInt(this PlanetRawData planetRawData, int index) {
            float baseHeight = 20;

            baseHeight += planetRawData.GetFactoredScale() * 200 * 100;

            return (int) (((planetRawData.modData[index >> 1] >> (((index & 1) << 2) + 2)) & 3) * 133 + baseHeight);
        }
    }
}