using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameDesign_FinalProject
{
    internal class Hero
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool IsJumping;

        float normalRunInterval = 0.08f;
        float sprintRunInterval = 0.04f;

        bool faceLeft = false;
        bool faceRight = true;

        Animation idleAnim, runAnim, jumpAnim, fallAnim;
        Animation currentAnim;

        SpriteEffects flip = SpriteEffects.None;

        public Hero(Texture2D idle, Texture2D run, Texture2D jump, Texture2D fall)
        {
            Position = new Vector2(100, 300); // starting position

            idleAnim = new Animation(idle, 8, 0.15f);  // 8 idle frames
            runAnim = new Animation(run, 7, 0.08f);   // example: 6 run frames
            jumpAnim = new Animation(jump, 3, 0.12f);  // example: 4 jump frames
            fallAnim = new Animation(fall, 4, 0.3f);   // example: 2 fall frames

            currentAnim = idleAnim;
        }

        public void Update(GameTime gameTime, KeyboardState key, GamePlatform[] platforms)
        {
            // Sprint modifier and speed logic
            float moveSpeed = 3f;
            bool isSprinting = key.IsKeyDown(Keys.LeftShift) || key.IsKeyDown(Keys.RightShift);

            if (isSprinting)
            {
                moveSpeed *= 2f; // Sprint speed
                runAnim.Interval = sprintRunInterval; // faster run animation
            }
            else
            {
                runAnim.Interval = normalRunInterval; // normal speed
            }


            Velocity.X = 0;

            // Run controls
            if (key.IsKeyDown(Keys.D) || key.IsKeyDown(Keys.Right))
            {
                Velocity.X = moveSpeed;
                flip = SpriteEffects.None;

                if (!IsJumping)
                    currentAnim = runAnim;

                faceRight = true;
                faceLeft = false;
            }
            else if (key.IsKeyDown(Keys.A) || key.IsKeyDown(Keys.Left))
            {
                Velocity.X = -moveSpeed;
                flip = SpriteEffects.FlipHorizontally;

                if (!IsJumping)
                    currentAnim = runAnim;

                faceLeft = true;
                faceRight = false;
            }
            else
            {
                if (!IsJumping)
                    currentAnim = idleAnim;
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

            currentAnim.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentAnim.Draw(spriteBatch, Position, flip, 150, 100); // Resize to 150x100
        }

        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, 150, 100); } // match hero draw size
        }


    }
}
