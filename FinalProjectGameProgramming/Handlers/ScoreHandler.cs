using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.Handlers
{
    /// <summary>
    /// Score handler handles the score of the game
    /// </summary>
    internal class ScoreHandler
    {
        public ScoreHandler(int score)
        {
            CurrentScore = score;
        }
        public int CurrentScore { get; private set; } = 0!;

        public void AddScore(int amount)
        {
            CurrentScore += amount;
        }

        /// <summary>
        /// calculates the score based on the time
        /// The score is 600 - time
        /// time is in seconds
        /// if the time is less than 0, the score is 0
        /// </summary>
        /// <param name="time"></param>
        public void CalculateTimeScore(int time)
        {
            int timeScore = 600 - time;
            timeScore = timeScore < 0 ? 0 : timeScore;
            AddScore(timeScore);
        }

        public int GetScore() => CurrentScore;

    }
}
