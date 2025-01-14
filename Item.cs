using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BartGame
{
    public class Item : Sprite
    {
        protected static Texture2D texture;

        public Item()
        {
        }
        public static void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Items");
        }

    }
}
