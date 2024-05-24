using System;
using System.IO;
using System.Text;
using UnityEditor;
using Zenject; // Make sure you have the Zenject namespace available

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
        var installerBuilder = new StringBuilder();

        var testInjectBuilder = new StringBuilder();
        testInjectBuilder.AppendLine("public class TestInject");
        testInjectBuilder.AppendLine("{");
        
        // Add fields and constructor parameters for all TestClassX classes
        for (int i = 0; i < count; i++)
        {
            testInjectBuilder.AppendLine($"    private TestClass{i} _testClass{i};");
        }

        testInjectBuilder.Append("    public TestInject(");
        for (int i = 0; i < count; i++)
        {
            testInjectBuilder.Append($"TestClass{i} testClass{i}");
            if (i < count - 1)
                testInjectBuilder.Append(", ");
        }
        
        testInjectBuilder.AppendLine(")");
        testInjectBuilder.AppendLine("    {");

        // Assign constructor parameters to the fields
        for (int i = 0; i < count; i++)
        {
            testInjectBuilder.AppendLine($"        _testClass{i} = testClass{i};");
        }

        testInjectBuilder.AppendLine("    }");
        testInjectBuilder.AppendLine("}");

        // Write the TestInject class to its own file
        File.WriteAllText(Path.Combine(directory, "TestInject.cs"), testInjectBuilder.ToString());
        
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

            // Add the binding for the current TestClass to the installer
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

        // Finalize the TestInstaller class
        
        installerBuilder.AppendLine("        Container.Bind<TestInject>().AsSingle().NonLazy();\n");
        installerBuilder.AppendLine("    }");
        installerBuilder.AppendLine("}");

        // Write the TestInstaller class to its own file
        File.WriteAllText(Path.Combine(directory, "TestInstaller.cs"), installerBuilder.ToString());
    }
}