using FinalProjectGameProgramming.Handlers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.Entities
{
    internal class Player
    {

        public Player()
        {
            speed = 300f;
            height = 56;
            width = 32;
            hitboxTopOffset = 40;
            hitboxSideOffset = 10;

        }

        public float speed { get; private set; }

        public int height { get; private set; }
        public int width { get; private set; }
        public int hitboxTopOffset { get; private set; }
        public int hitboxSideOffset { get; private set; }


    }
}
