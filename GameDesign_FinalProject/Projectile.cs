using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;

namespace GameDesign_FinalProject
{
    internal class Projectile : Sprite
    {
        private float velocity = 10f;
        private bool goingRight;
        private Animation anim;

        public Projectile(Texture2D spriteImage, Vector2 position, bool goingRight) : base(position)
        {

            this.SpriteImage = spriteImage;
            this.SpriteWidth = 64f;
            this.SpriteHeight = 64f;
            this.goingRight = goingRight;

            anim = new Animation(spriteImage, 4, 0.1f, true); // Assuming a single frame for the projectile
        }

        public void Update(GameTime gameTime)
        {
            Position = new Vector2(Position.X + (goingRight ? velocity : -velocity), Position.Y);

            anim.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            SpriteEffects flip = goingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            // ✅ Use base.Draw with flipping support
            spriteBatch.Draw(SpriteImage, PositionRectangle, null, Color.White, 0f, Vector2.Zero, flip, 0f);
        }

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y,(int)SpriteWidth, (int)SpriteHeight); // Adjust size accordingly
            }
        }
    }
}
