using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Boss_Projekt
{
    public class Game1 : Game
    {
        public Vector2 Position;
        private int Health = 100;

        private Texture2D _pixel;
        private Rectangle _bounds;
        private Color _color = Color.Red;
       
    }
}
