using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreezingArcher.Renderer
{
    internal class GraphicsResourceManager
    {
        #region Private Members
        private List<GraphicsResource> _GraphicsResources;
        #endregion

        #region Internal Properties
        internal int TotalCountOfGraphicsResources { get { return _GraphicsResources.Count; } }
        #endregion

        #region Internal Methods
        internal GraphicsResourceManager()
        {
            _GraphicsResources = new List<GraphicsResource>();
        }

        internal void AddResource(GraphicsResource resource)
        {
            foreach (GraphicsResource r in _GraphicsResources)
            {
                if (r.Name == resource.Name)
                    throw new Exception("Resource with name \"" + r.Name + "\" is already existing!");

                if (!resource.Created)
                    throw new Exception("Resource with name \"" + resource.Name + "\" is not created!");
            }

            _GraphicsResources.Add(resource);
        }

        internal void DeleteResource(GraphicsResource resource)
        {
            if (resource.InternalUseCount > 0)
                throw new Exception("Resource with name \"" + resource.Name + "\" is still used! InternalUseCount: " + resource.InternalUseCount);

            _GraphicsResources.Remove(resource);
        }

        internal GraphicsResource[] GetAllResources()
        {
            return _GraphicsResources.ToArray();
        }

        internal GraphicsResource GetGraphicsResourceByName(string name)
        {
            foreach (GraphicsResource r in _GraphicsResources)
            {
                if (r.Name == name)
                    return r;
            }

            throw new Exception("GraphicsResource with name \"" + name + "\" is not existing!");
        }

        #endregion
    }
}
