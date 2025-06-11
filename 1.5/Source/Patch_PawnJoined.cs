using HarmonyLib;
using RimWorld;
using Verse;

namespace Foxy.AutoTraining {
	[HarmonyPatch(typeof(Faction), nameof(Faction.Notify_PawnJoined))]
	public static class Patch_PawnJoined {
		public static void Prefix(Faction __instance, Pawn p) {
			Log.Message($"[AutoTraining] Pawn ({p}) joined faction ({__instance})");
			if (__instance != Faction.OfPlayerSilentFail) return;
			Log.Message($"[AutoTraining] It is a player faction");
			if (!p.IsAnimal()) return;
			Log.Message($"[AutoTraining] It is an animal");
			if (p.kindDef == null || Settings.IgnoredDefs.Contains(p.kindDef)) return;
			Log.Message($"[AutoTraining] It is not ignored ({p.kindDef})");
			if (p.training == null) return;
			Log.Message($"[AutoTraining] It has a training tracker");
			foreach (TrainableDef td in TrainableUtility.TrainableDefsInListOrder) {
				Log.Message($"[AutoTraining] Testing against {td?.LabelCap} training ({td})");
				if (p.training.CanAssignToTrain(td).Accepted) {
					Log.Message($"[AutoTraining] Training accepted");
					if (IsUnwanted(td)) continue;
					Log.Message($"[AutoTraining] Training wanted");
					p.training.SetWantedRecursive(td, true);
				}
			}
			Log.Message($"[AutoTraining] Patch completed");
		}

		private static bool IsUnwanted(TrainableDef td) {
			Log.Message($"[AutoTraining] Is {td.LabelCap} training wanted?");
			if (td?.prerequisites != null) {
				Log.Message($"[AutoTraining] It has prerequisites");
				foreach (TrainableDef req in td.prerequisites) {
					Log.Message($"[AutoTraining] Testing prerequisite: {req.LabelCap}");
					if (IsUnwanted(req)) return true;
					Log.Message($"[AutoTraining] Prerequisite passed");
				}
			}
			return Settings.IsUnwanted(td);
		}
	}
}
