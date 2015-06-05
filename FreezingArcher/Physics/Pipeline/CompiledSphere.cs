using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreezingArcher.Math;


namespace Henge3D.Pipeline
{
    /// <summary>
    /// Compiled sphere.
    /// </summary>
    public class CompiledSphere : CompiledPart
    {
        private Sphere _sphere;

        /// <summary>
        /// Initializes a new instance of the <see cref="Henge3D.Pipeline.CompiledSphere"/> class.
        /// </summary>
        public CompiledSphere ()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Henge3D.Pipeline.CompiledSphere"/> class.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        public CompiledSphere (Vector3 center, float radius)
        {
            _sphere = new Sphere (center, radius);
        }

        /// <summary>
        /// Tos the composition part.
        /// </summary>
        /// <returns>The composition part.</returns>
        public override Part ToCompositionPart ()
        {
            return new SpherePart (_sphere);
        }

        /// <summary>
        /// Transform the specified transform.
        /// </summary>
        /// <param name="transform">Transform.</param>
        public override void Transform (ref Matrix transform)
        {
            Vector3.Transform (ref _sphere.Center, ref transform, out _sphere.Center);
        }
    }
}
