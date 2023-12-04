using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.Handlers
{
    public class CameraHandler
    {
        public System.Numerics.Vector2 Position { get; set; }
        public float Zoom { get; set; }

        public CameraHandler()
        {
            Position = System.Numerics.Vector2.Zero;
            Zoom = 1;
        }



        public Matrix GetViewMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(-Position, 0)) *
                   Matrix.CreateScale(Zoom, Zoom, 1);
        }


    }
}
