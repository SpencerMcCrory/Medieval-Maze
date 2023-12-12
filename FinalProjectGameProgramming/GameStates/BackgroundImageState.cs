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
/*Initial load screen that is shown on launch*/
    internal class BackgroundImageState : IGameState
    {
        private Texture2D[] backgroundImages;
        ContentManager content;
        private GraphicsDevice graphicsDevice;
        private GraphicsDeviceManager graphicsDeviceManager;
        private int screenWidth;
        private int screenHeight;
        IGameState nextState;
        GameStateHandler gameStateHandler;
        private double timer;
        AnimationHandler animation;
        private int currentFrame;
        private double animationTimer;

        public BackgroundImageState(IGameState nextState, GameStateHandler gamestateHandler)
        {
            this.gameStateHandler = gamestateHandler;
            this.content = gameStateHandler.Content;
            this.graphicsDevice = gamestateHandler.GraphicsDevice;
            this.graphicsDeviceManager = gameStateHandler.GraphicsDeviceManager;
            this.screenWidth = graphicsDevice.Viewport.Width;
            this.screenHeight = graphicsDevice.Viewport.Height;
            this.nextState = nextState;
            
            this.timer = 0;


        }
        public void Draw(SpriteBatch spriteBatch)
        {
            
            spriteBatch.Begin();
            spriteBatch.Draw(animation.Frames[currentFrame], new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            spriteBatch.End();
        }

        public void Enter()
        {
            backgroundImages = new Texture2D[4];
            for (int i = 0; i < 4; i++)
            {
                backgroundImages[i] = content.Load<Texture2D>($"MedievalMaze{i+1}");
            }
            animation = new AnimationHandler(backgroundImages, 0.08);

        }

        public void Exit()
        {
            backgroundImages[0].Dispose();
            backgroundImages[1].Dispose();
            backgroundImages[2].Dispose();
            //backgroundImages[3].Dispose();
            GC.Collect();
        }

        public void Update(GameTime gameTime)
        {
            // Increment the timer by the elapsed game time in milliseconds
            timer += gameTime.ElapsedGameTime.TotalMilliseconds;

            // Check if 2 seconds have passed
            if (timer >= 5000)
            {
                timer = 0; // Reset the timer if you need to track this again later
                gameStateHandler.ChangeState(nextState);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                gameStateHandler.ChangeState(nextState);
            }

            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTimer > animation.TimePerFrame)
            {
                currentFrame = (currentFrame + 1) % 4;
                animationTimer -= animation.TimePerFrame;
            }
        }
    }
}
