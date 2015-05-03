using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FreezingArcher.Math;

namespace FreezingArcher.Renderer.Scene
{
    class BasicCamera : ICamera
    {
        public BasicCamera(int width, int height)
        {
            m_InternalWidth = width;
            m_InternalHeight = height;

            m_InternalPosition = new Vector3(0.0f, 0.0f, 0.0f);
            m_InternalLookAt = new Vector3(0.0f, 0.0f, -1.0f);
            Up = Vector3.UnitY;

            m_InternalNear = 1.0f;
            m_InternalFar = 100.0f;
            m_InternalAspectRatio = (float)m_InternalWidth / (float)m_InternalHeight;
            m_InternalFieldOfView = (float)System.Math.PI / 4.0f;
            ProjectionMode = BasicCameraProjectionMode.PerspectiveFieldOfView;
        }

        public enum BasicCameraProjectionMode
        {
            PerspectiveFieldOfView,
            Orthogonal
        };

        private Vector3 m_InternalPosition;
        private Vector3 m_InternalLookAt;
        private Vector3 m_InternalUp;

        private float m_InternalNear;
        private float m_InternalFar;
        private float m_InternalAspectRatio;
        private float m_InternalFieldOfView;
        private BasicCameraProjectionMode m_InternalProjectionMode;

        private int m_InternalWidth;
        private int m_InternalHeight;

        #region Interface have to
        public Matrix ProjectionMatrix { get; internal set; }
        public Matrix ViewMatrix { get; internal set; }

        public int Width
        {
            get
            {
                return m_InternalWidth;
            }
            set
            {
                m_InternalWidth = value;

                if (AutomaticAspectRatio)
                    m_InternalAspectRatio = (float)m_InternalWidth / (float)m_InternalHeight;

                switch(ProjectionMode)
                {
                    case BasicCameraProjectionMode.PerspectiveFieldOfView:
                        ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(m_InternalFieldOfView, m_InternalAspectRatio, m_InternalNear, m_InternalFar);
                        break;

                    case BasicCameraProjectionMode.Orthogonal:
                        ProjectionMatrix = Matrix.CreateOrthographic(m_InternalWidth, m_InternalHeight, m_InternalNear, m_InternalFar);
                        break;
                }
            }
        }
        
        public int Height 
        { 
            get
            {
                return m_InternalHeight;
            }
            set
            {
                m_InternalHeight = value;

                if (AutomaticAspectRatio)
                    m_InternalAspectRatio = (float)m_InternalWidth / (float)m_InternalHeight;

                switch(ProjectionMode)
                {
                    case BasicCameraProjectionMode.PerspectiveFieldOfView:
                        ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(m_InternalFieldOfView, m_InternalAspectRatio, m_InternalNear, m_InternalFar);
                        break;

                    case BasicCameraProjectionMode.Orthogonal:
                        ProjectionMatrix = Matrix.CreateOrthographic(m_InternalWidth, m_InternalHeight, m_InternalNear, m_InternalFar);
                        break;
                }
            }
        }
        #endregion

        #region ViewMatrix
        public Vector3 Position
        {
            get
            {
                return m_InternalPosition;
            }

            set
            {
                ViewMatrix = Matrix.LookAt(value, m_InternalLookAt, m_InternalUp);
                m_InternalPosition = value;
            }
        }

        public Vector3 LookAt
        {
            get
            {
                return m_InternalLookAt;
            }

            set
            {
                ViewMatrix = Matrix.LookAt(m_InternalPosition, value, m_InternalUp);
                m_InternalLookAt = value;
            }
        }

        public Vector3 Up
        {
            get
            {
                return m_InternalUp;
            }

            set
            {
                ViewMatrix = Matrix.LookAt(m_InternalPosition, m_InternalLookAt, value);
                m_InternalUp = value;
            }
        }
        #endregion
        #region ProjectionMatrix
        public BasicCameraProjectionMode ProjectionMode 
        { 
            get
            {
                return m_InternalProjectionMode;
            }
            set
            {
                m_InternalProjectionMode = value;

                switch(value)
                {
                    case BasicCameraProjectionMode.PerspectiveFieldOfView:
                        ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(m_InternalFieldOfView, m_InternalAspectRatio, m_InternalNear, m_InternalFar);
                        break;

                    case BasicCameraProjectionMode.Orthogonal:
                        ProjectionMatrix = Matrix.CreateOrthographic(m_InternalWidth, m_InternalHeight, m_InternalNear, m_InternalFar);
                        break;
                }
            }
        }
        public bool AutomaticAspectRatio { get; set; }
        public float AspectRatio
        {
            get
            {
                return m_InternalAspectRatio;
            }

            set
            {
                AutomaticAspectRatio = false;
                m_InternalAspectRatio = value;

                switch(ProjectionMode)
                {
                    case BasicCameraProjectionMode.PerspectiveFieldOfView:
                        ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(m_InternalFieldOfView, m_InternalAspectRatio, m_InternalNear, m_InternalFar);
                        break;

                    case BasicCameraProjectionMode.Orthogonal:
                        ProjectionMatrix = Matrix.CreateOrthographic(m_InternalWidth, m_InternalHeight, m_InternalNear, m_InternalFar);
                        break;
                }
            }
        }
        public float FieldOfView
        { 
            get
            {
                return m_InternalFieldOfView;
            }

            set
            {
                m_InternalFieldOfView = value;

                switch(ProjectionMode)
                {
                    case BasicCameraProjectionMode.PerspectiveFieldOfView:
                        ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(m_InternalFieldOfView, m_InternalAspectRatio, m_InternalNear, m_InternalFar);
                        break;

                    case BasicCameraProjectionMode.Orthogonal:
                        ProjectionMatrix = Matrix.CreateOrthographic(m_InternalWidth, m_InternalHeight, m_InternalNear, m_InternalFar);
                        break;
                }
            }
        }
        public float Near
        {
            get
            {
                return m_InternalNear;
            }

            set
            {
                m_InternalNear = value;

                switch(ProjectionMode)
                {
                    case BasicCameraProjectionMode.PerspectiveFieldOfView:
                        ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(m_InternalFieldOfView, m_InternalAspectRatio, m_InternalNear, m_InternalFar);
                        break;

                    case BasicCameraProjectionMode.Orthogonal:
                        ProjectionMatrix = Matrix.CreateOrthographic(m_InternalWidth, m_InternalHeight, m_InternalNear, m_InternalFar);
                        break;
                }
            }
        }

        public float Far
        {
            get
            {
                return m_InternalFar;
            }

            set
            {
                m_InternalFar = value;

                switch(ProjectionMode)
                {
                    case BasicCameraProjectionMode.PerspectiveFieldOfView:
                        ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(m_InternalFieldOfView, m_InternalAspectRatio, m_InternalNear, m_InternalFar);
                        break;

                    case BasicCameraProjectionMode.Orthogonal:
                        ProjectionMatrix = Matrix.CreateOrthographic(m_InternalWidth, m_InternalHeight, m_InternalNear, m_InternalFar);
                        break;
                }
            }
        }

        #endregion
    }
}
