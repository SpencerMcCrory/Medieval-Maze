﻿using FinalProjectGameProgramming.Entities;
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
using System.Diagnostics.Eventing.Reader;
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
		private float movementDelta;
		const int SPIKE_HITBOX_OFFSET = 10;
		private Texture2D playerTexture;
		private Texture2D slimeTexture;
		Player player;
		Texture2D[] slimeRunFrames;
		private Texture2D bigZombieTexture;
		private Rectangle monsterHitbox;
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


			// Load textures
			backgroundTexture = _content.Load<Texture2D>("Background");
			wallsTexture = _content.Load<Texture2D>("Walls_BackGround");
			door = _content.Load<Texture2D>("Door_Closed");
			button = _content.Load<Texture2D>("Button_not_pressed");
			playerTexture = _content.Load<Texture2D>("knight_f_run_anim_f0");

			slimeTexture = _content.Load<Texture2D>("swampy_anim_f0");
			bigZombieTexture = _content.Load<Texture2D>("big_zombie_run_anim_f0");

			float bigZombieSpeed = 32f; // Slower speed for BigZombie
			Vector2 bigZombieDirection = new Vector2(1, 0); // Initial direction for BigZombie

			float slimeMonsterSpeed = 64f; // Faster speed for SlimeMonster
			Vector2 slimeMonsterDirection = new Vector2(-1, 0); // Initial direction for SlimeMonster


			foreach (int[] spawnPoint in currentLevel.MonsterSpawnPoints)
			{
				if (spawnPoint[0] % 2 == 1)
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

			buttonClickSE = _content.Load<SoundEffect>("futuristic_button_click");
			font = _content.Load<SpriteFont>("galleryFont");

		}

		public override void UpdateLevel(GameTime gameTime)
		{
			/*if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();*/
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

			Vector2 newPlayerPosition = player.Position + direction * movementDelta;

			string collidingWith = CheckForCollision(newPlayerPosition);
			// Check for collisions with the wall
			if (collidingWith != "wall")
			{
				// Apply movement
				player.Position = newPlayerPosition;
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
			spriteBatch.Draw(wallsTexture, Vector2.Zero, Color.White);

			spriteBatch.Draw(door, Vector2.Zero, Color.White);
			spriteBatch.Draw(button, Vector2.Zero, Color.White);


			AnimationHandler currentAnimation = animations[currentAnimationKey];
			spriteBatch.Draw(currentAnimation.Frames[currentFrame], player.Position, Color.White);

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
				spriteBatch.Draw(pixel, monsterHitbox, Color.Red * 0.5f);
				monster.Draw(spriteBatch);
			}



			//for Debugging hitbox Draw the semi - transparent hitbox
			Color hitboxColorEnvironment = new Color(Color.Red, 0.5f); // Semi-transparent red
			spriteBatch.Draw(pixel, player.EnvironmentHitbox, hitboxColorEnvironment);
			Color hitboxColorMonster = new Color(Color.Red, 0.5f); // Semi-transparent red
			spriteBatch.Draw(pixel, player.MonsterHitbox, hitboxColorMonster);

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
				string transitionMessage = "Congratulations, you beat level 1!\nYour score is " + score.GetScore() + "\nPress ENTER to start Level 2.";
				IGameState nextState = new PlayingState(_graphics, _content, _graphicsDevice, gameStateHandler, 2, score.CurrentScore);
				gameStateHandler.ChangeState(new LevelTransitionState(_graphics, gameStateHandler, font, transitionMessage, nextState));
			}
			if (gameOver)
			{
				string transitionMessage = "Game Over!\nYour score is " + score.GetScore();
				// IGameState nextState = new MainMenu(gameStateHandler, font, _graphics, _content, _graphicsDevice);
				gameStateHandler.ChangeState(new GameOverState(_graphics, gameStateHandler, _content, _graphicsDevice, font, transitionMessage, score.GetScore()));
			}
			spriteBatch.End();
		}

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
					UnloadContent();
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