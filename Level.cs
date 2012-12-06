using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using DataType;

namespace Unstable
{
    /// <summary>
    /// The class that handles reading in the level information and tracking all relevent data.
    /// </summary>
    class Level
    {
        // The conversion factor between Farseer units (meters) and pixels
        const float unitToPixel = 100.0f;
        const float pixelToUnit = 1 / unitToPixel;

        // Required objects
        GraphicsDevice graphicsDevice;
        World world;
        ContentManager content;

        Body edges;
        Vector[] vertices;
        Texture2D blank;
        Texture2D portalTex;
        Rectangle portalRect;
        int levelNum;

        SoundEffectInstance background;
        SoundEffect teleport;

        public float MaxGravity;
        public Vector2 Spawn;

        /// <summary>
        /// Constructor that takes in the information necessiary to draw and load in sounds, and know which level it is.
        /// </summary>
        /// <param name="world">The Farseer Physics information object.</param>
        /// <param name="graphicsDevice">The object that is required to create a texture.</param>
        /// <param name="content">The object that is used to load in data (XML and .wav)</param>
        /// <param name="levelNum">The actual level that this is.</param>
        public Level(World world, GraphicsDevice graphicsDevice, ContentManager content, int levelNum)
        {
            // Saving the information for use later and setting the world up
            this.world = world;
            this.graphicsDevice = graphicsDevice;
            this.content = content;
            this.levelNum = levelNum;

            Initialize();
            LoadContent();
        }

        /// <summary>
        /// Initializing objects that aren't loaded from content.
        /// </summary>
        void Initialize()
        {
            // Creating the edges for the world and creating the black texture
            edges = new Body(world);

            blank = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
        }

        /// <summary>
        /// Initializing and setting up any objects that are content bound.
        /// </summary>
        void LoadContent()
        {
            // Loading in the background music
            background = content.Load<SoundEffect>("level" + levelNum + "back").CreateInstance();
            background.IsLooped = true;
            background.Play();

            // Loading in the file containing the level setup information
            vertices = content.Load<Vector[]>("level" + levelNum);

            // Building the world edges from the loaded in file and giving them physics
            Vertices terrain = new Vertices();
            for (int x = 3; x < vertices.Length; x++)
            {
                terrain.Add(new Vector2(vertices[x].X, vertices[x].Y) * pixelToUnit);
            }
            for (int i = 0; i < terrain.Count - 1; ++i)
            {
                FixtureFactory.AttachEdge(terrain[i], terrain[i + 1], edges);
            }
            edges.Friction = 0.6f;

            // Setting the spawn point from the file
            Spawn = new Vector2(vertices[0].X, vertices[0].Y);

            // Loading in the information for the portal's texture, where it should go, and what sound it makes
            portalTex = content.Load<Texture2D>("portal");
            portalRect = new Rectangle(0, 0, vertices[1].X, vertices[1].Y);
            teleport = content.Load<SoundEffect>("teleportEffect");

            // Setting the maximum gravity angle based on the loaded information
            MaxGravity = (float)Math.PI / 3 * vertices[2].X;
        }

        /// <summary>
        /// Allows the game class to check if the stick man has colided with the portal.
        /// </summary>
        /// <param name="rect">The location of the stick man.</param>
        /// <returns>Whether or not the stick man has reached the portal.</returns>
        public bool IntersectsPortal(Rectangle rect)
        {
            // If the stick man has reached the portal and the portal is being drawn, play the portal sound and remove the collision
            if (portalRect.Intersects(rect) && vertices[2].Y == 1)
            {
                background.Stop(true);
                teleport.Play();
                world.RemoveBody(edges);
                return true;
            }
            return false;
        }

        /// <summary>
        /// The method that will draw the level components to the screen (not the stick man).
        /// </summary>
        /// <param name="spriteBatch">The object that allows drawing.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Call the method to draw all of the lines that edge the level
            for (int x = 0; x < edges.FixtureList.Count; x++)
            {
                EdgeShape edge = (EdgeShape)edges.FixtureList[x].Shape;
                DrawLine(spriteBatch, 5f, Color.Black, edge.Vertex1 * unitToPixel, edge.Vertex2 * unitToPixel);
            }

            // If the portal is supported in the level, draw it
            if (vertices[2].Y == 1)
            {
                spriteBatch.Draw(portalTex, portalRect, Color.White);
            }
        }

        /// <summary>
        /// The method to draw the lines for the level edging.
        /// </summary>
        /// <param name="batch">The object that allows the drawing.</param>
        /// <param name="width">The thickness of the line.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="point1">The start point for the line.</param>
        /// <param name="point2">The end point for the line.</param>
        void DrawLine(SpriteBatch batch, float width, Color color, Vector2 point1, Vector2 point2)
        {
            // The angle between the two points (to get the rotation of the texture) and the distance between them
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);
            blank.SetData(new[] { color });

            // Drawing the empty texture from point1 to point2
            batch.Draw(blank, point1, null, color, angle, Vector2.Zero, new Vector2(length, width), SpriteEffects.None, 0);
        }
    }
}
