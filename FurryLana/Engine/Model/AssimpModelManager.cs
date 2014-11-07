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
using FurryLana.Engine.Model.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using Assimp;
using System.IO;

namespace FurryLana.Engine.Model
{
    public class AssimpModelManager : IModelManager
    {
        List<IModel>     models;
        PostProcessSteps AssimpPostProcessSteps;

        #region IModelManager implementation

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

        public IModel LoadFromXML (string filepath)
        {
            throw new NotImplementedException ("Sorry for that :(");
        }

        public void Clear ()
        {
            models.ForEach ((m) => { m.Destroy (); });
            models.Clear ();
        }

        #endregion

        #region IManager implementation

        public void Add (IModel item)
        {
            models.Add (item);
            item.Init ();

            NeedsLoad (this, null);
        }

        public void Remove (IModel item)
        {
            models.Remove (item);
        }

        public void Remove (string name)
        {
            models.RemoveAll ((m) => { return m.Name == name; });
        }

        public IModel GetByName (string name)
        {
            return models.Find ((m) => { return m.Name == name; });
        }

        public int Count
        {
            get
            {
                return models.Count;
            }
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator GetEnumerator ()
        {
            return models.GetEnumerator ();
        }

        #endregion

        #region IResource implementation

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

        public void Load ()
        {
            Loaded = false;
            models.ForEach ((m) => { m.Load (); });
            Loaded = true;
        }

        public void Destroy ()
        {
            models.ForEach ((m) => { m.Destroy (); });
        }

        public bool Loaded { get; protected set; }

        public EventHandler NeedsLoad { get; set; }

        #endregion
    }
}

