//
//  Wind.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2015 Fin Christensen
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
using FreezingArcher.Math;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Abstraction of a wind in an environment with a wind field.
    /// </summary>
    public class Wind
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Content.Wind"/> class.
        /// </summary>
        /// <param name="globalWind">Global wind force.</param>
        /// <param name="fieldResolution">Wind field resolution.</param>
        /// <param name="defaultFieldForce">Default field force.</param>
        public Wind(Vector3 globalWind, Vector3i fieldResolution, Vector3 defaultFieldForce = default(Vector3))
        {
            Global = globalWind;
            FieldResolution = fieldResolution;
            WindField = new Vector3[FieldResolution.X, FieldResolution.Y, FieldResolution.Z];

            for (int x = 0; x < WindField.GetLength(0); x++)
                for (int y = 0; y < WindField.GetLength(1); y++)
                    for (int z = 0; z < WindField.GetLength(2); z++)
                        WindField[x, y, z] = defaultFieldForce;
        }

        /// <summary>
        /// Gets or sets the global wind force.
        /// </summary>
        /// <value>The global wind force.</value>
        public Vector3 Global { get; set; }

        /// <summary>
        /// Gets or sets the wind field resolution.
        /// </summary>
        /// <value>The wind field resolution.</value>
        public Vector3i FieldResolution { get; protected set; }

        /// <summary>
        /// Gets or sets the wind field.
        /// </summary>
        /// <value>The wind field.</value>
        public Vector3[,,] WindField { get; protected set; }

        /// <summary>
        /// Gets the wind force at position.
        /// </summary>
        /// <returns>The wind force at position.</returns>
        /// <param name="position">Position.</param>
        public double GetWindForceAt(Vector3 position)
        {
            return GetWindForceAt(position.X, position.Y, position.Z);
        }

        /// <summary>
        /// Gets the wind force at x, y and z.
        /// </summary>
        /// <returns>The wind force at x, y and z.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        public double GetWindForceAt(double x, double y, double z)
        {
            int ix1 = (int) x;
            int ix2 = (int) (x + 0.5);
            int iy1 = (int) y;
            int iy2 = (int) (y + 0.5);
            int iz1 = (int) z;
            int iz2 = (int) (z + 0.5);

            double x2_fac = x - ix1;
            double x1_fac = 1 - x2_fac;
            double y2_fac = y - iy1;
            double y1_fac = 1 - y2_fac;
            double z2_fac = z - iz1;
            double z1_fac = 1 - z2_fac;

            Vector3 i1 = WindField[ix1, iy1, iz1];
            Vector3 i2 = WindField[ix2, iy2, iz2];

            double rx = i1.X * x1_fac + i2.X * x2_fac;
            double ry = i1.Y * y1_fac + i2.Y * y2_fac;
            double rz = i1.Z * z1_fac + i2.Z * z2_fac;

            return System.Math.Sqrt(rx * rx + ry * ry + rz * rz);
        }

        // TODO: add ApplyForceAt(Vector3 position, double strength) where the length of position
        //       describes the force radius and the strength the total strength of the force
    }
}
