using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign_FinalProject
{
    internal class GamePlatform
    {
        Texture2D platformTexture;
        Rectangle platformDisplay;
        Rectangle platformSource;
        Color platformColor;

        public GamePlatform(Texture2D platformTexture, Rectangle platormDisplay, Rectangle platformSource, Color platformColor)
        {
            this.platformTexture = platformTexture;
            this.platformDisplay = platormDisplay;
            this.platformSource = platformSource;
            this.platformColor = platformColor;
        }

        public Texture2D PlatformTexture { get => platformTexture; set => platformTexture = value; }
        public Rectangle PlatformDisplay { get => platformDisplay; set => platformDisplay = value; }
        public Rectangle PlatformSource { get => platformSource; set => platformSource = value; }
        public Color PlatformColor { get => platformColor; set => platformColor = value; }
    }
}
