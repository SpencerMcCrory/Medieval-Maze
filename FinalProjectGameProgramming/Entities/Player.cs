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
            Speed = 300f;
            height = 56;
            width = 32;
            hitboxTopOffset = 40;
            hitboxSideOffset = 10;
            position = new Vector2(0, 0);

        }
        public bool IsFacingRight { get; set; } = true;

        public float Speed { get; set; }

        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public int height { get; private set; }
        public int width { get; private set; }
        public int hitboxTopOffset { get; private set; }
        public int hitboxSideOffset { get; private set; }

        public Rectangle EnvironmentHitbox
        {
            get
            {
                return new Rectangle(
                    (int)Position.X + hitboxSideOffset,
                    (int)Position.Y + hitboxTopOffset,
                    width - 2 * hitboxSideOffset,
                    height - hitboxTopOffset);
            }
        }

        public Rectangle MonsterHitbox
        {
            get
            {
                // This could be the full size of the player or a different calculation
                return new Rectangle(
                    (int)Position.X + 14,
                    (int)Position.Y +27,
                    width - 2 * 10,
                    height - 30);
            }
        }

        public Rectangle SFXHitbox
        {
            get
            {
                // This could be the full size of the player or a different calculation
                return new Rectangle(
                    (int)Position.X ,
                    (int)Position.Y ,
                    width + 2 * 200,
                    height + 60 * 2);
            }
        }
    }
}
