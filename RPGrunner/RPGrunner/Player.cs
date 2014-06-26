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
        SpriteAnimation walkAnimation, attackAnimation, armWalkAnimation;
        Texture2D walkSheet, attackSheet, armWalkSheet;
        public Vector2 startLoc, loc, armLoc;

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
            public int attack, defense, health, mana, maxHealth, maxMana;
            public float resistance, dodge, atkSpeed;
        }

        public PStats primaryStats;
        public SStats secondaryStats;

        public Item currentItem;

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

            primaryStats.vitality = 10;
            primaryStats.strength = 10;
            primaryStats.dexterity = 10;
            primaryStats.intelligence = 10;

            secondaryStats.maxHealth = 20 + primaryStats.vitality * 10;
            secondaryStats.health = secondaryStats.maxHealth;
            secondaryStats.maxMana = 20 + primaryStats.intelligence * 10;
            secondaryStats.mana = secondaryStats.maxMana;

            currentItem = new Item(graphics, content, 0);

            secondaryStats.attack = currentItem.CalculateNextAttack(primaryStats.strength,
                primaryStats.intelligence, primaryStats.dexterity);
            secondaryStats.atkSpeed = currentItem.CalculateAtkSpd(primaryStats.dexterity);

            attackNum = 0;

            playerDimensions = new Vector2(60, 90);

            startLoc = new Vector2(Game1.screenWidth / 10, (float)(Game1.screenHeight / 1.33)) - playerDimensions/2;
            armLoc = startLoc;
            armLoc.X -= playerDimensions.X / 2;

            maxHealthBarLength = (int)Game1.screenWidth / 5;

            currentHealthBar = new Rectangle((int)Game1.screenWidth/20, (int)Game1.screenHeight/20,
                (int)Game1.screenWidth/5, (int)Game1.screenHeight/20);
            missingHealthBar = new Rectangle(currentHealthBar.Right, currentHealthBar.Y, 0, currentHealthBar.Height); 

            loc = startLoc;

            walkAnimation = new SpriteAnimation(8, walkSheet, .45, new Rectangle((int)startLoc.X, (int)startLoc.Y,
                                                                (int)playerDimensions.X, (int)playerDimensions.Y));
            armWalkAnimation = new SpriteAnimation(8, armWalkSheet, .45, new Rectangle((int)armLoc.X, (int)armLoc.Y,
                                                                (int)(playerDimensions.X * 1.5), (int)playerDimensions.Y));
            attackAnimation = new SpriteAnimation(2, attackSheet, secondaryStats.atkSpeed, new Rectangle((int)startLoc.X, (int)startLoc.Y,
                                                                (int)playerDimensions.X, (int)playerDimensions.Y));
        }
        
        public void UpdateAttack()
        {
            secondaryStats.attack = currentItem.CalculateNextAttack(primaryStats.strength,
                primaryStats.intelligence, primaryStats.dexterity);
        }

        public void UpdateStats()
        {
            secondaryStats.maxHealth = 20 + (primaryStats.vitality + currentItem.vitality) * 10;
            secondaryStats.health = secondaryStats.maxHealth;
            secondaryStats.maxMana = 20 + (primaryStats.intelligence + currentItem.intelligence) * 10;
            secondaryStats.mana = secondaryStats.maxMana;

            secondaryStats.atkSpeed = currentItem.CalculateAtkSpd(primaryStats.dexterity);
        }

        public void LoadContent()
        {
            walkSheet = content.Load<Texture2D>("Textures/HeroWalk");
            armWalkSheet = content.Load<Texture2D>("Textures/HeroArmWalk");
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

            armLoc.Y = loc.Y = startLoc.Y = (float)((float)(Game1.screenHeight / 1.33) + Game1.screenHeight * (currentDepth * .11)) - playerDimensions.Y / 2;

            walkAnimation.Update(gameTime, new Rectangle((int)startLoc.X, (int)startLoc.Y,
                                                                (int)playerDimensions.X, (int)playerDimensions.Y));
            armWalkAnimation.Update(gameTime, new Rectangle((int)armLoc.X, (int)armLoc.Y,
                                                                (int)(playerDimensions.X * 1.5), (int)playerDimensions.Y));
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
            if (pState == PlayerState.walking)
            {
                walkAnimation.Draw(spriteBatch);
                armWalkAnimation.Draw(spriteBatch);
            }

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
            armWalkAnimation.Reset();
            attackAnimation.Reset();
        }
    }
}
