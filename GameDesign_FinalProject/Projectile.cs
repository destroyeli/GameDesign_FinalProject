using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDesign_FinalProject
{
    internal class Projectile : Sprite
    {
        private Vector2 velocity;

        public Projectile(Vector2 position, Vector2 velocity, Texture2D spriteImage) : base(position)
        {
            this.velocity = velocity;
            this.SpriteImage = spriteImage;
            this.SpriteWidth = 64f;
            this.SpriteHeight = 64f;
        }

        public void Update()
        {
            Position += velocity;
        }
    }
}
