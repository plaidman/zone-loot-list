using System;
using System.Collections.Generic;
using System.Linq;
using XRL.UI;

namespace XRL.World.Parts {
	[Serializable]
	public class Plaidman_ItemPickup_ItemFinderPart : IPlayerPart {
		public static readonly string ItemListCommand = "PM_ItemPickup_ShowItemList";
		public Guid ActivatedAbility;

		public override void Attach() {
			if (ActivatedAbility == Guid.Empty)
				ActivatedAbility = ParentObject.AddActivatedAbility("Show Items", ItemListCommand, "Skill", Silent: true);
			base.Attach();
		}

		public override void Remove() {
			RemoveMyActivatedAbility(ref ActivatedAbility, ParentObject);
			base.Remove();
		}
		
		public override bool WantEvent(int id, int cascade) {
			return base.WantEvent(id, cascade)
				|| id == CommandEvent.ID;
		}

		public override bool HandleEvent(CommandEvent e) {
			if (e.Command == ItemListCommand && e.Actor == ParentObject) {
				ListItems();
			}

			return base.HandleEvent(e);
		}
		
		private void ListItems() {
			List<GameObject> gettableItems = ParentObject.CurrentZone.GetObjects(
				(GameObject go) => {
					var autogetByDefault = go.ShouldAutoget()
						&& !go.HasPart<Plaidman_ItemPickup_AutoGetPart>();

					return go.Physics.Takeable
						&& go.Physics.CurrentCell.IsExplored()
						&& !go.HasPropertyOrTag("DroppedByPlayer")
						&& !go.HasPropertyOrTag("NoAutoget")
						&& !autogetByDefault
						&& !go.IsOwned()
						&& go.GetInventoryCategory() != "Corpses"
						&& go.DisplayName != "trash";
				}
			);
			
			// TODOs
			// options:
			//   - list item's $/#
			//   - show/hide trash
			//   - show/hide corpses
			//   - uninstall?
			// add icons for the ability and popup
			// button to travel directly to an item (for armed mines?)
			// wish to uninstall the mod
			// use a keybind instead of ability
			// test to see how these behave with mineshell - spawn a mine layer mk 1? or something like that

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

			var toggledItemsEnumerator = PM.ItemPickup.Menus.ItemList_Popup.ShowPopup(
				options: gettableItems.Select(o => o.DisplayName).ToArray(),
				icons: gettableItems.Select(o => o.Render).ToArray(),
				initialSelections: initialSelections.ToArray()
			);

			foreach (PM.ItemPickup.Menus.ItemList_Popup.ToggledItem result in toggledItemsEnumerator) {
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
