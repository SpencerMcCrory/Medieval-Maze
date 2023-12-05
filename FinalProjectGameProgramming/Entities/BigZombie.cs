using FinalProjectGameProgramming.Entities;
using FinalProjectGameProgramming.Handlers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

internal class BigZombie : Monster
{
    private CollisionHandler collisionHandler;
    private AnimationHandler runAnimation;

    public int Height { get; private set; }
    public int Width { get; private set; }
    public int HitboxTopOffset { get; private set; }
    public int HitboxSideOffset { get; private set; }

    private int hitboxInsetX;
    private int hitboxInsetY;

    public BigZombie(Texture2D[] runFrames, Vector2 position, CollisionHandler collisionHandler, float speed, Vector2 direction)
        : base(runFrames[0], position, speed, direction) // Include speed and direction
    {
        this.collisionHandler = collisionHandler;

        Width = runFrames[0].Width;
        Height = runFrames[0].Height;

        HitboxTopOffset = 20; // Set these based on your game's needs
        HitboxSideOffset = 20;
        hitboxInsetX = 5; // Additional inset for X-axis
        hitboxInsetY = 60; // Additional inset for Y-axis
        runAnimation = new AnimationHandler(runFrames, 0.1);
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
            Direction = -Direction; // Reverse direction
        }
    }

    private Rectangle GetBounds()
    {
        return new Rectangle(
            (int)Position.X + (HitboxSideOffset + hitboxInsetX),
            (int)Position.Y + (HitboxTopOffset + hitboxInsetY),
            Width - 2 * (HitboxSideOffset + hitboxInsetX),
            Height - (HitboxTopOffset +hitboxInsetY));
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(runAnimation.CurrentFrame, Position, Color.White);
    }

    private void DrawLine(SpriteBatch spriteBatch, Texture2D pixel, Vector2 start, Vector2 end, Color color)
    {
        float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
        float length = Vector2.Distance(start, end);

        spriteBatch.Draw(pixel, start, null, color, angle, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
    }

    private void DrawRectangle(SpriteBatch spriteBatch, Texture2D pixel, Rectangle rect, Color color)
    {
        spriteBatch.Draw(pixel, new Rectangle(rect.Left, rect.Top, rect.Width, 1), color);
        spriteBatch.Draw(pixel, new Rectangle(rect.Left, rect.Bottom, rect.Width, 1), color);
        spriteBatch.Draw(pixel, new Rectangle(rect.Left, rect.Top, 1, rect.Height), color);
        spriteBatch.Draw(pixel, new Rectangle(rect.Right, rect.Top, 1, rect.Height), color);
    }
}
