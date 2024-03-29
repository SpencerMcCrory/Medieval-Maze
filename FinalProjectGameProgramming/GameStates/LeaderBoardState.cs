﻿using FinalProjectGameProgramming.Handlers;
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
    /// <summary>
    /// LeaderBoardState is the state that shows the top 10 scores
    /// </summary>
    internal class LeaderBoardState : IGameState
    {
        private GraphicsDeviceManager graphicsDeviceManager;
        private GameStateHandler gameStateHandler;
        private SpriteFont _font;
        private ContentManager content;
        private GraphicsDevice graphicsDevice;
        private MouseState mState;
        private Texture2D leaderBoardBG;
        private int screenWidth;
        private int screenHeight;
        // A dictionary of the top 5 scores
        // Using a static list to store the scores in memory
        private static List<Dictionary<string, float>> leaderBoardScores = new List<Dictionary<string, float>>(){
            new Dictionary<string, float>(){
                {"Alex", 1000},
            },
            new Dictionary<string, float>(){
                {"Bob", 800},
            },
            new Dictionary<string, float>(){
                {"Alice", 600},
            },
            new Dictionary<string, float>(){
                {"Ethan", 400},
            },
            new Dictionary<string, float>(){
                {"Perry", 200},
            }
        };
        private Dictionary<string, float> _userScore = new Dictionary<string, float>(){
            {"User", 0}
        };

        private bool isLoadedSavedFile = false;
        private CustomButton closedButton;


        public LeaderBoardState(GameStateHandler gameStateHandler, SpriteFont font, Dictionary<string, float> userScore = null)
        {
            this.gameStateHandler = gameStateHandler;
            graphicsDeviceManager = gameStateHandler.GraphicsDeviceManager;
            content = gameStateHandler.Content;
            _font = font;
            graphicsDevice = gameStateHandler.GraphicsDevice;
            _userScore = userScore;
            screenWidth = graphicsDevice.Viewport.Width;
            screenHeight = graphicsDevice.Viewport.Height;

            LoadLeaderBoardScoresFromSavedFile();
            AddUserScoreToLeaderBoard();

            // Load the button textures
            Texture2D buttonReleasedTexture = content.Load<Texture2D>("Button_Release_01a2");
            Texture2D buttonPressedTexture = content.Load<Texture2D>("Button_Release_01a1");

            // Create the close button
            closedButton = new CustomButton(buttonReleasedTexture, buttonPressedTexture, graphicsDevice, _font, "X");
            closedButton.SetPosition(new Vector2(100, 10)); // Set position for Close button
            closedButton.SetSize(new Vector2(50, 50)); // Set size for Close button

        }

        private void LoadLeaderBoardScoresFromSavedFile()
        {
            // If the leaderboard scores are not loaded from the saved file
            if (!isLoadedSavedFile)
            {
                SaveHandler saveHandler = new SaveHandler();
                List<Dictionary<string, float>> loadedUserScore = saveHandler.LoadLeaderboardFile();
                if (loadedUserScore != null && loadedUserScore.Count > 0)
                {
                    leaderBoardScores = loadedUserScore;
                }
            }
        }

        private void AddUserScoreToLeaderBoard()
        {
            if (_userScore != null)
            {
                // Add the user score to the leaderboard
                leaderBoardScores.Add(_userScore);
                // Sort the leaderboard
                leaderBoardScores = leaderBoardScores.OrderByDescending(score => score.Values.First()).ToList();
                // Save the leaderboard to the file
                new SaveHandler().SaveLeaderboardFile(ConvertLeaderBoardToString());
            }
        }

        private string ConvertLeaderBoardToString(){
            string leaderBoardString = "";
            for (int i = 0; i < leaderBoardScores.Count; i++)
            {
                // Get the current score
                Dictionary<string, float> score = leaderBoardScores[i];
                // Get the name of the player
                string name = score.Keys.First();
                // Get the score of the player
                string playerScore = score.Values.First().ToString();
                // Add the name and score to the string
                leaderBoardString += name + "," + playerScore + "\n";
            }
            return leaderBoardString;
        }

        public void Enter()
        {
            leaderBoardBG = content.Load<Texture2D>("LeaderBoardBG");
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
                gameStateHandler.ChangeState(new MainMenu(gameStateHandler, _font));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            //draw bg image
            spriteBatch.Draw(leaderBoardBG, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

            // Draw the close button
            closedButton.Draw(spriteBatch);

            // Loop through the top 5 scores
            int nameStartingPosition = 300;
            int scoreStartingPosition = graphicsDeviceManager.PreferredBackBufferWidth - 380;
            int yStartingPosition = 150;
            int yIncrement = 50;
            int leaderboardLimit = leaderBoardScores.Count > 10 ? 10 : leaderBoardScores.Count;
            for (int i = 0; i < leaderboardLimit; i++)
            {
                // Get the current score
                Dictionary<string, float> score = leaderBoardScores[i];
                // Get the name of the player
                string name = (i + 1).ToString() + ". " + score.Keys.First();
                // Get the score of the player
                string playerScore = score.Values.First().ToString();
                // Calculate positions
                Vector2 namePosition = new Vector2(nameStartingPosition, yStartingPosition + (i * yIncrement));
                Vector2 scorePosition = new Vector2(scoreStartingPosition, yStartingPosition + (i * yIncrement));

                // Draw the name and score
                spriteBatch.DrawString(_font, name, namePosition, Color.White);
                spriteBatch.DrawString(_font, playerScore, scorePosition, Color.White);

                // Draw a line connecting name and playerScore
                Vector2 lineStartingPoint = namePosition + new Vector2(_font.MeasureString(name).X, _font.MeasureString(name).Y);
                Vector2 lineEndingPoint = scorePosition + new Vector2(0, _font.MeasureString(playerScore).Y);
                DrawLine(spriteBatch, lineStartingPoint, lineEndingPoint, Color.White);
            }
            spriteBatch.End();
        }

        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness = 2)
        {
            Texture2D pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            float length = Vector2.Distance(start, end);

            spriteBatch.Draw(pixel, start, null, color, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0);
        }
    }
}
