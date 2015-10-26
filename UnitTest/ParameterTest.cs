using System;
using System.Linq;
using System.Numerics;
using Lury.Compiling.IR;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class ParameterTest
    {
        [TestMethod]
        public void NilTest()
        {
            var param = Parameter.Nil;

            Assert.IsNotNull(param);
            Assert.IsNull(param.Value);
            Assert.AreEqual(ParameterType.Nil, param.Type);
        }

        [TestMethod]
        public void TrueAndFalseTest()
        {
            var trueParam = Parameter.True;
            var falseParam = Parameter.False;

            Assert.IsNotNull(trueParam);
            Assert.IsInstanceOfType(trueParam.Value, typeof(bool));
            Assert.IsTrue((bool)trueParam.Value);
            Assert.AreEqual(ParameterType.Boolean, trueParam.Type);

            Assert.IsNotNull(falseParam);
            Assert.IsInstanceOfType(falseParam.Value, typeof(bool));
            Assert.IsFalse((bool)falseParam.Value);
            Assert.AreEqual(ParameterType.Boolean, falseParam.Type);
        }

        [TestMethod]
        public void GetIntegerTest()
        {
            const long value = 42;
            var param = Parameter.GetInteger(value);

            Assert.IsNotNull(param);
            Assert.IsInstanceOfType(param.Value, typeof(long));
            Assert.AreEqual(value, param.Value);
            Assert.AreEqual(ParameterType.Integer, param.Type);
        }

        [TestMethod]
        public void GetRealTest()
        {
            const double value = Math.PI;
            var param = Parameter.GetReal(value);

            Assert.IsNotNull(param);
            Assert.IsInstanceOfType(param.Value, typeof(double));
            Assert.AreEqual(value, param.Value);
            Assert.AreEqual(ParameterType.Real, param.Type);
        }

        [TestMethod]
        public void GetComplexTest()
        {
            const double valueRe = 1.2345;
            const double valueIm = 6.7890;
            var param = Parameter.GetComplex(valueRe, valueIm);

            Assert.IsNotNull(param);
            Assert.IsInstanceOfType(param.Value, typeof(Complex));
            Assert.AreEqual(valueRe, ((Complex)param.Value).Real);
            Assert.AreEqual(valueIm, ((Complex)param.Value).Imaginary);
            Assert.AreEqual(ParameterType.Complex, param.Type);
        }

        [TestMethod]
        public void GetBooleanTest()
        {
            var paramTrue = Parameter.GetBoolean(true);
            var paramFalse = Parameter.GetBoolean(false);

            Assert.IsNotNull(paramTrue);
            Assert.IsNotNull(paramFalse);
            Assert.AreSame(Parameter.True, paramTrue);
            Assert.AreSame(Parameter.False, paramFalse);
        }

        [TestMethod]
        public void GetStringTest()
        {
            const string value = "Test String";

            var param = Parameter.GetString(value);

            Assert.IsNotNull(param);
            Assert.IsInstanceOfType(param.Value, typeof(string));
            Assert.AreEqual(value, param.Value);
            Assert.AreEqual(ParameterType.String, param.Type);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetStringError()
        {
            Parameter.GetString(null);
        }

        [TestMethod]
        public void GetReferenceTest()
        {
            const int register = 3;
            const string name = "fizzbuzz";
            const string child1 = "foo";
            const string child2 = "bar";

            var registerParam1 = Parameter.GetReference(register, child1, child2);
            var registerParam2 = Parameter.GetReference(register);
            var nameParam1 = Parameter.GetReference(name, child1, child2);
            var nameParam2 = Parameter.GetReference(name);

            Assert.IsNotNull(registerParam1);
            Assert.IsNotNull(registerParam2);
            Assert.IsNotNull(nameParam1);
            Assert.IsNotNull(nameParam2);

            Assert.IsInstanceOfType(registerParam1.Value, typeof(Reference));
            Assert.IsInstanceOfType(registerParam2.Value, typeof(Reference));
            Assert.IsInstanceOfType(nameParam1.Value, typeof(Reference));
            Assert.IsInstanceOfType(nameParam2.Value, typeof(Reference));

            Assert.IsTrue(((Reference)registerParam1.Value).IsRegister);
            Assert.IsTrue(((Reference)registerParam2.Value).IsRegister);
            Assert.IsFalse(((Reference)nameParam1.Value).IsRegister);
            Assert.IsFalse(((Reference)nameParam2.Value).IsRegister);

            Assert.AreEqual(register, ((Reference)registerParam1.Value).Register);
            Assert.AreEqual(register, ((Reference)registerParam2.Value).Register);
            Assert.AreEqual(name, ((Reference)nameParam1.Value).Name);
            Assert.AreEqual(name, ((Reference)nameParam2.Value).Name);

            Assert.IsTrue(((Reference)registerParam1.Value).HasChildren);
            Assert.IsFalse(((Reference)registerParam2.Value).HasChildren);
            Assert.IsTrue(((Reference)nameParam1.Value).HasChildren);
            Assert.IsFalse(((Reference)nameParam2.Value).HasChildren);

            Assert.AreEqual(register, ((Reference)registerParam1.Value).Register);
            Assert.AreEqual(register, ((Reference)registerParam2.Value).Register);
            Assert.AreEqual(name, ((Reference)nameParam1.Value).Name);
            Assert.AreEqual(name, ((Reference)nameParam2.Value).Name);

            CollectionAssert.AreEquivalent(new[] { child1, child2 }, ((Reference)registerParam1.Value).Children.ToArray());
            CollectionAssert.AreEquivalent(new[] { child1, child2 }, ((Reference)nameParam1.Value).Children.ToArray());
            
            Assert.AreEqual(ParameterType.Reference, registerParam1.Type);
            Assert.AreEqual(ParameterType.Reference, registerParam2.Type);
            Assert.AreEqual(ParameterType.Reference, nameParam1.Type);
            Assert.AreEqual(ParameterType.Reference, nameParam2.Type);
        }

        [TestMethod]
        public void GetLabelTest()
        {
            const string value = "Test String";

            var param = Parameter.GetLabel(value);

            Assert.IsNotNull(param);
            Assert.IsInstanceOfType(param.Value, typeof(string));
            Assert.AreEqual(value, param.Value);
            Assert.AreEqual(ParameterType.Label, param.Type);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetLabelError1()
        {
            Parameter.GetLabel(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetLabelError2()
        {
            Parameter.GetLabel(string.Empty);
        }
    }
}
