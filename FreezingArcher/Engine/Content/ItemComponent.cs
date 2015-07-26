//
//  ItemComponent.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2015 Fin Christensen
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
using FreezingArcher.Math;
using FreezingArcher.DataStructures;
using FreezingArcher.Messaging;
using Jitter.Dynamics;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Item class describing an item for use in an inventory and placement in a map.
    /// </summary>
    public sealed class ItemComponent : EntityComponent
    {
        #region defaults

        /// <summary>
        /// The default location.
        /// </summary>
        public static readonly ItemLocation DefaultLocation = ItemLocation.Ground;

        /// <summary>
        /// The default attack classes.
        /// </summary>
        public static readonly AttackClass DefaultAttackClasses = AttackClass.Nothing;

        /// <summary>
        /// The default item usages.
        /// </summary>
        public static readonly ItemUsage DefaultItemUsages = ItemUsage.Placebo;

        /// <summary>
        /// The default protection.
        /// </summary>
        public static readonly Protection DefaultProtection = new Protection();

        /// <summary>
        /// The default orientation.
        /// </summary>
        public static readonly Orientation DefaultOrientation = Orientation.Vertical;

        /// <summary>
        /// The default size.
        /// </summary>
        public static readonly Vector2i DefaultSize = Vector2i.Zero;

        /// <summary>
        /// The default health delta.
        /// </summary>
        public static readonly float DefaultHealthDelta = 0;

        /// <summary>
        /// The default attack strength.
        /// </summary>
        public static readonly float DefaultAttackStrength = 0;

        /// <summary>
        /// The default throw power.
        /// </summary>
        public static readonly float DefaultThrowPower = 0;

        /// <summary>
        /// The default usage.
        /// </summary>
        public static readonly float DefaultUsage = 0;

        /// <summary>
        /// The default image location.
        /// </summary>
        public static readonly string DefaultImageLocation = string.Empty;

        /// <summary>
        /// The default description.
        /// </summary>
        public static readonly string DefaultDescription = string.Empty;

        /// <summary>
        /// The default player.
        /// </summary>
        public static readonly Entity DefaultPlayer = null;

        /// <summary>
        /// The default position offset.
        /// </summary>
        public static readonly Vector3 DefaultPositionOffset = Vector3.Zero;

        /// <summary>
        /// The default mass.
        /// </summary>
        public static readonly float DefaultMass = 0.2f;

        /// <summary>
        /// The default physics material.
        /// </summary>
        public static readonly Material DefaultPhysicsMaterial = new Material();

        /// <summary>
        /// The default usage delta per usage.
        /// </summary>
        public static readonly float DefaultUsageDeltaPerUsage = 0.2f;

        /// <summary>
        /// The default item usage handler.
        /// </summary>
        public static readonly IItemUsageHandler DefaultItemUsageHandler = null;

        #endregion

        /// <summary>
        /// Gets or sets the player.
        /// </summary>
        /// <value>The player.</value>
        public Entity Player { get; set; }

        /// <summary>
        /// Gets or sets the image location.
        /// </summary>
        /// <value>The image location.</value>
        public string ImageLocation { get; set; }

        /// <summary>
        /// Gets or sets the position offset.
        /// </summary>
        /// <value>The position offset.</value>
        public Vector3 PositionOffset { get; set; }

        /// <summary>
        /// Gets or sets the description describing the usage of this item.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the abstract locations of this item such as ground, wall or inventory.
        /// </summary>
        /// <value>The location.</value>
        public ItemLocation Location { get; set; }

        /// <summary>
        /// Gets the attack classes of this item such as enemy or object.
        /// </summary>
        /// <value>The attack classes.</value>
        public AttackClass AttackClasses { get; set; }

        /// <summary>
        /// Gets the item usages of this item such as eatable, throwable or usable.
        /// </summary>
        /// <value>The item usages.</value>
        public ItemUsage ItemUsages { get; set; }

        /// <summary>
        /// Gets the protection this item applies to the entity.
        /// </summary>
        /// <value>The applied protection.</value>
        public Protection Protection { get; set; }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public Orientation Orientation { get; set; }

        /// <summary>
        /// Gets the size of this item for use in an inventory.
        /// </summary>
        /// <value>The size.</value>
        public Vector2i Size { get; set; }

        /// <summary>
        /// Gets the health delta applied when this item is used.
        /// </summary>
        /// <value>The health delta.</value>
        public float HealthDelta { get; set; }

        /// <summary>
        /// Gets the attack strength applied when this item is used.
        /// </summary>
        /// <value>The attack strength.</value>
        public float AttackStrength { get; set; }

        /// <summary>
        /// Gets the throw power applied when this item is thrown.
        /// </summary>
        /// <value>The throw power.</value>
        public float ThrowPower { get; set; }

        /// <summary>
        /// Gets or sets the mass.
        /// </summary>
        /// <value>The mass.</value>
        public float Mass { get; set; }

        /// <summary>
        /// Gets or sets the physics material.
        /// </summary>
        /// <value>The physics material.</value>
        public Material PhysicsMaterial { get; set; }

        /// <summary>
        /// Gets or sets the usage delta per usage. This defines the value applied to the usage each time the item is
        /// used.
        /// </summary>
        /// <value>The usage delta per usage.</value>
        public float UsageDeltaPerUsage { get; set; }

        /// <summary>
        /// Gets or sets the item usage handler.
        /// </summary>
        /// <value>The item usage handler.</value>
        public IItemUsageHandler ItemUsageHandler { get; set; }

        float usage;

        /// <summary>
        /// Gets or sets the usage describing how much of this item is already used. The value range is from 0 to 1.
        /// </summary>
        /// <value>The usage value.</value>
        public float Usage
        {
            get
            {
                return usage;
            }
            set
            {
                usage = value;
                CreateMessage(new ItemUsageChangedMessage(value, Entity));
            }
        }
    }
}
