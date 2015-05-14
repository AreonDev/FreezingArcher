//
//  Main.cs
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
using FreezingArcher.Core;
using FreezingArcher.Content;
using System;
using System.Reflection.Emit;
using System.Runtime.Remoting.Channels;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices.WindowsRuntime;
using FreezingArcher.Renderer;
using FreezingArcher.Messaging;
using FreezingArcher.Output;
using FreezingArcher.Math;

namespace FreezingArcher.Game
{
	public class FurryLanaTest : FreezingArcher.Messaging.Interfaces.IMessageConsumer
	{
		#region IMessageConsumer Implementation

		public FurryLanaTest(MessageManager mssgmngr)
		{
			ValidMessages = new int[] { (int)MessageId.Input };
			mssgmngr += this;
		}

		public virtual int[] ValidMessages { get; protected set; }

		public virtual void ConsumeMessage(Messaging.Interfaces.IMessage msg)
		{
			InputMessage im = msg as InputMessage;
			if (im != null) 
			{
				if (im.IsKeyPressed ("left")) 
				{
					Cube_X -= 0.05f;
				}

				if (im.IsKeyPressed ("right")) 
				{
					Cube_X += 0.05f;
				}

				if (im.IsKeyPressed ("forward")) 
				{
					Cube_Z -= 0.05f;
				}

				if (im.IsKeyPressed ("backward")) 
				{
					Cube_Z += 0.05f;
				}
			}
		}

		#endregion


		/*
		 * AUFGABE 1
		 * Grundlegende Kameras schreiben und mit API vertraut machen!
		 * Zeit: Maximal 2 Stunden...
		 * Voraussetzungen: ALLE
		 * Wird diese Aufgabe Spaß machen?: ABSOLUT NICHT
		 * Wird sie dir was nutzen?: ABSOLUT JA, damit steigst du richtig ins Projekt ein
		 * 
		 * Du hast 3 Klassen, die du bearbeiten musst... 
		 * 1. MyFirstFreezingArcherCam
		 * 2. ThirdPersonCamera
		 * 3. FreeCamera
		 * 
		 * 
		 * Hinweis: Messaging System von FIN hat gerade seine Probleme.. InputMessage.IsKeyPressed immer NullReferenceException
		 * WindowResizeMessage wird nicht geworfen... vorübergehend erstmal unwichtig... sollte aber trotzdem gefixt werden
		 * Die Kamera-Objekte kannst du hier irgendwo reinschmeißen, wie du lustig bist....
		 * Ich wünsche mir aber ein Feature, oder eine Möglichkeit, zwischen den Kameras zu wechseln.....
		 */

        public void Draw ()
        {
            RendererContext rctx = Application.Instance.RendererContext;
            rctx.Begin ();

            rctx.Clear (Color4.AliceBlue);

            rctx.BasicEffect.Use ();

            //TODO: Hier soll dann die Kamera benutzt und gesetzt werden
            //Sprich: ViewMatrix der Kamera in BasicEffect.View
            //und ProjectionMatrix in BasicEffect.Projection
            rctx.BasicEffect.View = Matrix.LookAt (new Vector3 (_width / 1.0f, 10.0f, 10.0f), new Vector3 (_width / 1.0f, 0.0f, -_height / 1.0f), Vector3.UnitY); 
            rctx.BasicEffect.Projection = Matrix.CreatePerspectiveFieldOfView ((float)System.Math.PI / 4.0f, 
                (float)Application.Instance.RendererContext.ViewportSize.X / (float)Application.Instance.RendererContext.ViewportSize.Y, 0.1f, 100.0f); 

            rctx.BasicEffect.UseColor = true;

            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    rctx.BasicEffect.World = Matrix.CreateTranslation (new Vector3 (i * 2, 0.0f, -j * 2));
                    rctx.BasicEffect.Update ();

                    cubes [j * _width + i].Draw (rctx);
                }
            }

            //Draw red Cube

            rctx.BasicEffect.World = Matrix.CreateTranslation (new Vector3 (Cube_X, 2.0f, Cube_Z));
            rctx.BasicEffect.Update ();

            cubes [_width * _height].Draw (rctx);

            rctx.End ();
        }

        Renderer.HelperClasses.SimpleCube[] cubes;
        int _width, _height;
        float Cube_X, Cube_Z;

        public void InitCubes (int width, int height)
        {
            Cube_X = width;
            Cube_Z = -height;

            this._width = width;
            this._height = height;

            FreezingArcher.Math.Color4 red = FreezingArcher.Math.Color4.Red;
            FreezingArcher.Math.Color4 green = FreezingArcher.Math.Color4.Green;
            FreezingArcher.Math.Color4 blue = FreezingArcher.Math.Color4.Blue;

            cubes = new FreezingArcher.Renderer.HelperClasses.SimpleCube[width * height + 1];

            for (int i = 0; i < width * height; i++)
            {
                cubes [i] = new FreezingArcher.Renderer.HelperClasses.SimpleCube (((i % 2) == 0) ? green : blue);
                cubes [i].Init (Application.Instance.RendererContext);
            }

            cubes [width * height] = new FreezingArcher.Renderer.HelperClasses.SimpleCube (red);
            cubes [width * height].Init (Application.Instance.RendererContext);
        }
    }

    /// <summary>
    /// FurryLana static main class.
    /// </summary>
    public class FurryLana// : FreezingArcher.Messaging.Interfaces.IMessageConsumer
    {

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main (string[] args)
        {
            Application.Instance = new Application ("Freezing Archer", args);
            Application.Instance.Init ();
            Application.Instance.Load ();
	   
            ComponentRegistry.Instance.Register<TransformComponent> ();

            Entity test = Application.Instance.ObjectManager.CreateOrRecycle<Entity> ();
            test.Init ("Test Entity", Application.Instance.MessageManager);

            test.AddComponent<TransformComponent>();

	    var transComp = test.GetComponent<TransformComponent>();

            new LabyrinthGenerator(Application.Instance.ObjectManager, Application.Instance.MessageManager, new Random().Next());
            transComp = test.GetComponent<TransformComponent> ();
	    
            FurryLanaTest frln = new FurryLanaTest (Application.Instance.MessageManager);
            frln.InitCubes (20, 20);

            Application.Instance.Draw += frln.Draw;

            Application.Instance.Run ();
            Application.Instance.Destroy ();
        }
    }
}
