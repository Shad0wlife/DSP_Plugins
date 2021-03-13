﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize
{
	[HarmonyPatch(typeof(UIBuildingGrid))]
	public class PatchUIBuildingGrid
    {
		[HarmonyPrefix]
		[HarmonyPatch("Update")]
		public static void PrefixUpdate(UIBuildingGrid __instance, Material ___material, Material ___altMaterial, Renderer ___gridRnd)
		{
            if (refreshGridRadius != -1)
			{
				//DumpRendererData(___gridRnd);
				//DumpTextures(___material);

				if(___material == null)
                {
					Patch.Debug("Material was null!", BepInEx.Logging.LogLevel.Debug, true);
					return;
				}
				int segments = (int)(refreshGridRadius / 4f + 0.1f) * 4;
				if (ImageDataLUT.ContainsKey(segments))
				{
					Patch.Debug("Updating LUT for radius + " + refreshGridRadius + " and segments " + segments + "!", BepInEx.Logging.LogLevel.Debug, true);
					UpdateTextureToLUT(___material, segments);
				}
				else
				{
					//TODO
					Patch.Debug("LUT512 did not yet contain the texture for refreshing.", BepInEx.Logging.LogLevel.Debug, true);
					refreshGridRadius = -1;
				}
			}
		}

		public static void DumpRendererData(Renderer renderer)
        {
			if(renderer != null)
            {
				Patch.Debug("Renderer name: " + renderer.name, BepInEx.Logging.LogLevel.Debug, true);
				Patch.Debug("Renderer type: " + renderer.GetType().ToString(), BepInEx.Logging.LogLevel.Debug, true);
				Patch.Debug("Renderer bounds radius: " + renderer.bounds.extents.magnitude, BepInEx.Logging.LogLevel.Debug, true);
				foreach(Material mat in renderer.materials)
                {
					Patch.Debug("Renderer material: " + mat.name, BepInEx.Logging.LogLevel.Debug, true);
				}
				
			}
        }

		public static void DumpTextures(params Material[] mats)
        {
			foreach(Material mat in mats)
			{
				Patch.Debug("Material name: " + mat.name, BepInEx.Logging.LogLevel.Debug, true);
				Patch.Debug("Material shader name: " + mat.shader.name, BepInEx.Logging.LogLevel.Debug, true);
				Patch.Debug("Material shader type: " + mat.shader.GetType().ToString(), BepInEx.Logging.LogLevel.Debug, true);
				if (mat == null)
				{
					continue;
				}
				foreach (string str in mat.GetTexturePropertyNames())
				{
					Texture tex = mat.GetTexture(str);
					Patch.Debug("Texture: " + tex.name + "/ID: " + str + " with wrapMode " + tex.wrapMode.ToString() + ", width " + tex.width + ", height" + tex.height + ", dimension " + tex.dimension.ToString(), BepInEx.Logging.LogLevel.Debug, true);
				}
				Patch.Debug("IDs:", BepInEx.Logging.LogLevel.Debug, true);
			}
			texDumped = true;
        }

		public static void UpdateTextureToLUT(Material material, int segment)
        {
			Texture tex = material.GetTexture("_SegmentTable");
			if (tex.dimension == TextureDimension.Tex2D)
			{
				Texture2D tex2d = (Texture2D)tex;
				int[] targetLUT = ImageDataLUT[segment];
				int height = tex2d.height;
				int width = tex2d.width; ;

				Patch.Debug("Texture Update - LUT size " + targetLUT.Length, BepInEx.Logging.LogLevel.Debug, true);
				/*
				if (targetLUT.Length > 512 && width == 512)
                {
					int newWidth = MathUtils.RoundToNextPowerOf2(targetLUT.Length);
					Patch.Debug("Resizing Grid Texture to " + newWidth + ", " + height, BepInEx.Logging.LogLevel.Debug, true);
					tex2d.Resize(newWidth, height);
                }
				else if(targetLUT.Length <= 512 && width > 512)
				{
					Patch.Debug("Resizing Grid Texture to 512, " + height, BepInEx.Logging.LogLevel.Debug, true);
					tex2d.Resize(512, height);
				}
				*/
				tex2d.Resize(1024, height);

				for (int i = 0; i < 1024; i++)
				{
					float num = (targetLUT[i] / 4 + 0.05f)/255f;
					for(int heightIdx = 0; heightIdx < height; heightIdx++)
					{
						tex2d.SetPixel(i, heightIdx, new Color(num, num, num, 1f));
					}
				}

				/*
				//Fill remaining with highest lut value if lut is not of length 2^x
				if(targetLUT.Length < width)
				{
					float num = Mathf.Min((targetLUT[targetLUT.Length - 1] / 4 + 0.05f) / 255f, 1f);
					for (int i = targetLUT.Length; i < width; i++)
					{
						for (int heightIdx = 0; heightIdx < height; heightIdx++)
						{
							tex2d.SetPixel(i, heightIdx, new Color(num, num, num, 1f));
						}
					}
				}
				*/

				tex2d.Apply();
			}

			texUpdated = true;
			refreshGridRadius = -1;
		}

		public static bool texDumped = false;
		public static bool texUpdated = false;
		public static int refreshGridRadius = -1;

		public static Dictionary<int, int[]> ImageDataLUT = new Dictionary<int, int[]>(); //segment count to 512 lut
	}
}
