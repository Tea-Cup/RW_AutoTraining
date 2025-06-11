using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Foxy.AutoTraining {
	public class Settings : ModSettings {
		private static Settings instance;
		public static Settings Instance {
			get {
				if (instance == null) instance = LoadedModManager.GetMod<ModMain>().Settings;
				return instance;
			}
		}
		private static List<string> IgnoredDefNames {
			get {
				if (Instance.filter == null) Instance.filter = new List<string>();
				return Instance.filter;
			}
		}
		public static List<PawnKindDef> IgnoredDefs {
			get {
				if (Instance.ignored == null) Instance.ignored = new List<PawnKindDef>();
				return Instance.ignored;
			}
		}

		private HashSet<string> unwanted = null;
		private List<string> filter = null;

		private List<PawnKindDef> ignored = null;

		public override void ExposeData() {
			UpdateFilter();
			Scribe_Collections.Look(ref unwanted, "unwanted", LookMode.Value);
			Scribe_Collections.Look(ref filter, "filter", LookMode.Value);
			UpdateIgnored();
		}

		private static void UpdateFilter() {
			IgnoredDefNames.Clear();
			IgnoredDefNames.AddRange(IgnoredDefs.Select(x => x.defName));
		}
		private static void UpdateIgnored() {
			IgnoredDefs.Clear();
			foreach (string defname in IgnoredDefNames) {
				PawnKindDef def = DefDatabase<PawnKindDef>.GetNamed(defname, false);
				if (def != null) IgnoredDefs.Add(def);
			}
		}

		public static bool IsUnwanted(TrainableDef td) {
			return Instance.unwanted?.Contains(td.defName) ?? false;
		}

		public static void SetUnwanted(TrainableDef td, bool value) {
			if (value) {
				if (Instance.unwanted == null) Instance.unwanted = new HashSet<string>();
				Instance.unwanted.Add(td.defName);
			} else {
				Instance.unwanted?.Remove(td.defName);
			}
		}
	}
}
