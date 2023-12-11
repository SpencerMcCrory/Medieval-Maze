using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.Handlers
{
    public static class SFXHandler
    {
        public static SoundEffect RelicOpenSound { get; private set; }
        public static void LoadContent(ContentManager content)
        {
            RelicOpenSound = content.Load<SoundEffect>("relic_open_sfx");


        }
    }
}
