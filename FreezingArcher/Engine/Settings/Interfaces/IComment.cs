using System.Collections.Generic;

namespace FreezingArcher.Settings.Interfaces
{
    /// <summary>
    /// Comment interface.
    /// </summary>
    public interface IComment
    {
        /// <summary>
        /// Get or set the comments of the item.
        /// </summary>
        /// <value>The comment list.</value>
        List<string> Comments { get; set; }
    }
}
