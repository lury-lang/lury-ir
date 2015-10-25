using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Lury.Compiling.IR;
using Lury.Compiling.Utils;
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
    }
}
