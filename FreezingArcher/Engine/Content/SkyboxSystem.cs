//
//  SkyboxSystem.cs
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
using FreezingArcher.Messaging.Interfaces;

namespace FreezingArcher.Content
{
    public sealed class SkyboxSystem : EntitySystem
    {
        /// <summary>
        /// Initialize this system. This may be used as a constructor replacement.
        /// </summary>
        /// <param name="msgmnr">Msgmnr.</param>
        /// <param name="entity">Entity.</param>
        public override void Init(MessageManager msgmnr, Entity entity)
        {
            base.Init(msgmnr, entity);

            //Added needed components
            NeededComponents = new[] { typeof(TransformComponent), typeof(SkyboxComponent) };

            //Needs more Initializing?
            //Scene does this for me, so no!
            internalValidMessages = new[] { (int)MessageId.PositionChangedMessage };
            msgmnr += this;
        }

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public override void ConsumeMessage(IMessage msg)
        {
            SkyboxComponent sc = Entity.GetComponent<SkyboxComponent>();
            TransformComponent tc = Entity.GetComponent<TransformComponent>();

            if (sc == null || sc.Skybox == null)
                return;

            if (msg.MessageId == (int)MessageId.PositionChangedMessage)
            {
                PositionChangedMessage pcm = msg as PositionChangedMessage;
                if (pcm.Entity.Name == Entity.Name)
                    sc.Skybox.Position = tc.Position;
            }
        }
    }
}
