/*
 *  XWindow.h
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

#include "XWindowTypes.h"

/*
 * Get or set the size of the window in windowed mode
 */
Size* GetWindowedSize ();
void SetWindowedSize (Size*);

/*
 * Get or set the resolution in fullscreen mode
 */
Size* GetFullscreenSize ();
void SetFullscreenSize (Size*);

/*
 * Get or set the window title
 */
char* GetTitle ();
void SetTitle (char*);

/*
 * Get or set a value indication whether this window is in fullscreen mode
 */
bool GetFullscreen ();
void SetFullscreen (bool);

/*
 * Get or set the error function
 */
ErrorFun GetErrorFun ();
void SetErrorFun (ErrorFun);

/*
 * Get or set the monitor function
 */
MonitorFun GetMonitorFun ();
void SetMonitorFun (MonitorFun);

/*
 * Get or set the window close function
 */
WindowCloseFun GetWindowCloseFun ();
void SetWindowCloseFun (WindowCloseFun);

/*
 * Get or set the window position function
 */
WindowPosFun GetWindowPosFun ();
void SetWindowPosFun (WindowPosFun);

/*
 * Get or set the window refresh function
 */
WindowRefreshFun GetWindowRefreshFun ();
void SetWindowRefreshFun (WindowRefreshFun);

/*
 * Get or set the window size function
 */
WindowSizeFun GetWindowSizeFun ();
void SetWindowSizeFun (WindowSizeFun);

/*
 * Get or set the window focus function
 */
WindowFocusFun GetWindowFocusFun ();
void SetWindowFocusFun (WindowFocusFun);

/*
 * Get or set the key function
 */
KeyFun GetKeyFun ();
void SetKeyFun (KeyFun);

/*
 * Get or set the mouse button function
 */
MouseButtonFun GetMouseButtonFun ();
void SetMouseButtonFun (MouseButtonFun);

/*
 * Get or set the window minimize function
 */
WindowMinimizeFun GetWindowMinimizeFun ();
void SetWindowMinimizeFun (WindowMinimizeFun);

/*
 * Get or set the cursor position function
 */
CursorPosFun GetCursorPosFun ();
void SetCursorPosFun (CursorPosFun);

/*
 * Get or set the cursor enter function
 */
CursorEnterFun GetCursorEnterFun ();
void SetCursorEnterFun (CursorEnterFun);

/*
 * Get or set the scroll function
 */
ScrollFun GetScrollFun ();
void SetScrollFun (ScrollFun);

/*
 * Toggle between windowed and fullscreen mode
 */
void ToggleFullscreen ();

/*
 * Show this window
 */
void Show ();

/*
 * Hide this window
 */
void Hide ();

/*
 * Minimize this window
 */
void Minimize ();

/*
 * Restore the window if it was in minimized state
 */
void Restore ();

/*
 * Initialize this object
 */
void Init ();

/*
 * Load this object
 */
void Load ();

/*
 * Destroy this object. This should close the window and delete everything
 * associated with it
 */
void Destroy ();

/*
 * Get or set a bool indicating if this object is loaded or not
 */
bool GetLoaded ();
void SetLoaded (bool);

/*
 * Variable storing a function which should be called from within this object
 * is it needs to be reloaded
 */
Event NeedsLoad;

/*
 * Set the NeedsLoad function
 */
void SetNeedsLoad (Event);
