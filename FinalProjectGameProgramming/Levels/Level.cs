using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.Levels
{
    /*abstract class for building a level*/
    public abstract class Level
    {
        public abstract void Initialize();
        public abstract void UpdateLevel(GameTime gameTime);
        public abstract void DrawLevel(SpriteBatch spriteBatch);
        public abstract void UnloadContent();

        public abstract void LoadLevelContent();
    }
}
