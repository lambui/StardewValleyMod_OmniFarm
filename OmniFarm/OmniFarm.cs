using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using System.IO;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;
using StardewValley.Tools;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace OmniFarm
{
    public class OmniFarm : Mod
    {
        public override void Entry(IModHelper helper)
        {
            StardewModdingAPI.Events.TimeEvents.OnNewDay += UpdateTickEvent;

            /*
            StardewModdingAPI.Events.MineEvents.MineLevelChanged += (q, e) =>
            {
                if (ModConfig == null)
                    return;

                if ((Game1.currentLocation is MineShaft) == false)
                    return;

                List<Vector2> grassLocations = new List<Vector2>();
                AddVector2Grid(new Vector2(0, 0), new Vector2(50, 50), ref grassLocations);
                foreach (Vector2 tile in grassLocations)
                {
                    StardewValley.Object check;
                    if (Game1.currentLocation.objects.TryGetValue(tile, out check))
                    {
                        Log.Debug(check.name);
                        Log.Debug(check.bigCraftable);
                        Log.Debug(check.isOn);
                        Log.Debug(check.canBeGrabbed);
                        Log.Debug(check.canBeSetDown);
                        Log.Debug(check.parentSheetIndex);
                        Log.Debug(check.getHealth());
                        Log.Debug(check.fragility);
                        Log.Debug(check.type);
                        Log.Debug(check.GetType());
                    }
                }
            };
            */
        }

        static void UpdateTickEvent(object sender, EventArgs e)
        {
            List<Vector2> mineLocation = new List<Vector2>();
            AddVector2Grid(new Vector2(89, 3), new Vector2(96, 7), ref mineLocation);
            AddVector2Grid(new Vector2(97, 4), new Vector2(115, 10), ref mineLocation);
            AddVector2Grid(new Vector2(91, 8), new Vector2(96, 8), ref mineLocation);
            AddVector2Grid(new Vector2(92, 9), new Vector2(96, 9), ref mineLocation);
            AddVector2Grid(new Vector2(93, 10), new Vector2(96, 10), ref mineLocation);

            List<Vector2> stumpLocations = new List<Vector2>();
            AddVector2Grid(new Vector2(7, 24), new Vector2(7, 24), ref stumpLocations);
            AddVector2Grid(new Vector2(9, 26), new Vector2(9, 26), ref stumpLocations);
            AddVector2Grid(new Vector2(13, 27), new Vector2(13, 27), ref stumpLocations);

            List<Vector2> hollowLogLocations = new List<Vector2>();
            AddVector2Grid(new Vector2(3, 23), new Vector2(3, 23), ref hollowLogLocations);
            AddVector2Grid(new Vector2(4, 26), new Vector2(4, 26), ref hollowLogLocations);
            AddVector2Grid(new Vector2(18, 28), new Vector2(18, 28), ref hollowLogLocations);

            List<Vector2> grassLocations = new List<Vector2>();
            AddVector2Grid(new Vector2(99, 73), new Vector2(115, 84), ref grassLocations);
            AddVector2Grid(new Vector2(99, 96), new Vector2(115, 108), ref grassLocations);

            foreach (GameLocation GL in Game1.locations)
            {
                if (GL is Farm)
                {
                    Farm ourFarm = (Farm)GL;
                    foreach (Vector2 tile in stumpLocations)
                    {
                        ClearResourceClump(ref ourFarm.resourceClumps, tile);
                        ourFarm.addResourceClumpAndRemoveUnderlyingTerrain(ResourceClump.stumpIndex, 2, 2, tile);
                    }

                    foreach (Vector2 tile in hollowLogLocations)
                    {
                        ClearResourceClump(ref ourFarm.resourceClumps, tile);
                        ourFarm.addResourceClumpAndRemoveUnderlyingTerrain(ResourceClump.hollowLogIndex, 2, 2, tile);
                    }

                    GiantCrop haha = new GiantCrop(1, new Vector2(80, 16));

                    //grass
                    if (Game1.IsWinter == false)
                        foreach (Vector2 tile in grassLocations)
                        {
                            TerrainFeature check;
                            if (ourFarm.terrainFeatures.TryGetValue(tile, out check))
                            {
                                if (check is Grass)
                                {
                                    ((Grass)check).numberOfWeeds = 4;
                                }
                            }
                            else
                                ourFarm.terrainFeatures.Add(tile, new Grass(Grass.springGrass, 4));
                        }

                    //mine
                    Random randomGen = new Random();
                    foreach (Vector2 tile in mineLocation)
                    {
                        if (ourFarm.isObjectAt((int)tile.X, (int)tile.Y))
                            continue;

                        //calculate ore spawn
                        if (Game1.player.hasSkullKey)
                        {
                            //5% chance of spawn ore
                            if (randomGen.Next(0, 100) < 5)
                            {
                                addRandomOre(ref ourFarm, ref randomGen, 4, tile);
                                continue;
                            }
                        }
                        else
                        {
                            //check mine level
                            if (Game1.player.deepestMineLevel > 80) //gold level
                            {
                                //5% chance of spawn ore
                                if (randomGen.Next(0, 100) < 5)
                                {
                                    addRandomOre(ref ourFarm, ref randomGen, 3, tile);
                                    continue;
                                }
                            }
                            else if (Game1.player.deepestMineLevel > 40) //iron level
                            {
                                //4% chance of spawn ore
                                if (randomGen.Next(0, 100) < 4)
                                {
                                    addRandomOre(ref ourFarm, ref randomGen, 2, tile);
                                    continue;
                                }
                            }
                            else
                            {
                                //3% chance of spawn ore
                                if (randomGen.Next(0, 100) < 3)
                                {
                                    addRandomOre(ref ourFarm, ref randomGen, 1, tile);
                                    continue;
                                }
                            }
                        }

                        //if ore doesnt spawn then calculate gem spawn
                        //1% to spawn gem
                        if (randomGen.Next(0, 100) < 1)
                        {
                            //0.1% chance of getting mystic stone
                            if (Game1.player.hasSkullKey)
                                if (randomGen.Next(0, 100) < 1)
                                {
                                    ourFarm.setObject(tile, createOre("mysticStone", tile));
                                    continue;
                                }
                                else
                                if (randomGen.Next(0, 500) < 1)
                                {
                                    ourFarm.setObject(tile, createOre("mysticStone", tile));
                                    continue;
                                }

                            switch (randomGen.Next(0, 100) % 8)
                            {
                                case 0: ourFarm.setObject(tile, createOre("gemStone", tile)); break;
                                case 1: ourFarm.setObject(tile, createOre("diamond", tile)); break;
                                case 2: ourFarm.setObject(tile, createOre("ruby", tile)); break;
                                case 3: ourFarm.setObject(tile, createOre("jade", tile)); break;
                                case 4: ourFarm.setObject(tile, createOre("amethyst", tile)); break;
                                case 5: ourFarm.setObject(tile, createOre("topaz", tile)); break;
                                case 6: ourFarm.setObject(tile, createOre("emerald", tile)); break;
                                case 7: ourFarm.setObject(tile, createOre("aquamarine", tile)); break;
                                default: break;
                            }
                            continue;
                        }
                    }
                }
            }
        }

        static void addRandomOre(ref Farm input, ref Random randomGen, int highestOreLevel, Vector2 tileLocation)
        {
            switch (randomGen.Next(0, 100) % highestOreLevel)
            {
                case 0: input.setObject(tileLocation, createOre("copperStone", tileLocation)); break;
                case 1: input.setObject(tileLocation, createOre("ironStone", tileLocation)); break;
                case 2: input.setObject(tileLocation, createOre("goldStone", tileLocation)); break;
                case 3: input.setObject(tileLocation, createOre("iridiumStone", tileLocation)); break;
                default: break;
            }
        }

        static StardewValley.Object createOre(string oreName, Vector2 tileLocation)
        {
            switch (oreName)
            {
                case "mysticStone": return new StardewValley.Object(tileLocation, 46, "Stone", true, false, false, false);
                case "gemStone": return new StardewValley.Object(tileLocation, (Game1.random.Next(7) + 1) * 2, "Stone", true, false, false, false);
                case "diamond": return new StardewValley.Object(tileLocation, 2, "Stone", true, false, false, false);
                case "ruby": return new StardewValley.Object(tileLocation, 4, "Stone", true, false, false, false);
                case "jade": return new StardewValley.Object(tileLocation, 6, "Stone", true, false, false, false);
                case "amethyst": return new StardewValley.Object(tileLocation, 8, "Stone", true, false, false, false);
                case "topaz": return new StardewValley.Object(tileLocation, 10, "Stone", true, false, false, false);
                case "emerald": return new StardewValley.Object(tileLocation, 12, "Stone", true, false, false, false);
                case "aquamarine": return new StardewValley.Object(tileLocation, 14, "Stone", true, false, false, false);
                case "iridiumStone": return new StardewValley.Object(tileLocation, 765, 1);
                case "goldStone": return new StardewValley.Object(tileLocation, 764, 1);
                case "ironStone": return new StardewValley.Object(tileLocation, 290, 1);
                case "copperStone": return new StardewValley.Object(tileLocation, 751, 1);
                default: return null;
            }
        }

        static void ClearResourceClump(ref List<ResourceClump> input, Vector2 RCLocation)
        {
            for (int i = 0; i < input.Count; i++)
            {
                ResourceClump RC = input[i];
                if (RC.tile == RCLocation)
                {
                    input.RemoveAt(i);
                    i--;
                }
            }
        }

        static void ClearKey(ref SerializableDictionary<Vector2, TerrainFeature> input, Vector2 KeyToClear)
        {
            if (input.Remove(KeyToClear))
            {
                ClearKey(ref input, KeyToClear);
            }
        }

        static void AddVector2Grid(Vector2 TopLeftTile, Vector2 BottomRightTile, ref List<Vector2> grid)
        {
            if (TopLeftTile == BottomRightTile)
            {
                grid.Add(TopLeftTile);
                return;
            }

            int i = (int)TopLeftTile.X;
            while (i <= (int)BottomRightTile.X)
            {
                int j = (int)TopLeftTile.Y;
                while (j <= (int)BottomRightTile.Y)
                {
                    grid.Add(new Vector2(i, j));
                    j++;
                }
                i++;
            }
        }
    }
}
