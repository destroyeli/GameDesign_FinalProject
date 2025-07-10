using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign_FinalProject
{
    internal class GameBackground
    {
        Texture2D backgroundTexture;
        Rectangle backgroundDisplay;
        Rectangle backgroundSource;
        Color backgroundColor;

        public GameBackground(Texture2D backgroundTexture, Rectangle backgroundDisplay, Rectangle backgroundSource, Color backgroundColor)
        {
            this.backgroundTexture = backgroundTexture;
            this.backgroundDisplay = backgroundDisplay;
            this.backgroundSource = backgroundSource;
            this.backgroundColor = backgroundColor;
        }

        public Texture2D BackgroundTexture { get => backgroundTexture; set => backgroundTexture = value; }
        public Rectangle BackgroundDisplay { get => backgroundDisplay; set => backgroundDisplay = value; }
        public Rectangle BackgroundSource { get => backgroundSource; set => backgroundSource = value; }
        public Color BackgroundColor { get => backgroundColor; set => backgroundColor = value; }
    }
}
