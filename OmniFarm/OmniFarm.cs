using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace OmniFarm
{
    public class OmniFarm : Mod, IAssetLoader
    {
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Maps\Farm_Combat");
        }
        public T Load<T>(IAssetInfo asset)
        {
            return this.Helper.Content.Load<T>(@"assets\Farm_Combat.tbin");
        }

        private static OmniFarmConfig ModConfig;
        public override void Entry(IModHelper helper)
        {
            ModConfig = helper.ReadJsonFile<OmniFarmConfig>("config.json");
            if (ModConfig == null)
            {
                ModConfig = helper.ReadConfig<OmniFarmConfig>().Default();
                helper.WriteConfig<OmniFarmConfig>(ModConfig);
            }
            if (ModConfig.useOptionalCave == true)
            {
                helper.Content.AssetLoaders.Add(new MyAssetLoader(helper));
            }

            StardewModdingAPI.Events.TimeEvents.AfterDayStarted += AfterDayStarted;

        }

        static void AfterDayStarted(object sender, EventArgs e)
        {
            if (Game1.whichFarm == 4)
            {
                ChangeWarpPoints();

                if (ModConfig == null)
                    return;

                foreach (GameLocation GL in Game1.locations)
                {
                    if (GL is Farm)
                    {
                        Farm ourFarm = (Farm)GL;
                        foreach (Vector2 tile in ModConfig.stumpLocations)
                        {
                            ClearResourceClump(ourFarm.resourceClumps, tile);
                            ourFarm.addResourceClumpAndRemoveUnderlyingTerrain(ResourceClump.stumpIndex, 2, 2, tile);
                        }

                        foreach (Vector2 tile in ModConfig.hollowLogLocations)
                        {
                            ClearResourceClump(ourFarm.resourceClumps, tile);
                            ourFarm.addResourceClumpAndRemoveUnderlyingTerrain(ResourceClump.hollowLogIndex, 2, 2, tile);
                        }

                        foreach (Vector2 tile in ModConfig.meteoriteLocations)
                        {
                            ClearResourceClump(ourFarm.resourceClumps, tile);
                            ourFarm.addResourceClumpAndRemoveUnderlyingTerrain(ResourceClump.meteoriteIndex, 2, 2, tile);
                        }

                        foreach (Vector2 tile in ModConfig.boulderLocations)
                        {
                            ClearResourceClump(ourFarm.resourceClumps, tile);
                            ourFarm.addResourceClumpAndRemoveUnderlyingTerrain(ResourceClump.boulderIndex, 2, 2, tile);
                        }

                        foreach (Vector2 tile in ModConfig.largeRockLocations)
                        {
                            ClearResourceClump(ourFarm.resourceClumps, tile);
                            ourFarm.addResourceClumpAndRemoveUnderlyingTerrain(ResourceClump.mineRock1Index, 2, 2, tile);
                        }

                        //grass
                        if (Game1.IsWinter == false)
                            foreach (Vector2 tile in ModConfig.getGrassLocations())
                            {
                                TerrainFeature check;
                                if (ourFarm.terrainFeatures.TryGetValue(tile, out check))
                                {
                                    if (check is Grass)
                                    {
                                        ((Grass)check).numberOfWeeds.Value = ModConfig.GrassGrowth_1forsparse_4forFull;
                                    }
                                }
                                else
                                    ourFarm.terrainFeatures.Add(tile, new Grass(Grass.springGrass, ModConfig.GrassGrowth_1forsparse_4forFull));
                            }

                        //mine
                        Random randomGen = new Random();
                        foreach (Vector2 tile in ModConfig.getMineLocations())
                        {
                            if (ourFarm.isObjectAt((int)tile.X, (int)tile.Y))
                                continue;

                            //calculate ore spawn
                            if (Game1.player.hasSkullKey)
                            {
                                //5% chance of spawn ore
                                if (randomGen.NextDouble() < ModConfig.oreChance)
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
                                    if (randomGen.NextDouble() < ModConfig.oreChance)
                                    {
                                        addRandomOre(ref ourFarm, ref randomGen, 3, tile);
                                        continue;
                                    }
                                }
                                else if (Game1.player.deepestMineLevel > 40) //iron level
                                {
                                    if (randomGen.NextDouble() < ModConfig.oreChance)
                                    {
                                        addRandomOre(ref ourFarm, ref randomGen, 2, tile);
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (randomGen.NextDouble() < ModConfig.oreChance)
                                    {
                                        addRandomOre(ref ourFarm, ref randomGen, 1, tile);
                                        continue;
                                    }
                                }
                            }

                            //if ore doesnt spawn then calculate gem spawn
                            //1% to spawn gem
                            if (randomGen.NextDouble() < ModConfig.gemChance)
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
        }

        static void ChangeWarpPoints()
        {
            foreach (GameLocation GL in Game1.locations)
            {
                if (ModConfig.WarpFromForest.X != -1)
                {
                    if (GL is Forest)
                    {
                        foreach (Warp w in GL.warps)
                        {
                            if (w.TargetName.ToLower().Contains("farm"))
                            {
                                w.TargetX = (int)ModConfig.WarpFromForest.X;
                                w.TargetY = (int)ModConfig.WarpFromForest.Y;
                            }
                        }
                    }
                }

                if (ModConfig.WarpFromBackWood.X != -1)
                {
                    if (GL.Name.ToLower().Contains("backwood"))
                    {
                        foreach (Warp w in GL.warps)
                        {
                            if (w.TargetName.ToLower().Contains("farm"))
                            {
                                w.TargetX = (int)ModConfig.WarpFromBackWood.X;
                                w.TargetY = (int)ModConfig.WarpFromBackWood.Y;
                            }
                        }
                    }
                }

                if (ModConfig.WarpFromBusStop.X != -1)
                {
                    if (GL.Name.ToLower().Contains("busstop"))
                    {
                        foreach (Warp w in GL.warps)
                        {
                            if (w.TargetName.ToLower().Contains("farm"))
                            {
                                w.TargetX = (int)ModConfig.WarpFromBusStop.X;
                                w.TargetY = (int)ModConfig.WarpFromBusStop.Y;
                            }
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

        static void ClearResourceClump(IList<ResourceClump> input, Vector2 RCLocation)
        {
            for (int i = 0; i < input.Count; i++)
            {
                ResourceClump RC = input[i];
                if (RC.tile.Value == RCLocation)
                {
                    input.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
