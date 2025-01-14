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
    class TV : Enemy
    {
        public enum State { FullHP, Hit, Breaking, Broken };
        public State state { get; set; }
        public State previousState;
        private FrameSelector FullHPAnim, HitAnim, BreakingAnim, BrokeFrame;
        private List<Sprite> TVelfList;
        private List<Sprite> SpawnQueue;
        private float StateTimer, AnimationTimer, JumpTimer;
        public int StateSwitcher = 0;
        public static int TVHeight = 32;
        public static int TVWidth = 32;
        private static int TVStartX = Constants.tileSize * 12;
        private static int TVStartY = Constants.tileSize * 3;
        private float spawnTimer = 0;
        private float spawnTickTimer = 0;

        public TV(int x, int y, List<Sprite> elfList)
        {
            name = "tv";
            positionRectangle = new Rectangle(x, y, TVWidth, TVHeight);
            state = State.FullHP;
            AnimationTimer = 0;
            Initialize();
            TVelfList = new List<Sprite>();
            foreach (Sprite s in elfList)
                TVelfList.Add(s);
        }

        private void Initialize()
        {
            int i;
            //running
            List<Rectangle> TVFullHPList = new List<Rectangle>();
            for (i = 0; i < 2; i++) // id = 1, 2, 3
            {
                TVFullHPList.Add(new Rectangle((TVStartX) + (TVWidth * (i)), TVStartY + (TVWidth * (1)), TVWidth, TVHeight));
            }
            FullHPAnim = new FrameSelector(0.02f, TVFullHPList);
            //Squatting
            List<Rectangle> TVHitList = new List<Rectangle>();
            for (i = 0; i < 2; i++) // id = 1, 2, 3
            {
                TVHitList.Add(new Rectangle((TVStartX + 64) + (TVWidth * (i)), TVStartY + (TVWidth * (1)), TVWidth, TVHeight));
            }
            HitAnim = new FrameSelector(0.02f, TVHitList);
            //Jump/Hit
            List<Rectangle> TVBreakingList = new List<Rectangle>();
            for (i = 0; i < 4; i++) // id = 1, 2, 3
            {
                TVBreakingList.Add(new Rectangle((TVStartX) + (TVWidth * (i)), TVStartY, TVWidth, TVHeight));
            }
            TVBreakingList.Add(new Rectangle((TVStartX) + (TVWidth * (4)), TVStartY, TVWidth, TVHeight));

            BreakingAnim = new FrameSelector(0.1f, TVBreakingList);
            List<Rectangle> TVBrokenList = new List<Rectangle>();
            TVBrokenList.Add(new Rectangle((TVStartX) + (TVWidth * (5)), TVStartY, TVWidth, TVHeight));
            BrokeFrame = new FrameSelector(0.1f, TVBrokenList);
        }
        public Rectangle GetFrame()
        {
            switch (state)
            {
                case State.FullHP:
                    {
                        StateTimer += 0.01f;
                        return FullHPAnim.GetFrame(ref StateTimer);
                    }
                case State.Hit:
                    {
                        StateTimer += 0.01f;

                        return HitAnim.GetFrame(ref StateTimer);
                    }
                case State.Breaking:
                    {
                        StateTimer += 0.01f;

                        return BreakingAnim.GetFrame(ref StateTimer);
                    }
                case State.Broken:
                    {
                        return BrokeFrame.GetFrame(ref StateTimer);

                    }
                default: return new Rectangle();
            }
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            previousState = state;

            if (StateSwitcher < 1)
                state = State.FullHP;
            else if (StateSwitcher < 3)
                state = State.Hit;
            else if (StateSwitcher < 4)
            {
                AnimationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (AnimationTimer < 1.5)
                    state = State.Breaking;
                else if (AnimationTimer > 1.5)
                    state = State.Broken;

            }
            
            spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            ElfSpawn();

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


        private void ElfSpawn()
        {
            foreach (Sprite elf in TVelfList)
            {
                if (((ElfWeird)elf).state == ElfWeird.State.Spawning)
                {
                    elf.position = new Vector2(positionRectangle.X + 8, positionRectangle.Y);
                }

            }

            if ((state == State.FullHP || state == State.Hit) && spawnTimer > 4)
            {
                foreach (Sprite elf in TVelfList)
                {
                    spawnTimer = 0;
                    if (((ElfWeird)elf).state == ElfWeird.State.Spawning)
                    {
                        ((ElfWeird)elf).state = ElfWeird.State.Spawned;
                    }
                }
                //SpawnQueue.Dequeue();
            }

            if (state == State.Broken)
            {
                foreach (Sprite elf in TVelfList)
                {
                    ((ElfWeird)elf).respawning = false;
                }
            }


        }

        //END OF UPDATE

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, positionRectangle, GetFrame(), Color.White);
            if (previousState != state)
                Debug.WriteLine("TV " + state.ToString());
        }

    }

}
