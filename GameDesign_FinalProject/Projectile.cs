using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDesign_FinalProject
{
    internal class Projectile : Sprite
    {
        private float velocity = 10f;
        private bool goingRight;

        public Projectile(Texture2D spriteImage, Vector2 position, bool goingRight) : base(position)
        {

            this.SpriteImage = spriteImage;
            this.SpriteWidth = 64f;
            this.SpriteHeight = 64f;
            this.goingRight = goingRight;
        }

        public void Update()
        {
            Vector2 pos = Position;
            pos.X += goingRight ? velocity : -velocity;
            Position = pos;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            this.Draw(gameTime, spriteBatch);
        }

    }
}
