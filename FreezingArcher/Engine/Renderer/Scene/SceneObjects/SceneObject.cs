//
//  SceneObject.cs
//
//  Author:
//       dboeg <${AuthorEmail}>
//
//  Copyright (c) 2015 dboeg
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
using FreezingArcher.Renderer;
using System.Collections.Generic;

namespace FreezingArcher.Renderer.Scene.SceneObjects
{
    public delegate bool SceneObjectChangedDelegate(SceneObject sender);

    public abstract class SceneObject
    {
        private Vector3 m_Position;
        private Quaternion m_Rotation;
        private Vector3 m_Scaling;

        public Vector3 Position 
        {
            get
            {
                return m_Position;
            }

            set
            {
                m_Position = value;
                HasChanged = true;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return m_Rotation;
            }

            set
            {
                m_Rotation = value;
                HasChanged = true;
            }
        }

        public Vector3 Scaling 
        {
            get
            {
                return m_Scaling;
            }

            set
            {
                m_Scaling = value;
                HasChanged = true;
            }
        }

        private bool m_HasChanged = false;
        public bool HasChanged
        { 
            get
            {
                return m_HasChanged;
            }

            protected set
            {
                m_HasChanged = true;
                m_HasChanged = !SceneObjectChanged(this);
            }
        }

        public Matrix WorldMatrix 
        {
            get
            {
                return Matrix.CreateScale(Scaling) * Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Position);
            }
        }

        public virtual SceneObjectArrayInstanceData GetData()
        {
            SceneObjectArrayInstanceData data = new SceneObjectArrayInstanceData();
            data.World = WorldMatrix;

            return data;
        }

        public virtual void Update(){}

        public virtual bool Init(RendererContext rc){return true;}
        public abstract void Draw(RendererContext rc);
        public abstract void DrawInstanced(RendererContext rc, int count);
        public abstract string GetName();

        public event SceneObjectChangedDelegate SceneObjectChanged = delegate(SceneObject obj){return true;};

        public SceneObject()
        {
            Position = Vector3.Zero;
            Rotation = Quaternion.Identity;
            Scaling = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }
}

