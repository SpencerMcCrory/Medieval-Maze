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
    internal class LeaderBoardState : IGameState
    {
        private GraphicsDeviceManager _graphics;
        private GameStateHandler _gameStateHandler;
        private SpriteFont _font;
        private ContentManager _content;
        private GraphicsDevice _graphicsDevice;
        private MouseState mState;
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
        private Button closedButton;


        public LeaderBoardState(GraphicsDeviceManager graphics, ContentManager content, GameStateHandler gameStateHandler, GraphicsDevice graphicsDevice, SpriteFont font, Dictionary<string, float> userScore = null)
        {
            _graphics = graphics;
            _content = content;
            _gameStateHandler = gameStateHandler;
            _font = font;
            _graphicsDevice = graphicsDevice;
            _userScore = userScore;
            AddUserScoreToLeaderBoard();

            // Load the button textures
            Texture2D buttonReleasedTexture = content.Load<Texture2D>("Button_Release_01a2");
            Texture2D buttonPressedTexture = content.Load<Texture2D>("Button_Release_01a1");

            // Create the close button
            closedButton = new Button(buttonReleasedTexture, buttonPressedTexture, graphicsDevice, _font, "X");
            closedButton.SetPosition(new Vector2(100, 10)); // Set position for Close button
            closedButton.SetSize(new Vector2(50, 50)); // Set size for Close button

        }

        private void AddUserScoreToLeaderBoard()
        {
            if (_userScore != null)
            {
                // Add the user score to the leaderboard
                leaderBoardScores.Add(_userScore);
                // Sort the leaderboard
                leaderBoardScores = leaderBoardScores.OrderByDescending(score => score.Values.First()).ToList();
            }
        }

        public void Enter() { }

        public void Exit() { }

        public void Update(GameTime gameTime)
        {
            mState = Mouse.GetState();
            closedButton.Update(mState);
            if (closedButton.isClicked)
            {
                _gameStateHandler.ChangeState(new MainMenu(_gameStateHandler, _font, _graphics, _content, _graphicsDevice));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // TODO: Add a background image
            spriteBatch.Begin();
            // Draw the close button
            closedButton.Draw(spriteBatch);

            // HEADING
            string heading = "LEADERBOARD";
            // convert the message into a vector to get the size of the text
            Vector2 headingSize = _font.MeasureString(heading);
            // calculate the position of the text to center it on the screen
            Vector2 headingPosition = new Vector2(
                (_graphics.PreferredBackBufferWidth - headingSize.X) / 2,
                50
            );
            // Draw the heading
            spriteBatch.DrawString(_font, heading, headingPosition, Color.Black);
            // ./HEADING

            // Loop through the top 5 scores
            for (int i = 0; i < leaderBoardScores.Count; i++)
            {
                // Get the current score
                Dictionary<string, float> score = leaderBoardScores[i];
                // Get the name of the player
                string name = (i + 1).ToString() + ". " + score.Keys.First();
                // Get the score of the player
                string playerScore = score.Values.First().ToString();
                // Calculate positions
                Vector2 namePosition = new Vector2(300, 200 + (i * 50));
                Vector2 scorePosition = new Vector2(_graphics.PreferredBackBufferWidth - 300, 200 + (i * 50));

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
            Texture2D pixel = new Texture2D(_graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            float length = Vector2.Distance(start, end);

            spriteBatch.Draw(pixel, start, null, color, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0);
        }
    }
}
