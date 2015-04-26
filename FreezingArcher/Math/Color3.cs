#region License
// Copyright (c) 2013 Antonie Blom
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

#define MINIMAL

using System;
using System.Collections.Generic;
#if !MINIMAL
using System.Drawing;
#endif
using System.Text;
using System.Xml.Serialization;

namespace FreezingArcher.Math
{
	/// <summary>
	/// Represents a color with 4 floating-point components (R, G, B).
	/// </summary>
	[Serializable]
	public struct Color3 : IEquatable<Color3> {
		#region Fields

		/// <summary>
		/// The red component of this Color4 structure.
		/// </summary>
		public float R;

		/// <summary>
		/// The green component of this Color4 structure.
		/// </summary>
		public float G;

		/// <summary>
		/// The blue component of this Color4 structure.
		/// </summary>
		public float B;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new Color4 structure from the specified components.
		/// </summary>
		/// <param name="r">The red component of the new Color4 structure.</param>
		/// <param name="g">The green component of the new Color4 structure.</param>
		/// <param name="b">The blue component of the new Color4 structure.</param>
		public Color3(float r, float g, float b) {
			R = r;
			G = g;
			B = b;
		}

		/// <summary>
		/// Constructs a new Color4 structure from the specified components.
		/// </summary>
		/// <param name="r">The red component of the new Color4 structure.</param>
		/// <param name="g">The green component of the new Color4 structure.</param>
		/// <param name="b">The blue component of the new Color4 structure.</param>
		public Color3(byte r, byte g, byte b) {
			R = r / (float)Byte.MaxValue;
			G = g / (float)Byte.MaxValue;
			B = b / (float)Byte.MaxValue;
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Converts this color to an integer representation with 8 bits per channel.
		/// </summary>
		/// <returns>A <see cref="System.Int32"/> that represents this instance.</returns>
		/// <remarks>This method is intended only for compatibility with System.Drawing. It compresses the color into 8 bits per channel, which means color information is lost.</remarks>
		public int ToArgb() {
			uint value =
				(uint)(255 * Byte.MaxValue) << 24 |
				(uint)(R * Byte.MaxValue) << 16 |
				(uint)(G * Byte.MaxValue) << 8 |
				(uint)(B * Byte.MaxValue);

			return unchecked((int)value);
		}

		/// <summary>
		/// Compares the specified Color4 structures for equality.
		/// </summary>
		/// <param name="left">The left-hand side of the comparison.</param>
		/// <param name="right">The right-hand side of the comparison.</param>
		/// <returns>True if left is equal to right; false otherwise.</returns>
		public static bool operator ==(Color3 left, Color3 right) {
			return left.Equals(right);
		}

		/// <summary>
		/// Compares the specified Color4 structures for inequality.
		/// </summary>
		/// <param name="left">The left-hand side of the comparison.</param>
		/// <param name="right">The right-hand side of the comparison.</param>
		/// <returns>True if left is not equal to right; false otherwise.</returns>
		public static bool operator !=(Color3 left, Color3 right) {
			return !left.Equals(right);
		}

		//		/// <summary>
		//		/// Converts the specified System.Drawing.Color to a Color4 structure.
		//		/// </summary>
		//		/// <param name="color">The System.Drawing.Color to convert.</param>
		//		/// <returns>A new Color4 structure containing the converted components.</returns>
		//		public static implicit operator Color4(Color color) {
		//			return new Color4(color.R, color.G, color.B, color.A);
		//		}
		//
		//		/// <summary>
		//		/// Converts the specified Color4 to a System.Drawing.Color structure.
		//		/// </summary>
		//		/// <param name="color">The Color4 to convert.</param>
		//		/// <returns>A new System.Drawing.Color structure containing the converted components.</returns>
		//		public static explicit operator Color(Color4 color) {
		//			return Color.FromArgb(
		//				(int)(color.A * Byte.MaxValue),
		//				(int)(color.R * Byte.MaxValue),
		//				(int)(color.G * Byte.MaxValue),
		//				(int)(color.B * Byte.MaxValue));
		//		}

		/// <summary>
		/// Compares whether this Color4 structure is equal to the specified object.
		/// </summary>
		/// <param name="obj">An object to compare to.</param>
		/// <returns>True obj is a Color4 structure with the same components as this Color4; false otherwise.</returns>
		public override bool Equals(object obj) {
			if (!(obj is Color3))
				return false;

			return Equals((Color3)obj);
		}

		/// <summary>
		/// Calculates the hash code for this Color4 structure.
		/// </summary>
		/// <returns>A System.Int32 containing the hashcode of this Color4 structure.</returns>
		public override int GetHashCode() {
			return ToArgb();
		}

		/// <summary>
		/// Creates a System.String that describes this Color4 structure.
		/// </summary>
		/// <returns>A System.String that describes this Color4 structure.</returns>
		public override string ToString() {
			return String.Format("{{(R, G, B) = ({0}, {1}, {2})}}", R.ToString(), G.ToString(), B.ToString());
		}

		#region System colors

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 255, 255).
		/// </summary>
		public static Color3 Transparent { get { return new Color3(255, 255, 255); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (240, 248, 255).
		/// </summary>
		public static Color3 AliceBlue { get { return new Color3(240, 248, 255); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (250, 235, 215).
		/// </summary>
		public static Color3 AntiqueWhite { get { return new Color3(250, 235, 215); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (0, 255, 255).
		/// </summary>
		public static Color3 Aqua { get { return new Color3(0, 255, 255); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (127, 255, 212).
		/// </summary>
		public static Color3 Aquamarine { get { return new Color3(127, 255, 212); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (240, 255, 255).
		/// </summary>
		public static Color3 Azure { get { return new Color3(240, 255, 255); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (245, 245, 220).
		/// </summary>
		public static Color3 Beige { get { return new Color3(245, 245, 220); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 228, 196).
		/// </summary>
		public static Color3 Bisque { get { return new Color3(255, 228, 196); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (0, 0, 0).
		/// </summary>
		public static Color3 Black { get { return new Color3(0, 0, 0); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 235, 205).
		/// </summary>
		public static Color3 BlanchedAlmond { get { return new Color3(255, 235, 205); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (0, 0, 255).
		/// </summary>
		public static Color3 Blue { get { return new Color3(0, 0, 255); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (138, 43, 226).
		/// </summary>
		public static Color3 BlueViolet { get { return new Color3(138, 43, 226); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (165, 42, 42).
		/// </summary>
		public static Color3 Brown { get { return new Color3(165, 42, 42); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (222, 184, 135).
		/// </summary>
		public static Color3 BurlyWood { get { return new Color3(222, 184, 135); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (95, 158, 160).
		/// </summary>
		public static Color3 CadetBlue { get { return new Color3(95, 158, 160); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (127, 255, 0).
		/// </summary>
		public static Color3 Chartreuse { get { return new Color3(127, 255, 0); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (210, 105, 30).
		/// </summary>
		public static Color3 Chocolate { get { return new Color3(210, 105, 30); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 127, 80).
		/// </summary>
		public static Color3 Coral { get { return new Color3(255, 127, 80); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (100, 149, 237).
		/// </summary>
		public static Color3 CornflowerBlue { get { return new Color3(100, 149, 237); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 248, 220).
		/// </summary>
		public static Color3 Cornsilk { get { return new Color3(255, 248, 220); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (220, 20, 60).
		/// </summary>
		public static Color3 Crimson { get { return new Color3(220, 20, 60); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (0, 255, 255).
		/// </summary>
		public static Color3 Cyan { get { return new Color3(0, 255, 255); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (0, 0, 139).
		/// </summary>
		public static Color3 DarkBlue { get { return new Color3(0, 0, 139); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (0, 139, 139).
		/// </summary>
		public static Color3 DarkCyan { get { return new Color3(0, 139, 139); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (184, 134, 11).
		/// </summary>
		public static Color3 DarkGoldenrod { get { return new Color3(184, 134, 11); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (169, 169, 169).
		/// </summary>
		public static Color3 DarkGray { get { return new Color3(169, 169, 169); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (0, 100, 0).
		/// </summary>
		public static Color3 DarkGreen { get { return new Color3(0, 100, 0); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (189, 183, 107).
		/// </summary>
		public static Color3 DarkKhaki { get { return new Color3(189, 183, 107); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (139, 0, 139).
		/// </summary>
		public static Color3 DarkMagenta { get { return new Color3(139, 0, 139); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (85, 107, 47).
		/// </summary>
		public static Color3 DarkOliveGreen { get { return new Color3(85, 107, 47); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 140, 0).
		/// </summary>
		public static Color3 DarkOrange { get { return new Color3(255, 140, 0); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (153, 50, 204).
		/// </summary>
		public static Color3 DarkOrchid { get { return new Color3(153, 50, 204); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (139, 0, 0).
		/// </summary>
		public static Color3 DarkRed { get { return new Color3(139, 0, 0); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (233, 150, 122).
		/// </summary>
		public static Color3 DarkSalmon { get { return new Color3(233, 150, 122); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (143, 188, 139).
		/// </summary>
		public static Color3 DarkSeaGreen { get { return new Color3(143, 188, 139); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (72, 61, 139).
		/// </summary>
		public static Color3 DarkSlateBlue { get { return new Color3(72, 61, 139); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (47, 79, 79).
		/// </summary>
		public static Color3 DarkSlateGray { get { return new Color3(47, 79, 79); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (0, 206, 209).
		/// </summary>
		public static Color3 DarkTurquoise { get { return new Color3(0, 206, 209); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (148, 0, 211).
		/// </summary>
		public static Color3 DarkViolet { get { return new Color3(148, 0, 211); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 20, 147).
		/// </summary>
		public static Color3 DeepPink { get { return new Color3(255, 20, 147); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (0, 191, 255).
		/// </summary>
		public static Color3 DeepSkyBlue { get { return new Color3(0, 191, 255); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (105, 105, 105).
		/// </summary>
		public static Color3 DimGray { get { return new Color3(105, 105, 105); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (30, 144, 255).
		/// </summary>
		public static Color3 DodgerBlue { get { return new Color3(30, 144, 255); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (178, 34, 34).
		/// </summary>
		public static Color3 Firebrick { get { return new Color3(178, 34, 34); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 250, 240).
		/// </summary>
		public static Color3 FloralWhite { get { return new Color3(255, 250, 240); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (34, 139, 34).
		/// </summary>
		public static Color3 ForestGreen { get { return new Color3(34, 139, 34); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 0, 255).
		/// </summary>
		public static Color3 Fuchsia { get { return new Color3(255, 0, 255); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (220, 220, 220).
		/// </summary>
		public static Color3 Gainsboro { get { return new Color3(220, 220, 220); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (248, 248, 255).
		/// </summary>
		public static Color3 GhostWhite { get { return new Color3(248, 248, 255); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 215, 0).
		/// </summary>
		public static Color3 Gold { get { return new Color3(255, 215, 0); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (218, 165, 32).
		/// </summary>
		public static Color3 Goldenrod { get { return new Color3(218, 165, 32); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (128, 128, 128).
		/// </summary>
		public static Color3 Gray { get { return new Color3(128, 128, 128); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (0, 128, 0).
		/// </summary>
		public static Color3 Green { get { return new Color3(0, 128, 0); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (173, 255, 47).
		/// </summary>
		public static Color3 GreenYellow { get { return new Color3(173, 255, 47); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (240, 255, 240).
		/// </summary>
		public static Color3 Honeydew { get { return new Color3(240, 255, 240); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 105, 180).
		/// </summary>
		public static Color3 HotPink { get { return new Color3(255, 105, 180); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (205, 92, 92).
		/// </summary>
		public static Color3 IndianRed { get { return new Color3(205, 92, 92); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (75, 0, 130).
		/// </summary>
		public static Color3 Indigo { get { return new Color3(75, 0, 130); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 255, 240).
		/// </summary>
		public static Color3 Ivory { get { return new Color3(255, 255, 240); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (240, 230, 140).
		/// </summary>
		public static Color3 Khaki { get { return new Color3(240, 230, 140); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (230, 230, 250).
		/// </summary>
		public static Color3 Lavender { get { return new Color3(230, 230, 250); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 240, 245).
		/// </summary>
		public static Color3 LavenderBlush { get { return new Color3(255, 240, 245); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (124, 252, 0).
		/// </summary>
		public static Color3 LawnGreen { get { return new Color3(124, 252, 0); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 250, 205).
		/// </summary>
		public static Color3 LemonChiffon { get { return new Color3(255, 250, 205); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (173, 216, 230).
		/// </summary>
		public static Color3 LightBlue { get { return new Color3(173, 216, 230); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (240, 128, 128).
		/// </summary>
		public static Color3 LightCoral { get { return new Color3(240, 128, 128); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (224, 255, 255).
		/// </summary>
		public static Color3 LightCyan { get { return new Color3(224, 255, 255); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (250, 250, 210).
		/// </summary>
		public static Color3 LightGoldenrodYellow { get { return new Color3(250, 250, 210); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (144, 238, 144).
		/// </summary>
		public static Color3 LightGreen { get { return new Color3(144, 238, 144); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (211, 211, 211).
		/// </summary>
		public static Color3 LightGray { get { return new Color3(211, 211, 211); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 182, 193).
		/// </summary>
		public static Color3 LightPink { get { return new Color3(255, 182, 193); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 160, 122).
		/// </summary>
		public static Color3 LightSalmon { get { return new Color3(255, 160, 122); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (32, 178, 170).
		/// </summary>
		public static Color3 LightSeaGreen { get { return new Color3(32, 178, 170); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (135, 206, 250).
		/// </summary>
		public static Color3 LightSkyBlue { get { return new Color3(135, 206, 250); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (119, 136, 153).
		/// </summary>
		public static Color3 LightSlateGray { get { return new Color3(119, 136, 153); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (176, 196, 222).
		/// </summary>
		public static Color3 LightSteelBlue { get { return new Color3(176, 196, 222); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 255, 224).
		/// </summary>
		public static Color3 LightYellow { get { return new Color3(255, 255, 224); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (0, 255, 0).
		/// </summary>
		public static Color3 Lime { get { return new Color3(0, 255, 0); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (50, 205, 50).
		/// </summary>
		public static Color3 LimeGreen { get { return new Color3(50, 205, 50); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (250, 240, 230).
		/// </summary>
		public static Color3 Linen { get { return new Color3(250, 240, 230); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 0, 255).
		/// </summary>
		public static Color3 Magenta { get { return new Color3(255, 0, 255); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (128, 0, 0).
		/// </summary>
		public static Color3 Maroon { get { return new Color3(128, 0, 0); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (102, 205, 170).
		/// </summary>
		public static Color3 MediumAquamarine { get { return new Color3(102, 205, 170); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (0, 0, 205).
		/// </summary>
		public static Color3 MediumBlue { get { return new Color3(0, 0, 205); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (186, 85, 211).
		/// </summary>
		public static Color3 MediumOrchid { get { return new Color3(186, 85, 211); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (147, 112, 219).
		/// </summary>
		public static Color3 MediumPurple { get { return new Color3(147, 112, 219); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (60, 179, 113).
		/// </summary>
		public static Color3 MediumSeaGreen { get { return new Color3(60, 179, 113); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (123, 104, 238).
		/// </summary>
		public static Color3 MediumSlateBlue { get { return new Color3(123, 104, 238); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (0, 250, 154).
		/// </summary>
		public static Color3 MediumSpringGreen { get { return new Color3(0, 250, 154); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (72, 209, 204).
		/// </summary>
		public static Color3 MediumTurquoise { get { return new Color3(72, 209, 204); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (199, 21, 133).
		/// </summary>
		public static Color3 MediumVioletRed { get { return new Color3(199, 21, 133); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (25, 25, 112).
		/// </summary>
		public static Color3 MidnightBlue { get { return new Color3(25, 25, 112); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (245, 255, 250).
		/// </summary>
		public static Color3 MintCream { get { return new Color3(245, 255, 250); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 228, 225).
		/// </summary>
		public static Color3 MistyRose { get { return new Color3(255, 228, 225); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 228, 181).
		/// </summary>
		public static Color3 Moccasin { get { return new Color3(255, 228, 181); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 222, 173).
		/// </summary>
		public static Color3 NavajoWhite { get { return new Color3(255, 222, 173); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (0, 0, 128).
		/// </summary>
		public static Color3 Navy { get { return new Color3(0, 0, 128); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (253, 245, 230).
		/// </summary>
		public static Color3 OldLace { get { return new Color3(253, 245, 230); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (128, 128, 0).
		/// </summary>
		public static Color3 Olive { get { return new Color3(128, 128, 0); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (107, 142, 35).
		/// </summary>
		public static Color3 OliveDrab { get { return new Color3(107, 142, 35); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 165, 0).
		/// </summary>
		public static Color3 Orange { get { return new Color3(255, 165, 0); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 69, 0).
		/// </summary>
		public static Color3 OrangeRed { get { return new Color3(255, 69, 0); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (218, 112, 214).
		/// </summary>
		public static Color3 Orchid { get { return new Color3(218, 112, 214); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (238, 232, 170).
		/// </summary>
		public static Color3 PaleGoldenrod { get { return new Color3(238, 232, 170); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (152, 251, 152).
		/// </summary>
		public static Color3 PaleGreen { get { return new Color3(152, 251, 152); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (175, 238, 238).
		/// </summary>
		public static Color3 PaleTurquoise { get { return new Color3(175, 238, 238); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (219, 112, 147).
		/// </summary>
		public static Color3 PaleVioletRed { get { return new Color3(219, 112, 147); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 239, 213).
		/// </summary>
		public static Color3 PapayaWhip { get { return new Color3(255, 239, 213); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 218, 185).
		/// </summary>
		public static Color3 PeachPuff { get { return new Color3(255, 218, 185); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (205, 133, 63).
		/// </summary>
		public static Color3 Peru { get { return new Color3(205, 133, 63); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 192, 203).
		/// </summary>
		public static Color3 Pink { get { return new Color3(255, 192, 203); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (221, 160, 221).
		/// </summary>
		public static Color3 Plum { get { return new Color3(221, 160, 221); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (176, 224, 230).
		/// </summary>
		public static Color3 PowderBlue { get { return new Color3(176, 224, 230); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (128, 0, 128).
		/// </summary>
		public static Color3 Purple { get { return new Color3(128, 0, 128); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 0, 0).
		/// </summary>
		public static Color3 Red { get { return new Color3(255, 0, 0); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (188, 143, 143).
		/// </summary>
		public static Color3 RosyBrown { get { return new Color3(188, 143, 143); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (65, 105, 225).
		/// </summary>
		public static Color3 RoyalBlue { get { return new Color3(65, 105, 225); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (139, 69, 19).
		/// </summary>
		public static Color3 SaddleBrown { get { return new Color3(139, 69, 19); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (250, 128, 114).
		/// </summary>
		public static Color3 Salmon { get { return new Color3(250, 128, 114); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (244, 164, 96).
		/// </summary>
		public static Color3 SandyBrown { get { return new Color3(244, 164, 96); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (46, 139, 87).
		/// </summary>
		public static Color3 SeaGreen { get { return new Color3(46, 139, 87); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 245, 238).
		/// </summary>
		public static Color3 SeaShell { get { return new Color3(255, 245, 238); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (160, 82, 45).
		/// </summary>
		public static Color3 Sienna { get { return new Color3(160, 82, 45); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (192, 192, 192).
		/// </summary>
		public static Color3 Silver { get { return new Color3(192, 192, 192); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (135, 206, 235).
		/// </summary>
		public static Color3 SkyBlue { get { return new Color3(135, 206, 235); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (106, 90, 205).
		/// </summary>
		public static Color3 SlateBlue { get { return new Color3(106, 90, 205); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (112, 128, 144).
		/// </summary>
		public static Color3 SlateGray { get { return new Color3(112, 128, 144); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 250, 250).
		/// </summary>
		public static Color3 Snow { get { return new Color3(255, 250, 250); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (0, 255, 127).
		/// </summary>
		public static Color3 SpringGreen { get { return new Color3(0, 255, 127); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (70, 130, 180).
		/// </summary>
		public static Color3 SteelBlue { get { return new Color3(70, 130, 180); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (210, 180, 140).
		/// </summary>
		public static Color3 Tan { get { return new Color3(210, 180, 140); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (0, 128, 128).
		/// </summary>
		public static Color3 Teal { get { return new Color3(0, 128, 128); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (216, 191, 216).
		/// </summary>
		public static Color3 Thistle { get { return new Color3(216, 191, 216); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 99, 71).
		/// </summary>
		public static Color3 Tomato { get { return new Color3(255, 99, 71); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (64, 224, 208).
		/// </summary>
		public static Color3 Turquoise { get { return new Color3(64, 224, 208); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (238, 130, 238).
		/// </summary>
		public static Color3 Violet { get { return new Color3(238, 130, 238); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (245, 222, 179).
		/// </summary>
		public static Color3 Wheat { get { return new Color3(245, 222, 179); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 255, 255).
		/// </summary>
		public static Color3 White { get { return new Color3(255, 255, 255); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (245, 245, 245).
		/// </summary>
		public static Color3 WhiteSmoke { get { return new Color3(245, 245, 245); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (255, 255, 0).
		/// </summary>
		public static Color3 Yellow { get { return new Color3(255, 255, 0); } }

		/// <summary>
		/// Gets the system color with (R, G, B) = (154, 205, 50).
		/// </summary>
		public static Color3 YellowGreen { get { return new Color3(154, 205, 50); } }

		#endregion

		#endregion

		#region IEquatable<Color4> Members

		/// <summary>
		/// Compares whether this Color4 structure is equal to the specified Color4.
		/// </summary>
		/// <param name="other">The Color4 structure to compare to.</param>
		/// <returns>True if both Color4 structures contain the same components; false otherwise.</returns>
		public bool Equals(Color3 other) {
			return
				this.R == other.R &&
				this.G == other.G &&
				this.B == other.B;
		}

		#endregion
	}
}
