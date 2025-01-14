using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.IO;

namespace BartGame
{
    class Map1_2 : Map
    {
        int tileSize = Constants.tileSize;
        public Map1_2()
        {

            InitSprites();
            end = 3300;
        }

        public override void LoadContent(ContentManager content)
        {
            background = content.Load<Texture2D>("Map1_1");
            Player.LoadContent(content);
            Pellet.LoadContent(content);
            Tile.LoadContent(content);
            Enemy.LoadContent(content);
            Item.LoadContent(content);

        }


        public override void InitSprites()
        {
            mapRectangle = new Rectangle(0, 0, 3392, 224);
            player = new Player(80, 80);
            sprites = new List<Sprite>();
            enemies = new List<Enemy>();
            bricks = new List<Brick>();


            sprites.Add(player);


            #region staticSprite
            //ground
            sprites.Add(new Sprite(new Rectangle(0, 200, 3000, 24)));
            //block for collision test
            //sprites.Add(new Sprite(new Rectangle(150, 168, 16, 32)));
            //sprites.Add(new Sprite(new Rectangle(0, 168, 16, 32)));
            sprites.Add(new Sprite(new Rectangle(0, 0, 16, 200)));
            sprites.Add(new Sprite(new Rectangle(0, -16, 500, 16)));
            //AddBrick(100, 108);
            //AddBrick(116, 108);
            //AddBrick(132, 108);            
            //sprites.Add(new Sprite(new Rectangle(230, 168, 16, 32)));
            //sprites.Add(new Sprite(new Rectangle(440, 150, 16, 200)));
            //sprites.Add(new Sprite(new Rectangle(530, 168, 16, 32)));
            //sprites.Add(new Sprite(new Rectangle(2800, 168, 16, 32)));
            //Items
            //Item butterFinger = new ButterFinger(130, 135);
            //sprites.Add(butterFinger);
            sprites.Add(new ButterFinger(234, 106));
            //sprites.Add(new Sparkles(180, 106));
            sprites.Add(new Squishee(180, 106));
            sprites.Add(new Spraypaint(200, 106));
            sprites.Add(new ButterFinger(79 * tileSize - 4,  6 * tileSize));
            //Enemies
            //sprites.Add(new Platform(2, 2));

            enemies.Add(new MoeKnife(53 * tileSize, 9 * tileSize));

            //enemies.Add(new ElfWeird(260, 142, 0, false));
            //enemies.Add(new ElfWeird(21 * tileSize, 12 * tileSize, 0, false, false));
            //enemies.Add(new ElfWeird(264, 142, 0, false, false));
            //enemies.Add(new ElfWeird(282, 142, 0, false, false));
            //enemies.Add(new ElfWeird(250, 142, 1));
            //enemies.Add(new ElfWeird(270, 142, 2));
            //enemies.Add(new Cupcake(200, 185));    
            //enemies.Add(new Cupcake(200, 185));
            enemies.Add(new FallingSpike(12 * tileSize, 0, player));
            enemies.Add(new FallingSpike(13 * tileSize, 0, player));
            enemies.Add(new FakeoutSpike(14 * tileSize, 0, player));
            enemies.Add(new FakeoutSpike(31 * tileSize, 0, player));
            enemies.Add(new FakeoutSpike(32 * tileSize, 0, player));
            enemies.Add(new FakeoutSpike(33 * tileSize, 0, player));
            //enemies.Add(new Olmec(200, 0));
            //enemies.Add(new JaggedMetal(120, 182));
            //AddKrustyOs(72 * tileSize, 8 * tileSize - 8, 1, enemies);
            //AddKrustyOs(70 * tileSize, 11 * tileSize - 8, 1, enemies);
            //AddKrustyOs(74 * tileSize, 11 * tileSize - 8, 1, enemies);
            //AddKrustyOs(180, 169, 1, enemies);
            //AddKrustyOs(218, 169, 1, enemies);

            //enemies.Add(new JaggedMetal(120, 182));

            AddElfGroup(37 * tileSize, 6 * tileSize - 8, 3, enemies);
            //AddElfGroup(100, 142, 6, enemies);
            //AddElfGroup(100, 142, 3, enemies);
            AddTiles();

            #endregion

        }


        private void AddTiles()
        {
            StreamReader reader = new("C:\\Users\\belac\\Desktop\\Game\\BartGame\\Content\\" +
                                        "Data/level1_mg.csv");
            int y = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {

                string[] items = line.Split(',');

                for (int x = 0; x < items.Length; x++)
                {
                    if (int.TryParse(items[x], out int value))
                    {
                        if (value > -1 && value != 9)
                        {
                            var platform = new Platform(x, y, value);
                            sprites.Add(platform);
                        }
                        if (value == 9)
                        {
                            AddBrick(x, y);
                        }
                    }
                }

                y++;

            }
        }
    }
}
