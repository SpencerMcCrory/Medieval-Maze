using FinalProjectGameProgramming.Handlers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace FinalProjectGameProgramming
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
        private Texture2D backgroundTexture;
        private Texture2D wallsTexture;
        private Texture2D door;
        private Texture2D button;
        private LevelHandler currentLevel;
        private int tileSize = 64;
        private Vector2 playerPosition;
        private float playerSpeed = 300f;
        private float movementDelta;
        private Texture2D playerTexture;
        const int PLAYER_HEIGHT = 56;
        const int PLAYER_WIDTH = 32;

        private Texture2D[] playerAnimationFrames;
        const int playerFrameCount = 4;

        private string currentAnimationKey = "idle";
        private int currentFrame;
        private double animationTimer;
        private double timePerFrame = 0.2;

        private Dictionary<string, AnimationHandler> animations;

        private CameraHandler camera;

        SpriteFont font;

        string debugText;
        bool levelComplete;
        bool gameOver;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            
            //can't get relative path to work for some reason
            string csvFilePath = "C:\\PROG2370-23F\\Monogame\\FinalProjectGameProgramming\\FinalProjectGameProgramming\\Content\\IntGrid_Switch_Not_Clicked.csv";
            if (File.Exists(csvFilePath))
            {
                currentLevel = new LevelHandler(csvFilePath);
            }
            else
            {
                throw new FileNotFoundException("Unable to load walls.csv. File not found.");
            }

            playerPosition = new Vector2(currentLevel.StartPoint[0] * tileSize, currentLevel.StartPoint[1] * tileSize); // Starting position of the player
            camera = new CameraHandler();


            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            animations = new Dictionary<string, AnimationHandler>();

            // Load idle animation frames
            Texture2D[] idleFrames = new Texture2D[4];
            for (int i = 0; i < 4; i++)
            {
                idleFrames[i] = Content.Load<Texture2D>($"knight_f_idle_anim_f{i}");
            }
            animations["idle"] = new AnimationHandler(idleFrames, 0.5);

            // Load run animation frames
            Texture2D[] runFrames = new Texture2D[4];
            for (int i = 0; i < 4; i++)
            {
                runFrames[i] = Content.Load<Texture2D>($"knight_f_run_anim_f{i}");
            }
            animations["run"] = new AnimationHandler(runFrames, 0.1);

            // Load textures
            backgroundTexture = Content.Load<Texture2D>("Background");
            wallsTexture = Content.Load<Texture2D>("Walls_BackGround");
            door = Content.Load<Texture2D>("Door_Closed");
            button = Content.Load<Texture2D>("Button_not_pressed");
            playerTexture = Content.Load<Texture2D>("knight_f_run_anim_f0");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            movementDelta = (float)gameTime.ElapsedGameTime.TotalSeconds * playerSpeed;

            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTimer > timePerFrame)
            {
                currentFrame = (currentFrame + 1) % playerFrameCount;
                animationTimer -= timePerFrame;
            }

            // Movement input
            Vector2 direction = Vector2.Zero;


            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Up))
                direction.Y -= 1;
            if (state.IsKeyDown(Keys.Down))
                direction.Y += 1;
            if (state.IsKeyDown(Keys.Left))
                direction.X -= 1;
            if (state.IsKeyDown(Keys.Right))
                direction.X += 1;

            if (direction != Vector2.Zero)
            {
                currentAnimationKey = "run";
            }
            else
            {
                currentAnimationKey = "idle";
            }
            AnimationHandler currentAnimation = animations[currentAnimationKey];
            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTimer > currentAnimation.TimePerFrame)
            {
                currentFrame = (currentFrame + 1) % currentAnimation.FrameCount;
                animationTimer -= currentAnimation.TimePerFrame;
            }

            Vector2 newPlayerPosition = playerPosition + direction * movementDelta;

            string collidingWith = CheckForCollision(newPlayerPosition);
            // Check for collisions with the wall
            if (collidingWith != "wall")
            {
                // Apply movement
                playerPosition = newPlayerPosition;
            }

            //get the center of the window
            Microsoft.Xna.Framework.Vector2 tempVec = playerPosition - new Microsoft.Xna.Framework.Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);

            //update the camera position
            camera.Position = new System.Numerics.Vector2(tempVec.X, tempVec.Y);

            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);


            // Use the camera's view matrix
            spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

            // Draw the background, player, walls, etc.
            spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);
            spriteBatch.Draw(wallsTexture, Vector2.Zero, Color.White);
            spriteBatch.Draw(door, Vector2.Zero, Color.White);
            spriteBatch.Draw(button, Vector2.Zero, Color.White);
            AnimationHandler currentAnimation = animations[currentAnimationKey];
            spriteBatch.Draw(currentAnimation.Frames[currentFrame], playerPosition, Color.White);

            spriteBatch.End();


            //has to have it's own begin and and end otherwise wont pop up
            spriteBatch.Begin();
           
            if (levelComplete)
            {
                spriteBatch.DrawString(Content.Load<SpriteFont>("galleryFont"), "Winner!", new Vector2(100, 100), Color.White);
            }
            if (gameOver)
            {
                spriteBatch.DrawString(Content.Load<SpriteFont>("galleryFont"), "Game Over. You Died.", new Vector2(100, 100), Color.White);
            }


            spriteBatch.End();



            base.Draw(gameTime);
        }

        private string CheckForCollision(Vector2 position)
        {
            Rectangle playerBounds = new Rectangle((int)position.X, (int)position.Y, PLAYER_WIDTH, PLAYER_HEIGHT);

            // Check each corner of the player's bounding box for collision
            foreach (Vector2 corner in new Vector2[]
            {
        new Vector2(playerBounds.Left, playerBounds.Top + PLAYER_HEIGHT /3), //offseting by 3 because the height of the sprite isn't 16
        new Vector2(playerBounds.Right, playerBounds.Top + PLAYER_HEIGHT /3),//otherwise sprite when under wall would stop early
        new Vector2(playerBounds.Left, playerBounds.Bottom),
        new Vector2(playerBounds.Right, playerBounds.Bottom)
            })
            {
                int gridX = (int)corner.X / tileSize;
                int gridY = (int)corner.Y / tileSize;


                if (currentLevel.Grid[gridY, gridX] == 1)
                    return "wall"; // Collision detected
                if (currentLevel.Grid[gridY,gridX] == 3)
                {
                    door = Content.Load<Texture2D>("Door_Open");
                    button = Content.Load<Texture2D>("Button_pressed");
                    string csvFilePath = "C:\\PROG2370-23F\\Monogame\\FinalProjectGameProgramming\\FinalProjectGameProgramming\\Content\\IntGrid_Switch_Clicked.csv";
                    if (File.Exists(csvFilePath))
                    {
                        currentLevel = new LevelHandler(csvFilePath);
                    }
                    else
                    {
                        throw new FileNotFoundException("Unable to load walls.csv. File not found.");
                    }
                    return "button";
                }
                if (currentLevel.Grid[gridY, gridX] == 6)
                {
                    levelComplete = true;
                }
                if (currentLevel.Grid[gridY, gridX] == 9)
                {
                    gameOver = true;
                }
            }
            return "nothing"; // No collision
        }
    }
}