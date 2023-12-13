using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.Handlers
{
    /// <summary>
    /// Handles all animations in the game.
    /// Takes in an array of frames and updates them based on time.
    /// </summary>
    internal class AnimationHandler
    {
        public Texture2D[] Frames { get; set; }
        public double TimePerFrame { get; set; }
        private int currentFrame;
        private double timer;

        public int FrameCount => Frames.Length;

        public AnimationHandler(Texture2D[] frames, double timePerFrame)
        {
            Frames = frames;
            TimePerFrame = timePerFrame;
            currentFrame = 0;
            timer = 0.0;
        }

        public void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;
            if (timer >= TimePerFrame)
            {
                currentFrame = (currentFrame + 1) % Frames.Length;
                timer -= TimePerFrame;
            }
        }

        public Texture2D CurrentFrame => Frames[currentFrame];

    }
}
