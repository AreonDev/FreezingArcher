using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreezingArcher.Math;


namespace Henge3D.Pipeline
{
    /// <summary>
    /// Compiled capsule.
    /// </summary>
    public class CompiledCapsule : CompiledPart
    {
        private Capsule _capsule;

        /// <summary>
        /// Initializes a new instance of the <see cref="Henge3D.Pipeline.CompiledCapsule"/> class.
        /// </summary>
        public CompiledCapsule ()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Henge3D.Pipeline.CompiledCapsule"/> class.
        /// </summary>
        /// <param name="p1">P1.</param>
        /// <param name="p2">P2.</param>
        /// <param name="radius">Radius.</param>
        public CompiledCapsule (Vector3 p1, Vector3 p2, float radius)
        {
            _capsule = new Capsule (p1, p2, radius);
        }

        /// <summary>
        /// Tos the composition part.
        /// </summary>
        /// <returns>The composition part.</returns>
        public override Part ToCompositionPart ()
        {
            return new CapsulePart (_capsule);
        }

        /// <summary>
        /// Transform the specified transform.
        /// </summary>
        /// <param name="transform">Transform.</param>
        public override void Transform (ref Matrix transform)
        {
            Vector3.Transform (ref _capsule.P1, ref transform, out _capsule.P1);
            Vector3.Transform (ref _capsule.P2, ref transform, out _capsule.P2);
        }
    }
}
