using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Windows.Forms;

namespace GameDesign_FinalProject
{
    internal class Enemy
    {
        private Game1 root;
        private Vector2 position;
        private Texture2D spriteImage;
        private float spriteWidth;
        private Vector2 velocity;
        private Color spriteColor;

        public float spriteHeight
        {
            get
            {
                float scale = spriteWidth / spriteImage.Width;
                return spriteImage.Height * scale;
            }
        }

        public Rectangle PositionRectangle
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, (int)spriteWidth, (int)spriteHeight);
            }
        }

        public Enemy (Game1 root, Vector2 position)
        {
            this.root = root;
            this.position = position;
            this.spriteWidth = 150f;
            this.velocity = new Vector2(-1.0f,5.0f);
            this.spriteColor = Color.White;

            LoadContent();
        }

        public void LoadContent()
        {
            this.spriteImage = root.Content.Load<Texture2D>("1");
        }

        public void Update(GameTime gameTime)
        {
            position += velocity;

            if (position.Y < 0 || position.Y > (root.ScreenHeight - spriteHeight))
            {
                velocity.Y *= 0;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(spriteImage, PositionRectangle, spriteColor);
        }
    }
}
