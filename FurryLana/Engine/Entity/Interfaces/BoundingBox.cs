using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FurryLana.Engine.Entity.Interfaces
{
   public class BoundingBox
    {
        Vector3 position;
        Vector3 size;
        public BoundingBox(Vector3 position, Vector3 size) 
        {
            this.position = position;
            this.size = size;
        }
        public BoundingBox(Vector3 position)
        {
            this.position = position;
            this.size = new Vector3(1, 1, 1);
        }

    }
}
