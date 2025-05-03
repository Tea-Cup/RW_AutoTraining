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

		private HashSet<string> unwanted = null;

		public override void ExposeData() {
			Scribe_Collections.Look(ref unwanted, "unwanted", LookMode.Value);
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
