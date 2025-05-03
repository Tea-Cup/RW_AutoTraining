using HarmonyLib;
using UnityEngine;
using Verse;

namespace Foxy.AutoTraining {
	[StaticConstructorOnStartup]
	public static class Static {
		static Static() {
			Harmony h = new Harmony("Foxy.AutoTraining");
			h.PatchAll();
		}
	}
}
