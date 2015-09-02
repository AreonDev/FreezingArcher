//
//  Inventory.cs
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
using System.Linq;
using FreezingArcher.Math;
using FreezingArcher.DataStructures;
using System.Collections.Generic;
using FreezingArcher.Core;
using FreezingArcher.Output;
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Renderer.Scene.SceneObjects;
using Jitter.LinearMath;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using FreezingArcher.Content;
using FreezingArcher.Game.Maze;

namespace FreezingArcher.Game
{
    public sealed class Inventory : IMessageCreator
    {
        public Inventory(MessageProvider messageProvider, GameState state, Entity player, int sizeX, int sizeY, byte barSize)
            : this(messageProvider, state, player, new Vector2i(sizeX, sizeY), barSize)
        {
        }

        public Inventory(MessageProvider messageProvider, GameState state, Entity player, Vector2i size, byte barSize)
        {
            Size = size;
            messageProvider += this;
            this.messageProvider = messageProvider;
            this.player = player;
            gameState = state;
            storage = new int?[Size.X, Size.Y];

            if (barSize < 1 || barSize > 10)
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "Inventory",
                    "Inventory bar size must be between 1 and 10 but is {0}!", barSize);
                return;
            }

            inventoryBar = new int?[barSize];
        }

        public void SwitchItemsToGameState(GameState source, GameState dest, FreezingArcher.Content.Game game)
        {
            foreach (ItemComponent ic in items.Values)
            {
                game.MoveEntityToGameState (ic.Entity, source, dest);
            }
        }

        public Vector2i Size { get; private set; }

        readonly int?[,] storage;
        readonly Entity player;
        readonly GameState gameState;

        SortedDictionary<int, ItemComponent> items = new SortedDictionary<int, ItemComponent>();

        int?[] inventoryBar;

        public byte ActiveBarPosition { get; private set; }

        MessageProvider messageProvider;

        public bool PutInBar(int invPositionX, int invPositionY, byte barPosition)
        {
            if (inventoryBar.Contains(storage[invPositionX, invPositionY]))
            {
                MoveBarItemToPosition(GetItemAt(invPositionX, invPositionY), barPosition);
                return false;
            }

            if (barPosition >= 0 && barPosition < inventoryBar.Length &&
                invPositionX >= 0 && invPositionX < storage.GetLength(0) &&
                invPositionY >= 0 && invPositionY < storage.GetLength(1))
            {
                inventoryBar[barPosition] = storage[invPositionX, invPositionY];
                if (MessageCreated != null)
                {
                    var pos = new Vector2i(invPositionX, invPositionY);
                    MessageCreated(new ItemAddedToInventoryBarMessage(GetItemAt(pos), pos, barPosition));
                }
                return true;
            }

            return false;
        }

        public void RemoveFromBar(byte barPosition)
        {
            var item = GetBarItemAt(barPosition);
            inventoryBar[barPosition] = null;

            if (MessageCreated != null)
                MessageCreated(new ItemRemovedFromInventoryBarMessage(item));
        }

        public IEnumerable<Tuple<ItemComponent, Vector2i>> Items
        {
            get
            {
                foreach (var item in items.Values)
                    yield return new Tuple<ItemComponent, Vector2i>(item, GetPositionOfItem(item));
            }
        }

        public bool PutInBar(Vector2i invPosition, byte barPosition)
        {
            return PutInBar(invPosition.X, invPosition.Y, barPosition);
        }

        public bool PutInBar(int invPositionX, int invPositionY)
        {
            byte barPosition = 0;

            while (inventoryBar[barPosition] != null && barPosition < inventoryBar.Length)
                barPosition++;

            if (barPosition >= inventoryBar.Length)
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "Inventory",
                    "Inventory bar is full. Cannot add new item to inventory bar!");
                return false;
            }

            return PutInBar(invPositionX, invPositionY, barPosition);
        }

        public bool PutInBar(Vector2i position)
        {
            return PutInBar(position.X, position.Y);
        }

        public ItemComponent GetBarItemAt(byte position)
        {
            if (position < 0 || position >= inventoryBar.Length)
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "Inventory",
                    "The given inventory bar position '{0}' is out of range!", position);
                return null;
            }

            var id = inventoryBar[position];
            ItemComponent item = null;
            if (id != null)
                items.TryGetValue(id.Value, out item);
            return item;
        }

        public bool SetActiveBarItem(byte position)
        {
            if (position < 0 || position >= inventoryBar.Length)
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "Inventory",
                    "The given inventory bar position '{0}' is out of range!", position);
                return false;
            }

            ActiveBarPosition = position;

            if (MessageCreated != null)
                MessageCreated(new ActiveInventoryBarItemChangedMessage(GetBarItemAt(position), position));

            return true;
        }

        public ItemComponent ActiveBarItem
        {
            get
            {
                ItemComponent item = null;
                if (inventoryBar[ActiveBarPosition] != null)
                    items.TryGetValue(inventoryBar[ActiveBarPosition].Value, out item);
                
                return item;
            }
        }

        public ItemComponent[] InventoryBar
        {
            get
            {
                var bar = new ItemComponent[inventoryBar.Length];

                for (int i = 0; i < inventoryBar.Length; i++)
                {
                    if (inventoryBar[i] != null)
                        items.TryGetValue(inventoryBar[i].Value, out bar[i]);
                }

                return bar;
            }
        }

        public byte GetPositionOfBarItem(ItemComponent item)
        {
            var hc = item.GetHashCode();
            byte index = 0;

            while (index < inventoryBar.Length && inventoryBar[index] != hc)
            {
                index++;
            }

            return index;
        }

        public bool MoveBarItemToPosition(ItemComponent item, byte position)
        {
            if (inventoryBar[position] == null)
            {
                inventoryBar[GetPositionOfBarItem(item)] = null;
                inventoryBar[position] = item.GetHashCode();

                if (MessageCreated != null)
                    MessageCreated(new BarItemMovedMessage(item, position));

                return true;
            }
            return false;
        }

        public ItemComponent GetItemAt(Vector2i position)
        {
            return GetItemAt(position.X, position.Y);
        }

        public ItemComponent GetItemAt(int positionX, int positionY)
        {
            ItemComponent res = null;
            var id = storage[positionX, positionY];
            if (id != null)
                items.TryGetValue(id.Value, out res);
            return res;
        }

        public Vector2i GetPositionOfItem(ItemComponent item)
        {
            var hc = item.GetHashCode();

            int x, y = 0;
            for (; y < storage.GetLength(0); y++)
            {
                x = 0;
                for (; x < storage.GetLength(1); x++)
                {
                    if (storage[y, x] == hc)
                        return new Vector2i(y, x);
                }
            }

            Logger.Log.AddLogEntry(LogLevel.Error, "Inventory", "Item does not exist in inventory!");

            return new Vector2i(-1, -1);
        }

        public bool Insert(ItemComponent item)
        {
            bool result = false;
            int x = 0, y = 0;
            for (; y < storage.GetLength(1); y++)
            {
                x = 0;
                for (; x < storage.GetLength(0); x++)
                {
                    if (fits(new Vector2i(x, y), item.Size))
                    {
                        result = true;
                        item.Orientation = Orientation.Horizontal;
                        break;
                    }
                    if (fits(new Vector2i(x, y), item.Size.Yx))
                    {
                        result = true;
                        item.Orientation = Orientation.Vertical;
                        break;
                    }
                }
                if (result)
                    break;
            }

            if (!result)
            {
                Logger.Log.AddLogEntry(LogLevel.Debug, "Inventory", "Inventory is full - cannot add item!");
            }

            return result && Insert(item, new Vector2i(x, y));
        }

        public bool Insert(ItemComponent item, Vector2i position)
        {
            var size = item.Orientation == Orientation.Vertical ? item.Size.Yx : item.Size;
            if (fits(position, size))
            {
                for (int i = 0; i < size.X; i++)
                {
                    for (int k = 0; k < size.Y; k++)
                    {
                        storage[position.X + i, position.Y + k] = item.GetHashCode();
                    }
                }

                items.Add(item.GetHashCode(), item);

                return true;
            }
            else
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "Inventory",
                    "Failed to insert inventory item '{0}' at position <{1},{2}> - position already in use!",
                    item.Entity.Name, position.X, position.Y);
            }
            return false;
        }

        public ItemComponent TakeOut(Vector2i position)
        {
            return TakeOut(position.X, position.Y);
        }

        public ItemComponent TakeOut(ItemComponent item)
        {
            var pos = GetPositionOfItem(item);
            if (pos.X >= 0 && pos.Y >= 0)
                return TakeOut(pos);
            return null;
        }

        public ItemComponent TakeOut(int positionX, int positionY)
        {
            var id = storage[positionX, positionY];

            if (id == null)
                return null;

            int y, x = 0;
            for (; x < storage.GetLength(0); x++)
            {
                y = 0;
                for (; y < storage.GetLength(1); y++)
                {
                    if (storage[x, y] == id)
                        storage[x, y] = null;
                }
            }

            byte barpos = 0;
            while (barpos < inventoryBar.Length && inventoryBar[barpos] != id)
                barpos++;

            if (barpos < inventoryBar.Length)
                inventoryBar[barpos] = null;

            ItemComponent item;
            items.TryGetValue(id.Value, out item);
            items.Remove(id.Value);

            return item;
        }

        public void PrintStorage(string message)
        {
            string s = message;
            for (int i = 0; i < Size.Y; i++)
            {
                s += "\n";
                for (int k = 0; k < Size.X; k++)
                {
                    var item = GetItemAt(k, i);
                    s += item != null ? item.Entity.Name : "#";
                }
            }

            Logger.Log.AddLogEntry(LogLevel.Debug, "Inventory", s);
        }

        private bool fits(Vector2i position, Vector2i size)
        {
            for (int i = 0; i < size.X; i++)
            {
                for (int k = 0; k < size.Y; k++)
                {
                    if (position.X + i >= Size.X || position.Y + k >= Size.Y ||
                        storage[position.X + i, position.Y + k] != null)
                        return false;
                }
            }
            return true;
        }

        public static ItemComponent CreateNewItem(MessageProvider messageProvider, GameState state,
            string name, string imageLocation, string description,
            string modelPath, Vector2i size, Vector3 offset, Quaternion rotation, Shape shape, ItemLocation location,
            AttackClass attackClasses, ItemUsage itemUsages, Protection protection, Material physicsMaterial,
            float mass, float healthDelta, float usageDeltaPerUsage, float attackStrength, float throwPower, float usage)
        {
            var entity = EntityFactory.Instance.CreateWith(name, messageProvider,
                systems: new[] { typeof(ItemSystem), typeof(ModelSystem), typeof(PhysicsSystem) });

            var item = entity.GetComponent<ItemComponent>();
            item.ImageLocation = imageLocation;
            item.Description = description;
            item.Size = size;
            item.Location = location;
            item.AttackClasses = attackClasses;
            item.ItemUsages = itemUsages;
            item.Protection = protection;
            item.HealthDelta = healthDelta;
            item.AttackStrength = attackStrength;
            item.ThrowPower = throwPower;
            item.Usage = usage;
            item.UsageDeltaPerUsage = usageDeltaPerUsage;
            item.Mass = mass;
            item.PhysicsMaterial = physicsMaterial;
            item.PositionOffset = offset;
            item.Rotation = rotation;
            item.ItemUsageHandler = new MazeItemUseHandler();

            var model = new ModelSceneObject(modelPath);
            model.Enabled = true;
            state.Scene.AddObject(model);
            entity.GetComponent<ModelComponent>().Model = model;

            var transform = entity.GetComponent<TransformComponent>();
            var physics = entity.GetComponent<PhysicsComponent> ();

            if (shape == null)
            {
                List<JVector> vertices = new List<JVector>();
                model.Model.Meshes[0].Vertices.ForEach(x => vertices.Add(x.ToJitterVector()));

                List<TriangleVertexIndices> indices = new List<TriangleVertexIndices>();

                for(int i = 0; i < model.Model.Meshes[0].Indices.Length; i+= 3)
                {
                    int i0 = model.Model.Meshes[0].Indices[i+0];
                    int i1 = model.Model.Meshes[0].Indices[i+1];
                    int i2 = model.Model.Meshes[0].Indices[i+2];

                    indices.Add(new TriangleVertexIndices(i0, i1, i2));
                }

                shape = new TriangleMeshShape(new Octree(vertices, indices));
            }

            var body = new RigidBody(shape);
            body.Position = transform.Position.ToJitterVector ();
            if (mass >= 0)
                body.Mass = mass;
            body.Material = physicsMaterial;
            body.AllowDeactivation = true;
            body.Tag = entity;

            state.PhysicsManager.World.AddBody(body);
            physics.RigidBody = body;
            physics.World = state.PhysicsManager.World;
            physics.PhysicsApplying = AffectedByPhysics.Orientation | AffectedByPhysics.Position;
            physics.RigidBody.IsStatic = false;
            physics.RigidBody.IsActive = false;
            physics.RigidBody.Position = transform.Position.ToJitterVector();
            model.Position = transform.Position;

            return item;
        }

        #region IMessageCreator implementation

        public event MessageEvent MessageCreated;

        #endregion
    }
}
