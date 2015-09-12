//
//  UnderworldMazeTheme.cs
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
using FreezingArcher.Math;
using FreezingArcher.Core;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Content;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;

namespace FreezingArcher.Game.Maze
{
    public sealed class UnderworldMazeTheme : IMazeTheme
    {
        #region IMazeTheme implementation

        #region locals

        SceneObjectArray scnobjarr_wall, scnobjarr_ground, scnobjarr_ceiling;

        Type[] systems;

        Vector3 scale;

        GameState state;

        #endregion

        public void Init (GameState state)
        {
            this.state = state;

            scnobjarr_wall = new SceneObjectArray ("ModelSceneObject_lib/Renderer/TestGraphics/UnderworldWall/underworld_wall.xml");
            scnobjarr_wall.LayoutLocationOffset = 10;
            state.Scene.AddObject (scnobjarr_wall);

            scnobjarr_ground = new SceneObjectArray ("ModelSceneObject_lib/Renderer/TestGraphics/Ground/underworld_ground.xml");
            scnobjarr_ground.LayoutLocationOffset = 10;
            state.Scene.AddObject (scnobjarr_ground);

            scnobjarr_ceiling = new SceneObjectArray ("ModelSceneObject_lib/Renderer/TestGraphics/UnderworldCeiling/underworld_ceiling.xml");
            scnobjarr_ceiling.LayoutLocationOffset = 10;
            //state.Scene.AddObject (scnobjarr_ceiling);

            scnobjarr_wall.BeginPrepare ();
            scnobjarr_ground.BeginPrepare ();
            //scnobjarr_ceiling.BeginPrepare ();

            systems = new[] { typeof (ModelSystem), typeof (PhysicsSystem) };

            scale = new Vector3 (4, 4, 4);
        }

        public Entity ProcessAndAddCell (MazeCell cell, Vector3 worldPosition, Vector2i gridPosition)
        {
            Entity entity = null;
            if (cell.MazeCellType == MazeCellType.Ground)
            {
                entity = EntityFactory.Instance.CreateWith ("ground" + gridPosition.X + "." + gridPosition.Y,
                    state.MessageProxy, systems: systems);
                var model = new ModelSceneObject ("lib/Renderer/TestGraphics/Ground/underworld_ground.xml");
                entity.GetComponent<ModelComponent>().Model = model;

                scnobjarr_ground.AddObject (model);
                //state.Scene.AddObject(model);

                var transform = entity.GetComponent<TransformComponent>();
                transform.Position = worldPosition;
                transform.Scale = scale;

                var body = new RigidBody (new BoxShape (2.0f * scale.X, 0.2f, 2.0f * scale.Y));
                body.Position = new JVector(transform.Position.X, transform.Position.Y - 0.1f, transform.Position.Z);
                body.Material.Restitution = -10;
                body.IsStatic = true;
                body.Tag = cell;

                entity.GetComponent<PhysicsComponent> ().RigidBody = body;
                entity.GetComponent<PhysicsComponent> ().World = state.PhysicsManager.World;
                entity.GetComponent<PhysicsComponent> ().PhysicsApplying =
                    AffectedByPhysics.Orientation | AffectedByPhysics.Position;
                
                state.PhysicsManager.World.AddBody (body);

                if (cell.IsExit)
                {
                    var exitEntity = EntityFactory.Instance.CreateWith ("underworld_exit", state.MessageProxy,
                        systems: systems);

                    var exitModel = new ModelSceneObject("lib/Renderer/TestGraphics/UnderworldExit/underworld_exit.xml");
                    exitEntity.GetComponent<ModelComponent>().Model = exitModel;
                    state.Scene.AddObject(exitModel);

                    var exitTransform = exitEntity.GetComponent<TransformComponent>();
                    exitTransform.Position = new Vector3 (worldPosition.X, -.5f, worldPosition.Z);
                    exitTransform.Scale = scale;

                    if (cell.Position.Y == 0)
                    {
                        exitTransform.Rotation = Quaternion.FromAxisAngle (Vector3.UnitY, MathHelper.ThreePiOver2);
                    }
                    else if (cell.Position.X == 29)
                    {
                        exitTransform.Rotation = Quaternion.FromAxisAngle (Vector3.UnitY, MathHelper.Pi);
                    }
                    else if (cell.Position.Y == 29)
                    {
                        exitTransform.Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.PiOver2);
                    }

                    var exitBody = new RigidBody(new BoxShape (scale.X * 2, scale.Y * 4, scale.Z * 2));
                    exitBody.Position = exitTransform.Position.ToJitterVector () + JVector.Up * (scale.Y * 2);
                    exitBody.Orientation = JMatrix.CreateFromQuaternion(exitTransform.Rotation.ToJitterQuaternion());
                    exitBody.Material.Restitution = -10;
                    exitBody.IsStatic = true;
                    exitBody.Tag = exitEntity;

                    exitEntity.GetComponent<PhysicsComponent> ().RigidBody = exitBody;
                    exitEntity.GetComponent<PhysicsComponent> ().World = state.PhysicsManager.World;
                    exitEntity.GetComponent<PhysicsComponent> ().PhysicsApplying =
                        AffectedByPhysics.Orientation | AffectedByPhysics.Position;

                    state.PhysicsManager.World.AddBody(exitBody);

                    exitEntity.Suspend();
                }
                entity.Suspend();
            }
            else if (cell.MazeCellType == MazeCellType.Wall)
            {
                entity = EntityFactory.Instance.CreateWith ("wall" + gridPosition.X + "." + gridPosition.Y,
                    state.MessageProxy, new[] { typeof (HealthComponent), typeof(WallComponent) }, systems);
                var model = new ModelSceneObject ("lib/Renderer/TestGraphics/UnderworldWall/underworld_wall.xml");
                entity.GetComponent<ModelComponent>().Model = model;

                scnobjarr_wall.AddObject (model);
                //state.Scene.AddObject(model);

                var transform = entity.GetComponent<TransformComponent>();
                transform.Position = new Vector3 (worldPosition.X, -0.5f, worldPosition.Z);
                transform.Rotation = cell.Rotation;
                transform.Scale = scale;

                var body = new RigidBody (new BoxShape (scale.X * 2, scale.Y * 4.5f, scale.Z * 2));
                body.Position = transform.Position.ToJitterVector () + JVector.Up * (scale.Y * 4.5f * 0.5f);
                body.Material.Restitution = -10;
                body.IsStatic = true;
                body.Tag = entity;

                entity.GetComponent<PhysicsComponent> ().RigidBody = body;
                entity.GetComponent<PhysicsComponent> ().World = state.PhysicsManager.World;
                entity.GetComponent<PhysicsComponent> ().PhysicsApplying =
                    AffectedByPhysics.Orientation | AffectedByPhysics.Position;

                entity.GetComponent<WallComponent>().IsEdge = cell.IsEdge;
                entity.GetComponent<WallComponent>().IsOverworld = false;

                state.PhysicsManager.World.AddBody (body);

                entity.Suspend();

                var ground = EntityFactory.Instance.CreateWith ("ground" + gridPosition.X + "." + gridPosition.Y,
                    state.MessageProxy, systems: systems);
                var groundModel = new ModelSceneObject ("lib/Renderer/TestGraphics/Ground/underworld_ground.xml");
                ground.GetComponent<ModelComponent>().Model = groundModel;

                scnobjarr_ground.AddObject (groundModel);
                //state.Scene.AddObject(model);

                var groundTransform = ground.GetComponent<TransformComponent>();
                groundTransform.Position = worldPosition;
                groundTransform.Scale = scale;

                var groundBody = new RigidBody (new BoxShape (2.0f * scale.X, 0.2f, 2.0f * scale.Y));
                groundBody.Position = new JVector(groundTransform.Position.X, groundTransform.Position.Y - 0.1f, groundTransform.Position.Z);
                groundBody.Material.Restitution = -10;
                groundBody.IsStatic = true;
                groundBody.Tag = cell;

                ground.GetComponent<PhysicsComponent> ().RigidBody = groundBody;
                ground.GetComponent<PhysicsComponent> ().World = state.PhysicsManager.World;
                ground.GetComponent<PhysicsComponent> ().PhysicsApplying =
                    AffectedByPhysics.Orientation | AffectedByPhysics.Position;

                state.PhysicsManager.World.AddBody (groundBody);

                ground.Suspend();
            }

            return entity;
        }

        public void Finish ()
        {
            scnobjarr_wall.EndPrepare ();
            scnobjarr_ground.EndPrepare ();
            //scnobjarr_ceiling.EndPrepare ();
        }

        #endregion
    }
}
