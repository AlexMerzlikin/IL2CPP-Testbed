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

            CleanUpExistingFiles(directory);

            var fileIndex = 0;
            var filePath = $"{baseFilePath}{fileIndex}.cs";
            var fileBuilder = new StringBuilder();
            var installerBuilder = new StringBuilder();

            GenerateTestInjectClasses(count, directory);
            GenerateTestInstallerClass(count, baseFilePath, directory, ref fileIndex, ref filePath, ref fileBuilder, installerBuilder);
        }

        private static void CleanUpExistingFiles(string directory)
        {
            foreach (var file in Directory.EnumerateFiles(directory, "*.cs"))
            {
                File.Delete(file);
            }
        }

        private static void GenerateTestInjectClasses(int totalClasses, string directory)
        {
            var classesPerInject = 20;
            var injectClassCount = (totalClasses + classesPerInject - 1) / classesPerInject;

            for (var injectIndex = 0; injectIndex < injectClassCount; injectIndex++)
            {
                var testInjectBuilder = new StringBuilder();
                testInjectBuilder.AppendLine($"public class TestInject{injectIndex}");
                testInjectBuilder.AppendLine("{");

                var start = injectIndex * classesPerInject;
                var end = Math.Min(start + classesPerInject, totalClasses);

                for (var i = start; i < end; i++)
                {
                    testInjectBuilder.AppendLine($"    private TestClass{i} _testClass{i};");
                }

                testInjectBuilder.Append($"    public TestInject{injectIndex}(");
                for (var i = start; i < end; i++)
                {
                    testInjectBuilder.Append($"TestClass{i} testClass{i}");
                    if (i < end - 1)
                    {
                        testInjectBuilder.Append(", ");
                    }
                }

                testInjectBuilder.AppendLine(")");

                testInjectBuilder.AppendLine("    {");
                for (var i = start; i < end; i++)
                {
                    testInjectBuilder.AppendLine($"        _testClass{i} = testClass{i};");
                }

                testInjectBuilder.AppendLine("    }");
                testInjectBuilder.AppendLine("}");

                var testInjectFileName = $"TestInject{injectIndex}.cs";
                File.WriteAllText(Path.Combine(directory, testInjectFileName), testInjectBuilder.ToString());
            }
        }

        private static void GenerateTestInstallerClass(
            int count,
            string baseFilePath,
            string directory,
            ref int fileIndex,
            ref string filePath,
            ref StringBuilder fileBuilder,
            StringBuilder installerBuilder)
        {
            installerBuilder.AppendLine("using Zenject;");
            installerBuilder.AppendLine();
            installerBuilder.AppendLine("public class TestInstaller : MonoInstaller");
            installerBuilder.AppendLine("{");
            installerBuilder.AppendLine("    public override void InstallBindings()");
            installerBuilder.AppendLine("    {");

            for (var i = 0; i < count; i++)
            {
                var classContent = GenerateClass(i);
                fileBuilder.Append(classContent);

                installerBuilder.AppendLine($"        Container.Bind<TestClass{i}>().AsSingle().NonLazy();");

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

            var classesPerInject = 20;
            var injectClassCount = (count + classesPerInject - 1) / classesPerInject;

            for (var injectIndex = 0; injectIndex < injectClassCount; injectIndex++)
            {
                installerBuilder.AppendLine($"        Container.Bind<TestInject{injectIndex}>().AsSingle().NonLazy();");
            }

            installerBuilder.AppendLine("    }");
            installerBuilder.AppendLine("}");

            File.WriteAllText(Path.Combine(directory, "TestInstaller.cs"), installerBuilder.ToString());
        }
    }
}