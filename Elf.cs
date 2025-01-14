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
    class Elf : Enemy
    {
        public enum State { Running, Spawning, Squatting, Falling, Jumping };
        public State state { get; set; }
        public State previousState;
        private FrameSelector Run, Squat, Jump;
        private List<Rectangle> SquatAnim;
        private int lastY, lastYframe;
        private int lastX, lastXframe;
        private int elfType;
        private int jumpCheck;
        private float StateTimer, AnimationTimer, JumpTimer;
        public static int elfHeight = 22;
        public static int elfWidth = 16;
        private static int elfStart = Constants.tileSize * 24;

        public Elf(int x, int y, int type, bool movingRight = false)
        {
            name = "elf";
            positionRectangle = new Rectangle(x, y, elfWidth, elfHeight);
            state = State.Running;
            elfType = type;
            Initialize();
            this.movingRight = movingRight;
            JumpTimer = 0f;
        }

        private void Initialize()
        {
            int i;
            //running
            List<Rectangle> elfRunList = new List<Rectangle>();
            for (i = 0; i < 3; i++) // id = 1, 2, 3
            {
                elfRunList.Add(new Rectangle((elfStart) + (elfWidth * (i)), elfHeight * (elfType), elfWidth, elfHeight));
            }
            elfRunList.Add(new Rectangle((elfStart) + (elfWidth * (1)), elfHeight * (elfType), elfWidth, elfHeight));
            Run = new FrameSelector(0.1f, elfRunList);
            //Squatting
            List<Rectangle> elfSquatList = new List<Rectangle>();
            elfSquatList.Add(new Rectangle((elfStart) + (elfWidth * (3)), elfHeight * (elfType), elfWidth, elfHeight));
            Squat = new FrameSelector(0.1f, elfSquatList);
            //Jump/Hit
            List<Rectangle> elfJumpList = new List<Rectangle>();
            elfJumpList.Add(new Rectangle((elfStart) + (elfWidth * (4)), elfHeight * (elfType), elfWidth, elfHeight));
            Jump = new FrameSelector(0.1f, elfJumpList);
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
                        AnimationTimer += 0.01f;
                        return Jump.GetFrame(ref StateTimer);
                    }
                case State.Spawning:
                    {
                        StateTimer += 0.01f;
                        AnimationTimer += 0.01f;
                        return Squat.GetFrame(ref StateTimer);

                    }
                case State.Jumping:
                    {
                        StateTimer += 0.01f;
                        AnimationTimer += 0.01f;
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

                jumpCheck = random.Next(1, 3);
                if (jumpCheck == 2)
                {
                    Velocity.Y = -8f;
                    state = State.Jumping;
                }
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

                if ((state != State.Running) && AnimationTimer >= 0.26f)
                {
                    AnimationTimer = 0f;
                    state = State.Running;
                }

                if (state == State.Jumping)
                {

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
                case "elf":
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
                case "tv":
                    {
                        
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
                Debug.WriteLine("elf " + state.ToString());
        }

    }
}
