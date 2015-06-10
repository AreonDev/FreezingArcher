//
//  PhysicsTest.cs
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
using Henge3D;
using Henge3D.Physics;
using FreezingArcher.Renderer;
using FreezingArcher.Math;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Messaging;
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Output;

namespace FreezingArcher.Game
{
    public class PhysicsBody : RigidBody
    {
        ModelSceneObject model;

        public PhysicsBody(RendererContext rendererContext, MessageManager msgmnr, string xmlPath)
        {
            model = new ModelSceneObject(xmlPath);
            rendererContext.Scene.AddObject(model);

            model.Position = new Vector3 (10, 100, 10);
            Freeze();
            MassProperties = MassProperties.FromCuboid(3, new Vector3 (1, 2, 1));
            Vector3 p1 = new Vector3(0f, 0f, 1) + model.Position, p2 = new Vector3(0f, 0f, -1) + model.Position;
            Skin.Add(new CapsulePart(new Capsule(p1, p2, 0.5f)), new Henge3D.Physics.Material(0.5f, 2));
        }

        public void UpdateModel()
        {
            model.Position = Transform.Position;
            model.Rotation = Transform.Orientation;
            model.Scaling = new Vector3 (Transform.Scale, Transform.Scale, Transform.Scale);
            //Logger.Log.AddLogEntry(LogLevel.Info, "PhysicsBody", "Position: {0}", model.Position);
        }
    }

    public class PhysicsTest : IMessageConsumer, IDisposable
    {
        PhysicsManager physicsManager;
        PhysicsBody body;
        PhysicsGroundPlane plane;

        public PhysicsTest (RendererContext rc, MessageManager msgmnr)
        {
            physicsManager = new PhysicsManager();
            body = new PhysicsBody(rc, msgmnr, "lib/Renderer/TestGraphics/Wall/wall.xml");
            plane = new PhysicsGroundPlane(rc);
            physicsManager.Initialize();
            physicsManager.Add(body);
            physicsManager.Add(plane);
            physicsManager.Gravity = new Vector3(0, -9.81f, 0);
            msgmnr += this;
        }

        #region IMessageConsumer implementation

        Vector3 force = new Vector3(200,0,0);

        public void ConsumeMessage (IMessage msg)
        {
            UpdateMessage um = msg as UpdateMessage;
            if (um != null)
            {
                physicsManager.Update((float) um.TimeStamp.TotalMilliseconds);
                body.UpdateModel();
                plane.UpdateModel();
            }

            InputMessage im = msg as InputMessage;
            if (im != null)
            {
                if (im.IsActionPressed("frame"))
                {
                    Logger.Log.AddLogEntry(LogLevel.Debug, "Physics", "Forcing force to be forced on force...");
                    body.ApplyForce(ref force);
                }
            }
        }

        public int[] ValidMessages
        {
            get
            {
                return new[] { (int) MessageId.Update, (int) MessageId.Input };
            }
        }

        #endregion

        #region IDisposable implementation

        public void Dispose ()
        {
            physicsManager.Dispose();
        }

        #endregion
    }
}
