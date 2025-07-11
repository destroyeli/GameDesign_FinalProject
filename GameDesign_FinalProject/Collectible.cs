using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDesign_FinalProject
{
    public class Collectible
    {
        public Vector2 Position;
        public bool IsCollected = false;
        private Animation animation;

        public Rectangle BoundingBox => new Rectangle((int)Position.X, (int)Position.Y, 64, 64); 

        public Collectible(Texture2D texture, Vector2 position)
        {
            animation = new Animation(texture, 7, 0.15f); 
            Position = position;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsCollected)
                animation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsCollected)
                animation.Draw(spriteBatch, Position, SpriteEffects.None, 64, 64);
        }
    }
}
