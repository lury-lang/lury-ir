﻿using System;
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
    }
}
