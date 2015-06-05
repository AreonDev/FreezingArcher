﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreezingArcher.Math;

namespace Henge3D.Collision
{
    /// <summary>
    /// Capsule plane.
    /// </summary>
    public class CapsulePlane : NarrowPhase
    {
        /// <summary>
        /// When overridden in a derived class, performs a static overlap test between two parts.
        /// </summary>
        /// <param name="cf">The functor to which all collisions are reported.</param>
        /// <param name="partA">The first part to test.</param>
        /// <param name="partB">The second part to test.</param>
        public override void OverlapTest (CollisionFunctor cf, Part partA, Part partB)
        {
            var a = (CapsulePart)partA;
            var b = (PlanePart)partB;

            float ax, bx;
            Vector3 pa, pb, radius;
            Vector3.Multiply (ref b.Plane.Normal, -a.World.Radius, out radius);
            Vector3.Dot (ref b.Plane.Normal, ref b.Plane.P, out bx);

            Vector3.Dot (ref b.Plane.Normal, ref a.World.P1, out ax);
            if (ax - bx - a.World.Radius < Constants.Epsilon)
            {
                Vector3.Add (ref a.World.P1, ref radius, out pa);
                b.Plane.ClosestPointTo (ref pa, out pb);
                cf.WritePoint (ref pa, ref pb, ref b.Plane.Normal);
            }

            Vector3.Dot (ref b.Plane.Normal, ref a.World.P2, out ax);
            if (ax - bx - a.World.Radius < Constants.Epsilon)
            {
                Vector3.Add (ref a.World.P2, ref radius, out pa);
                b.Plane.ClosestPointTo (ref pa, out pb);
                cf.WritePoint (ref pa, ref pb, ref b.Plane.Normal);
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
            var a = (CapsulePart)partA;
            var b = (PlanePart)partB;

            float ax, bx, dx;
            Vector3 pa, pb, radius;
            Vector3.Multiply (ref b.Plane.Normal, -a.World.Radius, out radius);
            Vector3.Dot (ref b.Plane.Normal, ref b.Plane.P, out bx);
            Vector3.Dot (ref b.Plane.Normal, ref delta, out dx);
            dx = MathHelper.Min (dx, 0f);

            Vector3.Dot (ref b.Plane.Normal, ref a.World.P1, out ax);
            if (ax - bx - a.World.Radius + dx < Constants.Epsilon)
            {
                Vector3.Add (ref a.World.P1, ref radius, out pa);
                b.Plane.ClosestPointTo (ref pa, out pb);
                cf.WritePoint (ref pa, ref pb, ref b.Plane.Normal);
            }

            Vector3.Dot (ref b.Plane.Normal, ref a.World.P2, out ax);
            if (ax - bx - a.World.Radius + dx < Constants.Epsilon)
            {
                Vector3.Add (ref a.World.P2, ref radius, out pa);
                b.Plane.ClosestPointTo (ref pa, out pb);
                cf.WritePoint (ref pa, ref pb, ref b.Plane.Normal);
            }
        }
    }
}
