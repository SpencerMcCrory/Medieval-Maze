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
        private GraphicsDeviceManager _graphics;
        private ContentManager _content;
        private GraphicsDevice _graphicsDevice;

        public PlayingState(GraphicsDeviceManager graphics, ContentManager content, GraphicsDevice graphicsDevice, int levelNumber)
        {
            _graphics = graphics;
            _content = content;
            _graphicsDevice = graphicsDevice;
            InitializeLevel(levelNumber);
        }

        private void InitializeLevel(int levelNumber)
        {
            // Instantiate Level1 with required parameters
            switch (levelNumber)
            {
                case 1:
                    currentLevel = new Level1(_graphics, _content, _graphicsDevice);
                    break;
                // Add cases for other levels if necessary
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
            throw new NotImplementedException();
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
