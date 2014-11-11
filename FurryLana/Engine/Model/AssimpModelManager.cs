//
//  AssimpModelManager.cs
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assimp;
using FurryLana.Engine.Model.Interfaces;

namespace FurryLana.Engine.Model
{
    /// <summary>
    /// Assimp model manager.
    /// </summary>
    public class AssimpModelManager : IModelManager
    {
        List<IModel>     models;
        PostProcessSteps AssimpPostProcessSteps;

        #region IModelManager implementation

        /// <summary>
        /// Loads model from location.
        /// </summary>
        /// <returns>The model.</returns>
        /// <param name="location">The path to the model.</param>
        /// <param name="path">Path.</param>
        public IModel LoadFromLocation (string path)
        {
            Scene model;
            using (AssimpContext importer = new AssimpContext ())
            {
                try
                {
                    model = importer.ImportFile (path, AssimpPostProcessSteps);
                }
                catch (FileNotFoundException e)
                {
                    throw new FileNotFoundException ("Model file \"" + path + "\" was not found!", path, e);
                }
                catch (AssimpException e)
                {
                    throw new AssimpException ("Error during model loading via Assimp (see inner exception)!", e);
                }
                catch (ObjectDisposedException e)
                {
                    throw new ObjectDisposedException ("Invalid Assimp context!", e);
                }
                catch (Exception e)
                {
                    throw new Exception ("Unknown error during loading file \"" + path + " via Assimp!", e);
                }
                
                if (model == null)
                {
                    throw new Exception ("Unknown error during loading file \"" + path + " via Assimp!");
                }
            }

            IModel imodel = new AssimpModel (model);

            Add (imodel);

            return imodel;
        }

        /// <summary>
        /// Loads models from xml.
        /// </summary>
        /// <returns>The model.</returns>
        /// <param name="filepath">Path to xml file.</param>
        public IModel LoadFromXML (string filepath)
        {
            throw new NotImplementedException ("Sorry for that :(");
        }

        /// <summary>
        /// Clear all models from model manager.
        /// </summary>
        public void Clear ()
        {
            models.ForEach ((m) => { m.Destroy (); });
            models.Clear ();
        }

        #endregion

        #region IManager implementation

        /// <summary>
        /// Add the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Add (IModel item)
        {
            models.Add (item);
            item.Init ();

            NeedsLoad (this, null);
        }

        /// <summary>
        /// Remove the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Remove (IModel item)
        {
            models.Remove (item);
        }

        /// <summary>
        /// Remove the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="name">Name.</param>
        public void Remove (string name)
        {
            models.RemoveAll ((m) => { return m.Name == name; });
        }

        /// <summary>
        /// Gets the IManageable by name.
        /// </summary>
        /// <returns>The IManageable.</returns>
        /// <param name="name">Name.</param>
        public IModel GetByName (string name)
        {
            return models.Find ((m) => { return m.Name == name; });
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return models.Count;
            }
        }

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator GetEnumerator ()
        {
            return models.GetEnumerator ();
        }

        #endregion

        #region IResource implementation

        /// <summary>
        /// Init this resource. Initialzes the resource within a valid gl context.
        /// 
        /// Why not use the constructor?:
        /// The constructor may not have a valid gl context to initialize gl components.
        /// </summary>
        public void Init ()
        {
            models = new List<IModel> ();

            AssimpPostProcessSteps =
                PostProcessSteps.Triangulate |
                    PostProcessSteps.GenerateNormals |
                    PostProcessSteps.OptimizeMeshes |
                    PostProcessSteps.JoinIdenticalVertices |
                    PostProcessSteps.ImproveCacheLocality;
        }

        /// <summary>
        /// Load this resource. This method *should* be called from an extra loading thread with a shared gl context.
        /// </summary>
        public void Load ()
        {
            Loaded = false;
            models.ForEach ((m) => { if (!m.Loaded) m.Load (); });
            Loaded = true;
        }

        /// <summary>
        /// Destroy this resource.
        /// 
        /// Why not IDisposable:
        /// IDisposable is called from within the grabage collector context so we do not have a valid gl context there.
        /// Therefore I added the Destroy function as this would be called by the parent instance within a valid gl
        /// context.
        /// </summary>
        public void Destroy ()
        {
            models.ForEach ((m) => { m.Destroy (); });
            models.Clear();
            models = null;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Engine.Model.AssimpModelManager"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; protected set; }

        /// <summary>
        /// Fire this event when you need the Load function to be called.
        /// For example after init or when new resources needs to be loaded.
        /// </summary>
        /// <value>NeedsLoad handlers.</value>
        public EventHandler NeedsLoad { get; set; }

        #endregion
    }
}

