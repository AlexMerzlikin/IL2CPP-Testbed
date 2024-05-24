using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace StructAsDictionaryKey
{
    public class StructKeyTest : MonoBehaviour
    {
        private readonly StructKeyIEquatable _structKeyIEquatable = new();
        private readonly StructKeyNoIEquatable _structKeyNoIEquatable = new();

        private readonly Dictionary<StructKeyIEquatable, int> _structKeyIEquatableDictionary = new();
        private readonly Dictionary<StructKeyNoIEquatable, int> _structKeyNoIEquatableDictionary = new();
        private readonly Dictionary<TestEnum, int> _enumKeyDictionary = new();
        private readonly Dictionary<int, int> _intKeyDictionary = new();

        private ProfilerMarker _profilerMarker = new("Key_IEquatable");
        private ProfilerMarker _profilerMarkerNoIEquatable = new("Key_No IEquatable");
        private ProfilerMarker _profilerMarkerEnum = new("Key_Enum");

        private void Awake()
        {
            _structKeyIEquatableDictionary[_structKeyIEquatable] = 1;
            _structKeyNoIEquatableDictionary[_structKeyNoIEquatable] = 1;
            _enumKeyDictionary[TestEnum.None] = 1;
            _intKeyDictionary[0] = 1;
        }

        private void Update()
        {
            _profilerMarker.Begin();
            var a = _structKeyIEquatableDictionary[_structKeyIEquatable];
            _profilerMarker.End();

            _profilerMarkerNoIEquatable.Begin();
            var b = _structKeyNoIEquatableDictionary[_structKeyNoIEquatable];
            _profilerMarkerNoIEquatable.End();

            _profilerMarkerEnum.Begin();
            var c = _enumKeyDictionary[TestEnum.None];
            var e = _enumKeyDictionary[TestEnum.None];
            _profilerMarkerEnum.End();

            var d = _intKeyDictionary[0];
        }
    }
}