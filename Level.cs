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
    class Level
    {
        const float unitToPixel = 100.0f;
        const float pixelToUnit = 1 / unitToPixel;

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

        public Level(World world, GraphicsDevice graphicsDevice, ContentManager content, int levelNum)
        {
            this.world = world;
            this.graphicsDevice = graphicsDevice;
            this.content = content;
            this.levelNum = levelNum;

            Initialize();
            LoadContent();
        }

        void Initialize()
        {
            edges = new Body(world);

            blank = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.Black });
        }

        void LoadContent()
        {
            background = content.Load<SoundEffect>("level" + levelNum + "back").CreateInstance();
            background.IsLooped = true;
            background.Play();

            vertices = content.Load<Vector[]>("level" + levelNum);

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

            Spawn = new Vector2(vertices[0].X, vertices[0].Y);

            portalTex = content.Load<Texture2D>("portal");
            portalRect = new Rectangle(0, 0, vertices[1].X, vertices[1].Y);
            teleport = content.Load<SoundEffect>("teleportEffect");

            MaxGravity = (float)Math.PI / 3 * vertices[2].X;
        }

        public bool IntersectsPortal(Rectangle rect)
        {
            if (portalRect.Intersects(rect) && vertices[2].Y == 1)
            {
                background.Stop(true);
                teleport.Play();
                world.RemoveBody(edges);
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < edges.FixtureList.Count; x++)
            {
                EdgeShape edge = (EdgeShape)edges.FixtureList[x].Shape;
                DrawLine(spriteBatch, blank, 2f, Color.Black, edge.Vertex1 * unitToPixel, edge.Vertex2 * unitToPixel);
            }

            if (vertices[2].Y == 1)
            {
                spriteBatch.Draw(portalTex, portalRect, Color.White);
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
