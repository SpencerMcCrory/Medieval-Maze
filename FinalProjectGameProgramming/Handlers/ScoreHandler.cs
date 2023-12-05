using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.Handlers
{
    internal class ScoreHandler
    {
        public int CurrentScore { get; private set; } = 0!;

        public void AddScore(int amount)
        {
            CurrentScore += amount;
        }
    }
}
