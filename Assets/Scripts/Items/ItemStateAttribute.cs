using System;

[AttributeUsage(AttributeTargets.Class)]
public class ItemStateAttribute : Attribute {
    public Type DataType { get; private set; }

    public ItemStateAttribute(Type type) {
        if (type.IsValueType || type.IsPrimitive || type.IsEnum) {
            throw new ArgumentException("The ItemState type must be a class.");
        }
        Activator.CreateInstance(type);

        DataType = type;
    }
}