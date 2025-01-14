using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BartGame
{
    public abstract class Component
    {

        public abstract void Update(GameTime gameTime, List<Sprite> sprites);
        public abstract void Update(GameTime gameTime, List<Sprite> sprites, List<Sprite> elfList);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    }
}
