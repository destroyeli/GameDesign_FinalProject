using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace GameDesign_FinalProject
{
    internal class Sprite
    {
        private Vector2 position;
        private Texture2D spriteImage;
        private float spriteWidth;
        private float spriteHeight;

        public Sprite(Vector2 position)
        {
            this.position = position;
        } 
        public Vector2 Position { get => position; set => position = value; }
        public Texture2D SpriteImage { get => spriteImage; set => spriteImage = value; }
        public float SpriteWidth { get => spriteWidth; set => spriteWidth = value; }
        public float SpriteHeight { get => spriteHeight; set => spriteHeight = value; }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteImage, PositionRectangle, Color.White);
        }

        public Rectangle PositionRectangle
        {
            get { return new Rectangle((int)position.X, (int)position.Y, (int)spriteWidth, (int)spriteHeight); }
        }

    }
}
