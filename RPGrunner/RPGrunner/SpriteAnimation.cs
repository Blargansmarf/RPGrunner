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
    class SpriteAnimation
    {
        int cells, currentCell;
        Texture2D texture;
        double totalTime;
        Rectangle source;
        Rectangle destination;
        GameTime lastFrameTime;

        public SpriteAnimation(int num, Texture2D tex, double time, Rectangle dest)
        {
            cells = num;
            texture = tex;
            totalTime = time;
            destination = dest;
            source = new Rectangle(0, 0, tex.Width / cells, tex.Height);
            currentCell = 0;
            lastFrameTime = new GameTime();
        }

        public void Update(GameTime currentGameTime, Rectangle dest)
        {
            if (currentGameTime.TotalGameTime.TotalSeconds -
                lastFrameTime.TotalGameTime.TotalSeconds >= totalTime / cells)
            {
                lastFrameTime = new GameTime(currentGameTime.TotalGameTime, currentGameTime.ElapsedGameTime);
                currentCell++;
                if (currentCell == cells)
                    currentCell = 0;
            }

            source.X = source.Width * currentCell;

            destination = dest;
        }

        public void Reset()
        {
            source = new Rectangle(0, 0, texture.Width / cells, texture.Height);
            currentCell = 0;
            lastFrameTime = new GameTime();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, destination, source, Color.White);
        }
    }
}
