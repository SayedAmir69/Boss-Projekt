using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Boss_Projekt
{
    public class Game1 : Game
    {
        public Vector2 Position;
        private int Health = 100;

        private Texture2D _pixel;
        private Rectangle _bounds;
        private Color _color = Color.Red;

        private DesignerActionUIStateChangeEventArgs _currentState;
        private Random _rng = new Random();
        private double _phaseTimer;

        private List <Bullet> _bullets = new List<Bullet>();


        public Game1(Texture2D pixel, Rectangle bounds)
        {
            _pixel = pixel;
            _bounds = bounds;
            Position = new Vector2(bounds.Width / 2f - 50, 50);

            ChangeState(new ManualStillState());
        }

        public void Update(GameTime gameTime)
        {
            if(Health <= 25 && !(CurrentState is Phase4BaseState))
            {
                ChangeState(new Phase4BaseState());
            }
            else if(Health <= 50 && !(CurrentState is Phase3BaseState))
            {
                ChangeState(new Phase3OrbitState());
            }
            else if(Health <= 75 && !(CurrentState is Phase2BaseState))
            {
                ChangeState(new Phase2ChargeState());
            }

            _currentState?.Update(this ,gameTime);

            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                _bullets[i].Update(gameTime);
                if (!_bounds.Container(_bullets[i].Bounds))
                    _bullets.RemoveAt(i);
               
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_pixel, new Rectangle((int)Position.X, (int)Position.Y, 100, 60), _color);

            var barBack = new Rectangle((int)Position.X, (int)Position.Y - 10, 100, 6);
            spriteBatch.Draw(_pixel, barBack, Color.DarkGray);
            var hpWidth = (int)(100 * (Health / 100f));
            spriteBatch.Draw(_pixel, new Rectangle(barBack.X, barBack.Y, hpWidth, barBack.Height), Color.LimeGreen);

            foreach (var b in _bullets)
                b.Draw(spriteBatch, _pixel);

            _currentstate?.Draw(this, spriteBatch);

        }

        public void ChagneState(IState newState)
        {
            _currentState?.Exit(this);
            _currentState = newState;
            _currentState?.Enter(this);
        }
        public void 
}
