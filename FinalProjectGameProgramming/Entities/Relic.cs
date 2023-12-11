using FinalProjectGameProgramming.Handlers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.Entities
{
    internal class Relic
    {
        public int Score { get; set; }
        private bool isOpen { get; set; }
        public bool IsDeleted { get; set; }
        public Vector2 Position { get; set; }

        public static int RELIC_HITBOX_OFFSET = 10;

        private AnimationHandler animationHandler;
        public Relic(Vector2 position, Texture2D[] frames, double timePerFrame)
        {
            Random rand = new Random();
            Score = rand.Next(100);
            Position = position;
            this.animationHandler = new AnimationHandler(frames, timePerFrame);
            isOpen = false;
            IsDeleted = false;

        }

        public void Update(GameTime gameTime)
        {
            if (!isOpen)
            {
                if(IsDeleted) { 
                animationHandler.Update(gameTime);
                if (animationHandler.CurrentFrame == animationHandler.Frames.Last())
                {
                    isOpen = true; // Stop the animation on the last frame
                }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(animationHandler.CurrentFrame, Position, Color.White);
        }

    }
}
