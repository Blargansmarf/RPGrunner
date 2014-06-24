using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace RPGrunner
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static float screenWidth, screenHeight;

        Vector3 translation;

        Player player;

        List<Enemy> enemies;
        List<List<String>> menus;

        bool battle;
        int currentEnemy;

        GameTime lastEnemyAtk, lastPlayerAtk;

        KeyboardState prevKeyState, currKeyState;
        GamePadState prevGamePadState, currGamePadState;

        SpriteFont damageFont, titleFont, menuEntryFont;

        struct DamageText
        {
            public Vector2 pos;
            public Color color;
        }

        List <DamageText> enemyDamageText, playerDamageText;

        enum GameState { Playing, MainMenu };

        GameState gameState;

        bool paused;

        int currentMenuChoice;
        int mainMenuMaxChoice;

        Vector2 TitlePosition, StartEntryPosition, EntrySpacing;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            screenHeight = 700;
            screenWidth = 1100;

            graphics.PreferredBackBufferHeight = (int)screenHeight;
            graphics.PreferredBackBufferWidth = (int)screenWidth;

            graphics.ApplyChanges();

            lastEnemyAtk = new GameTime();
            lastPlayerAtk = new GameTime();

            battle = false;
            paused = false;

            gameState = GameState.MainMenu;

            menus = new List<List<String>>();

            InitializeMenus();

            base.Initialize();
        }

        private void InitializeMenus()
        {
            List<String> tempList = new List<String>();

            currentMenuChoice = 0;
            mainMenuMaxChoice = 3;

            tempList.Add("Main Menu");
            tempList.Add("Play Game");
            tempList.Add("Fullscreen Toggle");
            tempList.Add("Exit Game");

            menus.Add(tempList);

            TitlePosition = new Vector2(screenWidth * .33f, screenHeight * .33f);
            StartEntryPosition = new Vector2(screenWidth * .33f, screenHeight * .5f);
            EntrySpacing = new Vector2(0, screenHeight * .075f);
        }

        private void GameplayInitialize()
        {
            translation = new Vector3();
            enemyDamageText = new List<DamageText>();
            playerDamageText = new List<DamageText>();

            player = new Player(graphics, Content);
            enemies = new List<Enemy>();
            Enemy tempEnemy = new Enemy(graphics, Content);
            tempEnemy.Initialize(new Vector2(500, (float)(Game1.screenHeight / 1.33)), new Vector2(10, 10), 0);
            tempEnemy.AddAnimation("AnimationTest", 3, 1.5);
            enemies.Add(tempEnemy);

            tempEnemy = new Enemy(graphics, Content);
            tempEnemy.Initialize(new Vector2(750, (float)(Game1.screenHeight / 1.33)), new Vector2(10, 10), -1);
            tempEnemy.AddAnimation("AnimationTest", 3, 2);
            enemies.Add(tempEnemy);

            tempEnemy = new Enemy(graphics, Content);
            tempEnemy.Initialize(new Vector2(1300, (float)(Game1.screenHeight / 1.33)), new Vector2(10, 10), 0);
            tempEnemy.AddAnimation("AnimationTest", 3, .5);
            enemies.Add(tempEnemy);

            tempEnemy = new Enemy(graphics, Content);
            tempEnemy.Initialize(new Vector2(1300, (float)(Game1.screenHeight / 1.33)), new Vector2(10, 10), -1);
            tempEnemy.AddAnimation("AnimationTest", 3, .5);
            enemies.Add(tempEnemy);

            tempEnemy = new Enemy(graphics, Content);
            tempEnemy.Initialize(new Vector2(1300, (float)(Game1.screenHeight / 1.33)), new Vector2(10, 10), 1);
            tempEnemy.AddAnimation("AnimationTest", 3, .5);
            enemies.Add(tempEnemy);

            tempEnemy = new Enemy(graphics, Content);
            tempEnemy.Initialize(new Vector2(1400, (float)(Game1.screenHeight / 1.33)), new Vector2(10, 10), 0);
            tempEnemy.AddAnimation("AnimationTest", 3, .5);
            enemies.Add(tempEnemy);

            tempEnemy = new Enemy(graphics, Content);
            tempEnemy.Initialize(new Vector2(1500, (float)(Game1.screenHeight / 1.33)), new Vector2(10, 10), 0);
            tempEnemy.AddAnimation("AnimationTest", 3, .5);
            enemies.Add(tempEnemy);

            tempEnemy = new Enemy(graphics, Content);
            tempEnemy.Initialize(new Vector2(1400, (float)(Game1.screenHeight / 1.33)), new Vector2(10, 10), -1);
            tempEnemy.AddAnimation("AnimationTest", 3, .5);
            enemies.Add(tempEnemy);

            tempEnemy = new Enemy(graphics, Content);
            tempEnemy.Initialize(new Vector2(1400, (float)(Game1.screenHeight / 1.33)), new Vector2(10, 10), 1);
            tempEnemy.AddAnimation("AnimationTest", 3, .5);
            enemies.Add(tempEnemy);

            tempEnemy = new Enemy(graphics, Content);
            tempEnemy.Initialize(new Vector2(1000, (float)(Game1.screenHeight / 1.33)), new Vector2(10, 10), 1);
            tempEnemy.AddAnimation("AnimationTest", 3, .5);
            enemies.Add(tempEnemy);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            damageFont = Content.Load<SpriteFont>("SpriteFonts/DamageFont");
            titleFont = Content.Load<SpriteFont>("SpriteFonts/TitleFont");
            menuEntryFont = Content.Load<SpriteFont>("SpriteFonts/MenuEntryFont");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            currKeyState = Keyboard.GetState();
            currGamePadState = GamePad.GetState(PlayerIndex.One);

            if (currKeyState.IsKeyDown(Keys.Delete))
                Exit();

            if (gameState == GameState.MainMenu)
            {
                processMenuMove();
                
                if (currentMenuChoice < 0)
                    currentMenuChoice = mainMenuMaxChoice - 1;
                if (currentMenuChoice >= mainMenuMaxChoice)
                    currentMenuChoice = 0;

                if (currKeyState.IsKeyDown(Keys.Space) && prevKeyState.IsKeyUp(Keys.Space) ||
                    currGamePadState.IsButtonDown(Buttons.A) && prevGamePadState.IsButtonUp(Buttons.A))
                {
                    if (currentMenuChoice == 0)
                    {
                        gameState = GameState.Playing;
                        GameplayInitialize();
                    }
                    if (currentMenuChoice == 1)
                    {
                        graphics.IsFullScreen = !graphics.IsFullScreen;
                        graphics.ApplyChanges();
                    }
                    if (currentMenuChoice == 2)
                    {
                        Exit();
                    }
                }
            }

            if (gameState == GameState.Playing)
            {
                if (!battle)
                {
                    if (!paused)
                    {
                        if (prevGamePadState.IsButtonUp(Buttons.Start) && currGamePadState.IsButtonDown(Buttons.Start) ||
                            prevKeyState.IsKeyUp(Keys.Enter) && currKeyState.IsKeyDown(Keys.Enter))
                        {
                            paused = true;
                        }

                        translation.X += player.currentSpeed;

                        player.Update(gameTime);
                        foreach (Enemy enemy in enemies)
                        {
                            enemy.Update(gameTime);
                            if (Math.Abs(player.loc.X + player.playerDimensions.X - enemy.loc.X) <= 5
                                && enemy.depth == player.currentDepth)
                            {
                                battle = true;
                                currentEnemy = enemies.IndexOf(enemy);
                                lastEnemyAtk = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime);
                                lastPlayerAtk = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime);
                                player.pState = Player.PlayerState.attacking;
                                player.ResetAnimations();
                            }
                        }
                    }
                    else
                    {
                        if (prevGamePadState.IsButtonUp(Buttons.Start) && currGamePadState.IsButtonDown(Buttons.Start) ||
                            prevKeyState.IsKeyUp(Keys.Enter) && currKeyState.IsKeyDown(Keys.Enter))
                        {
                            paused = false;
                        }
                    }
                }
                else
                {
                    if (gameTime.TotalGameTime.TotalSeconds - lastEnemyAtk.TotalGameTime.TotalSeconds
                        >= enemies[currentEnemy].secondaryStats.atkSpeed)
                    {
                        enemies[currentEnemy].attackNum++;
                        player.secondaryStats.health -= enemies[currentEnemy].secondaryStats.attack;
                        lastEnemyAtk = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime);

                        DamageText tempDamText = new DamageText();
                        tempDamText.pos = player.startLoc;
                        tempDamText.pos.X += 1;
                        tempDamText.pos.Y -= 20;
                        tempDamText.color = new Color(255, 0, 0, 255);
                        enemyDamageText.Add(tempDamText);
                    }

                    if (gameTime.TotalGameTime.TotalSeconds - lastPlayerAtk.TotalGameTime.TotalSeconds
                        >= player.secondaryStats.atkSpeed)
                    {
                        player.attackNum++;
                        enemies[currentEnemy].secondaryStats.health -= player.secondaryStats.attack;
                        lastPlayerAtk = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime);

                        DamageText tempDamText2 = new DamageText();
                        tempDamText2.pos = player.startLoc;
                        tempDamText2.pos.X += 1;
                        tempDamText2.pos.Y -= 20;
                        tempDamText2.pos.X += 22;
                        tempDamText2.color = new Color(255, 0, 0, 255);
                        playerDamageText.Add(tempDamText2);
                    }

                    if (enemies[currentEnemy].secondaryStats.health <= 0)
                    {
                        enemies.RemoveAt(currentEnemy);
                        battle = false;
                        player.pState = Player.PlayerState.walking;
                        player.attackNum = 0;
                        player.ResetAnimations();
                        playerDamageText.Clear();
                        enemyDamageText.Clear();
                    }
                    else
                    {
                        for (int x = 0; x < enemyDamageText.Count; x++)
                        {
                            DamageText temp = new DamageText();
                            temp.pos.X = enemyDamageText[x].pos.X;
                            temp.pos.Y = enemyDamageText[x].pos.Y - .2f;
                            temp.color = enemyDamageText[x].color;
                            temp.color.A -= 3;
                            if (temp.color.A < 3)
                            {
                                enemyDamageText.RemoveAt(x);
                                x--;
                            }
                            else
                                enemyDamageText[x] = temp;
                        }

                        for (int x = 0; x < playerDamageText.Count; x++)
                        {
                            DamageText temp = new DamageText();
                            temp.pos.X = playerDamageText[x].pos.X;
                            temp.pos.Y = playerDamageText[x].pos.Y - .2f;
                            temp.color = new Color(255, 0, 0, playerDamageText[x].color.A);
                            temp.color.A -= 3;
                            if (temp.color.A < 3)
                            {
                                playerDamageText.RemoveAt(x);
                                x--;
                            }
                            else
                                playerDamageText[x] = temp;
                        }

                        if (!player.BattleUpdate(gameTime))
                        {
                            Exit();
                        }
                        enemies[currentEnemy].BattleUpdate(gameTime);
                    }
                }
            }

            prevKeyState = currKeyState;
            prevGamePadState = currGamePadState;

            base.Update(gameTime);
        }

        private void processMenuMove()
        {
            if (currKeyState.IsKeyDown(Keys.Down) && prevKeyState.IsKeyUp(Keys.Down) ||
                currGamePadState.IsButtonDown(Buttons.DPadDown) && prevGamePadState.IsButtonUp(Buttons.DPadDown) ||
                currGamePadState.ThumbSticks.Left.Y < -.3)
            {
                currentMenuChoice++;
            }
            if (currKeyState.IsKeyDown(Keys.Up) && prevKeyState.IsKeyUp(Keys.Up) ||
                currGamePadState.IsButtonDown(Buttons.DPadUp) && prevGamePadState.IsButtonUp(Buttons.DPadUp) ||
                currGamePadState.ThumbSticks.Left.Y > .3)
            {
                currentMenuChoice--;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            if(gameState == GameState.Playing)
                player.Draw(spriteBatch);
            
            if (battle)
            {
                foreach (DamageText text in enemyDamageText)
                    spriteBatch.DrawString(damageFont, enemies[currentEnemy].secondaryStats.attack + "", text.pos, text.color);
                foreach (DamageText text in playerDamageText)
                    spriteBatch.DrawString(damageFont, player.secondaryStats.attack + "", text.pos, text.color);
            }

            if (gameState == GameState.MainMenu)
            {
                DrawMenu(0);
            }

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Matrix.CreateTranslation(-translation));

            if(gameState == GameState.Playing)
                foreach (Enemy enemy in enemies)
                {
                    enemy.Draw(spriteBatch);
                }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawMenu(int x)
        {
            spriteBatch.DrawString(titleFont, menus[x][0], TitlePosition, Color.Black);

            for (int i = 1; i < menus[x].Count; i++)
            {
                Color textCol = new Color();
                
                if (currentMenuChoice == i - 1)
                    textCol = Color.Black;
                else
                    textCol = Color.DarkGray;

                spriteBatch.DrawString(menuEntryFont, menus[x][i], StartEntryPosition + EntrySpacing * (i - 1), textCol);
            }
        }
    }
}
