using System.Collections.Generic;
using System.Linq;
using Lury.Compiling.IR;
using Lury.Compiling.Utils;
using Lury.Engine;
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

        /*
        [TestMethod]
        public void ConstructorTest2()
        {
            const string sourceName = "test";

            RoutineBuilder rb1 = new RoutineBuilder("`_lambda0", sourceName);
            {
                rb1.SetRoutinePosition(3, 9, 20);

                rb1.Store(0, Parameter.GetReference("j"));
                rb1.SetPositionAtCurrent(3, 9, 1);

                var r0 = rb1.Power(Parameter.GetReference(0), Parameter.False, RoutineBuilder.NewAssign);
                rb1.SetPositionAtCurrent(3, 22, 6);

                var r1 = rb1.Call(Parameter.GetReference("println"), RoutineBuilder.NewAssign, Parameter.GetReference(r0));
                rb1.SetPositionAtCurrent(3, 14, 15);

                rb1.Return(Parameter.GetReference(r1));
            }
            Routine routine1 = rb1.Build();

            RoutineBuilder rb2 = new RoutineBuilder("main", sourceName);
            {
                rb2.Children.Add(routine1);
                rb2.SetRoutinePosition(1, 1);

                var r0 = rb2.Call(Parameter.GetReference("List", "create"), RoutineBuilder.NewAssign, Parameter.GetInteger(1), Parameter.GetInteger(5), Parameter.GetInteger(8), Parameter.GetInteger(2), Parameter.GetInteger(7));
                rb2.SetPositionAtCurrent(1, 5, 15);

                rb2.Store(r0, Parameter.GetReference("a"));
                rb2.SetPositionAtCurrent(1, 1, 19);

                rb2.BeginScope();
                rb2.SetPositionAtCurrent(2, 1);

                var r1 = rb2.Call(Parameter.GetReference(r0, "count"), RoutineBuilder.NewAssign);
                rb2.SetPositionAtCurrent(2, 15, 1);

                var r2 = rb2.IntDivide(Parameter.GetReference(r1), Parameter.GetInteger(2), RoutineBuilder.NewAssign);
                rb2.SetPositionAtCurrent(2, 15, 6);

                var r3 = rb2.Call(Parameter.GetReference("Range", "new"), RoutineBuilder.NewAssign, Parameter.GetInteger(0), Parameter.GetReference(r2), Parameter.GetBoolean(false));
                rb2.SetPositionAtCurrent(2, 12, 9);

                var r4 = rb2.Call(Parameter.GetReference("Slice", "new"), RoutineBuilder.NewAssign, Parameter.GetReference("a"), Parameter.GetReference(r3));
                rb2.SetPositionAtCurrent(2, 10, 12);

                var r5 = rb2.Call(Parameter.GetReference(r4, "iterator"), RoutineBuilder.NewAssign);

                var label0 = rb2.GetLabelName("for_begin");
                var label1 = rb2.GetLabelName("for_body");
                var label2 = rb2.GetLabelName("for_end");
                
                rb2.SetLabelAtCurrent(label0);
                {
                    var r6 = rb2.Call(Parameter.GetReference(r5, "next"), RoutineBuilder.NewAssign);
                    rb2.SetPositionAtCurrent(2, 7, 2);


                    rb2.JumpIfFalse(label2, Parameter.GetReference(r6));

                    var r7 = rb2.Call(Parameter.GetReference(r5, "current"), RoutineBuilder.NewAssign);

                    rb2.Store(r7, Parameter.GetReference("i"));
                    rb2.SetPositionAtCurrent(2, 5, 1);
                }

                rb2.SetLabelAtCurrent(label1);
                {
                    var r8 = rb2.CreateFunction(routine1, RoutineBuilder.NewAssign);
                    rb2.SetPositionAtCurrent(3, 9, 20);

                    rb2.Store(r8, Parameter.GetReference("k"));
                    rb2.SetPositionAtCurrent(3, 5, 24);

                    rb2.CallWithoutReturn(Parameter.GetReference("k"), Parameter.GetReference("i"));
                    rb2.SetPositionAtCurrent(4, 5, 4);

                    rb2.Jump(label0);
                }

                rb2.SetLabelAtCurrent(label2);
                {
                    rb2.EndScope();
                    rb2.SetPositionAtCurrent(5, 1);
                }
            }
            Routine routine2 = rb2.Build();
            var k = routine2.Dump();

            RoutineVerifier rv = new RoutineVerifier(routine2);
            rv.Verify();
        }
        */

        [TestMethod]
        public void ConstructorTest3()
        {
            const string sourceName = "test";

            RoutineBuilder rb = new RoutineBuilder("main", sourceName);
            {
                var inputX = Parameter.GetBoolean(true);
                var inputY = Parameter.GetBoolean(false);

                var r0 = rb.Not(inputX, RoutineBuilder.NewAssign);
                var r1 = rb.Not(inputY, RoutineBuilder.NewAssign);

                var r2 = rb.BitwiseAnd(inputX, Parameter.GetRegister(r1), RoutineBuilder.NewAssign);
                var r3 = rb.BitwiseAnd(Parameter.GetRegister(r0), inputY, RoutineBuilder.NewAssign);

                var r4 = rb.BitwiseOr(Parameter.GetRegister(r2), Parameter.GetRegister(r3), RoutineBuilder.NewAssign);

                rb.Return(Parameter.GetRegister(r4));
            }
            Routine routine = rb.Build();
            var k = routine.Dump();

            RoutineVerifier rv = new RoutineVerifier(routine);
            Assert.IsTrue(rv.Verify());

            ExecuteEngine ee = new ExecuteEngine(routine);
            var luryObject = ee.Execute();
        }

        
        [TestMethod]
        public void ConstructorTest4()
        {
            const string sourceName = "test";

            RoutineBuilder rb = new RoutineBuilder("main", sourceName);
            {
                var r0 = rb.Load(Parameter.GetBoolean(true), RoutineBuilder.NewAssign);
                var r1 = rb.Load(Parameter.GetBoolean(true), RoutineBuilder.NewAssign);

                rb.Store(r0, Parameter.GetReference("x"));
                rb.Store(r1, Parameter.GetReference("y"));

                var r2 = rb.Load(Parameter.GetReference("x"), RoutineBuilder.NewAssign);
                var r3 = rb.Load(Parameter.GetReference("y"), RoutineBuilder.NewAssign);
                
                var r4 = rb.Not(Parameter.GetRegister(r2), RoutineBuilder.NewAssign);
                var r5 = rb.Not(Parameter.GetRegister(r3), RoutineBuilder.NewAssign);

                var r6 = rb.BitwiseAnd(Parameter.GetRegister(r2), Parameter.GetRegister(r5), RoutineBuilder.NewAssign);
                var r7 = rb.BitwiseAnd(Parameter.GetRegister(r3), Parameter.GetRegister(r4), RoutineBuilder.NewAssign);

                var r8 = rb.BitwiseOr(Parameter.GetRegister(r6), Parameter.GetRegister(r7), RoutineBuilder.NewAssign);

                rb.Return(Parameter.GetRegister(r8));
            }
            Routine routine = rb.Build();
            var k = routine.Dump();

            RoutineVerifier rv = new RoutineVerifier(routine);
            Assert.IsTrue(rv.Verify());

            ExecuteEngine ee = new ExecuteEngine(routine);
            var luryObject = ee.Execute();
        }
        
    }
}
