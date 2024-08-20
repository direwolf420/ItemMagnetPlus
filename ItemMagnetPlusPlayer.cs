using ItemMagnetPlus.Buffs;
using ItemMagnetPlus.Core.Netcode.Packets;
using ItemMagnetPlus.Items;
using Microsoft.Xna.Framework;
using System;
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
		public int magnetGrabRadius = Config.RangeMin;
		public int magnetMinGrabRadius = Config.RangeMin;
		public int magnetMaxGrabRadius = Config.RangeMin; //60 half a screen radius
		public Config.ScaleModeType magnetScaleMode = Config.ScaleModeType.AlwaysMaxRange;
		public int magnetVelocity = Config.VelocityMin;
		public int magnetAcceleration = Config.AccelerationMin;
		private bool hadMagnetActive = false;
		public bool currentlyActive = false;

		public override void ResetEffects()
		{
			if (Config.Instance.Buff)
			{
				magnetActive = 0;
			}
			//these are changed by config
			//starting values
			magnetMaxGrabRadius = Config.RangeMin;
			magnetScaleMode = Config.ScaleModeType.AlwaysMaxRange;
			magnetVelocity = Config.VelocityMin;
			magnetAcceleration = Config.AccelerationMin;
		}

		public override void CopyClientState(ModPlayer clientClone)
		{
			ItemMagnetPlusPlayer clone = clientClone as ItemMagnetPlusPlayer;
			clone.magnetGrabRadius = magnetGrabRadius;
			clone.currentlyActive = currentlyActive;
		}

		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			ItemMagnetPlusPlayer clone = clientPlayer as ItemMagnetPlusPlayer;
			if (clone.magnetGrabRadius != magnetGrabRadius)
			{
				new SendIMPPlayerChangeRadiusPacket(this).Send();
			}

			if (clone.currentlyActive != currentlyActive)
			{
				new SendIMPPlayerChangeTogglePacket(this).Send();
			}
		}

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			new SyncIMPPlayerPacket(this).Send(toWho, fromWho);
		}

		public void ActivateMagnet()
		{
			if (!Config.Instance.Buff)
			{
				magnetActive = 1;
			}
			else
			{
				Player.AddBuff(ModContent.BuffType<ItemMagnetBuff>(), 60);
			}
		}

		public void DeactivateMagnet()
		{
			if (!Config.Instance.Buff)
			{
				magnetActive = 0;
			}

			//Clear buff either way
			Player.ClearBuff(ModContent.BuffType<ItemMagnetBuff>());
		}

		public override void OnEnterWorld()
		{
			if (Config.Instance.OnEnter && Player.HasItem(ModContent.ItemType<ItemMagnet>()))
			{
				ActivateMagnet();
			}
			else
			{
				DeactivateMagnet();
			}
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			if (Player.HasBuff(ModContent.BuffType<ItemMagnetBuff>()) || magnetActive != 0)
			{
				hadMagnetActive = true;
			}
			else
			{
				hadMagnetActive = false;
			}
		}

		public override void OnRespawn()
		{
			if (hadMagnetActive)
			{
				hadMagnetActive = false;
				ActivateMagnet();
			}
		}

		public void UpdateMagnetValues(int currentRadius)
		{
			//currentRadius is for creating steps between min and max range, and setting it accordingly
			magnetMaxGrabRadius = Config.Instance.Range;
			magnetScaleMode = Config.Instance.ScaleMode;
			magnetVelocity = Config.Instance.Velocity;
			magnetAcceleration = Config.Instance.Acceleration;

			Config.Update(magnetScaleMode, ref magnetMaxGrabRadius, ref magnetVelocity, ref magnetAcceleration);

			if (magnetScaleMode == Config.ScaleModeType.OnlyConfig)
			{
				magnetGrabRadius = magnetMaxGrabRadius;
				return;
			}

			Config.Clamp(ref magnetMaxGrabRadius, Config.RangeMin, Config.RangeMax);
			Config.Clamp(ref magnetVelocity, Config.VelocityMin, Config.VelocityMax);
			Config.Clamp(ref magnetAcceleration, Config.AccelerationMin, Config.AccelerationMax);

			if (magnetScaleMode != Config.ScaleModeType.Bosses)
			{
				magnetGrabRadius = magnetMaxGrabRadius;
				return;
			}

			if (currentRadius <= magnetMaxGrabRadius + 1)
			{
				magnetGrabRadius = currentRadius;
			}
			else
			{
				magnetGrabRadius = magnetMinGrabRadius;
			}
		}

		public override void PreUpdate()
		{
			//doing this only client side causes a small "lag" when the item first gets dragged toward the player
			Config cfg = Config.Instance;
			currentlyActive = cfg.Buff ? Player.HasBuff(ModContent.BuffType<ItemMagnetBuff>()) : magnetActive == 1;
			bool whileHeld = cfg.Held ? Player.HeldItem.type == ModContent.ItemType<ItemMagnet>() : true;

			if (currentlyActive && !Player.dead && whileHeld)
			{
				UpdateMagnetValues(magnetGrabRadius);

				int grabRadius = magnetGrabRadius * 16; //16 == to world coordinates
				int fullhdgrabRadius = (int)(grabRadius * 0.5625f);

				Rectangle grabRect = new Rectangle((int)Player.position.X - grabRadius, (int)Player.position.Y - fullhdgrabRadius, Player.width + grabRadius * 2, Player.height + fullhdgrabRadius * 2);

				int grabbedItems = 0;

				bool grabbingAtleastOneCoin = false;

				for (int j = 0; j < Main.maxItems; j++)
				{
					Item item = Main.item[j];
					if (!item.active || item.shimmerTime != 0f || item.noGrabDelay != 0 || (item.shimmered && item.velocity.LengthSquared() >= 0.2f * 0.2f) || ItemLoader.GrabStyle(item, Player) || !ItemLoader.CanPickup(item, Player))
					{
						continue;
					}

					if (Config.Instance.NeedsSpace)
					{
						Player.ItemSpaceStatus status = Player.ItemSpace(item);
						if (!Player.CanPullItem(item, status))
						{
							//Checks for encumbering stone
							continue;
						}
					}

					bool canGrabNetMode = true;
					//All: item.ownIgnore == -1 && item.keepTime == 0
					//Client: (above) && item.owner != 255 
					if (Main.netMode != NetmodeID.SinglePlayer)
					{
						if (item.instanced) canGrabNetMode &= item.playerIndexTheItemIsReservedFor == Player.whoAmI;
					}

					if (canGrabNetMode && grabRect.Intersects(item.getRect()))
					{
						if (ConfigWrapper.CanBePulled(item, Player))
						{
							grabbedItems++;
							//so it can go through walls
							item.beingGrabbed = true;
							item.shimmered = false;

							if (Array.BinarySearch(ConfigWrapper.CoinTypes, item.type) > -1)
							{
								grabbingAtleastOneCoin = true;
							}

							MergeNearbyItems(item, j);

							if (cfg.Instant)
							{
								item.Center = Player.Center;
								continue;
							}

							//velocity, higher = more speed
							float velo = magnetVelocity; //16 ideal

							Vector2 distance = Player.Center - item.Center;
							Vector2 normalizedDistance = distance;

							//adjustment term, increases velocity the closer to the player it is (0..2)
							float length = distance.Length();
							velo += 2 * (1 - length / grabRadius);

							if (length > 0)
							{
								normalizedDistance /= length;
							}
							normalizedDistance *= velo;

							//acceleration, higher = more acceleration
							int accel = -(magnetAcceleration - 41); //20 ideal

							item.velocity = (item.velocity * (accel - 1) + normalizedDistance) / (float)accel;

							if (Main.netMode != NetmodeID.Server)
							{
								float dustChance = length < Player.height ? 0.7f / (Player.height - length) : 0.7f;
								dustChance *= (11f - grabbedItems) / 10f;
								if (Main.rand.NextFloat() < dustChance - 0.02f)
								{
									Dust dust = Dust.NewDustDirect(item.position, item.width, item.height, 204, 0f, 0f, 0, new Color(255, 255, 255), 0.8f);
									dust.noGravity = true;
									dust.noLight = true;
								}
							}
						}
					}
				}

				//remove dust from grabbed coins while magnet is enabled
				//credit to hamstar (Uncluttered Projectiles)
				if (Main.netMode != NetmodeID.Server && grabbingAtleastOneCoin)
				{
					Dust dust;
					for (int i = 0; i < Main.maxDustToDraw; i++)
					{
						dust = Main.dust[i];
						if (dust == null || !dust.active) continue;

						//since items can't be referenced by index, just check if atleast one coin is being grapped, and then also check if the dust associated with it
						//is within bounds of the magnet range
						if (!grabRect.Contains(new Rectangle((int)dust.position.X, (int)dust.position.Y, 1, 1))) continue;

						//item type 71 to 74: 
						//int type = 244 + item.type - 71;
						//-> 244 to 247
						if (dust.type >= 244 && dust.type <= 247) Main.dust[i] = new Dust();
					}
				}
			}
		}

		private void MergeNearbyItems(Item item, int itemWhoAmI)
		{
			//Copied from the CombineWithNearbyItems. Edited to NOT merge if outside of Player.defaultItemGrabRange range from the player
			bool canMerge = true;
			int type = item.type;
			if (Array.BinarySearch(ConfigWrapper.CoinTypes, type) > -1)
			{
				canMerge = false;
			}

			if (ItemID.Sets.NebulaPickup[type])
			{
				canMerge = false;
			}

			if (!canMerge)
			{
				return;
			}

			if (item.playerIndexTheItemIsReservedFor == Main.myPlayer && item.CanCombineStackInWorld() && item.stack < item.maxStack)
			{
				if (Player.DistanceSQ(item.Center) > Player.defaultItemGrabRange * Player.defaultItemGrabRange)
				{
					return;
				}

				for (int j = itemWhoAmI + 1; j < Main.maxItems; j++)
				{
					Item otherItem = Main.item[j];
					if (otherItem.active && otherItem.type == type && otherItem.stack > 0 && otherItem.playerIndexTheItemIsReservedFor == item.playerIndexTheItemIsReservedFor && ItemLoader.CanStackInWorld(item, otherItem) && Math.Abs(item.Center.X - otherItem.Center.X) + Math.Abs(item.Center.Y - otherItem.Center.Y) < 30f)
					{
						item.position = (item.position + otherItem.position) / 2f;
						item.velocity = (item.velocity + otherItem.velocity) / 2f;
						int otherStack = otherItem.stack;
						if (otherStack > item.maxStack - item.stack)
						{
							otherStack = item.maxStack - item.stack;
						}

						otherItem.stack -= otherStack;
						item.stack += otherStack;
						if (otherItem.stack <= 0)
						{
							otherItem.SetDefaults();
							otherItem.active = false;
						}

						if (Main.netMode != NetmodeID.SinglePlayer && item.playerIndexTheItemIsReservedFor == Main.myPlayer)
						{
							NetMessage.SendData(MessageID.SyncItem, number: itemWhoAmI);
							NetMessage.SendData(MessageID.SyncItem, number: j);
						}
					}
				}
			}
		}
	}
}
