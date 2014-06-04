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
        Vector2 loc;
        Texture2D spriteSheet;
        List <SpriteAnimation> animations;
        int depth;

        GraphicsDeviceManager graphics;
        ContentManager content;

        public Enemy(GraphicsDeviceManager g, ContentManager c)
        {
            graphics = g;
            content = c;
        }

        public void Initialize(Vector2 position, Vector2 dim, int dept)
        {
            animations = new List<SpriteAnimation>();
            loc = position;
            depth = dept;
            loc.Y += (float)(Game1.screenHeight * (depth * .11));
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

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (SpriteAnimation anim in animations)
            {
                anim.Draw(spriteBatch);
            }
        }
    }
}
