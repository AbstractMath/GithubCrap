using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngineProjectRevive.Physics;

namespace GameEngineProjectRevive.Objects
{
    /// <summary>
    /// Provides a base for all sprites. Contains all BaseObject Properties
    /// </summary>
    public class GameObject : BaseObject
    {
        #region PUBLIC FIELDS
        /// <summary>
        /// Determines where the object is
        /// </summary>
        public Vector2 Translation;
        /// <summary>
        /// Determines how big the sprite/object should be
        /// </summary>
        public Vector2 Scale;
        /// <summary>
        /// Gives the full translation of an object on screen
        /// </summary>
        public Vector2 GlobalTranslation
        {
            get { return this.Translation + ((this.Parent is GameObject p) ? p.GlobalTranslation : Vector2.Zero); }//Check if the parent is a GameObject, so it has a translation.
        }
        /// <summary>
        /// The collider of this game object
        /// </summary>
        public PhysicsObject Collider;
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Renders the children objects of this object
        /// </summary>
        /// <param name="device">The graphics device of the game</param>
        /// <param name="batch">The sprite batch of the game</param>
        public virtual void Render(Camera activeCamera, GraphicsDevice device, SpriteBatch batch)
        {
            foreach (KeyValuePair<string, BaseObject> child in this.Children)
            {
                if (child.Value is GameObject p)
                {
                    p.Render(activeCamera, device, batch);
                }
            }
        }

        public virtual void Update(GameTime time)
        {
            foreach (KeyValuePair<string, BaseObject> Children in this.Children)
            {
                if (Children.Value is GameObject p)
                {
                    p.Update(time);
                }
            }
        }
        #endregion

        #region PUBLIC CONSTRUCTORS
        /// <summary>
        /// Creates an empty game object from a name, translation, and scale
        /// </summary>
        /// <param name="Name">The name of the object</param>
        /// <param name="Translation">The translation of the object</param>
        /// <param name="Scale">The scale of the object</param>
        public GameObject(string Name, Vector2 Translation, Vector2 Scale)
            : base(Name)
        {
            this.Translation = Translation;
            this.Scale = Scale;
        }

        /// <summary>
        /// Creates an empty game object from translation and scale
        /// </summary>
        /// <param name="Translation">The translation of the object</param>
        /// <param name="Scale">The scale of the object</param>
        public GameObject(Vector2 Translation, Vector2 Scale)
            : base("GameObject")
        {
            this.Translation = Translation;
            this.Scale = Scale;
        }

        /// <summary>
        /// Creates an empty game object with default properties
        /// </summary>
        public GameObject()
            : base("GameObject")
        {
            this.Translation = Vector2.Zero;
            this.Scale = new Vector2(1, 1);
        }
        #endregion
    }
}

