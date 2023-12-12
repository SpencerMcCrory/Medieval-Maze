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
        private GraphicsDeviceManager graphicsDeviceManager;
        private int currentLevelNumber;
        private Texture2D buttonReleasedTexture;
        private Texture2D buttonPressedTexture;
        private CustomButton saveButton;
        private CustomButton mainMenuButton;
        private CustomButton continueButton;
        private ContentManager content;
        private GraphicsDevice graphicsDevice;
        private int score;

        private MouseState mState;
        private CustomButton closedButton;
        private Texture2D backgroundImage;
        private const int BUTTON_GAP = 80;
        CustomButton[] buttons;



        public LevelTransitionState(GameStateHandler gameStateHandler, SpriteFont font, string message, IGameState nextState, int currentLevelNumber, int score)
        {
            this.gameStateHandler = gameStateHandler;
            content = gameStateHandler.Content;
            this.font = font;
            this.message = message;
            this.nextState = nextState;
            graphicsDeviceManager = gameStateHandler.GraphicsDeviceManager;
            graphicsDevice = gameStateHandler.GraphicsDevice;
            this.currentLevelNumber = currentLevelNumber;
            backgroundImage = content.Load<Texture2D>("TransitionBG");
            int screenWidth = graphicsDeviceManager.PreferredBackBufferWidth;
            int screenHeight = graphicsDeviceManager.PreferredBackBufferHeight;
            float centerX = screenWidth / 2;
            float centerY = screenHeight / 2;
            //
            //setting up texture for button
            buttonReleasedTexture = content.Load<Texture2D>("Button_Release_01a2");
            buttonPressedTexture = content.Load<Texture2D>("Button_Release_01a1");
            // Get the center position of the screen for the button

            // Create the save button
            saveButton = new CustomButton(buttonReleasedTexture, buttonPressedTexture, gameStateHandler.GraphicsDevice, font, "Save");
            saveButton.SetSize(new Vector2(350, 100));
            Vector2 saveButtonSize = saveButton.GetSize();
            //// Position the save button
            saveButton.SetPosition(new Vector2(screenWidth / 2 - saveButtonSize.X , screenHeight / 2 - saveButtonSize.Y +600 / 2));
            
            mainMenuButton = new CustomButton(buttonReleasedTexture, buttonPressedTexture, gameStateHandler.GraphicsDevice, font, "Return to Main Menu");
            mainMenuButton.SetPosition(new Vector2(screenWidth / 2 - saveButtonSize.X +350, screenHeight / 2 - saveButtonSize.Y + 600 / 2));
            mainMenuButton.SetSize(new Vector2(350, 100));
            continueButton = new CustomButton(buttonReleasedTexture, buttonPressedTexture, gameStateHandler.GraphicsDevice, font, $"Continue to Level {currentLevelNumber}");
            continueButton.SetPosition(new Vector2(screenWidth / 2 - saveButtonSize.X , screenHeight / 2 - saveButtonSize.Y + 200 / 2));
            continueButton.SetSize(new Vector2(700, 100));

            buttons = new CustomButton[] {  continueButton, saveButton, mainMenuButton};



            this.score = score;
        }

        public void Enter() {
           
        }

        public void Exit() {
            GC.Collect();
        }

        public void Update(GameTime gameTime)
        {
            mState = Mouse.GetState();
            foreach(CustomButton button in buttons) {
                button.Update(mState);
            }
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
            if(continueButton.isClicked)
            {
                gameStateHandler.ChangeState(nextState);
            }
            if (mainMenuButton.isClicked)
            {
                MainMenu mainMenu = new MainMenu(gameStateHandler, font);
                gameStateHandler.ChangeState(mainMenu);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, graphicsDeviceManager.PreferredBackBufferWidth, graphicsDeviceManager.PreferredBackBufferHeight), Color.White); ;
            // Draw the save button
            foreach(CustomButton button in buttons)
            {
                button.Draw(spriteBatch);
            }

            // convert the message into a vector to get the size of the text
            Vector2 textSize = font.MeasureString(message);
            // calculate the position of the text to center it on the screen
            Vector2 textPosition = new Vector2(
                (graphicsDeviceManager.PreferredBackBufferWidth - textSize.X) / 2,
                (graphicsDeviceManager.PreferredBackBufferHeight - textSize.Y) / 4
            );
            spriteBatch.DrawString(font, message, textPosition, Color.White);


            spriteBatch.End();
        }
    }
}
