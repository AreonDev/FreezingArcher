/*
 *  XWindowEnums.h
 *
 *  Author:
 *       Fin Christensen <christensen.fin@gmail.com>
 *
 *  Copyright (c) 2014 Fin Christensen
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
 */

/*
 * Define a boolean type
 */
enum bool
{
    true = 1,
    false = 0
};

/*
 * Indentifiers for error codes
 */
enum ErrorCode
{
    NoError,
    NotInitialized,
    NoCurrentContext,
    InvalidEnum,
    InvalidValue,
    OutOfMemory,
    APIUnavailable,
    VersionUnavailable,
    PlatformError,
    FormatUnavailable
};

/*
 * Identifiers for connection states
 */
enum ConnectionState
{
    Connected,
    Disconnected
};

/*
 * Identifiers for actions on keys or buttons
 */
enum KeyAction
{
    Release,
    Press,
    Repeat
};

/*
 * Identifiers for key modifiers
 */
enum KeyModifier
{
    Shift,
    Control,
    Alt,
    Super
};

/*
 * Identifiers for mouse buttons
 */
enum MouseButton
{
    Button1 = 0,
    Button2,
    Button3,
    Button4,
    Button5,
    Button6,
    Button7,
    Button8,
    LeftButton = 0,
    RightButton,
    MiddleButton
};
