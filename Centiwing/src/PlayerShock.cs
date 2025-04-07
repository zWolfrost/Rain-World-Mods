public partial class CentiwingPlayerData
{
	public const float shockRange = 40f;
	public const float shockDamage = 0.25f;
	public const int shockStun = 60;
	public const int shockSelfStun = 15;

	public int specialKeyHoldTime;
}

public partial class Plugin
{
	private void Player_Shock(On.Player.orig_Update orig, Player self, bool eu)
	{
		orig(self, eu);
		CentiwingPlayerData centiwing = self.Centiwing();
		if (!centiwing.IsCentiwing) return;

		/* Logger.LogInfo("IS SHOCKING: " + centiwing.isGliding.ToString()); */

		if (self.input[0].spec) centiwing.specialKeyHoldTime++;
		else centiwing.specialKeyHoldTime = 0;

		if (centiwing.specialKeyHoldTime != 20) return;

		self.room.PlaySound(SoundID.Jelly_Fish_Tentacle_Stun, self.mainBodyChunk);
		for (int i=0; i<6; i++)
		{
			self.room.AddObject(new Spark(self.mainBodyChunk.pos, Custom.RNV() * Mathf.Lerp(4f, 14f, Random.value), new Color(0.7f, 0.7f, 1f), null, 8, 14));
		}
		for (int i=0; i<self.bodyChunks.Length; i++)
		{
			self.bodyChunks[i].vel += Custom.RNV() * 6f * Random.value;
			self.bodyChunks[i].pos += Custom.RNV() * 6f * Random.value;
		}

		self.room.AddObject(new ShockWave(self.mainBodyChunk.pos, Mathf.Lerp(40f, 60f, Random.value), 0.07f, 6, false));

		self.Stun(CentiwingPlayerData.shockSelfStun);

		for (int i=0; i<self.room.physicalObjects.Length; i++)
		{
			for (int j=0; j<self.room.physicalObjects[i].Count; j++)
			{
				PhysicalObject curPhysicalObject = self.room.physicalObjects[i][j];
				BodyChunk curBodyChunk = curPhysicalObject.firstChunk;

				if (self == curPhysicalObject) continue;
				if (curPhysicalObject is not Creature) continue;

				float minDistance = float.MaxValue;
				for (int k=0; k<self.room.physicalObjects[i][j].bodyChunks.Length; k++)
				{
					float distance = Mathf.Min(float.MaxValue, Vector2.Distance(self.mainBodyChunk.pos, curPhysicalObject.bodyChunks[k].pos));
					if (distance < minDistance) minDistance = distance;
				}

				if (minDistance > CentiwingPlayerData.shockRange) continue;
				if (!self.room.VisualContact(self.mainBodyChunk.pos, curBodyChunk.pos)) continue;

				(curPhysicalObject as Creature).Violence(self.mainBodyChunk, null, curBodyChunk, null, Creature.DamageType.Electric, CentiwingPlayerData.shockDamage, CentiwingPlayerData.shockStun);
				(curPhysicalObject as Creature).Stun(CentiwingPlayerData.shockStun);
			}
		}
	}
}
