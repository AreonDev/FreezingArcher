//
//  AssimpModel.cs
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
using System;
using System.Collections.Generic;
using Assimp;
using FurryLana.Engine.Model.Interfaces;

namespace FurryLana.Engine.Model
{
    public class AssimpModel : IModel
    {
        public AssimpModel (Scene model)
        {
            this.model = model;
        }

        protected Scene model;

        #region IResource implementation

        public void Init ()
        {}

        public void Load ()
        {}

        public void Destroy ()
        {
            model.Clear ();
        }

        public bool Loaded { get; protected set; }

        public EventHandler NeedsLoad { get; set; }

        #endregion

        #region IFrameSyncedUpdate implementation

        public void FrameSyncedUpdate (float deltaTime)
        {
            throw new NotImplementedException ();
        }

        #endregion

        #region IUpdate implementation

        public void Update (int deltaTime)
        {}

        #endregion

        #region IDrawable implementation

        public void Draw ()
        {}

        #endregion

        #region IModel implementation

        public List<Animation> Animations
        {
            get
            {
                return model.Animations;
            }
        }

        public List<Assimp.Camera> Cameras
        {
            get
            {
                return model.Cameras;
            }
        }

        public List<Light> Lights
        {
            get
            {
                return model.Lights;
            }
        }

        public List<Assimp.Material> Materials
        {
            get
            {
                return model.Materials;
            }
        }

        public List<Mesh> Meshes
        {
            get
            {
                return model.Meshes;
            }
        }

        public List<EmbeddedTexture> Textures
        {
            get
            {
                return model.Textures;
            }
        }

        #endregion

        #region IManageable implementation

        public string Name { get; set; }

        #endregion
    }
}
