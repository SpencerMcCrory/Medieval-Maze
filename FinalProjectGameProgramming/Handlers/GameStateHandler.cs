using FinalProjectGameProgramming.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.Handlers
{
    internal class GameStateHandler
    {
        private IGameState currentState;
        private IGameState[,] gameStates;


        public GameStateHandler(GraphicsDeviceManager graphics, ContentManager content, GraphicsDevice graphicsDevice)
        {
            // Initialize the MainMenu as the starting state
            SpriteFont menuFont = content.Load<SpriteFont>("galleryFont"); // Load your font
            MainMenu nextState = new MainMenu(this, menuFont, graphics, content, graphicsDevice);
            BackgroundImageState backgroundImageState = new BackgroundImageState(content, graphics, nextState, this);
            ChangeState(backgroundImageState);

            // Delay for 2 seconds
            // System.Threading.Thread.Sleep(2000);
            ChangeState(mainMenu);
        }

        public void ChangeState(IGameState newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }

       

        public void Update(GameTime gameTime)
        {
            currentState?.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentState?.Draw(spriteBatch);
        }
    }
}

