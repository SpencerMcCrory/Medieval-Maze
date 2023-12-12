using FinalProjectGameProgramming.Handlers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.Entities
{
    abstract class PowerUp
    {

        public bool IsDeleted { get; set; }
        public bool IsActive { get; private set; }
        public Vector2 Position { get; set; }
        public string Type { get; set; }
        private double activationTime;
        private double powerUpDuration { get; set; }
        private double startTime {  get; set; }
        public Texture2D Texture { get; private set; }

        public static int POWERUP_HITBOX_OFFSET = 10;
        public Rectangle Hitbox => new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);


        public PowerUp(Vector2 position, Texture2D texture)
        {
            Position = position;
            Texture = texture;
            activationTime = 0;
            IsDeleted = false;
            IsActive = false;
        }
        public virtual void Collect(Player player)
        {
            // Change the power-up's state to inactive
            IsDeleted = true;
            IsActive = true;
            activationTime = 0;
        }
        public void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                activationTime += gameTime.ElapsedGameTime.TotalSeconds;

                if (activationTime >= powerUpDuration)
                {
                    Deactivate();
                    activationTime = 0;
                    powerUpDuration = 0;
                }
            }
        }

        public void SetDurationTimer(double time)
        {
            powerUpDuration = time + 15;
        }
        private void Deactivate()
        {
            IsActive = false;
            // Reverse the power-up effect from the player if necessary
        }



    }
}
