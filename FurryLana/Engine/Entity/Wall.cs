using FurryLana.Engine.Entity.Interfaces;
using FurryLana.Engine.Map;
using FurryLana.Engine.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace FurryLana.Engine.Entity
{
    class Wall : Tile 
    {

        public Wall(TiledMap map, IModel model, bool walkable) : base(map, model, walkable) { }
        public int ID
        {
            get { return 1; }
        }

        public BoundingBox BBox { get; set; }

        public Pencil.Gaming.MathUtils.Vector3 Position { get; set; }

        public Pencil.Gaming.MathUtils.Vector3 Rotation { get; set; }

        public string Name { get; set; }
    }
}
