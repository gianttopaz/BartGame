using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;

namespace BartGame
{
    class MoeKnife : Enemy
    {
        public enum State { Running, Parrying, Stabbing, Damaged, Falling, Jumping, Dying};
        public enum Health { One, Two, Dead  };
        public State state { get; set; }
        public State previousState;
        public Health health { get; set; }
        private FrameSelector Run, Parry, Stab, Hit, Jump;
        private List<Rectangle> parryAnim;
        private int lastY, lastYframe;
        private int lastX, lastXframe;
        public int StateSwitcher = 0;
        private float StateTimer, AnimationTimer, TurnTimer, DyingTimer;
        public static int moeHeight = 48;
        public static int moeWidth = 32;
        private bool lastDirection;


        public MoeKnife(int x, int y, bool movingRight = false)
        {
            name = "moeKnife";
            positionRectangle = new Rectangle(x, y, moeWidth, moeHeight);
            state = State.Running;
            Initialize();
            
        }

        private void Initialize()
        {
            int i;
            //running
            List<Rectangle> moeRunList = new List<Rectangle>();
            for (i = 0; i < 4; i++) // id = 1, 2, 3
            {
                moeRunList.Add(new Rectangle(moeWidth * (i), 0, moeWidth, moeHeight)); //id = 0
            }
            Run = new FrameSelector(0.1f, moeRunList);
            //Hitby
            List<Rectangle> moeHitList = new List<Rectangle>();
            for (i = 0; i < 2; i++) // id = 1, 2
            {
                moeHitList.Add(new Rectangle((moeWidth * 9) + (moeWidth * (i)), 0, moeWidth, moeHeight)); //id = 0
            }
            Hit = new FrameSelector(0.1f, moeHitList);
            //parrying
            List<Rectangle> moeParryList = new List<Rectangle>();
            for (i = 0; i < 3; i++) // id = 1, 2, 3
            {
                moeParryList.Add(new Rectangle((moeWidth * 6) + (moeWidth * (i)), 0, moeWidth, moeHeight)); //id = 0
            }
            Parry = new FrameSelector(0.1f, moeParryList);
            //stabby
            List<Rectangle> moeStabList = new List<Rectangle>();
            for (i = 0; i < 2; i++) // id = 1, 2, 3
            {
                moeStabList.Add(new Rectangle((moeWidth * 4) + (moeWidth * (i)), 0, moeWidth, moeHeight)); //id = 0
            }
            Stab = new FrameSelector(0.05f, moeStabList);
            TurnTimer = 0;
            DyingTimer = 0;
            //jump/fall frame
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
                case State.Damaged:
                    {
                        StateTimer += 0.01f;
                        AnimationTimer += 0.01f;
                        return Hit.GetFrame(ref StateTimer);
                    }
                case State.Dying:
                    {
                        StateTimer += 0.01f;
                        AnimationTimer += 0.01f;
                        return Hit.GetFrame(ref StateTimer);
                    }
                case State.Parrying:
                    {
                        Velocity.X = 0;
                        StateTimer += 0.01f;
                        AnimationTimer += 0.01f;
                        return Parry.GetFrame(ref StateTimer);

                    }
                case State.Stabbing:
                    {
                        Velocity.X = 0;
                        StateTimer += 0.01f;
                        AnimationTimer += 0.01f;
                        return Stab.GetFrame(ref StateTimer);
                    }
                default: return new Rectangle();
            }
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            if (previousState != state)
            {
                StateTimer = 0f;
                Parry.Reset();
            }
            if ((lastDirection && !movingRight) || (!lastDirection && movingRight))
            {
                TurnTimer = 0;
            }
            if (StateSwitcher == 3)
            {
                state = State.Dying;
            }

            lastDirection = movingRight;
            RunAway();

            previousState = state;

            CheckPlayer(sprites);
            Movement(gameTime);

            if (state == State.Dying)
                Die(gameTime);
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

        private void Movement(GameTime gameTime)
        {
            {
                if (state == State.Damaged && Velocity.Y < 0)
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
                else if (state == State.Damaged && Velocity.Y >= 0)
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

                if (state == State.Running && this.positionRectangle.Right < (PlayerPosition.Left - 40))
                {
                    if (!movingRight)
                        TurnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (TurnTimer < 0.8 && !movingRight)
                    {
                        Velocity.X = 0;
                    }

                    else if (TurnTimer >= 0.8 && !movingRight)
                    {
                        movingRight = true;
                        TurnTimer = 0;

                    }
                }
                else if (state == State.Running && this.positionRectangle.Left > (PlayerPosition.Right + 40))
                {
                    if (movingRight)
                        TurnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (TurnTimer < 0.8 && movingRight)
                    {
                        Velocity.X = 0;
                    }

                    else if (TurnTimer >= 0.8 && movingRight)
                    {
                        movingRight = false;
                        TurnTimer = 0;

                    }
                }
            }
        }
        //        if (state == State.Running && this.positionRectangle.Right < (PlayerPosition.Left - 40))
        //        {
        //            Turning(gameTime);
        //        }
        //        else if (state == State.Running && this.positionRectangle.Left > (PlayerPosition.Right + 40))
        //        {

        //            Turning(gameTime);
        //        }
        //    }
        //}

        //private void Turning(GameTime gameTime)
        //{
        //    TurnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        //    if (TurnTimer < 0.8)
        //    {
        //        Velocity.X = 0;
        //    }
        //    else
        //    {
        //        if (movingRight)
        //            movingRight = false;
        //        else
        //            movingRight = true;
        //    }
        //}



        private void RunAway()
        {
            if (state == State.Running && previousState == State.Stabbing && !movingRight)
            {
                movingRight = true;
            }
            else if (state == State.Running && previousState == State.Stabbing && movingRight)
            {
                movingRight = false;
            }
        }
        public void Parried()
        {
            state = State.Parrying;
        }
        public void Damaged()
        {
            state = State.Damaged;
            Bounce();

        }
        private void Bounce()
        {
            int bounceCount;
            bounceCount = 0;
            if (bounceCount == 0)
            Velocity.Y = -6f;
        }

        private void Die(GameTime gameTime)
        {
            canNotCollide = true;
            if (DyingTimer <= 0.02)
            {
                Velocity.Y -= 3;
            }

            DyingTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (DyingTimer >= 1)
                canRemove = true;
        }

        public override void XCollision(Sprite s)
        {
            switch (s.name)
            {
                case "player":
                    state = State.Stabbing;
                    Velocity.X = -Velocity.X;
                    break;
                case "gps":
                case "brick":
                case "itemBlock":
                case "moeKnife":
                    Velocity.X = -Velocity.X;
                    if (positionRectangle.Left < s.positionRectangle.Left)
                    {
                        position.X = s.positionRectangle.Left - positionRectangle.Width;
                        movingRight = false;

                    }
                    else if (positionRectangle.Right > s.positionRectangle.Right)
                    {
                        position.X = s.positionRectangle.Right;
                        movingRight = true;
                    }
                    break;
                case "pellet":
                    
                    break;

            }
        }

        public override void YCollision(Sprite s)
        {
            switch (s.name)
            {
                case "gps":
                case "brick":
                case "itemBlock":
                    {
                        if (positionRectangle.Bottom > s.positionRectangle.Top)
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
            //if (previousState != state)
            //    Debug.WriteLine("Moe " + state.ToString());
            //    Debug.WriteLine("Moe Turn Timer " + TurnTimer.ToString());
        }

    }
}
