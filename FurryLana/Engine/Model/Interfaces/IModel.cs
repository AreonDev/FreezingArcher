//
//  IModel.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2014 Fin Christensen
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
using System.Collections.Generic;
using Assimp;
using FurryLana.Engine.Graphics.Interfaces;

namespace FurryLana.Engine.Model.Interfaces
{
    /// <summary>
    /// Model interface.
    /// </summary>
    public interface IModel : IManageable, IGraphicsResource
    {
	/*List<IAnimation> Animations { get; set; }
	List<ICamera>    Cameras    { get; set; }
	List<ILight>     Lights     { get; set; }
	List<IMaterial>  Materials  { get; set; }
	List<IMesh>      Meshes     { get; set; }
	List<ITexture>   Textures   { get; set; }*/

        List<Animation>       Animations { get; }
        List<Assimp.Camera>   Cameras    { get; }
        List<Light>           Lights     { get; }
        List<Assimp.Material>        Materials  { get; }
        List<Mesh>            Meshes     { get; }
        List<EmbeddedTexture> Textures   { get; }
    }
}
