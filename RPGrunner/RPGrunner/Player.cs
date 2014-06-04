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
        Vector2 startLoc, loc;

        Vector2 playerDimensions;

        GraphicsDeviceManager graphics;
        ContentManager content;

        KeyboardState prevKeyState, currKeyState;
        GamePadState prevGamePadState, currGamePadState;

        public float currentSpeed;

        int currentDepth;

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
            currentSpeed = 1;

            playerDimensions = new Vector2(10, 10);

            startLoc = new Vector2(Game1.screenWidth / 10, (float)(Game1.screenHeight / 1.33));

            loc = startLoc;

            animation = new SpriteAnimation(3, spriteSheet, 1, startLoc);
        }

        public void LoadContent()
        {
            spriteSheet = content.Load<Texture2D>("AnimationTest");
        }

        public void Update(GameTime gameTime)
        {
            currGamePadState = GamePad.GetState(PlayerIndex.One);
            currKeyState = Keyboard.GetState();

            loc.X += currentSpeed;

            if ((currKeyState.IsKeyDown(Keys.Space) && prevKeyState.IsKeyDown(Keys.Space)
                || currGamePadState.IsButtonDown(Buttons.A) && prevGamePadState.IsButtonUp(Buttons.A)))
            {

            }

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

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch);
        }
    }
}
