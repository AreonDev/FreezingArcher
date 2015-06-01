//
//  EmptyClass.cs
//
//  Author:
//       wfailla <>
//
//  Copyright (c) 2015 wfailla
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
using FreezingArcher.DataStructures.Trees;
using System.Collections.Generic;
using System.Collections;

namespace FreezingArcher.Game
{
    public class CameraManager
    {
        Dictionary<string, Dictionary<string, SimpleCam>> CamDict = null;
        int counter;

        public CameraManager ()
        {
            CamDict.Add ("ThirdPerson", new Dictionary<string, SimpleCam>);
            CamDict.Add ("FreeCam", new Dictionary<string, SimpleCam>);
            counter = 0;
        }

        internal string getNextCamKey(){
            return ++counter;
        }

        public static void RegisterCam(string GlobalKey="FreeCam",
                                       string CamKey=this.getNextCamKey(),
                                       SimpleCam cam)
        {
            CamDict[GlobalKey][CamKey] = cam;
        }

        public static void RemoveCam(string cam){
            string key = null;
            IEnumerator List = CamDict.Keys;

            foreach (key in CamDict.Keys) {
                if (CamDict [key].ContainsKey (cam)) {
                    break;
                }
            }

            CamDict[key].Remove (cam);
        }

        public static void ToggelCam(string cam){
            string key = null;
            IEnumerator List = CamDict.Keys;

            foreach (key in CamDict.Keys) {
                if (CamDict [key].ContainsKey (cam)) {
                    break;
                }
            }

            List.MoveNext ();

            this.RegisterCam(List.Current,cam,CamDict[key][cam]);
            this.RemoveCam(cam);

        }

        public static CameraManager operator + (CameraManager cm, object cam)
        {
            cm.RegisterCam (cam);
            return cm;
        }

        public static CameraManager operator - (CameraManager cm, object cam)
        {
            cm.RemoveCam (cam);
            return cm;
        }
    }
}

