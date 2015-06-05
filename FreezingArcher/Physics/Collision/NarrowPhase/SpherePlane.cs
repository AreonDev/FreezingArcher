using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreezingArcher.Math;

namespace Henge3D.Collision
{
    /// <summary>
    /// Sphere plane.
    /// </summary>
    public class SpherePlane : NarrowPhase
    {
        /// <summary>
        /// When overridden in a derived class, performs a static overlap test between two parts.
        /// </summary>
        /// <param name="cf">The functor to which all collisions are reported.</param>
        /// <param name="partA">The first part to test.</param>
        /// <param name="partB">The second part to test.</param>
        public override void OverlapTest (CollisionFunctor cf, Part partA, Part partB)
        {
            var a = (SpherePart)partA;
            var b = (PlanePart)partB;

            Vector3 pa, pb;
            float ax, bx;
            Vector3.Dot (ref b.Plane.Normal, ref a.World.Center, out ax);
            Vector3.Dot (ref b.Plane.Normal, ref b.Plane.P, out bx);

            if (ax - bx - a.World.Radius >= Constants.Epsilon)
                return;

            b.Plane.ClosestPointTo (ref a.World.Center, out pb);
            Vector3.Multiply (ref b.Plane.Normal, -a.World.Radius, out pa);
            Vector3.Add (ref a.World.Center, ref pa, out pa);
            cf.WritePoint (ref pa, ref pb, ref b.Plane.Normal);
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
            var a = (SpherePart)partA;
            var b = (PlanePart)partB;

            Vector3 pa = a.World.Center, pb;
            float ax, bx;
            Vector3.Dot (ref b.Plane.Normal, ref b.Plane.P, out bx);
            Vector3.Dot (ref b.Plane.Normal, ref pa, out ax);

            if (ax - bx - a.World.Radius >= Constants.Epsilon)
            {
                Vector3.Add (ref a.World.Center, ref delta, out pa);
                Vector3.Dot (ref b.Plane.Normal, ref pa, out ax);
                if (ax - bx - a.World.Radius >= Constants.Epsilon)
                {
                    return;
                }
            }

            b.Plane.ClosestPointTo (ref pa, out pb);
            Vector3.Multiply (ref b.Plane.Normal, -a.World.Radius, out pa);
            Vector3.Add (ref a.World.Center, ref pa, out pa);
            cf.WritePoint (ref pa, ref pb, ref b.Plane.Normal);
        }
    }
}
