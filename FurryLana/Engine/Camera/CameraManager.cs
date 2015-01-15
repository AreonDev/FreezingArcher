//
//  CameraManager.cs
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
using FurryLana.Engine.Camera.Interfaces;
using FurryLana.Engine.Graphics;

namespace FurryLana.Engine.Camera
{
    /// <summary>
    /// Camera manager.
    /// </summary>
    public class CameraManager : ICameraManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Camera.CameraManager"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="initial">Initial.</param>
        public CameraManager (string name, ICameraManageable initial)
        {
            this.Name = name;
            Cameras = new List<ICameraManageable> ();
            Add (initial);
            if (initial is ICamera)
            {
                ActiveGroup = this;
                activeCamInternal = (ICamera) initial;
                activeCamInternal.Enable ();
            }
            else if (initial is ICameraManager)
            {
                ActiveGroup = (ICameraManager) initial;
                ActiveGroup.Enable ();
            }
            Loaded = true;
        }

        /// <summary>
        /// The cameras.
        /// </summary>
        protected List<ICameraManageable> Cameras;

        #region ICameraManager implementation

        /// <summary>
        /// Get the active camera
        /// </summary>
        /// <returns>The camera.</returns>
        public ICamera GetActive ()
        {
            if (ActiveGroup.Name == this.Name)
                return activeCamInternal;
            else
                return ActiveGroup.GetActive ();
        }

        /// <summary>
        /// Get the active camera group.
        /// </summary>
        /// <returns>The active group.</returns>
        public ICameraManager GetActiveGroup ()
        {
            return ActiveGroup;
        }

        /// <summary>
        /// Set the active camera.
        /// </summary>
        /// <param name="name">The camera name.</param>
        public void SetActive (string name)
        {
            if (name == null)
                throw new ArgumentNullException ("name", "The camera name must not be null!");
            if (ActiveGroup.Name != this.Name)
                ActiveGroup.SetActive (name);
            else
            {
                foreach (ICameraManageable c in Cameras)
                {
                    if (c is ICamera && c.Name == name)
                    {
                        activeCamInternal.Disable ();
                        this.ActiveGroup = this;
                        this.activeCamInternal = (ICamera) c;
                        activeCamInternal.Enable ();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Set the active camera.
        /// </summary>
        /// <param name="camera">Camera.</param>
        public void SetActive (ICamera camera)
        {
            if (camera == null)
                throw new ArgumentNullException ("camera", "The active camera must not be null!");
            
            SetActive (camera.Name);
        }

        /// <summary>
        /// Set the active camera and set a new group if necessary.
        /// </summary>
        /// <param name="name">The camera name.</param>
        public void SetActiveRecursive (string name)
        {
            if (name == null)
                throw new ArgumentNullException ("name", "The camera name must not be null!");
            foreach (ICameraManageable c in Cameras)
            {
                if (c is ICamera && c.Name == name)
                {
                    activeCamInternal.Disable ();
                    this.ActiveGroup = this;
                    this.activeCamInternal = (ICamera) c;
                    activeCamInternal.Enable ();
                    break;
                }
                else if (c is ICameraManager)
                { 
                    ((ICameraManager) c).SetActive (name);
                    this.ActiveGroup = (ICameraManager) c;
                    break;
                }
            }
        }

        /// <summary>
        /// Set the active camera and set a new group if necessary.
        /// </summary>
        /// <param name="camera">Camera.</param>
        public void SetActiveRecursive (ICamera camera)
        {
            if (camera == null)
                throw new ArgumentNullException ("camera", "The active camera must not be null!");
            
            SetActiveRecursive (camera.Name);
        }

        /// <summary>
        /// Set the active camera group.
        /// </summary>
        /// <param name="name">Name.</param>
        public void SetGroup (string name)
        {
            if (name == null)
                throw new ArgumentNullException ("name", "The group name must not be null!");
            if (name == this.Name)
            {
                if (this.activeCamInternal == null)
                    throw new InvalidOperationException ("You cannot set a group with no active camera! Have you " +
                                                         "forgot to add a camera?");
                this.ActiveGroup.Disable ();
                this.ActiveGroup = this;
                this.ActiveGroup.Enable ();
            }
            else
            {
                foreach (ICameraManageable c in Cameras)
                {
                    if (c is ICameraManager)
                    {
                        if (c.Name == name)
                        {
                            if (((ICameraManager) c).activeCamInternal == null)
                                throw new InvalidOperationException ("You cannot set a group with no active camera! " +
                                                                     "Have you forgot to add a camera?");
                            this.ActiveGroup.Disable ();
                            this.ActiveGroup = (ICameraManager) c;
                            this.ActiveGroup.Enable ();
                            break;
                        }
                        else
                        {
                            ((ICameraManager) c).SetGroup (name);
                            this.ActiveGroup = ((ICameraManager) c).GetActiveGroup ();
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set the active camera group.
        /// </summary>
        /// <param name="group">The group.</param>
        public void SetGroup (ICameraManager group)
        {
            if (group == null)
                throw new ArgumentNullException ("group", "The group must not be null!");
            SetGroup (group.Name);
        }

        /// <summary>
        /// Set ActiveCamera to the next camera in the collection
        /// </summary>
        public void Next ()
        {
            if (ActiveGroup.Name == this.Name)
            {
                int idx = Cameras.IndexOf (activeCamInternal) + 1;
                idx %= Count;
                ICameraManageable c = Cameras[idx];
                int i = 1;
                while (c is ICameraManager && i < Count)
                {
                    i++;
                    idx++;
                    idx %= Count;
                    c = Cameras[idx];
                }
                if (c is ICamera)
                    SetActive ((ICamera) c);
            }
            else
                ActiveGroup.Next ();
        }

        /// <summary>
        /// Set ActiveCamera to the previous camera in the collection
        /// </summary>
        public void Prev ()
        {
            if (ActiveGroup.Name == this.Name)
            {
                int idx = Cameras.IndexOf (activeCamInternal) - 1;
                idx = idx < 0 ? idx + Count : idx;
                ICameraManageable c = Cameras[idx];
                int i = 0;
                while (c is ICameraManager && i < Count)
                {
                    i++;
                    idx--;
                    idx = idx < 0 ? idx + Count : idx;
                    c = Cameras[idx];
                }
                if (c is ICamera)
                    SetActive ((ICamera) c);
            }
            else
                ActiveGroup.Prev ();
        }

        /// <summary>
        /// Get or set the currently active camera.
        /// </summary>
        /// <value>The camera.</value>
        public ICamera ActiveCamera
        {
            get
            {
                return GetActive ();
            }
            set
            {
                SetActive (value);
            }
        }

        /// <summary>
        /// Get or set the active camera group.
        /// </summary>
        /// <value>The active group.</value>
        public ICameraManager ActiveGroup { get; set; }

        /// <summary>
        /// Get the internal active camera (only use this if you are a camera manager).
        /// </summary>
        /// <value>The internal active camera.</value>
        public ICamera activeCamInternal { get; protected set; }

        #endregion

        #region IManager implementation

        /// <summary>
        /// Add the specified item.
        /// </summary>
        /// <param name="camera">Camera.</param>
        public void Add (ICameraManageable camera)
        {
            if (camera == null)
                throw new ArgumentNullException ("camera", "You cannot add a null camera!");
            if (activeCamInternal == null && camera is ICamera)
                activeCamInternal = (ICamera) camera;
            Cameras.Add (camera);
            Count++;
        }

        /// <summary>
        /// Remove the specified item.
        /// </summary>
        /// <param name="name">Name.</param>
        public void Remove (string name)
        {
            if (name == null)
                throw new ArgumentNullException ("name", "You cannot remove a camera with name null!");
            if (name == activeCamInternal.Name)
                throw new InvalidOperationException ("You cannot remove the currently active camera!");
            Cameras.RemoveAll (i => {
                if (i.Name == name)
                {
                    Count--;
                    return true;
                }
                return false;
            });
        }

        /// <summary>
        /// Remove the specified item.
        /// </summary>
        /// <param name="camera">Camera.</param>
        public void Remove (ICameraManageable camera)
        {
            if (camera == null)
                throw new ArgumentNullException ("camera", "You cannot remove a null camera!");
            Remove (camera.Name);
        }

        /// <summary>
        /// Gets the IManageable by name.
        /// </summary>
        /// <returns>The IManageable.</returns>
        /// <param name="name">Name.</param>
        public ICameraManageable GetByName (string name)
        {
            return Cameras.Find (c => c.Name == name);
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; protected set; }

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator GetEnumerator ()
        {
            return Cameras.GetEnumerator ();
        }

        #endregion

        #region ICameraManageable implementation

        /// <summary>
        /// This method is called when the camera manager switches to this subject.
        /// </summary>
        public void Enable ()
        {
            ActiveCamera.Enable ();
        }

        /// <summary>
        /// This method is called when the camera manager switches from this subject to another one.
        /// </summary>
        public void Disable ()
        {
            ActiveCamera.Disable ();
        }

        #endregion

        #region IResource implementation

        /// <summary>
        /// Init this resource. This method may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        public void Init ()
        {}

        /// <summary>
        /// Gets the init jobs.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs (List<Action> list)
        {
            foreach (var c in Cameras)
                list = c.GetInitJobs (list);
            return list;
        }

        /// <summary>
        /// Load this resource. This method *should* be called from an extra loading thread with a shared gl context.
        /// </summary>
        public void Load ()
        {}

        /// <summary>
        /// Gets the load jobs.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">Reloader.</param>
        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            foreach (var c in Cameras)
                list = c.GetLoadJobs (list, reloader);
            NeedsLoad = reloader;
            return list;
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
            Cameras.ForEach (c => c.Destroy ());
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Engine.Camera.CameraManager"/> is loaded.
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

        #region IFrameSyncedUpdate implementation

        /// <summary>
        /// This update is called before every frame draw inside a gl context.
        /// </summary>
        /// <param name="deltaTime">Time delta.</param>
        public void FrameSyncedUpdate (float deltaTime)
        {
            ActiveCamera.FrameSyncedUpdate (deltaTime);
        }

        #endregion

        #region IUpdate implementation

        /// <summary>
        /// This update is called in an extra thread which does not have a valid gl context.
        /// The updaterate might differ from the framerate.
        /// </summary>
        /// <param name="desc">Update description.</param>
        public void Update (UpdateDescription desc)
        {
            ActiveCamera.Update (desc);
        }

        #endregion

        #region IDrawable implementation

        /// <summary>
        /// Draw this instance.
        /// </summary>
        public void Draw ()
        {
            ActiveCamera.Draw ();
        }

        #endregion

        #region IManageable implementation

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion
    }
}

