using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace GameDesign_FinalProject
{
    internal class Hero //hero
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool IsJumping;
        public int Health { get; private set; } = 3;
        public List<Projectile> Projectiles => projectiles;

        int lastShotFrame = -1; // To prevent multiple bullets in the same frame

        bool isHit = false;
        float hitTimer = 0f;
        float hitDuration = 0.6f;

        bool isInvincible = false;
        float invincibilityTimer = 0f;
        float invincibilityDuration = 2f;

        bool isVisible = true;
        float blinkTimer = 0f;
        float blinkInterval = 0.1f;

        float normalRunInterval = 0.08f;
        float sprintRunInterval = 0.04f;

        bool faceLeft = false;
        bool faceRight = true;

        private MouseState previousMouse;
        private bool hasShot = false;



        bool canShoot = true;
       
        

        Animation idleAnim, runAnim, jumpAnim, fallAnim;
        Animation sprintAnim, shootAnim, hitAnim, deathAnim;
        Animation currentAnim;
        private List<Projectile> projectiles = new List<Projectile>();
        private Texture2D projectileTexture;

        bool isShooting = false;
        bool isDead = false;
        public bool DeathComplete { get; private set; } = false;

        SpriteEffects flip = SpriteEffects.None;

       
        public Hero(Texture2D idle, Texture2D run, Texture2D jump, Texture2D fall,
                    Texture2D sprint, Texture2D shoot, Texture2D hit, Texture2D death, Texture2D projectileTex)
        {
            Position = new Vector2(50, 450);

            Position = new Vector2(50, 450);
            idleAnim = new Animation(idle, 7, 0.15f);
            runAnim = new Animation(run, 7, normalRunInterval);
            jumpAnim = new Animation(jump, 7, 0.12f);
            fallAnim = new Animation(fall, 4, 0.3f);
            sprintAnim = new Animation(sprint, 7, sprintRunInterval);
            shootAnim = new Animation(shoot, 6, 0.06f);
            hitAnim = new Animation(hit, 5, 0.12f);
            deathAnim = new Animation(death, 7, 0.15f, false); // No loop

            sprintAnim = new Animation(sprint, 7, sprintRunInterval);
            shootAnim = new Animation(shoot, 6, 0.06f);
            hitAnim = new Animation(hit, 5, 0.12f);
            deathAnim = new Animation(death, 7, 0.15f, false); // No loop

            currentAnim = idleAnim;
            Health = 3;
            Health = 3;

            this.projectileTexture = projectileTex;
        }

        

        public void Update(GameTime gameTime, KeyboardState key, GamePlatform[] platforms, MouseState mouse)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

           

            

            if (isInvincible)
            {
                invincibilityTimer -= dt;
                blinkTimer -= dt;

                if (blinkTimer <= 0)
                {
                    isVisible = !isVisible;
                    blinkTimer = blinkInterval;
                }

                if (invincibilityTimer <= 0)
                {
                    isInvincible = false;
                    isVisible = true;
                }
            }

            if (isDead)
            {
                deathAnim.Update(gameTime);
                currentAnim = deathAnim;

                if (deathAnim.CurrentFrame == deathAnim.FrameCount - 1)
                {
                    DeathComplete = true;
                }
                return;
            }

            if (isHit)
            {
                hitTimer -= dt;
                hitAnim.Update(gameTime);
                currentAnim = hitAnim;

                if (hitTimer <= 0)
                {
                    isHit = false;
                }
                return;
            }

            if (isInvincible)
            {
                invincibilityTimer -= dt;
                blinkTimer -= dt;

                if (blinkTimer <= 0)
                {
                    isVisible = !isVisible;
                    blinkTimer = blinkInterval;
                }

                if (invincibilityTimer <= 0)
                {
                    isInvincible = false;
                    isVisible = true;
                }
            }

            bool mouseClicked = mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released;

            if (isShooting)
            {
                currentAnim = shootAnim;
                shootAnim.Update(gameTime);

                if (!hasShot && shootAnim.CurrentFrame == 0)
                {
                    Vector2 bulletPos = new Vector2(Position.X + (faceRight ? 90 : -10), Position.Y + 30);
                    projectiles.Add(new Projectile(projectileTexture, bulletPos, faceRight));
                    hasShot = true;
                }

                if (shootAnim.CurrentFrame == shootAnim.FrameCount - 1)
                {
                    isShooting = false;
                    hasShot = false;
                }
            }
            else if (mouseClicked && !IsJumping)
            {
                isShooting = true;
                shootAnim.CurrentFrame = 0;
                shootAnim.Timer = 0f;
                hasShot = false;
            }



            float moveSpeed = 3f;
            bool isSprinting = key.IsKeyDown(Keys.LeftShift) || key.IsKeyDown(Keys.RightShift);
            if (isSprinting)
                moveSpeed *= 2f;
            Velocity.X = 0;



            if (key.IsKeyDown(Keys.D) || key.IsKeyDown(Keys.Right))
            {
                Velocity.X = moveSpeed;
                flip = SpriteEffects.None;
                faceRight = true;
                faceLeft = false;
            }
            else if (key.IsKeyDown(Keys.A) || key.IsKeyDown(Keys.Left))
            {
                Velocity.X = -moveSpeed;
                flip = SpriteEffects.FlipHorizontally;
                faceLeft = true;
                faceRight = false;
            }

            // ↓ This block sets animations only if not shooting
            if (!isShooting)
            {
                if (Velocity.X > 0 || Velocity.X < 0)
                {
                    currentAnim = isSprinting ? sprintAnim : runAnim;
                }
                else if (!IsJumping)
                {
                    currentAnim = idleAnim;
                }
            }


            if (key.IsKeyDown(Keys.Space) && !IsJumping)
            {
                Velocity.Y = -15f;
                IsJumping = true;
                currentAnim = jumpAnim;
            }

            Velocity.Y += 0.4f;

            Vector2 nextPosition = Position + Velocity;
            Rectangle nextBounds = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, 150, 100);
            Rectangle currentBounds = BoundingBox;

            bool onPlatform = false;

            foreach (GamePlatform p in platforms)
            {
                if (p == null) continue;
                Rectangle plat = p.PlatformDisplay;

                if (currentBounds.Bottom <= plat.Top &&
                    nextBounds.Bottom >= plat.Top &&
                    nextBounds.Right > plat.Left &&
                    nextBounds.Left < plat.Right)
                {
                    nextPosition.Y = plat.Top - currentBounds.Height;
                    Velocity.Y = 0;
                    IsJumping = false;
                    onPlatform = true;
                }

                if (currentBounds.Top >= plat.Bottom &&
                    nextBounds.Top <= plat.Bottom &&
                    nextBounds.Right > plat.Left &&
                    nextBounds.Left < plat.Right)
                {
                    nextPosition.Y = plat.Bottom;
                    Velocity.Y = 2f;
                    currentAnim = fallAnim;
                }

                if (currentBounds.Right <= plat.Left &&
                    nextBounds.Right >= plat.Left &&
                    nextBounds.Bottom > plat.Top &&
                    nextBounds.Top < plat.Bottom)
                {
                    nextPosition.X = plat.Left - currentBounds.Width;
                    Velocity.X = 0;
                }

                if (currentBounds.Left >= plat.Right &&
                    nextBounds.Left <= plat.Right &&
                    nextBounds.Bottom > plat.Top &&
                    nextBounds.Top < plat.Bottom)
                {
                    nextPosition.X = plat.Right;
                    Velocity.X = 0;
                }
            }

            if (!onPlatform)
                IsJumping = true;

            Position = nextPosition;

            if (Velocity.Y > 1f && IsJumping)
                currentAnim = fallAnim;

            foreach (var p in projectiles)
            {
                p.Update(gameTime);
            }
                projectiles.RemoveAll(p => p.Position.X < -50 || p.Position.X > 1400);

            previousMouse = mouse;


            currentAnim.Update(gameTime);
        }

        
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (isVisible)
                currentAnim.Draw(spriteBatch, Position, flip, 100, 100);

            foreach (var p in projectiles)
            {
                p.Draw(spriteBatch, gameTime);
            }
        }



        public Rectangle BoundingBox => new Rectangle((int)Position.X, (int)Position.Y, 150, 100);

        public void CheckEnemyCollision(List<Enemy> enemies)
        {
            if (isHit || isInvincible || isDead) return;

            foreach (var enemy in enemies)
            {
                if (BoundingBox.Intersects(enemy.PositionRectangle))
                {
                    Health--; // Reduce health by 1

                    if (Health <= 0)
                    {
                        // Trigger death animation
                        isDead = true;
                        deathAnim.CurrentFrame = 0;
                        deathAnim.Timer = 0f;
                    }
                    else
                    {
                        // Trigger hit animation and invincibility
                        isHit = true;
                        isInvincible = true;
                        hitTimer = hitDuration;
                        invincibilityTimer = invincibilityDuration;
                        blinkTimer = blinkInterval;
                        hitAnim.CurrentFrame = 0;
                        hitAnim.Timer = 0f;
                    }

                    break; // Only one collision counted
                }
            }
        }

    }
}
