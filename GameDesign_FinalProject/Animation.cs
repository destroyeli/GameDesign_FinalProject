using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDesign_FinalProject
{
    public class Animation
    {
        public Texture2D Texture;
        public int FrameCount;
        public int CurrentFrame;
        public float Timer;
        public float Interval;
        public int FrameWidth;
        public int FrameHeight;
        public bool Loop; // 👈 NEW field to control looping

        public Animation(Texture2D texture, int frameCount, float interval, bool loop = true)
        {
            Texture = texture;
            FrameCount = frameCount;
            Interval = interval;
            Loop = loop;
            CurrentFrame = 0;
            Timer = 0f;
            FrameWidth = texture.Width / frameCount;
            FrameHeight = texture.Height;
        }

        public void Update(GameTime gameTime)
        {
            Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Timer >= Interval)
            {
                CurrentFrame++;
                Timer = 0f;

                if (CurrentFrame >= FrameCount)
                {
                    if (Loop)
                        CurrentFrame = 0;
                    else
                        CurrentFrame = FrameCount - 1; // Freeze on last frame if not looping
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects flip, int width = 0, int height = 0)
        {
            Rectangle sourceRect = new Rectangle(CurrentFrame * FrameWidth, 0, FrameWidth, FrameHeight);

            int drawWidth = (width > 0) ? width : FrameWidth;
            int drawHeight = (height > 0) ? height : FrameHeight;

            Rectangle destRect = new Rectangle(position.ToPoint(), new Point(drawWidth, drawHeight));

            spriteBatch.Draw(Texture, destRect, sourceRect, Color.White, 0f, Vector2.Zero, flip, 0f);
        }
    }
}
