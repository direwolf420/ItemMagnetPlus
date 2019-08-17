using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace ItemMagnetPlus
{
    class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        // Automatically assigned by tmodloader
        public static Config Instance;

        //-------------
        [SeparatePage]
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
        [SeparatePage]
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
        [OptionStrings(new string[] { "Blacklist", "Whitelist" })]
        [DefaultValue("Blacklist")]
        public string ListMode;

        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message)
        {
            message = "Only the host of this world can change the config! Do so in singleplayer.";
            return false;
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (ListMode != "Blacklist" && ListMode != "Whitelist") ListMode = "Blacklist";
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
            if (Instance.ListMode == "Blacklist")
            {
                if (Instance.Blacklist.Contains(item))
                {
                    can = false;
                }
            }
            else if (Instance.ListMode == "Whitelist")
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
