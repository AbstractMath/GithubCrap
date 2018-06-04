using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngineProjectRevive.Objects
{
    /// <summary>
    /// Acts like a root for the hierarchy tree. Will eventually have a render method that renders the entire scene. Should also have layers
    /// </summary>
    public class Scene
    {
        public LinkedList<SpriteLayer> Layers;
        private GraphicsDevice device;
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Renders the current scene
        /// </summary>
        public void Render(Camera currentCamera)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            foreach (SpriteLayer layer in Layers)
            {
                layer.Render(currentCamera, device, spriteBatch);
            }
            spriteBatch.End();
        }

        public void Update(GameTime time)
        {
            foreach (SpriteLayer layer in Layers)
            {
                layer.Update(time);
            }
        }

        public Scene(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            this.device = device;
            this.spriteBatch = spriteBatch;
            Layers = new LinkedList<SpriteLayer>();
        }
    }
}
