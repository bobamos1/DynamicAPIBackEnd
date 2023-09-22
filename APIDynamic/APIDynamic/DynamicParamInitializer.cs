﻿namespace APIDynamic
{
    public class DynamicParamInitializer
    {
        public string AssociatedVarName { get; set; }
        public object Value { get; set; }
        public bool IsStatic { get; set; }
        public DynamicParamInitializer(string AssociatedVarName, string Value, long CSharpType)
        {
            this.AssociatedVarName = AssociatedVarName;
            this.Value = SetValue(Value, (CSharpTypes)CSharpType);
        }
        public object SetValue(string value, CSharpTypes CSharpType)
        {
            if (CSharpType == CSharpTypes.REFERENCE)
            {
                this.IsStatic = false;
                return value;
            }
            this.IsStatic = true;
            return value;
        }
    }
}
