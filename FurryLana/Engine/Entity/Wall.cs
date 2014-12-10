using FurryLana.Engine.Entity.Interfaces;
using FurryLana.Engine.Map;
using FurryLana.Engine.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace FurryLana.Engine.Entity
{
    class Wall : Tile 
    {

        public Wall(TiledMap map, IModel model, bool walkable) : base(map, model, walkable) { }
        public int ID
        {
            get { return 1; }
        }

        public IModel Model { get; set; }

        public BoundingBox BBox { get; set; }

        public void Draw()
        {
            Model.Draw();
        }

        public void Update(int deltaTime)
        {
            
        }

        public void FrameSyncedUpdate(float deltaTime)
        {
            
        }

        public void Init()
        { }

        public List<Action> GetInitJobs(List<Action> list)
        {
            list.Add(Init);
            return list;
        }

        public void Load() { }

        public List<Action> GetLoadJobs(List<Action> list, EventHandler reloader)
        {
            list.Add(Load);
            NeedsLoad = reloader;
            return list;
        }

        public void Destroy()
        {
            Model.Destroy();
        }

        public bool Loaded
        {
            get; protected set; }

        public EventHandler NeedsLoad { get; set; }

        public Pencil.Gaming.MathUtils.Vector3 Position { get; set; }

        public Pencil.Gaming.MathUtils.Vector3 Rotation { get; set; }

        public string Name { get; set; }
    }
}
