using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FinalProjectGameProgramming.Entities
{
    abstract class Monster
    {
        public Vector2 Position { get; protected set; }
        public Texture2D Texture { get; protected set; }

        public float Speed { get; protected set; }
        public Vector2 Direction { get; protected set; }

        protected Monster(Texture2D texture, Vector2 position, float initialSpeed, Vector2 initialDirection)
        {
            Texture = texture;
            Position = position;
            Speed = initialSpeed; // Set initial speed
            Direction = initialDirection; // Set initial direction
        }

        public virtual void Update(GameTime gameTime)
        {
            Move(gameTime);
            CheckForCollisionAndChangeDirection();
        }

        protected virtual void Move(GameTime gameTime)
        {
            // Update position based on speed and direction
            Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        protected abstract void CheckForCollisionAndChangeDirection();
         
        /**
         * Changes the direction of the monster randomly
         */
        public Vector2 ChangeDirectionRandomly()
        {
            Random random = new Random();
            int randomDirection = random.Next(0, 4); // Generates a random number between 0 and 3

            // Assign new direction based on the random number
            switch (randomDirection)
            {
                case 0:
                    return new Vector2(1, 0); // Right
                case 1:
                    return new Vector2(-1, 0); // Left
                case 2:
                    return new Vector2(0, 1); // Down
                case 3:
                    return new Vector2(0, -1); // Up
                default:
                    return new Vector2(0, -1); // No movement
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
