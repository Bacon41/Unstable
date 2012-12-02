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

namespace Unstable
{
    class Level
    {
        const float unitToPixel = 100.0f;
        const float pixelToUnit = 1 / unitToPixel;

        int WIDTH;
        int HEIGHT;

        Body edges;
        Texture2D blank;

        List<DrawablePhysicsObject> objects;
        float prevAngle;

        public float MaxGravity;

        public Level(World world, GraphicsDevice graphicsDevice)
        {
            WIDTH = graphicsDevice.Viewport.Width;
            HEIGHT = graphicsDevice.Viewport.Height;

            edges = new Body(world);
            objects = new List<DrawablePhysicsObject>();

            blank = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.Black });

            Vertices terrain = new Vertices();
            terrain.Add(new Vector2(0, 0) * pixelToUnit);
            terrain.Add(new Vector2(50, 300) * pixelToUnit);
            terrain.Add(new Vector2(50, HEIGHT) * pixelToUnit);
            terrain.Add(new Vector2(WIDTH - 50, HEIGHT) * pixelToUnit);
            terrain.Add(new Vector2(WIDTH, 200) * pixelToUnit);
            terrain.Add(new Vector2(WIDTH, 0) * pixelToUnit);
            terrain.Add(new Vector2(300, 0) * pixelToUnit);
            terrain.Add(new Vector2(250, 50) * pixelToUnit);
            terrain.Add(new Vector2(200, 0) * pixelToUnit);
            terrain.Add(new Vector2(0, 0) * pixelToUnit);
            for (int i = 0; i < terrain.Count - 1; ++i)
            {
                FixtureFactory.AttachEdge(terrain[i], terrain[i + 1], edges);
            }
            edges.Friction = 0.6f;

            prevAngle = 0;

            MaxGravity = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < edges.FixtureList.Count; x++)
            {
                EdgeShape edge = (EdgeShape)edges.FixtureList[x].Shape;
                DrawLine(spriteBatch, blank, 2f, Color.Black, edge.Vertex1 * unitToPixel, edge.Vertex2 * unitToPixel);
            }

            for (int x = 0; x < objects.Count; x++)
            {
                objects[x].Draw(spriteBatch);
            }
        }

        void DrawLine(SpriteBatch batch, Texture2D blank, float width, Color color, Vector2 point1, Vector2 point2)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            batch.Draw(blank, point1, null, color, angle, Vector2.Zero, new Vector2(length, width), SpriteEffects.None, 0);
        }
    }
}
