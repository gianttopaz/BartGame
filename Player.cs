using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BartGame
{
    public class Player : Sprite
    {
        private static Texture2D texture;
        public enum Level { Normal, Skateboard, Slingshot, Both };
        public enum Power { Normal, Squishee };
        public enum Health { Three, Two, One, Dead };
        public enum State
        {
            Standing, Walking, Turn, Jumping, Squat, Damaged, Dying, Falling, Shooting,
            Skating, SkateJumping, SkateFalling
        };
        public enum SlingshotStrength { None, Low, Mid, Full };
        public enum ShState { Standing, Jumping, Falling };//shootState

        private FrameSelector Stand, Walk, Turn, Squat, Jump, Skateboard, Damage, Die, Shoot, Swim, SlideDown;
        private List<Rectangle> Squart, shootAnim, shootAnimJump, skateAnim;
        private float Transformation_Timer, InvisibleTimer, DeathTimer;

        public Level level { get; set; }
        public State state { get; set; }
        public Power power { get; set; }
        public Power previousPower { get; set; }
        public Health health { get; set; }
        public ShState shState { get; set; }//shootState
        public ShState lastshState;
        public SlingshotStrength ShotPower { get; set; }
        public State previousState;
        public State lastState;
        private Vector2 StartPosition { get; set; }
        public bool Sparkled { get; internal set; }

        private int lastY, lastYframe;
        private int lastX, lastXframe;
        private int bounces = 0;
        private float StateTimer, AnimationTimer, DamageTimer;
        public float SquisheeTimer;
        public static int bartHeight = 32;
        public static int bartWidth = 16;


        public bool Jumped, Speedup, Animating, isTransforming, isGrounded, bouncing, ReadyFire, WasJumping, ShootJumping;

        private KeyboardState currentKey, prevKey;
        public Player(int x, int y)
        {
            name = "player";
            positionRectangle = new Rectangle(x, y, 16, 32);
            position = new Vector2(x, y);
            StartPosition = new Vector2(x, y);
            Initialize();
        }

        private void Initialize()
        {
            int i = 0;
            //standing
            List<Rectangle> standList = new List<Rectangle>();
            standList.Add(new Rectangle(16 * (i++), 0, 16, 32)); //id = 0
            Stand = new FrameSelector(0.1f, standList);
            //walking
            List<Rectangle> wkList = new List<Rectangle>();
            for (i = 0; i < 6; i++) // id = 1, 2, 3
            {
                wkList.Add(new Rectangle(16 * (i), 0, 16, 32));
            }
            Walk = new FrameSelector(0.2f, wkList);
            //Jump
            List<Rectangle> jumpList = new List<Rectangle>();
            jumpList.Add(new Rectangle(bartWidth * 0, bartHeight * 1, 16, 32));
            jumpList.Add(new Rectangle(bartWidth * 1, bartHeight * 1, 16, 32));
            Jump = new FrameSelector(0.1f, jumpList);
            //Turn
            List<Rectangle> turnList = new List<Rectangle>();
            turnList.Add(new Rectangle(bartWidth * 8, 0, 16, 32)); //id = 0
            Turn = new FrameSelector(0.1f, turnList);
            //Damaged
            List<Rectangle> damagedList = new List<Rectangle>();
            damagedList.Add(new Rectangle(bartWidth * 2, bartHeight * 1, 16, 32)); //id = 0
            damagedList.Add(new Rectangle(bartWidth * 3, bartHeight * 1, 16, 32)); //id = 0
            Damage = new FrameSelector(0.1f, damagedList);
            //Die
            List<Rectangle> dieList = new List<Rectangle>();
            dieList.Add(new Rectangle(bartWidth * 4, bartHeight * 1, 16, 32)); //id = 0
            Die = new FrameSelector(0.1f, dieList);

            //squat
            this.Squart = new List<Rectangle>(3);
            Squart.Add(new Rectangle(bartWidth * 6, bartHeight * 0, 16, 32));
            Squart.Add(new Rectangle(bartWidth * 7, bartHeight * 0, 16, 32));
            //squat
            this.shootAnim = new List<Rectangle>(5);
            shootAnim.Add(new Rectangle(bartWidth * 0, bartHeight * 2, 16, 32));
            shootAnim.Add(new Rectangle(bartWidth * 1, bartHeight * 2, 16, 32));
            shootAnim.Add(new Rectangle(bartWidth * 2, bartHeight * 2, 16, 32));
            shootAnim.Add(new Rectangle(bartWidth * 3, bartHeight * 2, 16, 32));
            shootAnim.Add(new Rectangle(bartWidth * 4, bartHeight * 2, 16, 32));
            shootAnim.Add(new Rectangle(bartWidth * 6, bartHeight * 2, 16, 32));
            shootAnim.Add(new Rectangle(bartWidth * 7, bartHeight * 2, 16, 32));
            shootAnim.Add(new Rectangle(bartWidth * 8, bartHeight * 2, 16, 32));
            shootAnim.Add(new Rectangle(bartWidth * 9, bartHeight * 2, 16, 32));
            shootAnim.Add(new Rectangle(bartWidth * 10, bartHeight * 2, 16, 32));
            //skate
            this.skateAnim = new List<Rectangle>(8);
            //speedup
            skateAnim.Add(new Rectangle(bartWidth * 0, bartHeight * 4, 16, 32)); //0
            skateAnim.Add(new Rectangle(bartWidth * 1, bartHeight * 4, 16, 32)); //1
            skateAnim.Add(new Rectangle(bartWidth * 2, bartHeight * 4, 16, 32)); //2
            skateAnim.Add(new Rectangle(bartWidth * 3, bartHeight * 4, 16, 32)); //3
            skateAnim.Add(new Rectangle(bartWidth * 4, bartHeight * 4, 16, 32)); //4
            //coasting
            skateAnim.Add(new Rectangle(bartWidth * 5, bartHeight * 4, 16, 32)); //5
            skateAnim.Add(new Rectangle(bartWidth * 6, bartHeight * 4, 16, 32)); //6
            //jumping
            skateAnim.Add(new Rectangle(bartWidth * 7, bartHeight * 4, 16, 32)); //7

            this.Transformation_Timer = 0;
            this.DeathTimer = 0;
            this.level = Level.Normal;
            this.state = State.Standing;
            this.ShotPower = SlingshotStrength.None;
            this.shState = ShState.Standing;
            this.Jumped = true;
            this.ReadyFire = false;
            this.isGrounded = false;
            this.movingRight = true;
            this.bouncing = false;
            this.ShootJumping = false;
            this.isFlashing = false;
            this.prevKey = Keyboard.GetState();
            this.StateTimer = 0;
            this.AnimationTimer = 0;
            this.DamageTimer = 0;

        }

        public static void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("BartSheet");
        }
        private void GetFrametoDraw(float ElapsedGameTime)
        {
            if (level == Level.Normal)
            {
                switch (state)
                {
                    case State.Walking:
                        Walking();
                        sourceRectangle = Walk.GetFrame(ref StateTimer);
                        break;
                    case State.Turn:
                        Walking();
                        sourceRectangle = Turn.GetFrame(ref StateTimer);
                        break;
                    case State.Squat:
                        lastY = positionRectangle.Y;
                        lastX = positionRectangle.X;
                        Squatting(ElapsedGameTime);
                        break;
                    case State.Jumping:
                        Jumping();
                        sourceRectangle = Jump.GetFrame(ref StateTimer);
                        break;
                    case State.Damaged:
                        sourceRectangle = Damage.GetFrame(ref StateTimer);
                        break;
                    case State.Dying:
                        Dying(ElapsedGameTime);
                        sourceRectangle = Die.GetFrame(ref StateTimer);
                        break;
                    case State.Falling:
                        Falling();
                        break;
                    case State.Shooting:
                        Shooting(ElapsedGameTime);
                        break;
                    case State.Skating:
                        Skating(ElapsedGameTime);
                        break;
                    case State.SkateJumping:
                        SkateJump();
                        break;
                    case State.SkateFalling:
                        SkateFall();
                        break;
                    default:
                        sourceRectangle = Stand.GetFrame(ref StateTimer);
                        Standing();
                        break;
                }
            }
            positionRectangle.Width = sourceRectangle.Width;
            positionRectangle.Height = sourceRectangle.Height;

            StateTimer += ElapsedGameTime / 1000;

        }
        private void BoundBox()
        {
            positionRectangle = new Rectangle((int)position.X, (int)position.Y, bartWidth, bartHeight);
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            //BoundBox();

            prevKey = currentKey;
            currentKey = Keyboard.GetState();
            if (previousState != state)
            {
                lastState = previousState;
            }

            if (previousState == State.Damaged && state != State.Damaged && !isFlashing)
            {
                StartFlashing();
            }

            previousState = state;
            previousPower = power;
            lastshState = shState;

            if (isFlashing)
                Flashing(gameTime);
            else
                color = Color.White;

            GetFrametoDraw((float)gameTime.ElapsedGameTime.TotalMilliseconds);

            if (power == Power.Squishee)
            {
                SquisheeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (SquisheeTimer > 20)
                    power = Power.Normal;
            }
            else
            {
                Sparkled = false;
            }
            //if (currentKey.IsKeyDown(Keys.Enter))
            //{
            //    positionRectangle.X = (int)StartPosition.X;
            //    positionRectangle.Y = (int)StartPosition.Y;
            //}
            if (state == State.Damaged)
            {
                Damaged((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (state == State.Standing || state == State.Walking)
            {
                Animating = false;
                this.Transformation_Timer = 0;

            }
            //COLLISION
            Sprite s;
            if (!isTransforming)
            {
                positionRectangle.X += (int)Velocity.X;
                position.X += Velocity.X;
                s = CheckCollision(sprites);
                positionRectangle.Y += (int)Velocity.Y;
                position.Y += Velocity.Y;

                if (s != null)
                    XCollision(s);
                s = CheckCollision(sprites);
                if (s != null)
                    YCollision(s);
                else
                {
                    positionRectangle.Y += 1;
                    s = CheckCollision(sprites);
                    if (s == null && (state == State.SkateJumping || state == State.Skating))
                    {
                        state = State.SkateFalling;
                    }
                    else if (s == null && state != State.Jumping && state != State.SkateFalling && state != State.Damaged
                        && state != State.SkateJumping && state != State.Skating && state != State.Shooting)
                    {
                        state = State.Falling;
                    }
                    positionRectangle.Y -= 1;
                    position.Y -= 1;
                }
            }
            if (isTransforming)
            {
                Velocity.Y = 2;
                positionRectangle.Y += (int)Velocity.Y;
                position.Y += Velocity.Y;
            }

            #region test
            //test, use these function to change mario level
            if (currentKey.IsKeyDown(Keys.Y) && !prevKey.IsKeyDown(Keys.Y) && state != State.Skating && previousState != State.Skating)
            {
                startAnimation(2);
            }
            if (currentKey.IsKeyDown(Keys.T))
            {
                if (state == State.Jumping)
                    shState = ShState.Jumping;
                if (state == State.Falling)
                    shState = ShState.Falling;
                startAnimation(1);
            }
            if (currentKey.IsKeyDown(Keys.J) && !prevKey.IsKeyDown(Keys.Y) && state != State.Skating && previousState != State.Skating)
            {
                isTransforming = true;
                Debug.WriteLine(isTransforming.ToString());
            }
            #endregion
            if (health >= Health.Dead)
            {
                health = Health.Dead;
                //state = State.Dying;
            }

        }
        public void startAnimation(int s = 0)
        {
            if (Animating) return;
            this.Transformation_Timer = 0;
            Animating = true;
            switch (s)
            {
                case 0: //Squat
                    lastY = positionRectangle.Y;
                    lastX = positionRectangle.X;

                    Velocity.X = 0;
                    Velocity.Y = 0;
                    state = State.Squat;
                    break;
                case 1: //Slingshot
                    state = State.Shooting;
                    break;
                case 2: //Skateboard
                    lastY = positionRectangle.Y;
                    lastX = positionRectangle.X;

                    state = State.Skating;
                    break;
                case 3:
                    lastY = positionRectangle.Y;
                    lastX = positionRectangle.X;

                    state = State.SkateJumping;
                    break;
                default:
                    break;
            }
        }
        private void Standing()
        {

            Jumped = (prevKey.IsKeyDown(Keys.W) || prevKey.IsKeyDown(Keys.Space)) ? true : false;

            Velocity.Y = Velocity.X = 0;

            if (currentKey.IsKeyDown(Keys.D))
            {
                Walk.Reset();
                movingRight = true;
                state = State.Walking;
            }
            else if (currentKey.IsKeyDown(Keys.A))
            {
                Walk.Reset();
                movingRight = false;
                state = State.Walking;
            }
            else if (currentKey.IsKeyDown(Keys.S) && !prevKey.IsKeyDown(Keys.S))
            {
                startAnimation(0);
            }
            else if (currentKey.IsKeyDown(Keys.W))
            {
                if (!Jumped)
                {
                    lastY = positionRectangle.Y;
                    Velocity.Y = Constants.jump_velocity;
                    state = State.Jumping;
                }
            }
            if (currentKey.IsKeyDown(Keys.T) && level == Level.Normal)
            {
                startAnimation(1);
            }

        }

        private void Walking()
        {
            Jumped = prevKey.IsKeyDown(Keys.W) ? true : false;
            state = State.Walking;
            float max_v;
            if (currentKey.IsKeyDown(Keys.G)) //run key
            {
                max_v = Constants.max_run_velocity;
                xAcceleration = Constants.run_accelerate;
                Speedup = true;
            }
            else
            {
                max_v = Constants.max_walk_velocity;
                xAcceleration = Constants.walk_accelerate;
                Speedup = false;
            }

            if (currentKey.IsKeyDown(Keys.D) && state != State.Damaged)
            {
                //moving right
                movingRight = true;
                if (Velocity.X < 0) //walking left earlier
                {
                    state = State.Turn;
                    xAcceleration += 0.3f;
                }

                if (Velocity.X > max_v) //slow down if pre-state was spdup
                    Velocity.X -= xAcceleration;
                else if (Velocity.X < max_v) //speed up if not reach max_v
                    Velocity.X += xAcceleration;
            }
            else if (currentKey.IsKeyDown(Keys.A) && state != State.Damaged)
            {
                //moving left
                movingRight = false;
                if (Velocity.X > 0) //walking right earlier
                {
                    state = State.Turn;
                    xAcceleration += 0.3f;
                }

                if (Velocity.X < max_v * -1) //slow down if pre-state was spdup
                    Velocity.X += xAcceleration;
                else if (Velocity.X > max_v * -1) //speed up if not reach max_v
                    Velocity.X -= xAcceleration;
            }
            else
            {
                //slow down (moving right or left)
                if (movingRight)
                {
                    if (Velocity.X > 0) //slow down then stop
                        Velocity.X -= xAcceleration * 5;
                    else // moving left earlier -> stop immediately
                    {
                        Velocity.X = 0;
                        state = State.Standing;
                    }
                }
                else
                {
                    if (Velocity.X < 0) //slow down then stop
                        Velocity.X += xAcceleration * 5;
                    else // moving left earlier -> stop immediately
                    {
                        Velocity.X = 0;
                        state = State.Standing;
                    }
                }
            }
            if (currentKey.IsKeyDown(Keys.W) && state != State.Skating)
            {
                if (!Jumped)
                {
                    lastY = positionRectangle.Y;
                    Velocity.Y = Constants.jump_velocity;
                    state = State.Jumping;
                }
            }
            else if (currentKey.IsKeyDown(Keys.S))
            {
                state = State.Squat;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.T) && (level == Level.Normal))
            {
                startAnimation(1);
            }
        }
        private void Jumping()
        {
            Jumped = true;
            isGrounded = false;
            Velocity.Y += Constants.fall_velocity; //decrease Velocity.Y gradually
            if ((lastY - positionRectangle.Y) > 55)
            {
                if (state != State.SkateJumping && state != State.Shooting && state != State.Damaged)
                    state = State.Falling;
            }

            if (currentKey.IsKeyDown(Keys.D))
            {
                if (Velocity.X < (Constants.max_walk_velocity - 2))  //speed up if not reach max_v
                    Velocity.X += xAcceleration;
            }
            else if (currentKey.IsKeyDown(Keys.A))
            {
                if (Velocity.X > (Constants.max_walk_velocity - 2) * -1) //speed up if not reach max_v
                    Velocity.X -= xAcceleration;
            }

            if (!currentKey.IsKeyDown(Keys.W) && state != State.SkateJumping && state != State.Skating
                && state != State.Shooting && state != State.Damaged)
            {
                state = State.Falling;
            }
            else
                Velocity.Y -= (Constants.fall_velocity * 1);

        }
        private void Damaged(float ElapsedGameTime, int bounceCount = 0)
        {


            float Direction;

            DamageTimer += ElapsedGameTime;

            if (!movingRight)
            {
                Direction = 1;
            }
            else
            {
                Direction = -1;
            }
            Velocity.X = 1 * Direction;

            //when touching ground, or after damagetimer reaches certain amount
            //go back to other state
            //if last state was damaged
            //go invulnerable for a bit
            //flash transparent
            Velocity.X = MathHelper.Clamp(Velocity.X, -1, 1);

            //if (currentKey.IsKeyDown(Keys.D) || currentKey.IsKeyDown(Keys.A))
            //{
            //    Velocity.X = 0;
            //}
            if (DamageTimer < 0.05)
                Bounce(1);
            if (DamageTimer > 0.4)
            {
                if (isGrounded)
                    state = State.Standing;
                else
                    state = State.Falling;
                DamageTimer = 0;
            }
        }
        private void Bounce(int bounceCount)
        {
            Velocity.Y = MathHelper.Clamp(Velocity.Y, 0, 3);
            Velocity.Y -= 1;
            if (bounces >= bounceCount)
            {
                bouncing = false;
                bounces = 0;
            }
        }
        private void Dying(float ElapsedGameTime)
        {
            Velocity.X = 0;
            DeathTimer += ElapsedGameTime;
            canNotCollide = true;
            if (DeathTimer < 25)
            {
                Velocity.Y -= 2;
            }
            positionRectangle.Y += (int)Velocity.Y;
            Velocity.Y += Constants.fall_velocity;
            if (DeathTimer > 1000)
                canRemove = true;

        }
        private void Shooting(float ElapsedGameTime)
        {
            if (shState == ShState.Standing)
            {
                isGrounded = true;
                Jumped = false;
                Velocity.X = 0;
                Velocity.Y += Constants.fall_velocity * 1;
            }

            if (currentKey.IsKeyDown(Keys.W) && !prevKey.IsKeyDown(Keys.W) && isGrounded)
            {
                //holds last position pre-jump
                if (!Jumped)
                {
                    lastY = positionRectangle.Y;
                    Velocity.Y = Constants.jump_velocity;
                    shState = ShState.Jumping;
                }
                Jumped = true;
                isGrounded = false;
            }
            //Jumping
            if (shState == ShState.Jumping)
            {
                Velocity.Y += Constants.fall_velocity; //decrease Velocity.Y gradually
                if ((lastY - positionRectangle.Y) > 55)
                {
                    shState = ShState.Falling;
                }
                if (!currentKey.IsKeyDown(Keys.W))
                    shState = ShState.Falling;
                else
                    Velocity.Y -= (Constants.fall_velocity * 1);
            }
            //Falling
            if (shState == ShState.Falling)
            {
                isGrounded = false;
                if (Velocity.Y < Constants.max_y_velocity)
                {
                    Velocity.Y += Constants.fall_velocity * 1.25f;
                }

                if (state != State.Damaged)
                {
                    if (currentKey.IsKeyDown(Keys.D))
                    {
                        if (Velocity.X < (Constants.max_walk_velocity - 1))  //speed up if not reach max_v
                            Velocity.X += xAcceleration;
                    }
                    else if (currentKey.IsKeyDown(Keys.A))
                    {
                        if (Velocity.X > (Constants.max_walk_velocity - 1) * -1) //speed up if not reach max_v
                            Velocity.X -= xAcceleration;
                    }
                }

            }

            Transformation_Timer += ElapsedGameTime;
            int PhaseTimer = 60;
            if (power == Power.Squishee)
                PhaseTimer = 10;

            int JumpFrame;
            if (isGrounded)
                JumpFrame = 0;
            else
                JumpFrame = 5;
            //shot power
            if (Transformation_Timer < PhaseTimer * 4)
            {
                sourceRectangle = shootAnim[0 + JumpFrame];
                if (!currentKey.IsKeyDown(Keys.T))
                {
                    state = State.Standing;
                    sourceRectangle = Jump.GetFrame(ref StateTimer);
                }
            }
            else if (Transformation_Timer < PhaseTimer * 6)
            {
                ShotPower = SlingshotStrength.Low;
                sourceRectangle = shootAnim[1 + JumpFrame];
                if (!currentKey.IsKeyDown(Keys.T))
                    ShootPellet();
            }
            else if (Transformation_Timer < PhaseTimer * 7)
            {
                ShotPower = SlingshotStrength.Mid;
                sourceRectangle = shootAnim[2 + JumpFrame];
                if (!currentKey.IsKeyDown(Keys.T))
                    ShootPellet();
            }
            else if (Transformation_Timer >= PhaseTimer * 9)
            {
                ShotPower = SlingshotStrength.Full;
                ReadyFire = true;
                int switcher = ((int)Transformation_Timer % 120);
                if (switcher <= 60)
                    sourceRectangle = shootAnim[3 + JumpFrame];
                else
                    sourceRectangle = shootAnim[2 + JumpFrame];
                if (!currentKey.IsKeyDown(Keys.T))
                    ShootPellet();
            }
            //direction
            if (currentKey.IsKeyDown(Keys.A))
            {
                movingRight = false;
            }
            if (currentKey.IsKeyDown(Keys.D))
            {
                movingRight = true;
            }
        }
        private void ShootPellet()
        {
            ShotPower = SlingshotStrength.None;
            state = State.Standing;
            if (shState == ShState.Falling || shState == ShState.Jumping)
                sourceRectangle = shootAnim[2 + 5];
            shState = ShState.Standing;
        }

        private void Skating(float ElapsedGameTime)
        {
            Jumped = false;
            Transformation_Timer += ElapsedGameTime;
            if (Velocity.X > 0)
                Velocity.X -= 0.05f;

            var timer = 90;
            SkateBoost(timer);
            //jumping
            if (currentKey.IsKeyDown(Keys.W) && !prevKey.IsKeyDown(Keys.W))
            {
                if (!Jumped)
                {
                    lastY = positionRectangle.Y;
                    Velocity.Y = Constants.jump_velocity * 2;
                    sourceRectangle = skateAnim[7];
                    Animating = false;
                    state = State.SkateJumping;
                }

            }
            if (!currentKey.IsKeyDown(Keys.A) && !currentKey.IsKeyDown(Keys.D) && currentKey.IsKeyDown(Keys.Y) && !prevKey.IsKeyDown(Keys.Y))
                state = State.Standing;

            //else if (currentKey.IsKeyDown(Keys.A))
            //{
            //    if (Velocity.X > (Constants.max_walk_velocity - 2) * -1) //speed up if not reach max_v
            //        Velocity.X -= xAcceleration;
            //}
            //else if (currentKey.IsKeyDown(Keys.D))
            //{
            //    if (Velocity.X > (Constants.max_walk_velocity - 2) * -1) //speed up if not reach max_v
            //        Velocity.X -= xAcceleration;
            //}
            //    if (Keyboard.GetState().IsKeyDown(Keys.Y) && !prevKey.IsKeyDown(Keys.Y) && state == State.Skating)
            //{
            //    startAnimation(2);
            //}

        }

        private void SkateBoost(int timer)
        {
            if (Transformation_Timer < timer)
            {
                sourceRectangle = skateAnim[0];
            }
            else if (Transformation_Timer < timer * 2)
            {
                sourceRectangle = skateAnim[1];
            }
            else if (Transformation_Timer < timer * 3)
            {
                sourceRectangle = skateAnim[2];
            }
            else if (Transformation_Timer < timer * 6)
            {
                sourceRectangle = skateAnim[3];
            }
            else if (Transformation_Timer < timer * 6)
            {
                int switcher = ((int)Transformation_Timer % 200);
                if (switcher <= 50)
                    sourceRectangle = skateAnim[2];
                else
                    sourceRectangle = skateAnim[3];
            }
            else if (Transformation_Timer >= timer * 10)
            {
                int switcher = ((int)Transformation_Timer % 300);
                if (switcher <= 100)
                    sourceRectangle = skateAnim[4];
                else
                    sourceRectangle = skateAnim[5];
            }

            if (currentKey.IsKeyDown(Keys.A) && !prevKey.IsKeyDown(Keys.A) && Transformation_Timer >= timer * 6)
            {
                Velocity.X = -5f;
                if (Velocity.X < 0)
                    movingRight = false;
                Transformation_Timer = 0;
            }
            if (currentKey.IsKeyDown(Keys.D) && !prevKey.IsKeyDown(Keys.D) && Transformation_Timer >= timer * 6)
            {
                Velocity.X += 5f;
                if (Velocity.X > 0)
                    movingRight = true;
                Transformation_Timer = 0;
            }

        }

        private void SkateJump()
        {
            Jumped = true;
            Velocity.Y += Constants.fall_velocity; //decrease Velocity.Y gradually
            if ((lastY - positionRectangle.Y) > 55)
            {
                state = State.SkateFalling;
            }


            if (currentKey.IsKeyDown(Keys.D))
            {
                if (Velocity.X < (Constants.max_walk_velocity - 2))  //speed up if not reach max_v
                    Velocity.X += xAcceleration;
            }
            else if (currentKey.IsKeyDown(Keys.A))
            {
                if (Velocity.X > (Constants.max_walk_velocity - 2) * -1) //speed up if not reach max_v
                    Velocity.X -= xAcceleration;
            }

            if (!currentKey.IsKeyDown(Keys.W) && state != State.Skating)
            {
                state = State.SkateFalling;
            }
            else
                Velocity.Y -= Constants.fall_velocity;

        }
        private void SkateFall()
        {
            if (Velocity.Y < Constants.max_y_velocity)
            {
                Velocity.Y += Constants.fall_velocity * 1.25f;
            }

            if (currentKey.IsKeyDown(Keys.D))
            {
                if (Velocity.X < (Constants.max_walk_velocity - 1))  //speed up if not reach max_v
                    Velocity.X += xAcceleration;
            }
            else if (currentKey.IsKeyDown(Keys.A))
            {
                if (Velocity.X > (Constants.max_walk_velocity - 1) * -1) //speed up if not reach max_v
                    Velocity.X -= xAcceleration;
            }
            if (currentKey.IsKeyDown(Keys.W) && !prevKey.IsKeyDown(Keys.W))
            {
                Jumped = false;
                if (!Jumped)
                {
                    lastY = positionRectangle.Y;
                    Velocity.Y = Constants.jump_velocity;
                    state = State.Jumping;
                }
            }
        }

        private void Turning()
        {
        }
        private void Squatting(float ElapsedGameTime)
        {
            Transformation_Timer += ElapsedGameTime;
            if (Velocity.X > 0)
                Velocity.X -= 1;
            if (Velocity.X < 0)
                Velocity.X += 1;
            if (Transformation_Timer < 120) //mid size
            {
                sourceRectangle = Squart[0];
                positionRectangle.Y = lastY;
            }
            else
            {
                sourceRectangle = Squart[1];
                positionRectangle.Y = lastY;
            }
            if (!currentKey.IsKeyDown(Keys.S))
                state = State.Standing;

        }

        private void Falling()
        {
            isGrounded = false;
            if (Velocity.Y < Constants.max_y_velocity)
            {
                Velocity.Y += Constants.fall_velocity * 1.25f;
            }

            if (state != State.Damaged)
            {
                if (currentKey.IsKeyDown(Keys.D))
                {
                    if (Velocity.X < (Constants.max_walk_velocity - 1))  //speed up if not reach max_v
                        Velocity.X += xAcceleration;
                }
                else if (currentKey.IsKeyDown(Keys.A))
                {
                    if (Velocity.X > (Constants.max_walk_velocity - 1) * -1) //speed up if not reach max_v
                        Velocity.X -= xAcceleration;
                }
            }
        }

        public override void XCollision(Sprite s)
        {
            switch (s.name)
            {
                case "tv":
                case "platform":
                    break;
                case "gps":
                case "brick":
                case "itemBlock":
                case "krustyos":
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
                        break;
                    }
                case "moeKnife":
                case "elf":
                case "jaggedmetal":
                case "spike":
                    {
                        if (state != State.Damaged && !isFlashing)
                        {
                            if (s.positionRectangle.Right < positionRectangle.Left + 6)
                                movingRight = false;
                            else if (positionRectangle.Right - 6 < s.positionRectangle.Left)
                                movingRight = true;
                            state = State.Damaged;
                            health++;
                        }
                        break;
                    }
                case "olmec":
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
                        if (s.Velocity.Y < 0)
                        {
                            positionRectangle.Y += (int)s.Velocity.Y;
                            positionRectangle.Y = s.positionRectangle.Top - positionRectangle.Height;
                        }
                        if (s.Velocity.Y > 0)
                        {
                            positionRectangle.Y += (int)s.Velocity.Y;
                            positionRectangle.Y = s.positionRectangle.Top - positionRectangle.Height;
                        }

                        break;
                    }
                case "butterfinger":
                    {
                        s.canRemove = true;
                        health--;
                        break;
                    }
                case "squishee":
                    s.canRemove = true;
                    power = Power.Squishee;
                    break;
                case "spraypaint":
                    {
                        s.canRemove = true;
                        break;
                    }

                case "cupcake":
                    {
                        ((Cupcake)s).state = Cupcake.State.Shocking;
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
                        break;
                    }
            }
        }
        public override void YCollision(Sprite s)
        {
            switch (s.name)
            {
                case "tv":
                case "platform":

                    {
                        if (positionRectangle.Bottom > s.positionRectangle.Top && positionRectangle.Bottom < s.positionRectangle.Top + 4 && Velocity.Y >= 0)
                        {
                            positionRectangle.Y = s.positionRectangle.Top - positionRectangle.Height;
                            Velocity.Y = 0;
                            if (state != State.Shooting)
                                state = State.Walking;
                            else
                                shState = ShState.Standing;
                        }
                        else if (positionRectangle.Bottom > (s.positionRectangle.Top + 4)
                            && (state == State.Standing || state == State.Walking
                             && (positionRectangle.Left > s.positionRectangle.Left)))
                            state = State.Falling;
                        else if (positionRectangle.Bottom > s.positionRectangle.Top && state != State.SkateJumping)
                        {

                        }
                        break;
                    }

                case "brick":
                case "gps":
                case "krustyos":
                case "moeKnife":
                    {
                        if (positionRectangle.Bottom > s.positionRectangle.Top && Velocity.Y >= 0 && state != State.SkateJumping)
                        {
                            positionRectangle.Y = (s.positionRectangle.Top - positionRectangle.Height);
                            Velocity.Y = 0;
                            if (state == State.SkateFalling)
                                state = State.Skating;
                            else if (state == State.Shooting)
                            {
                                Velocity.X = 0;
                                state = State.Shooting;
                                shState = ShState.Standing;
                            }
                            else if (state == State.Damaged)
                            {
                                state = State.Damaged;
                                if (bouncing)
                                    bounces++;
                            }
                            else if (state != State.Skating)
                                state = State.Walking;
                        }
                        isGrounded = true;
                        break;
                    }
                case "elf":
                    {
                        if (state != State.Damaged && !isFlashing)
                        {
                            if (positionRectangle.Bottom > s.positionRectangle.Top && Velocity.Y >= 0 && state != State.SkateJumping)
                            {
                                positionRectangle.Y = s.positionRectangle.Top - positionRectangle.Height;
                                Velocity.Y = 0;
                                if (state == State.SkateFalling)
                                    state = State.Skating;
                                else if (state == State.Shooting)
                                {
                                    Velocity.X = 0;
                                    state = State.Shooting;
                                }
                                else if (state != State.Skating)
                                    state = State.Walking;
                            }
                            isGrounded = true;

                        }
                        break;
                    }
                case "olmec":
                    {
                        if (positionRectangle.Bottom > s.positionRectangle.Top && state != State.SkateJumping)
                        {
                            positionRectangle.Y = s.positionRectangle.Top - positionRectangle.Height;
                            Velocity.Y = 0;
                            if (state == State.SkateFalling)
                                state = State.Skating;
                            else if (state != State.Skating)
                                state = State.Walking;
                        }
                        if (s.Velocity.Y < 0)
                            positionRectangle.Y += (int)s.Velocity.Y;

                        break;
                    }
                case "cupcake":
                    {
                        ((Cupcake)s).state = Cupcake.State.Shocking;
                        if (positionRectangle.Bottom > s.positionRectangle.Top && state != State.SkateJumping)
                        {
                            positionRectangle.Y = s.positionRectangle.Top - positionRectangle.Height;
                            Velocity.Y = 0;
                            if (state == State.SkateFalling)
                                state = State.Skating;
                            else if (state != State.Skating)
                                state = State.Walking;
                        }
                        break;
                    }
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (movingRight)
                spriteBatch.Draw(texture, positionRectangle, sourceRectangle, color);
            else
                spriteBatch.Draw(texture, positionRectangle, sourceRectangle, color, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);



            


            if (previousState != state)
                Debug.WriteLine("Bart " + state.ToString());
            if (previousPower != power)
                Debug.WriteLine("Bart " + power.ToString());
            //Debug.WriteLine("Bart flashing " + isFlashing.ToString());
            //if (lastshState != shState)
            //    Debug.WriteLine("Bart Shoot" + shState.ToString());
            //if (state == State.Damaged)
            //{
            //    Debug.WriteLine(DamageTimer.ToString());
            //    Debug.WriteLine(health.ToString());
            //}
            //if (state == State.Shooting)
            //    Debug.WriteLine(Transformation_Timer.ToString());
            //if (state == State.Jumping)
            //Debug.WriteLine("isGrounded " + isGrounded.ToString());
            //Debug.WriteLine("ShootJumping "+ ShootJumping.ToString());
            //Debug.WriteLine((lastY - positionRectangle.Y).ToString());

        }
    }
}
