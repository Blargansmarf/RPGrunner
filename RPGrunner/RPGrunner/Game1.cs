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

        bool battle;
        int currentEnemy;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            screenHeight = graphics.GraphicsDevice.Viewport.Height;
            screenWidth = graphics.GraphicsDevice.Viewport.Width;

            battle = false;

            translation = new Vector3();

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
            tempEnemy.Initialize(new Vector2(1000, (float)(Game1.screenHeight / 1.33)), new Vector2(10, 10), 1);
            tempEnemy.AddAnimation("AnimationTest", 3, .5);
            enemies.Add(tempEnemy);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
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
            if (!battle)
            {
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
                    }
                }
            }
            else
            {
                enemies[currentEnemy].secondaryStats.health -= player.secondaryStats.attack;
                player.secondaryStats.health -= enemies[currentEnemy].secondaryStats.attack;

                if (enemies[currentEnemy].secondaryStats.health <= 0)
                {
                    enemies.RemoveAt(currentEnemy);
                    battle = false;
                }
                else
                {
                    player.BattleUpdate(gameTime);
                    enemies[currentEnemy].BattleUpdate(gameTime);
                }
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            player.Draw(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Matrix.CreateTranslation(-translation));

            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
