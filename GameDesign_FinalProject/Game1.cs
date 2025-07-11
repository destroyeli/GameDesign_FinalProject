using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GameDesign_FinalProject //sample
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        List<Collectible> collectibles = new List<Collectible>();

        enum GameState { MainMenu, Playing, Loading }
        GameState currentGameState = GameState.MainMenu;


        MainMenu mainMenu;
        Texture2D titleTex, playTex, loadTex, exitTex;

        private List<Projectile> projectiles;
        private Texture2D projectileTexture;

        Hero hero;
        Texture2D heroIdle, heroRun, heroJump, heroFall, heroShoot;

        List<Enemy> enemies = new List<Enemy>();

        GamePlatform[] platform;
        Texture2D platformTexture;
        Rectangle platformDisplay, platformSource, backgroundRec;
        Color platformColor, backgroundColor;

        Texture2D backgroundTexture;

        private List<string> stages;
        private int currentStageIndex;
        private string _sceneLayout;

        int spriteWidth = 64,
            spriteHeight = 64;

        //string _sceneLayout = "                    " +
        //                      "    E               " +
        //                      "                2111" +
        //                      "111112           777" +
        //                      "77777    22     E   " +
        //                      "                    " +
        //                      "               21111" +
        //                      "                7777" +
        //                      "          E         " +
        //                      "111113              " +
        //                      "666664111113        " +
        //                      "66666666666411111111";

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

        }

        protected override void Initialize()
        {
            backgroundRec = new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height);
            backgroundColor = Color.White;

            stages = new List<string>()
            {
                // Stage 1 layout
                "                    " +
                "    E            C  " +
                "  C             2111" +
                "111112           777" +
                "77777    22     E   " +
                "                    " +
                "               21111" +
                "                7777" +
                "          E         " +
                "111113  C           " +
                "666664111113        " +
                "66666666666411111111",

                // Stage 2 layout (edit this as you want)
                "                    " +
                "                    " +
                "                    " +
                "      21111112      " +
                "       777777       " +
                "                    " +
                "   21112     21112  " +
                "    777       777 E " +
                "                    " +
                "                    " +
                "11111111111111111111" +
                "66666666666666666666"
            };
            int borderWidth = Window.ClientBounds.Width;
            int borderHeight = Window.ClientBounds.Height;


            platformTexture = Content.Load<Texture2D>("Platform");
            platformColor = Color.White;

            projectiles = new List<Projectile>();

            
            base.Initialize();
        }

        protected override void LoadContent()
        {

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            projectileTexture = Content.Load<Texture2D>("bullet_final");

            backgroundTexture = Content.Load<Texture2D>("Background1");

            // Load hero textures
            heroIdle = Content.Load<Texture2D>("eli_Idle");
            heroRun = Content.Load<Texture2D>("eli_walk");
            heroJump = Content.Load<Texture2D>("eli_jump");
            heroFall = Content.Load<Texture2D>("eli_fall");
            heroShoot = Content.Load<Texture2D>("eli_shoot"); // ← your shoot spritesheet
            Texture2D heroSprint = Content.Load<Texture2D>("eli_sprint"); // ← your sprint spritesheet
            Texture2D heroHit = Content.Load<Texture2D>("eli_hit");
            Texture2D heroDeath = Content.Load<Texture2D>("eli_death_7");
            hero = new Hero(heroIdle, heroRun, heroJump, heroFall, heroSprint, heroShoot, heroHit, heroDeath, projectileTexture);

            



            Texture2D item1Tex = Content.Load<Texture2D>("item1");
            Texture2D item2Tex = Content.Load<Texture2D>("item2");
            Texture2D item3Tex = Content.Load<Texture2D>("item3");

            // Manually position collectibles where you want them on the map
            collectibles.Add(new Collectible(item1Tex, new Vector2(100, 128)));
            collectibles.Add(new Collectible(item2Tex, new Vector2(1064, 64)));
            collectibles.Add(new Collectible(item3Tex, new Vector2(500, 500)));


            titleTex = Content.Load<Texture2D>("Home Screen (1)");
            playTex = Content.Load<Texture2D>("PLAY");
            loadTex = Content.Load<Texture2D>("Load");
            exitTex = Content.Load<Texture2D>("Exit");

            mainMenu = new MainMenu(titleTex, playTex, loadTex, exitTex, screenWidth);

            foreach (Enemy e in enemies)
                e.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {

            KeyboardState key = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            switch (currentGameState)
            {
                case GameState.MainMenu:
                    mainMenu.Update(mouse);

                    if (mainMenu.PlayClicked)
                    {
                        currentStageIndex = 0; // Start from the first stage
                        ResetGame(stages[currentStageIndex]); // Reset the game state
                        currentGameState = GameState.Playing;
                    }
                    else if (mainMenu.LoadClicked)
                        currentGameState = GameState.Loading;
                    else if (mainMenu.ExitClicked)
                        Exit(); // Quit the game
                    break;

                case GameState.Playing:
                    hero.CheckEnemyCollision(enemies);
                    hero.Update(gameTime, key, platform, mouse);
                    hero.CheckProjectileEnemyCollision(enemies);

                    if (hero.DeathComplete)
                    {
                        currentGameState = GameState.MainMenu;
                    }

                    foreach (var collectible in collectibles)
                    {
                        if (!collectible.IsCollected && hero.BoundingBox.Intersects(collectible.BoundingBox))
                        {
                            collectible.IsCollected = true;
                            // Optional: Add score or sound effect here
                        }

                        collectible.Update(gameTime);
                    }

                    if (hero.DeathComplete)
                    {
                        currentGameState = GameState.MainMenu;
                    }

                    // --- STAGE CLEAR CHECK ---
                    bool allEnemiesDefeated = enemies.Count == 0;
                    bool allCollected = collectibles.TrueForAll(c => c.IsCollected);

                    if (allEnemiesDefeated && allCollected)
                    {
                        currentStageIndex++;

                        if (currentStageIndex < stages.Count)
                        {
                            ResetGame(stages[currentStageIndex]);
                        }
                        else
                        {
                            // No more stages → back to main menu or show "You Win" screen
                            currentGameState = GameState.MainMenu;
                        }
                    }
                    break;

                case GameState.Loading:
                    // You can implement your loading logic here
                    currentGameState = GameState.Playing;
                    break;
            }

            foreach (Enemy e in enemies)
                e.Update(gameTime, platform, hero);

            foreach(Projectile p in projectiles)
            {
                p.Update(gameTime);
            }


            //mark
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                Enemy enemy = enemies[i];

                for (int j = hero.Projectiles.Count - 1; j >= 0; j--)
                {
                    Projectile projectile = hero.Projectiles[j];

                    if (enemy.PositionRectangle.Intersects(projectile.BoundingBox))
                    {
                        enemies.RemoveAt(i); // Remove enemy
                        hero.Projectiles.RemoveAt(j); // Remove bullet
                        break; // Break out of projectile loop once enemy is hit
                    }
                }
            }
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            _spriteBatch.Draw(backgroundTexture, backgroundRec, backgroundColor);


            if (currentGameState == GameState.MainMenu)
            {
                mainMenu.Draw(_spriteBatch);
            }
            else if (currentGameState == GameState.Playing)
            {
                foreach (GamePlatform p1 in platform)
                {
                    if (p1 != null)
                        _spriteBatch.Draw(p1.PlatformTexture, p1.PlatformDisplay, p1.PlatformSource, p1.PlatformColor);
                }

                foreach (Enemy e in enemies)
                    e.Draw(gameTime, _spriteBatch);

                foreach (var collectible in collectibles)
                    collectible.Draw(_spriteBatch);

               
                
            hero.Draw(_spriteBatch, gameTime);
            }

            foreach(Projectile p in projectiles)
            {
                p.Draw(gameTime, _spriteBatch);
            }

            _spriteBatch.End();
            base.Draw(gameTime);

        }


        private void ResetGame(string layout)
        {
            _sceneLayout = layout;
            platform = new GamePlatform[_sceneLayout.Length];

            // Clear old enemies
            enemies.Clear();
            collectibles.Clear();

            // Reset platforms
            for (int i = 0; i < _sceneLayout.Length; i++)
            {
               char tile = _sceneLayout[i];
                int x = (i % 20) * spriteWidth;
                int y = (i / 20) * spriteHeight;
                Rectangle platformDisplay = new Rectangle(x, y, spriteWidth, spriteHeight);
                Rectangle platformSource;

                switch (tile)
                {
                    case '1':
                        platformSource = new Rectangle(platformTexture.Width / 7 * 0, 0, platformTexture.Width / 7, platformTexture.Height);
                        platform[i] = new GamePlatform(platformTexture, platformDisplay, platformSource, platformColor);
                        break;
                    case '2':
                        platformSource = new Rectangle(platformTexture.Width / 7 * 1, 0, platformTexture.Width / 7, platformTexture.Height);
                        platform[i] = new GamePlatform(platformTexture, platformDisplay, platformSource, platformColor);
                        break;
                    case '3':
                        platformSource = new Rectangle(platformTexture.Width / 7 * 2, 0, platformTexture.Width / 7, platformTexture.Height);
                        platform[i] = new GamePlatform(platformTexture, platformDisplay, platformSource, platformColor);
                        break;
                    case '4':
                        platformSource = new Rectangle(platformTexture.Width / 7 * 3, 0, platformTexture.Width / 7, platformTexture.Height);
                        platform[i] = new GamePlatform(platformTexture, platformDisplay, platformSource, platformColor);
                        break;
                    case '6':
                        platformSource = new Rectangle(platformTexture.Width / 7 * 5, 0, platformTexture.Width / 7, platformTexture.Height);
                        platform[i] = new GamePlatform(platformTexture, platformDisplay, platformSource, platformColor);
                        break;
                    case '7':
                        platformSource = new Rectangle(platformTexture.Width / 7 * 6, 0, platformTexture.Width / 7, platformTexture.Height);
                        platform[i] = new GamePlatform(platformTexture, platformDisplay, platformSource, platformColor);
                        break;
                    case 'E':
                        enemies.Add(new Enemy(this, new Vector2(x, y)));
                        platform[i] = null;
                        break;
                    case 'C': // 'C' = Collectible spot
                        collectibles.Add(new Collectible(Content.Load<Texture2D>("item1"), new Vector2(x, y)));
                        platform[i] = null;
                        break;
                    default:
                        platform[i] = null;
                        break;
                }
            }

            foreach (Enemy e in enemies)
                e.LoadContent();

            // Recreate hero
            hero = new Hero(heroIdle, heroRun, heroJump, heroFall,
                            Content.Load<Texture2D>("eli_sprint"),
                            Content.Load<Texture2D>("eli_shoot"),
                            Content.Load<Texture2D>("eli_hit"),
                            Content.Load<Texture2D>("eli_death_7"),
                            projectileTexture);
        }

    }
}
