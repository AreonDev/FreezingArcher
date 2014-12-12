//
//  IWindow.cs
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
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using FurryLana.Engine.Graphics.Interfaces;

namespace FurryLana.Engine.Application.Interfaces
{
    /// <summary>
    /// Window interface.
    /// </summary>
    public interface IWindow : IResource
    {
        /// <summary>
        /// Gets or sets the size of the window in windowed mode.
        /// </summary>
        /// <value>The size.</value>
        Vector2i             WindowedSize   { get; set; }

        /// <summary>
        /// Gets or sets resolution in fullscreen mode.
        /// </summary>
        /// <value>The resolution.</value>
        //Vector2i             FullscreenSize { get; set; }

        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        /// <value>The title.</value>
        string               Title          { get; set; }

        /// <summary>
        /// Gets or sets the binded resource.
        /// </summary>
        /// <value>The resource.</value>
        IGraphicsResource    Resource       { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="FurryLana.Base.Application.Interfaces.IWindow"/> is fullscreen.
        /// </summary>
        /// <value><c>true</c> if fullscreen; otherwise, <c>false</c>.</value>
        bool                 Fullscreen     { get; set; }

        /// <summary>
        /// Gets or sets the mouse move handlers.
        /// </summary>
        /// <value>The handlers.</value>
        GlfwCursorPosFun     MouseMove      { get; set; }

        /// <summary>
        /// Gets or sets the mouse over handlers.
        /// </summary>
        /// <value>The handlers.</value>
        GlfwCursorEnterFun   MouseOver      { get; set; }

        /// <summary>
        /// Gets or sets the mouse button handlers.
        /// </summary>
        /// <value>The handlers.</value>
        GlfwMouseButtonFun   MouseButton    { get; set; }

        /// <summary>
        /// Gets or sets the mouse scroll handlers.
        /// </summary>
        /// <value>The handlers.</value>
        GlfwScrollFun        MouseScroll    { get; set; }

        /// <summary>
        /// Gets or sets the key action handlers.
        /// </summary>
        /// <value>The handlers.</value>
        GlfwKeyFun           KeyAction      { get; set; }

        /// <summary>
        /// Gets or sets the window close handlers.
        /// </summary>
        /// <value>The handlers.</value>
        GlfwWindowCloseFun   WindowClose    { get; set; }

        /// <summary>
        /// Gets or sets the window focus handlers.
        /// </summary>
        /// <value>The handlers.</value>
        GlfwWindowFocusFun   WindowFocus    { get; set; }

        /// <summary>
        /// Gets or sets the window minimize handlers.
        /// </summary>
        /// <value>The handlers.</value>
        GlfwWindowIconifyFun WindowMinimize { get; set; }

        /// <summary>
        /// Gets or sets the window move handlers.
        /// </summary>
        /// <value>The handlers.</value>
        GlfwWindowPosFun     WindowMove     { get; set; }

        /// <summary>
        /// Gets or sets the window resize handlers.
        /// </summary>
        /// <value>The handlers.</value>
        GlfwWindowSizeFun    WindowResize   { get; set; }

        /// <summary>
        /// Gets or sets the window error handlers.
        /// </summary>
        /// <value>The handlers.</value>
        GlfwErrorFun         WindowError    { get; set; }

        /// <summary>
        /// Gets or sets the framebuffer size handler.
        /// </summary>
        /// <value>The framebuffer size handler.</value>
        GlfwFramebufferSizeFun FramebufferSize { get; set; }

        /// <summary>
        /// Toggles the fullscreen.
        /// </summary>
        void ToggleFullscreen ();

        /// <summary>
        /// Show this window.
        /// </summary>
        void Show ();

        /// <summary>
        /// Hide this window.
        /// </summary>
        void Hide ();

        /// <summary>
        /// Minimize this window.
        /// </summary>
        void Minimize ();

        /// <summary>
        /// Restore this window (unminimize).
        /// </summary>
        void Restore ();

        /// <summary>
        /// Run this instance.
        /// </summary>
        void Run ();
    }
}
