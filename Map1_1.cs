using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;

namespace BartGame
{
    class Map1_1 : Map
    {
        public Map1_1()
        {

            mapRectangle = new Rectangle(0, 0, 3392, 224);
            player = new Player(50,50);
            sprites = new List<Sprite>();
            sprites.Add(player);

            InitSprites();
            end = 3276;
        }

        public override void LoadContent(ContentManager content)
        {
            background = content.Load<Texture2D>("map1_1");
            Player.LoadContent(content);
            Pellet.LoadContent(content);

        }
        private void InitSprites()
        {
            #region staticSprite
            //ground
            sprites.Add(new Sprite(new Rectangle(0, 201, 1103, 24)));
            sprites.Add(new Sprite(new Rectangle(1136, 201, 239, 24)));
            sprites.Add(new Sprite(new Rectangle(1424, 201, 1023, 24)));
            sprites.Add(new Sprite(new Rectangle(2480, 201, 912, 24)));
            //pipe
            sprites.Add(new Sprite(new Rectangle(448, 168, 32, 32)));
            sprites.Add(new Sprite(new Rectangle(608, 152, 32, 48)));
            sprites.Add(new Sprite(new Rectangle(736, 136, 32, 64)));
            sprites.Add(new Sprite(new Rectangle(912, 136, 32, 64)));
            sprites.Add(new Sprite(new Rectangle(2608, 168, 32, 32)));
            sprites.Add(new Sprite(new Rectangle(2864, 168, 32, 32)));
            //stair
            int i = 0;
            for (i = 0; i < 4; i++)
            {
                sprites.Add(new Sprite(new Rectangle(2144 + 16 * i, 184 - 16 * i, 16, 16 * (i + 1))));
            }

            sprites.Add(new Sprite(new Rectangle(2240, 136, 16, 64)));
            sprites.Add(new Sprite(new Rectangle(2256, 152, 16, 48)));
            sprites.Add(new Sprite(new Rectangle(2272, 168, 16, 32)));
            sprites.Add(new Sprite(new Rectangle(2288, 184, 16, 16)));

            sprites.Add(new Sprite(new Rectangle(2368, 184, 16, 16)));
            sprites.Add(new Sprite(new Rectangle(2384, 168, 16, 32)));
            sprites.Add(new Sprite(new Rectangle(2400, 152, 16, 48)));
            sprites.Add(new Sprite(new Rectangle(2416, 136, 32, 64)));

            sprites.Add(new Sprite(new Rectangle(2480, 136, 16, 64)));
            sprites.Add(new Sprite(new Rectangle(2496, 152, 16, 48)));
            sprites.Add(new Sprite(new Rectangle(2512, 168, 16, 32)));
            sprites.Add(new Sprite(new Rectangle(2528, 184, 16, 16)));

            for (i = 0; i < 7; i++)
            {
                sprites.Add(new Sprite(new Rectangle(2896 + 16 * i, 184 - 16 * i, 16, 16 * (i + 1))));
            }
            sprites.Add(new Sprite(new Rectangle(2896 + 16 * i, 184 - 16 * i, 32, 16 * (i + 1))));

            sprites.Add(new Sprite(new Rectangle(3168, 184, 16, 16)));

            #endregion

        }
    }
}
