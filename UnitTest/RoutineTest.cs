using System.Collections.Generic;
using System.Linq;
using Lury.Compiling.IR;
using Lury.Compiling.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class RoutineTest
    {
        [TestMethod]
        public void ConstructorTest1()
        {
            const string name = "test";
            var routine = new Routine(name);

            Assert.AreEqual(name, routine.Name);
            Assert.AreEqual(0, routine.Children.Count);
            Assert.AreEqual(0, routine.Instructions.Count);
            Assert.AreEqual(0, routine.JumpLabels.Count);
            Assert.AreEqual(0, routine.CodePosition.Count);
        }

        [TestMethod]
        public void ConstructorTest2()
        {
            const string name = "test";
            var children = new Routine[] { new Routine(name) };
            var instructions = new Instruction[] { new Instruction(Operation.Nop), new Instruction(Operation.Nop) };
            var jumpLabels = new Dictionary<string, int> {
                {"%1", 0},
                {"%2", 1}
            };
            var codePosition = new Dictionary<int, CodePosition> {
                { 1, new CodePosition(string.Empty, CharPosition.BasePosition) },
                { 0, new CodePosition(string.Empty, CharPosition.BasePosition) }
            };
            var routine = new Routine(name, children, instructions, jumpLabels, codePosition);

            Assert.AreEqual(name, routine.Name);
            CollectionAssert.AreEqual(children, routine.Children.ToArray());
            CollectionAssert.AreEqual(instructions, routine.Instructions.ToArray());
            CollectionAssert.AreEquivalent(jumpLabels, routine.JumpLabels.ToDictionary(p => p.Key, p => p.Value));
            CollectionAssert.AreEquivalent(codePosition, routine.CodePosition.ToDictionary(p => p.Key, p => p.Value));
        }
    }
}
