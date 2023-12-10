using FinalProjectGameProgramming.Handlers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.GameStates
{
    internal class LevelTransitionState : IGameState
    {
        private GameStateHandler gameStateHandler;
        private SpriteFont font;
        private string message;
        private IGameState nextState;
        private GraphicsDeviceManager _graphics;


        public LevelTransitionState(GraphicsDeviceManager graphics, GameStateHandler gameStateHandler, SpriteFont font, string message, IGameState nextState)
        {
            this.gameStateHandler = gameStateHandler;
            this.font = font;
            this.message = message;
            this.nextState = nextState;
            _graphics = graphics;
        }

        public void Enter() { }

        public void Exit() { }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                gameStateHandler.ChangeState(nextState);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            // convert the message into a vector to get the size of the text
            Vector2 textSize = font.MeasureString(message);
            // calculate the position of the text to center it on the screen
            Vector2 textPosition = new Vector2(
                (_graphics.PreferredBackBufferWidth - textSize.X) / 2,
                (_graphics.PreferredBackBufferHeight - textSize.Y) / 2
            );
            spriteBatch.DrawString(font, message, textPosition, Color.White);
            spriteBatch.End();
        }
    }
}
