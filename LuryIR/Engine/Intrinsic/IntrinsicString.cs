//
// IntrinsicBoolean.cs
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

namespace Lury.Engine.Intrinsic
{
    [IntrinsicClass(FullName, TypeName)]
    class IntrinsicString
    {
        #region -- Public Fields --

        public const string FullName = "lury.core.String";
        public const string TypeName = "String";

        public static readonly LuryObject Empty = GetObject(string.Empty);

        #endregion

        #region -- Public Static Methods --

        public static LuryObject GetObject(object value)
            => new LuryObject(FullName, (string)value, freeze: true);

        [Intrinsic(OperatorEq)]
        public static LuryObject Equals(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return self.Value == other.Value ? IntrinsicBoolean.True : IntrinsicBoolean.False;
        }

        [Intrinsic(OperatorNe)]
        public static LuryObject NotEqual(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return self.Value != other.Value ? IntrinsicBoolean.True : IntrinsicBoolean.False;
        }

        [Intrinsic(OperatorLt)]
        public static LuryObject Lt(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return ((string)self.Value).CompareTo(other.Value) < 0 ? IntrinsicBoolean.True : IntrinsicBoolean.False;
        }

        [Intrinsic(OperatorLtq)]
        public static LuryObject Ltq(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return ((string)self.Value).CompareTo(other.Value) <= 0 ? IntrinsicBoolean.True : IntrinsicBoolean.False;
        }

        [Intrinsic(OperatorGt)]
        public static LuryObject Gt(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return ((string)self.Value).CompareTo(other.Value) > 0 ? IntrinsicBoolean.True : IntrinsicBoolean.False;
        }

        [Intrinsic(OperatorGtq)]
        public static LuryObject Gtq(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return ((string)self.Value).CompareTo(other.Value) >= 0 ? IntrinsicBoolean.True : IntrinsicBoolean.False;
        }

        [Intrinsic(OperatorCon)]
        public static LuryObject Con(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return new LuryObject(TypeName, (string)self.Value + (string)other.Value, freeze: true);
        }

        [Intrinsic(OperatorIn)]
        public static LuryObject In(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return ((string)self.Value).Contains((string)other.Value) ? IntrinsicBoolean.True : IntrinsicBoolean.False;
        }

        [Intrinsic(OperatorNotIn)]
        public static LuryObject NotIn(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return ((string)self.Value).Contains((string)other.Value) ? IntrinsicBoolean.False : IntrinsicBoolean.True;
        }

        #endregion
    }
}
