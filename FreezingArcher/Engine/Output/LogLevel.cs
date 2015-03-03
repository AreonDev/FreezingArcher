//
//  LogLevel.cs
//
//  Author:
//       Martin Koppehel <martin.koppehel@st.ovgu.de>
//
//  Copyright (c) 2015 Martin Koppehel
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

namespace FreezingArcher.Output
{
    /// <summary>
    /// LogLevel enumeration
    /// </summary>
    public enum LogLevel : byte
    {
        /// <summary>Messages for debug usage</summary>
        Debug = 1,
        /// <summary>Totally unneccessary status information</summary>
        Fine = 2,
        /// <summary>Standard log level</summary>
        Info = 3,
        /// <summary>just a warning, will not cause any crashes</summary>
        Warning = 4,
        /// <summary>error which may cause a crash</summary>
        Error = 5,
        /// <summary>severe error which will likely cause a crash</summary>
        Severe = 6,
        /// <summary>fatal error may be logged immediately before a crash</summary>
        Fatal = 7,
        /// <summary>there was an error which the program couldn't handle and the game crashed</summary>
        Crash = 8,
    }
}
