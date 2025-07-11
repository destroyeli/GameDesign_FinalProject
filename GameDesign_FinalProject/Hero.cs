using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GameDesign_FinalProject
{
    internal class Hero //hero
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool IsJumping;

        float normalRunInterval = 0.08f;
        float sprintRunInterval = 0.04f;

        bool faceLeft = false;
        bool faceRight = true;

        private bool hasFired = false;

        Animation sprintAnim;

        Animation shootAnim;
        bool isShooting = false;

        Animation idleAnim, runAnim, jumpAnim, fallAnim;
        Animation currentAnim;

        List<Projectile> projectiles = new List<Projectile>();
        Texture2D projectileTexture;

        SpriteEffects flip = SpriteEffects.None;

        public Hero(Texture2D idle, Texture2D run, Texture2D jump, Texture2D fall, Texture2D sprint, Texture2D shoot)
        {
            Position = new Vector2(50, 450); // starting position

            idleAnim = new Animation(idle, 7, 0.15f);
            runAnim = new Animation(run, 7, normalRunInterval);
            jumpAnim = new Animation(jump, 7, 0.12f);
            fallAnim = new Animation(fall, 4, 0.3f);

            sprintAnim = new Animation(sprint, 7, sprintRunInterval); // ← NEW!
            shootAnim = new Animation(shoot, 6, 0.06f);
            currentAnim = idleAnim;


        }


        public void Update(GameTime gameTime, KeyboardState key, GamePlatform[] platforms, MouseState mouse)
        {
            // Detect left mouse click
            if (mouse.LeftButton == ButtonState.Pressed && !isShooting && !IsJumping && !hasFired)
            {
                isShooting = true;
                hasFired = true; // Prevent multiple shots in one click
                shootAnim.CurrentFrame = 0;
                shootAnim.Timer = 0f;
                currentAnim = shootAnim;

                Vector2 bulletPos = new Vector2(Position.X + 50, Position.Y + 30); // adjust bullet start point
                bool direction = faceRight ? true : false;
                projectiles.Clear(); // only one bullet at a time
                projectiles.Add(new Projectile(projectileTexture, bulletPos, direction));

            }
            else if (mouse.LeftButton == ButtonState.Released)
            {
                hasFired = false; // allow firing again
            }

            // If shooting, override all other animations until done
            if (isShooting)
            {
                shootAnim.Update(gameTime);
                currentAnim = shootAnim;

                if (shootAnim.CurrentFrame == shootAnim.FrameCount - 1)
                {
                    isShooting = false;
                }

                return; // Skip the rest of update logic while shooting
            }


            // Sprint modifier and speed logic
            float moveSpeed = 3f;
            bool isSprinting = key.IsKeyDown(Keys.LeftShift) || key.IsKeyDown(Keys.RightShift);

            if (isSprinting)
            {
                moveSpeed *= 2f;
            }



            Velocity.X = 0;

            // Run controls
            // Movement & animation logic
            if (key.IsKeyDown(Keys.D) || key.IsKeyDown(Keys.Right))
            {
                Velocity.X = moveSpeed;
                flip = SpriteEffects.None;

                if (!IsJumping)
                {
                    currentAnim = isSprinting ? sprintAnim : runAnim;
                }

                faceRight = true;
                faceLeft = false;
            }
            else if (key.IsKeyDown(Keys.A) || key.IsKeyDown(Keys.Left))
            {
                Velocity.X = -moveSpeed;
                flip = SpriteEffects.FlipHorizontally;

                if (!IsJumping)
                {
                    currentAnim = isSprinting ? sprintAnim : runAnim;
                }

                faceLeft = true;
                faceRight = false;
            }
            else
            {
                //  No movement key is pressed
                Velocity.X = 0;

                if (!IsJumping)
                {
                    currentAnim = idleAnim;
                }
            }


            // Jump
            if (key.IsKeyDown(Keys.Space) && !IsJumping)
            {
                Velocity.Y = -15f;
                IsJumping = true;
                currentAnim = jumpAnim;
            }

            // Apply gravity
            Velocity.Y += 0.4f;

            // Predict new position
            Vector2 nextPosition = Position + Velocity;
            Rectangle nextBounds = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, 150, 100);
            Rectangle currentBounds = BoundingBox;

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
                    nextPosition.Y = plat.Top - currentBounds.Height;
                    Velocity.Y = 0;
                    IsJumping = false;
                    onPlatform = true;
                }

                // === Bottom Collision (head bumps) ===
                if (currentBounds.Top >= plat.Bottom &&
                    nextBounds.Top <= plat.Bottom &&
                    nextBounds.Right > plat.Left &&
                    nextBounds.Left < plat.Right)
                {
                    nextPosition.Y = plat.Bottom;
                    Velocity.Y = 2f; // push down enough to resume falling
                    currentAnim = fallAnim;
                }


                // === Right-side Collision ===
                if (currentBounds.Right <= plat.Left &&
                    nextBounds.Right >= plat.Left &&
                    nextBounds.Bottom > plat.Top &&
                    nextBounds.Top < plat.Bottom)
                {
                    nextPosition.X = plat.Left - currentBounds.Width;
                    Velocity.X = 0;
                }

                // === Left-side Collision ===
                if (currentBounds.Left >= plat.Right &&
                    nextBounds.Left <= plat.Right &&
                    nextBounds.Bottom > plat.Top &&
                    nextBounds.Top < plat.Bottom)
                {
                    nextPosition.X = plat.Right;
                    Velocity.X = 0;
                }
            }

            // If not on a platform, remain jumping/falling
            if (!onPlatform)
            {
                IsJumping = true;
            }

            // Apply final position
            Position = nextPosition;

            // Switch to fall animation if falling
            if (Velocity.Y > 1f && IsJumping)
            {
                currentAnim = fallAnim;
            }

            foreach (var p in projectiles)
            {
                p.Update();
            }

            currentAnim.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            currentAnim.Draw(spriteBatch, Position, flip, 100, 100); // Resize to 150x100

            foreach (var p in projectiles)
            {
                p.Draw(spriteBatch, gameTime); // Assuming you have a method to draw projectiles
            }
        }

        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, 150, 100); } // match hero draw size
        }


    }
}
