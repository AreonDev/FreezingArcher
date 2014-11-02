/*
 *  XWindowDefs.h
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

#include "XWindowEnums.h"

typedef int Monitor; // monitor identifier
typedef int Window;  // window identifier
typedef int Unicode; // unicode character

/*
 * Description:
 *   This function is called when an error occures
 *
 * Return:
 *   void
 *
 * Arguments:
 *   ErrorCode  error code
 */
typedef void (*ErrorFun) (ErrorCode);

/*
 * Description:
 *   This function is called when the monitor of the window changes
 *
 * Return:
 *   void
 *
 * Arguments:
 *   Monitor          identifier for the monitor
 *   ConnectionState  describes the connection state of the specified monitor
 */
typedef void (*MonitorFun) (Monitor, ConnectionState);

/*
 * Description:
 *   This function is called when the window gets closed
 *
 * Return:
 *   void
 *
 * Arguments:
 *   Window  identifier for the window
 */
typedef void (*WindowCloseFun) (Window);

/*
 * Description:
 *   This function ist called when the window position changes
 *
 * Return:
 *   void
 *
 * Arguments:
 *   Window  identifier for the window
 *   int     x position of the window
 *   int     y position of the window
 */
typedef void (*WindowPosFun) (Window, int, int);

/* Description:
 *   This function is called when the window gets a refresh from the window
 *   manager
 *
 * Return:
 *   void
 *
 * Arguments:
 *   Window  identifier for the window
 */
typedef void (*WindowRefreshFun) (Window);

/*
 * Description:
 *   This function is called when the window is resized
 *
 * Return:
 *   void
 *
 * Arguments:
 *   Window  identifier for the window
 *   int     width of the window
 *   int     height of the window
 */
typedef void (*WindowSizeFun) (Window, int, int);

/*
 * Description:
 *   This function is called when the focus state of the window changes
 *
 * Return:
 *   void
 *
 * Arguments:
 *   Window  identifier for the window
 *   bool    indicates current focus of the window
 */
typedef void (*WindowFocusFun) (Window, bool);

/*
 * Description:
 *   This function is called when a key or combination is pressed, released or
 *   repeated
 *
 * Return:
 *   void
 *
 * Arguments:
 *   Window      identifier for the window
 *   Unicode     describes the key as unicode character (int)
 *   int         scancode of the key
 *   KeyAction   describes the current action on this key
 *   KeyModifier describes the current active modifier on this key
 */
typedef void (*KeyFun) (Window, Unicode, int, KeyAction, KeyModifier);

/*
 * Description:
 *   This function is called when a mouse button is pressed, released or
 *   repeated
 *
 * Return:
 *   void
 *
 * Arguments:
 *   Window      identifier for the window
 *   MouseButton describes the active mouse button
 *   KeyAction   describes the current active action on the button
 */
typedef void (*MouseButtonFun) (Window, MouseButton, KeyAction);

/*
 * Description:
 *   This function is called when the window gets minimized
 *
 * Return:
 *   void
 *
 * Arguments:
 *   Window  identifier for the window
 *   bool    describes the minimize state
 */
typedef void (*WindowMinimizeFun) (Window, bool);

/*
 * Description:
 *   This function is called when the cursor/pointer position changes
 *
 * Return:
 *   void
 *
 * Arguments:
 *   Window  identifier for the window
 *   double  x position of the cursor
 *   double  y position of the cursor
 */
typedef void (*CursorPosFun) (Window, double, double);

/*
 * Description:
 *   This function is called when the cursor/pointer enters the window
 *
 * Return:
 *   void
 *
 * Arguments:
 *   Window  identifier for the window
 *   bool    describes if the cursor is inside the window or not
 */
typedef void (*CursorEnterFun) (Window, bool);

/*
 * Description:
 *   This function is called when the user scrolls with his mouse
 *
 * Return:
 *   void
 *
 * Arguments:
 *   Window  identifier for the window
 *   double  x offset
 *   double  y offset
 */
typedef void (*ScrollFun) (Window, double, double);

/*
 * Description:
 *   This function is called when the Load function of this object needs to be
 *   recalled
 *
 * Return:
 *   void
 *
 * Arguments:
 *   void
 */
typedef void (*Event) (void);

/*
 * Size structure
 */
struct Size
{
    int X;
    int Y;
};
