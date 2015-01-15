using FurryLana.Engine.Entity.Interfaces;
using FurryLana.Engine.Map;
using FurryLana.Engine.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming.MathUtils;
using FurryLana.Engine.Graphics;

namespace FurryLana.Engine.Entity
{
    /// <summary>
    /// Floor class.
    /// </summary>
    public class Floor : Tile, IEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Entity.Floor"/> class.
        /// </summary>
        /// <param name="map">Map.</param>
        /// <param name="model">Model.</param>
        /// <param name="walkable">If set to <c>true</c> walkable.</param>
        public Floor(TiledMap map, IModel model, bool walkable) : base (map,model,walkable) { }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public int ID
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets or sets the bounding box.
        /// </summary>
        /// <value>The B box.</value>
        public BoundingBox BBox { get; set; }

        /// <summary>
        /// Position in space
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the smoothed position.
        /// </summary>
        /// <value>The smoothed position.</value>
        public Vector3 SmoothedPosition { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public float Height { get; set; }

        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        /// <value>The rotation.</value>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
    }
}
