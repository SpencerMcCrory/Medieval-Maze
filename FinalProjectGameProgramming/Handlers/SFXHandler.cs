using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.Handlers
{
    /// <summary>
    /// Handles all sound effects in the game.
    /// </summary>
    public static class SFXHandler
    {
        public static SoundEffect RelicOpenSound { get; private set; }
        public static SoundEffect PlayerDyingSound { get; private set; }
        public static SoundEffect SpeedPotion { get; private set; }
        public static SoundEffect SlimeNoise { get; private set; }
        public static SoundEffect BigZombieNoise { get; private set; }

        /// <summary>
        /// Loads all sound effects in the game.
        /// </summary>
        /// <param name="content"></param>
        public static void LoadContent(ContentManager content)
        {
            RelicOpenSound = content.Load<SoundEffect>("relic_open_sfx");
            PlayerDyingSound = content.Load<SoundEffect>("guy_screaming");
            SpeedPotion = content.Load<SoundEffect>("speedPowerUp");
            SlimeNoise = content.Load<SoundEffect>("slimeSound");
            BigZombieNoise = content.Load<SoundEffect>("bigZombieSnarl");



        }
    }
}
