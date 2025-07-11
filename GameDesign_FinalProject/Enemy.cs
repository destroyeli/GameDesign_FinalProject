using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Windows.Forms;

namespace GameDesign_FinalProject
{
    internal class Enemy
    {
        private Game1 root;
        protected Vector2 position;
        protected Texture2D spriteImage;
        private float spriteWidth;
        private Vector2 velocity;
        private Color spriteColor;
        private float spriteHeight;

public Rectangle PositionRectangle
{
    get
    {
        int shrinkAmount = 20; // shrink collision box by 20 pixels total (10 on each side)
        return new Rectangle(
            (int)position.X + shrinkAmount / 2,
            (int)position.Y + shrinkAmount / 2,
            (int)spriteWidth - shrinkAmount,
            (int)spriteHeight - shrinkAmount
        );
    }
}

        public Enemy(Game1 root, Vector2 position)
        {
            this.root = root;
            this.position = position;
            this.spriteWidth = 150f;
            this.spriteHeight = 100f;
            this.velocity = new Vector2(0.0f, 0.0f);
            this.spriteColor = Color.White;

            LoadContent();
        }

        public void LoadContent()
        {
            this.spriteImage = root.Content.Load<Texture2D>("1");
        }

        public void Update(GameTime gameTime, GamePlatform[] platforms, Hero hero)
        {
            float gravity = 0.5f;
            
            velocity.Y += gravity; // Apply gravity

            if (hero.Position.X < position.X)
                velocity.X = -1.0f;
            else
                velocity.X = 1.0f;

            

            Vector2 nextPosition = position + velocity;
            Rectangle nextBounds = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, (int)spriteWidth, (int)spriteHeight);
            Rectangle currentBounds = PositionRectangle;

            bool onPlatform = false;

            foreach (GamePlatform p in platforms)
            {
                if (p == null) continue;
                Rectangle plat = p.PlatformDisplay;

                // === Top Collision (landing on platform) ===
                if (currentBounds.Bottom <= plat.Top &&
                    nextBounds.Bottom >= plat.Top &&
                    nextBounds.Right > plat.Left &&
                    nextBounds.Left < plat.Right)
                {
                    nextPosition.Y = plat.Top - spriteHeight;
                    velocity.Y = 0;
                    onPlatform = true;
                }

                // === Bottom Collision (head bumps) ===
                if (currentBounds.Top >= plat.Bottom &&
                    nextBounds.Top <= plat.Bottom &&
                    nextBounds.Right > plat.Left &&
                    nextBounds.Left < plat.Right)
                {
                    nextPosition.Y = plat.Bottom;
                    velocity.Y = 0; 
                }


                // === Right-side Collision ===
                if (currentBounds.Right <= plat.Left &&
                    nextBounds.Right >= plat.Left &&
                    nextBounds.Bottom > plat.Top &&
                    nextBounds.Top < plat.Bottom)
                {
                    nextPosition.X = plat.Left - (int)spriteWidth;
                    velocity.X = 0;
                }

                // === Left-side Collision ===
                if (currentBounds.Left >= plat.Right &&
                    nextBounds.Left <= plat.Right &&
                    nextBounds.Bottom > plat.Top &&
                    nextBounds.Top < plat.Bottom)
                {
                    nextPosition.X = plat.Right;
                    velocity.X = 0;
                }
            }

            // Apply final position
            position = nextPosition;
            
        }

        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(spriteImage, PositionRectangle, spriteColor);
        }
    }
}