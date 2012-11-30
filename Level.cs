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
        public const float unitToPixel = 100.0f;
        public const float pixelToUnit = 1 / unitToPixel;

        int WIDTH;
        int HEIGHT;

        Body edges;
        Texture2D blank;

        Vector2 edgeVertex1;
        Vector2 edgeVertex2;

        List<DrawablePhysicsObject> walls;
        float prevAngle;

        public Level(World world, GraphicsDevice graphicsDevice)
        {
            WIDTH = graphicsDevice.Viewport.Width;
            HEIGHT = graphicsDevice.Viewport.Height;

            edges = new Body(world);
            walls = new List<DrawablePhysicsObject>();

            blank = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.Black });

            Vertices terrain = new Vertices();
            terrain.Add(new Vector2(0, 0) * pixelToUnit);
            terrain.Add(new Vector2(WIDTH, 0) * pixelToUnit);
            terrain.Add(new Vector2(WIDTH, HEIGHT) * pixelToUnit);
            terrain.Add(new Vector2(0, HEIGHT) * pixelToUnit);
            terrain.Add(new Vector2(0, 0) * pixelToUnit);
            for (int i = 0; i < terrain.Count - 1; ++i)
            {
                //FixtureFactory.AttachEdge(terrain[i], terrain[i + 1], edges);
            }
            edges.Friction = 0.6f;

            DrawablePhysicsObject wall = new DrawablePhysicsObject(world, blank, new Vector2(WIDTH, 2), 1);
            wall.body.BodyType = BodyType.Static;
            wall.Position = new Vector2(WIDTH / 2, HEIGHT);
            walls.Add(wall);
            wall = new DrawablePhysicsObject(world, blank, new Vector2(2, HEIGHT), 1);
            wall.body.BodyType = BodyType.Static;
            wall.Position = new Vector2(0, HEIGHT / 2);
            walls.Add(wall);
            wall = new DrawablePhysicsObject(world, blank, new Vector2(2, HEIGHT), 1);
            wall.body.BodyType = BodyType.Static;
            wall.Position = new Vector2(100, 100);
            wall.body.Rotation = (float)Math.PI / 4;
            walls.Add(wall);
            wall = new DrawablePhysicsObject(world, blank, new Vector2(2, HEIGHT), 1);
            wall.body.BodyType = BodyType.Static;
            wall.Position = new Vector2(WIDTH, HEIGHT / 2);
            walls.Add(wall);
            wall = new DrawablePhysicsObject(world, blank, new Vector2(WIDTH, 2), 1);
            wall.body.BodyType = BodyType.Static;
            wall.Position = new Vector2(WIDTH / 2, 0);
            walls.Add(wall);

            prevAngle = 0;
        }

        public void MoveEdge(float angle)
        {
            /*
            edgeVertex1 = ((EdgeShape)edges.FixtureList[index].Shape).Vertex1;
            edgeVertex2 = ((EdgeShape)edges.FixtureList[index].Shape).Vertex2;
            Vector2 centerOfScreen = new Vector2(WIDTH / 2, HEIGHT / 2);
            Vector2 centerOfEdge = new Vector2((edgeVertex1.X + edgeVertex2.X) / 2, (edgeVertex1.Y + edgeVertex2.Y) / 2);
            float halfLength = Vector2.Distance(edgeVertex1, edgeVertex2) / 2;
            float distanceFromScreen = Vector2.Distance(centerOfScreen, centerOfEdge);

            edges.DestroyFixture(edges.FixtureList[index]);
            FixtureFactory.AttachEdge(edgeVertex1, edgeVertex2, edges);
             */
            Vector2 centerOfScreen = new Vector2(WIDTH / 2, HEIGHT / 2);
            for (int x = 0; x < walls.Count; x++)
            {
                walls[x].body.Rotation += angle - prevAngle;
                Vector2 pos = walls[x].Position;
                Vector2 newPos = Vector2.Zero;
                
                newPos.X = (float)Math.Cos(angle-prevAngle) * (pos.X - centerOfScreen.X)
                    - (float)Math.Sin(angle-prevAngle) * (pos.Y - centerOfScreen.Y) + centerOfScreen.X;
                newPos.Y = (float)Math.Sin(angle-prevAngle) * (pos.X - centerOfScreen.X)
                    + (float)Math.Cos(angle-prevAngle) * (pos.Y - centerOfScreen.Y) + centerOfScreen.Y;
                walls[x].Position = newPos;
            }
            prevAngle = angle;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < edges.FixtureList.Count; x++)
            {
                EdgeShape edge = (EdgeShape)edges.FixtureList[x].Shape;
                DrawLine(spriteBatch, blank, 2f, Color.Black, edge.Vertex1 * unitToPixel, edge.Vertex2 * unitToPixel);
            }

            for (int x = 0; x < walls.Count; x++)
            {
                walls[x].Draw(spriteBatch);
            }
        }

        public void DrawText(SpriteBatch batch, SpriteFont font, Color color, Vector2 position)
        {
            batch.DrawString(font, walls[3].Position.X+ ", " +walls[3].Position.Y, position, color);
        }

        void DrawLine(SpriteBatch batch, Texture2D blank, float width, Color color, Vector2 point1, Vector2 point2)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            batch.Draw(blank, point1, null, color, angle, Vector2.Zero, new Vector2(length, width), SpriteEffects.None, 0);
        }
    }
}
