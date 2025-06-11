using HarmonyLib;
using RimWorld;
using Verse;

namespace Foxy.AutoTraining {
	[HarmonyPatch(typeof(Faction), nameof(Faction.Notify_PawnJoined))]
	public static class Patch_PawnJoined {
		public static void Prefix(Faction __instance, Pawn p) {
			if (__instance != Faction.OfPlayerSilentFail) return;
			if (!p.IsAnimal()) return;
			if (p.kindDef == null || Settings.Instance.KindFilter.Contains(p.kindDef.defName)) return;
			if (p.training == null) return;
			foreach (TrainableDef td in TrainableUtility.TrainableDefsInListOrder) {
				if (p.training.CanAssignToTrain(td).Accepted) {
					if (IsUnwanted(td)) continue;
					p.training.SetWantedRecursive(td, true);
				}
			}
		}

		private static bool IsUnwanted(TrainableDef td) {
			if (td?.prerequisites != null) {
				foreach (TrainableDef req in td.prerequisites) {
					if (IsUnwanted(req)) return true;
				}
			}
			return Settings.Instance.IsUnwanted(td);
		}
	}
}
