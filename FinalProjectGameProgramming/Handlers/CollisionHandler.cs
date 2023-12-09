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
				}
			)
			{
				// Divide the corner's X and Y by the tile size to get the grid position
				// of the corner. If the grid position is 1, then there is a collision.
				// If the grid position is 0, then there is no collision.
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
