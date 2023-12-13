using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.Handlers
{
    /// <summary>
    /// Handles all music in the game.
    /// </summary>
    public static class MusicHandler
    {
        public static Song MainMenuMusic { get; private set; }
        public static Song Level1Music { get; private set; }
        public static Song Level2Music { get; private set; }

        /// <summary>
        /// Loads all music in the game.
        /// </summary>
        /// <param name="content"></param>
        public static void LoadContent(ContentManager content)
        {
            MainMenuMusic = content.Load<Song>("KevinMacLeod-8bit Dungeon Boss-Level1Music");
            Level1Music = content.Load<Song>("Joshua McLean - Mountain Trials-MainMenu");
            Level2Music = content.Load<Song>("Density-Time-MAZE");

        }
    }
}
