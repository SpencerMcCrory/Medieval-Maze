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
            MainMenu mainMenu = new MainMenu(this, menuFont, graphics, content, graphicsDevice);
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

