using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BartGame
{
    public class Pellet : Sprite
    {
        private static Texture2D texture;
        private const float fallVelocity = 0.15f;
        private const float pelletSpeed = 3f;
        private int startX;
        private float rawPellet;

        public Pellet(int x, int y, float z, GameTime gameTime, bool right)
        {

            if (right) Velocity.X = pelletSpeed * z;
            else Velocity.X = -pelletSpeed * z;
            rawPellet = z;
            Velocity.Y = -2f;
            position = new Vector2 (x, y);
            startX = x;
            sourceRectangle = new Rectangle(x, y, texture.Width * (int)z, texture.Height);
            name = "pellet";
        }
        public static void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("pellet");
        }
        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            Sprite s;
            Velocity.Y += fallVelocity * 2 ;
            position.Y += Velocity.Y;
            position.X += Velocity.X;
            BoundBox();
            s = CheckCollision(sprites);
            if (s != null) XCollision(s);
            if (s != null) YCollision(s);
            if (position.X > (startX + 165))
                canRemove = true;
        }

        private void BoundBox()
        {
            positionRectangle = new Rectangle((int)position.X, (int)position.Y, 3, 3);
        }

        public override void XCollision(Sprite s)
        {
            switch (s.name)
            {
                case "tv":
                    {
                        canRemove = true;
                        ((TV)s).StateSwitcher++;
                        break;
                    }
                case "krustyos":
                    {
                        canRemove = true;
                        ((KrustyOs)s).StateSwitcher++;
                        break;
                    }
                case "jaggedmetal":
                    {
                        canRemove = true;
                        ((JaggedMetal)s).NeedBoost = true;
                        ((JaggedMetal)s).state = JaggedMetal.State.Normal;
                        break;
                    }
                case "player": break;
                case "gps":
                    {

                        canRemove = true;
                        break;
                    }
                case "brick":
                    {
                        canRemove = true;
                        if (Velocity.X < -10 || Velocity.X > 10)
                            ((Brick)s).broken = true;
                        break;
                    }
                case "elf":
                    {

                        canRemove = true;
                        //if vel.x < 0
                        //bounce left
                        //if vel.x > 0
                        //bounce right
                        if (Velocity.X < 0)
                            ((ElfWeird)s).movingRight = true;
                        else
                            ((ElfWeird)s).movingRight = false;


                        ((ElfWeird)s).state = ElfWeird.State.Die;
                        break;
                    }
                case "moeKnife":
                    {
                        canRemove = true;
                        if (s.movingRight == true && Velocity.X > 0 || s.movingRight == false && Velocity.X < 0)
                        {
                            ((MoeKnife)s).Damaged();
                            ((MoeKnife)s).StateSwitcher++;
                            break;
                        } 
                        if (s.movingRight == false && Velocity.X > 0 || s.movingRight == true && Velocity.X < 0)
                        {
                            ((MoeKnife)s).Parried();
                            break;
                        } 
                        break;
                    }
            }
        }

        public override void YCollision(Sprite s)
        {
            switch (s.name)
            {
                case "player": break;
                case "gps":
                case "brick":
                case "moeKnife":
                    {
                        canRemove = true;
                        break;
                    }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, sourceRectangle, Color.White);
        }

    }
}
