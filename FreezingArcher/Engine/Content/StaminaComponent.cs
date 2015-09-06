//
//  StaminaComponent.cs
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
using FreezingArcher.Messaging;
using FreezingArcher.Output;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Player component.
    /// </summary>
    public sealed class StaminaComponent : EntityComponent
    {
        #region defaults

        /// <summary>
        /// The default stamina.
        /// </summary>
        public static readonly float DefaultStamina = 100f;

        /// <summary>
        /// The default maximum stamina.
        /// </summary>
        public static readonly float DefaultMaximumStamina = 100f;

        public static readonly float DefaultStaminaDeltaPerUpdate = 0.5f;

        #endregion

        /// <summary>
        /// Gets or sets the maximum stamina.
        /// </summary>
        /// <value>The maximum stamina.</value>
        public float MaximumStamina { get; set; }

        public float StaminaDeltaPerUpdate { get; set; }

        float stamina;

        /// <summary>
        /// Gets or sets the stamina.
        /// </summary>
        /// <value>The stamina.</value>
        public float Stamina
        {
            get
            {
                return stamina;
            }
            set
            {
                float new_stamina = value;
                new_stamina = new_stamina > MaximumStamina ? MaximumStamina : new_stamina;
                var delta = new_stamina - stamina;
                stamina = new_stamina;
                CreateMessage(new StaminaChangedMessage (value, delta, Entity));
            }
        }
    }
}
