using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameDesign_FinalProject
{
    public class MainMenu
    {
        Texture2D titleTexture, playBtn, loadBtn, exitBtn;
        Rectangle titleRect, playRect, loadRect, exitRect;

        public bool PlayClicked, LoadClicked, ExitClicked;

        public MainMenu(Texture2D title, Texture2D play, Texture2D load, Texture2D exit, int screenWidth)
        {
            titleTexture = title;
            playBtn = play;
            loadBtn = load;
            exitBtn = exit;

            int centerX = screenWidth / 2;

            titleRect = new Rectangle(0, 0, 1280, 768);
            playRect = new Rectangle(170, 550, 200, 70);
            loadRect = new Rectangle(540, 550, 200, 70);
            exitRect = new Rectangle(910, 550, 200, 70);
        }

        public void Update(MouseState mouse)
        {
            Point mousePoint = mouse.Position;

            PlayClicked = mouse.LeftButton == ButtonState.Pressed && playRect.Contains(mousePoint);
            LoadClicked = mouse.LeftButton == ButtonState.Pressed && loadRect.Contains(mousePoint);
            ExitClicked = mouse.LeftButton == ButtonState.Pressed && exitRect.Contains(mousePoint);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(titleTexture, titleRect, Color.White);
            spriteBatch.Draw(playBtn, playRect, Color.White);
            spriteBatch.Draw(loadBtn, loadRect, Color.White);
            spriteBatch.Draw(exitBtn, exitRect, Color.White);
        }
    }
}

