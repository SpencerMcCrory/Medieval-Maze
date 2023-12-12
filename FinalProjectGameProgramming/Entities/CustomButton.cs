using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectGameProgramming.Entities
{
    //custom button builder since we're not using windows form
    public class CustomButton
    {
        Texture2D releasedButtonTexture;
        Texture2D pressedButtonTexture;
        Texture2D currentTexture;
        Vector2 position;
        Rectangle rectangle;
        public string buttonText {  get; private set; }
        SpriteFont font;

        Color color = new Color(255, 255, 255, 255);

        public Vector2 size;

        private bool isPressed;
        private bool wasPressedLastFrame;

        public CustomButton(Texture2D releasedTexture, Texture2D pressedTexture, GraphicsDevice graphicsDevice, SpriteFont newFont, string text)
        {
            releasedButtonTexture = releasedTexture;
            pressedButtonTexture = pressedTexture;
            font = newFont;
            buttonText = text;
            // Set the size of the button
            size = new Vector2(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 8);
            currentTexture = releasedButtonTexture;
        }


        public bool isClicked;

        public void Update(MouseState mouse)
        {
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            Rectangle mouseRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);

            isPressed = mouseRectangle.Intersects(rectangle) && mouse.LeftButton == ButtonState.Pressed;

            if (isPressed)
            {
                currentTexture = pressedButtonTexture;
            }
            else
            {
                if (wasPressedLastFrame && mouseRectangle.Intersects(rectangle))
                {
                    // Mouse was released while over the button - trigger click action
                    isClicked = true;
                }
                currentTexture = releasedButtonTexture;
            }

            wasPressedLastFrame = isPressed;

        }

        public void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public Vector2 GetSize()
        {
            return size;
        }
        public void SetSize(Vector2 newSize)
        {
            size = newSize;
        }
        public void SetText(string newText)
        {
            buttonText = newText;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(currentTexture, rectangle, color);
            Vector2 textSize = font.MeasureString(buttonText);
            Vector2 textPosition = new Vector2(rectangle.X + (rectangle.Width - textSize.X) / 2, rectangle.Y + (rectangle.Height - textSize.Y) / 2);
            spriteBatch.DrawString(font, buttonText, textPosition, Color.Black);
        }
    }
}
