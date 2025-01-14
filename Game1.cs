using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace BartGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Map currentMap;
        Camera camera;
        KeyboardState currentKeyDown, prevKeyDown;
        private bool pause;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = Constants.viewHeight;
            _graphics.PreferredBackBufferWidth = Constants.viewWidth;
            Content.RootDirectory = "Content";
            currentMap = new Map1_2();
            IsMouseVisible = true;
            pause = false;
        }

        protected override void Initialize()
        {
            camera = new Camera();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            currentMap.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            prevKeyDown = currentKeyDown;
            currentKeyDown = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (currentKeyDown.IsKeyDown(Keys.Enter) && !prevKeyDown.IsKeyDown(Keys.Enter))
                currentMap.InitSprites();


            if (currentKeyDown.IsKeyDown(Keys.P) && !prevKeyDown.IsKeyDown(Keys.P))
            {
                Debug.WriteLine(pause.ToString());
                if (!pause)
                pause = true;
                else
                pause = false;
            }

            if (!pause)
            {
            currentMap.Update(camera, gameTime);
            camera.Update();
            }
            
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, camera.transform);
            currentMap.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
