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
    internal class PlayingState : IGameState
    {
        private Level currentLevel;
        private Level buttonPressedLevel;
        private GraphicsDeviceManager _graphics;
        private ContentManager _content;
        private GraphicsDevice _graphicsDevice;
        private GameStateHandler _gameStateHandler;

        public PlayingState(GraphicsDeviceManager graphics, ContentManager content, GraphicsDevice graphicsDevice, GameStateHandler gameStateHandler, int levelNumber, int score)
        {
            _graphics = graphics;
            _content = content;
            _graphicsDevice = graphicsDevice;
            _gameStateHandler = gameStateHandler;
            InitializeLevel(levelNumber, score);
        }

        private void InitializeLevel(int levelNumber, int score)
        {
            // Instantiate Level1 with required parameters
            switch (levelNumber)
            {
                case 1:
                    currentLevel = new Level1(_graphics, _content, _graphicsDevice, _gameStateHandler);

                    break;
                case 2:
                    //currentLevel.UnloadContent();
                    currentLevel = new Level2(_graphics, _content, _graphicsDevice, _gameStateHandler, score);
                    buttonPressedLevel = new Level2(_graphics, _content, _graphicsDevice, _gameStateHandler, score);

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
            MediaPlayer.Play(MusicHandler.Level1Music);
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
