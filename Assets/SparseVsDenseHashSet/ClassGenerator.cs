using System;
using System.IO;
using System.Text;
using UnityEditor;

namespace SparseVsDenseHashSet
{
    public static class ClassGenerator
    {
        private static readonly string[] BaseTypes = { "int", "float", "bool", "char", "string" };
        private static readonly Random Random = new();

#if UNITY_EDITOR
        [MenuItem("Tools/Generate")]
#endif
        public static void GenerateClasses()
        {
            GenerateClasses(1, "Assets/SparseVsDenseHashSet/Generated/TestClass");
        }

        public static void GenerateClasses(int count)
        {
            GenerateClasses(count, "Assets/SparseVsDenseHashSet/Generated/TestClass");
        }

        private static string GenerateClass(int index)
        {
            var fieldType = BaseTypes[Random.Next(BaseTypes.Length)];
            var className = $"TestClass{index}";
            var classBuilder = new StringBuilder();

            classBuilder.AppendLine($"public class {className}");
            classBuilder.AppendLine("{");
            classBuilder.AppendLine($"    private {fieldType} field;");
            classBuilder.AppendLine();
            classBuilder.AppendLine("    public void MyMethod()");
            classBuilder.AppendLine("    {");
            classBuilder.AppendLine("        var localVar = field;");
            classBuilder.AppendLine("    }");
            classBuilder.AppendLine("}");
            classBuilder.AppendLine();

            return classBuilder.ToString();
        }

        private static void GenerateClasses(int count, string baseFilePath)
        {
            var directory = Path.GetDirectoryName(baseFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            foreach (var file in Directory.EnumerateFiles(directory, "*.cs"))
            {
                File.Delete(file);
            }

            var fileIndex = 0;
            var fileBuilder = new StringBuilder();
            var filePath = $"{baseFilePath}{fileIndex}.cs";

            for (var i = 0; i < count; i++)
            {
                var classContent = GenerateClass(i);
                fileBuilder.Append(classContent);

                if (fileBuilder.Length >= 100000)
                {
                    File.WriteAllText(filePath, fileBuilder.ToString());
                    fileBuilder.Clear();

                    fileIndex++;
                    filePath = $"{baseFilePath}{fileIndex}.cs";
                }
            }

            if (fileBuilder.Length > 0)
            {
                File.WriteAllText(filePath, fileBuilder.ToString());
            }
        }
    }
}