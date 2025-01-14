using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection.Emit;
using static BartGame.Player;

namespace BartGame
{
    class ElfWeird : Enemy
    {
        public enum State { Running, Spawning, Spawned, Squatting, Bouncing, Falling, Jumping, Die };
        public State state { get; set; }
        public State previousState;
        private FrameSelector Run, Squat, Invis, Jump, Dying;
        private List<Rectangle> SquatAnim;
        private int lastY, lastYframe;
        private int lastX, lastXframe;
        private int elfType;
        private float DamageTimer;
        private int jumpCheck;
        private float StateTimer, AnimationTimer, SquatTimer, JumpTimer, DeathTimer, SpawnInTimer;
        public static int elfHeight = 22;
        public static int elfWidth = 16;
        private static int elfStart = Constants.tileSize * 24;
        private float fallVelocity = 0.2f;
        public bool moving, respawning, jumping;
        public ElfWeird(int x, int y, int type, bool movingRight = false, bool moving = true, bool respawning = true)
        {
            name = "elf";
            state = State.Running;
            elfType = type % 3;
            Initialize();

            this.movingRight = movingRight;
            this.moving = moving;
            position = new Vector2(x, y);
            JumpTimer = 0f;
            SquatTimer = 0f;
            DeathTimer = 0f;
            SpawnInTimer = 0f;
            spawned = false;
            jumping = false;
            active = false;
            this.respawning = respawning;
            Start.X = x; 
            Start.Y = y;
        }
        //public ElfWeird(int x, int y, int type, bool moving = true, bool movingRight = false)
        //{
        //    name = "elf";
        //    state = State.Running;
        //    elfType = type;
        //    Initialize();

        //    this.movingRight = movingRight;
        //    this.moving = moving;
        //    position = new Vector2(x, y);
        //    JumpTimer = 0f;
        //    spawned = false;
        //}

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
            List<Rectangle> elfDyingList = new List<Rectangle>();
            elfDyingList.Add(new Rectangle((elfStart) + (elfWidth * (4)), elfHeight * (elfType), elfWidth, elfHeight));
            elfDyingList.Add(new Rectangle((elfStart) + (elfWidth * (4)), elfHeight * (elfType), elfWidth, elfHeight));
            Dying = new FrameSelector(0.1f, elfDyingList);
            List<Rectangle> elfInvisList = new List<Rectangle>();
            elfInvisList.Add(new Rectangle((elfStart) + (elfWidth * (4)), elfHeight * (3), elfWidth, elfHeight));
            Invis = new FrameSelector(0.1f, elfInvisList);
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
                        return Squat.GetFrame(ref StateTimer);
                    }
                case State.Bouncing:
                    {
                        StateTimer += 0.01f;
                        AnimationTimer += 0.01f;
                        return Squat.GetFrame(ref StateTimer);
                    }
                case State.Spawning:
                    {
                        StateTimer += 0.01f;
                        AnimationTimer += 0.01f;
                        return Invis.GetFrame(ref StateTimer);

                    }
                case State.Spawned:
                    {
                        StateTimer += 0.01f;
                        AnimationTimer += 0.01f;
                        return Jump.GetFrame(ref StateTimer);
                    }
                case State.Jumping:
                    {
                        StateTimer += 0.01f;
                        AnimationTimer += 0.01f;
                        return Jump.GetFrame(ref StateTimer);
                    }
                case State.Die:
                    {
                        StateTimer += 0.01f;
                        AnimationTimer += 0.01f;
                        return Dying.GetFrame(ref StateTimer);
                    }
                default: return new Rectangle();
            }
        }
        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            if (!active)
            {
                position.X = Start.X; 
                position.Y = Start.Y;
            }
            if (active)
            {
                previousState = state;

                if (moving)
                    JumpTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (state != State.Die && state != State.Spawning)
                {
                    canNotCollide = false;
                    moving = true;
                }
                if (JumpTimer > 2 && state != State.Spawning && state != State.Die)
                {
                    Debug.WriteLine("JumpCheck");

                    jumpCheck = random.Next(1, 3);
                    if (jumpCheck == 2)
                    {
                        Velocity.X = 0;
                        state = State.Squatting;
                        SquatTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    if (SquatTimer > 0.2)
                    {
                        lastY = (int)position.Y;
                        state = State.Jumping;
                        jumping = true;
                    }
                }
                //JUMPING
                if (state == State.Jumping)
                {
                    if ((lastY - position.Y) < 200 && jumping)
                    {
                        Velocity.Y = -4f;
                        jumping = false;
                    }
                    if (movingRight)
                        Velocity.X = 1;
                    else
                        Velocity.X = -1;
                    SquatTimer = 0;
                    JumpTimer = 0;
                }
                //WAITING TO SPAWN
                if (state == State.Spawning)
                {
                    Velocity.X = 0;
                }
                //SPAWNING
                if (state == State.Spawned)
                {

                    SpawnInTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (SpawnInTimer < 0.1 && state == State.Spawned)
                        Velocity.Y = -3f;
                    if (SpawnInTimer < 0.3)
                        canNotCollide = true;
                    else
                    {
                        canNotCollide = false;
                        state = State.Jumping;
                        SpawnInTimer = 0;
                    }

                    if (movingRight)
                        Velocity.X = 1;
                    else
                        Velocity.X = -1;
                    SquatTimer = 0;
                    JumpTimer = 0;
                }

                if (previousState != state)
                {
                    StateTimer = 0f;
                    Squat.Reset();
                }


                //MOVEMENT
                CheckPlayer(sprites);
                if (moving)
                {
                    Movement();
                }
                Sprite s;
                position.X += Velocity.X;
                BoundBox();

                s = CheckCollision(sprites);
                if (s != null)
                    XCollision(s);
                s = CheckCollision(sprites);
                if (s != null)
                    YCollision(s);
                else
                {

                    position.Y += Velocity.Y;
                    positionRectangle.Y += (int)Velocity.Y;
                    s = CheckCollision(sprites);
                    if (s != null)
                    {
                        YCollision(s);

                    }
                    else
                    {
                        position.Y += 1;
                        positionRectangle.Y += 1;

                        s = CheckCollision(sprites);
                        if (s == null)
                            Velocity.Y += fallVelocity;
                        position.Y -= 1;
                        positionRectangle.Y -= 1;

                    }
                }

                //Sprite s = null;
                //position.Y += Velocity.Y;
                //position.X += Velocity.X;
                //BoundBox();
                //s = CheckCollision(sprites);
                //if (s == null)
                //{
                //    Velocity.Y += fallVelocity;

                //}
                //else
                //{
                //    XCollision(s);
                //    YCollision(s);
                //}


                //DEATH MECHANIC
                if (state == State.Die)
                {
                    canNotCollide = true;
                    Bounce();
                    if (movingRight)
                        Velocity.X = -3;
                    else
                        Velocity.X = 3;
                    DeathTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (DeathTimer > 0.1)
                    {
                        moving = false;
                        DeathTimer = 0;
                        if (respawning)
                            state = State.Spawning;
                        else
                            canRemove = true;
                    }
                }

            }

        }
        //END OF UPDATE
        private void BoundBox()
        {
            positionRectangle = new Rectangle((int)position.X, (int)position.Y, elfWidth, elfHeight);
        }

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

                if ((state == State.Bouncing) && AnimationTimer >= 0.26f)
                {
                    AnimationTimer = 0f;
                    state = State.Running;
                }

            }
        }
        private void Bounce()
        {
            int Speed = 4; 
            Velocity.Y = MathHelper.Clamp(Velocity.Y, 0, Speed);
            Velocity.Y -= Speed;
        }

        public void Damaged()
        {
            state = State.Squatting;

        }
        public override void XCollision(Sprite s)
        {
            switch (s.name)
            {
                case "player":

                    state = State.Bouncing;
                    Velocity.X = -Velocity.X;
                    Velocity.Y = -2f;
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
                case "gps":
                case "brick":
                case "itemBlock":
                case "elf":
                    {
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
                    }

                case "platform":
                    break;
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
                        break;
                    }
                case "gps":
                case "brick":
                case "itemBlock":
                    {
                        if (Velocity.Y >= 0 && positionRectangle.Bottom > s.positionRectangle.Top && positionRectangle.Top < s.positionRectangle.Top)
                        {
                            position.Y = s.positionRectangle.Top - positionRectangle.Height;
                            Velocity.Y = 0;
                        }
                        else if (positionRectangle.Bottom > s.positionRectangle.Bottom)
                        {
                            Velocity.Y /= 2;
                        }
                        state = State.Running;
                        break;
                    }
                case "tv":
                case "platform":

                    if (positionRectangle.Bottom > s.positionRectangle.Top && positionRectangle.Bottom < s.positionRectangle.Top + 4 && Velocity.Y >= 0)
                    {
                        positionRectangle.Y = s.positionRectangle.Top - positionRectangle.Height;
                        Velocity.Y = 0;
                            state = State.Running;
                    }
                    else if (positionRectangle.Bottom > (s.positionRectangle.Top + 4)
                        && (state == State.Running || state == State.Squatting
                         && (positionRectangle.Left > s.positionRectangle.Left)))
                        state = State.Falling;
                    else if (positionRectangle.Bottom > s.positionRectangle.Top)
                    {

                    }
                    break;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (movingRight)
                spriteBatch.Draw(texture, position, GetFrame(), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
            else
                spriteBatch.Draw(texture, position, GetFrame(), Color.White);
            if (previousState != state)
                Debug.WriteLine("Weird Elf " + state.ToString());
            //Debug.WriteLine("Elf position " + position.X.ToString());
            ////Debug.WriteLine("Weird Elf " + positionRectangle.Height.ToString());


        }

    }
}
