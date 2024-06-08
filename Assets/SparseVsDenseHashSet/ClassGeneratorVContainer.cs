using System;
using System.IO;
using System.Text;
using UnityEditor;

namespace SparseVsDenseHashSet
{
    public static class ClassGeneratorVContainer
    {
        private static readonly string[] BaseTypes = { "int", "float", "bool", "char", "string" };
        private static readonly Random Random = new();

#if UNITY_EDITOR
        [MenuItem("Tools/GenerateVContainer")]
#endif
        public static void GenerateClasses()
        {
            GenerateClasses(1, "Assets/SparseVsDenseHashSet/GeneratedVContainer/TestClass");
        }

        public static void GenerateClasses(int count)
        {
            GenerateClasses(count, "Assets/SparseVsDenseHashSet/GeneratedVContainer/TestClass");
        }

        private static string GenerateClass(int index)
        {
            var fieldType = BaseTypes[Random.Next(BaseTypes.Length)];
            var className = $"TestClassVContainer{index}";
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
                testInjectBuilder.AppendLine($"public class TestInjectVContainer{injectIndex}");
                testInjectBuilder.AppendLine("{");

                var start = injectIndex * classesPerInject;
                var end = Math.Min(start + classesPerInject, totalClasses);

                for (var i = start; i < end; i++)
                {
                    testInjectBuilder.AppendLine($"    private TestClassVContainer{i} _testClassVContainer{i};");
                }

                testInjectBuilder.Append($"    public TestInjectVContainer{injectIndex}(");
                for (var i = start; i < end; i++)
                {
                    testInjectBuilder.Append($"TestClassVContainer{i} testClassVContainer{i}");
                    if (i < end - 1)
                    {
                        testInjectBuilder.Append(", ");
                    }
                }

                testInjectBuilder.AppendLine(")");

                testInjectBuilder.AppendLine("    {");
                for (var i = start; i < end; i++)
                {
                    testInjectBuilder.AppendLine($"        _testClassVContainer{i} = testClassVContainer{i};");
                }

                testInjectBuilder.AppendLine("    }");
                testInjectBuilder.AppendLine("}");

                var testInjectFileName = $"TestInjectVContainer{injectIndex}.cs";
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
            installerBuilder.AppendLine("using VContainer;");
            installerBuilder.AppendLine("using VContainer.Unity;");
            installerBuilder.AppendLine("using VContainer.Unity;");
            installerBuilder.AppendLine();
            installerBuilder.AppendLine("public class TestInstallerVContainer : LifetimeScope");
            installerBuilder.AppendLine("{");
            installerBuilder.AppendLine("    protected override void Configure(IContainerBuilder builder)");
            installerBuilder.AppendLine("    {");

            for (var i = 0; i < count; i++)
            {
                var classContent = GenerateClass(i);
                fileBuilder.Append(classContent);

                installerBuilder.AppendLine($"        builder.Register<TestClassVContainer{i}>(Lifetime.Singleton);");

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
                installerBuilder.AppendLine($"        builder.Register<TestInjectVContainer{injectIndex}>(Lifetime.Singleton);");
            }

            installerBuilder.AppendLine("    }");
            installerBuilder.AppendLine("}");

            File.WriteAllText(Path.Combine(directory, "TestInstallerVContainer.cs"), installerBuilder.ToString());
        }
    }
}
