using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.Handlers
{
    internal class AnimationHandler
    {
        public Texture2D[] Frames { get; set; }
        public double TimePerFrame { get; set; }

        public int FrameCount => Frames.Length;

        public AnimationHandler(Texture2D[] frames, double timePerFrame)
        {
            Frames = frames;
            TimePerFrame = timePerFrame;
        }
    }
}
