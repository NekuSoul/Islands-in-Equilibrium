using Assets.Game.Code;
using UnityEngine;

namespace Assets.Game.Scripts
{
    public class Level : MonoBehaviour
    {
        public int Width;
        public int Height;
        public TileManager TileManager;
        public Sprite LevelSprite;

        public int CityCount;
        public int ForestCount;
        private bool _change;

        private GameObject[,] _tilesRep;
        private Tile[,] _tiles;

        // Fuck Unity!
        private Color BetterYellow = new Color(1, 1, 0, 1);

        // Use this for initialization
        public void Start()
        {

        }

        // Update is called once per frame
        public void Update()
        {

        }

        public void LoadLevel()
        {
            CityCount = 0;
            ForestCount = 0;
            _tiles = new Tile[Width, Height];
            _tilesRep = new GameObject[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Color color = LevelSprite.texture.GetPixel(x + (int)LevelSprite.textureRect.x, y + (int)LevelSprite.textureRect.y);

                    if (color == Color.blue)
                        ApplyTile(x, y, Tile.Water);
                    else if (color == Color.red)
                        ApplyTile(x, y, Tile.City);
                    else if (color == Color.red)
                        ApplyTile(x, y, Tile.Village);
                    else if (color == Color.magenta)
                        ApplyTile(x, y, Tile.Forest);
                    else if (color == BetterYellow)
                        ApplyTile(x, y, Tile.Desert);
                    else if (color == Color.green)
                        ApplyTile(x, y, Tile.Plain);
                    else if (color == Color.cyan)
                        ApplyTile(x, y, Tile.Village);
                    else if (color == Color.white)
                        ApplyTile(x, y, Tile.Mountain);
                }
            }
        }

        private void ApplyTile(int x, int y, Tile tile)
        {
            if (_tiles[x, y] == tile && _tilesRep[x, y] != null)
                return;

            // Remove old tile
            if (_tilesRep[x, y] != null)
                Destroy(_tilesRep[x, y]);

            // Update count

            if (_tiles[x, y] == Tile.City || _tiles[x, y] == Tile.DesertCity || _tiles[x, y] == Tile.Village || _tiles[x, y] == Tile.DesertVillage)
                CityCount--;

            if (_tiles[x, y] == Tile.Forest || _tiles[x, y] == Tile.MountainForest)
                ForestCount--;

            if (tile == Tile.City || tile == Tile.DesertCity || tile == Tile.Village || tile == Tile.DesertVillage)
                CityCount++;

            if (tile == Tile.Forest || tile == Tile.MountainForest)
                ForestCount++;

            // Place tile

            GameObject sourceTile = TileManager.GetByTile(tile);
            GameObject newTile = Instantiate(sourceTile);
            newTile.transform.parent = transform;
            newTile.name = string.Format("[{0}|{1}] {2}", x, y, sourceTile.name);
            newTile.transform.position = new Vector3(x - (Width - 1) / 2f, y - (Height - 1) / 2f, 0);
            _tiles[x, y] = tile;
            _tilesRep[x, y] = newTile;

            _change = true;
        }

        public bool AdvanceStep()
        {
            _change = false;
            SpreadTiles(Tile.City, Tile.DesertCity);
            SpreadTiles(Tile.Village, Tile.DesertVillage);
            SpreadTiles(Tile.Forest, Tile.MountainForest);
            return _change;
        }

        private void SpreadTiles(Tile tileA, Tile tileB)
        {
            Tile[,] oldTiles = _tiles;
            _tiles = new Tile[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    _tiles[x, y] = oldTiles[x, y];
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (oldTiles[x, y] == tileA || oldTiles[x, y] == tileB)
                        SpreadTile(x, y, tileA);
                }
            }
        }

        private void SpreadTile(int x, int y, Tile tile)
        {
            // Left
            if (x > 0)
            {
                PlaceTile(x - 1, y, tile);
            }

            // Right
            if (x + 1 < Width)
            {
                PlaceTile(x + 1, y, tile);
            }

            // Up
            if (y > 0)
            {
                PlaceTile(x, y - 1, tile);
            }

            // Down
            if (y + 1 < Height)
            {
                PlaceTile(x, y + 1, tile);
            }
        }

        private void PlaceTile(int x, int y, Tile tile)
        {
            Tile target = _tiles[x, y];

            switch (tile)
            {
                case Tile.City:
                    if (target == Tile.Forest || target == Tile.Plain || target == Tile.Desert)
                        ApplyTile(x, y, tile);
                    if (target == Tile.Desert)
                        ApplyTile(x, y, Tile.DesertCity);
                    break;

                case Tile.Village:
                    if (target == Tile.Plain || target == Tile.Desert)
                        ApplyTile(x, y, tile);
                    if (target == Tile.Desert)
                        ApplyTile(x, y, Tile.DesertCity);
                    break;

                case Tile.Forest:
                    if (target == Tile.Plain)
                        ApplyTile(x, y, tile);
                    if (target == Tile.Mountain)
                        ApplyTile(x, y, Tile.MountainForest);
                    break;

                default:
                    return;
            }
        }
    }
}
