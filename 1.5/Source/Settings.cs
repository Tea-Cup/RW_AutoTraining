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

		private HashSet<string> unwanted = null;
		private List<string> filter = null;

		public List<PawnKindDef> ignored = null;

		public override void ExposeData() {
			if (Scribe.mode == LoadSaveMode.Saving) UpdateFilter();
			Scribe_Collections.Look(ref unwanted, "unwanted", LookMode.Value);
			Scribe_Collections.Look(ref filter, "filter", LookMode.Value);
			if (Scribe.mode == LoadSaveMode.PostLoadInit) UpdateIgnored();
		}

		private void EnsureNotNull() {
			if (filter == null) filter = new List<string>();
			if (ignored == null) ignored = new List<PawnKindDef>();
		}
		private void UpdateFilter() {
			EnsureNotNull();
			filter.Clear();
			filter.AddRange(ignored.Select(x => x.defName));
		}
		private void UpdateIgnored() {
			EnsureNotNull();
			ignored.Clear();
			foreach (string defname in filter) {
				PawnKindDef def = DefDatabase<PawnKindDef>.GetNamed(defname, false);
				if (def != null) ignored.Add(def);
			}
		}

		public bool IsUnwanted(TrainableDef td) {
			return unwanted?.Contains(td.defName) ?? false;
		}

		public void SetUnwanted(TrainableDef td, bool value) {
			if (value) {
				if (unwanted == null) unwanted = new HashSet<string>();
				unwanted.Add(td.defName);
			} else {
				unwanted?.Remove(td.defName);
			}
		}
	}
}
