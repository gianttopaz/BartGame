using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BartGame
{
    class Olmec : Enemy
    {
        public enum State { Waiting, Falling, Rising };
        public State state { get; set; }
        public State previousState;
        private FrameSelector Fall, Rise, RisingDown, RisingUp;
        private List<Rectangle> SquatAnim;
        private int lastY, lastYframe;
        private int lastX, lastXframe;
        private float StateTimer, DropTimer, JumpTimer;
        public static int olmecHeight = 48;
        public static int olmecWidth = 48;
        private static int olmecStartX = Constants.tileSize * 0;
        private static int olmecStartY = Constants.tileSize * 13;
        private bool ready;
        private bool bartAbove;


        public Olmec(int x, int y)
        {
            name = "olmec";
            positionRectangle = new Rectangle(x, y, olmecWidth, olmecHeight);
            state = State.Waiting;
            Initialize();
            DropTimer = 0f;
        }

        private void Initialize()
        {
            int i;
            //running
            List<Rectangle> FallFrame = new List<Rectangle>();
            FallFrame.Add(new Rectangle(olmecStartX, olmecStartY, olmecWidth, olmecHeight));
            FallFrame.Add(new Rectangle(olmecStartX + 48, olmecStartY, olmecWidth, olmecHeight));
            Fall = new FrameSelector(0.02f, FallFrame);
            List<Rectangle> WaitDownFrame = new List<Rectangle>();
            WaitDownFrame.Add(new Rectangle(olmecStartX + (olmecWidth * 2), olmecStartY, olmecWidth, olmecHeight));
            WaitDownFrame.Add(new Rectangle(olmecStartX + (olmecWidth * 3), olmecStartY, olmecWidth, olmecHeight));
            WaitDownFrame.Add(new Rectangle(olmecStartX + (olmecWidth * 4), olmecStartY, olmecWidth, olmecHeight));
            WaitDownFrame.Add(new Rectangle(olmecStartX + (olmecWidth * 5), olmecStartY, olmecWidth, olmecHeight));
            Rise = new FrameSelector(0.02f, WaitDownFrame);
        }
        public Rectangle GetFrame()
        {
            switch (state)
            {
                case State.Falling:
                    {
                        StateTimer += 0.01f;
                        return Fall.GetFrame(ref StateTimer);
                    }
                case State.Waiting:
                    {
                        StateTimer += 0.01f;
                        return Fall.GetFrame(ref StateTimer);
                    }
                case State.Rising:
                    {
                        StateTimer += 0.0f;
                        if (PlayerPosition.Y > positionRectangle.Y + 10)
                            Rise.curFrameID = 0;
                        else if ( PlayerPosition.Y < positionRectangle.Y - 10 && PlayerPosition.Right < positionRectangle.Left && PlayerPosition.Left > positionRectangle.Right)
                            Rise.curFrameID = 2;
                        else if (PlayerPosition.Right > positionRectangle.Left && PlayerPosition.Left < positionRectangle.Right)
                            Rise.curFrameID = 3;
                        else
                            Rise.curFrameID = 1;


                        return Rise.GetFrame(ref StateTimer);

                    }
                default: return new Rectangle();
            }
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {

            previousState = state;

            CheckPlayer(sprites);


            if (state == State.Waiting)
            {
                Waiting(gameTime);
            }
            if (state == State.Rising)
            {
                Rising(gameTime);
            }

            Flip();

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
                        if (state == State.Falling)
                            Velocity.Y += 3;
                    positionRectangle.Y -= 1;
                }
            }
            positionRectangle.X += (int)Velocity.X;
            Risen();

        }

        private void Waiting(GameTime gameTime)
        {
            DropTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (DropTimer > 2)
            {
                ready = true;
            }
            if (ready && PlayerPosition.Right > (positionRectangle.Left - 50) && PlayerPosition.Left < (positionRectangle.Right+ 50))
            {
                DropTimer = 0;
                Drop();
            }

        }

        //END OF UPDATE
        private void Rising(GameTime gameTime)
        {
            DropTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (DropTimer > 2)
            {
                Velocity.Y -= 1;
            }

        }
        private void Risen()
        {
            if (positionRectangle.Y < 0)
            {
                positionRectangle.Y = 0;
                Velocity.Y = 0;
                DropTimer = 0;
                state = State.Waiting;
            }
        }
        private void Flip()
        {
            if (PlayerPosition.Right > (positionRectangle.Left + 24))
            {
                movingRight = true;
            }
            else
                movingRight = false;
        }
        private void Drop()
        {
            state = State.Falling;
        }
        public override void XCollision(Sprite s)
        {
            switch (s.name)
            {
                case "gps":
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

                case "player":
                case "brick":
                case "itemBlock":
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
                case "brick":
                case "itemBlock":
                    {
                        if (Velocity.Y < 0)
                            positionRectangle.Y -= (int)Velocity.Y;
                        if (Velocity.Y > 0)
                            positionRectangle.Y += (int)Velocity.Y;
                        break;
                    }
                case "gps":
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
                        ready = false;
                        state = State.Rising;
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
            {
                Debug.WriteLine("Olmec " + state.ToString());
                Debug.WriteLine("Olmec " + positionRectangle.Y.ToString());

            }


        }

    }
}
