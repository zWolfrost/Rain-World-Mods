public partial class CentiwingPlayerData
{
	public const int addedSprites = 26;

	public const string centiwingGreen = "50aa51";


	// Wings //
	public const float wingLength = 0.6f;
	public const float offsetWingsTop = 10f;
	public const float offsetWingsBottom = 26f;
	public const float distanceWingsTop = 2f;
	public const float distanceWingsBottom = 4f;
	public const float wingCrouchVertAdj = 2f;

	public const float rotationWingsTop = -70f;
	public const float rotationWingsBottom = 65f;
	public const float rotationWingsTopMin = 60f;
	public const float rotationWingsTopMax = 160f;
	public const float rotationWingsBottomMin = -90f;
	public const float rotationWingsBottomMax = 0f;

	public const string wingColor = "FFFFFF";

	public int firstWingSprite;
	public float[,] wingDeployment = new float[2, 2];
   public float[,] wingDeploymentSpeed = new float[2, 2];
	public float wingDeploymentGetTo;
	public float wingOffset;
	public float wingTimeAdd;
	public Vector2 zRotation;
	public Vector2 lastZRotation;
	public float wingYAdjust;

	public int WingSprite(int side, int wing) => firstWingSprite + side + wing + wing;


	// Antennae //
	public const float antennaeOffsetX = 0f;
	public const float antennaeOffsetY = 0f;

	public const string antennaeColor = "000900";

	public int firstAntennaeSprite;


	// Tail //
	public const int tailSuctionRows = 5;
	public const int tailSuctionLines = 3;
	public const int tailSuctionNumber = tailSuctionRows * tailSuctionLines;

	public const string tailSuctionColor = centiwingGreen;

	public int firstTailSuctionSprite;


	// Chest //
	public const float chestOffset = 1.25f;
	public const float chestScaleX = 0.75f;

	public const int chestNumber = 3;
	public const string chestColor = "1f1f1f";

	public int firstChestSprite;


	// Scales //
	public const int scalesNumber = 3;
	public const string scalesColor = "0c0d0e";

	public int firstScalesSprite;
}

public partial class Plugin
{
	private void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
	{
		orig(self, sLeaser, rCam);
		CentiwingPlayerData centiwing = self.player.Centiwing();
		if (!centiwing.IsCentiwing) return;

		centiwing.firstWingSprite = sLeaser.sprites.Length;
      centiwing.firstAntennaeSprite = centiwing.firstWingSprite + 4;
		centiwing.firstTailSuctionSprite = centiwing.firstAntennaeSprite + 1;
		centiwing.firstChestSprite = centiwing.firstTailSuctionSprite + CentiwingPlayerData.tailSuctionNumber;
		centiwing.firstScalesSprite = centiwing.firstChestSprite + CentiwingPlayerData.chestNumber;

		Array.Resize(ref sLeaser.sprites, sLeaser.sprites.Length + CentiwingPlayerData.addedSprites);

		// Wings //
		for (var i = 0; i < 2; i++)
		{
			for (var j = 0; j < 2; j++)
			{
				sLeaser.sprites[centiwing.WingSprite(i, j)] = new FSprite("centiwing_wings_" + (j == 0 ? "top" : "bottom")){ anchorX = 0f, scaleY = 1f };
			}
		}

		// Antennae //
      sLeaser.sprites[centiwing.firstAntennaeSprite] = new FSprite("centiwing_antennae_front");

		// Tail //
		for (int i = 0; i < CentiwingPlayerData.tailSuctionRows; i++)
		{
			for (int j = 0; j < CentiwingPlayerData.tailSuctionLines; j++)
			{
				sLeaser.sprites[centiwing.firstTailSuctionSprite + i * CentiwingPlayerData.tailSuctionLines + j] = new FSprite("centiwing_suction");
			}
		}

		// Chest //
		for (int i = 0; i < CentiwingPlayerData.chestNumber; i++)
		{
			sLeaser.sprites[centiwing.firstChestSprite + i] = new FSprite("centiwing_chest");
		}

		// Scales //
		for (int i = 0; i < CentiwingPlayerData.scalesNumber; i++)
		{
			sLeaser.sprites[centiwing.firstScalesSprite + i] = new FSprite("centiwing_scales");
		}

		self.AddToContainer(sLeaser, rCam, null);
	}

	private void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
	{
		orig(self, sLeaser, rCam, newContatiner);
		CentiwingPlayerData centiwing = self.player.Centiwing();
		if (!centiwing.IsCentiwing) return;

		/* Logger.LogInfo("AddToContainer - " + sLeaser.sprites.Length + " - " + centiwing.firstWingSprite); */

		// There's probably a better way to check for this, but this will do fine
		if (centiwing.firstWingSprite > 0 && sLeaser.sprites.Length >= centiwing.firstWingSprite + CentiwingPlayerData.addedSprites)
		{
			newContatiner ??= rCam.ReturnFContainer("Midground");

			// Wings //
			for (var i = 0; i < 2; i++)
			{
				for (var j = 0; j < 2; j++)
				{
					var sprite = sLeaser.sprites[centiwing.WingSprite(i, j)];
					newContatiner.AddChild(sprite);
					sprite.MoveBehindOtherNode(sLeaser.sprites[0]);
				}
			}

			// Antennae //
			newContatiner.AddChild(sLeaser.sprites[centiwing.firstAntennaeSprite]);
         sLeaser.sprites[centiwing.firstAntennaeSprite].MoveBehindOtherNode(sLeaser.sprites[centiwing.firstWingSprite]);

			// Tail //
			for (int i = 0; i < CentiwingPlayerData.tailSuctionNumber; i++)
			{
				newContatiner.AddChild(sLeaser.sprites[centiwing.firstTailSuctionSprite + i]);
			}

			// Chest //
			for (int i = 0; i < CentiwingPlayerData.chestNumber; i++)
			{
				newContatiner.AddChild(sLeaser.sprites[centiwing.firstChestSprite + i]);
			}

			// Scales //
			for (int i = 0; i < CentiwingPlayerData.scalesNumber; i++)
			{
				var sprite = sLeaser.sprites[centiwing.firstScalesSprite + i];
				newContatiner.AddChild(sprite);
				sprite.MoveBehindOtherNode(sLeaser.sprites[centiwing.firstChestSprite]);
			}
		}
	}

	private void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
	{
		orig(self, sLeaser, rCam, timeStacker, camPos);
		CentiwingPlayerData centiwing = self.player.Centiwing();
		if (!centiwing.IsCentiwing) return;

		var animationOffset = GetAnimationOffset(self);

		Vector2 bodyLerp = Vector2.Lerp(self.player.bodyChunks[1].lastPos, self.player.bodyChunks[1].pos, timeStacker);
		Vector2 headLerp = Vector2.Lerp(self.player.bodyChunks[0].lastPos, self.player.bodyChunks[0].pos, timeStacker);
		Vector2 rotLerp = Vector3.Slerp(centiwing.lastZRotation, centiwing.zRotation, timeStacker);


		// Wings // Borrowed from the beecat mod. I have no intention of cleaning it up. I'm sorry.
		{
			var playerPos = Vector2.Lerp(bodyLerp, headLerp, 0.5f);
			var normalized = (bodyLerp - headLerp).normalized;
			var perpVector = Custom.PerpendicularVector(-normalized);
			var num = Custom.AimFromOneVectorToAnother(-normalized, normalized);

			for (var i = 0; i < 2; i++)
			{
				for (var j = 0; j < 2; j++)
				{
					var off = (j == 0 ? CentiwingPlayerData.offsetWingsTop : CentiwingPlayerData.offsetWingsBottom) - 20 + 3f * Mathf.Abs(rotLerp.x);
					var dist = (j == 0 ? CentiwingPlayerData.distanceWingsTop : CentiwingPlayerData.distanceWingsBottom) * Mathf.Lerp(1f, 0.85f, Mathf.InverseLerp(0.5f, 0f, centiwing.wingDeployment[i, j]));
					var wingPos = playerPos + normalized * off + perpVector * dist * (i == 0 ? -1f : 1f) + perpVector * rotLerp.x * Mathf.Lerp(-3f, -5f, Mathf.InverseLerp(0.5f, 0f, centiwing.wingDeployment[i, j]));

					sLeaser.sprites[centiwing.WingSprite(i, j)].x = wingPos.x - camPos.x;
					sLeaser.sprites[centiwing.WingSprite(i, j)].y = wingPos.y - camPos.y;

					if (Mathf.Abs(num) < 105) centiwing.wingYAdjust = Mathf.Lerp(centiwing.wingYAdjust, CentiwingPlayerData.wingCrouchVertAdj, 0.05f);
					else centiwing.wingYAdjust = Mathf.Lerp(centiwing.wingYAdjust, 0, 0.05f);

					sLeaser.sprites[centiwing.WingSprite(i, j)].x += animationOffset.x;
					sLeaser.sprites[centiwing.WingSprite(i, j)].y += centiwing.wingYAdjust + animationOffset.y;

					sLeaser.sprites[centiwing.WingSprite(i, j)].alpha = 0.6f;
					sLeaser.sprites[centiwing.WingSprite(i, j)].color = Custom.hexToColor(CentiwingPlayerData.wingColor);

					if (centiwing.wingDeployment[i, j] == 1f)
					{
						float num10;
						float rotationMin;
						float rotationMax;
						if (j == 0)
						{
							num10 = Mathf.Pow(Custom.Decimal(centiwing.wingOffset + Mathf.InverseLerp(0f, 3f, centiwing.wingTimeAdd + timeStacker)), 0.75f);
							rotationMin = CentiwingPlayerData.rotationWingsTopMin;
							rotationMax = CentiwingPlayerData.rotationWingsTopMax;
						}
						else
						{
							num10 = Mathf.Pow(Custom.Decimal(centiwing.wingOffset + Mathf.InverseLerp(0f, 3f, centiwing.wingTimeAdd + timeStacker) + 0.8f), 1.3f);
							rotationMin = CentiwingPlayerData.rotationWingsBottomMin;
							rotationMax = CentiwingPlayerData.rotationWingsBottomMax;
						}
						num10 = Mathf.Pow(0.5f + 0.5f * Mathf.Sin(num10 * Mathf.PI * 2f), 0.7f);
						num10 = Mathf.Lerp(rotationMin, rotationMax, num10);

						sLeaser.sprites[centiwing.WingSprite(i, j)].scaleX = Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(Mathf.Abs(rotLerp.y), 1f, Mathf.Abs(0.5f - num10) * 1.4f)), 1f) * (i == 0 ? 1f : -1f) * CentiwingPlayerData.wingLength;
						sLeaser.sprites[centiwing.WingSprite(i, j)].rotation = num - 180f + ((j == 0 ? -20f : 24f) - 20 + num10) * (i == 0 ? 1f : -1f);
					}
					else
					{
						float rotation = (j == 0) ? CentiwingPlayerData.rotationWingsTop : CentiwingPlayerData.rotationWingsBottom;

						sLeaser.sprites[centiwing.WingSprite(i, j)].scaleX = (i == 0 ? 1f : -1f) * CentiwingPlayerData.wingLength;
						sLeaser.sprites[centiwing.WingSprite(i, j)].rotation = Custom.AimFromOneVectorToAnother(bodyLerp, wingPos) - rotation * (i == 0 ? 1f : -1f);
					}
				}
			}
		}


		// Antennae // Borrowed from the beecat mod.
		{
			var antennaePos = new Vector2(sLeaser.sprites[3].x + CentiwingPlayerData.antennaeOffsetX, sLeaser.sprites[3].y + CentiwingPlayerData.antennaeOffsetY);

			sLeaser.sprites[centiwing.firstAntennaeSprite].scaleX = sLeaser.sprites[3].scaleX * 1.3f;
			sLeaser.sprites[centiwing.firstAntennaeSprite].scaleY = 1.3f;
			sLeaser.sprites[centiwing.firstAntennaeSprite].rotation = sLeaser.sprites[3].rotation;
			sLeaser.sprites[centiwing.firstAntennaeSprite].x = antennaePos.x;
			sLeaser.sprites[centiwing.firstAntennaeSprite].y = antennaePos.y;
			sLeaser.sprites[centiwing.firstAntennaeSprite].element = Futile.atlasManager.GetElementWithName("centiwing_antennae_front");
			sLeaser.sprites[centiwing.firstAntennaeSprite].color = Custom.hexToColor(CentiwingPlayerData.antennaeColor);
		}


		// Tail // Borrowed from PlayerGraphics.TailSpeckles
		{
			for (int i = 0; i < CentiwingPlayerData.tailSuctionRows; i++)
			{
				float f = Mathf.InverseLerp(0f, CentiwingPlayerData.tailSuctionRows - 1, i);
				float s = Mathf.Lerp(0.1f, 0.95f, Mathf.Pow(f, 0.8f));
				PlayerGraphics.PlayerSpineData playerSpineData = self.SpinePosition(s, timeStacker);
				for (int j = 0; j < CentiwingPlayerData.tailSuctionLines; j++)
				{
					int curSprite = centiwing.firstTailSuctionSprite + i * CentiwingPlayerData.tailSuctionLines + j;

					float num3 = (j + ((i % 2 != 0) ? 0f : 0.5f)) / (CentiwingPlayerData.tailSuctionLines - 1);
					num3 = -1f + 2f * num3;
					if (num3 < -1f) num3 += 2f;
					else if (num3 > 1f) num3 -= 2f;

					float scale_center = (j == 1 && i % 2 != 0) ? 1.5f : 0.5f;

					num3 = Mathf.Sign(num3) * Mathf.Pow(Mathf.Abs(num3), 0.6f);
					Vector2 vector = playerSpineData.pos + playerSpineData.perp * (playerSpineData.rad + 0.5f) * num3;

					sLeaser.sprites[curSprite].x = vector.x - camPos.x;
					sLeaser.sprites[curSprite].y = vector.y - camPos.y;
					sLeaser.sprites[curSprite].color = Custom.hexToColor(CentiwingPlayerData.tailSuctionColor);
					sLeaser.sprites[curSprite].rotation = Custom.VecToDeg(playerSpineData.dir);
					sLeaser.sprites[curSprite].scaleX = Custom.LerpMap(Mathf.Abs(num3), 0.4f, 1f, 1f, 0f) * scale_center;
					sLeaser.sprites[curSprite].scaleY = 1f * scale_center;
				}
			}
		}


		// Chest & Scales // Borrowed from PlayerGraphics.CosmeticPearl
		{
			void ChestTranslate(int curSprite, int i)
			{
				Vector2 vector = Vector2.Lerp((self.owner.bodyChunks[0].lastPos + self.owner.bodyChunks[1].lastPos) / 2f, (self.owner.bodyChunks[0].pos + self.owner.bodyChunks[1].pos) / 2f, timeStacker);
				vector -= (i - CentiwingPlayerData.chestOffset) * 5.2f * (bodyLerp - headLerp).normalized;

				sLeaser.sprites[curSprite].x = vector.x - camPos.x;
				sLeaser.sprites[curSprite].y = vector.y - camPos.y;
				Vector2 normalized = self.player.mainBodyChunk.vel.normalized;
				sLeaser.sprites[curSprite].x += self.lookDirection.x + normalized.x;
				sLeaser.sprites[curSprite].y += self.lookDirection.y + normalized.y;
				sLeaser.sprites[curSprite].rotation = Custom.AimFromOneVectorToAnother(self.owner.bodyChunks[1].pos, self.owner.bodyChunks[0].pos);

				if (self.player.bodyMode == Player.BodyModeIndex.Crawl) sLeaser.sprites[curSprite].scaleX = 0.5f;
				else if (self.player.mainBodyChunk.vel.x != 0f) sLeaser.sprites[curSprite].scaleX = Custom.LerpMap(Mathf.Abs(self.player.mainBodyChunk.vel.x), 0f, 4f, 1f, 0.75f);
				else sLeaser.sprites[curSprite].scaleX = 1f;

				sLeaser.sprites[curSprite].scaleX *= 1f - Mathf.Abs(self.lookDirection.x) * 0.35f;
				sLeaser.sprites[curSprite].scaleX *= CentiwingPlayerData.chestScaleX;
			}

			for (int i = 0; i < CentiwingPlayerData.chestNumber; i++)
			{
				int curSprite = centiwing.firstChestSprite + i;
				ChestTranslate(curSprite, i);
				sLeaser.sprites[curSprite].color = Custom.hexToColor(CentiwingPlayerData.chestColor);
			}

			for (int i = 0; i < CentiwingPlayerData.scalesNumber; i++)
			{
				int curSprite = centiwing.firstScalesSprite + i;
				ChestTranslate(curSprite, i);
				sLeaser.sprites[curSprite].color = Custom.hexToColor(CentiwingPlayerData.scalesColor);
			}
		}
	}

	public static Vector2 GetAnimationOffset(PlayerGraphics self)
	{
		var result = Vector2.zero;

		if (self.player.bodyMode == Player.BodyModeIndex.Stand)
		{
			result.x += self.player.flipDirection * (self.RenderAsPup ? 2f : 6f) * Mathf.Clamp(Mathf.Abs(self.owner.bodyChunks[1].vel.x) - 0.2f, 0f, 1f) * 0.3f;
			result.y += Mathf.Cos((self.player.animationFrame + 0f) / 6f * 2f * Mathf.PI) * (self.RenderAsPup ? 1.5f : 2f) * 0.3f;
		}
		else if (self.player.bodyMode == Player.BodyModeIndex.Crawl)
		{
			var num4 = Mathf.Sin(self.player.animationFrame / 21f * 2f * Mathf.PI);
			var num5 = Mathf.Cos(self.player.animationFrame / 14f * 2f * Mathf.PI);
			result.x += num5 * self.player.flipDirection * 2f;
			result.y -= num4 * -1.5f - 3f;
		}
		else if (self.player.bodyMode == Player.BodyModeIndex.ClimbingOnBeam)
		{
			if (self.player.animation == Player.AnimationIndex.ClimbOnBeam)
			{
				result.x += self.player.flipDirection * 2.5f + self.player.flipDirection * 0.5f * Mathf.Sin(self.player.animationFrame / 20f * Mathf.PI * 2f);
			}
		}
		else if (self.player.bodyMode == Player.BodyModeIndex.WallClimb)
		{
			result.y += 2f;
			result.x -= self.player.flipDirection * (self.owner.bodyChunks[1].ContactPoint.y < 0 ? 3f : 5f);
		}
		else if (self.player.bodyMode == Player.BodyModeIndex.Default)
		{
			if (self.player.animation == Player.AnimationIndex.LedgeGrab)
			{
				result.x -= self.player.flipDirection * 5f;
			}
		}
		else if (self.player.animation == Player.AnimationIndex.CorridorTurn)
		{
			result += Custom.DegToVec(Random.value * 360f) * 3f * Random.value;
		}

		return result;
	}

	private void PlayerGraphics_Update(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
	{
		orig(self);
		CentiwingPlayerData centiwing = self.player.Centiwing();
		if (!centiwing.IsCentiwing) return;

		if (centiwing.isGliding) centiwing.wingDeploymentGetTo = 1f;
		else centiwing.wingDeploymentGetTo = 0.9f;

		centiwing.lastZRotation = centiwing.zRotation;
		centiwing.zRotation = Vector2.Lerp(centiwing.zRotation, Custom.DirVec(self.player.bodyChunks[1].pos, self.player.bodyChunks[0].pos), 0.15f);
		centiwing.zRotation = centiwing.zRotation.normalized;

		for (var i = 0; i < 2; i++)
		{
			for (var j = 0; j < 2; j++)
			{
				if (self.player.Consious)
				{
					if (Random.value < 0.033333335f)
					{
						centiwing.wingDeploymentSpeed[i, j] = 0.6f;
					}
					else if (centiwing.wingDeployment[i, j] < centiwing.wingDeploymentGetTo)
					{
						centiwing.wingDeployment[i, j] = Mathf.Min(centiwing.wingDeployment[i, j] + centiwing.wingDeploymentSpeed[i, j], centiwing.wingDeploymentGetTo);
					}
					else if (centiwing.wingDeployment[i, j] > centiwing.wingDeploymentGetTo)
					{
						centiwing.wingDeployment[i, j] = Mathf.Max(centiwing.wingDeployment[i, j] - centiwing.wingDeploymentSpeed[i, j], centiwing.wingDeploymentGetTo);
					}
				}
				else if (centiwing.wingDeployment[i, j] == 1f)
				{
					centiwing.wingDeployment[i, j] = 0.9f;
				}
			}
		}

		centiwing.wingOffset += 1f / Random.Range(50, 60);
		centiwing.wingTimeAdd += 1f;
		if (centiwing.wingTimeAdd >= 3f)
		{
			centiwing.wingTimeAdd = 0f;
		}
	}
}