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
    class GravityDisplay
    {
        public Texture2D Texture;

        public Vector2 Position;
        public Vector2 Size;

        public float Rotation;

        public GravityDisplay(Texture2D texture, Vector2 size)
        {
            Texture = texture;
            Size = size;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 scale = new Vector2(Size.X / (float)Texture.Width, Size.Y / (float)Texture.Height);
            spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, new Vector2(Texture.Width / 2.0f, Texture.Height / 2.0f), scale, SpriteEffects.None, 0);
        }
    }
}
