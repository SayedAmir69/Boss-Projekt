using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Boss
{
    // Simple Boss implementation with phases and behavior states.
    public class Boss
    {
        public Vector2 Position;
        public int Health = 100;

        private Texture2D _pixel;
        private Rectangle _bounds;
        private Color _color = Color.Red;

        private IState _currentState;
        private Random _rng = new Random();
        private double _phaseTimer;

        private List<Bullet> _bullets = new List<Bullet>();

        public Boss(Texture2D pixel, Rectangle bounds)
        {
            _pixel = pixel;
            _bounds = bounds;
            Position = new Vector2(bounds.Width / 2f - 50, 50);

            // start in manual still state
            ChangeState(new ManualStillState());
        }

        public void Update(GameTime gameTime)
        {
            // phased automatic switching based on health (increasing difficulty)
            if (Health <= 25 && !(_currentState is Phase4BaseState))
            {
                ChangeState(new Phase4RainState());
            }
            else if (Health <= 50 && !(_currentState is Phase3BaseState))
            {
                ChangeState(new Phase3OrbitState());
            }
            else if (Health <= 75 && !(_currentState is Phase2BaseState))
            {
                ChangeState(new Phase2ChargeState());
            }

            _currentState?.Update(this, gameTime);

            // update bullets
            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                _bullets[i].Update(gameTime);
                if (!_bounds.Contains(_bullets[i].Bounds))
                    _bullets.RemoveAt(i);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            // draw boss as a rectangle
            sb.Draw(_pixel, new Rectangle((int)Position.X, (int)Position.Y, 100, 60), _color);

            // draw health bar
            var barBack = new Rectangle((int)Position.X, (int)Position.Y - 10, 100, 6);
            sb.Draw(_pixel, barBack, Color.DarkGray);
            var hpWidth = (int)(100 * (Health / 100f));
            sb.Draw(_pixel, new Rectangle(barBack.X, barBack.Y, hpWidth, barBack.Height), Color.LimeGreen);

            // draw bullets
            foreach (var b in _bullets)
                b.Draw(sb, _pixel);

            _currentState?.Draw(this, sb);
        }

        public void ChangeState(IState newState)
        {
            _currentState?.Exit(this);
            _currentState = newState;
            _currentState?.Enter(this);
        }

        // helper to spawn bullets
        public void SpawnBullet(Vector2 pos, Vector2 velocity, Color color)
        {
            _bullets.Add(new Bullet(pos, velocity, color));
        }

        // helper to get a random float
        public float RandomFloat(float min, float max)
        {
            return (float)(_rng.NextDouble() * (max - min) + min);
        }

        // internal bullet class
        private class Bullet
        {
            public Vector2 Position;
            public Vector2 Velocity;
            public Color Color;

            public Rectangle Bounds => new Rectangle((int)Position.X - 4, (int)Position.Y - 4, 8, 8);

            public Bullet(Vector2 pos, Vector2 vel, Color color)
            {
                Position = pos;
                Velocity = vel;
                Color = color;
            }

            public void Update(GameTime gt)
            {
                Position += Velocity * (float)gt.ElapsedGameTime.TotalSeconds;
            }

            public void Draw(SpriteBatch sb, Texture2D pixel)
            {
                sb.Draw(pixel, Bounds, Color);
            }
        }
    }

    // State interfaces and implementations
    public interface IState
    {
        void Enter(Boss boss);
        void Exit(Boss boss);
        void Update(Boss boss, GameTime gameTime);
        void Draw(Boss boss, SpriteBatch sb);
    }

    // Manual still state: does nothing until changed
    public class ManualStillState : IState
    {
        public void Enter(Boss boss)
        {
            // no movement while in this state
        }

        public void Exit(Boss boss) { }

        public void Update(Boss boss, GameTime gameTime)
        {
            // remain stationary
        }

        public void Draw(Boss boss, SpriteBatch sb) { }
    }

    // Base for phase 2 states so we can detect phase
    public abstract class Phase2BaseState : IState
    {
        public abstract void Enter(Boss boss);
        public abstract void Exit(Boss boss);
        public abstract void Update(Boss boss, GameTime gameTime);
        public abstract void Draw(Boss boss, SpriteBatch sb);
    }

    // New base classes for Phase 3 and Phase 4
    public abstract class Phase3BaseState : IState
    {
        public abstract void Enter(Boss boss);
        public abstract void Exit(Boss boss);
        public abstract void Update(Boss boss, GameTime gameTime);
        public abstract void Draw(Boss boss, SpriteBatch sb);
    }

    public abstract class Phase4BaseState : IState
    {
        public abstract void Enter(Boss boss);
        public abstract void Exit(Boss boss);
        public abstract void Update(Boss boss, GameTime gameTime);
        public abstract void Draw(Boss boss, SpriteBatch sb);
    }

    // Phase 1 Idle: slow patrol, then switch to Shoot
    public class Phase1IdleState : IState
    {
        private double _timer;
        private int _dir = 1;

        public void Enter(Boss boss)
        {
            _timer = 1.5 + boss.RandomFloat(0f, 1f);
        }

        public void Exit(Boss boss) { }

        public void Update(Boss boss, GameTime gameTime)
        {
            _timer -= gameTime.ElapsedGameTime.TotalSeconds;

            // simple horizontal patrol inside bounds
            boss.Position.X += _dir * 30f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (boss.Position.X < 20) { boss.Position.X = 20; _dir = 1; }
            if (boss.Position.X > boss.Position.X + 400) { _dir = -1; } // guard, won't normally trigger

            if (_timer <= 0)
            {
                boss.ChangeState(new Phase1ShootState());
            }
        }

        public void Draw(Boss boss, SpriteBatch sb)
        {
            // no special draw
        }
    }

    // Phase 1 Shoot: fires a stream of bullets for a short time, then back to Idle
    public class Phase1ShootState : IState
    {
        private double _duration;
        private double _shootTimer;

        public void Enter(Boss boss)
        {
            _duration = 2.0;
            _shootTimer = 0.0;
        }

        public void Exit(Boss boss) { }

        public void Update(Boss boss, GameTime gameTime)
        {
            _duration -= gameTime.ElapsedGameTime.TotalSeconds;
            _shootTimer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (_shootTimer <= 0)
            {
                _shootTimer = 0.2; // fire rate
                // spawn bullets downward in a small spread
                for (int i = -1; i <= 1; i++)
                {
                    var dir = new Vector2(i * 40, 200);
                    var pos = boss.Position + new Vector2(50, 60);
                    boss.SpawnBullet(pos, dir, Color.OrangeRed);
                }
            }

            if (_duration <= 0)
                boss.ChangeState(new Phase1IdleState());
        }

        public void Draw(Boss boss, SpriteBatch sb)
        {
            // draw an indicator: boss brighter when shooting
            // We'll tint it by drawing an overlay
            sb.Draw(bossPixel(boss), new Rectangle((int)boss.Position.X, (int)boss.Position.Y, 100, 60), Color.Orange);
        }

        // helper to access boss's pixel (hacky but avoids exposing it)
        private Texture2D bossPixel(Boss b)
        {
            var t = typeof(Boss).GetField("_pixel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(b) as Texture2D;
            return t;
        }
    }

    // Manual variants of states: selected by buttons and do NOT auto-transition
    public class ManualPhase1IdleState : IState
    {
        private int _dir = 1;
        public void Enter(Boss boss) { }
        public void Exit(Boss boss) { }
        public void Update(Boss boss, GameTime gameTime)
        {
            boss.Position.X += _dir * 30f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            // use boss bounds via reflection
            var bounds = (Rectangle)typeof(Boss).GetField("_bounds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(boss);
            float minX = bounds.Left + 20;
            float maxX = bounds.Right - 120;
            if (boss.Position.X < minX) { boss.Position.X = minX; _dir = 1; }
            if (boss.Position.X > maxX) { boss.Position.X = maxX; _dir = -1; }
        }
        public void Draw(Boss boss, SpriteBatch sb) { }
    }

    public class ManualPhase1ShootState : IState
    {
        private double _shootTimer;
        public void Enter(Boss boss) { _shootTimer = 0.0; }
        public void Exit(Boss boss) { }
        public void Update(Boss boss, GameTime gameTime)
        {
            _shootTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (_shootTimer <= 0)
            {
                _shootTimer = 0.2;
                for (int i = -1; i <= 1; i++)
                {
                    var dir = new Vector2(i * 40, 200);
                    var pos = boss.Position + new Vector2(50, 60);
                    boss.SpawnBullet(pos, dir, Color.OrangeRed);
                }
            }
        }
        public void Draw(Boss boss, SpriteBatch sb)
        {
            sb.Draw(bossPixel(boss), new Rectangle((int)boss.Position.X, (int)boss.Position.Y, 100, 60), Color.Orange * 0.8f);
        }
        private Texture2D bossPixel(Boss b)
        {
            var t = typeof(Boss).GetField("_pixel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(b) as Texture2D;
            return t;
        }
    }

    public class ManualPhase2ChargeState : Phase2BaseState
    {
        private float _speed = 300f;
        private int _dir = 1;
        public override void Enter(Boss boss)
        {
            // change color to indicate phase 2
            var f = typeof(Boss).GetField("_color", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f.SetValue(boss, Color.Purple);
        }
        public override void Exit(Boss boss) { }
        public override void Update(Boss boss, GameTime gameTime)
        {
            boss.Position.X += _dir * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            var bounds = (Rectangle)typeof(Boss).GetField("_bounds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(boss);
            float minX = bounds.Left + 10;
            float maxX = bounds.Right - 100;
            if (boss.Position.X < minX) { boss.Position.X = minX; _dir = 1; }
            if (boss.Position.X > maxX) { boss.Position.X = maxX; _dir = -1; }
        }
        public override void Draw(Boss boss, SpriteBatch sb) { }
    }

    public class ManualPhase2SpawnState : Phase2BaseState
    {
        private bool _spawned;
        public override void Enter(Boss boss)
        {
            _spawned = false;
        }
        public override void Exit(Boss boss) { }
        public override void Update(Boss boss, GameTime gameTime)
        {
            if (!_spawned)
            {
                _spawned = true;
                var center = boss.Position + new Vector2(50, 30);
                int count = 12;
                for (int i = 0; i < count; i++)
                {
                    var ang = MathHelper.TwoPi * i / count;
                    var vel = new Vector2((float)Math.Cos(ang), (float)Math.Sin(ang)) * 140f;
                    boss.SpawnBullet(center, vel, Color.Yellow);
                }
            }
        }
        public override void Draw(Boss boss, SpriteBatch sb) { }
    }

    // Phase 3: spiral orbit and teleport burst states
    public class Phase3OrbitState : Phase3BaseState
    {
        private double _duration;
        private double _spawnTimer;
        private float _angle;

        public override void Enter(Boss boss)
        {
            _duration = 5.0;
            _spawnTimer = 0.03;
            _angle = 0f;
            var f = typeof(Boss).GetField("_color", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f.SetValue(boss, Color.SteelBlue);
        }

        public override void Exit(Boss boss) { }

        public override void Update(Boss boss, GameTime gameTime)
        {
            _duration -= gameTime.ElapsedGameTime.TotalSeconds;
            _spawnTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            _angle += 2f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // slight bobbing movement
            boss.Position.X += (float)Math.Sin(_angle * 0.5f) * 20f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_spawnTimer <= 0)
            {
                _spawnTimer = 0.05;
                var center = boss.Position + new Vector2(50, 30);
                var vel = new Vector2((float)Math.Cos(_angle), (float)Math.Sin(_angle)) * 160f;
                boss.SpawnBullet(center, vel, Color.Cyan);
            }

            if (_duration <= 0)
                boss.ChangeState(new Phase3TeleportState());
        }

        public override void Draw(Boss boss, SpriteBatch sb) { }
    }

    public class Phase3TeleportState : Phase3BaseState
    {
        private double _duration;
        private double _burstTimer;

        public override void Enter(Boss boss)
        {
            _duration = 4.0;
            _burstTimer = 0.0;
        }

        public override void Exit(Boss boss) { }

        public override void Update(Boss boss, GameTime gameTime)
        {
            _duration -= gameTime.ElapsedGameTime.TotalSeconds;
            _burstTimer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (_burstTimer <= 0)
            {
                _burstTimer = 0.6;
                // teleport to a random x position near top
                var bounds = (Rectangle)typeof(Boss).GetField("_bounds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(boss);
                float nx = boss.RandomFloat(bounds.Left + 20, bounds.Right - 120);
                boss.Position = new Vector2(nx, 40);

                // burst of bullets outward
                var center = boss.Position + new Vector2(50, 30);
                int count = 10;
                for (int i = 0; i < count; i++)
                {
                    var ang = MathHelper.TwoPi * i / count;
                    var vel = new Vector2((float)Math.Cos(ang), (float)Math.Sin(ang)) * boss.RandomFloat(80f, 220f);
                    boss.SpawnBullet(center, vel, Color.Magenta);
                }
            }

            if (_duration <= 0)
                boss.ChangeState(new Phase3OrbitState());
        }

        public override void Draw(Boss boss, SpriteBatch sb) { }
    }

    // Phase 4: enrage patterns - rain and ring
    public class Phase4RainState : Phase4BaseState
    {
        private double _duration;
        private double _spawnTimer;

        public override void Enter(Boss boss)
        {
            _duration = 6.0;
            _spawnTimer = 0.0;
            var f = typeof(Boss).GetField("_color", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f.SetValue(boss, Color.DarkRed);
        }

        public override void Exit(Boss boss) { }

        public override void Update(Boss boss, GameTime gameTime)
        {
            _duration -= gameTime.ElapsedGameTime.TotalSeconds;
            _spawnTimer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (_spawnTimer <= 0)
            {
                _spawnTimer = 0.06; // fast rain
                var bounds = (Rectangle)typeof(Boss).GetField("_bounds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(boss);
                float x = boss.RandomFloat(bounds.Left + 10, bounds.Right - 10);
                var pos = new Vector2(x, -10);
                var vel = new Vector2(0, boss.RandomFloat(180f, 360f));
                boss.SpawnBullet(pos, vel, Color.OrangeRed);
            }

            if (_duration <= 0)
                boss.ChangeState(new Phase4RingState());
        }

        public override void Draw(Boss boss, SpriteBatch sb) { }
    }

    public class Phase4RingState : Phase4BaseState
    {
        private double _duration;
        private bool _spawned;

        public override void Enter(Boss boss)
        {
            _duration = 4.0;
            _spawned = false;
        }

        public override void Exit(Boss boss) { }

        public override void Update(Boss boss, GameTime gameTime)
        {
            _duration -= gameTime.ElapsedGameTime.TotalSeconds;
            if (!_spawned)
            {
                _spawned = true;
                var center = boss.Position + new Vector2(50, 30);
                int waves = 3;
                for (int w = 0; w < waves; w++)
                {
                    int count = 18;
                    float speed = 60f + w * 40f;
                    for (int i = 0; i < count; i++)
                    {
                        var ang = MathHelper.TwoPi * i / count;
                        var vel = new Vector2((float)Math.Cos(ang), (float)Math.Sin(ang)) * speed;
                        boss.SpawnBullet(center, vel, Color.Gold);
                    }
                }
            }

            if (_duration <= 0)
                boss.ChangeState(new Phase4RainState());
        }

        public override void Draw(Boss boss, SpriteBatch sb) { }
    }

    // Phase 2 Charge: boss charges horizontally across the screen then switches to spawn
    public class Phase2ChargeState : Phase2BaseState
    {
        private double _duration;
        private float _speed;
        private int _dir = 1;

        public override void Enter(Boss boss)
        {
            _duration = 1.5;
            _speed = 300f;
            // change color to indicate phase 2
            var f = typeof(Boss).GetField("_color", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            f.SetValue(boss, Color.Purple);
            // start near top center
            boss.Position = new Vector2(boss.Position.X, 30);
        }

        public override void Exit(Boss boss) { }

        public override void Update(Boss boss, GameTime gameTime)
        {
            _duration -= gameTime.ElapsedGameTime.TotalSeconds;
            boss.Position.X += _dir * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (boss.Position.X < 10) { boss.Position.X = 10; _dir = 1; }
            if (boss.Position.X > 700) { boss.Position.X = 700; _dir = -1; }

            if (_duration <= 0)
            {
                boss.ChangeState(new Phase2SpawnState());
            }
        }

        public override void Draw(Boss boss, SpriteBatch sb) { }
    }

    // Phase 2 Spawn: spawn a burst of bullets in a radial pattern, then go back to Charge
    public class Phase2SpawnState : Phase2BaseState
    {
        private double _duration;
        private bool _spawned;

        public override void Enter(Boss boss)
        {
            _duration = 2.0;
            _spawned = false;
        }

        public override void Exit(Boss boss) { }

        public override void Update(Boss boss, GameTime gameTime)
        {
            _duration -= gameTime.ElapsedGameTime.TotalSeconds;
            if (!_spawned)
            {
                _spawned = true;
                // spawn radial bullets
                var center = boss.Position + new Vector2(50, 30);
                int count = 12;
                for (int i = 0; i < count; i++)
                {
                    var ang = MathHelper.TwoPi * i / count;
                    var vel = new Vector2((float)Math.Cos(ang), (float)Math.Sin(ang)) * 140f;
                    boss.SpawnBullet(center, vel, Color.Yellow);
                }
            }

            if (_duration <= 0)
                boss.ChangeState(new Phase2ChargeState());
        }

        public override void Draw(Boss boss, SpriteBatch sb) { }
    }
}
