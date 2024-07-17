using System;
using System.Collections.Generic;
using System.Linq;
using XRL.UI;

// TODOs
// add an option to use ability or not
// icon for mod and popup
// different title for mod
//  - one man's treasure
//  - autoloot spoils
//  - autoloot item list
// ensure manifest.json has the right labels
// write description
//  - improvements
//  - options
//  - keybinds
//  - with caveats
//    - stacked items (copper dagger x2)
//    - dropped by player
//    - mines
// button to travel directly to an item? (for player dropped items)

namespace XRL.World.Parts {
	[Serializable]
	public class Plaidman_ItemPickup_ItemFinderPart : IPlayerPart {
		public static readonly string ItemListCommand = "Plaidman_ItemPickup_ShowItemList";
		public static readonly string ItemListUninstall = "Plaidman_ItemPickup_Uninstall";
		public static readonly string TrashOption = "Plaidman_ItemPickup_Option_Trash";
		public static readonly string CorpsesOption = "Plaidman_ItemPickup_Option_Corpses";
		public static readonly string ValuesOption = "Plaidman_ItemPickup_Option_Value";

		public override bool WantEvent(int id, int cascade) {
			return base.WantEvent(id, cascade)
				|| id == CommandEvent.ID
				|| id == EnteringZoneEvent.ID;
		}

		public override bool HandleEvent(EnteringZoneEvent e) {
			var items = ParentObject.CurrentZone.GetObjectsWithPart("Plaidman_ItemPickup_AutoGetPart");
			foreach (var item in items) {
				item.RemovePart<Plaidman_ItemPickup_AutoGetPart>();
			}

			return base.HandleEvent(e);
		}

		public override bool HandleEvent(CommandEvent e) {
			if (e.Command == ItemListCommand) {
				ListItems();
			}

			if (e.Command == ItemListUninstall) {
				UninstallParts();
			}

			return base.HandleEvent(e);
		}

		private void UninstallParts() {
			var items = ParentObject.CurrentZone.GetObjectsWithPart("Plaidman_ItemPickup_AutoGetPart");
			foreach (var item in items) {
				item.RemovePart<Plaidman_ItemPickup_AutoGetPart>();
			}
			Messages.MessageQueue.AddPlayerMessage("Uninstall: removed item parts");

			ParentObject.RemovePart<Plaidman_ItemPickup_ItemFinderPart>();
			Messages.MessageQueue.AddPlayerMessage("Uninstall: removed player part");
			
			Popup.Show("Uninstall: Finished removing mod. Save your game, then you can remove this mod.");
		}
		
		private void ListItems() {
			var gettableItems = ParentObject.CurrentZone.GetObjects(FilterOptions);
			
			if (gettableItems.Count == 0) {
				Popup.Show("There are no gettable items in the zone that you have seen.");
				return;
			}

			var initialSelections = new List<int>();
			for (int i = 0; i < gettableItems.Count; i++) {
				if (gettableItems[i].HasPart<Plaidman_ItemPickup_AutoGetPart>()) {
					initialSelections.Add(i);
				}
			}

			var toggledItemsEnumerator = Plaidman.ItemPickup.Menus.ItemList_Popup.ShowPopup(
				options: gettableItems.Select(go => GetOptionLabel(go)).ToArray(),
				icons: gettableItems.Select(go => go.Render).ToArray(),
				initialSelections: initialSelections.ToArray()
			);

			foreach (Plaidman.ItemPickup.Menus.ItemList_Popup.ToggledItem result in toggledItemsEnumerator) {
				var item = gettableItems[result.Index];

				if (result.Value) {
					item.RequirePart<Plaidman_ItemPickup_AutoGetPart>();
				} else {
					item.RemovePart<Plaidman_ItemPickup_AutoGetPart>();
				}
			}
		}

		private string GetOptionLabel(GameObject go) {
			var label = go.GetDisplayName();

			if (Options.GetOption(ValuesOption) == "No") {
				return label;
			}
			
			if (go.GetWeight() <= 0.0) {  // 0 weight is cyan
				return label += "   [{{c|$}}]";
			}
			
			var value = go.Value / go.GetWeight();
			if (value < 1) {  // <1 is red
				return label += "   [{{R|$}}]";
			}

			if (value < 4) {  // 1-4 is yellow
				return label += "   [{{W|$}}]";
			}

			if (value < 10) {  // 4-10 is green
				return label += "   [{{G|$}}]";
			}

			if (value < 25) {  // 10-25 is double green
				return label += "   [{{G|$$}}]";
			}

			return label += "   [{{G|$$$}}]";  // >25 is triple green
		} 
		
		private bool FilterOptions(GameObject go) {
			var autogetByDefault = go.ShouldAutoget()
				&& !go.HasPart<Plaidman_ItemPickup_AutoGetPart>();
			var isCorpse = go.GetInventoryCategory() == "Corpses"
				&& Options.GetOption(CorpsesOption) != "Yes";
			var isTrash = go.DisplayName == "trash"
				&& Options.GetOption(TrashOption) != "Yes";

			var armedMine = false;
			if (go.HasPart<Tinkering_Mine>()) {
				armedMine = go.GetPart<Tinkering_Mine>().Armed;
			}

			return go.Physics.Takeable
				&& go.Physics.CurrentCell.IsExplored()
				&& !go.HasPropertyOrTag("DroppedByPlayer")
				&& !go.HasPropertyOrTag("NoAutoget")
				&& !go.IsOwned()
				&& !go.IsHidden
				&& !armedMine
				&& !autogetByDefault
				&& !isCorpse
				&& !isTrash;
		}
	}
}
