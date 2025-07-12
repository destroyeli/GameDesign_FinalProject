using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media; //song ohh lala
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

        private SpriteFont font; // for drawing 0/3 as text
        private Texture2D collectedBanner; // item collected text

        Song song;
        SoundEffect effect;
        MouseState previousMouseState;
        SoundEffect jumpEffect;
        KeyboardState previousKeyState;
        SoundEffect collectEffect;
        SoundEffect bossEffect;
        SoundEffect winEffect;

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

                // Stage 2 layout (boss arena)
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

            //hero sprites!!!
            heroIdle = Content.Load<Texture2D>("eli_Idle");
            heroRun = Content.Load<Texture2D>("eli_walk");
            heroJump = Content.Load<Texture2D>("eli_jump");
            heroFall = Content.Load<Texture2D>("eli_fall");
            heroShoot = Content.Load<Texture2D>("eli_shoot"); 
            Texture2D heroSprint = Content.Load<Texture2D>("eli_sprint"); 
            Texture2D heroHit = Content.Load<Texture2D>("eli_hit");
            Texture2D heroDeath = Content.Load<Texture2D>("eli_death_7");
            hero = new Hero(heroIdle, heroRun, heroJump, heroFall, heroSprint, heroShoot, heroHit, heroDeath, projectileTexture);


            heartTexture = Content.Load<Texture2D>("heart");

            collectedBanner = Content.Load<Texture2D>("collected"); 
            font = Content.Load<SpriteFont>("DefaultFont");

            Texture2D item1Tex = Content.Load<Texture2D>("item1");
            Texture2D item2Tex = Content.Load<Texture2D>("item2");
            Texture2D item3Tex = Content.Load<Texture2D>("item3");

            //collectibles positioning
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
            winResumeBtn = Content.Load<Texture2D>("back_win") ;
            winExitBtn = pauseExitTexture; 
            gameOverExitBtn = Content.Load<Texture2D>("back");
            gameOverResumeBtn = resumeTexture;

            winResumeRect = new Rectangle((screenWidth / 2) -150, (screenHeight / 2) + 300, 300, 70);
            winExitRect = new Rectangle((screenWidth / 2) - 50, (screenHeight / 2) + 400, 200, 70);

            gameOverResumeRect = new Rectangle((screenWidth / 2) - 150, (screenHeight / 2) + 200, 300, 70);
            gameOverExitRect = new Rectangle((screenWidth / 2) - 150, (screenHeight / 2) + 300, 300, 70);

            mainMenu = new MainMenu(titleTex, playTex, loadTex, exitTex, screenWidth);

            foreach (Enemy e in enemies)
                e.LoadContent();

            song = Content.Load<Song>("Audios/StageSong");
            MediaPlayer.Play(song);

            effect = Content.Load<SoundEffect>("Audios/Pewpew");
            jumpEffect = Content.Load<SoundEffect>("Audios/Jump");
            collectEffect = Content.Load<SoundEffect>("Audios/Collect");
            bossEffect = Content.Load<SoundEffect>("Audios/BossDeath");
            winEffect = Content.Load<SoundEffect>("Audios/Winner");
            pauseExitRect = new Rectangle((screenWidth / 2) - 100, (screenHeight / 2) + 130, 200, 70); 
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
                        currentStageIndex = 0; 
                        ResetGame(stages[currentStageIndex]); 
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
                        Exit(); //QUIT
                    break;

                case GameState.Playing:

                    if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !previousKeyState.IsKeyDown(Keys.Escape))
                    {
                        isPaused = !isPaused;
                    }

                    if (isPaused)
                    {
                        //BUTTONS pause
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

                        previousKeyState = key;
                        previousMouseState = mouse;
                        return;
                    }

                    if (!isPaused)
                    {
                        if (finalBoss != null)
                        {
                            finalBoss.Update(gameTime);

                            // projectile checking
                            for (int i = hero.Projectiles.Count - 1; i >= 0; i--)
                            {
                                if (finalBoss.CollidesWith(hero.Projectiles[i].BoundingBox))
                                {
                                    finalBoss.TakeDamage();

                                    hero.Projectiles.RemoveAt(i);
                                    break;
                                }
                            }

                            //touch damage
                            if (finalBoss.CollidesWith(hero.BoundingBox))
                            {
                                hero.TakeDamage();
                            }

                            if (finalBoss != null && finalBoss.IsDead)
                            {
                                bossDeathTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                                if (finalBoss != null && finalBoss.IsDead)
                                {
                                    if (!bossDeathHandled)
                                    {
                                        bossDeathHandled = true;

                                        bossEffect?.Play();
                                    }

                                    bossDeathTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                                    if (bossDeathTimer >= 1f)
                                    {
                                        finalBoss = null; 
                                    }
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
                            collectEffect?.Play();
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
                            winEffect?.Play();
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
                    currentGameState = GameState.Playing;
                    break;

                case GameState.GameOver:
                case GameState.Win:
                    if(mouse.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                    {
                        if (currentGameState == GameState.GameOver && gameOverResumeRect.Contains(mousePoint))
                        {
                        
                            ResetGame(stages[currentStageIndex]);
                            currentGameState = GameState.Playing;
                        }
                        else if (currentGameState == GameState.GameOver && gameOverExitRect.Contains(mousePoint))
                        {
                            
                            currentGameState = GameState.MainMenu;
                        }
                        else if (currentGameState == GameState.Win && winResumeRect.Contains(mousePoint))
                        {
                            currentGameState = GameState.MainMenu;
                        }

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
                        enemy.TakeDamage();

                      

                        hero.Projectiles.RemoveAt(j);
                        break;
                    }

                }
                if (enemy.IsDead && enemy.CurrentFrame == 3)
                {
                    enemies.RemoveAt(i); 
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

            //platforms
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

                //hearts/health
                for (int i = 0; i < hero.Health; i++)
                {
                    _spriteBatch.Draw(heartTexture, new Vector2(20 + i * 70, 20), Color.White);
                }

                //collected items text number
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
                    _spriteBatch.Draw(pauseTexture, backgroundRec, Color.White);
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

            enemies.Clear();
                    collectibles.Clear();

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
                            case 'C': // 'C' = collectible spot
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

            // hero
            hero = new Hero(heroIdle, heroRun, heroJump, heroFall,
                            Content.Load<Texture2D>("eli_sprint"),
                            Content.Load<Texture2D>("eli_shoot"),
                            Content.Load<Texture2D>("eli_hit"),
                            Content.Load<Texture2D>("eli_death_7"),
                            projectileTexture);

         

        }

    }
}
