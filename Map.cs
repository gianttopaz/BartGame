using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using static BartGame.Player;

namespace BartGame
{
    class Map
    {
        protected Texture2D background;
        protected Rectangle mapRectangle;
        protected List<Sprite> sprites;
        protected Player player;
        protected List<Enemy> enemies;
        protected List<Brick> bricks;
        protected int end;
        private float i;
        private int j = 0;
        private int position = 0;
        private int enemyPosition = 0;
        private KeyboardState currentKey, prevKey;
        public virtual void LoadContent(ContentManager content)
        {
        }
        public virtual void InitSprites()
        {

        }

        public virtual void Update(Camera camera, GameTime gameTime)
        {
            camera.updateLookPosition(player.positionRectangle);
            prevKey = currentKey;
            currentKey = Keyboard.GetState();

            if ((player.ShotPower != Player.SlingshotStrength.None) && prevKey.IsKeyDown(Keys.T) && !currentKey.IsKeyDown(Keys.T))
            {
                AddPellet(gameTime, player);
                player.ReadyFire = false;
            }
            foreach (Brick s in bricks)
            {

                if (s.justDestroyed)
                {
                    AddDebris((int)s.position.X, (int)s.position.Y);
                }

            }
            if (player.power == Power.Squishee && !player.Sparkled)
            {
                AddSparkles(gameTime, player);
                player.Sparkled = true;
            }
            foreach (Sprite s in sprites)
            {
                s.Update(gameTime, sprites);
            }
            //if (player.positionRectangle.X < camera.leftRectrict) player.positionRectangle.X = camera.leftRectrict;
            if (player.positionRectangle.X > end)
            {
                if (sprites.Contains(player))
                {
                    player.canRemove = true;
                }
            }

            RemoveNoLongerNeededSprite();
            ActiveEnemies(camera);
        }
        public static Sprite AddElf(int x, int y, int z, List<Enemy> enemies)
        {
            ElfWeird newElf;
            newElf = new ElfWeird(x, y, z);
            enemies.Add(newElf);
            return newElf;
        }
        public void AddDebris(int x = 100, int y = 80)
        {
            int thisY = y;
            int thisX = x;
            sprites.Add(new BrDebris(thisX, thisY + 8, (float)1.5, 1, false));
            sprites.Add(new BrDebris(thisX + 8, thisY, 1, 2, true));
            sprites.Add(new BrDebris(thisX, thisY, 1, 2, false));
            sprites.Add(new BrDebris(thisX + 8, thisY + 8, (float)1.5, 1, true));
        }
        public void AddBrick(int x, int y)
        {
            Brick brick = new Brick(x * Constants.tileSize, (y * Constants.tileSize) - 8);
            bricks.Add(brick);
            sprites.Add(brick);
        }

        public static void AddElfGroup(int x, int y, int v, List<Enemy> enemies)
        {

            List<Sprite> elfList = new();
            int i = 0;
            int a = x + (16 - 8);
            int b = y + (16 - 12);
            
            for (i = 0; i < v; i++) // id = 1, 2, 3
            {
                Sprite s;
                s = AddElf(a, b, i, enemies);
                elfList.Add(s);
            }
            enemies.Add(new TV(x, y, elfList));

        }
        public static JaggedMetal AddJaggedMetal(int x, int y, float velx, float yBoost, bool varySpin, bool movingRight, List<Enemy> enemies)
        {
            JaggedMetal nuMetal;
            nuMetal = new JaggedMetal(x, y, velx, yBoost, varySpin, movingRight);
            enemies.Add(nuMetal);
            return nuMetal;
        }

        public static void AddKrustyOs(int x, int y, int speed, List<Enemy> enemies, bool four = true)
        {
            List<JaggedMetal> metalList = new();
            int i = 0;
            int a = x + (4);
            int b = y + (16 - 8);
            float velx = speed;
            int yBoost = speed * 6;
            int count;

            if (four)
                count = 4;
            else
                count = 2;
            bool right;
            bool varySpin;
            for (i = 0; i < count; i++) // id = 1, 2, 3
            {
                if (i % 2 == 1)
                {
                    velx += 2;
                    right = false;
                    varySpin = false;
                }
                else
                {
                    yBoost -= 1;
                    right = true;
                    varySpin = true;
                }

                JaggedMetal s;
                s = AddJaggedMetal(a, b, velx, yBoost, varySpin, right, enemies);
                metalList.Add(s);
            }
            enemies.Add(new KrustyOs(x, y, metalList));

        }
        private void AddPellet(GameTime gameTime, Player player)
        {
            if (player.ShotPower == Player.SlingshotStrength.Low || player.ShotPower == Player.SlingshotStrength.Low)
                i = 1;

            if (player.ShotPower == Player.SlingshotStrength.Mid)
                i = 1.5f;

            if (player.ShotPower == Player.SlingshotStrength.Full)
                i = 4;

            Pellet newPellet;
            if (player.movingRight) newPellet = new Pellet(player.positionRectangle.Right, player.positionRectangle.Center.Y + 1, i, gameTime, true);
            else newPellet = new Pellet(player.positionRectangle.Left, player.positionRectangle.Center.Y + 1, i, gameTime, false);
            sprites.Add(newPellet);
        }
        private void AddSparkles(GameTime gameTime, Player player)
        {
            sprites.Add(new Sparkles(player.positionRectangle.X,player.positionRectangle.Y, player));
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, mapRectangle, Color.White);
            foreach (Sprite s in sprites)
            {
                s.Draw(gameTime, spriteBatch);
            }
        }
        public void RemoveNoLongerNeededSprite()
        {
            List<Sprite> toRemove = new List<Sprite>();
            Rectangle allowRectangle = new Rectangle(mapRectangle.X, mapRectangle.Y - 100, mapRectangle.Width, mapRectangle.Height + 200);
            foreach (Sprite s in sprites)
            {
                if (s.canRemove) toRemove.Add(s);
                if (Rectangle.Intersect(s.positionRectangle, allowRectangle) == Rectangle.Empty)
                {
                    toRemove.Add(s);
                }
            }
            foreach (Sprite s in toRemove)
            {
                sprites.Remove(s);
                Enemy e = s as Enemy;
                if (e != null) enemies.Remove(e);
            }
        }
        public void ActiveEnemies(Camera camera)
        {
            foreach (Enemy e in enemies)
            {
                if (!sprites.Contains(e))
                {
                    if ((e.position.X - player.positionRectangle.X) < (Constants.viewWidth / camera.zoom * 1.1))
                    {
                        e.active = true;
                        sprites.Add(e);
                    }
                }
                //if player is far from both the enemy and the enemy's start position
                //reset enemy's position to start position
                //if (e.positionRectangle.X - player.positionRectangle.X < Constants.viewWidth / camera.zoom * 1.1)
                //    { e.active = false;}

            }
        }

    }
}