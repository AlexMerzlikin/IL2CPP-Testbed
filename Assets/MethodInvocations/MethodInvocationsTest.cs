using System;
using UnityEngine;

namespace MethodInvocations
{
    public class MethodInvocationsTest : MonoBehaviour
    {
        private const string question = "What is the answer to the ultimate question of life, the universe, and everything?";

        private delegate int ImportantMethodDelegate(string question);

        private void CallDirectly()
        {
            var important = ImportantFactory();
            important.Method(question);
        }

        private void CallStaticMethodDirectly()
        {
            Important.StaticMethod(question);
        }

        private void CallViaDelegate()
        {
            var important = ImportantFactory();
            ImportantMethodDelegate indirect = important.Method;
            indirect(question);
        }

        private void CallViaRuntimeDelegate()
        {
            var important = ImportantFactory();
            var runtimeDelegate = Delegate.CreateDelegate(typeof(ImportantMethodDelegate), important, "Method");
            runtimeDelegate.DynamicInvoke(question);
        }

        private void CallViaInterface()
        {
            Interface importantViaInterface = new Important();
            importantViaInterface.MethodOnInterface(question);
        }

        private void CallViaReflection()
        {
            var important = ImportantFactory();
            var methodInfo = typeof(Important).GetMethod("Method");
            methodInfo.Invoke(important, new object[] { question });
        }

        private static Important ImportantFactory()
        {
            var important = new Important();
            return important;
        }

        void Start()
        {
        }
    }
}