using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// The class that contains all of the information that the stick man needs to keep track of.
    /// </summary>
    public class StickMan
    {
        // Conversion factor between Farseer units (meters) and pixels
        public const float unitToPixel = 100.0f;
        public const float pixelToUnit = 1 / unitToPixel;
 
        // The objects that are required for the stick man to track
        public Body body;

        /// <summary>
        /// Getting and setting the stick man's position (and converting between units).
        /// </summary>
        public Vector2 Position
        {
            get { return body.Position * unitToPixel; }
            set { body.Position = value * pixelToUnit; }
        }
 
        public Texture2D texture;
 
        private Vector2 size;
        /// <summary>
        /// Getting and setting the stick man's size (and converting between units).
        /// </summary>
        public Vector2 Size
        {
            get { return size * unitToPixel; }
            set { size = value * pixelToUnit; }
        }
 
        /// <summary>
        /// The constructor that initializes the pysics for the stick man and his texture.
        /// </summary>
        /// <param name="world">The Farseer world that the game uses.</param>
        /// <param name="texture">The texture that the stick man will use.</param>
        /// <param name="size">The size of the stick man.</param>
        /// <param name="mass">The mass of the stick man.</param>
        public StickMan(World world, Texture2D texture, Vector2 size, float mass)
        {
            // Creating the physics body for the stick man
            body = BodyFactory.CreateRectangle(world, size.X * pixelToUnit, size.Y * pixelToUnit, 1);
 
            this.Size = size;
            this.texture = texture;
        }
 
        /// <summary>
        /// The method that will allow us to draw the stick man to the screen.
        /// </summary>
        /// <param name="spriteBatch">The object that allows us to draw.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Getting the scale of the stick man and drawing him to the screen
            Vector2 scale = new Vector2(Size.X / (float)texture.Width, Size.Y / (float)texture.Height);

            spriteBatch.Draw(texture, Position, null, Color.White, body.Rotation,
                new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), scale, SpriteEffects.None, 0);
        }
    }
}
