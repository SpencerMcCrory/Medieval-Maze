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

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
