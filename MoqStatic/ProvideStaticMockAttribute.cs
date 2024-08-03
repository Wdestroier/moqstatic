using System;

namespace Moq
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProvideStaticMockAttribute : Attribute
    {
        private Type StaticType;
        private StaticMockBehavior Behavior;
        private bool CallBase;

        public ProvideStaticMockAttribute(
            Type type,
            StaticMockBehavior behavior = StaticMockBehavior.Loose,
            bool callBase = false)
        {
            StaticType = type;
            Behavior = behavior;
            CallBase = callBase;
        }
    }

    public enum StaticMockBehavior
    {
        Strict,
        Loose
    }
}