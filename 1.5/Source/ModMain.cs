using RimWorld;
using UnityEngine;
using Verse;

namespace Foxy.AutoTraining {
	public class ModMain : Mod {
		public Settings Settings => GetSettings<Settings>();

		public ModMain(ModContentPack content) : base(content) { }

		public override string SettingsCategory() {
			return Content.Name;
		}

		public override void DoSettingsWindowContents(Rect inRect) {
			Listing_Standard listing = new Listing_Standard() {
				maxOneColumn = true,
				ColumnWidth = inRect.width
			};
			listing.Begin(inRect);

			foreach (TrainableDef td in TrainableUtility.TrainableDefsInListOrder) {
				bool check = !Settings.IsUnwanted(td);
				Rect rect = listing.GetRect(28f);
				Widgets.DrawHighlightIfMouseover(rect);
				rect.xMin += td.indent * 10f;
				Widgets.CheckboxLabeled(rect, td.LabelCap, ref check);
				if (Mouse.IsOver(rect)) {
					TooltipHandler.TipRegion(rect, () => {
						return td.LabelCap + "\n\n" + td.description;
					}, (int)(rect.y * 612f + rect.x));
				}
				Settings.SetUnwanted(td, !check);
			}
			listing.End();
		}
	}
}
