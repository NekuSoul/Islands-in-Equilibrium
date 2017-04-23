using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Game.Code
{
    public struct TileInfo
    {
        public int X;
        public int Y;
        public Tile Type;

        public TileInfo(int x, int y, Tile type)
        {
            X = x;
            Y = y;
            Type = type;
        }
    }
}
