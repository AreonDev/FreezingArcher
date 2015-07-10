//
//  Game.cs
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
using System;
using System.Collections.Generic;
using System.Linq;
using FreezingArcher.Core;
using FreezingArcher.Core.Interfaces;
using FreezingArcher.DataStructures.Graphs;
using FreezingArcher.Messaging;
using FreezingArcher.Output;
using FreezingArcher.Renderer.Scene;
using System.Windows.Forms;
using FreezingArcher.Renderer;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Game.
    /// </summary>
    public sealed class Game : IManageable
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "Game";

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Content.Game"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="objmnr">Object Manager.</param>
        /// <param name="messageProvider">Message Manager.</param>
        public Game (string name, ObjectManager objmnr, MessageProvider messageProvider, RendererContext rendererContext)
        {
            Logger.Log.AddLogEntry (LogLevel.Info, ClassName, "Creating new game '{0}'", name);
            Name = name;
            MessageProvider = messageProvider;
            RendererContext = rendererContext;
            GameStateGraph = objmnr.CreateOrRecycle<DirectedWeightedGraph<GameState, GameStateTransition>>();
            GameStateGraph.Init();
        }

        /// <summary>
        /// Gets the state of the current game.
        /// </summary>
        /// <value>The state of the current game.</value>
        public GameState CurrentGameState
        {
            get
            {
                return currentNode.Data;
            }
        }

        DirectedWeightedNode<GameState, GameStateTransition> currentNode;
        MessageProvider MessageProvider;
        RendererContext RendererContext;

        /// <summary>
        /// Gets the game state graph.
        /// </summary>
        /// <value>The game state graph.</value>
        public DirectedWeightedGraph<GameState, GameStateTransition> GameStateGraph { get; private set; }

        /// <summary>
        /// Switch to the given game state.
        /// </summary>
        /// <returns><c>true</c>, if successfully switched game state, <c>false</c> otherwise.</returns>
        /// <param name="name">Game state name.</param>
        public bool SwitchToGameState(string name)
        {
            if (currentNode == null)
            {
                currentNode = GameStateGraph.Nodes.FirstOrDefault(n => n.Data.Name == name);

                if (currentNode == null)
                {
                    Logger.Log.AddLogEntry(LogLevel.Error, "Game",
                        "Cannot switch game state as '{0}' does not exist!", name);
                    return false;
                }

                if (currentNode.Data.Scene.CameraManager.ActiveCamera == null)
                {
                    currentNode.Data.Scene.CameraManager.ToggleCamera();
                }

                RendererContext.Scene = currentNode.Data.Scene;
                currentNode.Data.MessageProxy.StartProcessing();
                return true;
            }

            if (currentNode.Data.Name == name)
                return true;

            var newstateEdge = currentNode.OutgoingEdges.FirstOrDefault(e =>
                e.DestinationNode.Data.Name == name);

            if (newstateEdge == null)
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "Game", "Cannot change game state to '{0}'!", name);
                return false;
            }

            var newstate = newstateEdge.DestinationNode;

            if (newstate == null)
            {
                if (!GameStateGraph.Nodes.Any(n => n.Data.Name == name))
                {
                    Logger.Log.AddLogEntry(LogLevel.Error, ClassName,
                        "There is no game state '{0}' registered in this game!", name);
                    return false;
                }

                Logger.Log.AddLogEntry(LogLevel.Warning, ClassName,
                    "The game state '{0}' is not reachable from the current game state!", name);
                return false;
            }
            currentNode.Data.MessageProxy.StopProcessing();
            currentNode = newstate;

            if (currentNode.Data.Scene.CameraManager.ActiveCamera == null)
            {
                currentNode.Data.Scene.CameraManager.ToggleCamera();
            }

            RendererContext.Scene = currentNode.Data.Scene;
            currentNode.Data.MessageProxy.StartProcessing();
            return true;
        }

        /// <summary>
        /// Gets a game state by name.
        /// </summary>
        /// <returns>The game state.</returns>
        /// <param name="name">Name.</param>
        public GameState GetGameState(string name)
        {
            var node = GameStateGraph.Nodes.FirstOrDefault(n => n.Data.Name == name);
            return node != null ? node.Data : null;
        }

        /// <summary>
        /// Removes the state of the game.
        /// </summary>
        /// <returns><c>true</c>, if game state was removed, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        public bool RemoveGameState(string name)
        {
            var node = GameStateGraph.Nodes.FirstOrDefault(n => n.Data.Name == name);

            if (node != null)
                return GameStateGraph.RemoveNode(node);
            
            return false;
        }

        /// <summary>
        /// Adds the state of the game.
        /// </summary>
        /// <returns><c>true</c>, if game state was added, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        /// <param name="env">Env.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public bool AddGameState(string name, Environment env,
            IEnumerable<Tuple<string, GameStateTransition>> from = null,
            IEnumerable<Tuple<string, GameStateTransition>> to = null)
        {
            return AddGameState(new GameState(name, env, MessageProvider), from, to);
        }

        /// <summary>
        /// Adds the state of the game.
        /// </summary>
        /// <returns><c>true</c>, if game state was added, <c>false</c> otherwise.</returns>
        /// <param name="gameState">Game state.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public bool AddGameState(GameState gameState, IEnumerable<Tuple<string, GameStateTransition>> from = null,
            IEnumerable<Tuple<string, GameStateTransition>> to = null)
        {
            if (from == null && to == null)
            {
                GameStateGraph.AddNode(gameState);
            }
            else if (from == null && to != null)
            {
                var outgoing = new List<Pair<DirectedWeightedNode<GameState, GameStateTransition>, GameStateTransition>>();
                foreach (var t in to)
                {
                    var node = GameStateGraph.Nodes.FirstOrDefault(n => n.Data.Name == t.Item1);
                    if (node != null)
                    {
                        var trans = t.Item2 ?? GameStateTransition.DefaultTransition;
                        outgoing.Add(
                            new Pair<DirectedWeightedNode<GameState, GameStateTransition>, GameStateTransition>(
                                node, trans));
                    }
                }

                GameStateGraph.AddNode(gameState, outgoing);
            }
            else if (from != null && to == null)
            {
                var incoming = new List<Pair<DirectedWeightedNode<GameState, GameStateTransition>, GameStateTransition>>();
                foreach (var t in from)
                {
                    var node = GameStateGraph.Nodes.FirstOrDefault(n => n.Data.Name == t.Item1);

                    if (node != null)
                    {
                        var trans = t.Item2 ?? GameStateTransition.DefaultTransition;
                        incoming.Add(
                            new Pair<DirectedWeightedNode<GameState, GameStateTransition>, GameStateTransition>(
                                node, trans));
                    }
                }

                GameStateGraph.AddNode(gameState, null, incoming);
            }
            else if (from != null && to != null)
            {
                var outgoing = new List<Pair<DirectedWeightedNode<GameState, GameStateTransition>, GameStateTransition>>();
                foreach (var t in to)
                {
                    var node = GameStateGraph.Nodes.FirstOrDefault(n => n.Data.Name == t.Item1);
                    if (node != null)
                    {
                        var trans = t.Item2 ?? GameStateTransition.DefaultTransition;
                        outgoing.Add(
                            new Pair<DirectedWeightedNode<GameState, GameStateTransition>, GameStateTransition>(
                                node, trans));
                    }
                }

                var incoming = new List<Pair<DirectedWeightedNode<GameState, GameStateTransition>, GameStateTransition>>();
                foreach (var t in from)
                {
                    var node = GameStateGraph.Nodes.FirstOrDefault(n => n.Data.Name == t.Item1);

                    if (node != null)
                    {
                        var trans = t.Item2 ?? GameStateTransition.DefaultTransition;
                        incoming.Add(
                            new Pair<DirectedWeightedNode<GameState, GameStateTransition>, GameStateTransition>(
                                node, trans));
                    }
                }

                GameStateGraph.AddNode(gameState, outgoing, incoming);
            }
            else
            {
                Logger.Log.AddLogEntry(LogLevel.Severe, ClassName, Status.UnreachableLineReached);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Moves an entity from one game state to antother.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="source">Source.</param>
        /// <param name="destination">Destination.</param>
        public void MoveEntityToGameState(Entity entity, GameState source, GameState destination)
        {
            if (entity.HasComponent<ModelComponent>())
            {
                var model = entity.GetComponent<ModelComponent>().Model;
                source.Scene.RemoveObject(model);
                destination.Scene.AddObject(model);
            }

            if (entity.HasComponent<SkyboxComponent>())
            {
                var model = entity.GetComponent<SkyboxComponent>().Skybox;
                source.Scene.RemoveObject(model);
                destination.Scene.AddObject(model);
            }

            entity.SwitchMessageProvider(source.MessageProxy, destination.MessageProxy);
        }

        #region IManageable implementation

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion

        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public void Destroy()
        {
            foreach (var n in GameStateGraph.Nodes)
                n.Data.Destroy();
        }
    }
}
