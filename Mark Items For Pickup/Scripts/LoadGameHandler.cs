using XRL;
using XRL.World;
using XRL.World.Parts;

namespace Plaidman.ItemPickup.Scripts
{
	[HasCallAfterGameLoaded]
	public class LoadGameHandler
	{
		[CallAfterGameLoaded]
		public static void AfterLoaded()
		{
			The.Player?.RequirePart<Plaidman_ItemPickup_ItemFinderPart>();
		}
	}

	[PlayerMutator]
	public class NewCharacterHandler : IPlayerMutator
	{
		public void mutate(GameObject player)
		{
			player.RequirePart<Plaidman_ItemPickup_ItemFinderPart>();
		}
	}
}
