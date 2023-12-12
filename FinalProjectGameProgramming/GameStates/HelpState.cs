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
    internal class HelpState : IGameState
    {
        private GraphicsDeviceManager graphicsDeviceManager;
        private GameStateHandler gameStateHandler;
        private SpriteFont font;
        private SpriteFont helpFont;
        private ContentManager content;
        private GraphicsDevice graphicsDevice;
        private MouseState mState;
        // Textures for help state
        private Texture2D aboutBG;
        private Texture2D knight;
        private Texture2D bigZombie;
        private Texture2D slime;
        private Texture2D udlrKeyboard;
        private Texture2D wsadKeyboard;
        private Texture2D spike;
        private Texture2D boost;
        private Texture2D chest;
        private Texture2D unActivatedSwitch;
        private Texture2D activatedSwitch;
        private Texture2D unOpenedDoor;
        private Texture2D openedDoor;

        private int screenWidth;
        private int screenHeight;

        private CustomButton closedButton;


        public HelpState(GameStateHandler gameStateHandler, SpriteFont font)
        {
            this.gameStateHandler = gameStateHandler;
            graphicsDeviceManager = gameStateHandler.GraphicsDeviceManager;
            content = gameStateHandler.Content;
            this.font = font;
            graphicsDevice = gameStateHandler.GraphicsDevice;
            screenWidth = graphicsDevice.Viewport.Width;
            screenHeight = graphicsDevice.Viewport.Height;


            // Load the button textures
            Texture2D buttonReleasedTexture = content.Load<Texture2D>("Button_Release_01a2");
            Texture2D buttonPressedTexture = content.Load<Texture2D>("Button_Release_01a1");

            // Create the close button
            closedButton = new CustomButton(buttonReleasedTexture, buttonPressedTexture, graphicsDevice, font, "X");
            closedButton.SetPosition(new Vector2(100, 10)); // Set position for Close button
            closedButton.SetSize(new Vector2(50, 50)); // Set size for Close button

        }

        public void Enter()
        {
            // Load textures
            aboutBG = content.Load<Texture2D>("HelpBG");
            knight = content.Load<Texture2D>("knight_f_idle_anim_f1");
            bigZombie = content.Load<Texture2D>("big_zombie_run_anim_f0");
            slime = content.Load<Texture2D>("swampy_anim_f0");
            helpFont = content.Load<SpriteFont>("helpFont");
            udlrKeyboard = content.Load<Texture2D>("UDLR");
            wsadKeyboard = content.Load<Texture2D>("WSAD");
            spike = content.Load<Texture2D>("spike");
            boost = content.Load<Texture2D>("flask_blue_powerup");
            chest = content.Load<Texture2D>("chest_full_open_anim_f0");
            unActivatedSwitch = content.Load<Texture2D>("not_activated_switch");
            activatedSwitch = content.Load<Texture2D>("activated_switch");
            unOpenedDoor = content.Load<Texture2D>("unopened_door");
            openedDoor = content.Load<Texture2D>("opened_door");
        }

        public void Exit() {
            GC.Collect();
        }

        public void Update(GameTime gameTime)
        {
            mState = Mouse.GetState();
            closedButton.Update(mState);
            if (closedButton.isClicked)
            {
                gameStateHandler.ChangeState(new MainMenu(gameStateHandler, font));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // TODO: Add a background image
            spriteBatch.Begin();
            //draw bg image
            spriteBatch.Draw(aboutBG, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            // Get center position of X for the title
            int oddInstructionStart = 220;
            int evenInstructionStart = screenWidth - oddInstructionStart;
            int imagesYGap = 80;
            int gapBetweenInstructions = 200;
            Vector2 centerPosition = new Vector2(screenWidth / 2, 180);

            // Draw general information
            // string title = $"How to play game?";
            // Vector2 titleSize = font.MeasureString(title);
            // spriteBatch.DrawString(font, title, new Vector2(centerPosition.X - titleSize.X, centerPosition.Y), Color.DarkGreen);
            
            // Draw the instruction for the game
            // Instruction 1: Text
            string instruction1 = "1. Use the arrow keys or WASD\n to move the knight.";
            Vector2 instruction1Size = helpFont.MeasureString(instruction1);
            Vector2 instruction1Position = new Vector2(oddInstructionStart, centerPosition.Y);
            spriteBatch.DrawString(helpFont, instruction1, instruction1Position, Color.White);
            // Instruction 1: Images
            spriteBatch.Draw(udlrKeyboard, new Vector2(oddInstructionStart, instruction1Position.Y + imagesYGap), Color.White);
            spriteBatch.Draw(wsadKeyboard, new Vector2(oddInstructionStart + 120, instruction1Position.Y + imagesYGap), Color.White);
            spriteBatch.Draw(knight, new Vector2(oddInstructionStart + 240, instruction1Position.Y + imagesYGap), Color.White);

            // Instruction 2: Text
            string instruction2 = "2. While moving the knight, avoid\n the enemies and traps.";
            Vector2 instruction2Size = helpFont.MeasureString(instruction2);
            Vector2 instruction2Position = new Vector2(evenInstructionStart - instruction2Size.X, centerPosition.Y);
            spriteBatch.DrawString(helpFont, instruction2, instruction2Position, Color.White);
            // Instruction 2: Images
            spriteBatch.Draw(spike, new Vector2(instruction2Position.X, instruction2Position.Y + imagesYGap), Color.White);
            spriteBatch.Draw(slime, new Vector2(instruction2Position.X + 100, instruction2Position.Y + imagesYGap), Color.White);
            spriteBatch.Draw(bigZombie, new Vector2(instruction2Position.X + 170, instruction2Position.Y + 40), Color.White);

            // Instruction 3: Text
            string instruction3 = "3. Find a switch to activate the\n door and get to the next level.";
            Vector2 instruction3Size = helpFont.MeasureString(instruction3);
            Vector2 instruction3Position = new Vector2(oddInstructionStart, instruction1Position.Y + gapBetweenInstructions+40);
            spriteBatch.DrawString(helpFont, instruction3, instruction3Position, Color.White);
            // Instruction 3: Images
            spriteBatch.Draw(unActivatedSwitch, new Vector2(oddInstructionStart + 5, instruction3Position.Y + imagesYGap), Color.White);
            spriteBatch.Draw(unOpenedDoor, new Vector2(oddInstructionStart, instruction3Position.Y + imagesYGap + 60), Color.White);
            spriteBatch.Draw(activatedSwitch, new Vector2(oddInstructionStart + 5 + 100, instruction3Position.Y + imagesYGap), Color.White);
            spriteBatch.Draw(openedDoor, new Vector2(oddInstructionStart + 100, instruction3Position.Y + imagesYGap + 60), Color.White);

            // Instruction 4: Text
            // string instruction4 = "4. Find relics to increase your score\n and speed for the knight.";
            string instruction4 = "Relics give a bonus to your score.";
            Vector2 instruction4Size = helpFont.MeasureString(instruction4);
            Vector2 instruction4Position = new Vector2(evenInstructionStart - instruction4Size.X, instruction1Position.Y + gapBetweenInstructions);
            spriteBatch.DrawString(helpFont, instruction4, instruction4Position, Color.White);
            // Instruction 4: Images
            
            spriteBatch.Draw(chest, new Vector2(instruction4Position.X+110, instruction4Position.Y + imagesYGap/2), Color.White);

            string instruction5 = "Speed boost powerup.";
            Vector2 instruction5Size = helpFont.MeasureString(instruction4);
            Vector2 instruction5Position = new Vector2(evenInstructionStart - instruction5Size.X+instruction4Size.X/6, instruction4Position.Y +gapBetweenInstructions-60);
            spriteBatch.DrawString(helpFont, instruction5, instruction5Position, Color.White);
            spriteBatch.Draw(boost, new Vector2(instruction4Position.X+110, instruction5Position.Y + imagesYGap / 2 + 10), Color.White);

            // Draw the close button
            closedButton.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
