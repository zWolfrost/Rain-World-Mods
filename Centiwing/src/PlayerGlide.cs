public partial class CentiwingPlayerData
{
	public const float glideFallVelocity = -2f;
	public const float glideKickIn = 5f;

	public int currentGlideDuration;
   public bool isGliding;

	public bool CanGlide()
	{
		return player.input[0].jmp &&
			player.canJump <= 0 &&
			player.mainBodyChunk.vel.y < 0 &&
			player.canWallJump == 0 &&
			player.Consious &&
			player.bodyMode != Player.BodyModeIndex.Crawl &&
			player.bodyMode != Player.BodyModeIndex.CorridorClimb &&
			player.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut &&
			player.bodyMode != Player.BodyModeIndex.WallClimb &&
			player.bodyMode != Player.BodyModeIndex.Swimming &&
			player.animation != Player.AnimationIndex.HangFromBeam &&
			player.animation != Player.AnimationIndex.ClimbOnBeam &&
			player.animation != Player.AnimationIndex.AntlerClimb &&
			player.animation != Player.AnimationIndex.VineGrab &&
			player.animation != Player.AnimationIndex.ZeroGPoleGrab;
	}
}

public partial class Plugin
{
	private void Player_Glide(On.Player.orig_Update orig, Player self, bool eu)
	{
		orig(self, eu);
		CentiwingPlayerData centiwing = self.Centiwing();
		if (!centiwing.IsCentiwing) return;

		/* Logger.LogInfo("IS GLIDING: " + centiwing.isGliding.ToString()); */

		if (centiwing.isGliding)
		{
         centiwing.currentGlideDuration++;

			self.customPlayerGravity = 0f;

			for (int i = 0; i < self.bodyChunks.Length; i++)
			{
				self.bodyChunks[i].vel.y = Mathf.Lerp(
					self.bodyChunks[i].vel.y,
					CentiwingPlayerData.glideFallVelocity,
					centiwing.currentGlideDuration / CentiwingPlayerData.glideKickIn
				);
			}

			if (!centiwing.CanGlide())
			{
				centiwing.isGliding = false;

				self.customPlayerGravity = self.room.gravity;
			}
		}
		else if (centiwing.CanGlide())
		{
			centiwing.isGliding = true;

			centiwing.currentGlideDuration = 0;
		}
	}
}