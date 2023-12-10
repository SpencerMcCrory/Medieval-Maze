using FinalProjectGameProgramming.Handlers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FinalProjectGameProgramming.GameStates
{
    internal class BackgroundImageState : IGameState
    {
        private Texture2D backgroundImage;
        ContentManager content;
        private GraphicsDevice graphicsDevice;
        private int screenWidth;
        private int screenHeight;
        IGameState nextState;
        GameStateHandler gameStateHandler;
        private double timer;



        public BackgroundImageState(ContentManager content, GraphicsDeviceManager graphicsDeviceManager, IGameState nextState, GameStateHandler gamestateHandler)
        {
            this.content = content;
            this.graphicsDevice = graphicsDeviceManager.GraphicsDevice;
            this.screenWidth = graphicsDevice.Viewport.Width;
            this.screenHeight = graphicsDevice.Viewport.Height;
            this.nextState = nextState;
            this.gameStateHandler = gamestateHandler;
            this.timer = 0;


        }
        public void Draw(SpriteBatch spriteBatch)
        {
            
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            spriteBatch.End();
        }

        public void Enter()
        {
            backgroundImage = content.Load<Texture2D>("MedievalMaze");
        }

        public void Exit()
        {
            backgroundImage.Dispose();
            GC.Collect();
        }

        public void Update(GameTime gameTime)
        {
            // Increment the timer by the elapsed game time in milliseconds
            timer += gameTime.ElapsedGameTime.TotalMilliseconds;

            // Check if 2 seconds have passed
            if (timer >= 3000)
            {
                timer = 0; // Reset the timer if you need to track this again later
                gameStateHandler.ChangeState(nextState);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                gameStateHandler.ChangeState(nextState);
            }
        }
    }
}
