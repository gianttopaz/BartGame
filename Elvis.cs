using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BartGame
{
    class Elvis : Enemy
    {
        public enum State { Running, Spawning, Squatting, Falling, Jumping };
        public State state { get; set; }
        public State previousState;
        private FrameSelector Run, Squat, Jump;
        private List<Rectangle> SquatAnim;
        private int lastY, lastYframe;
        private int lastX, lastXframe;
        private float StateTimer, DropTimer, JumpTimer;
        public static int olmecHeight = 48;
        public static int olmecWidth = 48;
        private static int olmecStartX = Constants.tileSize * 0;
        private static int olmecStartY = Constants.tileSize * 6;

        public Elvis(int x, int y)
        {
            name = "elvis";
            positionRectangle = new Rectangle(x, y, olmecWidth, olmecHeight);
            state = State.Running;
            Initialize();
            JumpTimer = 0f;
        }

        private void Initialize()
        {
            int i;
            //running
            List<Rectangle> olmecRunList = new List<Rectangle>();
            for (i = 0; i < 3; i++) // id = 1, 2, 3
            {
                olmecRunList.Add(new Rectangle((olmecStartX) + (olmecWidth * (i)), olmecHeight, olmecWidth, olmecHeight));
            }
            olmecRunList.Add(new Rectangle((olmecStartX) + (olmecWidth * (1)), olmecHeight, olmecWidth, olmecHeight));
            Run = new FrameSelector(0.1f, olmecRunList);
            //Squatting
            List<Rectangle> olmecSquatList = new List<Rectangle>();
            olmecSquatList.Add(new Rectangle((olmecStartX) + (olmecWidth * (3)), olmecHeight, olmecWidth, olmecHeight));
            Squat = new FrameSelector(0.1f, olmecSquatList);
            //Jump/Hit
            List<Rectangle> olmecJumpList = new List<Rectangle>();
            olmecJumpList.Add(new Rectangle((olmecStartX) + (olmecWidth * (4)), olmecHeight, olmecWidth, olmecHeight));
            Jump = new FrameSelector(0.1f, olmecJumpList);
        }
        public Rectangle GetFrame()
        {
            switch (state)
            {
                case State.Running:
                    {
                        StateTimer += 0.01f;
                        return Run.GetFrame(ref StateTimer);
                    }
                case State.Squatting:
                    {
                        StateTimer += 0.01f;
                        return Jump.GetFrame(ref StateTimer);
                    }
                case State.Spawning:
                    {
                        StateTimer += 0.01f;
                        return Squat.GetFrame(ref StateTimer);

                    }
                case State.Jumping:
                    {
                        StateTimer += 0.01f;
                        return Jump.GetFrame(ref StateTimer);
                    }
                default: return new Rectangle();
            }
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            JumpTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (JumpTimer > 2)
            {
                Debug.WriteLine("JumpCheck");

                JumpTimer = 0;
            }
            if (previousState != state)
            {
                StateTimer = 0f;
                Squat.Reset();
            }

            RunAway();

            previousState = state;

            CheckPlayer(sprites);
            Movement();

            positionRectangle.X += (int)Velocity.X;
            Sprite s;
            s = CheckCollision(sprites);
            if (s != null)
                XCollision(s);
            else
            {
                positionRectangle.X -= (int)Velocity.X;
                positionRectangle.Y += (int)Velocity.Y;
                s = CheckCollision(sprites);
                if (s != null)
                    YCollision(s);
                else
                {
                    positionRectangle.Y += 1;
                    s = CheckCollision(sprites);
                    if (s == null)
                        Velocity.Y += 1;
                    positionRectangle.Y -= 1;
                }
            }
            positionRectangle.X += (int)Velocity.X;
        }
        //END OF UPDATE

        private void Movement()
        {
            {
                if (state == State.Falling && Velocity.Y < 0)
                {
                    if (movingRight)
                    {
                        Velocity.X = 3f;
                    }
                    if (!movingRight)
                    {
                        Velocity.X = -3f;
                    }
                }
                else if (state == State.Falling && Velocity.Y >= 0)
                {
                    Velocity.X = 0f;
                }
                if (state == State.Running)
                {
                    if (movingRight)
                    {
                        Velocity.X = 1;
                    }
                    else
                    {
                        Velocity.X = -1;
                    }
                }

                if ((state != State.Running) && DropTimer >= 0.26f)
                {
                    DropTimer = 0f;
                    state = State.Running;
                }

            }
        }



        private void RunAway()
        {
            if (state == State.Running && previousState == State.Jumping && !movingRight)
            {
                movingRight = true;
            }
            else if (state == State.Running && previousState == State.Jumping && movingRight)
            {
                movingRight = false;
            }
        }
        public void Parried()
        {
            //state = State.Squating;
        }
        public void Damaged()
        {
            state = State.Squatting;
            Bounce();

        }
        private void Bounce()
        {
            int bounceCount;
            bounceCount = 0;
            if (bounceCount == 0)
                Velocity.Y = -6f;
        }
        public override void XCollision(Sprite s)
        {
            switch (s.name)
            {
                case "player":
                    state = State.Squatting;
                    Velocity.X = -Velocity.X;
                    break;
                case "gps":
                case "brick":
                case "itemBlock":
                    {
                        if (positionRectangle.Left < s.positionRectangle.Left)
                        {
                            positionRectangle.X = s.positionRectangle.Left - positionRectangle.Width;
                            movingRight = false;

                        }
                        else if (positionRectangle.Right > s.positionRectangle.Right)
                        {
                            positionRectangle.X = s.positionRectangle.Right;
                            movingRight = true;
                        }
                        break;
                    }

                case "elf":
                case "pellet":
                    {
                        positionRectangle.X -= (int)Velocity.X;
                        break;
                    }


            }
        }

        public override void YCollision(Sprite s)
        {
            switch (s.name)
            {
                case "elf":
                case "pellet":
                    {
                        if (Velocity.Y < 0)
                            positionRectangle.Y -= (int)Velocity.Y;
                        if (Velocity.Y > 0)
                            positionRectangle.Y += (int)Velocity.Y;
                        break;
                    }
                case "gps":
                case "brick":
                case "itemBlock":
                    {
                        if (Velocity.Y > 0 && positionRectangle.Bottom > s.positionRectangle.Top)
                        {
                            positionRectangle.Y = s.positionRectangle.Top - positionRectangle.Height;
                            Velocity.Y = 0;
                        }
                        else
                        {
                            Velocity.Y = 0;
                        }
                        break;
                    }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (movingRight)
                spriteBatch.Draw(texture, positionRectangle, GetFrame(), Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            else
                spriteBatch.Draw(texture, positionRectangle, GetFrame(), Color.White);
            if (previousState != state)
                Debug.WriteLine("Olmec " + state.ToString());
        }

    }
}
