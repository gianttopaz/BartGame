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
    class Spike : Enemy
    {
        public enum State { NotFalling, Falling };
        public enum Primed { NotPrimed, Primed, Triggered};
        public State state { get; set; }
        public Primed primed { get; set; }
        public State previousState;
        //if player passes, increase state
        //prevent instant max-out by flipping on boolean
        //next time player passes, it drops
        private FrameSelector DespawnFrame, NormalAnim;
        private float StateTimer, AnimationTimer, JumpTimer;
        private float xSpeed, ySpeed;
        public int StateSwitcher = 0;
        private float SwitchTimer = 0;
        private int StartFrame;
        private int StartPosition;
        private int x = 1;
        private static int Height = 16;
        private static int Width = 16;
        private static int StartX = Constants.tileSize * 9;
        private static int StartY = Constants.tileSize * 7;
        private bool passing;
        Player player;
        Rectangle playerPosition;

        public Spike(int x, int y, Player player)
        {
            name = "spike";
            positionRectangle = new Rectangle(x, y, Width, Height);
            state = State.NotFalling;
            primed = Primed.NotPrimed;
            position = new Vector2(x, y);
            StartPosition = x;
            sourceRectangle = new Rectangle(StartX, StartY, Width, Height);
            this.player = player;
            canNotCollide = false;
            passing = false;
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            playerPosition = player.positionRectangle;
            if (playerPosition.Right > position.X && playerPosition.Left < (position.X + 48))
            {
                if (!passing)
                    primed++;
                passing = true;
            }
            if (playerPosition.Left > (position.X + 48))
                passing = false;

            if (playerPosition.Right > position.X && playerPosition.Left < (position.X + 48))
                state = State.Falling;
            if (state == State.NotFalling)
            {
                if (playerPosition.Right > (position.X - 32) && playerPosition.Left < (position.X + 48))
                {
                    StateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    SwitchTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (SwitchTimer >= 0.05)
                    {
                        x *= -1;
                        SwitchTimer = 0;
                    }
                    if (StateTimer > 0.05)
                    {
                        position.X += 1 * x;
                        StateTimer = 0;
                    }
                    position.X = MathHelper.Clamp(position.X, StartPosition, StartPosition + 1);

                }

            }
            else if (state == State.Falling && primed == Primed.Triggered)
            {
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
        }

        private void BoundBox()
        {
            positionRectangle = new Rectangle((int)position.X, (int)position.Y, Width, Height);
        }

        public override void YCollision(Sprite s)
        {
            switch (s.name)
            {
                case "bart":
                    break;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, sourceRectangle, Color.White);
            Debug.WriteLine(passing.ToString());
            Debug.WriteLine(primed.ToString());
        }
    }
}