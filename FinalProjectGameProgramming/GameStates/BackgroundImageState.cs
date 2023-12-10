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



        public BackgroundImageState(ContentManager content, GraphicsDeviceManager graphicsDeviceManager, IGameState nextState, GameStateHandler gamestateHandler)
        {
            this.content = content;
            this.graphicsDevice = graphicsDeviceManager.GraphicsDevice;
            this.screenWidth = graphicsDevice.Viewport.Width;
            this.screenHeight = graphicsDevice.Viewport.Height;
            this.nextState = nextState;
            this.gameStateHandler = gamestateHandler;


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
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                gameStateHandler.ChangeState(nextState);
            }
        }
    }
}
