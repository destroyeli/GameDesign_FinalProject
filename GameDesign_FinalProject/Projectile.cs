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

        private Animation animation;

        public Vector2 Position { get; set; }

        public Projectile(Texture2D spriteSheet, Vector2 position, bool goingRight) : base(position)
        {
            this.goingRight = goingRight;
            this.Position = position;

            animation = new Animation(spriteSheet, 4, 0.1f); 
        }

        public void Update(GameTime gameTime)
        {
            Position = new Vector2(Position.X + (goingRight ? velocity : -velocity), Position.Y);

            animation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            SpriteEffects flip = goingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            animation.Draw(spriteBatch, Position, flip, 40, 40);
        }

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, 40, 40);
            }
        }
    }
}


