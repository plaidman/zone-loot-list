using XRL;
using XRL.World;
using XRL.World.Parts;

namespace Plaidman.ItemPickup.Handlers
{
	[HasCallAfterGameLoaded]
	public class LoadGameHandler {
		[CallAfterGameLoaded]
		public static void AfterLoaded() {
			if (The.Player == null) return;

			The.Player.RequirePart<Plaidman_ItemPickup_ItemFinderPart>();
			var part = The.Player.GetPart<Plaidman_ItemPickup_ItemFinderPart>();
			part.ToggleAbility();
		}
	}

	[PlayerMutator]
	public class NewCharacterHandler : IPlayerMutator {
		public void mutate(GameObject player) {
			player.RequirePart<Plaidman_ItemPickup_ItemFinderPart>();
			var part = player.GetPart<Plaidman_ItemPickup_ItemFinderPart>();
			part.ToggleAbility();
		}
	}
	
	[HasOptionFlagUpdate]
	public class OptionChangeHandler {
		[OptionFlagUpdate]
		public static void FlagUpdate() {
			if (The.Player == null) return;
			if (!The.Player.HasPart<Plaidman_ItemPickup_ItemFinderPart>()) return;
			
			var part = The.Player.GetPart<Plaidman_ItemPickup_ItemFinderPart>();
			part.ToggleAbility();
		}
	}
}
