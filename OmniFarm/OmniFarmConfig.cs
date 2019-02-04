using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace OmniFarm
{
    internal class OmniFarmConfig
    {
        public bool useOptionalCave = false;
        public int GrassGrowth_1forsparse_4forFull = 4;
        private List<Vector2> mineLocations = new List<Vector2>();
        private List<Vector2> grassLocations = new List<Vector2>();

        public List<Tuple<Vector2, Vector2>> mineAreas { get; set; } = new List<Tuple<Vector2, Vector2>>();
        public List<Tuple<Vector2, Vector2>> grassAreas { get; set; } = new List<Tuple<Vector2, Vector2>>();

        public List<Vector2> stumpLocations { get; set; } = new List<Vector2>();
        public List<Vector2> hollowLogLocations { get; set; } = new List<Vector2>();
        public List<Vector2> meteoriteLocations { get; set; } = new List<Vector2>();
        public List<Vector2> boulderLocations { get; set; } = new List<Vector2>();
        public List<Vector2> largeRockLocations { get; set; } = new List<Vector2>();

        public double oreChance { get; set; } = 0.05;
        public double gemChance { get; set; } = 0.01;

        public Vector2 WarpFromForest { get; set; } = new Vector2(32, 117);
        public Vector2 WarpFromBackWood { get; set; } = new Vector2(-1, -1);
        public Vector2 WarpFromBusStop { get; set; } = new Vector2(-1, -1);

        public OmniFarmConfig() { }

        public OmniFarmConfig Default()
        {
            //mine
            mineAreas.Add(new Tuple<Vector2, Vector2>(new Vector2(89, 3), new Vector2(96, 7)));
            mineAreas.Add(new Tuple<Vector2, Vector2>(new Vector2(97, 4), new Vector2(115, 10)));
            mineAreas.Add(new Tuple<Vector2, Vector2>(new Vector2(91, 8), new Vector2(96, 8)));
            mineAreas.Add(new Tuple<Vector2, Vector2>(new Vector2(92, 9), new Vector2(96, 9)));
            mineAreas.Add(new Tuple<Vector2, Vector2>(new Vector2(93, 10), new Vector2(96, 10)));

            //grass
            grassAreas.Add(new Tuple<Vector2, Vector2>(new Vector2(99, 73), new Vector2(115, 84)));
            grassAreas.Add(new Tuple<Vector2, Vector2>(new Vector2(99, 96), new Vector2(115, 108)));

            //stump
            List<Vector2> stumpTemp = new List<Vector2>();
            AddVector2Grid(new Vector2(7, 24), new Vector2(7, 24), ref stumpTemp);
            AddVector2Grid(new Vector2(9, 26), new Vector2(9, 26), ref stumpTemp);
            AddVector2Grid(new Vector2(13, 27), new Vector2(13, 27), ref stumpTemp);
            stumpLocations = stumpTemp;

            //hollow log
            List<Vector2> hollowLogTemp = new List<Vector2>();
            AddVector2Grid(new Vector2(3, 23), new Vector2(3, 23), ref hollowLogTemp);
            AddVector2Grid(new Vector2(4, 26), new Vector2(4, 26), ref hollowLogTemp);
            AddVector2Grid(new Vector2(18, 28), new Vector2(18, 28), ref hollowLogTemp);
            hollowLogLocations = hollowLogTemp;

            //meterorite

            //boulder

            //large rock

            return this;
        }

        public List<Vector2> getMineLocations()
        {
            mineLocations.Clear();
            foreach (Tuple<Vector2, Vector2> T in mineAreas)
            {
                AddVector2Grid(T.Item1, T.Item2, ref mineLocations);
            }
            return mineLocations;
        }

        public List<Vector2> getGrassLocations()
        {
            grassLocations.Clear();
            foreach (Tuple<Vector2, Vector2> T in grassAreas)
            {
                AddVector2Grid(T.Item1, T.Item2, ref grassLocations);
            }
            return grassLocations;
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