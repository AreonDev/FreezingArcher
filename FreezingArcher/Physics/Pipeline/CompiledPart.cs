using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreezingArcher.Math;

namespace Henge3D
{
    /// <summary>
    /// Compiled part.
    /// </summary>
    public abstract class CompiledPart
    {
        /// <summary>
        /// Tos the composition part.
        /// </summary>
        /// <returns>The composition part.</returns>
        public abstract Part ToCompositionPart ();

        /// <summary>
        /// Transform the specified transform.
        /// </summary>
        /// <param name="transform">Transform.</param>
        public abstract void Transform (ref Matrix transform);
    }
}
