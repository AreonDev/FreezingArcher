using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Henge3D.Physics;

namespace Henge3D.Pipeline
{
    /// <summary>
    /// Rigid body model.
    /// </summary>
    public class RigidBodyModel
    {
        private MassProperties _mass;
        private CompiledPart[] _parts;
        private Material[] _materials;

        /// <summary>
        /// Gets the mass properties.
        /// </summary>
        /// <value>The mass properties.</value>
        public MassProperties MassProperties { get { return _mass; } }

        /// <summary>
        /// Gets the parts.
        /// </summary>
        /// <value>The parts.</value>
        public CompiledPart[] Parts { get { return _parts; } }

        /// <summary>
        /// Gets the materials.
        /// </summary>
        /// <value>The materials.</value>
        public Material[] Materials { get { return _materials; } }

        private RigidBodyModel ()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Henge3D.Pipeline.RigidBodyModel"/> class.
        /// </summary>
        /// <param name="mass">Mass.</param>
        /// <param name="parts">Parts.</param>
        /// <param name="materials">Materials.</param>
        public RigidBodyModel (MassProperties mass, CompiledPart[] parts,
                        Material[] materials)
        {
            _mass = mass;
            _parts = parts;
            _materials = materials;

            if (_parts.Length != _materials.Length)
            {
                throw new ArgumentException ("The count of supplied mesh parts and materials do not match.");
            }
        }
    }
}
