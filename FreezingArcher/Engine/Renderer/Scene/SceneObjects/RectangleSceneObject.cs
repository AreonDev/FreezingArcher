//
//  RectangleSceneObject.cs
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
using FreezingArcher.Math;

namespace FreezingArcher.Renderer.Scene.SceneObjects
{
    public class RectangleSceneObject : SceneObject
    {
        private Color4 m_Color;

        public Color4 Color
        { 
            get
            {
                return m_Color;
            }

            set
            {
                m_Color = value;
                HasChanged = true;
            }
        }

        public float LineWidth{ get; private set;}
        public bool Filled{ get; private set;}

        public RectangleSceneObject(float lineWidth, bool filled) : base()
        {
            LineWidth = lineWidth;
            Filled = filled;
        }

        public RectangleSceneObject()
        {
            LineWidth = 1;
            Filled = true;
        }

        public override void Draw(RendererContext rc)
        {
            Vector2 pos = Position.Xy;
            Vector2 size = Scaling.Xy;
            Color4 col = Color;

            if (!Filled)
                rc.DrawRectangleAbsolute(ref pos, ref size, LineWidth, ref col);
            else
                rc.DrawFilledRectangleAbsolute(ref pos, ref size, ref col, 1);
        }

        public override SceneObjectArrayInstanceData GetData()
        {
            SceneObjectArrayInstanceData data = base.GetData();
            data.Other1 = new Vector4(Color.R, Color.G, Color.B, Color.A);

            return data;
        }

        public override void DrawInstanced(RendererContext rc, int count)
        {
            Vector2 pos = Vector2.Zero;
            Vector2 size = new Vector2(1, 1);
            Color4 col = new Color4(0, 0, 0, 0);

            if (!Filled)
                rc.DrawRectangleAbsolute(ref pos, ref size, LineWidth, ref col);
            else
                rc.DrawFilledRectangleAbsolute(ref pos, ref size, ref col, count);
        }

        public override void PrepareInstanced(RendererContext rc, VertexBufferLayoutKind[] vblks, VertexBuffer vb)
        {
            rc._2DVertexBufferArray.BindVertexBufferArray();

            vb.BindBuffer();

            for (int i = 0; i < vblks.Length; i++)
            {
                rc.EnableVertexAttribute((int)vblks[i].AttributeID);
                rc.VertexAttributePointer(vblks[i]);
                rc.VertexAttributeDivisor((int)vblks[i].AttributeID, 1);
            }

            vb.UnbindBuffer();

            rc._2DVertexBufferArray.UnbindVertexBufferArray();
        }

        public override void UnPrepareInstanced(RendererContext rc, VertexBufferLayoutKind[] vblks)
        {
            rc._2DVertexBufferArray.BindVertexBufferArray();

            for (int i = 0; i < vblks.Length; i++)
                rc.DisableVertexAttribute((int)vblks[i].AttributeID);

            rc._2DVertexBufferArray.UnbindVertexBufferArray();
        }

        public override string GetName() {return "RectangleSceneObject_"+
            (Filled?"Filled":("Wired_"+LineWidth));}
    }
}

