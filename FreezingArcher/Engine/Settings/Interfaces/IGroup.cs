using System.Collections;
using System.Collections.Generic;

namespace FreezingArcher.Settings.Interfaces
{
    /// <summary>
    /// Group.
    /// </summary>
    public interface IGroup : ICfgString, ISettingsRoot, IComment, IEnumerable
    {
        /// <summary>
        /// Get or set the properties of this group.
        /// </summary>
        /// <value>The properties.</value>
        List<IProperty> Properties { get; set; }

        /// <summary>
        /// Get or set the name of this group.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Get a property by its name.
        /// </summary>
        /// <returns>The property.</returns>
        /// <param name="name">The name.</param>
        IProperty GetPropertyByName (string name);

        /// <summary>
        /// Get the <see cref="FreezingArcher.Settings.Interfaces.IGroup"/> by the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        IProperty this[string name] { get; }
    }
}
