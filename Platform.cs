using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BartGame
{
    public class Platform : Tile
    {
        private static int Height = 16;
        private static int Width = 16;
        private int sourceX;
        private int sourceY;
        private int num_tiles_per_row = 14;

        public Platform(int x, int y, int value = 0)
        {
            name = "gps";
            positionRectangle = new Rectangle(x * Constants.tileSize, y * Constants.tileSize - 8, Width, Height);
            position = new Vector2(x * Constants.tileSize, y * Constants.tileSize - 8);
            Velocity.X = 0; Velocity.Y = 0;
            this.sourceX = value % num_tiles_per_row;
            this.sourceY = value / num_tiles_per_row;
            sourceRectangle = new Rectangle(sourceX * Constants.tileSize, sourceY * Constants.tileSize, Width, Height);
            canNotCollide = false;
            if (value >= 4 && value <= 7)
                name = "platform";
            if (value >= 47)
                name = "spike";
        }


        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, sourceRectangle, Color.White);
            //Debug.WriteLine(sourceY.ToString());
        }
    }
}
