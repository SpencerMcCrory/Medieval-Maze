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


namespace FinalProjectGameProgramming.GameStates
{
    internal class MainMenu : IGameState
    {
        private SpriteFont menuFont;
        private string[] menuItems = { "Start Game", "Leaderboard", "Help", "About", "Exit" };
        private int selectedIndex = 0;
        private GameStateHandler gameStateHandler;
        GraphicsDeviceManager _graphics;
        ContentManager _content;
        GraphicsDevice _graphicsDevice;
        Texture2D buttonReleasedTexture;
        Texture2D buttonPressedTexture;
        Texture2D backgroundImage;


        private Button[] buttons;
        private Button playButton;
        private Button exitButton;
        private Button leaderBoardButton;
        private Button aboutButton;
        private Button helpButton;

        //to get the button to only execute on release and get animations
        private bool isHovering;
        private bool wasClicked;
        private MouseState previousMouseState;
        Song backgroundMusic;
        private int buttonGap = 120;

        public MainMenu(GameStateHandler gameStateHandler, SpriteFont font, GraphicsDeviceManager graphics, ContentManager content, GraphicsDevice graphicsDevice)
        {
            this.gameStateHandler = gameStateHandler;
            menuFont = font;
            _graphics = graphics;
            _content = content;
            _graphicsDevice = graphicsDevice;

            Texture2D buttonReleasedTexture = content.Load<Texture2D>("Button_Release_01a2");
            Texture2D buttonPressedTexture = content.Load<Texture2D>("Button_Release_01a1");// Load your button texture
            backgroundImage = content.Load<Texture2D>("MedievalMaze4");

            // Get the center position of the screen for the button
            Button button = new Button(buttonReleasedTexture, buttonPressedTexture, graphicsDevice, menuFont, "");
            float centerX = (_graphics.PreferredBackBufferWidth - button.GetSize().X) / 2;

            // Create the play button
            playButton = new Button(buttonReleasedTexture, buttonPressedTexture, graphicsDevice, menuFont, "Play");
            playButton.SetPosition(new Vector2(centerX, 100)); // Set position for Play button
            
            // Create the leaderboard button
            leaderBoardButton = new Button(buttonReleasedTexture, buttonPressedTexture, graphicsDevice, menuFont, "Leaderboard");
            leaderBoardButton.SetPosition(new Vector2(centerX, playButton.GetPosition().Y + buttonGap)); // Set position for Leaderboard button

            // create the help button
            helpButton = new Button(buttonReleasedTexture, buttonPressedTexture, graphicsDevice, menuFont, "Help");
            helpButton.SetPosition(new Vector2(centerX, leaderBoardButton.GetPosition().Y + buttonGap)); // Set position for Help button

            // Create the about button
            aboutButton = new Button(buttonReleasedTexture, buttonPressedTexture, graphicsDevice, menuFont, "About");
            aboutButton.SetPosition(new Vector2(centerX, helpButton.GetPosition().Y + buttonGap)); // Set position for About button

            // Create the exit button
            exitButton = new Button(buttonReleasedTexture, buttonPressedTexture, graphicsDevice, menuFont, "Exit");
            exitButton.SetPosition(new Vector2(centerX, aboutButton.GetPosition().Y + buttonGap)); // Set position for Exit button
            // Add the buttons to the array
            buttons = new Button[] { playButton, leaderBoardButton, helpButton, aboutButton, exitButton };
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


            // Dispose textures if they implement IDisposable
            buttonReleasedTexture?.Dispose();
            buttonPressedTexture?.Dispose();

            // Nullify large objects
            buttonReleasedTexture = null;
            buttonPressedTexture = null;


            // Optionally, force garbage collection (use sparingly)
            GC.Collect();
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            foreach (Button button in buttons)
            {
                button.Update(mouse);
            }

            // Start Game button action
            if (buttons[0].isClicked)
            {
                /*gameStateHandler.ChangeState(new PlayingState(_graphics, _content, _graphicsDevice, 1));*/
                gameStateHandler.ChangeState(new PlayingState(_graphics, _content, _graphicsDevice, gameStateHandler, 1, 0));
            }

            // Leaderboard button action
            if (buttons[1].isClicked)
            {
                gameStateHandler.ChangeState(new LeaderBoardState(_graphics, _content, gameStateHandler, _graphicsDevice, menuFont));
            }

            // Help button action
            if (buttons[2].isClicked)
            {
                gameStateHandler.ChangeState(new HelpState(_graphics, _content, gameStateHandler, _graphicsDevice, menuFont));
            }

            // About button action
            if (buttons[3].isClicked)
            {
                gameStateHandler.ChangeState(new AboutState(_graphics, _content, gameStateHandler, _graphicsDevice, menuFont));
            }

            // Exit button action
            if (buttons[4].isClicked)
            {
                Environment.Exit(0);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // TODO: Add a background image
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
            // Draw play button
            playButton.Draw(spriteBatch);
            // Draw leaderboard button
            leaderBoardButton.Draw(spriteBatch);
            // Draw help button
            helpButton.Draw(spriteBatch);
            // Draw about button
            aboutButton.Draw(spriteBatch);
            // Draw exit button
            exitButton.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
