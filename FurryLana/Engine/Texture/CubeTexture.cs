//
//  Texture.cs
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
using System.Drawing;
using FurryLana.Engine.Texture.Interfaces;
using Pencil.Gaming.Graphics;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace FurryLana.Engine.Texture
{
    /// <summary>
    /// Texture implementation.
    /// </summary>
    public class CubeTexture : ITexture
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Texture.Texture"/> class.
        /// </summary>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <param name="name">Name.</param>
        public CubeTexture (int width, int height, string name)
        {
            if (width < 0)
                throw new ArgumentException ("Texture width must not be lower than 0!", "width");
            if (height < 0)
                throw new ArgumentException ("Texture height must not be lower than 0!", "height");

            Loaded = false;

            Width = width;
            Height = height;
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Texture.Texture"/> class.
        /// </summary>
        /// <param name="bmp">Bmp.</param>
        /// <param name="name">Name.</param>
        public CubeTexture (Bitmap xPos, Bitmap xNeg,
                            Bitmap yPos, Bitmap yNeg,
                            Bitmap zPos, Bitmap zNeg, string name)
        {
            Loaded = false;

            if (xPos.Width  != xNeg.Width  ||
                xPos.Width  != yPos.Width  || 
                xPos.Width  != yNeg.Width  ||
                xPos.Width  != zPos.Width  ||
                xPos.Width  != zNeg.Width  ||
                xPos.Height != xNeg.Height ||
                xPos.Height != yPos.Height || 
                xPos.Height != yNeg.Height ||
                xPos.Height != zPos.Height ||
                xPos.Height != zNeg.Height)
            {
                throw new ArgumentException ("The width and height of all cube textures may be the same!");
            }

            Width = xPos.Width;
            Height = xPos.Height;
            Name = name;
            CubeBitmap[0] = xPos;
            CubeBitmap[1] = xNeg;
            CubeBitmap[2] = yPos;
            CubeBitmap[3] = yNeg;
            CubeBitmap[4] = zPos;
            CubeBitmap[5] = zNeg;
        }

        protected void UpdateTextures ()
        {
            GL.GenTextures (1, out id);
            GL.BindTexture (TextureTarget.TextureCubeMap, id); // bind it

            for (int i = 0; i < 6; i++)
                UpdateTexture (i);

            // prevent feedback, reading and writing to the same image is a bad idea
            GL.BindTexture (TextureTarget.TextureCubeMap, 0);
        }

        /// <summary>
        /// Updates the texture.
        /// </summary>
        protected void UpdateTexture (int i)
        {
            GL.TexParameter (TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter,
                             (int) TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter (TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter,
                             (int) TextureMagFilter.Linear);
            GL.TexParameter (TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS,
                             (int) TextureWrapMode.ClampToEdge);
            GL.TexParameter (TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT,
                             (int) TextureWrapMode.ClampToEdge);
            GL.TexParameter (TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR,
                             (int) TextureWrapMode.ClampToEdge);

            GL.TexImage2D (TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba,
                           Width, Height, 0, Pencil.Gaming.Graphics.PixelFormat.Bgra,
                           PixelType.UnsignedByte, IntPtr.Zero);

            GL.GenerateMipmap (GenerateMipmapTarget.TextureCubeMap);
        }

        /// <summary>
        /// The width.
        /// </summary>
        protected int width;

        /// <summary>
        /// The height.
        /// </summary>
        protected int height;

        /// <summary>
        /// The depth.
        /// </summary>
        protected int depth = 6;

        /// <summary>
        /// The identifier.
        /// </summary>
        protected uint id;

        /// <summary>
        /// The cube bitmap.
        /// </summary>
        protected Bitmap[] CubeBitmap = new Bitmap[6];

        #region ITexture implementation

        /// <summary>
        /// Tos the bitmap.
        /// </summary>
        /// <returns>The bitmap.</returns>
        public Bitmap ToBitmap ()
        {
            throw new NotImplementedException ();
        }

        /// <summary>
        /// Bind the texture.
        /// </summary>
        public void Bind ()
        {
            GL.BindTexture (TextureTarget.TextureCubeMap, id);
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                if (Loaded)
                    UpdateTextures ();
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                if (Loaded)
                    UpdateTextures ();
            }
        }

        /// <summary>
        /// Gets or sets the depth.
        /// </summary>
        /// <value>The depth.</value>
        public int Depth
        {
            get
            {
                return depth;
            }
            set
            {
                depth = value;
                if (Loaded)
                    UpdateTextures ();
            }
        }

        #endregion

        #region IResource implementation

        /// <summary>
        /// Init this resource. This method may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        public void Init ()
        {}

        /// <summary>
        /// Gets the init jobs.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs (List<Action> list)
        {
            list.Add (Init);
            return list;
        }

        /// <summary>
        /// Load this resource. This method *should* be called from an extra loading thread with a shared gl context.
        /// </summary>
        public void Load ()
        {
            Loaded = false;

            GL.GenTextures (1, out id);
            
            GL.BindTexture (TextureTarget.TextureCubeMap, id); // bind it

            for (int i = 0; i < 6; i++)
            {
                if (CubeBitmap[i] == null)
                {
                    UpdateTexture (i);
                }
                else
                {
                    BitmapData bmp_data =
                        CubeBitmap[i].LockBits (new Rectangle (0, 0, CubeBitmap[i].Width, CubeBitmap[i].Height),
                                                ImageLockMode.ReadOnly,
                                                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.TexImage2D (TextureTarget.TextureCubeMapPositiveX + i, 0,
                                   PixelInternalFormat.Rgba,
                                   bmp_data.Width, bmp_data.Height, 0,
                                   Pencil.Gaming.Graphics.PixelFormat.Bgra,
                                   PixelType.UnsignedByte, bmp_data.Scan0);

                    CubeBitmap[i].UnlockBits (bmp_data);

                    GL.TexParameter (TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter,
                                     (int) TextureMinFilter.LinearMipmapLinear);
                    GL.TexParameter (TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter,
                                     (int) TextureMagFilter.Linear);
                    GL.TexParameter (TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS,
                                     (int) TextureWrapMode.ClampToEdge);
                    GL.TexParameter (TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT,
                                     (int) TextureWrapMode.ClampToEdge);
                    GL.TexParameter (TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR,
                                     (int) TextureWrapMode.ClampToEdge);

                    GL.GenerateMipmap (GenerateMipmapTarget.TextureCubeMap);
                    CubeBitmap[i].Dispose ();

                    bmp_data = null;
                    CubeBitmap[i] = null;
                }
            }

            // prevent feedback, reading and writing to the same image is a bad idea
            GL.BindTexture (TextureTarget.TextureCubeMap, 0);

            Loaded = true;
        }

        /// <summary>
        /// Gets the load jobs.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">The NeedsLoad event handler.</param>
        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            list.Add (Load);
            NeedsLoad = reloader;
            return list;
        }

        /// <summary>
        /// Destroy this resource.
        /// 
        /// Why not IDisposable:
        /// IDisposable is called from within the grabage collector context so we do not have a valid gl context there.
        /// Therefore I added the Destroy function as this would be called by the parent instance within a valid gl
        /// context.
        /// </summary>
        public void Destroy ()
        {
            GL.DeleteTexture (id);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Engine.Texture.Texture"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; protected set; }

        /// <summary>
        /// Fire this event when you need the Load function to be called.
        /// For example after init or when new resources needs to be loaded.
        /// </summary>
        /// <value>NeedsLoad handlers.</value>
        public EventHandler NeedsLoad { get; set; }

        #endregion

        #region IManageable implementation

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion
    }
}
