using ItemMagnetPlus.Buffs;
using ItemMagnetPlus.Dusts;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ItemMagnetPlus.Items
{
	public class ItemMagnet : ModItem
	{
		public static LocalizedText CurrentRangeText { get; private set; }
		public static LocalizedText CurrentVelocityText { get; private set; }
		public static LocalizedText CurrentAccelerationText { get; private set; }

		public static LocalizedText LeftClickToChangeRangeText { get; private set; }
		public static LocalizedText LeftClickToToggleText { get; private set; }
		public static LocalizedText RightClickToShowCurrentRangeText { get; private set; }
		public static LocalizedText RightClickToTurnOffText { get; private set; }
		public static LocalizedText MagnetIsOffText { get; private set; }

		public static LocalizedText RangeText { get; private set; }
		public static LocalizedText NextRangeText { get; private set; }
		public static LocalizedText MagnetOffText { get; private set; }
		public static LocalizedText OffText { get; private set; }

		public override LocalizedText Tooltip => LocalizedText.Empty;

		public override void SetStaticDefaults()
		{
			CurrentRangeText = this.GetLocalization("CurrentRange");
			CurrentVelocityText = this.GetLocalization("CurrentVelocity");
			CurrentAccelerationText = this.GetLocalization("CurrentAcceleration");

			LeftClickToChangeRangeText = this.GetLocalization("LeftClickToChangeRange");
			LeftClickToToggleText = this.GetLocalization("LeftClickToToggle");
			RightClickToShowCurrentRangeText = this.GetLocalization("RightClickToShowCurrentRange");
			RightClickToTurnOffText = this.GetLocalization("RightClickToTurnOff");
			MagnetIsOffText = this.GetLocalization("MagnetIsOff");

			RangeText = this.GetLocalization("Range");
			NextRangeText = this.GetLocalization("NextRange");
			MagnetOffText = this.GetLocalization("MagnetOff");
			OffText = this.GetLocalization("Off");
		}

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 32;
			Item.value = Item.sellPrice(silver: 36);
			Item.rare = ItemRarityID.Green;
			Item.useAnimation = 10;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.HoldUp;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Player player = Main.LocalPlayer;
			ItemMagnetPlusPlayer mPlayer = player.GetModPlayer<ItemMagnetPlusPlayer>();

			float alpha = Main.mouseTextColor / 255f;
			string color1 = (new Color(128, 255, 128) * alpha).Hex3();
			string color2 = (new Color(159, 159, 255) * alpha).Hex3();
			string color3 = (new Color(255, 128, 128) * alpha).Hex3();
			tooltips.Add(new TooltipLine(Mod, "Buffa", (Config.Instance.ScaleMode == Config.ScaleModeType.Bosses ? LeftClickToChangeRangeText : LeftClickToToggleText).Format(color1)));
			tooltips.Add(new TooltipLine(Mod, "Buffb", Config.Instance.Buff ? RightClickToShowCurrentRangeText.Format(color2) : RightClickToTurnOffText.Format(color3)));

			if (player.HasBuff(ModContent.BuffType<ItemMagnetBuff>()) || mPlayer.magnetActive == 1)
			{
				mPlayer.UpdateMagnetValues(mPlayer.magnetGrabRadius);
				tooltips.Add(new TooltipLine(Mod, "Range", CurrentRangeText.Format(mPlayer.magnetGrabRadius)));
				tooltips.Add(new TooltipLine(Mod, "Velocity", CurrentVelocityText.Format(mPlayer.magnetVelocity)));
				tooltips.Add(new TooltipLine(Mod, "Acceleration", CurrentAccelerationText.Format(mPlayer.magnetAcceleration)));
			}
			else if (player.HasItem(Item.type))
			{
				tooltips.Add(new TooltipLine(Mod, "Range", MagnetIsOffText.ToString()));
			}
			// If player has buff, then he automatically also has the item
			// If player doesn't have the buff, he can still have the item, just not activated
		}

		public override void AddRecipes()
		{
			CreateRecipe(1).AddRecipeGroup(RecipeGroupID.IronBar, 12).AddTile(TileID.Anvils).Register();
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public static Dust QuickDust(Vector2 pos, Color color)
		{
			int type = ModContent.DustType<ColorableDustAlphaFade>();
			Dust dust = Dust.NewDustDirect(pos, 4, 4, type, 0f, 0f, 100, color, 1f);
			dust.customData = new InAndOutData(reduceScale: false);
			dust.position = pos;
			dust.velocity = Vector2.Zero;
			dust.fadeIn = 3f;
			dust.noLight = true;
			dust.noGravity = true;
			return dust;
		}

		public static void QuickDustLine(Vector2 start, Vector2 end, float splits, Color color)
		{
			QuickDust(start, color);
			float num = 1f / splits;
			for (float num2 = 0f; num2 < 1f; num2 += num)
			{
				QuickDust(Vector2.Lerp(start, end, num2), color);
			}
		}

		public static void DrawRectangle(Player player, int radius, Color color)
		{
			float fullhdradius = radius * 0.5625f;

			Vector2 pos = player.position;
			float leftx = pos.X - radius;
			float topy = pos.Y - fullhdradius;
			float rightx = leftx + player.width + radius * 2;
			float boty = topy + player.height + fullhdradius * 2;

			QuickDustLine(new Vector2(leftx, topy), new Vector2(rightx, topy), radius / 16f, color); //clock wise starting top left
			QuickDustLine(new Vector2(rightx, topy), new Vector2(rightx, boty), fullhdradius / 16f, color);
			QuickDustLine(new Vector2(rightx, boty), new Vector2(leftx, boty), radius / 16f, color);
			QuickDustLine(new Vector2(leftx, boty), new Vector2(leftx, topy), fullhdradius / 16f, color);
		}

		public override bool? UseItem(Player player)
		{
			ItemMagnetPlusPlayer mPlayer = player.GetModPlayer<ItemMagnetPlusPlayer>();
			mPlayer.UpdateMagnetValues(mPlayer.magnetGrabRadius);

			//string s = $"{Main.time} {player.itemTime} {player.itemAnimation}";
			//if (Main.netMode == NetmodeID.Server)
			//{
			//    System.Console.WriteLine(s);
			//}
			//else
			//{
			//    Main.NewText(s);
			//}

			if (player.whoAmI == Main.myPlayer /*&& player.itemTime == 0*/)
			{
				// Right click feature only shows the range
				if (player.altFunctionUse == 2)
				{
					if (mPlayer.magnetActive == 0)
					{
						// Nothing
						CombatText.NewText(player.getRect(), CombatText.DamagedFriendly, MagnetIsOffText.ToString());
					}
					else if (Config.Instance.Buff && player.HasBuff(ModContent.BuffType<ItemMagnetBuff>()))
					{
						// Shows the range
						DrawRectangle(player, mPlayer.magnetGrabRadius * 16, CombatText.HealMana);
						CombatText.NewText(player.getRect(), CombatText.HealMana, RangeText.Format(mPlayer.magnetGrabRadius));
					}
					else
					{
						// Deactivates
						mPlayer.DeactivateMagnet();
						CombatText.NewText(player.getRect(), CombatText.DamagedFriendly, MagnetOffText.ToString());
					}
				}
				else //if (player.altFunctionUse != 2)
				{
					int divider = (Main.hardMode || mPlayer.magnetGrabRadius >= mPlayer.magnetScreenRadius) ? 10 : 5;
					int radius = mPlayer.magnetGrabRadius;

					if (mPlayer.magnetActive == 0)
					{
						mPlayer.ActivateMagnet();

						SoundEngine.PlaySound(SoundID.MaxMana, player.Center);
						mPlayer.magnetActive = 1;
						mPlayer.UpdateMagnetValues(mPlayer.magnetMinGrabRadius);
						radius = mPlayer.magnetGrabRadius;
						divider = (Main.hardMode || mPlayer.magnetGrabRadius >= mPlayer.magnetScreenRadius) ? 10 : 5; //duplicate because need updated value
						DrawRectangle(player, mPlayer.magnetGrabRadius * 16, new Color(200, 255, 200));

						int shownRadius = radius + divider;
						string shownRadiusStr = shownRadius > mPlayer.magnetMaxGrabRadius ? OffText.ToString() : shownRadius.ToString();
						string ranges = $"{RangeText.Format(radius)}{NextRangeText.Format(shownRadiusStr)}";
						CombatText.NewText(player.getRect(), CombatText.HealLife, ranges);
					}
					else
					{
						radius += divider;

						if (radius > mPlayer.magnetMaxGrabRadius)
						{
							CombatText.NewText(player.getRect(), CombatText.DamagedFriendly, MagnetOffText.ToString());
							SoundEngine.PlaySound(SoundID.MaxMana, player.Center);
							mPlayer.DeactivateMagnet();
							return true;
						}

						mPlayer.UpdateMagnetValues(radius);
						DrawRectangle(player, mPlayer.magnetGrabRadius * 16, new Color(200, 255, 200));

						// Here radius is already + divider

						int shownRadius = radius + divider;
						string shownRadiusStr = shownRadius > mPlayer.magnetMaxGrabRadius ? OffText.ToString() : shownRadius.ToString();
						string ranges = $"{RangeText.Format(radius)}{NextRangeText.Format(shownRadiusStr)}";
						CombatText.NewText(player.getRect(), new Color(128, 255, 128), ranges);
					}
				}
			}
			return true;
		}
	}

	//public class MyG : GlobalItem
	//{
	//    public override bool UseItem(Item item, Player player)
	//    {
	//        string s = $"{Main.time} {player.itemTime} {player.itemAnimation}";
	//        if (Main.netMode == NetmodeID.Server)
	//        {
	//            System.Console.WriteLine(s);
	//        }
	//        else
	//        {
	//            Main.NewText(s);
	//        }

	//        return false;
	//    }
	//}
}
