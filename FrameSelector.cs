using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BartGame
{
    class FrameSelector
    {
        //animation frame selector
        public List<Rectangle> Frames;
        private float Fps;
        public int curFrameID;
        private bool LoopBack;
        private float ResetTimer;

        public FrameSelector(float fps, List<Rectangle> frames, bool loop = true)
        {
            this.Fps = fps;
            this.Frames = frames;

            this.curFrameID = 0;
            this.LoopBack = loop;
        }

        //public FrameSelector(float fps, List<Rectangle> frames, float rt, bool loop = true)
        //{
        //    this.Fps = fps;
        //    this.Frames = frames;

        //    ResetTimer = rt;
        //    this.curFrameID = 0;
        //    this.LoopBack = loop;
        //}
        public void Reset()
        {
            curFrameID = 0;
        }
        public Rectangle GetFrame(ref float dt)
        {

            if (dt > Fps)
            {
                dt = 0;
                //start separate timer that resets on squat
                //if timer is zero, set current frame to zero
                if (curFrameID < Frames.Count)
                {
                    curFrameID++;
                }
                if (curFrameID >= Frames.Count && LoopBack)
                {
                    curFrameID = 0; //reset if exceed animation                
                }
                else if (curFrameID >= Frames.Count && !LoopBack)
                {
                    curFrameID = Frames.Count-1;
                }
            }

            //create source rectangle to draw
            return Frames[curFrameID];

        }
        public void Update(GameTime gameTime)
        {

        }

    }
}