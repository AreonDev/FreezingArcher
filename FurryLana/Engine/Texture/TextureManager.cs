//
//  TextureManager.cs
//
//  Author:
//       Paul Stang <>
//
//  Copyright (c) 2014 Paul Stang
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
using System.Drawing;
using FurryLana.Engine.Texture.Interfaces;

namespace FurryLana.Engine.Texture
{
    /// <summary>
    /// TexturesManager implemantation
    /// </summary>
    public class TextureManager : ITextureManager
    {
        protected List<ITexture> textures;

        public void LoadFromLocation(string location)
        {
            Add(new Texture(new Bitmap(location), location));
        }

        public void LoadFromXMl(string filepath)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Clear all textures from the texture manager.
        /// </summary>
        public void Clear()
        {
            textures.Clear();
        }
        /// <summary>
        /// Add the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Add(ITexture item)
        {
            textures.Add(item);
            item.Init();

            NeedsLoad(this,null);
        }
        /// <summary>
        /// Remove the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Remove(ITexture item)
        {
            textures.Remove(item);
        }
        /// <summary>
        /// Remove by the specified name.
        /// </summary>
        /// <param name="name">Name.</param>
        public void Remove(string name)
        {
            textures.RemoveAll((t) => { return t.Name == name; });
        }
        /// <summary>
        /// Gets the IManageable by name.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>
        /// The IManageable.
        /// </returns>
        public ITexture GetByName(string name)
        {
            return textures.Find((t) => { return t.Name == name; });
        }
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count
        {
            get { return textures.Count; }
        }

        /// <summary>
        /// Init this resource. This method may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        public void Init()
        {
           textures = new List<ITexture>();
        }

        /// <summary>
        /// Gets the init jobs.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs (List<Action> list)
        {
            list.Add (Init);
            return list;
        }

        /// <summary>
        /// Load this resource. This method *should* be called from an extra loading thread with a shared gl context.
        /// </summary>
        public void Load()
        {
            Loaded = false;
            textures.ForEach((t) => {
                if (!t.Loaded)
                    t.Load();
            });
            Loaded = true;
        }

        /// <summary>
        /// Gets the load jobs.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            list.Add (Load);
            NeedsLoad = reloader;
            return list;
        }

        /// <summary>
        /// Destroy this resource.
        /// Why not IDisposable:
        /// IDisposable is called from within the grabage collector context so we do not have a valid gl context there.
        /// Therefore I added the Destroy function as this would be called by the parent instance within a valid gl
        /// context.
        /// </summary>
        public void Destroy()
        {
            textures.ForEach((t) => { t.Destroy(); });
            textures.Clear();
            textures = null;
        }

        /// <summary>
        /// Gets a value indicating whether this
        /// <see cref="FurryLana.Engine.Graphics.Interfaces.IResource" /> is loaded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if loaded; otherwise, <c>false</c>.
        /// </value>
        public bool Loaded { get; protected set; }

        public EventHandler NeedsLoad { get; set; }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return textures.GetEnumerator();
        }
    }
}
