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

namespace FurryLana.Base.Application.IApplication
{
    public interface IWindow
    {
        Vector2i             WindowedSize   { get; set; }
        Vector2i             FullscreenSize { get; set; }
        string               Title          { get; set; }
        IGraphicsResource    Resource       { get; set; }
        bool                 Fullscreen     { get; set; }
        GlfwCursorPosFun     MouseMove      { get; set; }
        GlfwCursorEnterFun   MouseOver      { get; set; }
        GlfwMouseButtonFun   MouseButton    { get; set; }
        GlfwScrollFun        MouseScroll    { get; set; }
        GlfwKeyFun           KeyAction      { get; set; }
        GlfwWindowCloseFun   WindowClose    { get; set; }
        GlfwWindowFocusFun   WindowFocus    { get; set; }
        GlfwWindowIconifyFun WindowMinimize { get; set; }
        GlfwWindowPosFun     WindowMove     { get; set; }
        GlfwWindowSizeFun    WindowResize   { get; set; }
        GlfwErrorFun         WindowError    { get; set; }
        
        void ToggleFullscreen ();
        void Show ();
        void Hide ();
        void Minimize ();
        void Restore ();
        void Run ();
    }
}
