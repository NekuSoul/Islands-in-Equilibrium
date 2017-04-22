using System;
using Assets.Game.Code;
using UnityEngine;

namespace Assets.Game.Scripts
{
    public class TileManager : MonoBehaviour
    {
        public GameObject WaterTile;
        public GameObject PlainTile;
        public GameObject DesertTile;
        public GameObject CityTile;
        public GameObject VillageTile;
        public GameObject ForestTile;
        public GameObject MountainTile;
        public GameObject DesertCityTile;
        public GameObject DesertVillageTile;
        public GameObject MountainForestTile;

        public GameObject GetByTile(Tile tile)
        {
            switch (tile)
            {
                case Tile.Water:
                    return WaterTile;
                case Tile.Plain:
                    return PlainTile;
                case Tile.Desert:
                    return DesertTile;
                case Tile.City:
                    return CityTile;
                case Tile.Village:
                    return VillageTile;
                case Tile.Forest:
                    return ForestTile;
                case Tile.Mountain:
                    return MountainTile;
                case Tile.DesertCity:
                    return DesertCityTile;
                case Tile.DesertVillage:
                    return DesertVillageTile;
                case Tile.MountainForest:
                    return MountainForestTile;
                default:
                    throw new ArgumentOutOfRangeException("tile", tile, null);
            }
        }
    }
}
