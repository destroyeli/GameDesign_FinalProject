using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GameDesign_FinalProject
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Hero hero;
        Texture2D heroIdle, heroRun, heroJump, heroFall;

        Enemy enemy;

        GamePlatform[] platform;
        Texture2D platformTexture;
        Rectangle platformDisplay, platformSource;
        Color platformColor;

        int spriteWidth = 64, 
            spriteHeight = 64;

        string _sceneLayout = "                    " +
                              "                    " +
                              "                ^^^^" +
                              "^^^^^^           ---" +
                              "-----   ^^^         " +
                              "                    " +
                              "              ^^^^^ " +
                              "               ---- " +
                              "                --  " +
                              "^^^^^^              " +
                              "------------        " +
                              "--------------------";
        private int screenWidth = 1280;

        public int ScreenWidth
        {
            get { return screenWidth = 1280; }
            set { screenWidth = value; }
        }

        private int screenHeight = 768;

        public int ScreenHeight
        {
            get { return screenHeight = 768; }
            set { screenHeight = value; }
        }


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferHeight = screenHeight;
            _graphics.PreferredBackBufferWidth = screenWidth;
            _graphics.ApplyChanges();

            enemy = new Enemy(this, new Vector2(ScreenWidth, 0));
        }

        protected override void Initialize()
        {
            int borderWidth = Window.ClientBounds.Width;
            int borderHeight = Window.ClientBounds.Height;

            platform = new GamePlatform[_sceneLayout.Length];
            
            Texture2D platformTexture = Content.Load<Texture2D>("Platform 9");
            Color platformColor = Color.White;

            for(int i = 0; i < _sceneLayout.Length; i++)
            {
                char tile = _sceneLayout[i];
                int x = (i % 20) * spriteWidth;
                int y = (i / 20) * spriteHeight;
                Rectangle platformDisplay = new Rectangle(x, y, spriteWidth, spriteHeight);
                Rectangle platformSource;


                switch (tile)
                {
                    case '-':
                        platformSource = new Rectangle(platformTexture.Width / 6 * 4 ,0, platformTexture.Width / 6, platformTexture.Height);
                        platform[i] = new GamePlatform(platformTexture, platformDisplay, platformSource, platformColor); 
                        break;
                    case '^':
                        platformSource = new Rectangle(platformTexture.Width / 6 * 0, 0, platformTexture.Width / 6, platformTexture.Height);
                        platform[i] = new GamePlatform(platformTexture, platformDisplay, platformSource, platformColor);
                        break;
                    default:
                        platform[i] = null;
                        break;
                }
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load hero textures
            heroIdle = Content.Load<Texture2D>("Knight_Idle");
            heroRun = Content.Load<Texture2D>("Knight_run_crop");
            heroJump = Content.Load<Texture2D>("knight_jump_crop");
            heroFall = Content.Load<Texture2D>("Knight_fall_crop");

            hero = new Hero(heroIdle, heroRun, heroJump, heroFall);

            enemy.LoadContent();

        }

        protected override void Update(GameTime gameTime)
        {

            KeyboardState key = Keyboard.GetState();
            hero.Update(gameTime, key, platform);
            enemy.Update(gameTime, platform, hero);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            foreach(GamePlatform p1 in platform)
            {
                if(p1 == null)
                    continue;
                _spriteBatch.Draw(p1.PlatformTexture, p1.PlatformDisplay, p1.PlatformSource, p1.PlatformColor);
            }

            hero.Draw(_spriteBatch);
            enemy.Draw(gameTime ,_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
