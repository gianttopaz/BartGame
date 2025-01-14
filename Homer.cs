using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BartGame
{
    public class Homer : Enemy
    {
        //private static Texture2D texture;
        private float timer;
        private int changeTimer = 3;
        private int lastY, lastYframe;
        private int lastX, lastXframe;
        private float StateTimer;
        private float MoveTimer;
        //public enum Level { Small, Big, Fire };
        public enum State { Standing, Walking, Turn, Jumping, Landing, Grabbing, Strangling, MissedGrab, Squat, Damaged, Dying, Drinking, ThrowBeer, Slided };
        private FrameSelector homerIdle, homerWalk, homerTurn, homerJump, homerGrab;
        private List<Rectangle> toBig, toSmall, grabAnim;
        private List<Rectangle> intersections;
        public bool Jumped, Speedup;
        public bool isTransforming, isInvisible, HurtInvisible, isGrabbing;
        Vector2 SpawnPoint;

        public Homer(int x, int y)
        {
            Initialize();
        }

        public void Initialize()
        {
            List<Rectangle> stList = new List<Rectangle>();
            stList.Add(new Rectangle(0, 0, 48, 80)); //id = 0
            homerIdle = new FrameSelector(0.1f, stList);

            //walking 
            List<Rectangle> walkList = new List<Rectangle>();
            walkList.Add(new Rectangle(0, 0, 48, 80)); //id = 1
            walkList.Add(new Rectangle(48, 0, 48, 80)); //id = 2
            walkList.Add(new Rectangle(96, 0, 48, 80)); //id = 3
            homerWalk = new FrameSelector(0.095f, walkList);

            //Turn
            List<Rectangle> tnList = new List<Rectangle>();
            tnList.Add(new Rectangle(0, 0, 48, 80)); //id = 4
            homerTurn = new FrameSelector(0.1f, tnList);

            //Jump
            List<Rectangle> jpList = new List<Rectangle>();
            jpList.Add(new Rectangle(0, 0, 48, 80)); //id = 5
            homerJump = new FrameSelector(0.1f, jpList);

            //Grab
            List<Rectangle> grbList = new List<Rectangle>();
            grbList.Add(new Rectangle(0, 80, 80, 48)); //id = 5
            grbList.Add(new Rectangle(80, 80, 80, 48)); //id = 5
            grbList.Add(new Rectangle(160, 80, 80, 48)); //id = 5
            homerGrab = new FrameSelector(0.1f, grbList);

            this.grabAnim = new List<Rectangle>(3);
            grabAnim.Add(new Rectangle(0, 80, 80, 48)); //id = 5
            grabAnim.Add(new Rectangle(80, 80, 80, 48)); //id = 5
            grabAnim.Add(new Rectangle(160, 80, 80, 48)); //id = 5
        }


        //public override void LoadContent(ContentManager content)
        //{
        //    texture = content.Load<Texture2D>("HomerSheet");
        //}

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            Move();

            if (timer >= changeTimer && !isGrabbing)
            {
                timer = 0;
                //Velocity.X = -32f;

                isGrabbing = true;
            }
            if (timer >= changeTimer && isGrabbing)
            {
                timer = 0;
                //Velocity.X = -32f;

                isGrabbing = false;
            }

            SetAnimations();

        }

        private void Move()
        {
            Velocity.Y += 0.5f;
            Velocity.Y = Math.Min(Velocity.Y, 6.0f);

            //if (!isDead)
            //{
            //    if (movingRight)
            //        Velocity.X = Speed;
            //    else if (!movingRight)
            //        Velocity.X = -Speed;
            //}
            //else if (isDead)
            Velocity.X = 0;
        }
        public void Grabbing(float ElapsedGameTime)
        {
            StateTimer += ElapsedGameTime;

            if (StateTimer < 120) //mid size
            {
                sourceRectangle = grabAnim[0];
                positionRectangle.Y = lastY + 32;
                if (movingRight)
                    positionRectangle.X = lastX + 16;
                else
                    positionRectangle.X = lastX - 32;
            }
            else if (StateTimer < 240) //small
            {
                sourceRectangle = grabAnim[1];
                positionRectangle.Y = lastY + 32;
                if (movingRight)
                    positionRectangle.X = lastX + 16;
                else
                    positionRectangle.X = lastX - 32;
            }
            else if (StateTimer < 360) //mid size
            {
                sourceRectangle = grabAnim[2];
                positionRectangle.Y = lastY + 32;
                if (movingRight)
                    positionRectangle.X = lastX + 16;
                else
                    positionRectangle.X = lastX - 32;
            }
            else if (StateTimer < 480) //small
            {
                sourceRectangle = grabAnim[0];
                positionRectangle.Y = lastY + 32;
                if (movingRight)
                    positionRectangle.X = lastX + 16;
                else
                    positionRectangle.X = lastX - 32;
            }
            //grab over
            else
            {
                //positionRectangle.Y = lastY;
                //positionRectangle.X = lastX;
                positionRectangle.Y = lastY;
                positionRectangle.X = lastX;
                //sourceRectangle = Stand.GetFrame(ref StateTimer);
                //state = State.Standing;
                StateTimer = 0;
                isTransforming = false;
                isGrabbing = false;
            }
        }
        protected static void SetAnimations()
        {
        }

        public override void Draw(GameTime gametime, SpriteBatch spriteBatch)
        {
            if (movingRight)
                spriteBatch.Draw(texture, positionRectangle, sourceRectangle, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            else
                spriteBatch.Draw(texture, positionRectangle, sourceRectangle, Color.White);
        }


    }
}
