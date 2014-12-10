using FurryLana.Engine.Entity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurryLana.Engine.Entity
{
    class SnakeBody : IEntity
    {

        public int ID
        {
            get { return 2; }
        }

        public Model.Interfaces.IModel Model { get; set; }

        public BoundingBox BBox { get; set; }

        public void Draw()
        {
         
        }

        public void Update(int deltaTime)
        {
        }

        public void FrameSyncedUpdate(float deltaTime)
        {
        }

        public void Init()
        {
        }

        public List<Action> GetInitJobs(List<Action> list)
        {
            list.Add(Init);
            return list;
        }

        public void Load()
        {
        }

        public List<Action> GetLoadJobs(List<Action> list, EventHandler reloader)
        {
            list.Add(Load);
            NeedsLoad = reloader;
            return list;
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public bool Loaded
        {
            get { throw new NotImplementedException(); }
        }

        public EventHandler NeedsLoad
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Pencil.Gaming.MathUtils.Vector3 Position
        {
            get { throw new NotImplementedException(); }
        }

        public Pencil.Gaming.MathUtils.Vector3 Rotation
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
