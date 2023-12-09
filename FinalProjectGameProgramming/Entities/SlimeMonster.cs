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
    internal class SlimeMonster : Monster
    {


        private CollisionHandler collisionHandler;
        private AnimationHandler runAnimation;

        // Assuming these properties are used for collision detection
        public int Height { get; private set; }
        public int Width { get; private set; }
        public int HitboxTopOffset { get; private set; }
        public int HitboxSideOffset { get; private set; }

        public SlimeMonster(Texture2D[] runFrames, Vector2 position, CollisionHandler collisionHandler, float speed, Vector2 direction)
            : base(runFrames[0], position, speed, direction) // Initial speed and direction
        {
            this.collisionHandler = collisionHandler;

            // Initialize properties based on the texture or specific values
            Width = runFrames[0].Width;
            Height = runFrames[0].Height;
            HitboxTopOffset = 0;
            HitboxSideOffset = 0;
            runAnimation = new AnimationHandler(runFrames, 0.1);
            Name = "Slime";
        }

        public override void Update(GameTime gameTime)
        {
            Move(gameTime);
            CheckForCollisionAndChangeDirection();
            runAnimation.Update(gameTime);
        }

        protected override void Move(GameTime gameTime)
        {
            Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        protected override void CheckForCollisionAndChangeDirection()
        {
            Rectangle bounds = GetBounds();
            if (collisionHandler.CheckCollisionWithEnvironment(bounds))
            {
                // Reverse direction
                Direction = -Direction;
                // When change direction randomly. there is a chance that the monster will get stuck in a wall because of the HitboxSideOffset
                // Direction = ChangeDirectionRandomly();
            }
        }

        private Rectangle GetBounds()
        {
            return new Rectangle(
                (int)Position.X + HitboxSideOffset,
                (int)Position.Y + HitboxTopOffset,
                Width - 2 * HitboxSideOffset,
                Height - HitboxTopOffset);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(runAnimation.CurrentFrame, Position, Color.White);
        }
    }
}
