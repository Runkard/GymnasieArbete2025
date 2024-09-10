using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using GymnasieArbete2025.Messanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Metadata;
using GymnasieArbete2025.Sprites;

namespace GymnasieArbete2025
{
    class GameObjectManager : DrawableGameComponent
    {
        Random rnd = new Random();

        List<Asteroid> asteroids = new List<Asteroid>();
        Texture2D asteroidBigTexture;
        Texture2D asteroidMediumTexture;
        Texture2D asteroidSmallTexture;

        List<Shot> shots = new List<Shot>();
        Texture2D bulletTexture;

        SoundEffect bulletSound;
        SoundEffect explosionSound;
        Texture2D explosionTexture;
        List<Explosion> explosions = new List<Explosion>();

        public GameObjectManager(Game game) : base(game) { }

        public override void Initialize()
        {
            ResetAsteroids();

            Messenger.Instance.Register<AddShotMessage>(this, AddShotMessageCallback);
            base.Initialize();
        }

        public void ResetAsteroids()
        {
            asteroids.Clear();  
            shots.Clear();
            explosions.Clear();

            while (asteroids.Count < 1)
            {
                var angle = rnd.Next() * MathHelper.TwoPi;
                var a = new Asteroid(AsteroidType.Big)
                {
                    Position = new Vector2(ScreenInfo.GameArea.Left + (float)rnd.NextDouble() * ScreenInfo.GameArea.Width,
                        ScreenInfo.GameArea.Top + (float)rnd.NextDouble() * ScreenInfo.GameArea.Height),
                    Rotation = angle,
                    Speed = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * rnd.Next(20, 60) / 30.0f
                };

                if (!ScreenInfo.RespawnArea.Contains((int)a.Position.X, (int)a.Position.Y))
                    asteroids.Add(a);
            }
        }

        private void AddShotMessageCallback(AddShotMessage message)
        {
            shots.Add(message.Shot);
            bulletSound.Play();
        }

        protected override void LoadContent()
        {
            bulletTexture = Game.Content.Load<Texture2D>("PlasmaBlast");

            explosionTexture = Game.Content.Load<Texture2D>("Explosion");
            asteroidBigTexture = Game.Content.Load<Texture2D>("Asteroid1");
            asteroidMediumTexture = Game.Content.Load<Texture2D>("AsteroidMedium");
            asteroidSmallTexture = Game.Content.Load<Texture2D>("AsteroidSmall");

            bulletSound = Game.Content.Load<SoundEffect>("laserSound");
            explosionSound = Game.Content.Load<SoundEffect>("explosionSound");

            base.LoadContent();
        }
        
        public void CheckPlayerCollision(Ship playerComponent)
        {
            var collidingAsteroid = asteroids.FirstOrDefault(a => a.CollidesWith(playerComponent));
            if (collidingAsteroid != null)
            {
                explosionSound.Play(0.5f, 0.0f, 0.0f);
                playerComponent.Lifes--;
                asteroids.Remove(collidingAsteroid);
                explosions.Add(new Explosion()
                {
                    Position = collidingAsteroid.Position,
                    Rotation = collidingAsteroid.Rotation,
                    Scale = collidingAsteroid.ExplosionScale
                });

                if (playerComponent.Lifes <= 0)
                    Messenger.Instance.Send(new GameStateChangedMessage() { NewState = GameState.Dead });
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Shot shot in shots)
            {
                shot.Update(gameTime);
                Asteroid asteroid = asteroids.FirstOrDefault(a => a.CollidesWith(shot));

                if (asteroid != null)
                {
                    asteroids.Remove(asteroid);
                    asteroids.AddRange(Asteroid.BreakAsteroid(asteroid));
                    explosions.Add(new Explosion()
                    {
                        Position = asteroid.Position,
                        Scale = asteroid.ExplosionScale
                    });
                    shot.IsDead = true;
                    explosionSound.Play(0.2f, 0f, 0f);
                }
            }

            foreach (Explosion explosion in explosions)
                explosion.Update(gameTime);

            foreach (Asteroid asteroid in asteroids)
                asteroid.Update(gameTime);

            shots.RemoveAll(s => s.IsDead || !ScreenInfo.GameArea.Contains((int)s.Position.X, (int)s.Position.Y));
            explosions.RemoveAll(e => e.IsDead);

            if (asteroids.Count == 0)
            {
                Messenger.Instance.Send(new GameStateChangedMessage() { NewState = GameState.Won });
            }

            base.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Shot s in shots)
            {
                spriteBatch.Draw(bulletTexture, s.Position, null, Color.White, s.Rotation,
                    new Vector2(bulletTexture.Width / 2, bulletTexture.Height / 2), 1.0f, SpriteEffects.None, 0f);
            }

            foreach (Asteroid asteroid in asteroids)
            {
                Texture2D asteroidTexture = asteroidSmallTexture;

                switch (asteroid.Type)
                {
                    case AsteroidType.Big: asteroidTexture = asteroidBigTexture; break;
                    case AsteroidType.Medium: asteroidTexture = asteroidMediumTexture; break;
                }

                spriteBatch.Draw(asteroidTexture, asteroid.Position, null, Color.White, asteroid.Rotation,
                    new Vector2(asteroidTexture.Width / 2, asteroidTexture.Height / 2), 1.0f, SpriteEffects.None, 0f);
            }

            foreach (Explosion explosion in explosions)
            {
                spriteBatch.Draw(explosionTexture, explosion.Position, null, explosion.Color, explosion.Rotation,
                    new Vector2(explosionTexture.Width / 2, explosionTexture.Height / 2), explosion.Scale, SpriteEffects.None, 0f);
            }
        }
    }
}
