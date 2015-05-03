using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreezingArcher.Messaging;
using FreezingArcher.Math;

using Pencil.Gaming.MathUtils;

namespace FreezingArcher.Renderer.Scene
{
    public class InteractivePerspectiveCamera : ICamera, Messaging.Interfaces.IMessageConsumer
    {
        protected int m_InternalWidth;
        protected int m_InternalHeight;

        protected FreezingArcher.Math.Vector3 m_InternalPosition;
        protected FreezingArcher.Math.Vector3 m_InternalLookAt;
        protected FreezingArcher.Math.Vector3 m_InternalUp;
        protected FreezingArcher.Math.Vector3 m_InternalDirection;

        protected float m_InternalFieldOfView;
        protected float m_InternalAspectRatio;
        protected float m_InternalNear;
        protected float m_InternalFar;

        public InteractivePerspectiveCamera(int start_width, int start_height, MessageManager mssgmngr)
        {
            ValidMessages = new int[] { (int)MessageId.WindowResizeMessage };
            mssgmngr += this;

            m_InternalWidth = start_width;
            m_InternalHeight = start_height;

            m_InternalPosition = new FreezingArcher.Math.Vector3(0.0f, 0.0f, 0.0f);
            m_InternalLookAt = new FreezingArcher.Math.Vector3(0.0f, 0.0f, -1.0f);
            m_InternalUp = FreezingArcher.Math.Vector3.UnitY;
            m_InternalDirection = FreezingArcher.Math.Vector3.Normalize(m_InternalLookAt - m_InternalPosition);

            m_InternalNear = 1.0f;
            m_InternalFar = 100.0f;
            m_InternalAspectRatio = (float)m_InternalWidth / (float)m_InternalHeight;
            m_InternalFieldOfView = (float)System.Math.PI / 4.0f;

            AutomaticSize = true;

            UpdateViewMatrix();
            UpdateProjectionMatrix();
        }

        public FreezingArcher.Math.Matrix ProjectionMatrix { get; protected set; }

        public FreezingArcher.Math.Matrix ViewMatrix { get; protected set; }

        public bool AutomaticSize { get; set; }

        public int Width
        {
            get
            {
                return m_InternalWidth;
            }
            set
            {
                m_InternalWidth = value;
                m_InternalAspectRatio = (float)m_InternalWidth / (float)m_InternalHeight;

                UpdateProjectionMatrix();
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
                m_InternalAspectRatio = (float)m_InternalWidth / (float)m_InternalHeight;

                UpdateProjectionMatrix();
            }
        }

        public virtual FreezingArcher.Math.Vector3 Position
        {
            get
            {
                return m_InternalPosition;
            }

            set
            {
                m_InternalPosition = value;
                m_InternalDirection = FreezingArcher.Math.Vector3.Normalize(m_InternalLookAt - m_InternalPosition);

                UpdateViewMatrix();
            }
        }

        public virtual FreezingArcher.Math.Vector3 Direction
        {
            get
            {
                return m_InternalDirection;
            }

            set
            {
                m_InternalDirection = FreezingArcher.Math.Vector3.Normalize(value);
                m_InternalLookAt = m_InternalPosition + m_InternalDirection;

                UpdateViewMatrix();
            }
        }

        public virtual FreezingArcher.Math.Vector3 LookAt
        {
            get
            {
                return m_InternalLookAt;
            }

            set
            {
                m_InternalLookAt = value;
                m_InternalDirection = FreezingArcher.Math.Vector3.Normalize(m_InternalLookAt - m_InternalPosition);

                UpdateViewMatrix();
            }
        }

        public virtual FreezingArcher.Math.Vector3 Up
        {
            get
            {
                return m_InternalUp;
            }

            set
            {
                m_InternalUp = value;
                UpdateViewMatrix();
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
                UpdateProjectionMatrix();
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
                UpdateProjectionMatrix();
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
                UpdateProjectionMatrix();
            }
        }

        public virtual void ConsumeMessage(Messaging.Interfaces.IMessage msg)
        {
            WindowResizeMessage wrm = msg as WindowResizeMessage;
            if(wrm != null)
            {
                if (AutomaticSize)
                {
                    m_InternalWidth = wrm.Width;
                    m_InternalHeight = wrm.Height;

                    m_InternalAspectRatio = (float)m_InternalWidth / (float)m_InternalHeight;

                    UpdateProjectionMatrix();
                }
            }
        }

        public virtual int[] ValidMessages { get; protected set; }

        protected virtual void UpdateProjectionMatrix()
        {
            ProjectionMatrix = FreezingArcher.Math.Matrix.CreatePerspectiveFieldOfView(m_InternalFieldOfView, m_InternalAspectRatio, m_InternalNear, m_InternalFar);
        }

        protected virtual void UpdateViewMatrix()
        {
            ViewMatrix = FreezingArcher.Math.Matrix.LookAt(m_InternalPosition, m_InternalLookAt, m_InternalUp);
        }
    }
}
