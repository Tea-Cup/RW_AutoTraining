using HarmonyLib;
using Verse;

namespace Foxy.AutoTraining {
	[StaticConstructorOnStartup]
	public static class Static {
		static Static() {
			Harmony h = new Harmony("Foxy.AutoTraining");
			h.PatchAll();
		}

		public static bool IsAnimal(this Pawn p) {
			if (p?.RaceProps == null) return false;
			return p.RaceProps.Animal && !p.IsMutant;
		}
		public static bool IsAnimal(this PawnKindDef p) {
			if (p?.RaceProps == null) return false;
			return p.RaceProps.Animal && p.mutant == null;
		}
	}
}
