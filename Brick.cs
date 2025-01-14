using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace BartGame
{
    public class Brick : Tile
    {
        private static int Height = 16;
        private static int Width = 16;
        private int sourceX = 9;
        private int sourceY = 0;
        public bool justDestroyed = false;
        public bool broken = false;
        public bool wasBroken = false;
        private float removeTimer = 0;
        public Brick(int x, int y)
        {
            name = "brick";
            positionRectangle = new Rectangle(x , y, Width, Height);
            position = new Vector2(x , y);
            //positionRectangle = new Rectangle(x * Constants.tileSize, y * Constants.tileSize - 8, Width, Height);
            //position = new Vector2(x * Constants.tileSize, y * Constants.tileSize - 8);
            Velocity.X = 0; Velocity.Y = 0;
            sourceRectangle = new Rectangle(sourceX * Constants.tileSize, sourceY * Constants.tileSize, Width, Height);
            canNotCollide = false;
            canRemove = false;
        }


        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            if (broken && !wasBroken)
                justDestroyed = true;
            else 
                justDestroyed = false;
            wasBroken = broken;
            if (broken)
            {
                removeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (removeTimer > 0.05)
                    canRemove = true;
            }

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, sourceRectangle, Color.White);
            //Debug.WriteLine(sourceY.ToString());
        }
    }
}