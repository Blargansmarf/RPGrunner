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
        SpriteAnimation walkAnimation, attackAnimation;
        Texture2D walkSheet, attackSheet;
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
            public float resistance, dodge, criticalChance, criticalBonus, atkSpeed;
        }

        public PStats primaryStats;
        public SStats secondaryStats;

        public float currentSpeed;

        public int currentDepth;
        public int attackNum;

        public enum PlayerState { walking, attacking };

        public PlayerState pState;

        public Player(GraphicsDeviceManager g, ContentManager c)
        {
            graphics = g;
            content = c;

            Initialize();
        }
 
        public void Initialize()
        {
            LoadContent();

            pState = PlayerState.walking;

            currentDepth = 0;
            currentSpeed = Game1.screenWidth / 500;

            primaryStats = new PStats();
            secondaryStats = new SStats();

            secondaryStats.maxHealth = 500;
            secondaryStats.health = 500;
            secondaryStats.attack = 20;
            secondaryStats.atkSpeed = 1;

            attackNum = 0;

            playerDimensions = new Vector2(60, 90);

            startLoc = new Vector2(Game1.screenWidth / 10, (float)(Game1.screenHeight / 1.33)) - playerDimensions/2;

            maxHealthBarLength = (int)Game1.screenWidth / 5;

            currentHealthBar = new Rectangle((int)Game1.screenWidth/20, (int)Game1.screenHeight/20,
                (int)Game1.screenWidth/5, (int)Game1.screenHeight/20);
            missingHealthBar = new Rectangle(currentHealthBar.Right, currentHealthBar.Y, 0, currentHealthBar.Height); 

            loc = startLoc;

            walkAnimation = new SpriteAnimation(4, walkSheet, .3, new Rectangle((int)startLoc.X, (int)startLoc.Y,
                                                                (int)playerDimensions.X, (int)playerDimensions.Y));
            attackAnimation = new SpriteAnimation(2, attackSheet, secondaryStats.atkSpeed, new Rectangle((int)startLoc.X, (int)startLoc.Y,
                                                                (int)playerDimensions.X, (int)playerDimensions.Y));
        }

        public void LoadContent()
        {
            walkSheet = content.Load<Texture2D>("Textures/HeroWalk(IP)");
            attackSheet = content.Load<Texture2D>("Textures/TestSpriteAttack");
            healthBarTexture = content.Load<Texture2D>("Textures/WhiteSquare");
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

            startLoc.Y = (float)((float)(Game1.screenHeight / 1.33) + Game1.screenHeight * (currentDepth * .11)) - playerDimensions.Y / 2;
            loc.Y = startLoc.Y;

            walkAnimation.Update(gameTime, new Rectangle((int)startLoc.X, (int)startLoc.Y,
                                                                (int)playerDimensions.X, (int)playerDimensions.Y));
            attackAnimation.Update(gameTime, new Rectangle((int)startLoc.X, (int)startLoc.Y,
                                                                (int)playerDimensions.X, (int)playerDimensions.Y));
            attackAnimation.Reset();

            prevGamePadState = currGamePadState;
            prevKeyState = currKeyState;
        }

        public bool BattleUpdate(GameTime gameTime)
        {
            float currentHealthPercent = secondaryStats.health / (float)secondaryStats.maxHealth;

            currentHealthBar.Width = (int)(maxHealthBarLength * currentHealthPercent);
            missingHealthBar.Width = maxHealthBarLength - currentHealthBar.Width;

            missingHealthBar.X = currentHealthBar.Right;

            attackAnimation.Update(gameTime, new Rectangle((int)startLoc.X, (int)startLoc.Y,
                                                                (int)playerDimensions.X, (int)playerDimensions.Y));

            return secondaryStats.health > 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(pState == PlayerState.walking)
                walkAnimation.Draw(spriteBatch);

            if (pState == PlayerState.attacking)
                attackAnimation.Draw(spriteBatch);

            spriteBatch.Draw(healthBarTexture, currentHealthBar, Color.Green);
            spriteBatch.Draw(healthBarTexture, missingHealthBar, Color.Red);
        }

        public void MenuDraw(SpriteBatch spriteBatch, int depth)
        {

        }

        public void ResetAnimations()
        {
            walkAnimation.Reset();
            attackAnimation.Reset();
        }
    }
}
