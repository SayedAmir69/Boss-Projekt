using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Boss
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _pixel;
        private Boss _boss;

        private Rectangle _btnStill;
        private Rectangle _btnManualPhase1Idle;
        private Rectangle _btnManualPhase1Shoot;
        private Rectangle _btnManualPhase2Charge;
        private Rectangle _btnManualPhase2Spawn;

        private Rectangle _btnAutoP1Idle;
        private Rectangle _btnAutoP1Shoot;
        private Rectangle _btnAutoP2Charge;
        private Rectangle _btnAutoP2Spawn;
        private Rectangle _btnAutoP3Orbit;

        private string[] _manualLabels;
        private string[] _autoLabels;
        private string _manualHeader;
        private string _autoHeader;

        private MouseState _prevMouse;

        private Dictionary<char, int[]> _font;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            var mode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            _graphics.PreferredBackBufferWidth = mode.Width;
            _graphics.PreferredBackBufferHeight = mode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            var bounds = GraphicsDevice.Viewport.Bounds;
            _boss = new Boss(_pixel, bounds);

            BuildFont();

            _manualHeader = "MANUAL CONTROLS";
            _autoHeader = "AUTOMATIC PHASES";

            _manualLabels = new[]
            {
                "MANUAL STILL",
                "MANUAL PHASE1 PATROL",
                "MANUAL PHASE1 SHOOT",
                "MANUAL PHASE2 CHARGE",
                "MANUAL PHASE2 SPAWN"
            };

            _autoLabels = new[]
            {
                "AUTO PHASE1 PATROL",
                "AUTO PHASE1 SHOOT",
                "AUTO PHASE2 CHARGE",
                "AUTO PHASE2 SPAWN",
                "AUTO PHASE3 ORBIT"
            };

            int scale = 3;
            int padding = 28;
            int h = 36;
            int margin = 18;
            int spacing = 8;

            int leftWidth = 0;
            for (int i = 0; i < _manualLabels.Length; i++)
            {
                var w = (int)MeasureText(_manualLabels[i], scale).X + padding;
                if (w > leftWidth) leftWidth = w;
            }

            int rightWidth = 0;
            for (int i = 0; i < _autoLabels.Length; i++)
            {
                var w = (int)MeasureText(_autoLabels[i], scale).X + padding;
                if (w > rightWidth) rightWidth = w;
            }

            int lx = margin;
            int ly = bounds.Height - margin - h;

            _btnStill = new Rectangle(lx, ly, leftWidth, h); ly -= (h + spacing);
            _btnManualPhase1Idle = new Rectangle(lx, ly, leftWidth, h); ly -= (h + spacing);
            _btnManualPhase1Shoot = new Rectangle(lx, ly, leftWidth, h); ly -= (h + spacing);
            _btnManualPhase2Charge = new Rectangle(lx, ly, leftWidth, h); ly -= (h + spacing);
            _btnManualPhase2Spawn = new Rectangle(lx, ly, leftWidth, h);

            int rx = bounds.Width - margin - rightWidth;
            int ry = bounds.Height - margin - h;

            _btnAutoP1Idle = new Rectangle(rx, ry, rightWidth, h); ry -= (h + spacing);
            _btnAutoP1Shoot = new Rectangle(rx, ry, rightWidth, h); ry -= (h + spacing);
            _btnAutoP2Charge = new Rectangle(rx, ry, rightWidth, h); ry -= (h + spacing);
            _btnAutoP2Spawn = new Rectangle(rx, ry, rightWidth, h); ry -= (h + spacing);
            _btnAutoP3Orbit = new Rectangle(rx, ry, rightWidth, h);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released)
            {
                if (_btnStill.Contains(mouse.Position))
                {
                    var t = typeof(Boss).Assembly.GetType("Boss.ManualStillState");
                    if (t != null) _boss?.ChangeState((IState)Activator.CreateInstance(t));
                }
                else if (_btnManualPhase1Idle.Contains(mouse.Position))
                {
                    var t = typeof(Boss).Assembly.GetType("Boss.ManualPhase1IdleState");
                    if (t != null) _boss?.ChangeState((IState)Activator.CreateInstance(t));
                }
                else if (_btnManualPhase1Shoot.Contains(mouse.Position))
                {
                    var t = typeof(Boss).Assembly.GetType("Boss.ManualPhase1ShootState");
                    if (t != null) _boss?.ChangeState((IState)Activator.CreateInstance(t));
                }
                else if (_btnManualPhase2Charge.Contains(mouse.Position))
                {
                    var t = typeof(Boss).Assembly.GetType("Boss.ManualPhase2ChargeState");
                    if (t != null) _boss?.ChangeState((IState)Activator.CreateInstance(t));
                }
                else if (_btnManualPhase2Spawn.Contains(mouse.Position))
                {
                    var t = typeof(Boss).Assembly.GetType("Boss.ManualPhase2SpawnState");
                    if (t != null) _boss?.ChangeState((IState)Activator.CreateInstance(t));
                }
                else if (_btnAutoP1Idle.Contains(mouse.Position))
                {
                    var t = typeof(Boss).Assembly.GetType("Boss.Phase1IdleState");
                    if (t != null) _boss?.ChangeState((IState)Activator.CreateInstance(t));
                }
                else if (_btnAutoP1Shoot.Contains(mouse.Position))
                {
                    var t = typeof(Boss).Assembly.GetType("Boss.Phase1ShootState");
                    if (t != null) _boss?.ChangeState((IState)Activator.CreateInstance(t));
                }
                else if (_btnAutoP2Charge.Contains(mouse.Position))
                {
                    var t = typeof(Boss).Assembly.GetType("Boss.Phase2ChargeState");
                    if (t != null) _boss?.ChangeState((IState)Activator.CreateInstance(t));
                }
                else if (_btnAutoP2Spawn.Contains(mouse.Position))
                {
                    var t = typeof(Boss).Assembly.GetType("Boss.Phase2SpawnState");
                    if (t != null) _boss?.ChangeState((IState)Activator.CreateInstance(t));
                }
                else if (_btnAutoP3Orbit.Contains(mouse.Position))
                {
                    var t = typeof(Boss).Assembly.GetType("Boss.Phase3OrbitState");
                    if (t != null) _boss?.ChangeState((IState)Activator.CreateInstance(t));
                }
            }

            _prevMouse = mouse;

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                var f = typeof(Boss).GetField("Health", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (f != null)
                {
                    int hp = (int)f.GetValue(_boss);
                    hp -= (int)(50 * gameTime.ElapsedGameTime.TotalSeconds);
                    if (hp < 0) hp = 0;
                    f.SetValue(_boss, hp);
                }
            }

            _boss?.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _boss?.Draw(_spriteBatch);

            int headerScale = 4;
            var leftHeaderSize = MeasureText(_manualHeader, headerScale);
            var leftHeaderPos = new Vector2(_btnManualPhase2Spawn.X + (_btnManualPhase2Spawn.Width - leftHeaderSize.X) / 2, _btnManualPhase2Spawn.Y - 28);
            DrawText(_manualHeader, leftHeaderPos, Color.White, headerScale);

            var rightHeaderSize = MeasureText(_autoHeader, headerScale);
            var rightHeaderPos = new Vector2(_btnAutoP3Orbit.X + (_btnAutoP3Orbit.Width - rightHeaderSize.X) / 2, _btnAutoP3Orbit.Y - 28);
            DrawText(_autoHeader, rightHeaderPos, Color.White, headerScale);

            var mouse = Mouse.GetState();

            DrawButton(_btnStill, _manualLabels[0], _btnStill.Contains(mouse.Position));
            DrawButton(_btnManualPhase1Idle, _manualLabels[1], _btnManualPhase1Idle.Contains(mouse.Position));
            DrawButton(_btnManualPhase1Shoot, _manualLabels[2], _btnManualPhase1Shoot.Contains(mouse.Position));
            DrawButton(_btnManualPhase2Charge, _manualLabels[3], _btnManualPhase2Charge.Contains(mouse.Position));
            DrawButton(_btnManualPhase2Spawn, _manualLabels[4], _btnManualPhase2Spawn.Contains(mouse.Position));

            DrawButton(_btnAutoP1Idle, _autoLabels[0], _btnAutoP1Idle.Contains(mouse.Position));
            DrawButton(_btnAutoP1Shoot, _autoLabels[1], _btnAutoP1Shoot.Contains(mouse.Position));
            DrawButton(_btnAutoP2Charge, _autoLabels[2], _btnAutoP2Charge.Contains(mouse.Position));
            DrawButton(_btnAutoP2Spawn, _autoLabels[3], _btnAutoP2Spawn.Contains(mouse.Position));
            DrawButton(_btnAutoP3Orbit, _autoLabels[4], _btnAutoP3Orbit.Contains(mouse.Position));

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawButton(Rectangle r, string text, bool hover)
        {
            var bg = hover ? Color.DarkSlateGray : Color.Black * 0.9f;
            _spriteBatch.Draw(_pixel, r, bg);

            _spriteBatch.Draw(_pixel, new Rectangle(r.X, r.Y, r.Width, 2), Color.White * 0.9f);
            _spriteBatch.Draw(_pixel, new Rectangle(r.X, r.Y + r.Height - 2, r.Width, 2), Color.White * 0.9f);
            _spriteBatch.Draw(_pixel, new Rectangle(r.X, r.Y, 2, r.Height), Color.White * 0.9f);
            _spriteBatch.Draw(_pixel, new Rectangle(r.X + r.Width - 2, r.Y, 2, r.Height), Color.White * 0.9f);

            int scale = 3;
            var textSize = MeasureText(text, scale);
            var textPos = new Vector2(r.X + (r.Width - textSize.X) / 2, r.Y + (r.Height - textSize.Y) / 2);
            DrawText(text.ToUpperInvariant(), textPos, Color.White, scale);
        }

        private void BuildFont()
        {
            _font = new Dictionary<char, int[]>();
            _font['A'] = new[] { 2, 5, 7, 5, 5 };
            _font['B'] = new[] { 6, 5, 6, 5, 6 };
            _font['C'] = new[] { 3, 4, 4, 4, 3 };
            _font['D'] = new[] { 6, 5, 5, 5, 6 };
            _font['E'] = new[] { 7, 4, 6, 4, 7 };
            _font['G'] = new[] { 3, 4, 5, 5, 3 };
            _font['H'] = new[] { 5, 5, 7, 5, 5 };
            _font['I'] = new[] { 7, 2, 2, 2, 7 };
            _font['L'] = new[] { 4, 4, 4, 4, 7 };
            _font['N'] = new[] { 5, 7, 7, 7, 5 };
            _font['O'] = new[] { 2, 5, 5, 5, 2 };
            _font['P'] = new[] { 6, 5, 6, 4, 4 };
            _font['R'] = new[] { 6, 5, 6, 5, 5 };
            _font['S'] = new[] { 3, 4, 2, 1, 6 };
            _font['T'] = new[] { 7, 2, 2, 2, 2 };
            _font['W'] = new[] { 5, 5, 5, 7, 5 };
            _font['D'] = new[] { 6, 5, 5, 5, 6 };

            _font['M'] = new[] { 5, 7, 7, 7, 5 };
            _font['U'] = new[] { 5, 5, 5, 5, 7 };

            _font['1'] = new[] { 2, 6, 2, 2, 7 };
            _font['2'] = new[] { 7, 1, 7, 4, 7 };

            _font[' '] = new[] { 0, 0, 0, 0, 0 };
            _font['?'] = new[] { 7, 1, 2, 0, 2 };
        }

        private Vector2 MeasureText(string text, int scale)
        {
            int w = 0;
            int h = 5 * scale;
            for (int i = 0; i < text.Length; i++)
            {
                w += 3 * scale;
                if (i < text.Length - 1) w += 1 * scale;
            }
            return new Vector2(w, h);
        }

        private void DrawText(string text, Vector2 pos, Color color, int scale)
        {
            float x = pos.X;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (!_font.ContainsKey(c)) c = '?';
                if (_font.ContainsKey(c))
                {
                    var rows = _font[c];
                    for (int ry = 0; ry < 5; ry++)
                    {
                        int row = rows[ry];
                        for (int rx = 0; rx < 3; rx++)
                        {
                            bool on = ((row >> (2 - rx)) & 1) == 1;
                            if (on)
                            {
                                var r = new Rectangle((int)(x + rx * scale), (int)(pos.Y + ry * scale), scale, scale);
                                _spriteBatch.Draw(_pixel, r, color);
                            }
                        }
                    }
                }
                x += (3 * scale) + (1 * scale);
            }
        }
    }
}
