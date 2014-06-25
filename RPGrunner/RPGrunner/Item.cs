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

        Texture2D itemSpriteSheet;
        Vector2 dimensions;
        Vector2 loc;

        List<string> buffs;

        public float minDmg, maxDmg;
        public float atkSpd;

        enum StatType { intel, dex, str };
        enum WeaponType { magic, pierce, slash, blunt };

        StatType statType;
        WeaponType weaponType;

        public Item(GraphicsDeviceManager g, ContentManager c, int weapon)
        {
            graphics = g;
            content = c;

            if (weapon == 0)
            {
                LoadContent("Sword");

                minDmg = 1f;
                maxDmg = 5f;

                statType = StatType.str;
                weaponType = WeaponType.slash;

                atkSpd = 1f;
            }
        }

        private void LoadContent(string sSheet)
        {
            itemSpriteSheet = content.Load<Texture2D>(sSheet);
        }
    }
}
