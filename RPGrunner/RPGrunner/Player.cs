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
    class Player
    {
        SpriteAnimation animation;
        Texture2D spriteSheet;
        public Vector2 startLoc, loc;

        public Vector2 playerDimensions;

        GraphicsDeviceManager graphics;
        ContentManager content;

        KeyboardState prevKeyState, currKeyState;
        GamePadState prevGamePadState, currGamePadState;

        Rectangle currentHealthBar, missingHealthBar;
        Texture2D healthBarTexture;

        int maxHealthBarLength;

        public struct PStats
        {
            public int strength, vitality, intelligence, dexterity;
        }

        public struct SStats
        {
            public int attack, defense, health, magic, maxHealth, maxMana;
            public float resistance, dodge, criticalChance, criticalBonus;
        }

        public PStats primaryStats;
        public SStats secondaryStats;

        public float currentSpeed;

        public int currentDepth;

        public Player(GraphicsDeviceManager g, ContentManager c)
        {
            graphics = g;
            content = c;

            Initialize();
        }
 
        public void Initialize()
        {
            LoadContent();

            currentDepth = 0;
            currentSpeed = 2;

            primaryStats = new PStats();
            secondaryStats = new SStats();

            secondaryStats.maxHealth = 500;
            secondaryStats.health = 500;
            secondaryStats.attack = 3;

            playerDimensions = new Vector2(10, 10);

            startLoc = new Vector2(Game1.screenWidth / 10, (float)(Game1.screenHeight / 1.33));

            maxHealthBarLength = (int)Game1.screenWidth / 5;

            currentHealthBar = new Rectangle((int)Game1.screenWidth/20, (int)Game1.screenHeight/20,
                (int)Game1.screenWidth/5, (int)Game1.screenHeight/20);
            missingHealthBar = new Rectangle(currentHealthBar.Right, currentHealthBar.Y, 0, currentHealthBar.Height); 

            loc = startLoc;

            animation = new SpriteAnimation(3, spriteSheet, 1, startLoc);
        }

        public void LoadContent()
        {
            spriteSheet = content.Load<Texture2D>("AnimationTest");
            healthBarTexture = content.Load<Texture2D>("WhiteSquare");
        }

        public void Update(GameTime gameTime)
        {
            currGamePadState = GamePad.GetState(PlayerIndex.One);
            currKeyState = Keyboard.GetState();

            loc.X += currentSpeed;

            if ((currKeyState.IsKeyDown(Keys.Down) && prevKeyState.IsKeyUp(Keys.Down)
                || currGamePadState.ThumbSticks.Left.Y < -.33 && prevGamePadState.ThumbSticks.Left.Y >= -.33)
                && currentDepth < 1)
            {
                currentDepth++;
            }

            if ((currKeyState.IsKeyDown(Keys.Up) && prevKeyState.IsKeyUp(Keys.Up)
                || currGamePadState.ThumbSticks.Left.Y > .33 && prevGamePadState.ThumbSticks.Left.Y <= .33)
                && currentDepth > -1)
            {
                currentDepth--;
            }

            startLoc.Y = (float)((float)(Game1.screenHeight / 1.33) + Game1.screenHeight * (currentDepth * .11));
            loc.Y = startLoc.Y;

            animation.Update(gameTime, startLoc);

            prevGamePadState = currGamePadState;
            prevKeyState = currKeyState;
        }

        public void BattleUpdate(GameTime gameTime)
        {
            float currentHealthPercent = secondaryStats.health / (float)secondaryStats.maxHealth;

            currentHealthBar.Width = (int)(maxHealthBarLength * currentHealthPercent);
            missingHealthBar.Width = maxHealthBarLength - currentHealthBar.Width;

            missingHealthBar.X = currentHealthBar.Right;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch);

            spriteBatch.Draw(healthBarTexture, currentHealthBar, Color.Green);
            spriteBatch.Draw(healthBarTexture, missingHealthBar, Color.Red);
        }
    }
}
