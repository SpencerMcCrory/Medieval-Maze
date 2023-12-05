using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.Handlers
{
    internal class CollisionHandler
    {
        private int tileSize;
        private LevelHandler currentLevel;

        public CollisionHandler(LevelHandler currentLevel, int tileSize)
        {
            this.currentLevel = currentLevel;
            this.tileSize = tileSize;
        }

        public bool CheckCollisionWithEnvironment(Rectangle entityBounds)
        {
            // Check each corner of the entity's bounding box for collision
            foreach (Vector2 corner in new Vector2[]
            {
            new Vector2(entityBounds.Left, entityBounds.Top),
            new Vector2(entityBounds.Right, entityBounds.Top),
            new Vector2(entityBounds.Left, entityBounds.Bottom),
            new Vector2(entityBounds.Right, entityBounds.Bottom)
            })
            {
                int gridX = (int)corner.X / tileSize;
                int gridY = (int)corner.Y / tileSize;

                if (currentLevel.Grid[gridY, gridX] == 1) 
                {
                    return true; // Collision detected
                }
            }
            return false; // No collision
        }
    }
}
