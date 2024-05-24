using System;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;

namespace SparseVsDenseHashSet
{
    public static class ClassInvoker
    {
        private static ProfilerMarker profilerMarker = new("ReflectionInvocation");
#if UNITY_EDITOR
        [MenuItem("Tools/Generate And Invoke")]
#endif
        public static void GenerateClasses()
        {
            const int count = 10;
            ClassGenerator.GenerateClasses(count);
        }

#if UNITY_EDITOR
        [MenuItem("Tools/Invoke Generated Methods")]
#endif
        public static void InvokeGeneratedMethods()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name != "SparseVsDenseHashSet")
                {
                    continue;
                }

                profilerMarker.Begin();
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && type.Name.StartsWith("TestClass"))
                    {
                        var constructor = type.GetConstructor(Type.EmptyTypes);
                        if (constructor != null)
                        {
                            var instance = constructor.Invoke(null);

                            var method = type.GetMethod("MyMethod");
                            if (method != null)
                            {
                                method.Invoke(instance, null);
                            }
                            else
                            {
                                Debug.LogError($"Method MyMethod not found in {type.Name}.");
                            }
                        }
                        else
                        {
                            Debug.LogError($"{type.Name} does not have a parameterless constructor.");
                        }
                    }
                }

                profilerMarker.End();
            }
        }
    }
}