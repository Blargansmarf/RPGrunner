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
    class Enemy
    {
        public Vector2 loc;
        Texture2D spriteSheet;
        List <SpriteAnimation> animations;
        public int depth;
        Vector2 dimensions;

        Rectangle currentHealthBar, missingHealthBar;
        Texture2D healthBarTexture;

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

        GraphicsDeviceManager graphics;
        ContentManager content;

        public int attackNum;

        public Enemy(GraphicsDeviceManager g, ContentManager c)
        {
            graphics = g;
            content = c;
        }

        public void Initialize(Vector2 position, Vector2 dim, int dept)
        {
            healthBarTexture = content.Load<Texture2D>("WhiteSquare");

            dimensions = dim;
            primaryStats = new PStats();
            secondaryStats = new SStats();

            secondaryStats.maxHealth = 100;
            secondaryStats.health = 100;
            secondaryStats.attack = 15;
            secondaryStats.atkSpeed = 2;

            animations = new List<SpriteAnimation>();
            loc = position - dimensions/2;
            depth = dept;
            loc.Y += (float)(Game1.screenHeight * (depth * .11));

            attackNum = 0;

            currentHealthBar = new Rectangle((int)loc.X - (int)dimensions.X,
                (int)loc.Y - (int)dimensions.Y, (int)dimensions.X * 3, (int)dimensions.Y / 2);
            missingHealthBar = new Rectangle(currentHealthBar.Right, currentHealthBar.Y, 0, currentHealthBar.Height); 
        }

        public void AddAnimation(string spriteName, int num, double time)
        {
            spriteSheet = content.Load<Texture2D>(spriteName);

            SpriteAnimation anim = new SpriteAnimation(num, spriteSheet, time, loc);

            animations.Add(anim);
        }

        public void Update(GameTime gameTime)
        {
            foreach (SpriteAnimation anim in animations)
            {
                anim.Update(gameTime, loc);
            }
        }

        public void BattleUpdate(GameTime gameTime)
        {
            float currentHealthPercent = secondaryStats.health / (float)secondaryStats.maxHealth;

            currentHealthBar.Width = (int)(dimensions.X * 3 * currentHealthPercent);
            missingHealthBar.Width = (int)dimensions.X * 3 - currentHealthBar.Width;

            missingHealthBar.X = currentHealthBar.Right;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animations[0].Draw(spriteBatch);

            spriteBatch.Draw(healthBarTexture, currentHealthBar, Color.Green);
            spriteBatch.Draw(healthBarTexture, missingHealthBar, Color.Red);
        }
    }
}
