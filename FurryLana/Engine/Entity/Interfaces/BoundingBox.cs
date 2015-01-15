using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FurryLana.Engine.Entity.Interfaces
{
    /// <summary>
    /// Bounding box.
    /// </summary>
    public class BoundingBox
    {
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public Vector3 Size { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Entity.Interfaces.BoundingBox"/> class.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="size">Size.</param>
        public BoundingBox(Vector3 position, Vector3 size) 
        {
            this.Position = position;
            this.Size = size;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Entity.Interfaces.BoundingBox"/> class.
        /// </summary>
        /// <param name="position">Position.</param>
        public BoundingBox(Vector3 position)
        {
            this.Position = position;
            this.Size = new Vector3(1, 1, 1);
        }
    }
}
