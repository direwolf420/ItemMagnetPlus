# ItemMagnetPlus

![Banner](https://raw.githubusercontent.com/direwolf420/ItemMagnetPlus/master/banner.png)

Terraria Forum link: https://forums.terraria.org/index.php?threads/itemmagnetplus-customizable-item-magnet.74425/

ItemMagnetPlus adds a single item called "Item Magnet" that does the obvious thing: Sucking in items around you so you don't have to run around and collect them yourself.

Adds:
Item Magnet (and corresponding buff) that
* can be crafted with simple materials (Iron/Lead)
* works while in your inventory (doesn't waste an accessory slot)
* can be toggled on and off
* has adjustable range and item succ speed
* has increased stats after killing bosses
* customizable via config (\Documents\My Games\Terraria\ModLoader\Mod Configs) to fit your playstyle

How to use:
* Left click cycles through its ranges
* Right click shows current range (you can always check the stats on the buff tooltip)
* This right click functionality changes to "turn off" when "buff" flag in the config is set to 0
* Killing bosses improves its stats
* If you want it to either be off or on, there is a config entry called "scale", set it to 0 or 2

Multiplayer:
* This mod is multiplayer compatible, the config is entirely server side, but the buff flag is forced to 1
* Items that are inbetween two players with magnets will "float"
* Certain items (i.e. Boss bags in expert mode) will be pulled by all players even though it might not be picked up
* Due to the way the "grab delay" is only set in singleplayer, items dropped by the player will instantly latch onto the player, which is normal behavior
* Items might not get sucked up and turn into a "ghost" with the Auto Trash Mod enabled
* Report any bugs in the forum thread

Progression: (default config)

 Starts with:

* Range = 10 (Blocks in each direction)
* Velocity = 8
* Acceleration = 8

  Ends with: (killing Moonlord)

* Range = 120 (one and a half screens)
* Velocity = 32
* Acceleration = 32


 About the config:
* Buff decides if it gives you a corresponding buff icon to show the status of the magnet
* Filter decides what do ignore when using the magnet (only hearts, mana stars and coins supported for now)
* Range starts from 10, but can be as big as you want
* If you increase Vel or Acc too much from those recommended above, items might get "stuck" on you until you deactivate it again
* Beware of lag when increasing these values, especially range
* If the difference between velocity and acceleration is too big, items will go in circles around you or get stuck until you deactivate it
* If you change the version number, your config might get reset, so don't touch it 

 Changelog:

 v0.1.9: Fixed desync bug with more than two players in MP

 v0.1.8: Made config entirely server side now, the mod is stable in MP

 v0.1.7: Made config entirely client side now

 v0.1.6.2: Reverted hotfix

 v0.1.6.1: Hotfix bug where items get deleted very rarely when opening crate-like things (now you won't attract items with a full inventory)

 v0.1.6: Added persistency: Magnet now keeps its state when you respawn! (Also switched scale mode 0 and 2 around, check your config)
 
 v0.1.5: Added new scale config mode: Keep all the buffs from killing bosses but don't shuffle through each range again upon activation

 v0.1.4.1 + 2 + 3 + 4: Filter hotfix, fix stuck range when using magnet inside inventory, another filter hotfix
 
 v0.1.4: Added blacklist "filter" to be able to filter hearts, mana stars and coins (for now)

 v0.1.3: Added config flag "buff" to be able to decide if a buff should be applied while the magnet is active

 v0.1.2: Added Buff- and Tooltip to show range, updated icons (Thanks to Harblesnargits!)

 v0.1.1: Fixed incompatibility with Even More Modifiers, changed acceleration values (updating will set it back to default)

 v0.1: Initial release
