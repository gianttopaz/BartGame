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
    class BrDebris : Tile
    {
        private FrameSelector NormalAnim;
        private float StateTimer, RemoveTimer;
        private float xSpeed, ySpeed;
        private int StartFrame;
        private static int Height = 8;
        private static int Width = 8;
        private int sourceY = 0 * Constants.tileSize;
        private int sourceX = 10 * Constants.tileSize;

        public BrDebris(int x, int y, float velx = 0, float yBoost = 0, bool movingRight = false, bool midFrame = false)
        {
            name = "brdebris";
            positionRectangle = new Rectangle(x, y, Width, Height);
            RemoveTimer = 0;
            if (midFrame)
                StartFrame = 2;
            else
                StartFrame = 0;
            position = new Vector2(x, y);
            this.movingRight = movingRight;
            xSpeed = velx;
            ySpeed = -3 - yBoost;
            Initialize();
            canNotCollide = true;
            Velocity.Y = ySpeed;
        }

        private void Initialize()
        {
            //running
            List<Rectangle> Normal = new List<Rectangle>();
                Normal.Add(new Rectangle(sourceX + (0), sourceY, Width, Height));
                Normal.Add(new Rectangle(sourceX + (8), sourceY, Width, Height));
                Normal.Add(new Rectangle(sourceX + (8), sourceY + 8, Width, Height));
                Normal.Add(new Rectangle(sourceX + (0), sourceY + 8, Width, Height));
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
            //if (Keyboard.GetState().IsKeyDown(Keys.D1))
            //{
                RemoveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                canNotCollide = false;
                Velocity.Y += 0.4f;

                if (movingRight)
                {
                    Velocity.X = xSpeed;
                }
                else
                {

                    Velocity.X = -xSpeed;
                }
                position.Y += Velocity.Y;
                position.X += Velocity.X;

                if (RemoveTimer > 0.5)
                    canRemove = true;

            //}


        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, GetFrame(), Color.White);
        }

    }
}