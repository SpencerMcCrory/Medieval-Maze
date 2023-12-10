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



        public LevelTransitionState(GameStateHandler gameStateHandler, SpriteFont font, string message, IGameState nextState)
        {
            this.gameStateHandler = gameStateHandler;
            this.font = font;
            this.message = message;
            this.nextState = nextState;
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
            spriteBatch.DrawString(font, message, new Vector2(100, 100), Color.White);
            spriteBatch.End();
        }
    }
}
