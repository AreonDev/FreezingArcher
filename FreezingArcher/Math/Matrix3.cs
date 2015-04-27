/*
* Copyright (c) 2012-2014 AssimpNet - Nicholas Woodfield
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using FreezingArcher.Math;

namespace FreezingArcher.Math
{
	/// <summary>
	/// Represents a 3x3 matrix. Assimp docs say their matrices are always row-major,
	/// and it looks like they're only describing the memory layout. Matrices are treated
	/// as column vectors however (X base in the first column, Y base the second, and Z base the third)
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix3 : IEquatable<Matrix3>
	{
		/// <summary>
		/// Value at row 1, column 1 of the matrix
		/// </summary>
		public float M11;

		/// <summary>
		/// Value at row 1, column 2 of the matrix
		/// </summary>
		public float M12;

		/// <summary>
		/// Value at row 1, column 3 of the matrix
		/// </summary>
		public float M13;

		/// <summary>
		/// Value at row 2, column 1 of the matrix
		/// </summary>
		public float M21;

		/// <summary>
		/// Value at row 2, column 2 of the matrix
		/// </summary>
		public float M22;

		/// <summary>
		/// Value at row 2, column 3 of the matrix
		/// </summary>
		public float M23;

		/// <summary>
		/// Value at row 3, column 1 of the matrix
		/// </summary>
		public float M31;

		/// <summary>
		/// Value at row 3, column 2 of the matrix
		/// </summary>
		public float M32;

		/// <summary>
		/// Value at row 3, column 3 of the matrix
		/// </summary>
		public float M33;

		private static Matrix3 _identity = new Matrix3(1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f);

		/// <summary>
		/// Gets the identity matrix.
		/// </summary>
		public static Matrix3 Identity
		{
			get
			{
				return _identity;
			}
		}

		/// <summary>
		/// Gets if this matrix is an identity matrix.
		/// </summary>
		public bool IsIdentity
		{
			get
			{
				float epsilon = 10e-3f;

				return (M12 <= epsilon && M12 >= -epsilon &&
					M13 <= epsilon && M13 >= -epsilon &&
					M21 <= epsilon && M21 >= -epsilon &&
					M23 <= epsilon && M23 >= -epsilon &&
					M31 <= epsilon && M31 >= -epsilon &&
					M32 <= epsilon && M32 >= -epsilon &&
					M11 <= 1.0f + epsilon && M11 >= 1.0f - epsilon &&
					M22 <= 1.0f + epsilon && M22 >= 1.0f - epsilon &&
					M33 <= 1.0f + epsilon && M33 >= 1.0f - epsilon);
			}
		}

		/// <summary>
		/// Gets or sets the value at the specific one-based row, column
		/// index. E.g. i = 1, j = 2 gets the value in row 1, column 2 (MA2). Indices
		/// out of range return a value of zero.
		/// 
		/// </summary>
		/// <param name="i">One-based Row index</param>
		/// <param name="j">One-based Column index</param>
		/// <returns>Matrix value</returns>
		public float this[int i, int j]
		{
			get
			{
				switch(i)
				{
				case 1:
					switch(j)
					{
					case 1:
						return M11;
					case 2:
						return M12;
					case 3:
						return M13;
					default:
						return 0;
					}
				case 2:
					switch(j)
					{
					case 1:
						return M21;
					case 2:
						return M22;
					case 3:
						return M23;
					default:
						return 0;
					}
				case 3:
					switch(j)
					{
					case 1:
						return M31;
					case 2:
						return M32;
					case 3:
						return M33;
					default:
						return 0;
					}
				default:
					return 0;
				}
			}
			set
			{
				switch(i)
				{
				case 1:
					switch(j)
					{
					case 1:
						M11 = value;
						break;
					case 2:
						M12 = value;
						break;
					case 3:
						M13 = value;
						break;
					}
					break;
				case 2:
					switch(j)
					{
					case 1:
						M21 = value;
						break;
					case 2:
						M22 = value;
						break;
					case 3:
						M23 = value;
						break;
					}
					break;
				case 3:
					switch(j)
					{
					case 1:
						M31 = value;
						break;
					case 2:
						M32 = value;
						break;
					case 3:
						M33 = value;
						break;
					}
					break;
				}
			}
		}

		/// <summary>
		/// Constructs a new Matrix3.
		/// </summary>
		/// <param name="a1">Element at row 1, column 1</param>
		/// <param name="a2">Element at row 1, column 2</param>
		/// <param name="a3">Element at row 1, column 3</param>
		/// <param name="b1">Element at row 2, column 1</param>
		/// <param name="b2">Element at row 2, column 2</param>
		/// <param name="b3">Element at row 2, column 3</param>
		/// <param name="c1">Element at row 3, column 1</param>
		/// <param name="c2">Element at row 3, column 2</param>
		/// <param name="c3">Element at row 3, column 3</param>
		public Matrix3(float a1, float a2, float a3, float b1, float b2, float b3, float c1, float c2, float c3)
		{
			this.M11 = a1;
			this.M12 = a2;
			this.M13 = a3;
			this.M21 = b1;
			this.M22 = b2;
			this.M23 = b3;
			this.M31 = c1;
			this.M32 = c2;
			this.M33 = c3;
		}

		/// <summary>
		/// Constructs a new Matrix3.
		/// </summary>
		/// <param name="rotMatrix">A 4x4 matrix to construct from, only taking the rotation/scaling part.</param>
		public Matrix3(Matrix rotMatrix)
		{
			this.M11 = rotMatrix.M11;
			this.M12 = rotMatrix.M12;
			this.M13 = rotMatrix.M13;

			this.M21 = rotMatrix.M21;
			this.M22 = rotMatrix.M22;
			this.M23 = rotMatrix.M23;

			this.M31 = rotMatrix.M31;
			this.M32 = rotMatrix.M32;
			this.M33 = rotMatrix.M33;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Assimp.Matrix3"/> struct.
		/// </summary>
		/// <param name="copy">Source matrix</param>
		public Matrix3(Matrix3 copy)
		{
			this.M11 = copy.M11;
			this.M12 = copy.M12;
			this.M13 = copy.M13;

			this.M21 = copy.M21;
			this.M22 = copy.M22;
			this.M23 = copy.M23;

			this.M31 = copy.M31;
			this.M32 = copy.M32;
			this.M33 = copy.M33;
		}
		/// <summary>
		/// Transposes this matrix (rows become columns, vice versa).
		/// </summary>
		public void Transpose()
		{
			Matrix3 m = new Matrix3(this);

			M12 = m.M21;
			M13 = m.M31;

			M21 = m.M12;
			M23 = m.M32;

			M31 = m.M13;
			M32 = m.M23;
		}

		/// <summary>
		/// Inverts the matrix. If the matrix is *not* invertible all elements are set to <see cref="float.NaN"/>.
		/// </summary>
		public void Inverse()
		{
			float det = Determinant();
			if(det == 0.0f)
			{
				// Matrix not invertible. Setting all elements to NaN is not really
				// correct in a mathematical sense but it is easy to debug for the
				// programmer.
				M11 = float.NaN;
				M12 = float.NaN;
				M13 = float.NaN;

				M21 = float.NaN;
				M22 = float.NaN;
				M23 = float.NaN;

				M31 = float.NaN;
				M32 = float.NaN;
				M33 = float.NaN;
			}

			float invDet = 1.0f / det;

			float a1 = invDet * (M22 * M33 - M23 * M32);
			float a2 = -invDet * (M12 * M33 - M13 * M32);
			float a3 = invDet * (M12 * M23 - M13 * M22);

			float b1 = -invDet * (M21 * M33 - M23 * M31);
			float b2 = invDet * (M11 * M33 - M13 * M31);
			float b3 = -invDet * (M11 * M23 - M13 * M21);

			float c1 = invDet * (M21 * M32 - M22 * M31);
			float c2 = -invDet * (M11 * M32 - M12 * M31);
			float c3 = invDet * (M11 * M22 - M12 * M21);

			M11 = a1;
			M12 = a2;
			M13 = a3;

			M21 = b1;
			M22 = b2;
			M23 = b3;

			M31 = c1;
			M32 = c2;
			M33 = c3;
		}

		/// <summary>
		/// Compute the determinant of this matrix.
		/// </summary>
		/// <returns>The determinant</returns>
		public float Determinant()
		{
			return M11 * M22 * M33 - M11 * M23 * M32 + M12 * M23 * M31 - M12 * M21 * M33 + M13 * M21 * M32 - M13 * M22 * M31;
		}

		/// <summary>
		/// Creates a rotation matrix from a set of euler angles.
		/// </summary>
		/// <param name="x">Rotation angle about the x-axis, in radians.</param>
		/// <param name="y">Rotation angle about the y-axis, in radians.</param>
		/// <param name="z">Rotation angle about the z-axis, in radians.</param>
		/// <returns>The rotation matrix</returns>
		public static Matrix3 FromEulerAnglesXYZ(float x, float y, float z)
		{
			float cr = (float) System.Math.Cos(x);
			float sr = (float) System.Math.Sin(x);
			float cp = (float) System.Math.Cos(y);
			float sp = (float) System.Math.Sin(y);
			float cy = (float) System.Math.Cos(z);
			float sy = (float) System.Math.Sin(z);

			float srsp = sr * sp;
			float crsp = cr * sp;

			Matrix3 m;
			m.M11 = cp * cy;
			m.M12 = cp * sy;
			m.M13 = -sp;

			m.M21 = srsp * cy - cr * sy;
			m.M22 = srsp * sy + cr * cy;
			m.M23 = sr * cp;

			m.M31 = crsp * cy + sr * sy;
			m.M32 = crsp * sy - sr * cy;
			m.M33 = cr * cp;

			return m;
		}

		/// <summary>
		/// Creates a rotation matrix from a set of euler angles.
		/// </summary>
		/// <param name="angles">Vector containing the rotation angles about the x, y, z axes, in radians.</param>
		/// <returns>The rotation matrix</returns>
		public static Matrix3 FromEulerAnglesXYZ(Vector3 angles)
		{
			return Matrix3.FromEulerAnglesXYZ(angles.X, angles.Y, angles.Z);
		}

		/// <summary>
		/// Creates a rotation matrix for a rotation about the x-axis.
		/// </summary>
		/// <param name="radians">Rotation angle in radians.</param>
		/// <returns>The rotation matrix</returns>
		public static Matrix3 FromRotationX(float radians)
		{
			/*
                 |  1  0       0      |
             M = |  0  cos(A) -sin(A) |
                 |  0  sin(A)  cos(A) |	
            */
			Matrix3 m = Identity;
			m.M22 = m.M33 = (float) System.Math.Cos(radians);
			m.M32 = (float) System.Math.Sin(radians);
			m.M23 = -m.M32;
			return m;
		}

		/// <summary>
		/// Creates a rotation matrix for a rotation about the y-axis.
		/// </summary>
		/// <param name="radians">Rotation angle in radians.</param>
		/// <returns>The rotation matrix</returns>
		public static Matrix3 FromRotationY(float radians)
		{
			/*
                 |  cos(A)  0   sin(A) |
             M = |  0       1   0      |
                 | -sin(A)  0   cos(A) |
            */
			Matrix3 m = Identity;
			m.M11 = m.M33 = (float) System.Math.Cos(radians);
			m.M13 = (float) System.Math.Sin(radians);
			m.M31 = -m.M13;
			return m;
		}

		/// <summary>
		/// Creates a rotation matrix for a rotation about the z-axis.
		/// </summary>
		/// <param name="radians">Rotation angle in radians.</param>
		/// <returns>The rotation matrix</returns>
		public static Matrix3 FromRotationZ(float radians)
		{
			/*
                 |  cos(A)  -sin(A)   0 |
             M = |  sin(A)   cos(A)   0 |
                 |  0        0        1 |
             */
			Matrix3 m = Identity;
			m.M11 = m.M22 = (float) System.Math.Cos(radians);
			m.M21 = (float) System.Math.Sin(radians);
			m.M12 = -m.M21;
			return m;
		}

		/// <summary>
		/// Creates a rotation matrix for a rotation about an arbitrary axis.
		/// </summary>
		/// <param name="radians">Rotation angle, in radians</param>
		/// <param name="axis">Rotation axis, which should be a normalized vector.</param>
		/// <returns>The rotation matrix</returns>
		public static Matrix3 FromAngleAxis(float radians, Vector3 axis)
		{
			float x = axis.X;
			float y = axis.Y;
			float z = axis.Z;

			float sin = (float) System.Math.Sin((double) radians);
			float cos = (float) System.Math.Cos((double) radians);

			float xx = x * x;
			float yy = y * y;
			float zz = z * z;
			float xy = x * y;
			float xz = x * z;
			float yz = y * z;

			Matrix3 m;
			m.M11 = xx + (cos * (1.0f - xx));
			m.M21 = (xy - (cos * xy)) + (sin * z);
			m.M31 = (xz - (cos * xz)) - (sin * y);

			m.M12 = (xy - (cos * xy)) - (sin * z);
			m.M22 = yy + (cos * (1.0f - yy));
			m.M32 = (yz - (cos * yz)) + (sin * x);

			m.M13 = (xz - (cos * xz)) + (sin * y);
			m.M23 = (yz - (cos * yz)) - (sin * x);
			m.M33 = zz + (cos * (1.0f - zz));

			return m;
		}

		/// <summary>
		/// Creates a scaling matrix.
		/// </summary>
		/// <param name="scaling">Scaling vector</param>
		/// <returns>The scaling vector</returns>
		public static Matrix3 FromScaling(Vector3 scaling)
		{
			Matrix3 m = Identity;
			m.M11 = scaling.X;
			m.M22 = scaling.Y;
			m.M33 = scaling.Z;
			return m;
		}

		/// <summary>
		/// Creates a rotation matrix that rotates a vector called "from" into another
		/// vector called "to". Based on an algorithm by Tomas Moller and John Hudges:
		/// <para>
		/// "Efficiently Building a Matrix to Rotate One Vector to Another"         
		/// Journal of Graphics Tools, 4(4):1-4, 1999
		/// </para>
		/// </summary>
		/// <param name="from">Starting vector</param>
		/// <param name="to">Ending vector</param>
		/// <returns>Rotation matrix to rotate from the start to end.</returns>
		public static Matrix3 FromToMatrix(Vector3 from, Vector3 to)
		{
			float e = Vector3.Dot(from, to);
			float f = (e < 0) ? -e : e;

			Matrix3 m = Identity;

			//"from" and "to" vectors almost parallel
			if(f > 1.0f - 0.00001f)
			{
				Vector3 u, v; //Temp variables
				Vector3 x; //Vector almost orthogonal to "from"

				x.X = (from.X > 0.0f) ? from.X : -from.X;
				x.Y = (from.Y > 0.0f) ? from.Y : -from.Y;
				x.Z = (from.Z > 0.0f) ? from.Z : -from.Z;

				if(x.X < x.Y)
				{
					if(x.X < x.Z)
					{
						x.X = 1.0f;
						x.Y = 0.0f;
						x.Z = 0.0f;
					}
					else
					{
						x.X = 0.0f;
						x.Y = 0.0f;
						x.Z = 1.0f;
					}
				}
				else
				{
					if(x.Y < x.Z)
					{
						x.X = 0.0f;
						x.Y = 1.0f;
						x.Z = 0.0f;
					}
					else
					{
						x.X = 0.0f;
						x.Y = 0.0f;
						x.Z = 1.0f;
					}
				}

				u.X = x.X - from.X;
				u.Y = x.Y - from.Y;
				u.Z = x.Z - from.Z;

				v.X = x.X - to.X;
				v.Y = x.Y - to.Y;
				v.Z = x.Z - to.Z;

				float c1 = 2.0f / Vector3.Dot(u, u);
				float c2 = 2.0f / Vector3.Dot(v, v);
				float c3 = c1 * c2 * Vector3.Dot(u, v);

				for(int i = 1; i < 4; i++)
				{
					for(int j = 1; j < 4; j++)
					{
						//This is somewhat unreadable, but the indices for u, v vectors are "zero-based" while
						//matrix indices are "one-based" always subtract by one to index those
						m[i, j] = -c1 * u[i - 1] * u[j - 1] - c2 * v[i - 1] * v[j - 1] + c3 * v[i - 1] * u[j - 1];
					}
					m[i, i] += 1.0f;
				}

			}
			else
			{
				//Most common case, unless "from" = "to" or "from" =- "to"
				Vector3 v = Vector3.Cross(from, to);

				//Hand optimized version (9 mults less) by Gottfried Chen
				float h = 1.0f / (1.0f + e);
				float hvx = h * v.X;
				float hvz = h * v.Z;
				float hvxy = hvx * v.Y;
				float hvxz = hvx * v.Z;
				float hvyz = hvz * v.Y;

				m.M11 = e + hvx * v.X;
				m.M12 = hvxy - v.Z;
				m.M13 = hvxz + v.Y;

				m.M21 = hvxy + v.Z;
				m.M22 = e + h * v.Y * v.Y;
				m.M23 = hvyz - v.X;

				m.M31 = hvxz - v.Y;
				m.M32 = hvyz + v.X;
				m.M33 = e + hvz * v.Z;
			}

			return m;
		}

		/// <summary>
		/// Tests equality between two matrices.
		/// </summary>
		/// <param name="a">First matrix</param>
		/// <param name="b">Second matrix</param>
		/// <returns>True if the matrices are equal, false otherwise</returns>
		public static bool operator ==(Matrix3 a, Matrix3 b)
		{
			return (((a.M11 == b.M11) && (a.M12 == b.M12) && (a.M13 == b.M13))
				&& ((a.M21 == b.M21) && (a.M22 == b.M22) && (a.M23 == b.M23))
				&& ((a.M31 == b.M31) && (a.M32 == b.M32) && (a.M33 == b.M33)));
		}

		/// <summary>
		/// Tests inequality between two matrices.
		/// </summary>
		/// <param name="a">First matrix</param>
		/// <param name="b">Second matrix</param>
		/// <returns>True if the matrices are not equal, false otherwise</returns>
		public static bool operator !=(Matrix3 a, Matrix3 b)
		{
			return (((a.M11 != b.M11) || (a.M12 != b.M12) || (a.M13 != b.M13))
				|| ((a.M21 != b.M21) || (a.M22 != b.M22) || (a.M23 != b.M23))
				|| ((a.M31 != b.M31) || (a.M32 != b.M32) || (a.M33 != b.M33)));
		}


		/// <summary>
		/// Performs matrix multiplication.Multiplication order is B x A. That way, SRT concatenations
		/// are left to right.
		/// </summary>
		/// <param name="a">First matrix</param>
		/// <param name="b">Second matrix</param>
		/// <returns>Multiplied matrix</returns>
		public static Matrix3 operator *(Matrix3 a, Matrix3 b)
		{
			return new Matrix3(a.M11 * b.M11 + a.M21 * b.M12 + a.M31 * b.M13,
				a.M12 * b.M11 + a.M22 * b.M12 + a.M32 * b.M13,
				a.M13 * b.M11 + a.M23 * b.M12 + a.M33 * b.M13,
				a.M11 * b.M21 + a.M21 * b.M22 + a.M31 * b.M23,
				a.M12 * b.M21 + a.M22 * b.M22 + a.M32 * b.M23,
				a.M13 * b.M21 + a.M23 * b.M22 + a.M33 * b.M23,
				a.M11 * b.M31 + a.M21 * b.M32 + a.M31 * b.M33,
				a.M12 * b.M31 + a.M22 * b.M32 + a.M32 * b.M33,
				a.M13 * b.M31 + a.M23 * b.M32 + a.M33 * b.M33);
		}

		/// <summary>
		/// Implicit conversion from a 4x4 matrix to a 3x3 matrix.
		/// </summary>
		/// <param name="mat">4x4 matrix</param>
		/// <returns>3x3 matrix</returns>
		public static implicit operator Matrix3(Matrix mat)
		{
			Matrix3 m;
			m.M11 = mat.M11;
			m.M12 = mat.M12;
			m.M13 = mat.M13;

			m.M21 = mat.M21;
			m.M22 = mat.M22;
			m.M23 = mat.M23;

			m.M31 = mat.M31;
			m.M32 = mat.M32;
			m.M33 = mat.M33;
			return m;
		}

		/// <summary>
		/// Tests equality between this matrix and another.
		/// </summary>
		/// <param name="other">Other matrix to test</param>
		/// <returns>True if the matrices are equal, false otherwise</returns>
		public bool Equals(Matrix3 other)
		{
			return (((M11 == other.M11) && (M12 == other.M12) && (M13 == other.M13))
				&& ((M21 == other.M21) && (M22 == other.M22) && (M23 == other.M23))
				&& ((M31 == other.M31) && (M32 == other.M32) && (M33 == other.M33)));
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(Object obj)
		{
			if(obj is Matrix3)
			{
				return Equals((Matrix3) obj);
			}
			return false;
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return M11.GetHashCode() + M12.GetHashCode() + M13.GetHashCode() + M21.GetHashCode() + M22.GetHashCode() + M23.GetHashCode() +
				M31.GetHashCode() + M32.GetHashCode() + M33.GetHashCode();
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override String ToString()
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			Object[] args = new object[] { M11.ToString(info), M12.ToString(info), M13.ToString(info),
				M21.ToString(info), M22.ToString(info), M23.ToString(info),
				M31.ToString(info), M32.ToString(info), M33.ToString(info)};
			return String.Format(info, "{{[M11:{0} M12:{1} M13:{2}] [M21:{3} M22:{4} M23:{5}] [M31:{6} M32:{7} M33:{8}]}}", args);
		}
	}
}
