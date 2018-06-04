using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngineProjectRevive.Objects
{
    public class Sprite : GameObject
    {
        public Texture2D image;

        public override void Render(Camera activeCamera, GraphicsDevice device, SpriteBatch batch)
        {
            batch.Draw(image, new Rectangle((int)this.GlobalTranslation.X - (int)activeCamera.Translation.X, (int)this.GlobalTranslation.Y - (int)activeCamera.Translation.Y, (int)(this.Scale.X), (int)(this.Scale.Y)), Color.White);
            base.Render(activeCamera, device, batch);
        }

        public void RenderChildren(Camera activeCamera, GraphicsDevice device, SpriteBatch batch)
        {
            base.Render(activeCamera, device, batch);
        }

        public Sprite(Texture2D spriteImage)
            : base()
        {
            image = spriteImage;
        }
    }
}