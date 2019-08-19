using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ItemMagnetPlus
{
    class ItemMagnetPlus : Mod
    {
        public override void Load()
        {
            ConfigWrapper.Load();
        }

        public override void Unload()
        {
            ConfigWrapper.Unload();
        }

        // Mod Helpers compat
        public static string GithubUserName { get { return "direwolf420"; } }
        public static string GithubProjectName { get { return "ItemMagnetPlus"; } }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            IMPMessageType msgType = (IMPMessageType)reader.ReadByte();
            byte playernumber;
            int range;

            byte arrayLength;

            BitsByte variousBooleans;
            bool currentlyActive;

            ItemMagnetPlusPlayer mPlayer;
            switch (msgType)
            {
                case IMPMessageType.SyncPlayer:
                    if(Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        // Get all info about the players

                        arrayLength = reader.ReadByte();
                        if (arrayLength > 0)
                        {
                            byte[] indexes = new byte[arrayLength];
                            int[] ranges = new int[arrayLength];
                            bool[] currentlyActives = new bool[arrayLength];

                            for (int i = 0; i < arrayLength; i++)
                            {
                                indexes[i] = reader.ReadByte();
                                ranges[i] = reader.ReadInt32();
                                currentlyActives[i] = reader.ReadBoolean();
                            }

                            for (int i = 0; i < arrayLength; i++)
                            {
                                mPlayer = Main.player[indexes[i]].GetModPlayer<ItemMagnetPlusPlayer>();
                                mPlayer.magnetGrabRadius = ranges[i];
                                mPlayer.currentlyActive = currentlyActives[i];
                            }
                        }
                    }
                    break;
                case IMPMessageType.SendClientChanges:
                    playernumber = reader.ReadByte();
                    range = reader.ReadInt32();

                    variousBooleans = reader.ReadByte();
                    currentlyActive = variousBooleans[0];

                    mPlayer = Main.player[playernumber].GetModPlayer<ItemMagnetPlusPlayer>();
                    mPlayer.magnetGrabRadius = range;
                    mPlayer.currentlyActive = currentlyActive;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = GetPacket();
                        packet.Write((byte)IMPMessageType.SendClientChanges);
                        packet.Write(playernumber);
                        packet.Write((int)range);
                        packet.Write((byte)variousBooleans);
                        packet.Send(-1, playernumber);
                    }
                    break;
                default:
                    Logger.Info("Unknown Message type: " + msgType);
                    break;
            }
        }
    }

    enum IMPMessageType : byte
    {
        SyncPlayer,
        SendClientChanges
    }
}
