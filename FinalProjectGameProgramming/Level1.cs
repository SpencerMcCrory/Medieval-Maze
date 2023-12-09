using FinalProjectGameProgramming.Entities;
using FinalProjectGameProgramming.GameStates;
using FinalProjectGameProgramming.Handlers;
using FinalProjectGameProgramming.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Windows.Forms;
using System.Xml.Linq;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace FinalProjectGameProgramming
{
	public class Level1 : Level
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
		private float movementDelta;
		const int SPIKE_HITBOX_OFFSET = 10;
		private Texture2D playerTexture;
		private Texture2D slimeTexture;
		Player player;
		Texture2D[] slimeRunFrames;
		private Texture2D bigZombieTexture;
		List<Monster> monsters = new List<Monster>();

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
		private GameStateHandler gameStateHandler;
		private ScoreHandler score;
		// Get the path of the directory that contains the content
		private string contentRootDirectory;


		internal Level1(GraphicsDeviceManager graphics, ContentManager content, GraphicsDevice graphicsDevice, GameStateHandler gameStateHandler)
		{
			// Initialize fields using passed-in graphics and content
			_graphics = graphics;
			_content = content;
			_graphicsDevice = graphicsDevice;
			this.gameStateHandler = gameStateHandler;
			score = new ScoreHandler(0);
			contentRootDirectory = _content.RootDirectory;
		}

		public override void Initialize()
		{
			//can't get relative path to work for some reason
			//string csvFilePath = "C:\\PROG2370-23F\\Monogame\\FinalProjectGameProgramming\\FinalProjectGameProgramming\\Content\\IntGrid_Switch_Not_Clicked.csv";
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
			playerPosition = new Vector2(currentLevel.StartPoint[0] * tileSize, currentLevel.StartPoint[1] * tileSize); // Starting position of the player
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


			// Load textures
			backgroundTexture = _content.Load<Texture2D>("Background");
			wallsTexture = _content.Load<Texture2D>("Walls_BackGround");
			door = _content.Load<Texture2D>("Door_Closed");
			button = _content.Load<Texture2D>("Button_not_pressed");
			playerTexture = _content.Load<Texture2D>("knight_f_run_anim_f0");

			slimeTexture = _content.Load<Texture2D>("swampy_anim_f0");
			bigZombieTexture = _content.Load<Texture2D>("big_zombie_run_anim_f0");

			float bigZombieSpeed = 16f; // Slower speed for BigZombie
			Vector2 bigZombieDirection = new Vector2(1, 0); // Initial direction for BigZombie

			float slimeMonsterSpeed = 32f; // Faster speed for SlimeMonster
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

			buttonClickSE = _content.Load<SoundEffect>("futuristic_button_click");
			font = _content.Load<SpriteFont>("galleryFont");

			//hitbox debug
			/*pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });*/
		}

		public override void UpdateLevel(GameTime gameTime)
		{
			/*if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();*/

			//will be used to update the monsters
			foreach (Monster monster in monsters)
			{
				monster.Update(gameTime);
				Rectangle monsterBounds = new Rectangle((int)monster.Position.X,
														(int)monster.Position.Y,
														monster.Texture.Width,
														monster.Texture.Height);
				if (collisionHandler.CheckCollisionWithEnvironment(monsterBounds))
				{
					// Handle monster collision with the environment
				}
			}
			movementDelta = (float)gameTime.ElapsedGameTime.TotalSeconds * player.speed;

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
				direction.Y -= 1;
			if (state.IsKeyDown(Keys.Down) || state.IsKeyDown(Keys.S))
				direction.Y += 1;
			if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
				direction.X -= 1;
			if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
				direction.X += 1;

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

			elapsedTime += gameTime.ElapsedGameTime;
		}

		public override void DrawLevel(SpriteBatch spriteBatch)
		{
			_graphicsDevice.Clear(Color.Black);

			// Use the camera's view matrix
			spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

			// Draw the background, player, walls, etc.
			spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);
			spriteBatch.Draw(wallsTexture, Vector2.Zero, Color.White);

			spriteBatch.Draw(door, Vector2.Zero, Color.White);
			spriteBatch.Draw(button, Vector2.Zero, Color.White);

			AnimationHandler currentAnimation = animations[currentAnimationKey];
			spriteBatch.Draw(currentAnimation.Frames[currentFrame], playerPosition, Color.White);

			foreach (Monster monster in monsters)
			{
				monster.Draw(spriteBatch);
			}

			Rectangle playerHitbox = new Rectangle(
				(int)playerPosition.X + player.hitboxSideOffset, // X position plus side offset
				(int)playerPosition.Y + player.hitboxTopOffset, // Y position plus top offset
				player.width - 2 * player.hitboxSideOffset, // Width minus both side offsets
				player.height - player.hitboxTopOffset
			);

			//for Debugging hitbox Draw the semi-transparent hitbox
			/*Color hitboxColor = new Color(Color.Red, 0.5f); // Semi-transparent red
            spriteBatch.Draw(pixel, playerHitbox, hitboxColor);*/

			spriteBatch.End();
			//has to have it's own begin to not be affected by the camera positioning
			spriteBatch.Begin();
			// Draw the timer
			spriteBatch.DrawString(font, $"Time: {elapsedTime.Minutes:D2}:{elapsedTime.Seconds:D2}", new Vector2(10, 10), Color.White);

			// Draw the score
			spriteBatch.DrawString(font, $"Score: {score.GetScore()}", new Vector2(10, 30), Color.White);
			spriteBatch.End();

			//has to have it's own begin and and end otherwise wont pop up
			spriteBatch.Begin();

			if (levelComplete)
			{
				int totalTime = (int)elapsedTime.TotalSeconds;
				score.CalculateTimeScore(totalTime);
				string transitionMessage = "Congratulations, you beat level 1!\nYour score is " + score.GetScore() + "\nPress ENTER to start Level 2.";
				IGameState nextState = new PlayingState(_graphics, _content, _graphicsDevice, gameStateHandler, 2, score.CurrentScore);
				gameStateHandler.ChangeState(new LevelTransitionState(gameStateHandler, font, transitionMessage, nextState));
			}
			if (gameOver)
			{
				// spriteBatch.DrawString(_content.Load<SpriteFont>("galleryFont"), "Game Over. You Died.", new Vector2(100, 100), Color.White);
				int totalTime = (int)elapsedTime.TotalSeconds;
				score.CalculateTimeScore(totalTime);
				string transitionMessage = "Game Over!\nYour score is " + score.GetScore() + "\nPress ENTER to continue.";
				IGameState nextState = new MainMenu(gameStateHandler, font, _graphics, _content, _graphicsDevice);
				gameStateHandler.ChangeState(new GameOverState(gameStateHandler, font, transitionMessage, nextState));
			}
			spriteBatch.End();
		}

		private string CheckForCollision(Vector2 position)
		{
			Rectangle playerBounds = new Rectangle((int)position.X + player.hitboxSideOffset, (int)position.Y + player.hitboxTopOffset, player.width - 2 * player.hitboxSideOffset, player.height - player.hitboxTopOffset);

			// Check each corner of the player's bounding box for collision
			foreach (Vector2 corner in new Vector2[]
			{
				new Vector2(playerBounds.Left, playerBounds.Top + player.height /3), //offseting by 3 because the height of the sprite isn't 16
				new Vector2(playerBounds.Right, playerBounds.Top + player.height /3),//otherwise sprite when under wall would stop early
				new Vector2(playerBounds.Left, playerBounds.Bottom),
				new Vector2(playerBounds.Right, playerBounds.Bottom)
			})
			{
				int gridX = (int)corner.X / tileSize;
				int gridY = (int)corner.Y / tileSize;


				if (currentLevel.Grid[gridY, gridX] == 1)
					return "wall"; // Collision detected
				if (currentLevel.Grid[gridY, gridX] == 3)
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
						door = _content.Load<Texture2D>("Door_Open");
						button = _content.Load<Texture2D>("Button_pressed");
					}
					//string csvFilePath = "C:\\PROG2370-23F\\Monogame\\FinalProjectGameProgramming\\FinalProjectGameProgramming\\Content\\IntGrid_Switch_Clicked.csv";
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
					return "button";
				}
				if (currentLevel.Grid[gridY, gridX] == 6)
				{
					levelComplete = true;
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
					if (playerBounds.Intersects(spikeHitbox))
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
			throw new NotImplementedException();
		}
	}
}