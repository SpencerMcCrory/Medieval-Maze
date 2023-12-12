using FinalProjectGameProgramming.Entities;
using FinalProjectGameProgramming.Handlers;
using FinalProjectGameProgramming.Levels;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalProjectGameProgramming.GameStates;

namespace FinalProjectGameProgramming
{
	internal class Level2 : Level
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch spriteBatch;
		private Texture2D backgroundTexture;
		private Texture2D wallsTexture;
		private Texture2D greenSpikes;
		private Texture2D purpleSpikes;
		private Texture2D door;
		private Texture2D button;
		private LevelHandler currentLevel;
		private int tileSize = 64;
		private float movementDelta;
		const int SPIKE_HITBOX_OFFSET = 10;
		Player player;
		Texture2D[] slimeRunFrames;
		Rectangle monsterHitbox;
		List<Relic> relics = new List<Relic>();
		List<Monster> monsters = new List<Monster>();
        List<PowerUp> powerUps = new List<PowerUp>();
        PowerUp removedPowerUp = null;

        int timingDelay = 0;

		private Texture2D[] playerAnimationFrames;
		const int playerFrameCount = 4;

		private string currentAnimationKey = "playerIdle";
		private int currentFrame;
		private double animationTimer;
		private double timePerFrame = 0.4;

		private Dictionary<string, AnimationHandler> animations;

		private CameraHandler camera;
		private CollisionHandler collisionHandler;

		SpriteFont font;

		string debugText;
		bool levelComplete;
		bool gameOver;
		bool buttonPressed;

		Texture2D pixel;

		SoundEffect buttonClickSE;

		private TimeSpan elapsedTime = TimeSpan.Zero;

		private ContentManager _content;
		private GraphicsDevice _graphicsDevice;
		private Game _game;
		private ScoreHandler score;
		private GameStateHandler gameStateHandler;
		// Get the root directory of the content project
		private string rootContentDirectory;
		public Level2(GraphicsDeviceManager graphics, ContentManager content, GraphicsDevice graphicsDevice, GameStateHandler gameStateHandler, int currentScore)
		{
			// Initialize fields using passed-in graphics and content
			_graphics = graphics;
			_content = content;
			_graphicsDevice = graphicsDevice;
			score = new ScoreHandler(currentScore);
			this.gameStateHandler = gameStateHandler;
			rootContentDirectory = _content.RootDirectory;
		}

		public override void Initialize()
		{

			//can't get relative path to work for some reason
			//string csvFilePath = "C:\\PROG2370-23F\\Monogame\\FinalProjectGameProgramming\\FinalProjectGameProgramming\\Content\\Level2_IntGrid_Switch_Not_Clicked.csv";
			string csvFileRelativePath = "Level2_IntGrid_Switch_Not_Clicked.csv";
			string csvFilePath = Path.Combine(rootContentDirectory, csvFileRelativePath);
			if (File.Exists(csvFilePath))
			{
				currentLevel = new LevelHandler(csvFilePath);
			}
			else
			{
				throw new FileNotFoundException("Unable to load walls.csv. File not found.");
			}
			player = new Player();
			player.Position = new Vector2(currentLevel.StartPoint[0] * tileSize, currentLevel.StartPoint[1] * tileSize); // Starting position of the player
			camera = new CameraHandler();
			collisionHandler = new CollisionHandler(currentLevel, tileSize);
			buttonPressed = false;

		}

		public override void LoadLevelContent()
		{
			/*spriteBatch = new SpriteBatch(_graphicsDevice);*/

			animations = new Dictionary<string, AnimationHandler>();

			// Load idle animation frames for player
			Texture2D[] playerIdleFrames = new Texture2D[4];
			for (int i = 0; i < 4; i++)
			{
				playerIdleFrames[i] = _content.Load<Texture2D>($"knight_f_idle_anim_f{i}");
			}
			animations["playerIdle"] = new AnimationHandler(playerIdleFrames, 1);



			// Load run animation frames for player
			Texture2D[] playerRunFrames = new Texture2D[4];
			for (int i = 0; i < 4; i++)
			{
				playerRunFrames[i] = _content.Load<Texture2D>($"knight_f_run_anim_f{i}");
			}
			animations["playerRun"] = new AnimationHandler(playerRunFrames, 0.1);

			//Load run animation frames for slime monster
			Texture2D[] slimeRunFrames = new Texture2D[4];
			for (int i = 0; i < 4; i++)
			{
				slimeRunFrames[i] = _content.Load<Texture2D>($"swampy_anim_f{i}");
			}

			//Load run animation frames for big zombie monster
			Texture2D[] bigZombieRunFrames = new Texture2D[4];
			for (int i = 0; i < 4; i++)
			{
				bigZombieRunFrames[i] = _content.Load<Texture2D>($"big_zombie_run_anim_f{i}");
			}
			//Load relic open animation frames
			Texture2D[] relicFrames = new Texture2D[3];
			for (int i = 0; i < 3; i++)
			{
				relicFrames[i] = _content.Load<Texture2D>($"chest_full_open_anim_f{i}");
			}

            

            // Load textures
            backgroundTexture = _content.Load<Texture2D>("compositeBGLevel2");
            Texture2D speedPowerUpTexture = _content.Load<Texture2D>("flask_blue_powerup");

            door = _content.Load<Texture2D>("Level2_Door_Closed");
			button = _content.Load<Texture2D>("L2_Button_not_pressed");
			greenSpikes = _content.Load<Texture2D>("Spikes_green");
			purpleSpikes = _content.Load<Texture2D>("Spikes_purple");


			float bigZombieSpeed = 32f; // Slower speed for BigZombie
			Vector2 bigZombieDirection = new Vector2(0, 1); // Initial direction for BigZombie

			float slimeMonsterSpeed = 64f; // Faster speed for SlimeMonster
			Vector2 slimeMonsterDirection = new Vector2(-1, 0); // Initial direction for SlimeMonster


			foreach (int[] spawnPoint in currentLevel.MonsterSpawnPoints)
			{
				if (spawnPoint[0] % 2 == 0)
				{
					Vector2 position = new Vector2(spawnPoint[0] * tileSize - 15, spawnPoint[1] * tileSize - 60);
					monsters.Add(new BigZombie(bigZombieRunFrames, position, collisionHandler, bigZombieSpeed, bigZombieDirection));
				}
				else
				{
					Vector2 position = new Vector2(spawnPoint[0] * tileSize + 5, spawnPoint[1] * tileSize);
					monsters.Add(new SlimeMonster(slimeRunFrames, position, collisionHandler, slimeMonsterSpeed, slimeMonsterDirection));
				}
			}
			//used for drawing hitboxes
			pixel = new Texture2D(_graphicsDevice, 1, 1);
			pixel.SetData(new[] { Color.White });

			foreach (int[] spawnPoint in currentLevel.RelicSpawnPoints)
			{

				Vector2 position = new Vector2(spawnPoint[0] * tileSize + 5, spawnPoint[1] * tileSize);
				relics.Add(new Relic(position, relicFrames, 0.08));

			}

            foreach (int[] spawnPoint in currentLevel.PowerUpLocations)
            {
                Vector2 position = new Vector2(spawnPoint[0] * tileSize, spawnPoint[1] * tileSize);
                powerUps.Add(new SpeedPowerUp(position, speedPowerUpTexture, "speed"));

            }

            buttonClickSE = _content.Load<SoundEffect>("futuristic_button_click");
			font = _content.Load<SpriteFont>("galleryFont");

		}

		public override void UpdateLevel(GameTime gameTime)
		{
			/*if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();*/

			Rectangle playerHitbox = new Rectangle(
				(int)player.Position.X + player.hitboxSideOffset,
				(int)player.Position.Y + player.hitboxTopOffset,
				player.width - 2 * player.hitboxSideOffset,
				player.height - player.hitboxTopOffset);

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
					monsterHitbox = new Rectangle(
												(int)monster.Position.X,
												(int)monster.Position.Y,
												monster.Texture.Width,
												monster.Texture.Height);
				}

				if (collisionHandler.CheckCollisionWithEnvironment(monsterHitbox))
				{
					// Handle monster collision with the environment
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
                    if (playerHitbox.Intersects(monsterHitbox))
				{
					gameOver = true;
				}
			}
			movementDelta = (float)gameTime.ElapsedGameTime.TotalSeconds * player.Speed;

			animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
			if (animationTimer > timePerFrame)
			{
				currentFrame = (currentFrame + 1) % playerFrameCount;
				animationTimer -= timePerFrame;
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

                    // Call a method on the power-up to handle the player collecting it
                    powerUp.Collect(player);
                    powerUp.SetDurationTimer(elapsedTime.TotalSeconds);
                    SFXHandler.SpeedPotion.Play();


                    // Optionally play a sound effect or animation to indicate collection
                    // SFXHandler.PowerUpCollectedSound.Play();
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
            //update relic
            foreach (Relic relic in relics)
			{
				relic.Update(gameTime);
			}

			//get the center of the window
			Microsoft.Xna.Framework.Vector2 tempVec = player.Position - new Microsoft.Xna.Framework.Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);

			//update the camera position
			camera.Position = new System.Numerics.Vector2(tempVec.X, tempVec.Y);

			elapsedTime += gameTime.ElapsedGameTime;
		}

		public override void DrawLevel(SpriteBatch spriteBatch)
		{
			_graphicsDevice.Clear(Color.Black);


			// Use the camera's view matrix
			spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

			// Draw the background, player, walls, etc.
			spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);

			spriteBatch.Draw(door, Vector2.Zero, Color.White);
			spriteBatch.Draw(button, Vector2.Zero, Color.White);
			spriteBatch.Draw(greenSpikes, Vector2.Zero, Color.White);
			spriteBatch.Draw(purpleSpikes, Vector2.Zero, Color.White);

			SpriteEffects spriteEffects = player.IsFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			AnimationHandler currentAnimation = animations[currentAnimationKey];
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
			// spriteBatch.Draw(
			// 	currentAnimation.Frames[currentFrame], player.Position, Color.White);

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

            //for Debugging hitbox Draw the semi - transparent hitbox
            /* Color hitboxColorEnvironment = new Color(Color.Red, 0.5f); // Semi-transparent red
			spriteBatch.Draw(pixel, player.EnvironmentHitbox, hitboxColorEnvironment);
			Color hitboxColorMonster = new Color(Color.Red, 0.5f); // Semi-transparent red
			spriteBatch.Draw(pixel, player.MonsterHitbox, hitboxColorMonster); */

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

			if (levelComplete)
			{
                // IGameState nextState = new MainMenu(gameStateHandler, font, _graphics, _content, _graphicsDevice);
                gameStateHandler.ChangeState(new GameOverState( gameStateHandler, font, score.GetScore(),true));
            }
			if (gameOver)
			{
                SFXHandler.PlayerDyingSound.Play();
                // IGameState nextState = new MainMenu(gameStateHandler, font, _graphics, _content, _graphicsDevice);
                gameStateHandler.ChangeState(new GameOverState(gameStateHandler, font, score.GetScore(),false));

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

        private string CheckForCollision(Vector2 position)
		{
			Rectangle playerHitbox = new Rectangle(
				(int)position.X + player.hitboxSideOffset,
				(int)position.Y + player.hitboxTopOffset,
				player.width - 2 * player.hitboxSideOffset,
				player.height - player.hitboxTopOffset);

			// Check each corner of the player's bounding box for collision
			foreach (Vector2 corner in new Vector2[] {
				new Vector2(playerHitbox.Left, playerHitbox.Top + player.height /3), //offseting by 3 because the height of the sprite isn't 16
				new Vector2(playerHitbox.Right, playerHitbox.Top + player.height /3),//otherwise sprite when under wall would stop early
				new Vector2(playerHitbox.Left, playerHitbox.Bottom),
				new Vector2(playerHitbox.Right, playerHitbox.Bottom)
			})
			{
				int gridX = (int)corner.X / tileSize;
				int gridY = (int)corner.Y / tileSize;


				if (currentLevel.Grid[gridY, gridX] == 1)
				{
					return "wall"; // Collision detected
				}
				else if (currentLevel.Grid[gridY, gridX] == 2)
				{
					Rectangle relicHitbox = new Rectangle(
						gridX * tileSize + SPIKE_HITBOX_OFFSET,
						gridY * tileSize + SPIKE_HITBOX_OFFSET,
						tileSize - 2 * SPIKE_HITBOX_OFFSET,
						tileSize - 2 * SPIKE_HITBOX_OFFSET
					);

					// Check if the player's hitbox intersects with the relic hitbox
					if (playerHitbox.Intersects(relicHitbox))
					{
						Relic removedRelic = null;
						foreach (Relic relic in relics)
						{
							//detecting which relic was hit
							Vector2 relicPosition = new Vector2(gridX * tileSize + 5, gridY * tileSize);
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
				if (currentLevel.Grid[gridY, gridX] == 3)
				{
					if (!buttonPressed)
					{
						buttonPressed = true;
						buttonClickSE.Play();
						score.AddScore(200);
					}
					//sound is off by about 8 miliseconds
					timingDelay++;
					if (timingDelay == 8)
					{
						door = _content.Load<Texture2D>("Level2_Door_Open");
						button = _content.Load<Texture2D>("L2_Button_pressed");
						greenSpikes = _content.Load<Texture2D>("Spikes_green_button_pressed");
						purpleSpikes = _content.Load<Texture2D>("Spikes_purple_button_clicked");

                        string csvFileRelativePath = "Level2_IntGrid_Switch_Clicked.csv";
                        string csvFilePath = Path.Combine(rootContentDirectory, csvFileRelativePath);
                        if (File.Exists(csvFilePath))
                        {
                            currentLevel = new LevelHandler(csvFilePath);
                        }
                        else
                        {
                            throw new FileNotFoundException("Unable to load walls.csv. File not found.");
                        }
                        UnloadContent();
                    }
					//string csvFilePath = "C:\\PROG2370-23F\\Monogame\\FinalProjectGameProgramming\\FinalProjectGameProgramming\\Content\\Level2_IntGrid_Switch_Clicked.csv";
					
                    return "button";
				}
				if (currentLevel.Grid[gridY, gridX] == 6)
				{
					levelComplete = true;
				}
                if (currentLevel.Grid[gridY, gridX] == 8)
                {
                    return "powerUp";

                }
                if (currentLevel.Grid[gridY, gridX] == 9)
				{
					Rectangle spikeHitbox = new Rectangle(
						gridX * tileSize + SPIKE_HITBOX_OFFSET,
						gridY * tileSize + SPIKE_HITBOX_OFFSET,
						tileSize - 2 * SPIKE_HITBOX_OFFSET,
						tileSize - 2 * SPIKE_HITBOX_OFFSET
					);

					// Check if the player's hitbox intersects with the spike hitbox
					if (playerHitbox.Intersects(spikeHitbox))
					{
						// Handle collision with spike
						gameOver = true;
						return "spikes";
					}

				}
				if (currentLevel.Grid[gridY, gridX] == 11)
				{
					Rectangle noFloorHitbox = new Rectangle(
						gridX * tileSize + SPIKE_HITBOX_OFFSET,
						gridY * tileSize + SPIKE_HITBOX_OFFSET,
						tileSize - 2 * SPIKE_HITBOX_OFFSET,
						tileSize - 2 * SPIKE_HITBOX_OFFSET
					);

					// Check if the player's hitbox intersects with the spike hitbox
					if (playerHitbox.Intersects(noFloorHitbox))
					{
						// Handle collision with no floor
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
