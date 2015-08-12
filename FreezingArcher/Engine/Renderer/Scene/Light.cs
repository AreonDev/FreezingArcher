using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FreezingArcher.Math;

namespace FreezingArcher.Renderer.Scene
{
    public enum LightType : int
    {
        DirectionalLight = 0,
        PointLight = 1,
        SpotLight = 2
    }

    public struct LightDefinition
    {
        public LightType Type
        {
            get
            {
                return (LightType)_Type;
            }

            set
            {
                _Type = (int)value;
            }
        }

        public bool On 
        {
            get
            {
                return _On == 1;
            }

            set
            {
                _On = value ? 1 : 0;
            }
        }

        public Color4 LightColor
        {
            get
            {
                return new Color4(_LightColorR, _LightColorG, _LightColorB, _LightColorA);
            }

            set
            {
                _LightColorR = value.R;
                _LightColorG = value.G;
                _LightColorB = value.B;
                _LightColorA = value.A;
            }
        }
            
        public Vector3 DirectionalLightDirection
        {
            get
            {
                return new Vector3(_LightDirectionX, _LightDirectionY,
                    _LightDirectionZ);
            }

            set
            {
                _LightDirectionX = value.X;
                _LightDirectionY = value.Y;
                _LightDirectionZ = value.Z;
            }
        }

        public Vector3 PointLightPosition
        {
            get
            {
                return new Vector3(_LightPositionX, _LightPositionY, _LightPositionZ);
            }

            set
            {
                _LightPositionX = value.X;
                _LightPositionY = value.Y;
                _LightPositionZ = value.Z;
            }
        }

        public float PointLightConstantAttenuation
        {
            get
            {
                return _LightPointLightConstantAtt;
            }

            set
            {
                _LightPointLightConstantAtt = value;
            }
        }

        public float PointLightLinearAttenuation
        {
            get
            {
                return _LightPointLightLinearAtt;
            }

            set
            {
                _LightPointLightLinearAtt = value;
            }
        }

        public float PointLightExponentialAttenuation
        {
            get
            {
                return _LightPointLightExpAtt;
            }

            set
            {
                _LightPointLightExpAtt = value;
            }
        }

        public float SpotLightConeAngle
        {
            get
            {
                return _SpotLightConeAngle;
            }

            set
            {
                _SpotLightConeAngle = value;
            }
        }

        public float SpotLightConeCosine
        {
            get
            {
                return _SpotLightConeCosine;
            }

            set
            {
                _SpotLightConeCosine = value;   
            }
        }

        int _Type;

        int _On;

        float _LightColorB;
        float _LightColorG;
        float _LightColorR;
        float _LightColorA;

        //Directional Light
        float _LightDirectionX;
        float _LightDirectionY;
        float _LightDirectionZ;
        float _LightDirectionW;

        //Point Light
        float _LightPositionX;
        float _LightPositionY;
        float _LightPositionZ;
        float _LightPositionW;

        float _LightPointLightConstantAtt;
        float _LightPointLightLinearAtt;
        float _LightPointLightExpAtt;

        //Spot Light
        float _SpotLightConeAngle;
        float _SpotLightConeCosine;
    }

    public class Light
    {
        internal LightDefinition _Definition;

        public LightType Type 
        {
            get
            {
                return _Definition.Type;
            }

            private set
            {
                _Definition.Type = value;
            }
        }

        public bool On
        {
            get
            {
                return _Definition.On;
            }

            set
            {
                _Definition.On = value;
            }
        }

        public Color4 Color 
        { 
            get
            {
                return _Definition.LightColor;
            }

            set
            {
                _Definition.LightColor = value;
            }
        }

        public Vector3 DirectionalLightDirection 
        {
            get
            {
                return _Definition.DirectionalLightDirection;
            }

            set
            {
                _Definition.DirectionalLightDirection = value;
            }
        }

        public Vector3 PointLightPosition
        {
            get
            {
                return _Definition.PointLightPosition;
            }

            set
            {
                _Definition.PointLightPosition = value;
            }
        }

        public float PointLightConstantAttenuation
        {
            get
            {
                return _Definition.PointLightConstantAttenuation;
            }

            set
            {
                _Definition.PointLightConstantAttenuation = value;
            }
        }

        public float PointLightLinearAttenuation
        {
            get
            {
                return _Definition.PointLightLinearAttenuation;
            }

            set
            {
                _Definition.PointLightLinearAttenuation = value;
            }
        }

        public float PointLightExponentialAttenuation
        {
            get
            {
                return _Definition.PointLightExponentialAttenuation;
            }

            set
            {
                _Definition.PointLightExponentialAttenuation = value;
            }
        }

        public float SpotLightConeAngle
        {
            get
            {
                return _Definition.SpotLightConeAngle;
            }


            set
            {
                _Definition.SpotLightConeAngle = value;
                _Definition.SpotLightConeCosine = (float)System.Math.Cos(SpotLightConeAngle);
            }
        }

        public float SpotLightConeCosine
        {
            get
            {
                return _Definition.SpotLightConeCosine;
            }


            private set
            {
                _Definition.SpotLightConeCosine = value;
            }
        }

        public Light(LightType type)
        {
            _Definition = new LightDefinition();

            On = true;

            Color = Color4.White;
            DirectionalLightDirection = new Vector3(1.0f, -1.0f, 0.0f);

            PointLightPosition = Vector3.Zero;
            PointLightConstantAttenuation = 0.01f;
            PointLightLinearAttenuation = 0.0167f;
            PointLightExponentialAttenuation = 0.00000f;

            SpotLightConeAngle = 13.66f;

            Type = type;
        }
    }
}
