# ItemMagnetPlus

![Banner](https://raw.githubusercontent.com/direwolf420/ItemMagnetPlus/1.4/banner.png)

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
* "Activate on enter" decides if magnet should be automatically activated when entering the world
* Filter function: Presets (hearts, mana stars, coins, pickup effects), blacklist/whitelist. Presets override black/whitelist
* Magnet stats are limited to sensible values (Range only goes up to about three screens in any direction)
* If you increase Vel or Acc too much from those recommended above, items might get "stuck" on you until you deactivate it again
* Beware of lag when increasing these values, especially range
* If the difference between velocity and acceleration is too big, items will go in circles around you or get stuck until you deactivate it
* After you change the values in the config, use the magnet again to take effect

## Localization
If you wish to contribute translations, visit the [tML wiki page](https://github.com/tModLoader/tModLoader/wiki/Contributing-Localization) on that.
This mod uses `.hjson` files in the `Localization` folder.
Translate things that are in english and commented out (either via `//` or `/* */`, remove the comment markers after translating)

List of localization contributors:
* Polish: **Taco**
* Russian: **GodHybrid**
* Italian: **Sea Noodle**
* Brazilian Portuguese: **marviN**
* Simplified Chinese: **906170601, Lacewing**
* Spanish: **CDT_Maggot**