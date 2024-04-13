using System;

namespace StructAsDictionaryKey
{
    public struct StructKeyIEquatable : IEquatable<StructKeyIEquatable>
    {
        public int Value;

        public bool Equals(StructKeyIEquatable other) => Value == other.Value;

        public override int GetHashCode() => Value;
    }
}