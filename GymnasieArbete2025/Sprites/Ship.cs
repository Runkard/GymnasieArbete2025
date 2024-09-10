using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using GymnasieArbete2025.Messanges;


namespace GymnasieArbete2025.Sprites
{
    class Ship : DrawableGameComponent, IGameObject
    {
        public bool IsDead { get; set; }
        public int Lifes { get; set; }
        public Vector2 Position { get; set; }
        public float Radius { get; set; }
        public Vector2 Speed { get; set; }
        public float Rotation { get; set; }
        public Vector2 Distance;
        public bool CanShoot { get { return reloadTimer == 0; } }

        private Texture2D playerTexture;
        private Texture2D lifeTexture;
        private int reloadTimer = 0;
        private Random rnd = new Random();

        public Ship(Game game) : base(game) 
        { 
            Radius = 15;
            Reset();
        }

        protected override void LoadContent()
        {
            playerTexture = Game.Content.Load<Texture2D>("Ship1");
            lifeTexture = Game.Content.Load<Texture2D>("MineHeart");

            base.LoadContent();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(playerTexture, Position, null, Color.White, Rotation + MathHelper.PiOver2,
                new Vector2(playerTexture.Width / 2, playerTexture.Height / 2), 1.0f, SpriteEffects.None, 0f);

            for (int i = 0; i < Lifes; i++)
            {
                spriteBatch.Draw(lifeTexture, new Vector2(40 + i * 50, 40), Color.White);
            }
        }

        public override void Update(GameTime gameTime)
        {
            MouseState _mouseState = Mouse.GetState();
            Distance.X = _mouseState.X - Position.X;
            Distance.Y = _mouseState.Y - Position.Y;
            Rotation = (float)Math.Atan2(Distance.Y, Distance.X);
            
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.W))
                Accelerate();
            if (state.IsKeyDown(Keys.A))
                AccelerateLeft();
            else if (state.IsKeyDown(Keys.D))
                AccelerateRight();
            if (state.IsKeyDown(Keys.S))
                Reverse();
            if (state.IsKeyDown(Keys.Space))
                Brake();

            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && CanShoot)
                Messenger.Instance.Send(new AddShotMessage() { Shot = Shoot() });

            Position += Speed;

            if (reloadTimer > 0)
                reloadTimer--;

            if (Position.X < ScreenInfo.GameArea.Left)
                Position = new Vector2(ScreenInfo.GameArea.Right, Position.Y);
            if (Position.X > ScreenInfo.GameArea.Right)
                Position = new Vector2(ScreenInfo.GameArea.Left, Position.Y);
            if (Position.Y < ScreenInfo.GameArea.Top)
                Position = new Vector2(Position.X, ScreenInfo.GameArea.Bottom);
            if (Position.Y > ScreenInfo.GameArea.Bottom)
                Position = new Vector2(Position.X, ScreenInfo.GameArea.Top);

            base.Update(gameTime);
        }
        public void Reset()
        {
            Speed = Vector2.Zero;
            Lifes = 3;
            Position = ScreenInfo.CenterScreen;
        }

        public void Accelerate()
        {
            Speed += new Vector2((float)Math.Cos(MathHelper.ToRadians(270)),
                (float)Math.Sin(MathHelper.ToRadians(270))) * 0.05f;

            if (Speed.LengthSquared() > 9)
                Speed = Vector2.Normalize(Speed) * 3;
        }

        public void AccelerateLeft()
        {
            Speed += new Vector2((float)Math.Cos(MathHelper.ToRadians(180)),
                (float)Math.Sin(MathHelper.ToRadians(180))) * 0.05f;

            if (Speed.LengthSquared() > 9)
                Speed = Vector2.Normalize(Speed) * 3;
        }
        public void AccelerateRight()
        {
            Speed += new Vector2((float)Math.Cos(MathHelper.ToRadians(0)),
                (float)Math.Sin(MathHelper.ToRadians(0))) * 0.05f;

            if (Speed.LengthSquared() > 9)
                Speed = Vector2.Normalize(Speed) * 3;
        }

        public void Reverse()
        {
            Speed += new Vector2((float)Math.Cos(MathHelper.ToRadians(90)),
                        (float)Math.Sin(MathHelper.ToRadians(90))) * 0.05f;

            if (Speed.LengthSquared() > 9)
                Speed = Vector2.Normalize(Speed) * 3; ;
        }

        public void Brake()
        {
            Speed *= 0.978f;
        }

        public Shot Shoot()
        {
            if (!CanShoot)
                return null;

            reloadTimer = 10;

            return new Shot()
            {
                Position = Position,
                Speed = Speed + 10f * new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation)),
                Rotation = rnd.Next() * MathHelper.TwoPi
            };
        }
    }
}
