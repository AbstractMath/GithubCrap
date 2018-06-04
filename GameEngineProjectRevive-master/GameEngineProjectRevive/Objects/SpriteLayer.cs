using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameEngineProjectRevive.Objects
{
    public class SpriteLayer : GameObject
    {
        public Vector2 ScrollRate;

        public SpriteLayer()
            : base("SpriteLayer", Vector2.Zero, new Vector2(1, 1))
        {

        }
    }
}
