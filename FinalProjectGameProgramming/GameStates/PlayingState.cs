using FinalProjectGameProgramming.Handlers;
using FinalProjectGameProgramming.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.GameStates
{
    /// <summary>
    /// The state of the game when the player is playing
    /// </summary>
    internal class PlayingState : IGameState
    {
        private Level currentLevel;
        private Level buttonPressedLevel;
        private GraphicsDeviceManager graphicsDeviceManager;
        private ContentManager content;
        private GraphicsDevice graphicsDevice;
        private GameStateHandler gameStateHandler;

        public PlayingState(GameStateHandler gameStateHandler, int levelNumber, int score)
        {
            this.gameStateHandler = gameStateHandler;
            graphicsDeviceManager = gameStateHandler.GraphicsDeviceManager;
            graphicsDevice = gameStateHandler.GraphicsDevice;
            content = gameStateHandler.Content;

            
            InitializeLevel(levelNumber, score);
        }

        private void InitializeLevel(int levelNumber, int score)
        {
            // Instantiate Level1 with required parameters
            switch (levelNumber)
            {
                case 1:
                    MediaPlayer.Play(MusicHandler.Level1Music);
                    currentLevel = new Level1(gameStateHandler);

                    break;
                case 2:
                    //currentLevel.UnloadContent();
                    MediaPlayer.Play(MusicHandler.Level2Music);
                    currentLevel = new Level2(gameStateHandler, score);
                    buttonPressedLevel = new Level2(gameStateHandler, score);

                    break;
                default:
                    throw new ArgumentException("Invalid level number");
            }

            Enter();
        }


        public void Enter()
        {
            currentLevel.Initialize();
            currentLevel.LoadLevelContent();
           MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.2f;
        }

        public void Exit()
        {
            GC.Collect(); // Unloads all content loaded via this ContentManager
           
        }

        public void Update(GameTime gameTime)
        {
            currentLevel.UpdateLevel(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentLevel.DrawLevel(spriteBatch);
        }
    }
}
