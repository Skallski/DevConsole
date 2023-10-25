using Console.Commands;
using UnityEngine;

namespace Tests
{
    public class TestMonoBehaviour : MonoBehaviour
    {
        [SerializeField, ConsoleModifiableVariable("test")] private int _someVariable = 15;
        [SerializeField] private TestScriptable _testScriptable;
    }
}