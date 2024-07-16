using System;
using System.Collections.Generic;
using System.Linq;
using XRL.UI;

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
				|| id == CommandEvent.ID;
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
			Messages.MessageQueue.AddPlayerMessage("not implemented yet");
			Messages.MessageQueue.AddPlayerMessage("value option: " + Options.GetOption(ValuesOption));
		}
		
		private void ListItems() {
			List<GameObject> gettableItems = ParentObject.CurrentZone.GetObjects(
				(GameObject go) => {
					var autogetByDefault = go.ShouldAutoget()
						&& !go.HasPart<Plaidman_ItemPickup_AutoGetPart>();
					var isCorpse = go.GetInventoryCategory() == "Corpses"
						&& Options.GetOption(CorpsesOption) != "Yes";
					var isTrash = go.DisplayName == "trash"
						&& Options.GetOption(TrashOption) != "Yes";

					return go.Physics.Takeable
						&& go.Physics.CurrentCell.IsExplored()
						&& !go.HasPropertyOrTag("DroppedByPlayer")
						&& !go.HasPropertyOrTag("NoAutoget")
						&& !autogetByDefault
						&& !go.IsOwned()
						&& !isCorpse
						&& !isTrash;
				}
			);
			
			// TODOs
			// options:
			//   - list item's $/#
			// keybinds
			//   - uninstall
			// add icons for the ability and popup
			// test to see how these behave with mineshell - spawn a mine layer mk 1? or something like that
			// button to travel directly to an item (for armed mines?)
			// better title for mod
			// ensure manifest.json has the right labels

			if (gettableItems.Count == 0) {
				Popup.Show("There are no gettable items in the zone that you have seen.");
				return;
			}

			List<int> initialSelections = new();
			for (int i = 0; i < gettableItems.Count; i++) {
				if (gettableItems[i].HasPart<Plaidman_ItemPickup_AutoGetPart>()) {
					initialSelections.Add(i);
				}
			}

			var toggledItemsEnumerator = Plaidman.ItemPickup.Menus.ItemList_Popup.ShowPopup(
				options: gettableItems.Select(o => o.DisplayName).ToArray(),
				icons: gettableItems.Select(o => o.Render).ToArray(),
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
	}
}
