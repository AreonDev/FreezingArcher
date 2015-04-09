//
//  KeyRegistry.cs
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
using Pencil.Gaming.MathUtils;
using System.Collections.Generic;
using FreezingArcher.Messaging;
using FreezingArcher.Configuration;
using Section = System.Collections.Generic.Dictionary<string, FreezingArcher.Configuration.Value>;
using Pencil.Gaming;

namespace FreezingArcher.Input
{
    public static class KeyRegistry
    {
        public static Dictionary<string, Key> Keys = new Dictionary<string, Key> ();

        static KeyRegistry ()
        {
            Keys.Add ("forward", Key.W);
            Keys.Add ("backward", Key.S);
            Keys.Add ("left", Key.A);
            Keys.Add ("right", Key.D);
            Keys.Add ("sneek", Key.LeftShift);
            Keys.Add ("run", Key.LeftControl);
        }

        public static InputMessage GenerateInputMessage (List<KeyboardInput> keys, List<MouseInput> mouse,
            Vector2 mouseMovement, Vector2 mouseScroll, float deltaTime)
        {
            List<string> skeys = new List<string> ();
            List<string> ikeys = new List<string> ();

            foreach (KeyboardInput i in keys)
                skeys.Add (i.ToString ());

            Section section;
            ConfigManager.DefaultConfig.B.TryGetValue ("keymapping", out section);
            foreach (string k in skeys)
                foreach (string s in section.Keys)
                    if (ConfigManager.Instance["freezing_archer"].GetString ("keymapping", s) == k)
                        ikeys.Add (s);

            return new InputMessage (ikeys, mouse, mouseMovement, mouseScroll, deltaTime);
        }
    }
}
