//
//  FreezingArcherGwenRenderer.cs
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
using System.Drawing;
using System.IO;
using FreezingArcher.Renderer;
using FreezingArcher.Output;
using FreezingArcher.Math;
using System.Collections.Generic;

using FreezingArcher.Output;
using Pencil.Gaming.Graphics;

namespace Gwen.Renderer
{
    public class FreezingArcherGwenRenderer : Gwen.Renderer.Base
    {
        internal RendererContext PrivateRendererContext;

        private bool m_ClipEnabled;

        private bool m_WasBlendEnabled, m_WasTexture2DEnabled,
        m_WasDepthTestEnabled;

        private int m_PrevBlendSrc, m_PrevBlendDst, m_PrevAlphaFunc;
        private float m_PrevAlphaRef;
        private bool m_RestoredRenderState;

        private StringFormat m_StringFormat;
        private readonly Graphics m_Graphics;
        private readonly Dictionary<Tuple<String, Font>, TextRenderer> m_StringCache;

        public FreezingArcherGwenRenderer(RendererContext rc)
        {
            PrivateRendererContext = rc;

            m_StringCache = new Dictionary<Tuple<string, Font>, TextRenderer>();

            m_StringFormat = new StringFormat(StringFormat.GenericTypographic);
            m_StringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

            m_Graphics = Graphics.FromImage(new Bitmap(1024, 1024, System.Drawing.Imaging.PixelFormat.Format32bppArgb));

            m_ClipEnabled = false;
        }

        /// <summary>
        /// Starts rendering.
        /// </summary>
        public override void Begin()
        {
            m_PrevBlendSrc = PrivateRendererContext.GetBlendSrc();
            m_PrevBlendDst = PrivateRendererContext.GetBlendDst();

            m_WasBlendEnabled = PrivateRendererContext.IsEnabled(RendererEnableCap.Blend);
            m_WasDepthTestEnabled = PrivateRendererContext.IsEnabled(RendererEnableCap.DepthTest);

            PrivateRendererContext.SetBlendFunc(RendererBlendingFactorSrc.SrcAlpha, RendererBlendingFactorDest.OneMinusSrcAlpha);
        }

        /// <summary>
        /// Stops rendering.
        /// </summary>
        public override void End()
        {
            PrivateRendererContext.SetBlendFunc((RendererBlendingFactorSrc)m_PrevBlendSrc, (RendererBlendingFactorDest)m_PrevBlendDst);

            PrivateRendererContext.EnableDepthTest(m_WasDepthTestEnabled);

            if (!m_WasDepthTestEnabled)
                PrivateRendererContext.Disable(RendererEnableCap.DepthTest);

        }

        /// <summary>
        /// Gets or sets the current drawing color.
        /// </summary>
        public override Color DrawColor { get; set; }

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public override void DrawLine(int x, int y, int a, int b)
        {
            Translate(ref x, ref y);
            Translate(ref a, ref b);

            Vector2 posa = new Vector2((float)x, (float)y);
            Vector2 posb = new Vector2((float)a, (float)b);

            FreezingArcher.Math.Color4 col = new FreezingArcher.Math.Color4(DrawColor.R, DrawColor.G, DrawColor.B, DrawColor.A);

            PrivateRendererContext.DrawLineAbsolute(ref posa, ref posb, 1.0f, ref col);
        }

        /// <summary>
        /// Draws a solid filled rectangle.
        /// </summary>
        /// <param name="rect"></param>
        public override void DrawFilledRect(Rectangle input)
        {
            Rectangle rect = Translate(input);

            if (m_ClipEnabled)
            {
                if (rect.Y < ClipRegion.Y)
                {
                    int oldHeight = rect.Height;
                    int delta = ClipRegion.Y - rect.Y;
                    rect.Height -= delta;

                    if (rect.Height <= 0)
                        return;
                }
                    
                if ((rect.Y + rect.Height) > (ClipRegion.Y + ClipRegion.Height))
                {
                    int oldHeight = rect.Height;
                    int delta = (rect.Y + rect.Height) - (ClipRegion.Y + ClipRegion.Height);

                    rect.Height -= delta;

                    if (rect.Height <= 0)
                        return;
                }

                if (rect.X < ClipRegion.X)
                {
                    int oldWidth = rect.Width;
                    int delta = ClipRegion.X - rect.X;
                    rect.Width -= delta;

                    if (rect.Width <= 0)
                        return;
                }

                if ((rect.X + rect.Width) > (ClipRegion.X + ClipRegion.Width))
                {
                    int oldWidth = rect.Width;
                    int delta = (rect.X + rect.Width) - (ClipRegion.X + ClipRegion.Width);
                    rect.Width -= delta;

                    if (rect.Width <= 0)
                        return;
                }
            }

            Vector2 pos = new Vector2((float)rect.X, (float)rect.Y);
            Vector2 size = new Vector2((float)rect.Width, (float)rect.Height);

            FreezingArcher.Math.Color4 col = new FreezingArcher.Math.Color4(DrawColor.R, DrawColor.G, DrawColor.B, DrawColor.A);

                PrivateRendererContext.DrawFilledRectangleAbsolute(ref pos, ref size, ref col);
        }

        /// <summary>
        /// Starts clipping to the current clipping rectangle.
        /// </summary>
        public override void StartClip()
        {
            m_ClipEnabled = true;
        }

        /// <summary>
        /// Stops clipping.
        /// </summary>
        public override void EndClip()
        {
            m_ClipEnabled = false;
        }

        /// <summary>
        /// Loads the specified texture.
        /// </summary>
        /// <param name="t"></param>
        public override void LoadTexture(Gwen.Texture t)
        {
            Texture2D tex;

            try
            {
                tex = PrivateRendererContext.CreateTexture2D(t.Name + "_" + DateTime.Now.Ticks, true, t.Name);
            }catch(Exception e)
            {
                t.Failed = true;
                return;
            }

            t.Failed = false;

            t.RendererData = tex;
            t.Width = tex.Width;
            t.Height = tex.Height;
        }

        /// <summary>
        /// Initializes texture from raw pixel data.
        /// </summary>
        /// <param name="t">Texture to initialize. Dimensions need to be set.</param>
        /// <param name="pixelData">Pixel data in RGBA format.</param>
        public override void LoadTextureRaw(Gwen.Texture t, byte[] pixelData)
        {
            Texture2D tex;
            Bitmap bmp;

            try
            {
                unsafe
                {
                    fixed (byte* ptr = &pixelData[0])
                    {
                         bmp = new Bitmap(t.Width, t.Height, 4 * t.Width, 
                             System.Drawing.Imaging.PixelFormat.Format32bppArgb, (IntPtr)ptr);
                    }
                }

                tex = PrivateRendererContext.CreateTexture2D(t.Name + "_" + DateTime.Now.Ticks, true, bmp);
            }catch(Exception e)
            {
                t.Failed = true;
                return;
            }

            t.RendererData = tex;
            t.Width = tex.Width;
            t.Height = tex.Height;
        }

        /// <summary>
        /// Initializes texture from image file data.
        /// </summary>
        /// <param name="t">Texture to initialize.</param>
        /// <param name="data">Image file as stream.</param>
        public override void LoadTextureStream(Gwen.Texture t, Stream data)
        {
            Texture2D tex;
            Bitmap bmp;

            try
            {
                bmp = new Bitmap(data);
                tex = PrivateRendererContext.CreateTexture2D(t.Name + "_" + DateTime.Now.Ticks, true, bmp);
            }catch(Exception e)
            {
                t.Failed = true;
                return;
            }

            t.RendererData = tex;
        }

        /// <summary>
        /// Frees the specified texture.
        /// </summary>
        /// <param name="t">Texture to free.</param>
        public override void FreeTexture(Gwen.Texture t)
        {
            PrivateRendererContext.DeleteGraphicsResourceAsync((GraphicsResource)t.RendererData);
            t.RendererData = null;
        }

        /// <summary>
        /// Draws textured rectangle.
        /// </summary>
        /// <param name="t">Texture to use.</param>
        /// <param name="targetRect">Rectangle bounds.</param>
        /// <param name="u1">Texture coordinate u1.</param>
        /// <param name="v1">Texture coordinate v1.</param>
        /// <param name="u2">Texture coordinate u2.</param>
        /// <param name="v2">Texture coordinate v2.</param>
        public override void DrawTexturedRect(Gwen.Texture t, Rectangle targetRect, float u1 = 0, float v1 = 0, float u2 = 1, float v2 = 1)
        {
            if (t.RendererData == null)
            {
                DrawMissingImage(targetRect);
                return;
            }

            Rectangle rect = Translate(targetRect);

            if (m_ClipEnabled)
            {
                // cpu scissors test

                if (rect.Y < ClipRegion.Y)
                {
                    int oldHeight = rect.Height;
                    int delta = ClipRegion.Y - rect.Y;
                    rect.Y = ClipRegion.Y;
                    rect.Height -= delta;

                    if (rect.Height <= 0)
                    {
                        return;
                    }

                    float dv = (float)delta / (float)oldHeight;

                    v1 += dv * (v2 - v1);
                }

                if ((rect.Y + rect.Height) > (ClipRegion.Y + ClipRegion.Height))
                {
                    int oldHeight = rect.Height;
                    int delta = (rect.Y + rect.Height) - (ClipRegion.Y + ClipRegion.Height);

                    rect.Height -= delta;

                    if (rect.Height <= 0)
                    {
                        return;
                    }

                    float dv = (float)delta / (float)oldHeight;

                    v2 -= dv * (v2 - v1);
                }

                if (rect.X < ClipRegion.X)
                {
                    int oldWidth = rect.Width;
                    int delta = ClipRegion.X - rect.X;
                    rect.X = ClipRegion.X;
                    rect.Width -= delta;

                    if (rect.Width <= 0)
                    {
                        return;
                    }

                    float du = (float)delta / (float)oldWidth;

                    u1 += du * (u2 - u1);
                }

                if ((rect.X + rect.Width) > (ClipRegion.X + ClipRegion.Width))
                {
                    int oldWidth = rect.Width;
                    int delta = (rect.X + rect.Width) - (ClipRegion.X + ClipRegion.Width);

                    rect.Width -= delta;

                    if (rect.Width <= 0)
                    {
                        return;
                    }

                    float du = (float)delta / (float)oldWidth;

                    u2 -= du * (u2 - u1);
                }
            }
                
            Vector2 pos = new Vector2(rect.X, rect.Y);
            Vector2 size = new Vector2(rect.Width, rect.Height);

            Vector2[] texcoords = new Vector2[6];
            texcoords[0] = new Vector2(u1, v1);
            texcoords[1] = new Vector2(u2, v1);
            texcoords[2] = new Vector2(u1, v2);
            texcoords[3] = new Vector2(u1, v2);
            texcoords[4] = new Vector2(u2, v1);
            texcoords[5] = new Vector2(u2, v2);

             PrivateRendererContext.DrawTexturedRectangleAbsolute((Texture2D)t.RendererData, ref pos, ref size, texcoords, no_blend:true);
        }

        /// <summary>
        /// Draws "missing image" default texture.
        /// </summary>
        /// <param name="rect">Target rectangle.</param>
        public override void DrawMissingImage(Rectangle rect)
        {
            //DrawColor = Color.FromArgb(255, rnd.Next(0,255), rnd.Next(0,255), rnd.Next(0, 255));
            DrawColor = Color.Pink;
            DrawFilledRect(rect);
        }

        /// <summary>
        /// Cache to texture provider.
        /// </summary>
        public virtual Gwen.Renderer.ICacheToTexture CTT { get { return null; } }

        /// <summary>
        /// Loads the specified font.
        /// </summary>
        /// <param name="font">Font to load.</param>
        /// <returns>True if succeeded.</returns>
        public override bool LoadFont(Gwen.Font font)
        {
            font.RealSize = font.Size * Scale;
            System.Drawing.Font sysFont = font.RendererData as System.Drawing.Font;

            if (sysFont != null)
                sysFont.Dispose();

            sysFont = new System.Drawing.Font(font.FaceName, font.Size);
            font.RendererData = sysFont;
            
            return true;
        }

        /// <summary>
        /// Frees the specified font.
        /// </summary>
        /// <param name="font">Font to free.</param>
        public override void FreeFont(Gwen.Font font)
        {
            if (font.RendererData == null)
                return;

            System.Drawing.Font sysFont = font.RendererData as System.Drawing.Font;
            if (sysFont == null)
                Logger.Log.AddLogEntry(LogLevel.Crash, "FreezingArcherGwenRenderer",
                    FreezingArcher.Core.Status.BadData);

            sysFont.Dispose();
            font.RendererData = null;
        }

        /// <summary>
        /// Returns dimensions of the text using specified font.
        /// </summary>
        /// <param name="font">Font to use.</param>
        /// <param name="text">Text to measure.</param>
        /// <returns>Width and height of the rendered text.</returns>
        public override Point MeasureText(Gwen.Font font, string text)
        {
            System.Drawing.Font sysFont = font.RendererData as System.Drawing.Font;

            if (sysFont == null || Math.Abs(font.RealSize - font.Size * Scale) > 2)
            {
                FreeFont(font);
                LoadFont(font);
                sysFont = font.RendererData as System.Drawing.Font;
            }

            var key = new Tuple<String, Font>(text, font);

            if (m_StringCache.ContainsKey(key))
            {
                var tex = m_StringCache[key].Texture;
                return new Point(tex.Width, tex.Height);
            }

            SizeF TabSize = m_Graphics.MeasureString("....", sysFont);

            m_StringFormat.SetTabStops(0f, new float[] { TabSize.Width });
            SizeF size = m_Graphics.MeasureString(text, sysFont, Point.Empty, m_StringFormat);

            return new Point((int)Math.Round(size.Width+5), (int)Math.Round(size.Height+5));
        }

        /// <summary>
        /// Renders text using specified font.
        /// </summary>
        /// <param name="font">Font to use.</param>
        /// <param name="position">Top-left corner of the text.</param>
        /// <param name="text">Text to render.</param>
        public override void RenderText(Gwen.Font font, Point position, string text)
        {
            System.Drawing.Font sysFont = font.RendererData as System.Drawing.Font;

            if (sysFont == null || Math.Abs(font.RealSize - font.Size * Scale) > 2)
            {
                FreeFont(font);
                LoadFont(font);
                sysFont = font.RendererData as System.Drawing.Font;
            }

            var key = new Tuple<String, Font>(text, font);

            if (!m_StringCache.ContainsKey(key))
            {
                Point size = MeasureText(font, text);
                TextRenderer tr = new TextRenderer(size.X, size.Y, this);
                tr.DrawString(text, sysFont, new SolidBrush(DrawColor), Point.Empty, m_StringFormat);

                DrawTexturedRect(tr.Texture, new Rectangle(position.X, position.Y, tr.Texture.Width, tr.Texture.Height));

                m_StringCache[key] = tr;
            }
            else
            {
                TextRenderer tr = m_StringCache[key];

                PrivateRendererContext.SetBlendFunc(RendererBlendingFactorSrc.DstAlpha, RendererBlendingFactorDest.One);

                DrawTexturedRect(tr.Texture, new Rectangle(position.X, position.Y, tr.Texture.Width, tr.Texture.Height));

                PrivateRendererContext.SetBlendFunc(RendererBlendingFactorSrc.SrcAlpha, RendererBlendingFactorDest.OneMinusSrcAlpha);
            }
        }

        public override Color PixelColor(Texture texture, uint x, uint y)
        {
            return PixelColor(texture, x, y, Color.Yellow);
        }

        /// <summary>
        /// Gets pixel color of a specified texture, returning default if otherwise failed. Slow.
        /// </summary>
        /// <param name="texture">Texture.</param>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <param name="defaultColor">Color to return on failure.</param>
        /// <returns>Pixel color.</returns>
        public override Color PixelColor(Texture texture, uint x, uint y, Color defaultColor)
        {
            Texture2D tex = texture.RendererData as Texture2D;

            if (tex != null)
            {
                FreezingArcher.Math.Color4 col = tex.GetPixelColor((int)x, (int)y);

                return Color.FromArgb((int)(col.A*255), (int)(col.R*255), (int)(col.G*255), (int)(col.B*255));
            }

            return defaultColor;
        }
    }
}

