using FinalProjectGameProgramming.Entities;
using FinalProjectGameProgramming.Handlers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalProjectGameProgramming.GameStates
{
    internal class MainMenu : IGameState
    {
        private SpriteFont menuFont;
        private string[] menuItems = { "Start Game", "Leaderboard", "Exit" };
        private int selectedIndex = 0;
        private GameStateHandler gameStateHandler;
        GraphicsDeviceManager graphicsDeviceManager;
        ContentManager content;
        GraphicsDevice graphicsDevice;
        Texture2D buttonReleasedTexture;
        Texture2D buttonPressedTexture;
        Texture2D backgroundImage;


        private CustomButton[] buttons;
        private CustomButton playButton;
        private CustomButton exitButton;
        private CustomButton loadSaveButton;
        private CustomButton leaderBoardButton;

        //to get the button to only execute on release and get animations
        private bool isHovering;
        private bool wasClicked;
        private MouseState previousMouseState;
        Song backgroundMusic;
        private int buttonGap = 120;

        public MainMenu(GameStateHandler gameStateHandler, SpriteFont font)
        {
            this.gameStateHandler = gameStateHandler;
            menuFont = font;
            graphicsDeviceManager = gameStateHandler.GraphicsDeviceManager;
            content = gameStateHandler.Content;
            graphicsDevice = gameStateHandler.GraphicsDevice;

            Texture2D buttonReleasedTexture = content.Load<Texture2D>("Button_Release_01a2");
            Texture2D buttonPressedTexture = content.Load<Texture2D>("Button_Release_01a1");// Load your button texture
            backgroundImage = content.Load<Texture2D>("MedievalMaze4");

            // Get the center position of the screen for the button
            CustomButton button = new CustomButton(buttonReleasedTexture, buttonPressedTexture, graphicsDevice, menuFont, "");
            float centerX = (graphicsDeviceManager.PreferredBackBufferWidth - button.GetSize().X) / 2;

            // Create the play button
            playButton = new CustomButton(buttonReleasedTexture, buttonPressedTexture, graphicsDevice, menuFont, "Play");
            playButton.SetPosition(new Vector2(centerX, 100)); // Set position for Play button
            // Create the load save button
            loadSaveButton = new CustomButton(buttonReleasedTexture, buttonPressedTexture, graphicsDevice, menuFont, "Load Game");
            loadSaveButton.SetPosition(new Vector2(centerX, playButton.GetPosition().Y + buttonGap)); // Set position for Leaderboard button
            // Create the leaderboard button
            leaderBoardButton = new CustomButton(buttonReleasedTexture, buttonPressedTexture, graphicsDevice, menuFont, "Leaderboard");
            leaderBoardButton.SetPosition(new Vector2(centerX, loadSaveButton.GetPosition().Y + buttonGap)); // Set position for Leaderboard button
            // Create the exit button
            exitButton = new CustomButton(buttonReleasedTexture, buttonPressedTexture, graphicsDevice, menuFont, "Exit");
            exitButton.SetPosition(new Vector2(centerX, leaderBoardButton.GetPosition().Y + buttonGap)); // Set position for Exit button
            // Add the buttons to the array
            buttons = new CustomButton[] { playButton, loadSaveButton, leaderBoardButton, exitButton };
        }

        public void Enter()
        {
            MediaPlayer.Play(MusicHandler.MainMenuMusic);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.3f;
        }

        public void Exit()
        {
            // Clean up resources if needed


            // Dispose textures
            buttonReleasedTexture?.Dispose();
            buttonPressedTexture?.Dispose();

            // Nullify large objects
            buttonReleasedTexture = null;
            buttonPressedTexture = null;


            // Force garbage collection
            GC.Collect();
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            foreach (CustomButton button in buttons)
            {
                button.Update(mouse);
            }

            // Start Game button action
            if (playButton.isClicked)
            {
                playButton.isClicked = false;
                /*gameStateHandler.ChangeState(new PlayingState(_graphics, _content, _graphicsDevice, 1));*/
                gameStateHandler.ChangeState(new PlayingState(gameStateHandler, 1, 0));
            }

            // Leaderboard button action
            if (leaderBoardButton.isClicked)
            {
                leaderBoardButton.isClicked = false;
                /*gameStateHandler.ChangeState(new PlayingState(_graphics, _content, _graphicsDevice, 1));*/
                gameStateHandler.ChangeState(new LeaderBoardState(gameStateHandler, menuFont));
            }
            if (loadSaveButton.isClicked)
            {
                loadSaveButton.isClicked = false;
                SaveHandler save = new SaveHandler();
                string[] saveFile = save.LoadFile();
                try { 
                    if (saveFile == null) {
                    throw new ArgumentException("No save file found. If you died after your save, your file gets deleted.");
                    }
                    if (saveFile.Length != 2)
                    {
                        throw new ArgumentException("File corrupted.\nFile format does not match Medieval Maze save format");
                    }
                    int score = int.Parse(saveFile[0]);
                    int level = int.Parse(saveFile[1]);
                    gameStateHandler.ChangeState(new PlayingState(gameStateHandler, level, score));

                } catch(Exception ex) {
                    System.Windows.Forms.MessageBox.Show(ex.Message,"No file found",MessageBoxButtons.OKCancel);
                }



            }

            // Exit button action
            if (exitButton.isClicked)
            {
                exitButton.isClicked = false;
                Environment.Exit(0);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // TODO: Add a background image
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, graphicsDeviceManager.PreferredBackBufferWidth, graphicsDeviceManager.PreferredBackBufferHeight), Color.White);
            playButton.Draw(spriteBatch);
            loadSaveButton.Draw(spriteBatch);
            exitButton.Draw(spriteBatch);
            leaderBoardButton.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
