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

using System;
using System.Numerics;
using Lury.Compiling.Utils;

namespace Lury.Compiling.IR
{
    /// <summary>
    /// 操作に用いられるパラメータをカプセル化します。
    /// </summary>
    public class Parameter
    {
        #region -- Public Fields --

        /// <summary>
        /// nil を表す Parameter オブジェクトです。この変数は読み取り専用です。
        /// </summary>
        public readonly static Parameter Nil = new Parameter(null, ParameterType.Nil);

        /// <summary>
        /// true を表す Parameter オブジェクトです。この変数は読み取り専用です。
        /// </summary>
        public readonly static Parameter True = new Parameter(true, ParameterType.Boolean);

        /// <summary>
        /// false を表す Parameter オブジェクトです。この変数は読み取り専用です。
        /// </summary>
        public readonly static Parameter False = new Parameter(false, ParameterType.Boolean);

        #endregion

        #region -- Public Properties --

        /// <summary>
        /// パラメータの値を取得します。
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// パラメータの種類を表す <see cref="Lury.Compiling.IR.ParameterType"/> の値を取得します。
        /// </summary>
        public ParameterType Type { get; }

        #endregion

        #region -- Constructors --

        private Parameter(object value, ParameterType type)
        {
            this.Value = value;
            this.Type = type;
        }

        #endregion

        #region -- Public Methods --

        /// <summary>
        /// このオブジェクトを表す文字列を取得します。
        /// </summary>
        /// <returns>オブジェクトの種類とその値を含んだ文字列。</returns>
        public override string ToString()
        {
            switch (this.Type)
            {
                case ParameterType.Nil: return "nil";
                case ParameterType.Integer: return "int(" + this.Value + ")";
                case ParameterType.Real: return "real(" + this.Value + ")";
                case ParameterType.Complex: return "complex" + this.Value;
                case ParameterType.Boolean: return ((bool)this.Value ? "bool(true)" : "bool(false)");
                case ParameterType.String: return "string(\"" + ((string)this.Value).ConvertControlChars() + "\")";
                case ParameterType.Reference: return this.Value.ToString();
                case ParameterType.Register: return "%" + this.Value;
                case ParameterType.Label: return "::" + this.Value;
                default:
                    return "(invalid parameter)";
            }
        }

        #endregion

        #region -- Public Static Methods --

        /// <summary>
        /// 整数値から <see cref="Lury.Compiling.IR.Parameter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="value">パラメータとして渡される整数値。</param>
        /// <returns>生成された <see cref="Lury.Compiling.IR.Parameter"/> オブジェクト。</returns>
        public static Parameter GetInteger(object value)
        {
            if (value is decimal)
                return new Parameter(new BigInteger((decimal)value), ParameterType.Integer);
            else if (value is double)
                return new Parameter(new BigInteger((double)value), ParameterType.Integer);
            else if (value is float)
                return new Parameter(new BigInteger((float)value), ParameterType.Integer);
            else if (value is ulong)
                return new Parameter(new BigInteger((ulong)value), ParameterType.Integer);
            else if (value is long)
                return new Parameter(new BigInteger((long)value), ParameterType.Integer);
            else if (value is uint)
                return new Parameter(new BigInteger((uint)value), ParameterType.Integer);
            else if (value is int)
                return new Parameter(new BigInteger((int)value), ParameterType.Integer);
            else if (value is BigInteger)
                return new Parameter((BigInteger)value, ParameterType.Integer);
            else if (value is string)
                return new Parameter(BigInteger.Parse((string)value), ParameterType.Integer);
            else if (value is byte[])
                return new Parameter(new BigInteger((byte[])value), ParameterType.Integer);
            else
                throw new ArgumentException();
        }

        /// <summary>
        /// 実数値から <see cref="Lury.Compiling.IR.Parameter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="value">パラメータとして渡される実数値。</param>
        /// <returns>生成された <see cref="Lury.Compiling.IR.Parameter"/> オブジェクト。</returns>
        public static Parameter GetReal(double value)
            => new Parameter(value, ParameterType.Real);

        /// <summary>
        /// 複素数値から <see cref="Lury.Compiling.IR.Parameter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="re">パラメータとして渡される複素数値の実数部分。</param>
        /// <param name="im">パラメータとして渡される複素数値の虚数部分。</param>
        /// <returns>生成された <see cref="Lury.Compiling.IR.Parameter"/> オブジェクト。</returns>
        public static Parameter GetComplex(double re, double im)
            => new Parameter(new Complex(re, im), ParameterType.Complex);

        /// <summary>
        /// 複素数値から <see cref="Lury.Compiling.IR.Parameter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="value">パラメータとして渡される複素数値。</param>
        /// <returns>生成された <see cref="Lury.Compiling.IR.Parameter"/> オブジェクト。</returns>
        public static Parameter GetComplex(Complex value)
            => new Parameter(value, ParameterType.Complex);

        /// <summary>
        /// 真偽値から <see cref="Lury.Compiling.IR.Parameter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="value">パラメータとして渡される真偽値。</param>
        /// <returns>生成された <see cref="Lury.Compiling.IR.Parameter"/> オブジェクト。</returns>
        public static Parameter GetBoolean(bool value)
            => value ? True : False;

        /// <summary>
        /// 文字列から <see cref="Lury.Compiling.IR.Parameter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="value">パラメータとして渡される文字列。</param>
        /// <returns>生成された <see cref="Lury.Compiling.IR.Parameter"/> オブジェクト。</returns>
        public static Parameter GetString(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return new Parameter(value, ParameterType.String);
        }

        /// <summary>
        /// 変数名と子参照から <see cref="Lury.Compiling.IR.Parameter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="name">パラメータとして渡される変数名を表す文字列。</param>
        /// <returns>生成された <see cref="Lury.Compiling.IR.Parameter"/> オブジェクト。</returns>
        public static Parameter GetReference(string name)
            => new Parameter(new Reference(name), ParameterType.Reference);

        /// <summary>
        /// 変数名と子参照から <see cref="Lury.Compiling.IR.Parameter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="value">パラメータとして渡されるレジスタ番号。</param>
        /// <returns>生成された <see cref="Lury.Compiling.IR.Parameter"/> オブジェクト。</returns>
        public static Parameter GetRegister(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));

            return new Parameter(value, ParameterType.Register);
        }

        /// <summary>
        /// ラベル名から <see cref="Lury.Compiling.IR.Parameter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="value">パラメータとして渡されるラベル名を表す文字列。</param>
        /// <returns>生成された <see cref="Lury.Compiling.IR.Parameter"/> オブジェクト。</returns>
        public static Parameter GetLabel(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            return new Parameter(value, ParameterType.Label);
        }

        #endregion
    }

    /// <summary>
    /// パラメータの種類を表す列挙体です。
    /// </summary>
    public enum ParameterType
    {
        /// <summary>
        /// nil。
        /// </summary>
        Nil,

        /// <summary>
        /// 整数値。
        /// </summary>
        Integer,

        /// <summary>
        /// 実数値。
        /// </summary>
        Real,

        /// <summary>
        /// 複素数値。
        /// </summary>
        Complex,

        /// <summary>
        /// 真偽値。
        /// </summary>
        Boolean,

        /// <summary>
        /// 文字列。
        /// </summary>
        String,

        /// <summary>
        /// レジスタへの参照。
        /// </summary>
        Register,

        /// <summary>
        /// 変数への参照。
        /// </summary>
        Reference,

        /// <summary>
        /// ラベル。
        /// </summary>
        Label,
    }
}
