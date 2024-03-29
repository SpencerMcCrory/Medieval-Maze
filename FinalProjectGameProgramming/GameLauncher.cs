﻿using FinalProjectGameProgramming.Handlers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;



namespace FinalProjectGameProgramming
{
    public class GameLauncher : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
        GameStateHandler gameStateHandler;
        Song mainMenuMusic;
        Song level1Music;
        public GameLauncher()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            // Change game title
            Window.Title = "Medieval Maze";
        }

        protected override void Initialize()
        {
            //setting up window specifications
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {   
            spriteBatch = new SpriteBatch(GraphicsDevice);
            MusicHandler.LoadContent(Content);
            SFXHandler.LoadContent(Content);
            SoundEffect.MasterVolume = 0.2f;
            gameStateHandler = new GameStateHandler(_graphics, Content, GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            gameStateHandler.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue); // Clear screen to a default color        
            gameStateHandler.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}