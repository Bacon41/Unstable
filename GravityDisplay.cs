using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// This is the class that handles the information for the gravity HUD indicator
    /// </summary>
    class GravityDisplay
    {
        // The objects that the indicator needs to keep track of
        public Texture2D Texture;

        public Vector2 Position;
        public Vector2 Size;

        public float Rotation;

        /// <summary>
        /// Constructor setting up the given texture and size.
        /// </summary>
        /// <param name="texture">The texture to draw (circle with line).</param>
        /// <param name="size">The size of the HUD element.</param>
        public GravityDisplay(Texture2D texture, Vector2 size)
        {
            // Saving the information
            Texture = texture;
            Size = size;
        }

        /// <summary>
        /// Drawing the HUD element to the screen.
        /// </summary>
        /// <param name="spriteBatch">The object that is used to draw.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Calculating the scale of the element and drawing it
            Vector2 scale = new Vector2(Size.X / (float)Texture.Width, Size.Y / (float)Texture.Height);
            spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, new Vector2(Texture.Width / 2.0f, Texture.Height / 2.0f), scale, SpriteEffects.None, 0);
        }
    }
}
