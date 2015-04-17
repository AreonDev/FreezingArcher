using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreezingArcher.Math;


namespace Henge3D.Pipeline
{
	public class CompiledSphere : CompiledPart
	{
		private Sphere _sphere;

		public CompiledSphere()
		{
		}

		public CompiledSphere(Vector3 center, float radius)
		{
			_sphere = new Sphere(center, radius);
		}

		public override Part ToCompositionPart()
		{
			return new SpherePart(_sphere);
		}

		public override void Transform(ref Matrix transform)
		{
			Vector3.Transform(ref _sphere.Center, ref transform, out _sphere.Center);
		}
	}
}
