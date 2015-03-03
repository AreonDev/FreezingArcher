using System;
using FreezingArcher.Settings;
using System.Collections.Generic;

namespace FreezingArcher.Settings
{
    public class PropertyTypes
    {
        public static Dictionary<Type, Types[]> TypeRegistry { get; set; }

        static PropertyTypes ()
        {
            TypeRegistry = new Dictionary<Type, Types[]> ();
            TypeRegistry.Add (typeof (IntegerValue), IntegerPropertyTypes);
            TypeRegistry.Add (typeof (FloatValue), FloatPropertyTypes);
            TypeRegistry.Add (typeof (StringValue), StringPropertyTypes);
            TypeRegistry.Add (typeof (ReferencedValue), ReferencePropertyTypes);
            TypeRegistry.Add (typeof (ArrayValue), ArrayPropertyTypes);
        }

        static Types[] IntegerPropertyTypes =
        {
            Types.SpinField,
            Types.NotchedSlider,
            Types.Dropdown,
            Types.SingleSelectList,
            Types.Checkbox,
            Types.Radiobutton
        };

        static Types[] FloatPropertyTypes =
        {
            Types.Slider,
            Types.NotchedSlider
        };

        static Types[] StringPropertyTypes =
        {
            Types.TextInput,
            Types.PasswordInput,
            Types.Dropdown,
            Types.Radiobutton,
            Types.SingleSelectList
        };

        static Types[] ReferencePropertyTypes =
        {
            Types.Dropdown,
            Types.Radiobutton,
            Types.SingleSelectList
        };

        static Types[] ArrayPropertyTypes =
        {
            Types.GroupBox
        };

        public enum Types
        {
            SpinField,
            NotchedSlider,
            Slider,
            Dropdown,
            Checkbox,
            Radiobutton,
            TextInput,
            PasswordInput,
            SingleSelectList,
            GroupBox
        }

    }
}
