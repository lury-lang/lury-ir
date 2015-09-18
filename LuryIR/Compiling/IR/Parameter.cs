//
// Parameter.cs
//
// Author:
//       Tomona Nanase <nanase@users.noreply.github.com>
//
// The MIT License (MIT)
//
// Copyright (c) 2015 Tomona Nanase
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

using System.Numerics;

namespace Lury.Compiling.IR
{
    public class Parameter
    {
        #region -- Public Properties --

        public object Value { get; private set; }

        public ParameterType Type { get; private set; }

        #endregion

        #region -- Constructors --

        private Parameter(object value, ParameterType type)
        {
            this.Value = value;
            this.Type = type;
        }

        #endregion

        #region -- Public Static Methods --

        public static Parameter Nil()
        {
            return new Parameter(null, ParameterType.Nil);
        }

        public static Parameter Integer(long value)
        {
            return new Parameter(value, ParameterType.Integer);
        }

        public static Parameter Real(double value)
        {
            return new Parameter(value, ParameterType.Real);
        }

        public static Parameter Complex(double re, double im)
        {
            return new Parameter(new Complex(re, im), ParameterType.Complex);
        }

        public static Parameter Boolean(bool value)
        {
            return new Parameter(value, ParameterType.Boolean);
        }

        public static Parameter String(string value)
        {
            return new Parameter(value, ParameterType.String);
        }

        public static Parameter Reference(string value)
        {
            return new Parameter(value, ParameterType.Reference);
        }

        #endregion
    }

    public enum ParameterType
    {
        Nil,
        Integer,
        Real,
        Complex,
        Boolean,
        String,
        Reference,
    }
}
