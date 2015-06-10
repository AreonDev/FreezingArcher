//
//  ModelSceneObject.cs
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
using System.Collections.Generic;

namespace FreezingArcher.Renderer.Scene.SceneObjects
{
    public class ModelSceneObject : SceneObject
    {
        private string ModelPath;
        private bool   LoadModel;
        private Model  MyModel;
        private bool IsInitialized = false;

        private RendererContext PrivateRendererContext = null;

        private static object CachingListLock = new object();
        private static List<ModelSceneObject> CachingList;

        public Model Model
        {
            get
            {
                return MyModel;
            }
        }

        public override void Draw(RendererContext rc)
        {
            if(MyModel != null)
                rc.DrawModel(MyModel, this.WorldMatrix, 1, rc.Scene);
        }

        public override void DrawInstanced(RendererContext rc, int count)
        {
            if(MyModel != null)
                rc.DrawModel(MyModel, count == 1 ? WorldMatrix : FreezingArcher.Math.Matrix.Identity, count, rc.Scene);

            if(!LoadModel)
                rc.DrawModel(MyModel, count == 1 ? WorldMatrix : FreezingArcher.Math.Matrix.Identity, count, rc.Scene);
        }

        public override void PrepareInstanced(RendererContext rc, VertexBufferLayoutKind[] vblks, VertexBuffer vb)
        {
            if (MyModel != null)
            {
                foreach (Mesh msh in MyModel.Meshes)
                {
                    msh.m_VertexBufferArray.BindVertexBufferArray();

                    vb.BindBuffer();

                    foreach (VertexBufferLayoutKind vblk in vblks)
                    {
                        rc.EnableVertexAttribute((int)vblk.AttributeID);
                        rc.VertexAttributePointer(vblk);
                        rc.VertexAttributeDivisor((int)vblk.AttributeID, 1);
                    }

                    vb.UnbindBuffer();

                    msh.m_VertexBufferArray.UnbindVertexBufferArray();
                }
            }
        }

        public override void UnPrepareInstanced(RendererContext rc, VertexBufferLayoutKind[] vblks)
        {
            if (MyModel != null)
            {
                foreach (Mesh msh in MyModel.Meshes)
                {
                    msh.m_VertexBufferArray.BindVertexBufferArray();

                    foreach (VertexBufferLayoutKind vblk in vblks)
                        rc.DisableVertexAttribute((int)vblk.AttributeID);

                    msh.m_VertexBufferArray.UnbindVertexBufferArray();
                }
            }
            
        }

        public override bool Init(RendererContext rc)
        {
            if (!IsInitialized)
            {
                if (LoadModel)
                    MyModel = rc.LoadModel(ModelPath);

                PrivateRendererContext = rc;

                IsInitialized = true;
            }

            return true;
        }

        public override string GetName()
        {
            return "ModelSceneObject_" + ModelPath;
        }

        public override SceneObject Clone()
        {
            ModelSceneObject msobj = new ModelSceneObject(ModelPath, false);

            //Delegate[] bla = this.SceneObjectChanged.GetInvocationList();

            //for (int i = 0; i < bla.Length; i++)
            //    msobj.SceneObjectChanged += bla[i];
                
            msobj.Scaling = this.Scaling;
            msobj.Rotation = this.Rotation;
            msobj.Position = this.Position;
            msobj.MyModel = this.MyModel;
            msobj.LoadModel = false;

            return msobj;
        }

        public ModelSceneObject(string path, bool load = true)
        {
            LoadModel = load;
            ModelPath = path;

            lock (CachingListLock)
            {
                if (CachingList == null)
                    CachingList = new List<ModelSceneObject>();

                foreach (ModelSceneObject obj in CachingList)
                {
                    if (obj.ModelPath == path)
                    {
                        LoadModel = false;
                        this.MyModel = obj.MyModel;
                    }
                }
            }

            CachingList.Add(this);
        }

        ~ModelSceneObject()
        {
            lock (CachingListLock)
            {
                if (CachingList != null)
                {
                    CachingList.Remove(this);

                    //Check, if something more in this list, if not, clear loaded model
                    bool contains = false;

                    foreach (ModelSceneObject obj in CachingList)
                    {
                        if (obj.ModelPath == this.ModelPath)
                            contains = true;
                    }

                    if (!contains)
                        PrivateRendererContext.DeleteModel(this.MyModel);
                }
            }
        }
    }
}

