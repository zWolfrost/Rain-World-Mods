using System.IO;
using System.Runtime.CompilerServices;
#pragma warning disable CS0618

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

[BepInPlugin("zWolfrost.Centiwing", "Centiwing", "1.0.0")]
public partial class Plugin : BaseUnityPlugin
{
	private void OnEnable()
	{
		On.RainWorld.OnModsInit += RainWorldOnOnModsInit;
	}

	public static Texture2D TailTexture;

	private bool IsInit;
	private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
	{
		orig(self);
		if (IsInit) return;

		try
		{
			IsInit = true;

			Futile.atlasManager.LoadAtlas("atlases/centiwing_antennae");
			Futile.atlasManager.LoadAtlas("atlases/centiwing_wings");
			Futile.atlasManager.LoadAtlas("atlases/centiwing_suction");
			Futile.atlasManager.LoadAtlas("atlases/centiwing_chest");

			On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
			On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
			On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
			On.PlayerGraphics.Update += PlayerGraphics_Update;

			On.Player.Update += Player_Glide;
			On.Player.Update += Player_Shock;
		}
		catch (Exception ex)
		{
			Logger.LogError(ex);
		}
	}
}

public partial class CentiwingPlayerData(Player player)
{
	public static SlugcatStats.Name CentiwingName = new("centiwing");

   public readonly bool IsCentiwing = player.slugcatStats.name == CentiwingName;

   public readonly Player player = player;
}

public static class PlayerExtension
{
	private static readonly ConditionalWeakTable<Player, CentiwingPlayerData> _cwt = new();

	public static CentiwingPlayerData Centiwing(this Player player) => _cwt.GetValue(player, _ => new CentiwingPlayerData(player));
}
