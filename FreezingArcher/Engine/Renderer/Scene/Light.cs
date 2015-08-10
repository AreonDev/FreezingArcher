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

        public Color4 AmbientColor
        {
            get
            {
                return new Color4(_AmbientColorR, _AmbientColorG, _AmbientColorB, _AmbientColorA);
            }

            set
            {
                _AmbientColorR = value.R;
                _AmbientColorG = value.G;
                _AmbientColorB = value.B;
                _AmbientColorA = value.A;
            }
        }

        public float AmbientIntensity 
        {
            get
            {
                return _AmbientIntensity;
            }

            set
            {
                _AmbientIntensity = value;
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

        float _LightColorB;
        float _LightColorG;
        float _LightColorR;
        float _LightColorA;

        float _AmbientColorR;
        float _AmbientColorG;
        float _AmbientColorB;
        float _AmbientColorA;

        float _AmbientIntensity;

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

        public Color4 AmbientColor
        {
            get
            {
                return _Definition.AmbientColor;
            }

            set
            {
                _Definition.AmbientColor = value;
            }
        }

        public float  AmbientIntensity  
        {
            get
            {
                return _Definition.AmbientIntensity;
            }

            set
            {
                _Definition.AmbientIntensity = value;
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
            }
        }

        public float SpotLightConeCosine
        {
            get
            {
                return _Definition.SpotLightConeCosine;
            }


            set
            {
                _Definition.SpotLightConeCosine = value;
            }
        }

        public Light(LightType type)
        {
            _Definition = new LightDefinition();

            Color = Color4.White;
            AmbientIntensity = 1.0f;
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
