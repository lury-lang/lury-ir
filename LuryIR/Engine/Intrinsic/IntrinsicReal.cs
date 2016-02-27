﻿//
// IntrinsicReal.cs
//
// Author:
//       Tomona Nanase <nanase@users.noreply.github.com>
//
// The MIT License (MIT)
//
// Copyright (c) 2016 Tomona Nanase
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using static Lury.Engine.Intrinsic.IntrinsicIntrinsic;
using static Lury.Engine.Intrinsic.IntrinsicBoolean;
using System.Numerics;

namespace Lury.Engine.Intrinsic
{
    [IntrinsicClass(FullName, TypeName)]
    class IntrinsicReal
    {
        #region -- Public Fields --

        public const string FullName = "lury.core.Real";
        public const string TypeName = "Real";

        #endregion

        #region -- Public Static Methods --

        public static LuryObject GetObject(double value)
        {
            return new LuryObject(FullName, value, freeze: true);
        }

        [Intrinsic(OperatorPow)]
        public static LuryObject Pow(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName == IntrinsicInteger.FullName)
                return GetObject(Math.Pow((double)self.Value, (double)(BigInteger)other.Value));
            else if (other.LuryTypeName == FullName)
                return GetObject(Math.Pow((double)self.Value, (double)other.Value));
            else if (other.LuryTypeName == IntrinsicComplex.FullName)
                return IntrinsicComplex.GetObject(Complex.Pow((double)self.Value, (Complex)other.Value));
            else
                throw new ArgumentException();
        }

        [Intrinsic(OperatorInc)]
        public static LuryObject Inc(LuryObject self)
        {
            return GetObject((double)self.Value + 1.0);
        }

        [Intrinsic(OperatorDec)]
        public static LuryObject Dec(LuryObject self)
        {
            return GetObject((double)self.Value - 1.0);
        }

        [Intrinsic(OperatorPos)]
        public static LuryObject Pos(LuryObject self)
        {
            return self;
        }

        [Intrinsic(OperatorNeg)]
        public static LuryObject Neg(LuryObject self)
        {
            return GetObject(-(double)self.Value);
        }
        
        [Intrinsic(OperatorMul)]
        public static LuryObject Mul(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName == IntrinsicInteger.FullName)
                return GetObject((double)self.Value * (double)(BigInteger)other.Value);
            else if (other.LuryTypeName == FullName)
                return GetObject((double)self.Value * (double)other.Value);
            else if (other.LuryTypeName == IntrinsicComplex.FullName)
                return IntrinsicComplex.GetObject((double)self.Value * (Complex)other.Value);
            else
                throw new ArgumentException();
        }

        [Intrinsic(OperatorDiv)]
        public static LuryObject Div(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName == IntrinsicInteger.FullName)
                return GetObject((double)self.Value / (double)(BigInteger)other.Value);
            else if (other.LuryTypeName == FullName)
                return GetObject((double)self.Value / (double)other.Value);
            else if (other.LuryTypeName == IntrinsicComplex.FullName)
                return IntrinsicComplex.GetObject((double)self.Value / (Complex)other.Value);
            else
                throw new ArgumentException();
        }

        [Intrinsic(OperatorIDiv)]
        public static LuryObject IDiv(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName == IntrinsicInteger.FullName)
                return IntrinsicInteger.GetObject(new BigInteger((double)self.Value) / (BigInteger)other.Value);
            else if (other.LuryTypeName == FullName)
                return IntrinsicInteger.GetObject(new BigInteger((double)self.Value) / new BigInteger((double)other.Value));
            else
                throw new ArgumentException();
        }

        [Intrinsic(OperatorMod)]
        public static LuryObject Mod(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName == IntrinsicInteger.FullName)
                return GetObject((double)self.Value % (double)(BigInteger)other.Value);
            else if (other.LuryTypeName == FullName)
                return GetObject((double)self.Value % (double)other.Value);
            else
                throw new ArgumentException();
        }

        [Intrinsic(OperatorAdd)]
        public static LuryObject Add(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName == IntrinsicInteger.FullName)
                return GetObject((double)self.Value + (double)(BigInteger)other.Value);
            else if (other.LuryTypeName == FullName)
                return GetObject((double)self.Value + (double)other.Value);
            else if (other.LuryTypeName == IntrinsicComplex.FullName)
                return IntrinsicComplex.GetObject((double)self.Value + (Complex)other.Value);
            else
                throw new ArgumentException();
        }

        [Intrinsic(OperatorSub)]
        public static LuryObject Sub(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName == IntrinsicInteger.FullName)
                return GetObject((double)self.Value - (double)(BigInteger)other.Value);
            else if (other.LuryTypeName == FullName)
                return GetObject((double)self.Value - (double)other.Value);
            else if (other.LuryTypeName == IntrinsicComplex.FullName)
                return IntrinsicComplex.GetObject((double)self.Value - (Complex)other.Value);
            else
                throw new ArgumentException();
        }
        
        [Intrinsic(OperatorEq)]
        public static LuryObject Equals(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName == IntrinsicInteger.FullName)
                return (double)self.Value == (double)(BigInteger)other.Value ? True : False;
            else if (other.LuryTypeName == FullName)
                return (double)self.Value == (double)other.Value ? True : False;
            else if (other.LuryTypeName == IntrinsicComplex.FullName)
                return (Complex)(double)self.Value == (Complex)other.Value ? True : False;
            else
                throw new ArgumentException();
        }

        [Intrinsic(OperatorNe)]
        public static LuryObject NotEqual(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName == IntrinsicInteger.FullName)
                return (double)self.Value != (double)(BigInteger)other.Value ? True : False;
            else if (other.LuryTypeName == FullName)
                return (double)self.Value != (double)other.Value ? True : False;
            else if (other.LuryTypeName == IntrinsicComplex.FullName)
                return (Complex)(double)self.Value != (Complex)other.Value ? True : False;
            else
                throw new ArgumentException();
        }

        [Intrinsic(OperatorLt)]
        public static LuryObject Lt(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName == IntrinsicInteger.FullName)
                return (double)self.Value < (double)(BigInteger)other.Value ? True : False;
            else if (other.LuryTypeName == FullName)
                return (double)self.Value < (double)other.Value ? True : False;
            else
                throw new ArgumentException();
        }

        [Intrinsic(OperatorLtq)]
        public static LuryObject Ltq(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName == IntrinsicInteger.FullName)
                return (double)self.Value <= (double)(BigInteger)other.Value ? True : False;
            else if (other.LuryTypeName == FullName)
                return (double)self.Value <= (double)other.Value ? True : False;
            else
                throw new ArgumentException();
        }

        [Intrinsic(OperatorGt)]
        public static LuryObject Gt(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName == IntrinsicInteger.FullName)
                return (double)self.Value > (double)(BigInteger)other.Value ? True : False;
            else if (other.LuryTypeName == FullName)
                return (double)self.Value > (double)other.Value ? True : False;
            else
                throw new ArgumentException();
        }

        [Intrinsic(OperatorGtq)]
        public static LuryObject Gtq(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName == IntrinsicInteger.FullName)
                return (double)self.Value >= (double)(BigInteger)other.Value ? True : False;
            else if (other.LuryTypeName == FullName)
                return (double)self.Value >= (double)other.Value ? True : False;
            else
                throw new ArgumentException();
        }

        #endregion
    }
}
