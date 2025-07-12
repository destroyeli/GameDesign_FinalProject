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
        public int CurrentFrame => currentFrame;

        public bool IsDead => isDead;

        private int frameWidth = 660;
        private int frameHeight = 780;

        private int currentFrame = 0;
        private int frameRow = 0; // 0 = walk, 1 = hit, 2 = death
        private double animationTimer = 0;
        private double timePerFrame = 0.15;

        private int life = 2;
        private bool isDead = false;
        private bool isHit = false;
        private bool isMoving = true;


        private Rectangle spriteRectangle; 
        private Rectangle hitboxRectangle; //collision box

        public Rectangle PositionRectangle
        {
            get
            {
                int marginX = 50;
                int marginY = 5;
                return new Rectangle(
                    (int)position.X + marginX,
                    (int)position.Y + marginY,
                    (int)spriteWidth - 2 * marginX,
                    (int)spriteHeight - 2 * marginY
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
            this.spriteImage = root.Content.Load<Texture2D>("Enemy1");

            if (spriteImage == null)
                System.Diagnostics.Debug.WriteLine("Enemy1.png failed to load!");
        }

        public void Update(GameTime gameTime, GamePlatform[] platforms, Hero hero)
        {
            float gravity = 0.5f;
            
            velocity.Y += gravity; // graviti

            if (hero.Position.X < position.X)
            {
                velocity.X = -1.0f;
                isMoving = true;
            }
            else if (hero.Position.X > position.X)
            {
                velocity.X = 1.0f;
                isMoving = true;
            }
            else
            {
                velocity.X = 0f;
                isMoving = false;
            }




            Vector2 nextPosition = position + velocity;
            Rectangle currentBounds = PositionRectangle;
            Rectangle nextBounds = new Rectangle(currentBounds.X + (int)velocity.X, currentBounds.Y + (int)velocity.Y, currentBounds.Width, currentBounds.Height);

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
            if (isDead)
            {
                frameRow = 2; // death
                isMoving = false;
            }
            else if (isHit)
            {
                frameRow = 1; // taking damage
            }
            else if (isMoving)
            {
                frameRow = 0; // walking
            }
            else
            {
                currentFrame = 0; // Idle frame
            }

            if (isMoving || isHit || isDead)
            {
                animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (animationTimer >= timePerFrame)
                {
                    currentFrame++;
                    animationTimer = 0;

                    if (frameRow == 2 && currentFrame > 3)
                    {
                        currentFrame = 3; // stay at last death frame
                    }
                    else if (frameRow == 1 && currentFrame > 3)
                    {
                        currentFrame = 0;
                        isHit = false; 
                    }
                    else if (currentFrame > 3)
                    {
                        currentFrame = 0;
                    }
                }
            }

        }

        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        {
            Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, frameRow * frameHeight, frameWidth, frameHeight);
            Rectangle destRect = new Rectangle((int)position.X, (int)position.Y, (int)spriteWidth, (int)spriteHeight);

            _spriteBatch.Draw(spriteImage, destRect, sourceRect, spriteColor);

            //Texture2D debugTex = new Texture2D(root.GraphicsDevice, 1, 1);
            //debugTex.SetData(new[] { Color.Red });
            //_spriteBatch.Draw(debugTex, PositionRectangle, Color.Red * 0.5f);
        }

        public void TakeDamage()
        {
            life--;
            isHit = true;
            currentFrame = 0;

            if (life <= 0)
            {
                isDead = true;
                currentFrame = 0;
            }
        }

    }
}