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
    /// <summary>
    /// game state handler handles swithcing between states. ie from main menu to playing
    /// </summary>
    internal class GameStateHandler
    {
        private IGameState currentState;
        private IGameState[,] gameStates;
        public ContentManager Content { get; private set; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }


        public GameStateHandler(GraphicsDeviceManager graphics, ContentManager content, GraphicsDevice graphicsDevice)
        {
            this.Content = content;
            this.GraphicsDeviceManager = graphics;
            this.GraphicsDevice = graphicsDevice;
            // Initialize the MainMenu as the starting state
            SpriteFont menuFont = content.Load<SpriteFont>("galleryFont"); // Load your font
            MainMenu mainMenu = new MainMenu(this, menuFont);
            BackgroundImageState backgroundImageState = new BackgroundImageState(mainMenu, this);
            ChangeState(backgroundImageState);
        }

        /// <summary>
        /// changes the state of the game
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(IGameState newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }

        /// <summary>
        /// updates the game state
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            currentState?.Update(gameTime);
        }

        /// <summary>
        /// draws the game state
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            currentState?.Draw(spriteBatch);
        }
    }
}

