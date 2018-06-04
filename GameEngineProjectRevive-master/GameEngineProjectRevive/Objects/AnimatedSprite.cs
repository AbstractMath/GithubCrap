using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngineProjectRevive.Objects
{
    public class AnimatedSprite : Sprite
    {
        private int[] keyframes;
        private float lastKey = 0;
        private int lastKeyID = 0;
        private int FrameID;
        public Vector2 SpriteSize;
        public bool IsLooped;
        public bool IsPlaying;

        public override void Update(GameTime time)
        {
            if (IsPlaying)
            {
                float elapsedTime = time.ElapsedGameTime.Milliseconds - lastKey;

                if (elapsedTime >= keyframes[lastKeyID])
                {
                    lastKeyID++;
                    lastKeyID = lastKeyID % keyframes.Length;
                    FrameID++;
                    FrameID = FrameID % keyframes.Length;
                    lastKey = 0;

                    if (!IsLooped && lastKeyID >= keyframes.Length)
                    {
                        IsPlaying = false;
                    }
                }
            }

            base.Update(time);
        }

        public void Play(GameTime time)
        {
            lastKey = time.ElapsedGameTime.Milliseconds;
            lastKeyID = 0;
        }

        public override void Render(Camera activeCamera, GraphicsDevice device, SpriteBatch batch)
        {


            base.RenderChildren(activeCamera, device, batch);
        }

        public AnimatedSprite(Texture2D spriteSheet, int[] keyframes)
            : base(spriteSheet)
        {
            this.keyframes = keyframes;

        }
    }
}
