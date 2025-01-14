using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace BartGame
{
    class KrustyOs : Enemy
    {
        public enum State { Normal, Breaking };
        public State state { get; set; }
        public State previousState;
        public bool Touched;
        private List<JaggedMetal> nuMetalList;
        private FrameSelector NormalFrame, BreakAnim, BreakingAnim, BrokeFrame;
        private float StateTimer, AnimationTimer, VanishTimer;
        public int StateSwitcher = 0;
        private static int Height = 31;
        private static int Width = 28;
        private static int StartX = 323;
        private static int StartY = 81;
        private float spawnTimer = 0;

        public KrustyOs(int x, int y, List<JaggedMetal> metalList)
        {
            name = "krustyos";
            positionRectangle = new Rectangle(x, y, Width, Height);
            state = State.Normal;
            Touched = false;
            AnimationTimer = 0;
            VanishTimer = 0;
            Initialize();
            nuMetalList = new List<JaggedMetal>();
            foreach (JaggedMetal s in metalList)
                nuMetalList.Add(s);

        }

        private void Initialize()
        {
            int i;
            //running
            List<Rectangle> Normal = new List<Rectangle>();
            Normal.Add(new Rectangle(StartX, StartY, Width, Height));
            NormalFrame = new FrameSelector(0.02f, Normal);
            //Squatting
            List<Rectangle> ShockList = new List<Rectangle>();
            for (i = 0; i < 4; i++) // id = 1, 2, 3
            {
                ShockList.Add(new Rectangle((StartX) + Width + (Width * (i)), StartY, Width, Height));
            }
            BreakAnim = new FrameSelector(0.1f, ShockList);
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
                case State.Breaking:
                    {
                        StateTimer += 0.01f;

                        return BreakAnim.GetFrame(ref StateTimer);
                    }
                default: return new Rectangle();
            }
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            previousState = state;

            if (StateSwitcher < 1)
                state = State.Normal;
            else
                state = State.Breaking;

            if (state == State.Breaking)
            {
                canNotCollide = true;
                MetalSpawn();
                VanishTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (VanishTimer >= 0.5)
                    canRemove = true;
                Velocity.Y = 0; 
            }


            Sprite s;
            positionRectangle.X += (int)Velocity.X;
            s = CheckCollision(sprites);
            if (state != State.Breaking)
            {
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

        }
        private void MetalSpawn()
        {
            foreach (JaggedMetal metal in nuMetalList)
            {

                if (metal.state == JaggedMetal.State.Despawned)
                {
                    metal.position.X = positionRectangle.X + 6;
                    metal.position.Y = positionRectangle.Y;
                    metal.NeedBoost = true;
                    metal.state = JaggedMetal.State.Normal;
                }
                if (metal.state == JaggedMetal.State.Normal)
                {
                }
            }
        }


        //END OF UPDATE
        public override void XCollision(Sprite s)
        {
            switch (s.name)
            {
                case "jaggedmetal":

                    StateSwitcher++;

                    break;
                default:
                    if (positionRectangle.Left < s.positionRectangle.Left)
                    {
                        Velocity.X = 0;

                    }
                    else if (positionRectangle.Right > s.positionRectangle.Right)
                    {
                        Velocity.X = 0;
                    }

                    break;
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
                Debug.WriteLine(name + state.ToString());
        }

    }
}
