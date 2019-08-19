using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace ItemMagnetPlus
{
    class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        // Automatically assigned by tmodloader
        public static Config Instance;

        public const int RangeMin = 10;
        public const int RangeMax = 240;
        public const int VelocityMin = 1;
        public const int VelocityMax = 80;
        public const int AccelerationMin = 1;
        public const int AccelerationMax = 40;

        public const string BlacklistName = "Blacklist";
        public const string WhitelistName = "Whitelist";

        public const string ScaleModeBosses = "Increase With Bosses";
        public const string ScaleModeAlwaysMaxRange = "Bosses + Max Range";
        public const string ScaleModeOnlyConfig = "Custom + Max Range";

        //-------------
        [Header("Preset Item Blacklist")]

        [Label("[i:58] Hearts")]
        [Tooltip("Toggle if hearts should be picked up by the magnet")]
        [DefaultValue(true)]
        public bool Hearts;

        [Label("[i:184] Mana Stars")]
        [Tooltip("Toggle if mana stars should be picked up by the magnet")]
        [DefaultValue(true)]
        public bool ManaStars;

        [Label("[i:73] Coins")]
        [Tooltip("Toggle if coins should be picked up by the magnet")]
        [DefaultValue(false)]
        public bool Coins;

        //-------------
        [Header("Custom Item Filter")]

        [Label("Magnet Blacklist")]
        [Tooltip("Customize which items the magnet should never pick up")]
        [BackgroundColor(30, 30, 30)]
        public List<ItemDefinition> Blacklist = new List<ItemDefinition>();

        [Label("Magnet Whitelist")]
        [Tooltip("Customize which items the magnet should always pick up")]
        [BackgroundColor(220, 220, 220)]
        public List<ItemDefinition> Whitelist = new List<ItemDefinition>();

        [Label("List Mode Toggle")]
        [Tooltip("Change to select which list will be used for the item filter")]
        [DrawTicks]
        [OptionStrings(new string[] { BlacklistName, WhitelistName })]
        [DefaultValue(BlacklistName)]
        public string ListMode;

        //-------------
        [Header("General")]
        [Label("Buff")]
        [Tooltip("Toggle if having the magnet active gives you a buff")]
        [DefaultValue(true)]
        public bool Buff;

        [Label("Held")]
        [Tooltip("Toggle if magnet should only work when you hold it (instead of being in the inventory)")]
        [DefaultValue(false)]
        public bool Held;

        //-------------
        [Header("Magnet Behavior (Only works ingame)")]

        //[Label("Range")]
        [Tooltip("Base magnet radius in tiles")]
        [Slider]
        [SliderColor(255, 255, 50)]
        [Range(RangeMin, RangeMax)]
        [Increment(5)]
        [DefaultValue(10)]
        public int Range;

        //[Label("Velocity")]
        [Tooltip("Speed at which items get pulled towards you")]
        [Slider]
        [SliderColor(255, 255, 50)]
        [Range(VelocityMin, VelocityMax)]
        [DefaultValue(8)]
        public int Velocity;

        //[Label("Acceleration")]
        [Tooltip("How fast an item reaches its peak speed")]
        [Slider]
        [SliderColor(255, 255, 50)]
        [Range(AccelerationMin, AccelerationMax)]
        [DefaultValue(8)]
        public int Acceleration;

        //[Label("Acceleration")]
        [Tooltip("Scaling Mode")]
        [DrawTicks]
        [SliderColor(255, 255, 50)]
        [OptionStrings(new string[] { ScaleModeBosses, ScaleModeAlwaysMaxRange, ScaleModeOnlyConfig })]
        [DefaultValue(ScaleModeAlwaysMaxRange)]
        public string Scale;

        //-------------
        [Header("Resulting Magnet stats")]
        [Label("Resulting Max Range")]
        [Slider]
        [SliderColor(50, 255, 50)]
        [JsonIgnore]
        [Range(RangeMin, RangeMax + 110)]
        public int CurrentMaxRange
        {
            get
            {
                int range = Range;
                UpdateRange(ref range);
                return range;
            }
        }

        [Label("Resulting Velocity")]
        [Slider]
        [SliderColor(50, 255, 50)]
        [JsonIgnore]
        [Range(VelocityMin, VelocityMax)]
        public int CurrentVelocity
        {
            get
            {
                int velocity = Velocity;
                UpdateVelocity(ref velocity);
                return velocity;
            }
        }

        [Label("Resulting Acceleration")]
        [Slider]
        [SliderColor(50, 255, 50)]
        [JsonIgnore]
        [Range(AccelerationMin, AccelerationMax)]
        public int CurrentAcceleration
        {
            get
            {
                int acceleration = Acceleration;
                UpdateAcceleration(ref acceleration);
                return acceleration;
            }
        }

        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message)
        {
            message = "Only the host of this world can change the config! Do so in singleplayer.";
            return true;
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            // Correct invalid names
            if (ListMode != BlacklistName && ListMode != WhitelistName) ListMode = BlacklistName;
            if (Scale != ScaleModeBosses && Scale != ScaleModeAlwaysMaxRange && Scale != ScaleModeOnlyConfig) Scale = ScaleModeBosses;
            // Clamp
            Clamp(ref Range, RangeMin, RangeMax);
            Clamp(ref Velocity, VelocityMin, VelocityMax);
            Clamp(ref Acceleration, AccelerationMin, AccelerationMax);
            if (Main.netMode == NetmodeID.Server) Buff = true; // Enforce buff in multiplayer
        }

        // This is stupid don't do this
        public void UpdateRange(ref int range)
        {
            int a = 0;
            int b = 0;
            Update(Scale, ref range, ref a, ref b);
            Clamp(ref range, RangeMin, RangeMax + 110);
        }

        public void UpdateVelocity(ref int velocity)
        {
            int a = 0;
            int b = 0;
            Update(Scale, ref a, ref velocity, ref b);
            Clamp(ref velocity, VelocityMin, VelocityMax);
        }

        public void UpdateAcceleration(ref int acceleration)
        {
            int a = 0;
            int b = 0;
            Update(Scale, ref a, ref b, ref acceleration);
            Clamp(ref acceleration, AccelerationMin, AccelerationMax);
        }

        private void Clamp(ref int value, int min, int max)
        {
            value = value < min ? min : (value > max ? max : value);
        }

        public void Update(string scale, ref int range, ref int velocity, ref int acceleration)
        {
            // Input as default values into here
            if (scale == ScaleModeOnlyConfig)
            {
                //magnetGrabRadius = range;
                return;
            }
            if (NPC.downedSlimeKing)
            {
                //Starts at
                //range = 10;
                //velocity = 8;
                //acceleration = 8;

                velocity += 4;
                acceleration += 2;
            }
            if (NPC.downedBoss1) //Eye of Cthulhu
            {
                range += 5;
            }
            if (NPC.downedBoss2) //Eater/Brain
            {
                range += 5;
            }
            if (NPC.downedQueenBee)
            {
                velocity += 4;
                acceleration += 10;
            }
            if (NPC.downedBoss3) //Skeletron
            {
                range += 5;
            }
            if (Main.hardMode) //Wall of flesh
            {
                range += 5;

                //Ideal at
                //range = 30; //quarter screen
                //velocity = 16;
                //acceleration = 20;
            }
            if (NPC.downedMechBoss1) //Destroyer
            {
                range += 10;
            }
            if (NPC.downedMechBoss2) //Twins
            {
                range += 10;
            }
            if (NPC.downedMechBoss3) //Skeletron prime
            {
                range += 10;
            }
            if (NPC.downedPlantBoss)
            {
                range += 10;
                velocity += 4;
                acceleration += 2;
            }
            if (NPC.downedGolemBoss)
            {
                range += 10;
                velocity += 4;
                acceleration += 2;
            }
            if (NPC.downedFishron)
            {
                range += 10;
                velocity += 4;
                acceleration += 2;
            }
            if (NPC.downedAncientCultist)
            {
                range += 10;
            }
            if (NPC.downedMoonlord)
            {
                range += 20;
                velocity += 4;
                acceleration += 6;

                //Final at
                //range = 120; //one screen
                //magnetVelocity = 32;
                //acceleration = 32;
            }
        }
    }

    static class ConfigWrapper
    {
        public static Config Instance;

        public static int[] HeartTypes;

        public static int[] ManaStarTypes;

        public static int[] CoinTypes;

        private static bool CheckIfItemIsInPresetBlacklist(int type)
        {
            if (Instance.Hearts && Array.BinarySearch(HeartTypes, type) > -1)
            {
                return true;
            }
            if (Instance.ManaStars && Array.BinarySearch(ManaStarTypes, type) > -1)
            {
                return true;
            }
            if (Instance.Coins && Array.BinarySearch(CoinTypes, type) > -1)
            {
                return true;
            }
            return false;
        }

        public static bool CanBePulled(int type)
        {
            bool can = !CheckIfItemIsInPresetBlacklist(type);
            ItemDefinition item = new ItemDefinition(type);
            if (Instance.ListMode == Config.BlacklistName)
            {
                if (Instance.Blacklist.Contains(item))
                {
                    can = false;
                }
            }
            else if (Instance.ListMode == Config.WhitelistName)
            {
                can = Instance.Whitelist.Contains(item);
            }
            return can;
        }

        public static void Load()
        {
            Instance = Config.Instance;
            HeartTypes = new int[] { ItemID.Heart, ItemID.CandyApple, ItemID.CandyCane };
            ManaStarTypes = new int[] { ItemID.Star, ItemID.SoulCake, ItemID.SugarPlum };
            CoinTypes = new int[] { ItemID.CopperCoin, ItemID.SilverCoin, ItemID.GoldCoin, ItemID.PlatinumCoin };
            Array.Sort(HeartTypes);
            Array.Sort(ManaStarTypes);
            Array.Sort(CoinTypes);
        }

        public static void Unload()
        {
            Instance = null;
            HeartTypes = null;
            ManaStarTypes = null;
            CoinTypes = null;
        }
    }
}
