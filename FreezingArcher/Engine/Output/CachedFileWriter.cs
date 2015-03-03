//
//  CachedFileWriter.cs
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
using System.IO;

namespace FreezingArcher.Output
{
    /// <summary>
    /// Filewriter which will cache the underlying file to changes
    /// </summary>
    public class CachedFileWriter
    {
        /// <summary>
        /// The cache
        /// </summary>
        private int cache;
        /// <summary>
        /// The items in the cache
        /// </summary>
        private int itemsInCache = 0;
        /// <summary>
        /// The actually writer
        /// </summary>
        private StreamWriter writerInt;
        /// <summary>
        /// The lock object
        /// </summary>
        private object lockObj = new object ();

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedFileWriter"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="append">if set to <c>true</c> the inputs will be appended to the file.</param>
        /// <param name="cacheLengt">The cache length</param>
        public CachedFileWriter (FileInfo file, bool append, int cacheLengt = 1024)
        {
            writerInt = new StreamWriter (file.FullName, append);
            writerInt.AutoFlush = false;
            cache = cacheLengt;
        }

        /// <summary>
        /// Flushes this instance.
        /// <remarks>may cause lag.</remarks>
        /// </summary>
        public void Flush ()
        {
            lock (lockObj)
            {
                writerInt.WriteLine ("-----------------------------------------------------------------------------------------");
                writerInt.WriteLine ("-----------------------------------------------------------------------------------------");
                writerInt.Flush ();
                itemsInCache = 0;
            }
        }

        /// <summary>
        /// Writes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Write (string item)
        {
            lock (lockObj)
            {
                writerInt.WriteLine (item);
                itemsInCache++;
                if (itemsInCache >= cache)
                {
                    writerInt.Flush ();
                    itemsInCache = 0;
                }
            }
        }
    }
}
