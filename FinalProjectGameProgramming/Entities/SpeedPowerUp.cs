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
    internal class SpeedPowerUp : PowerUp
    {
        public SpeedPowerUp(Vector2 position, Texture2D texture, string type) : base(position, texture)
        {
            Type = type;
        }
        

        public override void Collect(Player player)
        {

            // Add specific behavior for speed power-up
            player.Speed = 500; // Example: increase player's speed
                                 // You can also trigger any animations or sound effects here

            // If you want the texture to change, assign a new texture to Texture property
            // Assuming you have a texture for the collected state
            // Texture = CollectedTexture;
            base.Collect(player);
        }
    }
}
