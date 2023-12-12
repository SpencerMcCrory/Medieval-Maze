using FinalProjectGameProgramming.Handlers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace FinalProjectGameProgramming.GameStates
{
    internal class GameOverState : IGameState
    {
        private GraphicsDeviceManager graphicsDeviceManager;
        private GameStateHandler gameStateHandler;
        private ContentManager content;
        private GraphicsDevice graphicsDevice;
        private SpriteFont font;
        private string message;
        private float score;
        private IGameState nextState;
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;
        // The user input: user can enter their name
        private StringBuilder userInput = new StringBuilder("");
        private bool isEnteringName = true;
        private Dictionary<string, float> playerScore;
        private Texture2D gameOverBG;

        public GameOverState(GameStateHandler gameStateHandler, SpriteFont font, float score, bool didTheyWin)
        {
            this.gameStateHandler = gameStateHandler;
            graphicsDeviceManager = gameStateHandler.GraphicsDeviceManager;
            this.content = gameStateHandler.Content;
            this.graphicsDevice = gameStateHandler.GraphicsDevice;
            this.font = font;
            this.score = score;
            SaveHandler save = new SaveHandler();
            save.DeleteSave();
            if (didTheyWin)
            {
                gameOverBG = content.Load<Texture2D>("YouWonBG");
            }
            else
            {
                gameOverBG = content.Load<Texture2D>("GameOverBG");
            }
            
        }

        public void Enter() { }

        public void Exit() { }

        public void Update(GameTime gameTime)
        {
            // if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            // {
            //     gameStateHandler.ChangeState(nextState);
            // }
            currentKeyboardState = Keyboard.GetState();

            // Process input
            ProcessInput();

            // Save the current state for the next frame
            previousKeyboardState = currentKeyboardState;

            // base.Update(gameTime);
        }

        private void ProcessInput()
        {
            // Check for key presses
            Keys[] pressedKeys = currentKeyboardState.GetPressedKeys();
            foreach (Keys key in pressedKeys)
            {
                if (!previousKeyboardState.IsKeyDown(key))
                {
                    if (key == Keys.Enter)
                    {
                        string userName = string.IsNullOrEmpty(userInput.ToString()) ? "User" : userInput.ToString().Trim();
                        playerScore = new Dictionary<string, float>(){
                            {userName, score}
                        };
                        // Change to next state

                        nextState = new LeaderBoardState( gameStateHandler, font, playerScore);
                        gameStateHandler.ChangeState(nextState);
                        // Toggle name entry mode
                        isEnteringName = !isEnteringName;
                    }
                    else if (key == Keys.Back && userInput.Length > 0)
                    {
                        // Handle backspace (remove the last character)
                        userInput.Remove(userInput.Length - 1, 1);
                    }
                    else if (isEnteringName)
                    {
                        // Append other keys to the input string
                        // Get the character corresponding to the key pressed, also check if Shift is pressed
                        char pressedChar = GetCharFromKey(key, currentKeyboardState.IsKeyDown(Keys.LeftShift) || currentKeyboardState.IsKeyDown(Keys.RightShift));
                        if (pressedChar != '\0')
                        {
                            userInput.Append(pressedChar);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the character corresponding to the key pressed
        /// </summary>
        /// <param name="key"></param>
        /// <param name="shiftPressed"></param>
        /// <returns></returns>
        private char GetCharFromKey(Keys key, bool shiftPressed)
        {
            // Check if the key is a printable character
            if ((key >= Keys.A && key <= Keys.Z) || (key >= Keys.D0 && key <= Keys.D9))
            {
                // If it's an alphabetic character, return uppercase or lowercase based on Shift
                if (key >= Keys.A && key <= Keys.Z)
                {
                    return shiftPressed ? key.ToString()[0] : char.ToLower(key.ToString()[0]);
                }
                // If it's a numeric character, return the corresponding digit
                else if (key >= Keys.D0 && key <= Keys.D9)
                {
                    return (char)('0' + (key - Keys.D0));
                }
            }
            else
            {
                // Handle specific non-alphanumeric keys
                switch (key)
                {
                    case Keys.Space:
                        return ' ';
                    case Keys.OemMinus:
                        return shiftPressed ? '_' : '-';
                    case Keys.OemPlus:
                        return shiftPressed ? '+' : '=';
                        // Add more cases for other keys as needed
                }
            }

            // If the key is not a printable character, return null character
            return '\0';
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            // TODO: Add a background image
            spriteBatch.Begin();
            spriteBatch.Draw(gameOverBG, new Rectangle(0, 0, graphicsDeviceManager.PreferredBackBufferWidth, graphicsDeviceManager.PreferredBackBufferHeight), Color.White);
            // convert the message into a vector to get the size of the text
            Vector2 textSize = font.MeasureString(score.ToString());
            // calculate the position of the text
            Vector2 textPosition = new Vector2(
                (graphicsDeviceManager.PreferredBackBufferWidth - textSize.X) / 2,
                (graphicsDeviceManager.PreferredBackBufferHeight - textSize.Y) -325
            );
            spriteBatch.DrawString(font, score.ToString(), textPosition, Color.White);

            // Add user input
            string displayText = userInput.ToString();
            Vector2 displayTextSize = font.MeasureString(displayText);
            //Vector2 userInputPos = new Vector2(textPosition.X, textPosition.Y + textSize.Y + 120);
            
            Vector2 userInputPos = new Vector2(
                (graphicsDeviceManager.PreferredBackBufferWidth - displayTextSize.X) / 2, // This will keep it centered
                textPosition.Y + textSize.Y + 120 // Position Y below the score or wherever you'd like it
            );


            // Draw user input or entered name
            
            spriteBatch.DrawString(font, displayText, userInputPos, Color.White);
            string exitText = "Enter your name then press ENTER to Main Menu";
            Vector2 exitTextSize = font.MeasureString(exitText);
            spriteBatch.DrawString(font,exitText , new Vector2(graphicsDeviceManager.PreferredBackBufferWidth / 2 - (exitTextSize.X/2), 
                                    userInputPos.Y +90), Color.White);

            spriteBatch.End();
        }
    }
}
