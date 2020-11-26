using System;

namespace BonkInterfacing
{
    [Serializable]
    public class CompoundKeyValueSet<TPrimaryKey, TSecondaryKey, TValue>
    {
        public TPrimaryKey PrimaryKey { get; set; }
        public TSecondaryKey SecondaryKey { get; set; }
        public TValue Value { get; set; }

        public CompoundKeyValueSet(TPrimaryKey primaryKey, TSecondaryKey secondaryKey, TValue value)
        {
            PrimaryKey = primaryKey;
            SecondaryKey = secondaryKey;
            Value = value;
        }
        public CompoundKeyValueSet()
        {

        }

        public override string ToString()
        {
            return $"{PrimaryKey},{SecondaryKey},{Value}";
        }
    }
}
