using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Foxy.AutoTraining {
	public class ModMain : Mod {
		public Settings Settings => GetSettings<Settings>();

		private static readonly Color HoverColor = new Color(1f, 1f, 1f, 0.3f);
		private static readonly Color EvenColor = new Color(0f, 0f, 0f, 0.2f);

		private string settings_input = string.Empty;
		private Vector2 filter_scroll = Vector2.zero;
		private Vector2 list_scroll = Vector2.zero;
		private List<PawnKindDef> ignored = null;
		private readonly List<PawnKindDef> filter = new List<PawnKindDef>();

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

			listing.GapLine();

			listing.Label("Ignored defs:");
			DrawList(listing);
			listing.Gap();

			string old_input = settings_input;
			Rect inputRect = listing.GetRect(Text.LineHeight);
			inputRect.width -= 16f;
			settings_input = Widgets.TextField(inputRect, settings_input);
			if (old_input != settings_input) UpdateFilter();
			listing.End();

			DrawFilter(inRect.BottomPartPixels(inRect.height - listing.CurHeight));
		}

		private void DrawList(Listing_Standard listing) {
			if (ignored == null) {
				ignored = new List<PawnKindDef>();
				foreach (string defName in Settings.KindFilter) {
					PawnKindDef def = DefDatabase<PawnKindDef>.GetNamedSilentFail(defName);
					if (def != null) ignored.Add(def);
				}
			}
			int count = ignored.Count;
			Rect listRect = listing.GetRect(5 * Text.LineHeight);
			Rect viewRect = new Rect(0, 0, listRect.width - 32f, count * Text.LineHeight);
			Rect rowRect = viewRect.TopPartPixels(Text.LineHeight);
			rowRect.x = 16f;
			Widgets.BeginScrollView(listRect, ref list_scroll, viewRect);
			PawnKindDef removed = null;
			bool even = false;
			foreach (PawnKindDef def in ignored) {
				bool hover = Mouse.IsOver(rowRect);
				if (hover || even) {
					Widgets.DrawRectFast(new Rect(0, rowRect.y, listRect.width - 16f, rowRect.height), hover ? HoverColor : EvenColor);
				}
				rowRect.SplitVertically(rowRect.width - Text.LineHeight, out Rect leftRect, out Rect buttonRect);
				Widgets.LabelFit(leftRect.LeftHalf(), def.defName);
				Widgets.LabelFit(leftRect.RightHalf(), def.LabelCap);
				if (Widgets.ButtonText(buttonRect, "-")) {
					removed = def;
				}
				rowRect.y += Text.LineHeight;
				even = !even;
			}
			if (removed != null) RemoveIgnoreDef(removed);

			Widgets.EndScrollView();
		}

		private void DrawFilter(Rect inRect) {
			Rect viewRect = new Rect(0, 0, inRect.width - 32f, filter.Count * Text.LineHeight);
			Rect rowRect = viewRect.TopPartPixels(Text.LineHeight);
			rowRect.x = 16f;
			Widgets.BeginScrollView(inRect, ref filter_scroll, viewRect);

			bool even = false;
			foreach (PawnKindDef def in filter) {
				bool hover = Mouse.IsOver(rowRect);
				if (hover || even) {
					Widgets.DrawRectFast(new Rect(0, rowRect.y, inRect.width - 16f, rowRect.height), hover ? HoverColor : EvenColor);
				}
				bool isIgnored = ignored.Contains(def);
				rowRect.SplitVertically(rowRect.width - Text.LineHeight, out Rect leftRect, out Rect buttonRect);
				rowRect.y += Text.LineHeight;
				Widgets.LabelFit(leftRect.LeftHalf(), def.defName);
				Widgets.LabelFit(leftRect.RightHalf(), def.LabelCap);
				if (Widgets.ButtonText(buttonRect, isIgnored ? "-" : "+")) {
					if (isIgnored) RemoveIgnoreDef(def);
					else AddIgnoreDef(def);
				}
				even = !even;
			}

			Widgets.EndScrollView();
		}

		private void AddIgnoreDef(PawnKindDef def) {
			ignored.Add(def);
			Settings.KindFilter.Add(def.defName);
			list_scroll = Vector2.zero;
		}
		private void RemoveIgnoreDef(PawnKindDef def) {
			ignored.Remove(def);
			Settings.KindFilter.Remove(def.defName);
			list_scroll = Vector2.zero;
		}

		private void UpdateFilter() {
			string input = settings_input.ToLower();
			filter.Clear();
			filter_scroll = Vector2.zero;
			if (string.IsNullOrWhiteSpace(input)) return;
			foreach (PawnKindDef def in DefDatabase<PawnKindDef>.AllDefsListForReading) {
				if (!def.IsAnimal()) continue;
				bool matchesDef = def.defName.ToLower().Contains(input);
				bool matchesName = def.label.ToLower().Contains(input);
				if (matchesDef || matchesName) filter.Add(def);
			}
		}
	}
}
