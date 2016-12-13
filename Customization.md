#Customization

####If you are not already familiar with the coordinate system of the map. Read this: 

This is the coordinate of all maps in Stardew Valley:

```
origin  
(0, 0) ------------>  X 
  |  
  |  
  |  
  |  
  |  
  |
  V 
  
  Y
```

##config.json
If you don't find this file where it's supposed to be try lauching SMAPI. The file will be generated automatically.
###1. Lists of pairs of coordinate
**mineAreas** stores a list of pairs of tile coordinates that defines the areas where ores, gem stones can spawn.  
  + each pair stores 2 tile coordinates that make up a rectangular area of the map, Item1 and Item2.  
  + Item1 is top left tile, Item2 is bottom right tile.
  + **Adding/Removing** pairs of tile coordinates to **add/remove** areas that ore/gem can spawn in each day.
  
``` javascript
mineAreas: [ 
  { // this is first pair of coordinate 
    "Item1": "89, 3", //top left
    "Item2": "96, 7"  //bottom right
  },
  { // this is second pair of coordinate 
    "Item1": "97, 4",
    "Item2": "115, 10"
  },
  ... // so on and so forth
]
```
**grassAreas** are the same as **mineAreas** above. It defines areas that animal grass grows each day.  
  
###2. Lists of coordinate
These store tile coordinates (not pairs like before) of where objects should spawn each day.  
These coordinates tell the game where the top left of the 2x2 spawn objects will be.  
**Adding/Removing** tile coordinates to **spawn/prevent** objects at those locations each day.  
+ **stumpLocations** for tree stump.  
+ **hollowLogLocations** for hollow log.
+ **meteoriteLocations** for meteorite.
+ **boulderLocations** for large boulder.
+ **largeRockLocations** for large rock.

```javascript
"stumpLocations": [
    "7, 24",  // first location
    "9, 26",  // second location
    "13, 27", // third location
    ...       // so on and so forth
  ],
```

###3. Ore/Gem chance
**oreChance** is the spawn chance of ore (copper/silver/gold/iridium) on each mining area tile.
+ There is an equal chance for spawned in ore to be one of the four ore variations.
+ Default is 0.05 (5%).

**gemChance** is the spawn chance of gem (diamond, ruby, emerald, aquamarine...) on each mining area tile.
+ **oreChance** will be calculated first. If tile doesn't spawn ore then the mod will calculate **gemChance**.
+ There is an equal chance for spawned in gem to be one of the many gem variations.
+ Default is 0.01 (1%).

###4. Modifying warp coordinates of other game locations leading to Farm
These stores a single tile coordinate of the Farm where the player will be warped in.  
If entry's value is ```"-1, -1"```, the mod ignores the entry and does not override vanilla game warps.  
+ **WarpFromForest**: tile where player will appear in Farm coming from the Forest South of Farm.  
+ **WarpFromBackWood**: tile where player will appear in Farm coming from the little wood North of Farm.  
+ **WarpFromBusStop**: tile where player will appear in Farm coming from the bus stop area.  

These are default value:
```javascript
  "WarpFromForest": "32, 117",
  "WarpFromBackWood": "-1, -1",
  "WarpFromBusStop": "-1, -1"
```

###5. Restore Default config values:
Simply delete config.json and relaunch SMAPI.
