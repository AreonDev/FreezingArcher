using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FurryLana.Engine.Entity.Interfaces
{
   public class BoundingBox
    {
        public Vector3 Position { get; set; }
        public Vector3 Size { get; set; }
        public BoundingBox(Vector3 position, Vector3 size) 
        {
            this.Position = position;
            this.Size = size;
        }
        public BoundingBox(Vector3 position)
        {
            this.Position = position;
            this.Size = new Vector3(1, 1, 1);
        }

    }
}
