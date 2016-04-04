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
            const long Value = 42;
            var param = Parameter.GetInteger(Value);

            Assert.IsNotNull(param);
            Assert.IsInstanceOfType(param.Value, typeof(BigInteger));
            Assert.AreEqual(new BigInteger(Value), param.Value);
            Assert.AreEqual(ParameterType.Integer, param.Type);
        }

        [TestMethod]
        public void GetRealTest()
        {
            const double Value = Math.PI;
            var param = Parameter.GetReal(Value);

            Assert.IsNotNull(param);
            Assert.IsInstanceOfType(param.Value, typeof(double));
            Assert.AreEqual(Value, param.Value);
            Assert.AreEqual(ParameterType.Real, param.Type);
        }

        [TestMethod]
        public void GetComplexTest()
        {
            const double ValueRe = 1.2345;
            const double ValueIm = 6.7890;
            var param = Parameter.GetComplex(ValueRe, ValueIm);

            Assert.IsNotNull(param);
            Assert.IsInstanceOfType(param.Value, typeof(Complex));
            Assert.AreEqual(ValueRe, ((Complex)param.Value).Real);
            Assert.AreEqual(ValueIm, ((Complex)param.Value).Imaginary);
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
            const string Value = "Test String";

            var param = Parameter.GetString(Value);

            Assert.IsNotNull(param);
            Assert.IsInstanceOfType(param.Value, typeof(string));
            Assert.AreEqual(Value, param.Value);
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
            const string Name = "fizzbuzz";
            
            var nameParam = Parameter.GetReference(Name);
            
            Assert.IsNotNull(nameParam);
            Assert.IsInstanceOfType(nameParam.Value, typeof(Reference));
            Assert.AreEqual(Name, ((Reference)nameParam.Value).Name);
            Assert.AreEqual(ParameterType.Reference, nameParam.Type);
        }

        [TestMethod]
        public void GetRegisterTest()
        {
            const int Register = 3;

            var registerParam = Parameter.GetRegister(Register);

            Assert.IsNotNull(registerParam);
            Assert.IsInstanceOfType(registerParam.Value, typeof(int));
            Assert.AreEqual(Register, (int)registerParam.Value);
            Assert.AreEqual(ParameterType.Register, registerParam.Type);
        }

        [TestMethod]
        public void GetLabelTest()
        {
            const string Value = "Test String";

            var param = Parameter.GetLabel(Value);

            Assert.IsNotNull(param);
            Assert.IsInstanceOfType(param.Value, typeof(string));
            Assert.AreEqual(Value, param.Value);
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

        [TestMethod]
        public void ToStringTest()
        {
            Assert.AreEqual("nil", Parameter.Nil.ToString());
            Assert.AreEqual("int(42)", Parameter.GetInteger(42).ToString());
            Assert.AreEqual("real(3.5)", Parameter.GetReal(3.5).ToString());
            Assert.AreEqual("complex(3.5, -4.2)", Parameter.GetComplex(3.5, -4.2).ToString());
            Assert.AreEqual("bool(true)", Parameter.True.ToString());
            Assert.AreEqual("bool(false)", Parameter.False.ToString());
            Assert.AreEqual("string(\"test\\r\\nstring\")", Parameter.GetString("test\r\nstring").ToString());
            Assert.AreEqual("%0", Parameter.GetRegister(0).ToString());
            Assert.AreEqual("*hoge", Parameter.GetReference("hoge").ToString());
            Assert.AreEqual("::test_label", Parameter.GetLabel("test_label").ToString());
        }
    }
}
