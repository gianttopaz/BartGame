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
using Microsoft.Xna.Framework.Input;

namespace BartGame
{
    class Sparkles : Item
    {
        private FrameSelector NormalAnim;
        private float StateTimer, RemoveTimer;
        private float xSpeed, ySpeed;
        private int StartFrame;
        private static int Height = 32;
        private static int Width = 16;
        private static int sourceY = 2 * tileSize;
        private static int sourceX = 0 * tileSize;
        Player player;

        public Sparkles(int x, int y, Player player = null)
        {
            name = "sparkles";
            positionRectangle = new Rectangle(x, y, Width, Height);
            RemoveTimer = 0;
            position = new Vector2(x, y);
            Initialize();
            canNotCollide = true;
            this.player = player;
        }

        private void Initialize()
        {
            //running
            List<Rectangle> Normal = new List<Rectangle>();
                Normal.Add(new Rectangle(sourceX + (0), sourceY, Width, Height));
                Normal.Add(new Rectangle(sourceX + (tileSize * 1), sourceY, Width, Height));
                Normal.Add(new Rectangle(sourceX + (tileSize * 2), sourceY, Width, Height));
                Normal.Add(new Rectangle(sourceX + (tileSize * 3), sourceY, Width, Height));
            NormalAnim = new FrameSelector(0.05f, Normal);
            NormalAnim.curFrameID = StartFrame;
        }
        public Rectangle GetFrame()
        {
            StateTimer += 0.01f;
            return NormalAnim.GetFrame(ref StateTimer);
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
                position.X = player.positionRectangle.X;
                position.Y = player.positionRectangle.Y;
                if (player.SquisheeTimer > 20)
                {
                    canRemove = true;
                }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, GetFrame(), Color.White);
        }

    }
}