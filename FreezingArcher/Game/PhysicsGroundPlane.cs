//
//  PhysicsGroundPlane.cs
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
using System;
using Henge3D.Physics;
using Henge3D;
using FreezingArcher.Math;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Renderer;
using FreezingArcher.Output;

namespace FreezingArcher.Game
{
    public class PhysicsGroundPlane : RigidBody
    {
        ModelSceneObject model;

        public PhysicsGroundPlane (RendererContext rc)
        {
            Skin.DefaultMaterial = new Henge3D.Physics.Material(1f, 0.5f);
            MassProperties = new MassProperties(float.PositiveInfinity,Matrix.Identity);
            Skin.Add(new PlanePart(-Vector3.UnitY, Vector3.UnitY));
            model = new ModelSceneObject("lib/Renderer/TestGraphics/Ground/ground.xml");
            rc.Scene.AddObject(model);
        }

        public void UpdateModel ()
        {
            model.Position = Transform.Position;
            model.Rotation = Transform.Orientation;
            model.Scaling = new Vector3(Transform.Scale, Transform.Scale, Transform.Scale);
            //Logger.Log.AddLogEntry(LogLevel.Info, "PhysicsGroundPlane", "Position: {0}", model.Position);
        }
    }
}
