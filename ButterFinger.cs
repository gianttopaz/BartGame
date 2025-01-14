using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BartGame
{
    public class ButterFinger : Item
    {
        private static int Height = 10;
        private static int Width = 24;
        private float StateTimer;
        private float SwitchTimer;
        private float StartPosition;
        private int x;

        public ButterFinger(int x, int y)
        {

            name = "butterfinger";
            positionRectangle = new Rectangle(x, y, Width, Height);
            sourceRectangle = new Rectangle(0, 0, Width, Height);
            position = new Vector2(x, y);

            StartPosition = y;
            position = new Vector2(x, y);
            Velocity.X = 0; Velocity.Y = 0;
            this.x = 1;
        }


        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            StateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            SwitchTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (SwitchTimer >= 1.5)
            {
                x *= -1;
                SwitchTimer = 0;
            }
            if (StateTimer > 0.5) 
            {
                position.Y += 1 * x;
                StateTimer = 0;
            }
            position.Y = MathHelper.Clamp(position.Y, StartPosition, StartPosition + 2);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, sourceRectangle, Color.White);
        }
    }
}
