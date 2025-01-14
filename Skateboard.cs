using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BartGame
{
    public class Skateboard : Sprite
    {
        public Skateboard()
        {
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            base.Update(gameTime, sprites);
        }
        public override Sprite CheckCollision(List<Sprite> sprites)
        {
            return base.CheckCollision(sprites);
        }

        public override void XCollision(Sprite s)
        {
            base.XCollision(s);
        }
        public override void YCollision(Sprite s)
        {
            base.YCollision(s);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }


    }
}
