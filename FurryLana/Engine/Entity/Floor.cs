using FurryLana.Engine.Entity.Interfaces;
using FurryLana.Engine.Map;
using FurryLana.Engine.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurryLana.Engine.Entity
{
    public class Floor : Tile
    {
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

        public BoundingBox BBox { get; set; }

        public Pencil.Gaming.MathUtils.Vector3 Position { get; set; }

        public Pencil.Gaming.MathUtils.Vector3 Rotation { get; set; }

        public string Name { get; set; }
    }
}
