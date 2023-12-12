using FinalProjectGameProgramming.Handlers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using FinalProjectGameProgramming.Entities;
using System.Linq;
using System;

namespace FinalProjectGameProgramming.GameStates
{
    internal class AboutState : IGameState
    {
        private GraphicsDeviceManager _graphics;
        private GameStateHandler _gameStateHandler;
        private SpriteFont _font;
        private ContentManager _content;
        private GraphicsDevice _graphicsDevice;
        private MouseState mState;
        private Texture2D aboutBG;
        private Texture2D knight;
        private Texture2D bigZombie;
        private Texture2D slime;
        private int screenWidth;
        private int screenHeight;
        
        private Button closedButton;


        public AboutState(GraphicsDeviceManager graphics, ContentManager content, GameStateHandler gameStateHandler, GraphicsDevice graphicsDevice, SpriteFont font)
        {
            _graphics = graphics;
            _content = content;
            _gameStateHandler = gameStateHandler;
            _font = font;
            _graphicsDevice = graphicsDevice;
             screenWidth = graphicsDevice.Viewport.Width;
            screenHeight = graphicsDevice.Viewport.Height;
            

            // Load the button textures
            Texture2D buttonReleasedTexture = content.Load<Texture2D>("Button_Release_01a2");
            Texture2D buttonPressedTexture = content.Load<Texture2D>("Button_Release_01a1");

            // Create the close button
            closedButton = new Button(buttonReleasedTexture, buttonPressedTexture, graphicsDevice, _font, "X");
            closedButton.SetPosition(new Vector2(100, 10)); // Set position for Close button
            closedButton.SetSize(new Vector2(50, 50)); // Set size for Close button

        }

        public void Enter() {
            aboutBG = _content.Load<Texture2D>("LeaderBoardBG");
            knight = _content.Load<Texture2D>("knight_f_idle_anim_f1");
            bigZombie = _content.Load<Texture2D>("big_zombie_run_anim_f0");
            slime = _content.Load<Texture2D>("swampy_anim_f0");
        }

        public void Exit() { }

        public void Update(GameTime gameTime)
        {
            mState = Mouse.GetState();
            closedButton.Update(mState);
            if (closedButton.isClicked)
            {
                _gameStateHandler.ChangeState(new MainMenu(_gameStateHandler, _font, _graphics, _content, _graphicsDevice));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // TODO: Add a background image
            spriteBatch.Begin();
            //draw bg image
            spriteBatch.Draw(aboutBG, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            // Get center position of X for the title
            int titleGap = 100;
            int nameGap = 100;
            int characterStartXPosition = 350; 
            int characterEndXPosition = screenWidth - 390;
            int characterYDifference = 20;
            int slimeYDifference = 10;
            int bigZombieStartXPosition = 320;
            int bigZombieEndXPosition = screenWidth - 410;
            int bigZombieYDifference = 50;
            SpriteEffects flipEffect = SpriteEffects.FlipHorizontally;
            Vector2 centerPosition = new Vector2(screenWidth / 2, 200);

            // Draw general information
            string information = $"\"Medieval Maze\" is a game developed by Group #4,";
            spriteBatch.DrawString(_font, information, new Vector2(250, 150), Color.White);
            spriteBatch.DrawString(_font, "including:", new Vector2(260, 190), Color.White);

            // Draw the knight and first developer
            // Set developer name and get size 
            string firstDeveloper = "Spencer McCrory";
            Vector2 firstDeveloperSize = _font.MeasureString(firstDeveloper);
            // Calculate the center position of the first developer
            Vector2 firstDeveloperPosition = new Vector2(centerPosition.X - firstDeveloperSize.X / 2, centerPosition.Y + titleGap);
            // Draw the knights and first developer
            spriteBatch.Draw(knight, new Vector2(characterStartXPosition, firstDeveloperPosition.Y - characterYDifference), Color.White);
            spriteBatch.DrawString(_font, firstDeveloper, firstDeveloperPosition, Color.White);
            // Flip the knight
            spriteBatch.Draw(
                texture: knight, // The texture to draw
                position: new Vector2(characterEndXPosition, firstDeveloperPosition.Y - characterYDifference),
                sourceRectangle: null, // The area of the texture to draw (use null for the whole texture)
                color: Color.White,
                rotation: 0f, // The rotation of the texture
                origin: Vector2.Zero, // The origin of the texture
                scale: 1f, // The scale factor
                effects: flipEffect, // The SpriteEffects value (SpriteEffects.FlipHorizontally)
                layerDepth: 0f // The depth of the layer
            );

            // Draw the slime and second developer
            // Set developer name and get size
            string secondDeveloper = "David Florez";
            Vector2 secondDeveloperSize = _font.MeasureString(secondDeveloper);
            // Calculate the center position of the second developer
            Vector2 secondDeveloperPosition = new Vector2(centerPosition.X - secondDeveloperSize.X / 2, firstDeveloperPosition.Y + nameGap);
            // Draw the slime and second developer
            spriteBatch.Draw(slime, new Vector2(characterStartXPosition, secondDeveloperPosition.Y - slimeYDifference), Color.White);
            spriteBatch.DrawString(_font, secondDeveloper, secondDeveloperPosition, Color.White);
            // Flip the slime
            spriteBatch.Draw(
                texture: slime, // The texture to draw
                position: new Vector2(characterEndXPosition, secondDeveloperPosition.Y - slimeYDifference),
                sourceRectangle: null, // The area of the texture to draw (use null for the whole texture)
                color: Color.White,
                rotation: 0f, // The rotation of the texture
                origin: Vector2.Zero, // The origin of the texture
                scale: 1f, // The scale factor
                effects: flipEffect, // The SpriteEffects value (SpriteEffects.FlipHorizontally)
                layerDepth: 0f // The depth of the layer
            );

            // Draw the big zombie and third developer
            // Set developer name and get size
            string thirdDevlper = "Hoang Tuan Nguyen";
            Vector2 thirdDeveloperSize = _font.MeasureString(thirdDevlper);
            // Calculate the center position of the third developer
            Vector2 thirdDeveloperPosition = new Vector2(centerPosition.X - thirdDeveloperSize.X / 2, secondDeveloperPosition.Y + nameGap);
            // Draw the big zombie and third developer
            spriteBatch.Draw(bigZombie, new Vector2(bigZombieStartXPosition, thirdDeveloperPosition.Y - bigZombieYDifference), Color.White);
            spriteBatch.DrawString(_font, thirdDevlper, thirdDeveloperPosition, Color.White);
            // Flip the big zombie
            spriteBatch.Draw(
                texture: bigZombie, // The texture to draw
				position: new Vector2(bigZombieEndXPosition, thirdDeveloperPosition.Y - bigZombieYDifference),
				sourceRectangle: null, // The area of the texture to draw (use null for the whole texture)
				color: Color.White,
				rotation: 0f, // The rotation of the texture 
				origin: Vector2.Zero, // The origin of the texture 
				scale: 1f, // The scale factor 
				effects: flipEffect, // The SpriteEffects value (SpriteEffects.FlipHorizontally)
				layerDepth: 0f // The depth of the layer
            );

            // Draw the close button
            closedButton.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
