using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngineProjectRevive.Objects
{
    //Declared as abstract because we don't actually want any instances of this out there, we also want for the hierarchy system to be implemented the same way across ALL objects. 
    /// <summary>
    /// Provides a structural base for all objects
    /// </summary>
    public abstract class BaseObject
    {
        #region PRIVATE FIELDS
        //We need something to be able to return it later on
        private BaseObject parent;
        #endregion

        #region PUBLIC FIELDS
        //We want to store this by dictionary so we can get children by name at fast speeds
        /// <summary>
        /// Contains all the children in this object.
        /// </summary>
        public Dictionary<string, BaseObject> Children { get; private set; }

        /// <summary>
        /// The name of this object.
        /// </summary>
        public string Name;

        /// <summary>
        /// The parent of this object. All transformations will be inherited from the parent
        /// </summary>
        public BaseObject Parent
        {
            //Accessing should be fine, so don't add a getter
            get { return parent; }
            set//We need to change the hierarchy of the parent
            {
                if (Parent != null)//We want to make sure this actually exists before we try to access it
                {
                    Parent.Children.Remove(this.Name);//If it exists, go ahead and remove it from this current dictionary.
                }
                value.Children.Add(this.Name, this);//Add it to the children of the new parent.
                this.parent = value;//Set the private parent property. 
            }
        }
        #endregion

        #region PUBLIC CONSTRUCTORS
        /// <summary>
        /// Constructor template must include a name
        /// </summary>
        /// <param name="Name"> The name of this object</param>
        public BaseObject(string Name)
        {
            this.Name = Name;
            Children = new Dictionary<string, BaseObject>();//Go ahead and instance a new one of these.
        }
        #endregion
    }
}

