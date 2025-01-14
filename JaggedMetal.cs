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
    class JaggedMetal : Enemy
    {
        public enum State { Normal, Despawned };
        public State state { get; set; }
        public State previousState;
        public bool Touched;
        public bool NeedBoost;
        private FrameSelector DespawnFrame, NormalAnim;
        private float StateTimer, AnimationTimer, JumpTimer;
        private float xSpeed, ySpeed;
        public int StateSwitcher = 0;
        private int StartFrame;
        private static int Height = 16;
        private static int Width = 16;
        private static int StartX = 320;
        private static int StartY = 112;
        private float spawnTimer = 0;

        public JaggedMetal(int x, int y, float velx = 0, float yBoost = 0, bool midFrame = false, bool movingRight = false)
        {
            name = "jaggedmetal";
            positionRectangle = new Rectangle(x, y, Width, Height);
            state = State.Despawned;
            AnimationTimer = 0;
            if (midFrame)
                StartFrame = 2;
            else
                StartFrame = 0;
            position = new Vector2(x, y);
            this.movingRight = movingRight;
            xSpeed = velx /4;
            ySpeed = yBoost;
            Initialize();
            canNotCollide = true;
        }

        private void Initialize()
        {
            int i;
            List<Rectangle> Despawn = new List<Rectangle>();
            Despawn.Add(new Rectangle(StartX + (Width * 4), StartY, Width, Height));
            DespawnFrame = new FrameSelector(0.02f, Despawn);
            //running
            List<Rectangle> Normal = new List<Rectangle>();
            for (i = 0; i < 4; i++) // id = 1, 2, 3
            {
                Normal.Add(new Rectangle(StartX + (Width * i), StartY, Width, Height));
            }
            NormalAnim = new FrameSelector(0.02f, Normal);
            NormalAnim.curFrameID = StartFrame;
        }
        public Rectangle GetFrame()
        {
            switch (state)
            {
                case State.Despawned:
                    {
                        return DespawnFrame.GetFrame(ref StateTimer);
                    }
                case State.Normal:
                    {
                        StateTimer += 0.01f;
                        return NormalAnim.GetFrame(ref StateTimer);
                    }
                default: return new Rectangle();
            }
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {

            if (NeedBoost)
            {
                Velocity.Y = -ySpeed;
                NeedBoost = false;
            }

            if (state == State.Normal)
            {
                canNotCollide = false;
                Velocity.Y += 0.2f;

                if (movingRight)
                {
                    Velocity.X = xSpeed;
                }
                else
                {

                    Velocity.X = -xSpeed;
                }
                Sprite s = null;
                position.Y += Velocity.Y;
                position.X += Velocity.X;
                BoundBox();
                s = CheckCollision(sprites);
                if (s != null) XCollision(s);
                if (s != null) YCollision(s);
                BoundBox();

            }
            //Sprite s;
            //positionRectangle.X += (int)Velocity.X;
            //s = CheckCollision(sprites);
            //if (s != null) XCollision(s);
            //else
            //{
            //    positionRectangle.X -= (int)Velocity.X;
            //    positionRectangle.Y += (int)Velocity.Y;
            //    s = CheckCollision(sprites);
            //    if (s != null) YCollision(s);
            //    else
            //    {
            //        positionRectangle.Y += 1;
            //        s = CheckCollision(sprites);
            //        if (s == null) Velocity.Y += 1;
            //        positionRectangle.Y -= 1;
            //    }
            //}
            //positionRectangle.X += (int)Velocity.X;
        }

        private void BoundBox()
        {
            positionRectangle = new Rectangle((int)position.X, (int)position.Y, Width, Height);
        }


        //END OF UPDATE
        public override void XCollision(Sprite s)
        {
            switch (s.name)
            {
                case "gps":
                    //Velocity.Y -= 2;
                    break;
                case "krustyos":

                    ((KrustyOs)s).StateSwitcher++;


                    break;
                default:
                    break;
            }
        }
        public override void YCollision(Sprite s)
        {
            switch (s.name)
            {
                case "gps":
                    //Velocity.Y -= 2;
                    break;
                case "krustyos":

                    ((KrustyOs)s).StateSwitcher++;

                    break;
                default:
                    break;
            }
        }
        //public override void YCollision(Sprite s)
        //{
        //    if (positionRectangle.Bottom > s.positionRectangle.Top)
        //    {
        //        positionRectangle.Y = s.positionRectangle.Top - positionRectangle.Height;
        //        Velocity.Y = 0;
        //    }
        //}


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, GetFrame(), Color.White);
        }

    }
}