﻿using System;
using System.IO;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;

namespace ItemMagnetPlus
{
    // Tutorial by goldenapple: https://forums.terraria.org/index.php?threads/modders-guide-to-config-files-and-optional-features.48581/
    public static class ModConf
    {
        public const int configVersion = 3;
        private readonly static string modName = "ItemMagnetPlus";

        private const string rangeField = "range";
        internal static int range = 10;
        public static int Range
        {
            get
            {
                if(range <= 0)
                {
                    return 10;
                }
                return range;
            }
        }

        private const string scaleField = "scale";
        internal static int scale = 1;
        public static int Scale
        {
            get
            {
                return scale;
            }
        }

        private const string velocityField = "velocity";
        internal static int velocity = 8;
        public static int Velocity
        {
            get
            {
                if (velocity <= 0)
                {
                    return 1;
                }
                return velocity;
            }
        }

        private const string accelerationField = "acceleration";
        internal static int acceleration = 8;
        public static int Acceleration
        {
            get
            {
                if (acceleration <= 0)
                {
                    return 1;
                }
                if (acceleration > 40)
                {
                    return 40;
                }
                return acceleration;
            }
        }

        static readonly string ConfigPath = Path.Combine(Main.SavePath, "Mod Configs/" + modName + ".json");

        static Preferences ModConfig = new Preferences(ConfigPath);

        internal static void Load()
        {
            bool success = ReadConfig();
            if (!success)
            {
                ErrorLogger.Log("ItemMagnetPlus: Couldn't load config file, creating new file.");
                CreateConfig();
            }
        }

        // true if loaded successfully
        internal static bool ReadConfig()
        {
            if (ModConfig.Load())
            {
                int readVersion = 0;
                ModConfig.Get("version", ref readVersion);
                if (readVersion != configVersion)
                {
                    bool canUpdate = false;
                    if (readVersion == 1)
                    {
                        ErrorLogger.Log("ItemMagnetPlus: updated Version");
                        canUpdate = true;
                        ModConfig.Put("version", 2);
                        ModConfig.Put(rangeField, range);
                        ModConfig.Put("readme", "First off, make sure to reload before the configs will take any effect. Range: Item pull range (default 10). Scale: 0 => always use max radius, also ignores increased stats by boss progression; 1 => switch through min to max radius (default 1), Velocity: how fast items move towards you (default 8), Acceleration: how fast items speed up when moving towards you (default 8). WARNING: Clients will desync if their local config is different to the server - this cannot be fixed without forcing the clients to download the server's mods and forcing the mods to reload. So don't mess with this too much outside of singleplayer unless you know what you're doing. And no I'm too lazy to find out how to even fix this behaviour, though a simple server mismatch warning might be a good idea. Feel free to delete this.");
                        ModConfig.Save();
                    }
                    if (readVersion == 2)
                    {
                        ErrorLogger.Log("ItemMagnetPlus: updated Version");
                        canUpdate = true;
                        ModConfig.Put("version", 3);
                        ModConfig.Put(accelerationField, acceleration);
                        ModConfig.Put("readme", "First off, make sure to reload before the configs will take any effect. Range: Item pull range (default 10). Scale: 0 => always use max radius, also ignores increased stats by boss progression; 1 => switch through min to max radius (default 1), Velocity: how fast items move towards you (default 8), Acceleration: how fast items speed up when moving towards you (default 8). WARNING: Clients will desync if their local config is different to the server - this cannot be fixed without forcing the clients to download the server's mods and forcing the mods to reload. So don't mess with this too much outside of singleplayer unless you know what you're doing. And no I'm too lazy to find out how to even fix this behaviour, though a simple server mismatch warning might be a good idea. Feel free to delete this.");
                        ModConfig.Save();
                    }

                    if (!canUpdate) return false;
                }

                ModConfig.Get(rangeField, ref range);
                ModConfig.Get(scaleField, ref scale);
                ModConfig.Get(velocityField, ref velocity);
                ModConfig.Get(accelerationField, ref acceleration);

                return true;
            }
            return false;
        }

        // Create a new config file for the player to edit. 
        internal static void CreateConfig()
        {
            ModConfig.Clear();
            ModConfig.Put("version", configVersion);

            ModConfig.Put(rangeField, range);
            ModConfig.Put(scaleField, scale);
            ModConfig.Put(velocityField, velocity);
            ModConfig.Put(accelerationField, acceleration);

            ModConfig.Put("readme", "First off, make sure to reload before the configs will take any effect. Range: Item pull range (default 10). Scale: 0 => always use max radius, also ignores increased stats by boss progression; 1 => switch through min to max radius (default 1), Velocity: how fast items move towards you (default 8), Acceleration: how fast items speed up when moving towards you (default 8). WARNING: Clients will desync if their local config is different to the server - this cannot be fixed without forcing the clients to download the server's mods and forcing the mods to reload. So don't mess with this too much outside of singleplayer unless you know what you're doing. And no I'm too lazy to find out how to even fix this behaviour, though a simple server mismatch warning might be a good idea. Feel free to delete this.");

            ModConfig.Save();
        }
    }
}
