﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SWEN_Game._Entities;
using SWEN_Game._Graphics;
using SWEN_Game._Items;
using SWEN_Game._Shooting;

namespace SWEN_Game._Utils
{
    /// <summary>
    /// Provides debug functionality such as input-based toggles, power-up injection,
    /// and hitbox/collision rendering for development and testing.
    /// </summary>
    public class Debug
    {
        private readonly Player _player;
        private readonly Renderer _renderer;
        private readonly PowerupManager _powerupManager;
        private readonly EnemyManager _enemyManager;
        private readonly PlayerWeapon _playerWeapon;
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        private bool _showHitboxes = false;

        public Debug(Player player, Renderer renderer, PowerupManager powerupmanager, EnemyManager enemyManager, PlayerWeapon playerWeapon)
        {
            _player = player;
            _renderer = renderer;
            _powerupManager = powerupmanager;
            _enemyManager = enemyManager;
            _playerWeapon = playerWeapon;
        }

        /// <summary>
        /// Checks debug input and executes relevant debug actions, such as toggling invincibility or applying powerups.
        /// </summary>
        public void DebugUpdate()
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            // DEBUG ITEMS
            if (_currentKeyboardState.IsKeyDown(Keys.F1) && !_previousKeyboardState.IsKeyDown(Keys.F1))
            {
                _powerupManager.AddItem(1); // itemID 1 = GunpowderPowerup
                _powerupManager.AddItem(2); // itemID 2 = MultiShotPowerup
                _powerupManager.AddItem(3); // itemID 3 = PiercerPowerup
                _powerupManager.AddItem(4); // itemID 4 = AdrenalinePowerup
                _powerupManager.AddItem(5); // itemID 5 = RocketspeedPowerup
                _powerupManager.AddItem(6); // itemID 6 = RancidEnergyDrinkPowerup
                _powerupManager.AddItem(7); // itemID 7 = ShadowBulletsPowerup
                _powerupManager.AddItem(8); // itemID 8 = QuickHandsPowerup
                _powerupManager.AddItem(9); // itemID 9 = SpicyNoodlesPowerup
                _powerupManager.AddItem(10); // itemID 10 = DeadeyePowerup
                _powerupManager.AddItem(11); // itemID 11 = HeavyMagsPowerup
                _powerupManager.AddItem(12); // itemID 12 = ExtremeTeapowderPowerup
                _powerupManager.AddItem(13); // itemID 13 = FrozenTearsPowerup
                _powerupManager.AddItem(14); // itemID 14 = SpeedColaPowerup

                PlayerGameData.Instance.UpdateWeaponGameData();
            }

            if (_currentKeyboardState.IsKeyDown(Keys.F4) && !_previousKeyboardState.IsKeyDown(Keys.F4))
            {
                _showHitboxes = !_showHitboxes;
            }
        }

        /// <summary>
        /// Draws visual debug overlays, including player hitboxes, real position rectangles, and collision boxes.
        /// </summary>
        public void DrawWorldDebug()
        {
            if (!_showHitboxes)
            {
                return;
            }

            Globals.SpriteBatch.Begin(
                SpriteSortMode.FrontToBack,
                transformMatrix: _renderer.CalcTranslation(),
                samplerState: SamplerState.PointClamp);

            DrawPlayerCollision();

            DrawAllWorldCollisions();

            DrawPlayerRealPos();

            DrawPlayerHitbox();

            DrawEnemyHitboxes();

            DrawPlayerBullets();

            Globals.SpriteBatch.End();
        }

        private void DrawPlayerCollision()
        {
            // Draw the player's collision box for debugging, using a pink overlay.
            Rectangle playerCollision = new Rectangle(
                (int)_player.Position.X + 4,
                (int)_player.Position.Y + 6,
                8,
                8);
            Globals.SpriteBatch.Draw(
                Globals.Content.Load<Texture2D>("debug_rect"),
                playerCollision,
                null,
                Color.Pink,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.992f);
        }

        private void DrawAllWorldCollisions()
        {
            // Draw any collision areas in red.
            foreach (var collision in Globals.Collisions)
            {
                Globals.SpriteBatch.Draw(
                    Globals.Content.Load<Texture2D>("debug_rect"),
                    collision,
                    null,
                    Color.Red,
                    0f,
                    new Vector2(0, 0),
                    SpriteEffects.None,
                    1f);
            }
        }

        private void DrawPlayerRealPos()
        {
            // Draw Player Position/Rectangle
            Rectangle realPositionRect = new Rectangle(
                (int)_player.RealPos.X,
                (int)_player.RealPos.Y,
                4,
                4);
            Globals.SpriteBatch.Draw(
                Globals.Content.Load<Texture2D>("debug_rect"),
                realPositionRect,
                null,
                Color.Blue,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.993f);
        }

        private void DrawPlayerHitbox()
        {
            // Draw the player's hitbox for debugging, using a purple overlay.
            Rectangle playerHitbox = _player.Hitbox;
            Globals.SpriteBatch.Draw(
                Globals.Content.Load<Texture2D>("debug_rect"),
                playerHitbox,
                null,
                Color.Purple,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.991f);
        }

        private void DrawEnemyHitboxes()
        {
            foreach (var enemy in _enemyManager.GetAllEnemies())
            {
                if (enemy.IsAlive)
                {
                    Globals.SpriteBatch.Draw(
                        Globals.Content.Load<Texture2D>("debug_rect"),
                        enemy.Hitbox,
                        null,
                        Color.Blue,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        0.999f);

                    DrawEnemyBullets(enemy);
                }
            }
        }

        private void DrawEnemyBullets(Enemy enemy)
        {
            // If enemy shoots bullets, draw bullet hitboxes
            if (enemy is IBulletShooter shooter)
            {
                foreach (var bullet in shooter.GetBullets())
                {
                    if (bullet.IsVisible)
                    {
                        Globals.SpriteBatch.Draw(
                            Globals.Content.Load<Texture2D>("debug_rect"),
                            bullet.BulletHitbox,
                            null,
                            Color.Black,
                            0f,
                            Vector2.Zero,
                            SpriteEffects.None,
                            0.998f);
                    }
                }
            }
        }

        private void DrawPlayerBullets()
        {
            var bullets = _playerWeapon.GetBullets();
            foreach (var bullet in bullets)
            {
                if (bullet.IsVisible)
                {
                    Globals.SpriteBatch.Draw(
                        Globals.Content.Load<Texture2D>("debug_rect"),
                        bullet.BulletHitbox,
                        null,
                        Color.Yellow, // Use a distinct color for player bullets
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        0.997f);
                }
            }
        }
    }
}