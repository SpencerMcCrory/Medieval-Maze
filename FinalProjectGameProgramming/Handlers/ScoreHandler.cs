using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.Handlers
{
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

        public void CalculateTimeScore(int time)
        {
            int timeScore = 600 - time;
            AddScore(timeScore);
        }

        public int GetScore() => CurrentScore;

    }
}
