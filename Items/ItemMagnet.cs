using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ItemMagnetPlus.Items
{
    public class ItemMagnet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Item Magnet");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = Item.sellPrice(silver: 36);
            item.rare = 2;
            item.useAnimation = 10;
            item.useTime = 10;
            item.useStyle = 4;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            ItemMagnetPlusPlayer mPlayer = Main.LocalPlayer.GetModPlayer<ItemMagnetPlusPlayer>(mod);

            tooltips.Add(new TooltipLine(mod, "Buffa", "Left Click to " + ((mPlayer.clientConf.Scale == 1) ? "[c/80FF80:change range ]" : "[c/80FF80:toggle on/off ]")));
            tooltips.Add(new TooltipLine(mod, "Buffb", "Right Click to " + ((mPlayer.clientConf.Buff == 1) ? "[c/9999FF:show current range ]" : "[c/FF8080:turn off ]")));

            if (Main.LocalPlayer.HasBuff(mod.BuffType("ItemMagnetBuff")) || Main.LocalPlayer.GetModPlayer<ItemMagnetPlusPlayer>(mod).magnetActive == 1)
            {
                mPlayer.UpdateMagnetValues(mPlayer.magnetGrabRadius);
                tooltips.Add(new TooltipLine(mod, "Range", "Current Range: " + mPlayer.magnetGrabRadius));
                tooltips.Add(new TooltipLine(mod, "Velocity", "Current Velocity: " + mPlayer.magnetVelocity));
                tooltips.Add(new TooltipLine(mod, "Acceleration", "Current Acceleration: " + mPlayer.magnetAcceleration));
            }
            else if (Main.LocalPlayer.HasItem(mod.ItemType("ItemMagnet")))
            {
                tooltips.Add(new TooltipLine(mod, "Range", "Magnet is off"));
            }
            //If player has buff, then he automatically also has the item
            //If player doesn't have the buff, he can still have the item, just not activated
        }

        public override void AddRecipes()
        {
            //iron/lead
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("IronBar", 12);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public static Dust QuickDust(Vector2 pos, Color color)
        {
            int type = 1;
            Dust dust = Main.dust[Dust.NewDust(pos, 4, 4, type, 0f, 0f, 120, color, 2f)];
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

        private void DrawRectangle(ItemMagnetPlusPlayer mPlayer, int radius, Color color)
        {
            int stage = radius / (mPlayer.magnetScreenRadius * 16);
            radius = radius - mPlayer.magnetScreenRadius * 16 * stage;
            float fullhdradius = radius * 0.5625f;
            color = new Color(color.R + stage * 30, color.G, color.B - stage * 30);

            //before: radius in world coordinates
            Vector2 pos = mPlayer.player.position;
            float leftx = pos.X - radius;
            float topy = pos.Y - fullhdradius;
            float rightx = leftx + mPlayer.player.width + radius * 2;
            float boty = topy + mPlayer.player.height + fullhdradius * 2;

            //after radius in tile coordinates
            QuickDustLine(new Vector2(leftx, topy), new Vector2(rightx, topy), radius / 16f, color); //clock wise starting top left
            QuickDustLine(new Vector2(rightx, topy), new Vector2(rightx, boty), fullhdradius / 16f, color);
            QuickDustLine(new Vector2(rightx, boty), new Vector2(leftx, boty), radius / 16f, color);
            QuickDustLine(new Vector2(leftx, boty), new Vector2(leftx, topy), fullhdradius / 16f, color);
        }

        public override bool UseItem(Player player)
        {
            ItemMagnetPlusPlayer mPlayer = player.GetModPlayer<ItemMagnetPlusPlayer>(mod);
            mPlayer.UpdateMagnetValues(mPlayer.magnetGrabRadius);

            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                //right click feature only shows the range
                if (player.altFunctionUse == 2)
                {
                    if(mPlayer.magnetActive == 0)
                    {
                        //nothing
                        CombatText.NewText(player.getRect(), CombatText.DamagedFriendly, "magnet is off");
                    }
                    else if(player.HasBuff(mod.BuffType("ItemMagnetBuff")))
                    {
                        //shows the range
                        DrawRectangle(mPlayer, mPlayer.magnetGrabRadius * 16, CombatText.HealMana);
                        CombatText.NewText(player.getRect(), CombatText.HealMana, "range:" + mPlayer.magnetGrabRadius);
                    }
                    else
                    {
                        //deactivates
                        mPlayer.DeactivateMagnet(player);
                        CombatText.NewText(player.getRect(), CombatText.DamagedFriendly, "magnet off");
                    }
                }
                else //if (player.altFunctionUse != 2)
                {
                    int divider = (Main.hardMode || mPlayer.magnetGrabRadius >= mPlayer.magnetScreenRadius) ? 10 : 5;
                    int radius = mPlayer.magnetGrabRadius;

                    if (mPlayer.magnetActive == 0)
                    {
                        mPlayer.ActivateMagnet(player);

                        Main.PlaySound(SoundID.MaxMana, player.position, 1);
                        mPlayer.magnetActive = 1;
                        mPlayer.UpdateMagnetValues(mPlayer.magnetMinGrabRadius);
                        radius = mPlayer.magnetGrabRadius;
                        divider = (Main.hardMode || mPlayer.magnetGrabRadius >= mPlayer.magnetScreenRadius) ? 10 : 5; //duplicate because need updated value
                        DrawRectangle(mPlayer, mPlayer.magnetGrabRadius * 16, new Color(200, 255, 200));

                        string ranges = "range:" + radius;
                        if (radius + divider > mPlayer.magnetMaxGrabRadius)
                        {
                            ranges += "| next:off";
                        }
                        else
                        {
                            ranges += "| next:" + (radius + divider);
                        }
                        
                        CombatText.NewText(player.getRect(), CombatText.HealLife, ranges);
                    }
                    else
                    {
                        radius += divider;

                        if (radius > mPlayer.magnetMaxGrabRadius)
                        {
                            CombatText.NewText(player.getRect(), CombatText.DamagedFriendly, "magnet off");
                            Main.PlaySound(SoundID.MaxMana, player.position, 1);
                            mPlayer.DeactivateMagnet(player);
                            return true;
                        }

                        mPlayer.UpdateMagnetValues(radius);
                        DrawRectangle(mPlayer, mPlayer.magnetGrabRadius * 16, new Color(200, 255, 200));

                        //here radius is already + divider
                        string ranges = "range:" + radius;
                        if (radius + divider > mPlayer.magnetMaxGrabRadius)
                        {
                            ranges += "| next:off";
                        }
                        else
                        {
                            ranges += "| next:" + (radius + divider);
                        }
                        CombatText.NewText(player.getRect(), new Color(128, 255, 128), ranges);
                    }
                }
            }
            return true;
        }
    }
}