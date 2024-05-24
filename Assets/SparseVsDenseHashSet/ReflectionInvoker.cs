using UnityEngine;

namespace SparseVsDenseHashSet
{
    public class ReflectionInvoker : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 30;
        }

        private void Update()
        {
            ClassInvoker.InvokeGeneratedMethods();
        }
    }
}