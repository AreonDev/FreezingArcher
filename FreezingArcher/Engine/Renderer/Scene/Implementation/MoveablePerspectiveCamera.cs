using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using FreezingArcher.Messaging;
using FreezingArcher.Input;

namespace FreezingArcher.Renderer.Scene
{
    public class MoveablePerspectiveCamera : InteractivePerspectiveCamera
    {
        public MoveablePerspectiveCamera(int width, int height, MessageManager mssgmngr)
            : base(width, height, mssgmngr)
        {
            mssgmngr.UnregisterMessageConsumer(this);

            ValidMessages = new int[] { (int)MessageId.WindowResizeMessage, (int)MessageId.Update };

            mssgmngr += this;

            MouseSensitivity = 5;
            MovementSpeed = 3f;

            InvertX = false;
            InvertY = false;

            m_InternalStartDirection = m_InternalDirection;
            m_InternalStartUp = m_InternalUp;

            RotationX = 0.0f;
            RotationY = 0.0f;
            RotationZ = 0.0f;

            KeyRegistry.Instance.RegisterKey("leftroll", Key.Q);
            KeyRegistry.Instance.RegisterKey("rightroll", Key.E);
        }

        private FreezingArcher.Math.Vector3 m_InternalStartDirection;
        private FreezingArcher.Math.Vector3 m_InternalStartUp;

        public int MouseSensitivity { get; set; }
        public bool InvertX { get; set; }
        public bool InvertY { get; set; }
        public float MovementSpeed { get; set; }

        public float RotationX { get; private set; }
        public float RotationY { get; private set; }
        public float RotationZ { get; private set; }

        protected override void UpdateViewMatrix()
        {
            ViewMatrix = FreezingArcher.Math.Matrix.LookAt(m_InternalPosition, m_InternalLookAt, m_InternalUp);
        }

        public override FreezingArcher.Math.Vector3 Position
        {
            get
            {
                return m_InternalPosition;
            }

            set
            {
                //Have to guess UP-Vector!
                //and first rotations

                //m_InternalPosition = value;
                //m_InternalDirection = Vector3.Normalize(m_InternalLookAt - m_InternalPosition);

                //UpdateViewMatrix();
            }
        }

        public override FreezingArcher.Math.Vector3 Direction
        {
            get
            {
                return m_InternalDirection;
            }

            set
            {
                //Have to guess UP-Vector!
                //and first rotations

                //m_InternalDirection = Vector3.Normalize(value);
                //m_InternalLookAt = m_InternalPosition + m_InternalDirection;

                //UpdateViewMatrix();
            }
        }

        public override FreezingArcher.Math.Vector3 LookAt
        {
            get
            {
                return m_InternalLookAt;
            }

            set
            {
                //Have to guess UP-Vector!
                //and first rotations

                //m_InternalLookAt = value;
                //m_InternalDirection = Vector3.Normalize(m_InternalLookAt - m_InternalPosition);

                //UpdateViewMatrix();
            }
        }

        public override FreezingArcher.Math.Vector3 Up
        {
            get
            {
                return m_InternalUp;
            }

            set
            {
                //BIG TROUBLE

                //m_InternalUp = value;
               //UpdateViewMatrix();
            }
        }

        private bool forward_down = false;
        private bool backward_down = false;
        private bool left_down = false;
        private bool right_down = false;
        private bool up_down = false;
        private bool down_down = false;
        private bool leftroll_down = false;
        private bool rightroll_down = false;

        public override void ConsumeMessage(Messaging.Interfaces.IMessage msg)
        {
            base.ConsumeMessage(msg);
            /*

            InputMessage um = msg as UpdateMessage;
            if (um != null)
            {
                #region MouseInput
                float mouse_x_delta = InvertX ? 1.0f : -1.0f * um.MouseMovement.X * MathHelper.ToRadians(MouseSensitivity / 5.0f) * (float)um.DeltaTime;
                float mouse_y_delta = InvertY ? -1.0f : 1.0f * um.MouseMovement.Y * MathHelper.ToRadians(MouseSensitivity / 5.0f) * (float)um.DeltaTime;
                #endregion

                #region KeyboardInput
                foreach (KeyboardInput ki in um.Keys)
                {
                    if (ki.KeyAction == "forward")
                    {
                        switch (ki.Action)
                        {
                            case KeyAction.Press:
                            case KeyAction.Repeat:
                                forward_down = true;
                                break;

                            case KeyAction.Release:
                                forward_down = false;
                                break;
                        }
                    }

                    if (ki.KeyAction == "backward")
                    {
                        switch (ki.Action)
                        {
                            case KeyAction.Press:
                            case KeyAction.Repeat:
                                backward_down = true;
                                break;

                            case KeyAction.Release:
                                backward_down = false;
                                break;
                        }
                    }

                    if (ki.KeyAction == "left")
                    {
                        switch (ki.Action)
                        {
                            case KeyAction.Press:
                            case KeyAction.Repeat:
                                left_down = true;
                                break;

                            case KeyAction.Release:
                                left_down = false;
                                break;
                        }
                    }

                    if (ki.KeyAction == "right")
                    {
                        switch (ki.Action)
                        {
                            case KeyAction.Press:
                            case KeyAction.Repeat:
                                right_down = true;
                                break;

                            case KeyAction.Release:
                                right_down = false;
                                break;
                        }
                    }

                    if (ki.KeyAction == "sneek")
                    {
                        switch (ki.Action)
                        {
                            case KeyAction.Press:
                            case KeyAction.Repeat:
                                up_down = true;
                                break;

                            case KeyAction.Release:
                                up_down = false;
                                break;
                        }
                    }

                    if (ki.KeyAction == "run")
                    {
                        switch (ki.Action)
                        {
                            case KeyAction.Press:
                            case KeyAction.Repeat:
                                down_down = true;
                                break;

                            case KeyAction.Release:
                                down_down = false;
                                break;
                        }
                    }

                    if (ki.KeyAction == "leftroll")
                    {
                        switch (ki.Action)
                        {
                            case KeyAction.Press:
                            case KeyAction.Repeat:
                                leftroll_down = true;
                                break;

                            case KeyAction.Release:
                                leftroll_down = false;
                                break;
                        }
                    }

                    if (ki.KeyAction == "rightroll")
                    {
                        switch (ki.Action)
                        {
                            case KeyAction.Press:
                            case KeyAction.Repeat:
                                rightroll_down = true;
                                break;

                            case KeyAction.Release:
                                rightroll_down = false;
                                break;
                        }
                    }
                }
                #endregion

                RotationX += mouse_x_delta;
                RotationY += mouse_y_delta;

                Quaternion quaternionRotation = Quaternion.FromAxisAngle(Vector3.UnitY, RotationX) * Quaternion.FromAxisAngle(Vector3.UnitX, RotationY) *
                    Quaternion.FromAxisAngle(Vector3.UnitZ, RotationZ);

                m_InternalDirection = Vector3.Transform(m_InternalStartDirection, quaternionRotation);
                m_InternalUp = Vector3.Transform(m_InternalStartUp, quaternionRotation);

                Vector3 right = Vector3.Normalize(Vector3.Cross(m_InternalUp, m_InternalDirection));

                if (forward_down)
                {
                    m_InternalPosition += MovementSpeed * (float)um.DeltaTime * m_InternalDirection;
                }

                if (backward_down)
                {
                    m_InternalPosition -= MovementSpeed * (float)um.DeltaTime * m_InternalDirection;
                }

                if (right_down)
                {
                    m_InternalPosition -= MovementSpeed * (float)um.DeltaTime * right;
                }

                if (left_down)
                {
                    m_InternalPosition += MovementSpeed * (float)um.DeltaTime * right;
                }

                if(up_down)
                {
                    m_InternalPosition += MovementSpeed * (float)um.DeltaTime * m_InternalUp;
                }

                if(down_down)
                {
                    m_InternalPosition -= MovementSpeed * (float)um.DeltaTime * m_InternalUp;
                }

                if(leftroll_down)
                {
                    RotationZ += MouseSensitivity * (float)um.DeltaTime;
                }

                if(rightroll_down)
                {
                    RotationZ -= MouseSensitivity * (float)um.DeltaTime;
                }

                m_InternalLookAt = m_InternalPosition + m_InternalDirection;

                UpdateViewMatrix();
            }
             */
        }
    }
}
