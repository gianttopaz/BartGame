using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BartGame
{
    public class Cupcake : Enemy
    {
        public enum State { Normal, Shocking};
        public State state { get; set; }
        public State previousState;
        public bool Touched;
        private FrameSelector NormalFrame, ShockAnim, BreakingAnim, BrokeFrame;
        private float StateTimer, AnimationTimer, JumpTimer;
        public int StateSwitcher = 0;
        public static int CupcakeHeight = 15;
        public static int CupcakeWidth = 14;
        private static int CupcakeStartX = 144;
        private static int CupcakeStartY = 96;
        private float spawnTimer = 0;

        public Cupcake(int x, int y)
        {
            name = "cupcake";
            positionRectangle = new Rectangle(x, y, CupcakeWidth, CupcakeHeight);
            state = State.Normal;
            Touched = false;
            AnimationTimer = 0;
            Initialize();
        }

        private void Initialize()
        {
            int i;
            //running
            List<Rectangle> Normal = new List<Rectangle>();
            Normal.Add(new Rectangle(CupcakeStartX, CupcakeStartY, CupcakeWidth, CupcakeHeight));
            NormalFrame = new FrameSelector(0.02f, Normal);
            //Squatting
            List<Rectangle> CupcakeShockList = new List<Rectangle>();
            for (i = 0; i < 3; i++) // id = 1, 2, 3
            {
                CupcakeShockList.Add(new Rectangle((CupcakeStartX) + (CupcakeWidth * (i)), CupcakeStartY, CupcakeWidth, CupcakeHeight));
            }
            ShockAnim = new FrameSelector(0.02f, CupcakeShockList);
        }
        public Rectangle GetFrame()
        {
            switch (state)
            {
                case State.Normal:
                    {
                        StateTimer += 0.01f;
                        return NormalFrame.GetFrame(ref StateTimer);
                    }
                case State.Shocking:
                    {
                        StateTimer += 0.01f;

                        return ShockAnim.GetFrame(ref StateTimer);
                    }
                default: return new Rectangle();
            }
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            previousState = state;

            if ( !Touched && state != State.Normal)
            {
                AnimationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (AnimationTimer >= 0.5)
                {
                    state = State.Normal;
                    AnimationTimer = 0;
                }
            }


            Sprite s;
            positionRectangle.X += (int)Velocity.X;
            s = CheckCollision(sprites);
            if (s != null) XCollision(s);
            else
            {
                positionRectangle.X -= (int)Velocity.X;
                positionRectangle.Y += (int)Velocity.Y;
                s = CheckCollision(sprites);
                if (s != null) YCollision(s);
                else
                {
                    positionRectangle.Y += 1;
                    s = CheckCollision(sprites);
                    if (s == null) Velocity.Y += 1;
                    positionRectangle.Y -= 1;
                }
            }
            positionRectangle.X += (int)Velocity.X;
        }


        //END OF UPDATE
        public override void XCollision(Sprite s)
        {
            if (positionRectangle.Left < s.positionRectangle.Left)
            {
                positionRectangle.X = s.positionRectangle.Left - positionRectangle.Width;
                Velocity.X = 0;

            }
            else if (positionRectangle.Right > s.positionRectangle.Right)
            {
                positionRectangle.X = s.positionRectangle.Right;
                Velocity.X = 0;
            }
        }
        public override void YCollision(Sprite s)
        {
            if (positionRectangle.Bottom > s.positionRectangle.Top)
            {
                positionRectangle.Y = s.positionRectangle.Top - positionRectangle.Height;
                Velocity.Y = 0;
            }
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, positionRectangle, GetFrame(), Color.White);
            if (previousState != state)
                Debug.WriteLine("Cupcake " + state.ToString());
        }

    }

}
