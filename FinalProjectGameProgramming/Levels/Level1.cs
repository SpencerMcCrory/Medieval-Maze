﻿using FinalProjectGameProgramming.Entities;
using FinalProjectGameProgramming.GameStates;
using FinalProjectGameProgramming.Handlers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Windows.Forms;
using System.Xml.Linq;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace FinalProjectGameProgramming.Levels
{
    public class Level1 : Level
    {
        private ContentManager content;
        private GraphicsDevice graphicsDevice;
        private GameStateHandler gameStateHandler;
        private GraphicsDeviceManager graphicsDeviceManager;
        private SpriteBatch spriteBatch;
        private Texture2D backgroundTexture;
        private Texture2D wallsTexture;
        private Texture2D door;
        private Texture2D button;
        private Texture2D[] slimeRunFrames;
        private Texture2D[] relicFrames;
        private Texture2D[] playerAnimationFrames;
        Texture2D pixel;
        private Dictionary<string, AnimationHandler> animations;
        private LevelHandler currentLevel;
        private const int TILE_SIZE = 64;
        private float movementDelta;
        const int SPIKE_HITBOX_OFFSET = 10;
        private Rectangle monsterHitbox;
        Player player;
        List<Monster> monsters = new List<Monster>();
        List<Relic> relics = new List<Relic>();
        List<PowerUp> powerUps = new List<PowerUp>();
        int timingDelay = 0;
        const int PLAYER_FRAME_COUNT = 4;
        private string currentAnimationKey = "playerIdle";
        private int currentFrame;
        private double animationTimer;
        private const double TIME_PER_FRAME = 0.4;
        private CameraHandler camera;
        private CollisionHandler collisionHandler;
        SpriteFont font;
        string debugText;
        bool levelComplete;
        bool gameOver;
        bool buttonPressed;
        PowerUp removedPowerUp = null;
        SoundEffect buttonClickSE;
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private ScoreHandler score;
        // Get the path of the directory that contains the content
        private string contentRootDirectory;

        //pass the game statehandler to the constructor to change between states
        internal Level1(GameStateHandler gameStateHandler)
        {
            // Initialize fields using passed-in graphics and content
            this.gameStateHandler = gameStateHandler;
            graphicsDeviceManager = gameStateHandler.GraphicsDeviceManager;
            content = gameStateHandler.Content;
            graphicsDevice = gameStateHandler.GraphicsDevice;
            score = new ScoreHandler(0);
            contentRootDirectory = content.RootDirectory;
        }

        public override void Initialize()
        {

            // Change "Copy to Output directory" to "Copy if newer" of the file.
            string csvRelativePath = "IntGrid_Switch_Not_Clicked.csv";
            string csvFilePath = Path.Combine(contentRootDirectory, csvRelativePath);

            if (File.Exists(csvFilePath))
            {
                currentLevel = new LevelHandler(csvFilePath);
            }
            else
            {
                throw new FileNotFoundException("Unable to load walls.csv. File not found.");
            }
            player = new Player();
            player.Position = new Vector2(currentLevel.StartPoint[0] * TILE_SIZE, currentLevel.StartPoint[1] * TILE_SIZE); // Starting position of the player
            camera = new CameraHandler();
            collisionHandler = new CollisionHandler(currentLevel, TILE_SIZE);
            buttonPressed = false;
        }

        public override void LoadLevelContent()
        {


            animations = new Dictionary<string, AnimationHandler>();

            // Load idle animation frames for player
            Texture2D[] playerIdleFrames = new Texture2D[4];
            for (int i = 0; i < 4; i++)
            {
                playerIdleFrames[i] = content.Load<Texture2D>($"knight_f_idle_anim_f{i}");
            }
            animations["playerIdle"] = new AnimationHandler(playerIdleFrames, 1);



            // Load run animation frames for player
            Texture2D[] playerRunFrames = new Texture2D[4];
            for (int i = 0; i < 4; i++)
            {
                playerRunFrames[i] = content.Load<Texture2D>($"knight_f_run_anim_f{i}");
            }
            animations["playerRun"] = new AnimationHandler(playerRunFrames, 0.1);

            //Load run animation frames for slime monster
            Texture2D[] slimeRunFrames = new Texture2D[4];
            for (int i = 0; i < 4; i++)
            {
                slimeRunFrames[i] = content.Load<Texture2D>($"swampy_anim_f{i}");
            }

            //Load run animation frames for big zombie monster
            Texture2D[] bigZombieRunFrames = new Texture2D[4];
            for (int i = 0; i < 4; i++)
            {
                bigZombieRunFrames[i] = content.Load<Texture2D>($"big_zombie_run_anim_f{i}");
            }
            Texture2D[] relicFrames = new Texture2D[3];
            for (int i = 0; i < 3; i++)
            {
                relicFrames[i] = content.Load<Texture2D>($"chest_full_open_anim_f{i}");
            }

            Texture2D speedPowerUpTexture = content.Load<Texture2D>("flask_blue_powerup");


            // Load textures
            backgroundTexture = content.Load<Texture2D>("Background");
            wallsTexture = content.Load<Texture2D>("Walls_BackGround");
            door = content.Load<Texture2D>("Door_Closed");
            button = content.Load<Texture2D>("Button_not_pressed");



            float bigZombieSpeed = 32f; // Slower speed for BigZombie
            Vector2 bigZombieDirection = new Vector2(1, 0); // Initial direction for BigZombie

            float slimeMonsterSpeed = 64f; // Faster speed for SlimeMonster
            Vector2 slimeMonsterDirection = new Vector2(-1, 0); // Initial direction for SlimeMonster


            foreach (int[] spawnPoint in currentLevel.MonsterSpawnPoints)
            {
                if (spawnPoint[0] % 2 == 1)
                {
                    Vector2 position = new Vector2(spawnPoint[0] * TILE_SIZE - 15, spawnPoint[1] * TILE_SIZE - 60);
                    monsters.Add(new BigZombie(bigZombieRunFrames, position, collisionHandler, bigZombieSpeed, bigZombieDirection));
                }
                else
                {
                    Vector2 position = new Vector2(spawnPoint[0] * TILE_SIZE + 5, spawnPoint[1] * TILE_SIZE);
                    monsters.Add(new SlimeMonster(slimeRunFrames, position, collisionHandler, slimeMonsterSpeed, slimeMonsterDirection));
                }
            }
            //used for drawing hitboxes
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            foreach (int[] spawnPoint in currentLevel.RelicSpawnPoints)
            {

                Vector2 position = new Vector2(spawnPoint[0] * TILE_SIZE + 5, spawnPoint[1] * TILE_SIZE);
                relics.Add(new Relic(position, relicFrames, 0.08));

            }

            foreach (int[] spawnPoint in currentLevel.PowerUpLocations)
            {
                Vector2 position = new Vector2(spawnPoint[0] * TILE_SIZE, spawnPoint[1] * TILE_SIZE);
                powerUps.Add(new SpeedPowerUp(position, speedPowerUpTexture, "speed"));

            }

            buttonClickSE = content.Load<SoundEffect>("futuristic_button_click");
            font = content.Load<SpriteFont>("galleryFont");



        }
        //Update all the entities (movement, animation frame, sound effects) in the map as well as check for collision
        public override void UpdateLevel(GameTime gameTime)
        {

            Rectangle playerHitbox = player.MonsterHitbox;

            //will be used to update the monsters
            foreach (Monster monster in monsters)
            {
                monster.Update(gameTime);
                if (monster.Name == "Big Zombie")
                {
                    monsterHitbox = new Rectangle(
                        (int)monster.Position.X + 20,
                        (int)monster.Position.Y + 10,
                        monster.Texture.Width - 2 * 15,
                        monster.Texture.Height - 10
                    );
                }
                else
                {
                    monsterHitbox = new Rectangle((int)monster.Position.X,
                                                        (int)monster.Position.Y,
                                                        monster.Texture.Width,
                                                        monster.Texture.Height);
                }

                if (collisionHandler.CheckCollisionWithEnvironment(monsterHitbox))
                {
                    // Handle monster collision with the environment
                }
                if (playerHitbox.Intersects(monsterHitbox))
                {
                    gameOver = true;
                }
                if (player.SFXHitbox.Intersects(monsterHitbox))
                {
                    monster.InitializeSFXTimer(elapsedTime.TotalSeconds);
                    monster.PlaySound(elapsedTime.TotalSeconds);
                    if (monster.CanPlaySound)
                    {
                        if (monster.Name == "Slime")
                        {
                            SFXHandler.SlimeNoise.Play();
                        }
                        if (monster.Name == "Big Zombie")
                        {
                            SFXHandler.BigZombieNoise.Play();
                        }

                    }
                }
            }
            movementDelta = (float)gameTime.ElapsedGameTime.TotalSeconds * player.Speed;

            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTimer > TIME_PER_FRAME)
            {
                currentFrame = (currentFrame + 1) % PLAYER_FRAME_COUNT;
                animationTimer -= TIME_PER_FRAME;
            }

            // Movement input
            Vector2 direction = Vector2.Zero;


            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.W))
            {
                direction.Y -= 1;
            }
            if (state.IsKeyDown(Keys.Down) || state.IsKeyDown(Keys.S))
            {
                direction.Y += 1;
            }
            if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
            {
                direction.X -= 1;
                player.IsFacingRight = false;//change direction of player texture
            }

            if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
            {
                direction.X += 1;
                player.IsFacingRight = true;//change direction of player texture
            }


            if (direction != Vector2.Zero)
            {
                currentAnimationKey = "playerRun";
            }
            else
            {
                currentAnimationKey = "playerIdle";
            }

            AnimationHandler currentAnimation = animations[currentAnimationKey];
            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTimer > currentAnimation.TimePerFrame)
            {
                currentFrame = (currentFrame + 1) % currentAnimation.FrameCount;
                animationTimer -= currentAnimation.TimePerFrame;
            }

            Vector2 newPlayerPosition = player.Position + direction * movementDelta;

            string collidingWith = CheckForCollision(newPlayerPosition);
            // Check for collisions with the wall
            if (collidingWith != "wall")
            {
                // Apply movement
                player.Position = newPlayerPosition;
            }


            foreach (PowerUp powerUp in powerUps)
            {
                if (!powerUp.IsDeleted && player.MonsterHitbox.Intersects(powerUp.Hitbox))
                {
                    powerUp.IsDeleted = true;

                    //handle the player collecting the powerup
                    powerUp.Collect(player);
                    powerUp.SetDurationTimer(elapsedTime.TotalSeconds);
                    SFXHandler.SpeedPotion.Play();
                    break;
                }
                powerUp.Update(gameTime);
                if (powerUp.IsDeleted == true)
                {
                    if (powerUp.IsActive == false)
                    {
                        if (powerUp.Type == "speed")
                        {
                            player.Speed = 300;
                            removedPowerUp = powerUp;

                        }
                    }
                }
            }
            //remove powerup from list
            if (removedPowerUp != null)
            {
                RemovePowerUp(removedPowerUp);
                removedPowerUp = null;
            }

            foreach (Relic relic in relics)
            {
                relic.Update(gameTime);
            }

            //get the center of the window
            Vector2 tempVec = player.Position - new Vector2(graphicsDeviceManager.PreferredBackBufferWidth / 2, graphicsDeviceManager.PreferredBackBufferHeight / 2);

            //update the camera position
            camera.Position = new System.Numerics.Vector2(tempVec.X, tempVec.Y);

            elapsedTime += gameTime.ElapsedGameTime;

        }

        public override void DrawLevel(SpriteBatch spriteBatch)
        {
            graphicsDevice.Clear(Color.Black);

            // Use the camera's view matrix
            spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

            // Draw the background, player, walls, etc.
            spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);
            spriteBatch.Draw(wallsTexture, Vector2.Zero, Color.White);
            spriteBatch.Draw(door, Vector2.Zero, Color.White);
            spriteBatch.Draw(button, Vector2.Zero, Color.White);
            //get which direction player is moving and flip the image based on that
            SpriteEffects spriteEffects = player.IsFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            //draw player animation current frame
            AnimationHandler currentAnimation = animations[currentAnimationKey];
            //to use sprite effects you need to include other perameters
            spriteBatch.Draw(
                texture: currentAnimation.Frames[currentFrame], // The texture to draw
                position: player.Position,
                sourceRectangle: null, // The area of the texture to draw (use null for the whole texture)
                color: Color.White,
                rotation: 0f, // The rotation of the texture 
                origin: Vector2.Zero, // The origin of the texture 
                scale: 1f, // The scale factor 
                effects: spriteEffects, // The SpriteEffects value (SpriteEffects.FlipHorizontally)
                layerDepth: 0f // The depth of the layer
            );

            foreach (Monster monster in monsters)
            {
                //debugging hitbox
                if (monster.Name == "Big Zombie")
                {
                    monsterHitbox = new Rectangle(
                        (int)monster.Position.X + 20,
                        (int)monster.Position.Y + 10,
                        monster.Texture.Width - 2 * 20,
                        monster.Texture.Height - 10
                    );
                }
                else
                {
                    monsterHitbox = new Rectangle(
                        (int)monster.Position.X,
                        (int)monster.Position.Y,
                        monster.Texture.Width,
                        monster.Texture.Height
                    );
                }

                // Draw the hitbox as a red rectangle
                // spriteBatch.Draw(pixel, monsterHitbox, Color.Red * 0.5f);
                monster.Draw(spriteBatch);
            }

            foreach (Relic relic in relics)
            {
                relic.Draw(spriteBatch);
            }

            foreach (PowerUp powerUp in powerUps)
            {
                if (!powerUp.IsDeleted)
                {
                    spriteBatch.Draw(powerUp.Texture, powerUp.Position, Color.White);
                }
            }
            spriteBatch.End();

            //has to have it's own begin to not be affected by the camera positioning
            spriteBatch.Begin();
            // Draw the timer
            spriteBatch.DrawString(font, $"Time: {elapsedTime.Minutes:D2}:{elapsedTime.Seconds:D2}", new Vector2(10, 10), Color.White);

            // Draw the score
            spriteBatch.DrawString(font, $"Score: {score.GetScore()}", new Vector2(10, 50), Color.White);
            spriteBatch.End();

            //has to have it's own begin and and end otherwise wont pop up
            spriteBatch.Begin();

            // If level is complete or game over, display the appropriate message and score
            if (levelComplete)
            {
                int totalTime = (int)elapsedTime.TotalSeconds;
                score.CalculateTimeScore(totalTime);
                string transitionMessage = "Congratulations, you beat level 1!\n\nYour score is " + score.GetScore() + "\n\nPress ENTER to start Level 2.";
                IGameState nextState = new PlayingState(gameStateHandler, 2, score.CurrentScore);
                gameStateHandler.ChangeState(new LevelTransitionState(gameStateHandler, font, transitionMessage, nextState, 2, score.GetScore()));
            }
            if (gameOver)
            {

                SFXHandler.PlayerDyingSound.Play();
                gameStateHandler.ChangeState(new GameOverState(gameStateHandler, font, score.GetScore(), false));
            }
            spriteBatch.End();
        }

        private void RemovePowerUp(PowerUp powerUp)
        {
            if (powerUp != null)
            {
                powerUps.Remove(powerUp);
            }
        }
        //using the int grid it checks for collision
        private string CheckForCollision(Vector2 newPosition)
        {
            Rectangle playerHitbox = new Rectangle(
                (int)newPosition.X + player.hitboxSideOffset,
                (int)newPosition.Y + player.hitboxTopOffset,
                player.width - 2 * player.hitboxSideOffset,
                player.height - player.hitboxTopOffset);

            // Check each corner of the player's bounding box for collision
            foreach (Vector2 corner in new Vector2[]
            {
                new Vector2(playerHitbox.Left, playerHitbox.Top + player.height /3), //offseting by 3 because the height of the sprite isn't 16
				new Vector2(playerHitbox.Right, playerHitbox.Top + player.height /3),//otherwise sprite when under wall would stop early
				new Vector2(playerHitbox.Left, playerHitbox.Bottom),
                new Vector2(playerHitbox.Right, playerHitbox.Bottom)
            })
            {
                int gridX = (int)corner.X / TILE_SIZE;
                int gridY = (int)corner.Y / TILE_SIZE;


                if (currentLevel.Grid[gridY, gridX] == 1)
                {
                    return "wall"; // Collision detected
                }
                else if (currentLevel.Grid[gridY, gridX] == 2)
                {
                    Rectangle relicHitbox = new Rectangle(
                        gridX * TILE_SIZE,
                        gridY * TILE_SIZE,
                        TILE_SIZE ,
                        TILE_SIZE
                    );

                    // Check if the player's hitbox intersects with the relic hitbox
                    if (player.EnvironmentHitbox.Intersects(relicHitbox))
                    {
                        Relic removedRelic = null;
                        foreach (Relic relic in relics)
                        {
                            //detecting which relic was hit
                            Vector2 relicPosition = new Vector2(gridX * TILE_SIZE + 5, gridY * TILE_SIZE);
                            if (relicPosition == relic.Position)
                            {
                                //making sure relic was not already opened
                                if (relic.IsDeleted == false)
                                {
                                    relic.IsDeleted = true;
                                    int relicScore = relic.Score;
                                    score.AddScore(relicScore);
                                    removedRelic = relic;
                                    SFXHandler.RelicOpenSound.Play();
                                }

                            }
                        }
                        if (removedRelic != null)
                        {
                            //remove relic from list
                            relics.Remove(removedRelic);
                        }
                        return "relic";
                    }

                }
                else if (currentLevel.Grid[gridY, gridX] == 3)
                {
                    if (!buttonPressed)
                    {
                        buttonPressed = true;
                        buttonClickSE.Play();
                        score.AddScore(100);
                    }
                    //sound is off by about 8 miliseconds
                    timingDelay++;
                    if (timingDelay > 8)
                    {
                        door = content.Load<Texture2D>("Door_Open");
                        button = content.Load<Texture2D>("Button_pressed");
                    }
                    //update the new level int grid - used for collision management
                    string csvRelativePath = "IntGrid_Switch_Clicked.csv";
                    string csvFilePath = Path.Combine(contentRootDirectory, csvRelativePath);
                    if (File.Exists(csvFilePath))
                    {
                        currentLevel = new LevelHandler(csvFilePath);
                    }
                    else
                    {
                        throw new FileNotFoundException("Unable to load walls.csv. File not found.");
                    }
                    UnloadContent();
                    return "button";
                }
                else if (currentLevel.Grid[gridY, gridX] == 6)
                {
                    levelComplete = true;
                    return "complete";
                }
                else if (currentLevel.Grid[gridY, gridX] == 8)
                {
                    return "powerUp";

                }
                else if (currentLevel.Grid[gridY, gridX] == 9)
                {
                    Rectangle spikeHitbox = new Rectangle(
                        gridX * TILE_SIZE + SPIKE_HITBOX_OFFSET,
                        gridY * TILE_SIZE + SPIKE_HITBOX_OFFSET,
                        TILE_SIZE - 2 * SPIKE_HITBOX_OFFSET,
                        TILE_SIZE - 2 * SPIKE_HITBOX_OFFSET
                    );

                    // Check if the player's hitbox intersects with the spike hitbox
                    if (playerHitbox.Intersects(spikeHitbox))
                    {
                        // Handle collision with spike
                        gameOver = true;
                    }

                }

            }
            return "nothing"; // No collision
        }

        public override void UnloadContent()
        {
            GC.Collect();
        }
    }
}