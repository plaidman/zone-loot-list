using System;
using XRL;
using XRL.World;

// todo: change this to XRL.World.Parts when this mod is broken by qud changes
namespace Plaidman.ZoneLootList.Parts {
	[Serializable]
	public class ZLL_AutoGetItem : IPart {
        public override void Register(GameObject go, IEventRegistrar registrar) {
			registrar.Register(AutoexploreObjectEvent.ID);
			registrar.Register(AddedToInventoryEvent.ID);
			registrar.Register(The.Game, ZoneActivatedEvent.ID);

            base.Register(go, registrar);
        }

        public override bool HandleEvent(ZoneActivatedEvent e) {
			if (e.Zone.IsWorldMap()) {
				return base.HandleEvent(e);
			}

			if (ParentObject.InZone(e.Zone)) {
				return base.HandleEvent(e);
			}

			ParentObject.RemovePart<ZLL_AutoGetItem>();
			return base.HandleEvent(e);
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