using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Unstable
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont menuFont;

        KeyboardState currentKeyboard;
        KeyboardState oldKeyboard;

        int WIDTH;
        int HEIGHT;

        World world;
        Vector2 gravity;
        float gravityAngle;
        GravityDisplay gravityDisplay;

        Level level1;

        DrawablePhysicsObject stickMan;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            WIDTH = GraphicsDevice.Viewport.Width;
            HEIGHT = GraphicsDevice.Viewport.Height;

            currentKeyboard = Keyboard.GetState();
            oldKeyboard = currentKeyboard;

            gravityAngle = 0;
            gravity = new Vector2(0f, 9.8f);
            world = new World(gravity);

            level1 = new Level(world, GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            menuFont = Content.Load<SpriteFont>("MenuFont");

            gravityDisplay = new GravityDisplay(Content.Load<Texture2D>("circle"), new Vector2(50, 50));
            gravityDisplay.Position = new Vector2(WIDTH - 50, HEIGHT - 50);

            stickMan = new DrawablePhysicsObject(world, Content.Load<Texture2D>("white"), new Vector2(50, 50), 1);
            stickMan.Position = new Vector2(300, 10);
            stickMan.body.BodyType = BodyType.Dynamic;
            stickMan.body.SleepingAllowed = false;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            currentKeyboard = Keyboard.GetState();

            if (currentKeyboard.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (currentKeyboard.IsKeyDown(Keys.W) || currentKeyboard.IsKeyDown(Keys.Up))
            {
                gravityAngle = (float)Math.PI;
            }
            if (currentKeyboard.IsKeyDown(Keys.A) || currentKeyboard.IsKeyDown(Keys.Left))
            {
                gravityAngle += 2 * (float)Math.PI - (float)Math.PI / 120;
            }
            if (currentKeyboard.IsKeyDown(Keys.S) || currentKeyboard.IsKeyDown(Keys.Down))
            {
                gravityAngle = 0;
            }
            if (currentKeyboard.IsKeyDown(Keys.D) || currentKeyboard.IsKeyDown(Keys.Right))
            {
                gravityAngle += (float)Math.PI / 120;
            }
            gravityAngle %= 2 * (float)Math.PI;

            gravityDisplay.Rotation = -gravityAngle;

            gravity.X = (float)(9.8 * Math.Sin(gravityAngle));
            gravity.Y = (float)(9.8 * Math.Cos(gravityAngle));
            //world.Gravity = gravity;

            level1.MoveEdge(gravityAngle);

            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            oldKeyboard = currentKeyboard;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            stickMan.Draw(spriteBatch);
            gravityDisplay.Draw(spriteBatch);

            level1.Draw(spriteBatch);
            level1.DrawText(spriteBatch, menuFont, Color.Black, new Vector2(0, 30));

            // Debug Printout
            spriteBatch.DrawString(menuFont, "" + gravityAngle, new Vector2(10, 10), Color.Black);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
