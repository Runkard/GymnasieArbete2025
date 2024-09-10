using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymnasieArbete2025.Sprites
{
    enum AsteroidType
    {
        Big,
        Medium,
        Small
    }
    class Asteroid : GameObject
    {
        public AsteroidType Type { get; private set; }
        public float ExplosionScale { get; private set; }

        public Asteroid(AsteroidType type)
        {
            Type = type;

            switch (type)
            {
                case AsteroidType.Big:
                    Radius = 40;
                    ExplosionScale = 1f;
                    break;
                case AsteroidType.Medium:
                    Radius = 21;
                    ExplosionScale = 0.5f;
                    break;
                case AsteroidType.Small:
                    Radius = 8;
                    ExplosionScale = 0.2f;
                    break;
            }
        }
        public void Update(GameTime gameTime)
        {
            Position += Speed;

            if (Position.X < ScreenInfo.GameArea.Left)
                Position = new Vector2(ScreenInfo.GameArea.Right, Position.Y);
            if (Position.X > ScreenInfo.GameArea.Right)
                Position = new Vector2(ScreenInfo.GameArea.Left, Position.Y);
            if (Position.Y < ScreenInfo.GameArea.Top)
                Position = new Vector2(Position.X, ScreenInfo.GameArea.Bottom);
            if (Position.Y > ScreenInfo.GameArea.Bottom)
                Position = new Vector2(Position.X, ScreenInfo.GameArea.Top);

            Rotation += 0.04f;
            if (Rotation > MathHelper.TwoPi)
                Rotation = 0;
        }

        public static IEnumerable<Asteroid> BreakAsteroid(Asteroid asteroid)
        {
            List<Asteroid> asteroids = new List<Asteroid>();
            if (asteroid.Type == AsteroidType.Small)
                return asteroids;

            for (int i = 0; i < 3; i++)
            {
                var angle = (float)Math.Atan2(asteroid.Speed.Y, asteroid.Speed.X)
                    - MathHelper.PiOver4 + MathHelper.PiOver4 * i;

                asteroids.Add(new Asteroid(asteroid.Type + 1)
                {
                    Position = asteroid.Position,
                    Rotation = angle,
                    Speed = new Vector2((float)Math.Cos(angle),
                        (float)Math.Sin(angle) * asteroid.Speed.Length() * 0.5f)

                });
            }
            return asteroids;
        }
    }
}
