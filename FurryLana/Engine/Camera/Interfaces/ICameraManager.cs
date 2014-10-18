//
//  ICameraManager.cs
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
using System.Collections.Generic;
using FurryLana.Engine.Graphics.Interfaces;

namespace FurryLana.Engine.Camera.Interfaces
{
    public interface ICameraManager : IGraphicsResource, ICameraManageable
    {
        /// <summary>
        /// Get the active camera
        /// </summary>
        /// <returns>The camera.</returns>
        ICamera GetActive ();
            
        /// <summary>
        /// Get the active camera group.
        /// </summary>
        /// <returns>The active group.</returns>
        ICameraManager GetActiveGroup ();
            
        /// <summary>
        /// Set the active camera.
        /// </summary>
        /// <param name="name">The camera name.</param>
        void SetActive (string name);
            
        /// <summary>
        /// Set the active camera in the currently active group. If the camera is not available it will not be changed.
        /// </summary>
        /// <param name="camera">The camera.</param>
        void SetActive (ICamera camera);
            
        /// <summary>
        /// Set the active camera and set a new group if necessary.
        /// </summary>
        /// <param name="name">The camera name.</param>
        void SetActiveRecursive (string name);
            
        /// <summary>
        /// Set the active camera and set a new group if necessary.
        /// </summary>
        /// <param name="camera">The camera.</param>
        void SetActiveRecursive (ICamera camera);
            
        /// <summary>
        /// Set the active camera group.
        /// </summary>
        /// <param name="group">The group.</param>
        void SetGroup (ICameraManager group);
            
        /// <summary>
        /// Set the active camera group.
        /// </summary>
        /// <param name="name">The camera groups name.</param>
        void SetGroup (string name);
            
        /// <summary>
        /// Set ActiveCamera to the next camera in the collection
        /// </summary>
        void Next ();
            
        /// <summary>
        /// Set ActiveCamera to the previous camera in the collection
        /// </summary>
        void Prev ();
            
        /// <summary>
        /// Add a camera to the manager.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="camera">The camera.</param>
        void Add (ICameraManageable camera);
            
        /// <summary>
        /// Remove camera by name.
        /// </summary>
        /// <param name="name">The name.</param>
        void Remove (string name);
            
        /// <summary>
        /// Remove the specified camera.
        /// </summary>
        /// <param name="camera">The camera.</param>
        void Remove (ICameraManageable camera);
            
        /// <summary>
        /// Get the count of cameras in this manager
        /// </summary>
        /// <value>The count.</value>
        int Count { get; }
            
        /// <summary>
        /// Get or set the currently active camera.
        /// </summary>
        /// <value>The camera.</value>
        ICamera ActiveCamera { get; set; }
            
        /// <summary>
        /// Get or set the active camera group.
        /// </summary>
        /// <value>The active group.</value>
        ICameraManager ActiveGroup { get; set; }
            
        /// <summary>
        /// Get a list of manageable objects inside the camera manager
        /// </summary>
        /// <value>The items.</value>
        List<ICameraManageable> Items { get; }
            
        /// <summary>
        /// Get the internal active camera (only use this if you are a camera manager).
        /// </summary>
        /// <value>The internal active camera.</value>
        ICamera activeCamInternal { get; }
    }
}
