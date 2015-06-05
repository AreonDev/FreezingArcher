using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreezingArcher.Math;

namespace Henge3D.Collision
{
    /// <summary>
    /// Polyhedron plane.
    /// </summary>
    public class PolyhedronPlane : NarrowPhase
    {
        /// <summary>
        /// When overridden in a derived class, performs a static overlap test between two parts.
        /// </summary>
        /// <param name="cf">The functor to which all collisions are reported.</param>
        /// <param name="partA">The first part to test.</param>
        /// <param name="partB">The second part to test.</param>
        public override void OverlapTest (CollisionFunctor cf, Part partA, Part partB)
        {
            var a = (PolyhedronPart)partA;
            var b = (PlanePart)partB;

            PolyhedronPlane.DoOverlapTest (cf, a, b, 0f);
        }

        private static void DoOverlapTest (CollisionFunctor cf, PolyhedronPart a, PlanePart b, float offset)
        {
            Vector3 normalNeg;
            Vector3.Multiply (ref b.Plane.Normal, -1, out normalNeg);
            int vIndex = a.ExtremeVertex (ref normalNeg).Index;

            float ax, bx;
            Vector3 p = b.Plane.P;
            if (offset > 0f)
            {
                Vector3.Multiply (ref b.Plane.Normal, offset, out p);
                Vector3.Add (ref b.Plane.P, ref p, out p);
            }

            Vector3.Dot (ref b.Plane.Normal, ref p, out bx);

            for (int i = 0; i < a.FaceCount; i++)
            {
                var face = a.Face (i);
                bool skip = true;
                for (int j = 0; j < face.Length; j++)
                {
                    if (face [j] == vIndex)
                    {
                        skip = false;
                        break;
                    }
                }
                if (skip)
                    continue;

                for (int j = 0; j < face.Length; j++)
                {
                    Vector3 pa, pb;
                    a.World (face [j], out pa);
                    Vector3.Dot (ref b.Plane.Normal, ref pa, out ax);
                    if (ax - bx < Constants.Epsilon)
                    {
                        b.Plane.ClosestPointTo (ref pa, out pb);
                        cf.WritePoint (ref pa, ref pb, ref b.Plane.Normal);
                    }
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, performs a swept (or simulated-swept) test between two parts.
        /// </summary>
        /// <param name="cf">The functor to which all collisions are reported.</param>
        /// <param name="partA">The first part to test.</param>
        /// <param name="partB">The second part to test.</param>
        /// <param name="delta">The direction and magnitude of movement of partA, relative to partB.</param>
        public override void SweptTest (CollisionFunctor cf, Part partA, Part partB, Vector3 delta)
        {
            var a = (PolyhedronPart)partA;
            var b = (PlanePart)partB;

            var plane = b.Plane;
            float dx;
            Vector3.Dot (ref plane.Normal, ref delta, out dx);
            if (dx > 0f)
            {
                dx = 0f;
            }

            PolyhedronPlane.DoOverlapTest (cf, a, b, -dx);
        }
    }
}
