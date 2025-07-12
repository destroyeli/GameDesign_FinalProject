using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDesign_FinalProject
{
    public class FinalBoss
    {
        private Animation walkAnim, hitAnim, deathAnim;
        private Animation currentAnim;

        private Texture2D walkTexture, hitTexture, deathTexture;

        private Vector2 position;
        private Vector2 velocity;

        private int screenWidth;
        private int health = 10;

        public bool IsDead => isDead && deathAnim.CurrentFrame == deathAnim.FrameCount - 1;
        private bool isHit = false;
        private bool isDead = false;

        private float hitTimer = 0f;
        private float hitDuration = 0.4f;

        public Rectangle BoundingBox => new Rectangle((int)position.X, (int)position.Y, 300, 200);

        public FinalBoss(Texture2D walkTex, Texture2D hitTex, Texture2D deathTex, Vector2 startPos, int screenWidth)
        {
            walkTexture = walkTex;
            hitTexture = hitTex;
            deathTexture = deathTex;

            walkAnim = new Animation(walkTexture, 6, 0.15f);
            hitAnim = new Animation(hitTexture, 4, 0.1f);
            deathAnim = new Animation(deathTexture, 7, 0.2f, false);

            currentAnim = walkAnim;

            position = startPos;
            velocity = new Vector2(-10f, 0); // Moves left initially
            this.screenWidth = screenWidth;
        }

        public void Update(GameTime gameTime)
        {
            if (isDead)
            {
                currentAnim = deathAnim;
                deathAnim.Update(gameTime);
                return;
            }

            if (isHit)
            {
                hitTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                currentAnim = hitAnim;
                hitAnim.Update(gameTime);

                if (hitTimer <= 0)
                {
                    isHit = false;
                }
            }
            else
            {
                // Move boss and bounce off screen edges
                position += velocity;

                if (position.X < 0 || position.X + 150 > screenWidth)
                {
                    velocity.X *= -1;
                }

                currentAnim = walkAnim;
                walkAnim.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentAnim.Draw(spriteBatch, new Vector2(position.X, position.Y - 100), velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 300, 200);

        }

        public void TakeDamage()
        {
            if (isDead) return;

            health--;

            if (health <= 0)
            {
                isDead = true;
                deathAnim.CurrentFrame = 0;
                deathAnim.Timer = 0f;
            }
            else
            {
                isHit = true;
                hitTimer = hitDuration;
                hitAnim.CurrentFrame = 0;
                hitAnim.Timer = 0f;
            }
        }

        public bool CollidesWith(Rectangle rect) => BoundingBox.Intersects(rect);
    }
}
