using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media; //song
using System.Collections.Generic;

namespace GameDesign_FinalProject //sample
{
    public class Game1 : Game
    {
        private float bossDeathTimer = 0f;
        private bool bossDeathHandled = false;

        private Texture2D pauseExitTexture;
        private Rectangle pauseExitRect;
        private bool pauseExitClicked = false;

        private int savedStageIndex;
        private string savedSceneLayout;
        private Vector2 savedHeroPosition;


        FinalBoss finalBoss;
        Texture2D bossWalkTex, bossHitTex, bossDeathTex;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D heartTexture;

        List<Collectible> collectibles = new List<Collectible>();

        private SpriteFont font; // For drawing 0/3 as text
        private Texture2D collectedBanner; // For drawing "Item Collected" banner

        Song song;
        SoundEffect effect;
        MouseState previousMouseState;
        SoundEffect jumpEffect;
        KeyboardState previousKeyState;

        enum GameState { MainMenu, Playing, Loading, GameOver, Win }
        GameState currentGameState = GameState.MainMenu;


        MainMenu mainMenu;
        Texture2D titleTex, playTex, loadTex, exitTex;

        Texture2D winScreenTex, gameOverTex;
        Texture2D winResumeBtn, winExitBtn, gameOverResumeBtn, gameOverExitBtn;
        Rectangle winResumeRect, winExitRect, gameOverResumeRect, gameOverExitRect;

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

        private bool isPaused = false;
        private Texture2D pauseTexture;
        private Texture2D resumeTexture;
        private Rectangle resumeButtonRect;
        private bool resumeClicked = false;


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
                "                   E" +
                "    E            B  " +
                "  C             2111" +
                "111112           777" +
                "77777    22     E   " +
                "                    " +
                "              211111" +
                "                7777" +
                "          E         " +
                "111113  L        E  " +
                "666664111113        " +
                "66666666666411111111",

                // Stage 2 layout (edit this as you want)
                "                    " +
                "           E        " +
                "                    " +
                "      21111112      " +
                "     E  7777    E   " +
                "                    " +
                "   21112     21112  " +
                "    777       777   " +
                "                    " +
                "                Z   " +
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


            heartTexture = Content.Load<Texture2D>("heart");

            collectedBanner = Content.Load<Texture2D>("collected"); // Make sure it's in your Content folder
            font = Content.Load<SpriteFont>("DefaultFont"); // Create this in Content Pipeline


            Texture2D item1Tex = Content.Load<Texture2D>("item1");
            Texture2D item2Tex = Content.Load<Texture2D>("item2");
            Texture2D item3Tex = Content.Load<Texture2D>("item3");

            // Manually position collectibles where you want them on the map
            collectibles.Add(new Collectible(item1Tex, new Vector2(100, 128)));
            collectibles.Add(new Collectible(item2Tex, new Vector2(1064, 64)));
            collectibles.Add(new Collectible(item3Tex, new Vector2(1000, 600)));
            bossWalkTex = Content.Load<Texture2D>("boss_walk_small");
            bossHitTex = Content.Load<Texture2D>("boss_hit_small");
            bossDeathTex = Content.Load<Texture2D>("boss_death_small");


            titleTex = Content.Load<Texture2D>("Home Screen (1)");
            playTex = Content.Load<Texture2D>("PLAY");
            loadTex = Content.Load<Texture2D>("Load");
            exitTex = Content.Load<Texture2D>("Exit");

            pauseTexture = Content.Load<Texture2D>("Paused");
            resumeTexture = Content.Load<Texture2D>("resume");

            pauseExitTexture = Content.Load<Texture2D>("Exit");
          

            winScreenTex = Content.Load<Texture2D>("Win");
            gameOverTex = Content.Load<Texture2D>("Game_Over");
            winResumeBtn = resumeTexture;
            winExitBtn = pauseExitTexture; 
            gameOverResumeBtn = resumeTexture;
            gameOverExitBtn = pauseExitTexture;

            winResumeRect = new Rectangle((screenWidth / 2) - 100, (screenHeight / 2) + 50, 200, 70);
            winExitRect = new Rectangle((screenWidth / 2) - 100, (screenHeight / 2) + 130, 200, 70);

            gameOverResumeRect = new Rectangle((screenWidth / 2) - 100, (screenHeight / 2) + 200, 200, 70);
            gameOverExitRect = new Rectangle((screenWidth / 2) - 100, (screenHeight / 2) + 300, 200, 70);

            mainMenu = new MainMenu(titleTex, playTex, loadTex, exitTex, screenWidth);

            foreach (Enemy e in enemies)
                e.LoadContent();

            song = Content.Load<Song>("Audios/StageSong");
            MediaPlayer.Play(song);

            effect = Content.Load<SoundEffect>("Audios/Pewpew");
            jumpEffect = Content.Load<SoundEffect>("Audios/Jump");

            pauseExitRect = new Rectangle((screenWidth / 2) - 100, (screenHeight / 2) + 130, 200, 70); // below Resume


            // Position resume button at center bottom
            resumeButtonRect = new Rectangle((screenWidth / 2) - 100, (screenHeight / 2) + 50, 200, 70);



        }

        protected override void Update(GameTime gameTime)
        {
           


            KeyboardState key = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();
            Point mousePoint = mouse.Position;

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
                    {
                        currentStageIndex = savedStageIndex;
                        ResetGame(savedSceneLayout);
                        hero.Position = savedHeroPosition;
                        currentGameState = GameState.Playing;
                    }
                    else if (mainMenu.ExitClicked)
                        Exit(); // Quit the game
                    break;

                case GameState.Playing:

                    if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !previousKeyState.IsKeyDown(Keys.Escape))
                    {
                        isPaused = !isPaused;
                    }

                    if (isPaused)
                    {
                        // Pause menu input (buttons)
                        if (mouse.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                        {
                             mousePoint = mouse.Position;

                            if (resumeButtonRect.Contains(mousePoint))
                            {
                                isPaused = false;
                            }
                            else if (pauseExitRect.Contains(mousePoint))
                            {
                                savedStageIndex = currentStageIndex;
                                savedSceneLayout = _sceneLayout;
                                savedHeroPosition = hero.Position;

                                isPaused = false;
                                currentGameState = GameState.MainMenu;
                            }
                        }

                        // Stop further updates if paused
                        previousKeyState = key;
                        previousMouseState = mouse;
                        return; // ✅ skip rest of Update while paused
                    }

                    if (!isPaused)
                    {
                        if (finalBoss != null)
                        {
                            finalBoss.Update(gameTime);

                            // Check for projectile collision
                            for (int i = hero.Projectiles.Count - 1; i >= 0; i--)
                            {
                                if (finalBoss.CollidesWith(hero.Projectiles[i].BoundingBox))
                                {
                                    finalBoss.TakeDamage();
                                    hero.Projectiles.RemoveAt(i);
                                    break;
                                }
                            }

                            // Damage hero if touching boss
                            if (finalBoss.CollidesWith(hero.BoundingBox))
                            {
                                hero.TakeDamage();
                                // Still triggers hit state
                            }

                            // If boss defeated, maybe trigger something
                            if (finalBoss != null && finalBoss.IsDead)
                            {
                                bossDeathTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                                if (!bossDeathHandled && bossDeathTimer >= 1f)
                                {
                                    bossDeathHandled = true;
                                    finalBoss = null; //  Remove the boss sprite from view


                                }
                            }

                        }
                    }

                    if (key.IsKeyDown(Keys.Space) && !previousKeyState.IsKeyDown(Keys.Space))
                    {
                        if (!hero.IsJumping)
                            jumpEffect.Play();
                    }

                    hero.CheckEnemyCollision(enemies);
                    hero.Update(gameTime, key, platform, mouse);
                    hero.CheckProjectileEnemyCollision(enemies);

                    previousKeyState = key;

                    if (mouse.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                    {
                        effect.Play();
                    }


                    foreach (var collectible in collectibles)
                    {
                        if (!collectible.IsCollected && hero.BoundingBox.Intersects(collectible.BoundingBox))
                        {
                            collectible.IsCollected = true;
                        }

                        collectible.Update(gameTime);
                    }

                    if (hero.DeathComplete)
                    {
                        currentGameState = GameState.MainMenu;
                    }

                    bool allEnemiesDefeated = enemies.Count == 0;
                    bool allCollected = collectibles.TrueForAll(c => c.IsCollected);
                    bool bossDefeated = finalBoss == null || finalBoss.IsDead;

                    if (allEnemiesDefeated && allCollected && bossDefeated)
                    {
                        currentStageIndex++;

                        if (currentStageIndex < stages.Count)
                        {
                            ResetGame(stages[currentStageIndex]);
                        }
                        else
                        {
                            currentGameState = GameState.Win;

                        }
                    }

                    if (hero.DeathComplete)
                    {
                        savedStageIndex = currentStageIndex;
                        savedSceneLayout = _sceneLayout;
                        savedHeroPosition = hero.Position;
                        currentGameState = GameState.GameOver;
                    }

                    break;


                case GameState.Loading:
                    // You can implement your loading logic here
                    currentGameState = GameState.Playing;
                    break;

                case GameState.GameOver:
                case GameState.Win:
                    if (currentGameState == GameState.GameOver && gameOverResumeRect.Contains(mousePoint))
                    {
                        currentStageIndex = 0; // Restart from the first level
                        ResetGame(stages[currentStageIndex]);
                        currentGameState = GameState.Playing;
                    }
                    else if (currentGameState == GameState.Win && winResumeRect.Contains(mousePoint))
                    {
                        ResetGame(savedSceneLayout);
                        hero.Position = savedHeroPosition;
                        currentStageIndex = savedStageIndex;
                        currentGameState = GameState.Playing;
                    }
                    else if ((currentGameState == GameState.GameOver && gameOverExitRect.Contains(mousePoint)) ||
                             (currentGameState == GameState.Win && winExitRect.Contains(mousePoint)))
                    {
                        currentGameState = GameState.MainMenu;
                    }
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
                        enemy.TakeDamage(); // Instead of removing immediately

                      

                        hero.Projectiles.RemoveAt(j);
                        break;
                    }

                }
                if (enemy.IsDead && enemy.CurrentFrame == 3)
                {
                    enemies.RemoveAt(i); // ✅ remove after death anim plays
                }
            }
            previousMouseState = mouse;
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

                if (finalBoss != null)
                    finalBoss.Draw(_spriteBatch);

                // Draw health hearts
                for (int i = 0; i < hero.Health; i++)
                {
                    _spriteBatch.Draw(heartTexture, new Vector2(20 + i * 70, 20), Color.White);
                }

                // Draw collected banner and count
                int collectedCount = 0;
                foreach (var item in collectibles)
                {
                    if (item.IsCollected)
                        collectedCount++;
                }


                int bannerWidth = collectedBanner.Width;
                int bannerHeight = collectedBanner.Height;

                Vector2 bannerPos = new Vector2(screenWidth - bannerWidth + 350, screenHeight - bannerHeight - 680);
                float scale = 0.4f; 
                _spriteBatch.Draw(collectedBanner, bannerPos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);


                string collectedText = $"{collectedCount}/{collectibles.Count}";
                float textScale = 1.5f; 

                Vector2 textPos = new Vector2(screenWidth - 70, screenHeight - 735);
                _spriteBatch.DrawString(font, collectedText, textPos, Color.White, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);

                if (isPaused)
                {
                    // Darken the screen (optional semi-transparent overlay)
                    _spriteBatch.Draw(pauseTexture, backgroundRec, Color.White);

                    // Draw resume button
                    _spriteBatch.Draw(resumeTexture, resumeButtonRect, Color.White);
                    _spriteBatch.Draw(pauseExitTexture, pauseExitRect, Color.White);
                }

            }

            else if (currentGameState == GameState.GameOver)
            {
                _spriteBatch.Draw(gameOverTex, backgroundRec, Color.White);
                _spriteBatch.Draw(gameOverResumeBtn, gameOverResumeRect, Color.White);
                _spriteBatch.Draw(gameOverExitBtn, gameOverExitRect, Color.White);
            }
            else if (currentGameState == GameState.Win)
            {
                _spriteBatch.Draw(winScreenTex, backgroundRec, Color.White);
                _spriteBatch.Draw(winResumeBtn, winResumeRect, Color.White);
                _spriteBatch.Draw(winExitBtn, winExitRect, Color.White);
            }

            foreach (Projectile p in projectiles)
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
                    finalBoss = null;
                    bossDeathHandled = false;
                    bossDeathTimer = 0f;

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
                        Rectangle platformSource;switch (tile)
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
                            case 'B': //collectible but second item
                                collectibles.Add(new Collectible(Content.Load<Texture2D>("item2"), new Vector2(x, y)));
                                platform[i] = null;
                                break;
                            case 'L': //collectible but third item
                                collectibles.Add(new Collectible(Content.Load<Texture2D>("item3"), new Vector2(x, y)));
                                platform[i] = null;
                                break;
                            case 'Z': // Final boss spawn point
                                finalBoss = new FinalBoss(bossWalkTex, bossHitTex, bossDeathTex, new Vector2(x, y), screenWidth);
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
