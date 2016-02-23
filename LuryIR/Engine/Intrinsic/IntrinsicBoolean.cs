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

namespace Lury.Engine.Intrinsic
{
    class IntrinsicBoolean
    {
        #region -- Public Fields --

        public const string TypeName = "lury.core.Boolean";

        #endregion

        #region -- Public Static Methods --

        [Intrinsic(TypeName, "opEq")]
        public static LuryObject Equals(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return self.Value == other.Value ? LuryObject.True : LuryObject.False;
        }

        [Intrinsic(TypeName, "opNe")]
        public static LuryObject NotEqual(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return self.Value != other.Value ? LuryObject.True : LuryObject.False;
        }

        [Intrinsic(TypeName, "opAnd")]
        public static LuryObject Amd(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return (bool)self.Value & (bool)other.Value ? LuryObject.True : LuryObject.False;
        }

        [Intrinsic(TypeName, "opXor")]
        public static LuryObject Xor(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return (bool)self.Value ^ (bool)other.Value ? LuryObject.True : LuryObject.False;
        }

        [Intrinsic(TypeName, "opOr")]
        public static LuryObject Or(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return (bool)self.Value | (bool)other.Value ? LuryObject.True : LuryObject.False;
        }

        [Intrinsic(TypeName, "opNot")]
        public static LuryObject Not(LuryObject self)
        {
            return (bool)self.Value ? LuryObject.False : LuryObject.True;
        }

        #endregion
    }
}
