using FinalProjectGameProgramming.Handlers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalProjectGameProgramming.Entities;
using Microsoft.Xna.Framework.Content;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace FinalProjectGameProgramming.GameStates
{
    internal class LevelTransitionState : IGameState
    {
        private GameStateHandler gameStateHandler;
        private SpriteFont font;
        private string message;
        private IGameState nextState;
        private GraphicsDeviceManager graphics;
        private int currentLevelNumber;
        private Texture2D buttonReleasedTexture;
        private Texture2D buttonPressedTexture;
        private CustomButton saveButton;
        private ContentManager content;
        private GraphicsDevice graphicsDevice;
        private int score;

        private MouseState mState;
        private CustomButton closedButton;


        public LevelTransitionState(GameStateHandler gameStateHandler, SpriteFont font, string message, IGameState nextState, int currentLevelNumber, int score)
        {
            this.gameStateHandler = gameStateHandler;
            content = gameStateHandler.Content;
            this.font = font;
            this.message = message;
            this.nextState = nextState;
            graphics = gameStateHandler.GraphicsDeviceManager;
            graphicsDevice = gameStateHandler.GraphicsDevice;
            this.currentLevelNumber = currentLevelNumber;
            //
            int screenWidth = graphics.PreferredBackBufferWidth;
            int screenHeight = graphics.PreferredBackBufferHeight;
            float centerX = screenWidth / 2;
            float centerY = screenHeight / 2;
            //
            //setting up texture for button
            buttonReleasedTexture = content.Load<Texture2D>("Button_Release_01a2");
            buttonPressedTexture = content.Load<Texture2D>("Button_Release_01a1");
            // Get the center position of the screen for the button




            // Adjust the position of the closedButton
            // Assuming you want this button to be in the top right, adjust as needed


            Texture2D buttonReleasedTexture2 = content.Load<Texture2D>("Button_Release_01a2");
            Texture2D buttonPressedTexture2 = content.Load<Texture2D>("Button_Release_01a1");


            // Create the save button
            saveButton = new CustomButton(buttonReleasedTexture2, buttonPressedTexture2, gameStateHandler.GraphicsDevice, font, "Save");
            Vector2 saveButtonSize = saveButton.GetSize();
            //// Position the save button
            saveButton.SetPosition(new Vector2(screenWidth / 2 - saveButtonSize.X / 2, screenHeight / 2 - saveButtonSize.Y +500 / 2));



            this.score = score;
        }

        public void Enter() {
           
        }

        public void Exit() { }

        public void Update(GameTime gameTime)
        {
            mState = Mouse.GetState();
            saveButton.Update(mState);
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                gameStateHandler.ChangeState(nextState);
                
            }
            if (saveButton.isClicked)
            {
                
                if (saveButton.buttonText == "Save")
                {
                    SaveHandler save = new SaveHandler(currentLevelNumber, score);
                    saveButton.SetText("Saved");
                }
                
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            // Draw the save button
            saveButton.Draw(spriteBatch);
            // convert the message into a vector to get the size of the text
            Vector2 textSize = font.MeasureString(message);
            // calculate the position of the text to center it on the screen
            Vector2 textPosition = new Vector2(
                (graphics.PreferredBackBufferWidth - textSize.X) / 2,
                (graphics.PreferredBackBufferHeight - textSize.Y) / 2
            );
            spriteBatch.DrawString(font, message, textPosition, Color.White);


            spriteBatch.End();
        }
    }
}
