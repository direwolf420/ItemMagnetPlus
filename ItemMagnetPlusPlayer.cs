using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ItemMagnetPlus
{
    public class ItemMagnetPlusPlayer : ModPlayer
    {
        public int magnetActive = 0;
        public int magnetScreenRadius = 60 + 1;
        public int magnetGrabRadius = 10;
        public int magnetMinGrabRadius = 10;
        public int magnetMaxGrabRadius = 10; //60 half a screen radius
        public int magnetScale = 1;
        public int magnetVelocity = 8;
        public int magnetAcceleration = 8;
        public int[] magnetBlacklist = new int[50]; //only populated when player activates magnet, not changed during gameplay
        //public int counter = 30;
        public int clientcounter = 30;

        public override void ResetEffects()
        {
            if (ModConf.Buff == 1)
            {
                magnetActive = 0;
            }

            //magnetGrabRadius = 0;
            //these are changed by config
            magnetMaxGrabRadius = 10; //60 //vvvvvvvvv starting values vvvvvvvvvvvvvv
            magnetScale = 1; //1
            magnetVelocity = 8; //16
            magnetAcceleration = 8; //20
        }

        public override void clientClone(ModPlayer clientClone)
        {
            ItemMagnetPlusPlayer clone = clientClone as ItemMagnetPlusPlayer;
        }

        private int[] MagnetBlacklist(ItemMagnetPlusPlayer mPlayer)
        {
            //list of item types to ignore
            //TODO make this more efficient with LINQ stuff
            //also make this more general
            string[] stringBlacklist = ModConf.Filter.Split(new string[] { "," }, 50, StringSplitOptions.RemoveEmptyEntries);
            string[] lowerCase = new string[stringBlacklist.Length];
            int[] typeBlacklist = new int[50];
            for (int i = 0; i < stringBlacklist.Length; i++)
            {
                lowerCase[i] = stringBlacklist[i].Trim();
            }
            string[] lowerCaseDistinct = lowerCase.Distinct().ToArray();
            int j = -1;
            for (int i = 0; i < lowerCaseDistinct.Length; i++)
            {
                if (lowerCaseDistinct[i] == "heart")
                {
                    typeBlacklist[++j] = 58;
                    typeBlacklist[++j] = 1734;
                    typeBlacklist[++j] = 1867;
                    //58 || Main.item[j].type == 1734 || Main.item[j].type == 1867
                }
                if (lowerCaseDistinct[i] == "mana")
                {
                    typeBlacklist[++j] = 184;
                    typeBlacklist[++j] = 1735;
                    typeBlacklist[++j] = 1868;
                    //184 || Main.item[j].type == 1735 || Main.item[j].type == 1868
                }
                if (lowerCaseDistinct[i] == "coin")
                {
                    typeBlacklist[++j] = 330;
                    typeBlacklist[++j] = 331;
                    typeBlacklist[++j] = 332;
                    typeBlacklist[++j] = 333;
                    //184 || Main.item[j].type == 1735 || Main.item[j].type == 1868
                }
            }
            Array.Resize(ref typeBlacklist, j + 1);
            return typeBlacklist;
        }

        public void ActivateMagnet(Player player)
        {
            magnetBlacklist = MagnetBlacklist(player.GetModPlayer<ItemMagnetPlusPlayer>(mod));
            if (ModConf.Buff != 0) // != 0 is buff
            {
                player.AddBuff(mod.BuffType("ItemMagnetBuff"), 3600, true);
            }
            else
            {
                ItemMagnetPlusPlayer mPlayer = player.GetModPlayer<ItemMagnetPlusPlayer>(mod);
                mPlayer.magnetActive = 1;
            }
        }

        public void DeactivateMagnet(Player player)
        {
            if (ModConf.Buff != 0) // != 0 is buff
            {
                player.ClearBuff(mod.BuffType("ItemMagnetBuff"));
            }
            else
            {
                player.ClearBuff(mod.BuffType("ItemMagnetBuff"));
                ItemMagnetPlusPlayer mPlayer = player.GetModPlayer<ItemMagnetPlusPlayer>(mod);
                mPlayer.magnetActive = 0;
            }
        }

        public override void OnEnterWorld(Player player)
        {
            DeactivateMagnet(player);
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            DeactivateMagnet(player);
        }

        public void UpdateMagnetValues(ItemMagnetPlusPlayer mPlayer, int currentRadius)
        {
            mPlayer.magnetMaxGrabRadius = ModConf.Range;
            mPlayer.magnetScale = ModConf.Scale;
            mPlayer.magnetVelocity = ModConf.Velocity;
            mPlayer.magnetAcceleration = ModConf.Acceleration;

            if (mPlayer.magnetScale == 0)
            {
                mPlayer.magnetGrabRadius = mPlayer.magnetMaxGrabRadius;
                return;
            }
            if (NPC.downedSlimeKing)
            {
                //Starts at
                //magnetMaxGrabRadius = 10;
                //magnetVelocity = 8;
                //magnetAcceleration = 8;

                mPlayer.magnetVelocity += 4;
                mPlayer.magnetAcceleration += 2;
            }
            if (NPC.downedBoss1) //Eye of Cthulhu
            {
                mPlayer.magnetMaxGrabRadius += 5;
            }
            if (NPC.downedBoss2) //Eater/Brain
            {
                mPlayer.magnetMaxGrabRadius += 5;
            }
            if (NPC.downedQueenBee)
            {
                mPlayer.magnetVelocity += 4;
                mPlayer.magnetAcceleration += 10;
            }
            if (NPC.downedBoss3) //Skeletron
            {
                mPlayer.magnetMaxGrabRadius += 5;
            }
            if (Main.hardMode) //Wall of flesh
            {
                //Ideal at
                //magnetMaxGrabRadius = 30; //quarter screen
                //magnetVelocity = 16;
                //magnetAcceleration = 20;

                mPlayer.magnetMaxGrabRadius += 5; //quarter of a screen range if ^ satisfied
            }
            if (NPC.downedMechBoss1) //Destroyer
            {
                mPlayer.magnetMaxGrabRadius += 10;
            }
            if (NPC.downedMechBoss2) //Twins
            {
                mPlayer.magnetMaxGrabRadius += 10;
            }
            if (NPC.downedMechBoss3) //Skeletron prime
            {
                mPlayer.magnetMaxGrabRadius += 10;
            }
            if (NPC.downedPlantBoss)
            {
                mPlayer.magnetMaxGrabRadius += 10;
                mPlayer.magnetVelocity += 4;
                mPlayer.magnetAcceleration += 2;
            }
            if (NPC.downedGolemBoss)
            {
                mPlayer.magnetMaxGrabRadius += 10;
                mPlayer.magnetVelocity += 4;
                mPlayer.magnetAcceleration += 2;
            }
            if (NPC.downedFishron)
            {
                mPlayer.magnetMaxGrabRadius += 10;
                mPlayer.magnetVelocity += 4;
                mPlayer.magnetAcceleration += 2;
            }
            if (NPC.downedAncientCultist)
            {
                mPlayer.magnetMaxGrabRadius += 10;
            }
            if (NPC.downedMoonlord)
            {
                //Final at
                //magnetMaxGrabRadius = 120; //one screen
                //magnetVelocity = 32;
                //magnetAcceleration = 32;
                mPlayer.magnetMaxGrabRadius += 20;
                mPlayer.magnetVelocity += 4;
                mPlayer.magnetAcceleration += 6;
            }

            if (currentRadius <= mPlayer.magnetMaxGrabRadius + 1)
            {
                mPlayer.magnetGrabRadius = currentRadius;
            }
            else
            {
                mPlayer.magnetGrabRadius = mPlayer.magnetMinGrabRadius;
            }
        }

        public override void PreUpdate()
        {

            //Main.NewText(magnetGrabRadius);
            //Main.NewText(magnetMaxGrabRadius);
            //Main.NewText(magnetMinGrabRadius);
            //Main.NewText(magnetActive);

            //if (Main.netMode != NetmodeID.MultiplayerClient)
            //{
            if (magnetActive > 0)
            {
                ItemMagnetPlusPlayer mPlayer = player.GetModPlayer<ItemMagnetPlusPlayer>(mod);
                UpdateMagnetValues(mPlayer, magnetGrabRadius);

                int grabRadius = (int)(magnetGrabRadius * 16); //16 == to world coordinates
                int fullhdgrabRadius = (int)(grabRadius * 0.5625f);
                //Main.NewText("grabradius: " + grabRadius);
                for (int j = 0; j < 400; j++)
                {
                    //Main.NewText(j);
                    //if (j ==0) Main.NewText("start for loop");
                    if(Main.item[j].active && Main.item[j].noGrabDelay == 0 && !ItemLoader.GrabStyle(Main.item[j], player) && ItemLoader.CanPickup(Main.item[j], player)) {
                        //Main.NewText("position " + player.position);
                        //Main.NewText("Tile " + player.position.ToTileCoordinates());
                        Rectangle rect = new Rectangle((int)player.position.X - grabRadius, (int)player.position.Y - fullhdgrabRadius, player.width + grabRadius * 2, player.height + fullhdgrabRadius * 2);
                        if (rect.Intersects(new Rectangle((int)Main.item[j].position.X, (int)Main.item[j].position.Y, Main.item[j].width, Main.item[j].height)))
                        {
                            if (player.inventory[player.selectedItem].type != 0 || player.itemAnimation <= 0)
                            {
                                if(Array.BinarySearch(magnetBlacklist, Main.item[j].type) < 0)
                                {
                                    //Main.NewText("wtf");
                                    //so it can go through walls
                                    Main.item[j].beingGrabbed = true;
                                    //velocity, higher = more speed
                                    int velo = magnetVelocity; //16 default

                                    Vector2 vector = new Vector2(Main.item[j].position.X + (float)(Main.item[j].width / 2), Main.item[j].position.Y + (float)(Main.item[j].height / 2));
                                    float distanceX = player.Center.X - vector.X;
                                    float distanceY = player.Center.Y - vector.Y;
                                    float normalDistance = (float)Math.Sqrt((double)(distanceX * distanceX + distanceY * distanceY));
                                    normalDistance = ((float)velo) / normalDistance;
                                    distanceX *= normalDistance;
                                    distanceY *= normalDistance;

                                    //acceleration, higher = more acceleration
                                    if (magnetAcceleration > 40) magnetAcceleration = 40;
                                    int accel = -(magnetAcceleration - 41); //20 default

                                    // num1 goes linear, num2 goes inverse
                                    Main.item[j].velocity.X = (Main.item[j].velocity.X * (float)(accel - 1) + distanceX) / (float)accel;
                                    Main.item[j].velocity.Y = (Main.item[j].velocity.Y * (float)(accel - 1) + distanceY) / (float)accel;

                                    if (Main.rand.NextFloat() < 0.7f)
                                    {
                                        Dust dust = Main.dust[Dust.NewDust(Main.item[j].position, 30, 30, 204, 0f, 0f, 0, new Color(255, 255, 255), 0.8f)];
                                        dust.noGravity = true;
                                        dust.noLight = true;
                                    }
                                    //Main.NewText("drawing in item " + Main.item[j].Name);
                                }
                            }
                        }
                    }
                       // }
                    else
                    {
                        Main.item[j].beingGrabbed = false;
                    }
                    //Main.item[j].beingGrabbed = false;
                    //if (j == 399) Main.NewText("end for loop");
                }
            }
            //}

            //if (Main.netMode == NetmodeID.Server)
            //{
            //    if (counter == 0)
            //    {
            //        Console.WriteLine(" " + player.name + " active " + magnetActive);
            //        Console.WriteLine(" " + player.name + " grabradius " + magnetGrabRadius);
            //        counter = 30;
            //    }
            //    counter--;
            //}

            //if (Main.netMode == NetmodeID.MultiplayerClient)
            //{
            //    if (clientcounter == 0)
            //    {
            //        Main.NewText(" " + player.name + " active " + magnetActive);
            //        Main.NewText(" " + player.name + " grabradius " + magnetGrabRadius);

            //        clientcounter = 30;
            //    }
            //    clientcounter--;
            //}

            //if (Main.netMode == NetmodeID.SinglePlayer)
            //{
            //    if (clientcounter == 0)
            //    {
            //        Main.NewText("active " + magnetActive);
            //        Main.NewText("grabradius " + magnetGrabRadius);

            //        clientcounter = 30;
            //    }
            //    clientcounter--;
            //}
        }
    }
}
