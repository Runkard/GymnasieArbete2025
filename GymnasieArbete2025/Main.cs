using GymnasieArbete2025.Messanges;
using GymnasieArbete2025.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymnasieArbete2025
{
    public class Main : Game
    {
        GameState state;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D backgroundTexture;
        SpriteFont fontTexture;
        Ship player;
        GameObjectManager gameObjectManager;
        KeyboardState previousKbState;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = ScreenInfo.ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenInfo.ScreenHeight;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            if (GraphicsDevice == null)
            {
                graphics.ApplyChanges();
            }
            graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            Window.IsBorderless = true;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            player = new Ship(this);
            Components.Add(player);
            gameObjectManager = new GameObjectManager(this);
            Components.Add(gameObjectManager);

            Messenger.Instance.Register<GameStateChangedMessage>(this, OnGameStateChangedCallback);
            Messenger.Instance.Send(new GameStateChangedMessage() { NewState = GameState.GetReady });
            base.Initialize();
        }

        private void OnGameStateChangedCallback(GameStateChangedMessage message)
        {
            if (message.NewState == state)
                return;
            
            switch(message.NewState)
            {
                case GameState.GetReady:
                    gameObjectManager.ResetAsteroids();
                    player.Reset();
                    player.Enabled = gameObjectManager.Enabled = false; 
                    break;
                case GameState.Playing:
                    player.Enabled = gameObjectManager.Enabled = true; 
                    break;
                case GameState.Dead:
                case GameState.Won:
                    player.Enabled = gameObjectManager.Enabled = false;
                    break;
            }

            state = message.NewState;
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            backgroundTexture = Content.Load<Texture2D>("Space");
            fontTexture = Content.Load<SpriteFont>("Font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState kbState = Keyboard.GetState();

            switch (state)
            {
                case GameState.Dead:
                case GameState.Won:
                    if (kbState.IsKeyDown(Keys.Space) && previousKbState.IsKeyDown(Keys.Space))
                        Messenger.Instance.Send(new GameStateChangedMessage() { NewState = GameState.GetReady });
                    break;
                case GameState.GetReady :
                    if (kbState.IsKeyDown(Keys.Space) && previousKbState.IsKeyDown(Keys.Space))
                        Messenger.Instance.Send(new GameStateChangedMessage() { NewState = GameState.Playing });
                    break;
                case GameState.Playing:
                    gameObjectManager.CheckPlayerCollision(player);
                    break;
            }

            previousKbState = kbState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            for (int y = 0; y < ScreenInfo.ScreenHeight; y += backgroundTexture.Width)
            {
                for (int x = 0; x < ScreenInfo.ScreenWidth; x += backgroundTexture.Width)
                {
                    spriteBatch.Draw(backgroundTexture, new Vector2(x, y), Color.White);
                }
            }

            gameObjectManager.Draw(spriteBatch);
            player.Draw(spriteBatch);

            string overlayText = null;

            switch (state)
            {
                case GameState.GetReady:
                    overlayText = "Press space to start!";
                    break;
                case GameState.Dead:
                    overlayText = "GAME OVER!";
                    break;  
                case GameState.Won:
                    overlayText = "Congrats!";
                    break;
            }

            if (!string.IsNullOrEmpty(overlayText))
            {
                var size = fontTexture.MeasureString(overlayText);
                spriteBatch.DrawString(fontTexture, overlayText, ScreenInfo.CenterScreen - size / 2.0f, Color.Cyan);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
