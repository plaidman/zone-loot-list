using System;

namespace XRL.World.Parts {
	[Serializable]
    public class Plaidman_ItemPickup_AutoGetPart : IPart {
        public override bool WantEvent(int id, int cascade) {
            return base.WantEvent(id, cascade)
                || id == AutoexploreObjectEvent.ID
                || id == AddedToInventoryEvent.ID;
        }

        public override bool HandleEvent(AutoexploreObjectEvent e) {
            e.Command ??= "Autoget";
            Messages.MessageQueue.AddPlayerMessage(ParentObject.DisplayName + " command: " + e.Command);
            return base.HandleEvent(e);
        } 

        public override bool HandleEvent(AddedToInventoryEvent e) {
            if (e.Item.ID == ParentObject.ID) {
                Messages.MessageQueue.AddPlayerMessage("JSON picked up " + ParentObject.DisplayName);
                ParentObject.RemovePart<Plaidman_ItemPickup_AutoGetPart>();
            }

            return base.HandleEvent(e);
        } 
    }
}