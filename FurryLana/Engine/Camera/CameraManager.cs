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
    public class CameraManager : ICameraManager
    {
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

        protected List<ICameraManageable> Cameras;

        #region ICameraManager implementation

        public ICamera GetActive ()
        {
            if (ActiveGroup.Name == this.Name)
                return activeCamInternal;
            else
                return ActiveGroup.GetActive ();
        }

        public ICameraManager GetActiveGroup ()
        {
            return ActiveGroup;
        }

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

        public void SetActive (ICamera camera)
        {
            if (camera == null)
                throw new ArgumentNullException ("camera", "The active camera must not be null!");
            
            SetActive (camera.Name);
        }

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

        public void SetActiveRecursive (ICamera camera)
        {
            if (camera == null)
                throw new ArgumentNullException ("camera", "The active camera must not be null!");
            
            SetActiveRecursive (camera.Name);
        }

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

        public void SetGroup (ICameraManager group)
        {
            if (group == null)
                throw new ArgumentNullException ("group", "The group must not be null!");
            SetGroup (group.Name);
        }

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

        public ICameraManager ActiveGroup { get; set; }

        public ICamera activeCamInternal { get; protected set; }

        #endregion

        #region IManager implementation

        public void Add (ICameraManageable camera)
        {
            if (camera == null)
                throw new ArgumentNullException ("camera", "You cannot add a null camera!");
            if (activeCamInternal == null && camera is ICamera)
                activeCamInternal = (ICamera) camera;
            Cameras.Add (camera);
            Count++;
        }

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

        public void Remove (ICameraManageable camera)
        {
            if (camera == null)
                throw new ArgumentNullException ("camera", "You cannot remove a null camera!");
            Remove (camera.Name);
        }

        public ICameraManageable GetByName (string name)
        {
            return Cameras.Find (c => c.Name == name);
        }

        public int Count { get; protected set; }

        #endregion

        #region IEnumerable implementation

        public IEnumerator GetEnumerator ()
        {
            return Cameras.GetEnumerator ();
        }

        #endregion

        #region ICameraManageable implementation

        public void Enable ()
        {
            ActiveCamera.Enable ();
        }

        public void Disable ()
        {
            ActiveCamera.Disable ();
        }

        #endregion

        #region IResource implementation

        public void Init ()
        {}

        public List<Action> GetInitJobs (List<Action> list)
        {
            foreach (var c in Cameras)
                list = c.GetInitJobs (list);
            return list;
        }

        public void Load ()
        {}

        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            foreach (var c in Cameras)
                list = c.GetLoadJobs (list, reloader);
            NeedsLoad = reloader;
            return list;
        }

        public void Destroy ()
        {
            Cameras.ForEach (c => c.Destroy ());
        }

        public bool Loaded { get; protected set; }

        public EventHandler NeedsLoad { get; set; }

        #endregion

        #region IFrameSyncedUpdate implementation

        public void FrameSyncedUpdate (float deltaTime)
        {
            ActiveCamera.FrameSyncedUpdate (deltaTime);
        }

        #endregion

        #region IUpdate implementation

        public void Update (UpdateDescription desc)
        {
            ActiveCamera.Update (desc);
        }

        #endregion

        #region IDrawable implementation

        public void Draw ()
        {
            ActiveCamera.Draw ();
        }

        #endregion

        #region IManageable implementation

        public string Name { get; set; }

        #endregion
    }
}

