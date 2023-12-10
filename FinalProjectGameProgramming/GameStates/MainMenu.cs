using FinalProjectGameProgramming.Entities;
using FinalProjectGameProgramming.Handlers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;


namespace FinalProjectGameProgramming.GameStates
{
    internal class MainMenu : IGameState
    {
        private SpriteFont menuFont;
        private string[] menuItems = { "Start Game", "Exit" };
        private int selectedIndex = 0;
        private GameStateHandler gameStateHandler;
        GraphicsDeviceManager _graphics;
        ContentManager _content;
        GraphicsDevice _graphicsDevice;
        Texture2D buttonReleasedTexture;
        Texture2D buttonPressedTexture;


        private Button[] buttons;
        private Button playButton;
        private Button exitButton;

        //to get the button to only execute on release and get animations
        private bool isHovering;
        private bool wasClicked;
        private MouseState previousMouseState;
        Song backgroundMusic;

        public MainMenu(GameStateHandler gameStateHandler, SpriteFont font, GraphicsDeviceManager graphics, ContentManager content, GraphicsDevice graphicsDevice)
        {
            this.gameStateHandler = gameStateHandler;
            menuFont = font;
            _graphics = graphics;
            _content = content;
            _graphicsDevice = graphicsDevice;

            Texture2D buttonReleasedTexture = content.Load<Texture2D>("Button_Release_01a2");
            Texture2D buttonPressedTexture = content.Load<Texture2D>("Button_Release_01a1");// Load your button texture
            

            playButton = new Button(buttonReleasedTexture,buttonPressedTexture, graphicsDevice, menuFont, "Play");
            playButton.SetPosition(new Vector2(200, 100)); // Set position for Play button

            exitButton = new Button(buttonReleasedTexture, buttonPressedTexture, graphicsDevice, menuFont, "Exit");
            exitButton.SetPosition(new Vector2(200, 170)); // Set position for Exit button

            buttons = new Button[] { playButton, exitButton };   
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

            if (buttons[0].isClicked)
            {
                // Start Game button action
                /*gameStateHandler.ChangeState(new PlayingState(_graphics, _content, _graphicsDevice, 1));*/
                gameStateHandler.ChangeState(new PlayingState(_graphics, _content, _graphicsDevice, gameStateHandler, 1,0));
            }
            if (buttons[1].isClicked)
            {
                // Exit button action
                Environment.Exit(0);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // TODO: Add a background image
            spriteBatch.Begin();

            playButton.Draw(spriteBatch);
            exitButton.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
