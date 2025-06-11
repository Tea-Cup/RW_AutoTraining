using System.Collections.Generic;
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
		public List<string> KindFilter {
			get {
				if (filter == null) filter = new List<string>();
				return filter;
			}
		}

		private HashSet<string> unwanted = new HashSet<string>();
		private List<string> filter = new List<string>();

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Collections.Look(ref unwanted, "unwanted");
			Scribe_Collections.Look(ref filter, "filter");
		}

		public bool IsUnwanted(TrainableDef td) {
			return Instance.unwanted?.Contains(td.defName) ?? false;
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
