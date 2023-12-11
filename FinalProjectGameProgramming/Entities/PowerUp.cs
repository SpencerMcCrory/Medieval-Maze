using FinalProjectGameProgramming.Handlers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.Entities
{
    internal class PowerUp
    {

        public bool IsDeleted { get; set; }
        public Vector2 Position { get; set; }

        public static int POWERUP_HITBOX_OFFSET = 10;


        public PowerUp(Vector2 position)
        {
            Position = position;

        }
    }
}
