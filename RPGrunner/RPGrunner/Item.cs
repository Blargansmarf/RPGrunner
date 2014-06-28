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
    class Item
    {
        GraphicsDeviceManager graphics;
        ContentManager content;

        SpriteAnimation animation;
        Texture2D itemSpriteSheet;
        Vector2 dimensions;
        Vector2 loc;

        List<string> buffs;

        public int vitality, dexterity, intelligence, strength;

        public float minDmg, maxDmg;
        public float atkSpd;
        float critChance, critBonus;

        enum StatType { intel, dex, str };
        public enum WeaponType { magic, pierce, slash, blunt };

        StatType statType;
        public WeaponType weaponType;

        Random random;

        public Item(GraphicsDeviceManager g, ContentManager c, int weapon, Vector2 startLoc, Vector2 playerDim)
        {
            graphics = g;
            content = c;

            dimensions = new Vector2(90, 120);
            loc = new Vector2(startLoc.X - playerDim.X / 2, startLoc.Y - playerDim.Y / 3);

            random = new Random();

            if (weapon == 0)
            {
                LoadContent("Textures/ShortSwordWalk");

                vitality = 0;
                dexterity = 0;
                intelligence = 0;
                strength = 0;

                critBonus = 0;
                critChance = 0;

                minDmg = 1f;
                maxDmg = 5f;

                statType = StatType.str;
                weaponType = WeaponType.slash;

                atkSpd = 1f;
            }

            animation = new SpriteAnimation(8, itemSpriteSheet, .45, new Rectangle((int)loc.X, (int)loc.Y,
                                                                      (int)dimensions.X, (int)dimensions.Y));
        }

        private void LoadContent(string sSheet)
        {
            itemSpriteSheet = content.Load<Texture2D>(sSheet);
        }

        public float CalculateAtkSpd(int dex)
        {
            return atkSpd - dex * .01f;
        }

        public int CalculateNextAttack(int str, int intel, int dex)
        {
            int atkDamage;

            critBonus = 2.0f + (str + strength) * .05f;
            critChance = (dex + dexterity) * .005f;

            if (statType == StatType.str)
            {
                atkDamage = (int)(minDmg + (maxDmg - minDmg) * (float)random.NextDouble());
                atkDamage += (int)((str + strength) * .5f);
                if (random.NextDouble() <= critChance)
                {
                    atkDamage = (int)(atkDamage * critBonus);
                }
                return atkDamage;
            }
            else if (statType == StatType.dex)
            {
                atkDamage = (int)(minDmg + (maxDmg - minDmg) * (float)random.NextDouble());
                atkDamage += (int)(dex * .5f);
                if (random.NextDouble() <= critChance)
                {
                    atkDamage = (int)(atkDamage * critBonus);
                }
                return atkDamage;
            }
            else
            {
                atkDamage = (int)(minDmg + (maxDmg - minDmg) * (float)random.NextDouble());
                atkDamage += (int)(intel * .5f);
                if (random.NextDouble() <= critChance)
                {
                    atkDamage = (int)(atkDamage * critBonus);
                }
                return atkDamage;
            }
        }

        public void UpdateLoc(GameTime gameTime, float yLoc)
        {
            animation.Update(gameTime, new Rectangle((int)loc.X, (int)yLoc, 
                    (int)dimensions.X, (int)dimensions.Y));
        }

        public void ResetAnimation()
        {
            animation.Reset();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch);
        }
    }
}
