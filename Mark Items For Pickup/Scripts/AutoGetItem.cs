using System;
using XRL.World;

namespace Plaidman.ZoneLootList.Parts {
	[Serializable]
	public class ZLL_AutoGetItem : IPart {
		public override bool WantEvent(int id, int cascade) {
			return base.WantEvent(id, cascade)
				|| id == AutoexploreObjectEvent.ID
				|| id == AddedToInventoryEvent.ID;
		}

		public override bool HandleEvent(AutoexploreObjectEvent e) {
			e.Command ??= "Autoget";
			return base.HandleEvent(e);
		} 

		public override bool HandleEvent(AddedToInventoryEvent e) {
			if (e.Item.ID == ParentObject.ID) {
				ParentObject.RemovePart<ZLL_AutoGetItem>();
			}

			return base.HandleEvent(e);
		}
    }
}