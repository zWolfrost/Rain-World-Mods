public partial class CentiwingPlayerData
{
	public bool addedSpawn = false;
}

public partial class Plugin
{
	private void Player_Spawn(On.Player.orig_Update orig, Player self, bool eu)
	{
		orig(self, eu);
		CentiwingPlayerData centiwing = self.Centiwing();
		if (!centiwing.IsCentiwing) return;

		bool isFirstCycle = self.room.game.GetStorySession.saveState.cycleNumber == 0;

		if (self.room.abstractRoom.name == "SI_A09x" && isFirstCycle && !centiwing.addedSpawn)
		{
			centiwing.addedSpawn = true;
			self.room.AddObject(new SI_A09xSpawn(self.room));
		}
	}
}

public class SI_A09xSpawn : UpdatableAndDeletable
{
	public SI_A09xSpawn(Room room) => this.room = room;

	public override void Update(bool eu)
	{
		base.Update(eu);

		if (!room.game.AllPlayersRealized) return;

		for (int i = 0; i < room.game.Players.Count; i++)
		{
			Player player = room.game.Players[i].realizedCreature as Player;
			player.bodyChunks[0].HardSetPosition(new Vector2(253f, 365.6f));
			player.bodyChunks[1].HardSetPosition(new Vector2(253f, 348));
			player.standing = true;
		}

		Destroy();
	}
}