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

        private ProfilerMarker _profilerMarker = new("IEquatable");
        private ProfilerMarker _profilerMarkerNoIEquatable = new("No IEquatable");
        
        private void Awake()
        {
            _structKeyIEquatableDictionary[_structKeyIEquatable] = 1;
            _structKeyNoIEquatableDictionary[_structKeyNoIEquatable] = 1;
        }

        private void Update()
        {
            _profilerMarker.Begin();
            var a = _structKeyIEquatableDictionary[_structKeyIEquatable];
            _profilerMarker.End();
            _profilerMarkerNoIEquatable.Begin();
            var b = _structKeyNoIEquatableDictionary[_structKeyNoIEquatable];
            _profilerMarkerNoIEquatable.End();
        }
    }
}