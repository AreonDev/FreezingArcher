//
//  Maze.cs
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
using System;
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.DataStructures.Graphs;
using FreezingArcher.Math;
using FreezingArcher.Core;
using FreezingArcher.Output;
using Pencil.Gaming.Graphics;

namespace FreezingArcher.Game.Maze
{
    /// <summary>
    /// Initialize maze delegate.
    /// </summary>
    delegate void InitializeMazeDelegate(ref ObjectManager objectManager,
        ref WeightedGraph<MazeCell, MazeCellEdgeWeight> graph, ref ModelSceneObject[,] models, ref Random rand,
        MazeColorTheme theme, uint x, uint y);

    /// <summary>
    /// Generate maze delegate.
    /// </summary>
    delegate void GenerateMazeDelegate(ref WeightedGraph<MazeCell, MazeCellEdgeWeight> graph, ref Random rand,
        int maximumContinuousPathLength, double turbulence);

    /// <summary>
    /// Add maze to scene delegate.
    /// </summary>
    delegate void AddMazeToSceneDelegate(ref WeightedGraph<MazeCell, MazeCellEdgeWeight> graph,
        ref ModelSceneObject[,] models, ref CoreScene scene, float scaling, uint maxX, int offsX, int offsY);

    /// <summary>
    /// Calculate path to exit delegate.
    /// </summary>
    delegate void CalculatePathToExitDelegate(ref WeightedGraph<MazeCell, MazeCellEdgeWeight> graph);

    /// <summary>
    /// Spawn portals delegate.
    /// </summary>
    delegate void PlaceFeaturesDelegate(WeightedGraph<MazeCell, MazeCellEdgeWeight> previous,
        WeightedGraph<MazeCell, MazeCellEdgeWeight> current, WeightedGraph<MazeCell, MazeCellEdgeWeight> next,
        Random rand, uint portalSpawnFactor);

    /// <summary>
    /// Maze.
    /// </summary>
    public class Maze
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.Maze.Maze"/> class.
        /// </summary>
        /// <param name="objmnr">Object manager.</param>
        /// <param name="seed">Seed.</param>
        /// <param name="sizeX">Size x.</param>
        /// <param name="sizeY">Size y.</param>
        /// <param name="scale">Scale.</param>
        /// <param name="theme">The theme of this maze.</param>
        /// <param name="initFunc">Init func.</param>
        /// <param name="generateFunc">Generate func.</param>
        /// <param name="addToSceneDelegate">Add to scene delegate.</param>
        /// <param name="exitFunc">Calculate path to exit function.</param>
        /// <param name="placeFeaturesFunc">Spawn portals function.</param>
        /// <param name="turbulence">Turbulence.</param>
        /// <param name="maximumContinuousPathLength">Maximum continuous path length.</param>
        /// <param name="portalSpawnFactor">Portal spawn factor.</param>
        internal Maze (ObjectManager objmnr, int seed, int sizeX, int sizeY, float scale, MazeColorTheme theme,
            InitializeMazeDelegate initFunc, GenerateMazeDelegate generateFunc,
            AddMazeToSceneDelegate addToSceneDelegate, CalculatePathToExitDelegate exitFunc,
            PlaceFeaturesDelegate placeFeaturesFunc, double turbulence, int maximumContinuousPathLength,
            uint portalSpawnFactor)
        {
            objectManager = objmnr;
            Seed = seed;
            Size = new Vector2i(sizeX, sizeY);
            Theme = theme;
            this.scale = scale;
            Turbulence = turbulence;
            PortalSpawnFactor = portalSpawnFactor;
            MaximumContinuousPathLength = maximumContinuousPathLength;
            rand = new Random (seed);
            initMazeDelegate = initFunc;
            generateMazeDelegate = generateFunc;
            addMazeToSceneDelegate = addToSceneDelegate;
            calcExitPathDelegate = exitFunc;
            placeFeaturesDelegate = placeFeaturesFunc;
        }

        internal WeightedGraph<MazeCell, MazeCellEdgeWeight> graph;

        Random rand;

        ModelSceneObject[,] models;

        ObjectManager objectManager;

        readonly InitializeMazeDelegate initMazeDelegate;

        readonly GenerateMazeDelegate generateMazeDelegate;

        readonly AddMazeToSceneDelegate addMazeToSceneDelegate;

        readonly CalculatePathToExitDelegate calcExitPathDelegate;

        readonly PlaceFeaturesDelegate placeFeaturesDelegate;

        readonly float scale;

        /// <summary>
        /// The theme of this maze.
        /// </summary>
        public readonly MazeColorTheme Theme;

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public Vector2i Offset { get; set; }

        /// <summary>
        /// Gets the turbulence.
        /// </summary>
        /// <value>The turbulence.</value>
        public double Turbulence { get; private set; }

        /// <summary>
        /// Gets the maximum continuous path length.
        /// </summary>
        /// <value>The maximum coninous path.</value>
        public int MaximumContinuousPathLength { get; private set; }

        /// <summary>
        /// Gets the seed.
        /// </summary>
        /// <value>The seed.</value>
        public int Seed { get; private set; }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public Vector2i Size { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is generated.
        /// </summary>
        /// <value><c>true</c> if this instance is generated; otherwise, <c>false</c>.</value>
        public bool IsGenerated { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the exit path has been calculated.
        /// </summary>
        /// <value><c>true</c> if exit path was calculated; otherwise, <c>false</c>.</value>
        public bool IsExitPathCalculated { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="FreezingArcher.Game.Maze.Maze"/> are features placed.
        /// </summary>
        /// <value><c>true</c> if are features placed; otherwise, <c>false</c>.</value>
        public bool AreFeaturesPlaced { get; private set; }

        /// <summary>
        /// Gets the portal spawn factor.
        /// </summary>
        /// <value>The portal spawn factor.</value>
        public uint PortalSpawnFactor { get; private set; }

        /// <summary>
        /// Init this instance.
        /// </summary>
        public void Init()
        {
            if (initMazeDelegate != null)
            {
                IsInitialized = true;
                initMazeDelegate(ref objectManager, ref graph, ref models, ref rand, Theme, (uint) Size.X, (uint) Size.Y);
            }
            else
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "Maze", "Cannot initialize maze as the initializer is null!");
            }
        }

        /// <summary>
        /// Generate this instance.
        /// </summary>
        public void Generate(CoreScene scene = null)
        {
            if (!IsInitialized)
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "Maze", "Cannot generate maze as it is not initialized!");
                return;
            }

            if (IsGenerated)
            {
                Logger.Log.AddLogEntry(LogLevel.Warning, "Maze", "The maze has already been generated!");
                return;
            }

            if (generateMazeDelegate != null)
            {
                IsGenerated = true;
                generateMazeDelegate(ref graph, ref rand, MaximumContinuousPathLength, Turbulence);

                if (scene != null)
                    AddToScene(scene);
            }
            else
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "Maze", "Failed to generate maze as the generator is null!");
            }
        }

        /// <summary>
        /// Spawns the features.
        /// </summary>
        /// <param name="previous">Previous maze graph.</param>
        /// <param name="next">Next maze graph.</param>
        public void SpawnFeatures(WeightedGraph<MazeCell, MazeCellEdgeWeight> previous = null,
            WeightedGraph<MazeCell, MazeCellEdgeWeight> next = null)
        {
            if (!IsInitialized)
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "Maze", "Cannot place features as maze is not initialized!");
                return;
            }

            if (!IsGenerated)
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "Maze", "Cannot place features as maze is not generated!");
                return;
            }

            if (AreFeaturesPlaced)
            {
                Logger.Log.AddLogEntry(LogLevel.Warning, "Maze", "Features are already placed in this maze!");
                return;
            }

            if (placeFeaturesDelegate != null)
            {
                AreFeaturesPlaced = true;
                placeFeaturesDelegate (previous, graph, next, rand, PortalSpawnFactor);
            }
            else
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "Maze", "Failed to place features as placing delegate is null!");
            }
        }

        /// <summary>
        /// Adds to scene.
        /// </summary>
        /// <param name="scene">Scene.</param>
        public void AddToScene(CoreScene scene)
        {
            if (!IsInitialized)
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "Maze", "Cannot add maze to scene as the maze is not initialized!");
                return;
            }

            if (addMazeToSceneDelegate != null)
            {
                addMazeToSceneDelegate(ref graph, ref models, ref scene, scale, (uint) Size.X, Offset.X, Offset.Y);
            }
            else
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "Maze", "Cannot add maze to scene as scene adder is null!");
            }
        }

        /// <summary>
        /// Calculates the path to maze exit.
        /// </summary>
        public void CalculatePathToExit()
        {
            if (!IsInitialized)
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "Maze", "Cannot calculate path as maze is not initialized!");
                return;
            }

            if (!IsGenerated)
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "Maze", "Cannot calculate path as maze is not generated!");
                return;
            }

            if (IsExitPathCalculated)
            {
                Logger.Log.AddLogEntry(LogLevel.Warning, "Maze", "Exit path is already calculated!");
                return;
            }

            if (calcExitPathDelegate != null)
            {
                IsExitPathCalculated = true;
                calcExitPathDelegate(ref graph);
            }
            else
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "Maze", "Failed to calculate path to exit as calculator is null!");
            }
        }
    }
}
