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
    [IntrinsicClass(FullName, TypeName)]
    class IntrinsicBoolean
    {
        #region -- Public Fields --

        public const string FullName = "lury.core.Boolean";
        public const string TypeName = "Boolean";
        
        public static readonly LuryObject True = new LuryObject(IntrinsicBoolean.TypeName, true, freeze: true);
        public static readonly LuryObject False = new LuryObject(IntrinsicBoolean.TypeName, false, freeze: true);
        
        #endregion

        #region -- Public Static Methods --

        [Intrinsic("opEq")]
        public static LuryObject Equals(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return self.Value == other.Value ? True : False;
        }

        [Intrinsic("opNe")]
        public static LuryObject NotEqual(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return self.Value != other.Value ? True : False;
        }

        [Intrinsic("opAnd")]
        public static LuryObject And(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return (bool)self.Value & (bool)other.Value ? True : False;
        }

        [Intrinsic("opXor")]
        public static LuryObject Xor(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return (bool)self.Value ^ (bool)other.Value ? True : False;
        }

        [Intrinsic("opOr")]
        public static LuryObject Or(LuryObject self, LuryObject other)
        {
            if (other.LuryTypeName != TypeName)
                throw new ArgumentException();

            return (bool)self.Value | (bool)other.Value ? True : False;
        }

        [Intrinsic("opNot")]
        public static LuryObject Not(LuryObject self)
        {
            return (bool)self.Value ? False : True;
        }

        #endregion
    }
}
