namespace MethodInvocations
{
    /// <summary>
    /// Code from https://blog.unity.com/engine-platform/il2cpp-internals-method-calls to test the latest IL2CPP output
    /// </summary>
    interface Interface
    {
        int MethodOnInterface(string question);
    }

    class Important : Interface
    {
        public int Method(string question)
        {
            return 42;
        }

        public int MethodOnInterface(string question)
        {
            return 42;
        }

        public static int StaticMethod(string question)
        {
            return 42;
        }
    }
}