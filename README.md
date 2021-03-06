# ItemMagnetPlus

![Banner](https://raw.githubusercontent.com/direwolf420/ItemMagnetPlus/master/banner.png)

Terraria Forum link: https://forums.terraria.org/index.php?threads/itemmagnetplus-customizable-item-magnet.74425/

ItemMagnetPlus adds a single item called "Item Magnet" that does the obvious thing: Sucking in items around you so you don't have to run around and collect them yourself.

### Adds:
Item Magnet (and corresponding buff) that
* can be crafted with simple materials (Iron/Lead)
* works while in your inventory
* can be toggled on and off
* has adjustable range and item succ speed
* has increased stats after killing bosses
* customizable via config ingame to fit your playstyle

### How to use:
* Left click cycles through its ranges
* Right click shows current range (you can always check the stats on the buff tooltip)
* This right click functionality changes to "turn off" when "Buff" in the config is false
* Killing bosses improves its stats
* If you want it to either be on or off, change "Scale" in the config to anything but "Increase With Bosses"

### Multiplayer:
* This mod is multiplayer compatible, the config is entirely server side, but the "Buff" is enabled
* Items that are inbetween two players with magnets will "float"
* If "Instant" is enabled and multiple players are in range of an item, it will always go to the same player (it is based on join order)
* Due to the way the "grab delay" is only set in singleplayer, items dropped by the player will instantly latch onto the player, which is normal behavior
* Items might not get sucked up and turn into a "ghost" with the Auto Trash Mod enabled
* Lost items due to this bug won't be recovered
* Report any bugs in the forum thread (please)

### Progression: (default config)

Starts with:

* Range = 10 (blocks in each direction)
* Velocity = 8
* Acceleration = 8

Ends with: (killing Moonlord)

* Range = 120 (one and a half screens)
* Velocity = 32
* Acceleration = 32


### About the config:
* Buff decides if it gives you a corresponding buff icon to show the status of the magnet
* Held decides if the magnet works only when held
* "Activate On Enter" decides if magnet should be automatically activated when entering the world
* Filter function: Presets (hearts, mana stars, coins, pickup effects), blacklist/whitelist
* Magnet stats are limited to sensible values (Range only goes up to about three screens in any direction)
* If you increase Vel or Acc too much from those recommended above, items might get "stuck" on you until you deactivate it again
* Beware of lag when increasing these values, especially range
* If the difference between velocity and acceleration is too big, items will go in circles around you or get stuck until you deactivate it
* "Instant" skips any velocity of acceleration checks and directly teleports the item to your location
* After you change the values in the config, use the magnet again to take effect

### Changelog:

 v1.0.3: Added "Needs Space" and "Instant" settings

 v1.0.2.1: Fixed the "coin" dust not disappearing properly when grabbing coins

 v1.0.2: Added "Activate On Enter" setting that allows a magnet in your inventory to automatically activate itself when entering a world

 v1.0.1.3: tml 0.11.7 update. Cleaner range indicator, now also shows when mouseovering the buff icon

 v1.0.1.2: tml 0.11.6 update. Fixed unintended config saving in multiplayer, fixed preset filter being inverted

 v1.0.1.1: Fixed incompatibility with Jpans Bags of Holding, the "Pickup Effect Items" setting will be ignored with this mod enabled

 v1.0.1: Added filter for nebula pickup and modded items that have pickup effects (Thorium music notes for example), grabbed coins don't overproduce sparkles

 v1.0.0.1 + 2: Fix velocity cap, other stat caps

 v1.0: updated to tml 0.11.4. Fixed multiplayer, updated to ingame config, fixed coin filter, added custom black/whitelist

 v0.2: Updated to ingame config, fixed coin filter

 v0.1.11: Added config flag to have the magnet work either from inside the inventory or only when held

 v0.1.10: Scale spark rate on pulled items, fixed some desync bugs in MP, fixed grab bags being pulled in by all players

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
