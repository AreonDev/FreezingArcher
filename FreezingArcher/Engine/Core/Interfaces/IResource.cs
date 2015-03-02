//
//  IResource.cs
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

namespace FreezingArcher.Core.Interfaces
{
    // TODO
    // statt Action eine Struktur mit unmananged function ptr (schneller)
    // Init rauswerfen
    // per Interface zwischen Resource und GLResource unterscheiden (nur noch eine Load Methode je interface)

    delegate void Handler ();

    /// <summary>
    /// Resource interface.
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// Gets the init jobs. The init jobs may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        /// <returns>The init jobs.</returns>
        List<Action> GetInitJobs (List<Action> list);

        /// <summary>
        /// Gets the load jobs. The load jobs will be executed sequentially in the gl thread.
        /// </summary>
        /// <returns>The load jobs.</returns>
        List<Action> GetLoadJobs (List<Action> list, Handler reloader);
        
        /// <summary>
        /// Destroy this resource.
        /// 
        /// Why not IDisposable:
        /// IDisposable is called from within the garbage collector context so we do not have a valid gl context there.
        /// Therefore I added the Destroy function as this would be called by the parent instance within a valid gl
        /// context.
        /// </summary>
        void Destroy ();
        
        /// <summary>
        /// Gets a value indicating whether this
        /// <see cref="FreezingArcher.Core.Interfaces.IResource"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        bool Loaded { get; }

        /// <summary>
        /// Fire this event when you need the binded load function to be called.
        /// For example after init or when new resources needs to be loaded.
        /// </summary>
        event Handler NeedsLoad;
    }
}
