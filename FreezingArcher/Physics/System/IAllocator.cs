using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Henge3D
{
    /// <summary>
    /// I allocator.
    /// </summary>
    public interface IAllocator<T> where T : class, new()
    {
        /// <summary>
        /// Allocate this instance.
        /// </summary>
        T Allocate ();

        /// <summary>
        /// Recycle the specified obj.
        /// </summary>
        /// <param name="obj">Object.</param>
        void Recycle (T obj);

        /// <summary>
        /// Recycle the specified objList.
        /// </summary>
        /// <param name="objList">Object list.</param>
        void Recycle (IList<T> objList);
    }
}
