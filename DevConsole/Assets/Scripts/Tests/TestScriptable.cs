using Console.Commands;
using UnityEngine;

namespace Tests
{
    [CreateAssetMenu(fileName = "test", menuName = "test")]
    public class TestScriptable : ScriptableObject
    {
        [SerializeField, ConsoleModifiableVariable("testTwo")] private int _someVariable = 999;
    }
}