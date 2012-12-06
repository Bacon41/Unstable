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
        // Declairing necessisary objects
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont menuFont;

        KeyboardState currentKeyboard;
        KeyboardState oldKeyboard;

        int WIDTH;
        int HEIGHT;

        bool gameOver;

        // The world (for Farseer Physics) and the gravity information
        World world;
        Vector2 gravity;
        float gravityAngle;
        GravityDisplay gravityDisplay;

        // The level information
        int currentLevel;
        Level level;

        // Our stickman
        StickMan stickMan;

        // The camera object
        Camera camera;

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

            gameOver = false;

            currentKeyboard = Keyboard.GetState();
            oldKeyboard = currentKeyboard;

            // Setting the gravity to be straight down, and creating the world
            gravityAngle = 0;
            gravity = new Vector2(0f, 9.8f);
            world = new World(gravity);

            // Creating the first level
            currentLevel = 0;
            level = new Level(world, GraphicsDevice, Content, currentLevel);

            // Creating the camera and setting it to start in the center of the screen
            camera = new Camera(WIDTH, HEIGHT);
            camera.Pos = new Vector2(WIDTH / 2, HEIGHT / 2);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            menuFont = Content.Load<SpriteFont>("MenuFont");

            // Initializing the gravity display to show the player which direction gravity is pulling on the stickman
            gravityDisplay = new GravityDisplay(Content.Load<Texture2D>("images/circle"), new Vector2(50, 50));
            gravityDisplay.Position = new Vector2(WIDTH - 50, HEIGHT - 50);

            // Creating the stickman with his initial conditions (some pulled from the level)
            stickMan = new StickMan(world, Content.Load<Texture2D>("images/stickman"), new Vector2(25, 50), 1);
            stickMan.Position = level.Spawn;
            // Telling Farseer that the stickman can move and that its physics should always be calculated
            stickMan.body.BodyType = BodyType.Dynamic;
            stickMan.body.SleepingAllowed = false;
            stickMan.body.OnCollision += new OnCollisionEventHandler(stickManCollision);
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

            // A quick way to exit the game
            if (currentKeyboard.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (!gameOver)
            {
                // If the current level gravity is within the upper and lower bounds of the level's possible gravity
                if (gravityAngle <= level.MaxGravity || gravityAngle >= 2 * (float)Math.PI - level.MaxGravity)
                {
                    // Basic rotation of the level - Up and Down are 90 degrees, Left and Right are 1.5 degrees
                    if ((currentKeyboard.IsKeyDown(Keys.W) && !oldKeyboard.IsKeyDown(Keys.W))
                        || (currentKeyboard.IsKeyDown(Keys.Up) && !oldKeyboard.IsKeyDown(Keys.Up)))
                    {
                        gravityAngle += (float)Math.PI / 2;
                    }
                    if (currentKeyboard.IsKeyDown(Keys.A) || currentKeyboard.IsKeyDown(Keys.Left))
                    {
                        gravityAngle += 2 * (float)Math.PI - (float)Math.PI / 120;
                    }
                    if ((currentKeyboard.IsKeyDown(Keys.S) && !oldKeyboard.IsKeyDown(Keys.S))
                        || (currentKeyboard.IsKeyDown(Keys.Down) && !oldKeyboard.IsKeyDown(Keys.Down)))
                    {
                        gravityAngle -= (float)Math.PI / 2;
                    }
                    if (currentKeyboard.IsKeyDown(Keys.D) || currentKeyboard.IsKeyDown(Keys.Right))
                    {
                        gravityAngle += (float)Math.PI / 120;
                    }
                    // Setting the angle to be within 360 degrees and making sure that it's positive
                    gravityAngle %= 2 * (float)Math.PI;
                    gravityAngle = (gravityAngle < 0) ? 2 * (float)Math.PI + gravityAngle : gravityAngle;
                    
                    // Making sure that the hud is always showing the gravity pointing down
                    gravityDisplay.Rotation = 0;

                    // Setting the new world gravity based on the rotation
                    gravity.X = (float)(9.8 * Math.Sin(gravityAngle));
                    gravity.Y = (float)(9.8 * Math.Cos(gravityAngle));
                    world.Gravity = gravity;
                }
                // If the current angle is greater than the maximum left rotation, set it back
                if (gravityAngle > level.MaxGravity && gravityAngle < (float)Math.PI)
                {
                    gravityAngle = level.MaxGravity - .0001f;
                }
                // If the current angle is less than the minimum right rotation, set it back
                if (gravityAngle < 2 * (float)Math.PI - level.MaxGravity && gravityAngle > (float)Math.PI)
                {
                    gravityAngle = 2 * (float)Math.PI - level.MaxGravity + .001f;
                }

                // If the stickman is going slowly and it's rotated far from being normal to the plane, set it back
                if (stickMan.body.LinearVelocity.Length() < .0001f)
                {
                    if (stickMan.body.Rotation > (gravityAngle * 3 % (2 * (float)Math.PI)) + (float)Math.PI / 4
                        || stickMan.body.Rotation < (gravityAngle * 3 % (2 * (float)Math.PI)) - (float)Math.PI / 4)
                    {
                        stickMan.body.Rotation = -gravityAngle;
                    }
                }
                // Makeing sure that the stickman's rotation is less than the maximum and is positive
                stickMan.body.Rotation %= 2 * (float)Math.PI;
                stickMan.body.Rotation = (stickMan.body.Rotation < 0) ? 2 * (float)Math.PI + stickMan.body.Rotation : stickMan.body.Rotation;

                // Setting the camera to rotate to the same direction as gravity and make it follow the stick man in the center
                camera.Rotation = gravityAngle;
                camera.Pos = stickMan.Position;

                // If the stickman hits the portal, move to the next level
                if (level.IntersectsPortal(new Rectangle((int)stickMan.Position.X, (int)stickMan.Position.Y,
                    (int)(stickMan.texture.Width / 2.0f), (int)(stickMan.texture.Height / 2.0f))))
                {
                    currentLevel++;
                    newLevel();
                }

                // Step the physics forward in time
                world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            // If the keyboard is pressed, cancel the game over
            if (currentKeyboard.IsKeyDown(Keys.Space) && !oldKeyboard.IsKeyDown(Keys.Space))
            {
                gameOver = false;
            }

            oldKeyboard = currentKeyboard;
            base.Update(gameTime);
        }

        /// <summary>
        /// Our method to calculate when the stick man should die and have a game over.
        /// </summary>
        /// <param name="fixtureA">The first collision shape involved.</param>
        /// <param name="fixtureB">The second collision shape involved.</param>
        /// <param name="contact">The object documenting the actual contact between the fixtures.</param>
        /// <returns></returns>
        bool stickManCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            // If the stick man is going too fast, kill him and end the game
            if (stickMan.body.LinearVelocity.Length() > 10f)
            {
                gameOver = true;
            }
            return true;
        }

        /// <summary>
        /// The general method for creating a level.
        /// </summary>
        void newLevel()
        {
            // Create the level, reset the stick man to stationary at the spawn point, and set the camera and gravity to normal
            level = new Level(world, GraphicsDevice, Content, currentLevel);

            stickMan.Position = level.Spawn;
            stickMan.body.LinearVelocity = Vector2.Zero;
            stickMan.body.AngularVelocity = 0;
            stickMan.body.Rotation = 0;

            camera.Rotation = 0;

            gravityAngle = 0;
            gravity = new Vector2(0f, 9.8f);
            world.Gravity = gravity;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // The background color
            GraphicsDevice.Clear(Color.Gray);

            // Drawing things to the screen that will rotate with the camera (stick man and level)
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.getTransformation());
            level.Draw(spriteBatch);
            stickMan.Draw(spriteBatch);
            spriteBatch.End();

            // Drawing things to the screen that won't rotate with the camera (HUD, information text)
            spriteBatch.Begin();
            gravityDisplay.Draw(spriteBatch);
            if (gameOver)
            {
                spriteBatch.DrawString(menuFont, "Game Over!", new Vector2(WIDTH / 2 - 40, HEIGHT / 2 - 10), Color.White);
                spriteBatch.DrawString(menuFont, "(Esc to exit)", new Vector2(WIDTH / 2 - 55, HEIGHT / 2 + 20), Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
