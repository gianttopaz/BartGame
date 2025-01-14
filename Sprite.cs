using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BartGame
{
    public class Sprite : Component
    {
        public bool hidden { get; set; }
        public bool canNotCollide { get; set; }
        public bool isFlashing { get; set; }
        public string name;
        public bool spawned;
        public Vector2 position;
        public Color color = Color.White;
        public bool canRemove;
        public Rectangle positionRectangle;
        protected Rectangle sourceRectangle;
        public Vector2 Velocity;
        public Vector2 Start;
        protected float xAcceleration;
        protected float yAcceleration;
        public float flashSwitcher = 0;
        public float flashLength = 0;
        public float flashSpeed = 0;
        public float flashLengthMax = 0;
        private int lastY, lastYframe;
        private int lastX, lastXframe;
        public static int tileSize = Constants.tileSize;
        public bool movingRight;
        public Random random = new Random();

        public Rectangle PlayerPosition { get; set; }

        public Sprite()
        {

        }
        public Sprite(Rectangle position)
        {
            positionRectangle = position;
            name = "gps";
        }
        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            
        }
        public override void Update(GameTime gameTime, List<Sprite> sprites, List<Sprite> elfList)
        {

        }
        public void CheckPlayer(List<Sprite> sprites)
        {
            foreach (var player in sprites)
            {
                if (player is Player)
                {
                    PlayerPosition = player.positionRectangle;
                }
            }
        }
        public virtual Sprite CheckCollision(List<Sprite> sprites)
        {
            foreach (Sprite s in sprites)
            {
                if (this == s) continue;
                if (s.canNotCollide || canNotCollide) continue;
                if (positionRectangle.Intersects(s.positionRectangle))
                {
                    return s;
                }
            }
            return null;
        }
        public virtual void StartFlashing(float speed = 0.05f, float duration = 2)
        {
            flashLengthMax = duration;
            flashSpeed = speed;
            isFlashing = true;
        }
        public virtual void Flashing(GameTime gameTime, float speed = 0.05f, float duration = 2)
        {
            flashLength += (float)gameTime.ElapsedGameTime.TotalSeconds;
            flashSwitcher += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (flashLength < duration)
            {
                isFlashing = true;
            }
            else
            {
                isFlashing = false;
                flashLength = 0;
            }

            float switchTimer = speed;
            if (flashSwitcher < switchTimer * 3)
                color = Color.White;
            else if (flashSwitcher < switchTimer * 5)
                color = Color.Transparent;
            else if (flashSwitcher >= switchTimer * 5)
                flashSwitcher = 0;
        }
        public virtual void XCollision(Sprite s)
        {
        }
        public virtual void YCollision(Sprite s)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
        }

    }
}