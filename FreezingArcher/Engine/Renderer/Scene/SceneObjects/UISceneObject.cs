//
//  UISceneObject.cs
//
//  Author:
//       dboeg <${AuthorEmail}>
//
//  Copyright (c) 2015 dboeg
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

using FreezingArcher.Renderer;

namespace FreezingArcher.Renderer.Scene.SceneObjects
{
    public class UISceneObject : SceneObject
    {
        private RendererContext PrivateRendererContext;

        public Gwen.Renderer.Base Renderer { get; private set;}
        public Gwen.Skin.TexturedBase Skin { get; private set;}
        public Gwen.Control.Canvas Canvas{ get; private set;}

        public override bool Init(RendererContext rc)
        {
            if (!IsInitialized)
            {
                Renderer = new Gwen.Renderer.FreezingArcherGwenRenderer(rc);
                Skin = new Gwen.Skin.TexturedBase(Renderer, "lib/UI/Skins/FreezingArcherSkin.png");
                Canvas = new Gwen.Control.Canvas(Skin);

                PrivateRendererContext = rc;

                IsInitialized = true;
            }

            return true;
        }

        public UISceneObject()
        {
            IsInitialized = false;
        }

        #region implemented abstract members of SceneObject

        public override void Draw(RendererContext rc)
        {
            if(IsInitialized)
                Canvas.RenderCanvas();
        }

        public override void DrawInstanced(RendererContext rc, int count)
        {
            //Do nothing.... How should i draw it instanced??
        }

        public override string GetName()
        {
            return "UISceneObject";
        }

        public override SceneObject Clone()
        {
            UISceneObject uiobj = new UISceneObject();
            uiobj.Renderer = new Gwen.Renderer.FreezingArcherGwenRenderer(PrivateRendererContext);
            uiobj.PrivateRendererContext = this.PrivateRendererContext;
            uiobj.Skin = new Gwen.Skin.TexturedBase(uiobj.Renderer, "DefaultSkin.jpg");
            uiobj.Canvas = new Gwen.Control.Canvas(Skin);

            uiobj.IsInitialized = true;

            return uiobj;
        }

        #endregion
    }
}

