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
using System.Text;

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
        public object Value { get; private set; }

        /// <summary>
        /// パラメータの種類を表す <see cref="Lury.Compiling.IR.ParameterType"/> の値を取得します。
        /// </summary>
        public ParameterType Type { get; private set; }

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
                case ParameterType.String: return "string(\"" + ConvertEscapeSequence((string)this.Value) + "\")";
                case ParameterType.Reference: return this.Value.ToString();
                case ParameterType.Label: return "::" + this.Value;
                default:
                    return "(unknown parameter)";
            }
        }

        #endregion

        #region -- Public Static Methods --

        /// <summary>
        /// 整数値から <see cref="Lury.Compiling.IR.Parameter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="value">パラメータとして渡される整数値。</param>
        /// <returns>生成された <see cref="Lury.Compiling.IR.Parameter"/> オブジェクト。</returns>
        public static Parameter GetInteger(long value)
        {
            return new Parameter(value, ParameterType.Integer);
        }

        /// <summary>
        /// 実数値から <see cref="Lury.Compiling.IR.Parameter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="value">パラメータとして渡される実数値。</param>
        /// <returns>生成された <see cref="Lury.Compiling.IR.Parameter"/> オブジェクト。</returns>
        public static Parameter GetReal(double value)
        {
            return new Parameter(value, ParameterType.Real);
        }

        /// <summary>
        /// 複素数値から <see cref="Lury.Compiling.IR.Parameter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="re">パラメータとして渡される複素数値の実数部分。</param>
        /// <param name="im">パラメータとして渡される複素数値の虚数部分。</param>
        /// <returns>生成された <see cref="Lury.Compiling.IR.Parameter"/> オブジェクト。</returns>
        public static Parameter GetComplex(double re, double im)
        {
            return new Parameter(new Complex(re, im), ParameterType.Complex);
        }

        /// <summary>
        /// 真偽値から <see cref="Lury.Compiling.IR.Parameter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="value">パラメータとして渡される真偽値。</param>
        /// <returns>生成された <see cref="Lury.Compiling.IR.Parameter"/> オブジェクト。</returns>
        public static Parameter GetBoolean(bool value)
        {
            return value ? True : False;
        }

        /// <summary>
        /// 文字列から <see cref="Lury.Compiling.IR.Parameter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="value">パラメータとして渡される文字列。</param>
        /// <returns>生成された <see cref="Lury.Compiling.IR.Parameter"/> オブジェクト。</returns>
        public static Parameter GetString(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return new Parameter(value, ParameterType.String);
        }

        /// <summary>
        /// レジスタ番号と子参照から <see cref="Lury.Compiling.IR.Parameter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="register">パラメータとして渡されるレジスタ番号。</param>
        /// <param name="children">パラメータとして渡される子参照を表す文字列の配列。</param>
        /// <returns>生成された <see cref="Lury.Compiling.IR.Parameter"/> オブジェクト。</returns>
        public static Parameter GetReference(int register, params string[] children)
        {
            return new Parameter(new Reference(register, children), ParameterType.Reference);
        }

        /// <summary>
        /// 変数名と子参照から <see cref="Lury.Compiling.IR.Parameter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="name">パラメータとして渡される変数名を表す文字列。</param>
        /// <param name="children">パラメータとして渡される子参照を表す文字列の配列。</param>
        /// <returns>生成された <see cref="Lury.Compiling.IR.Parameter"/> オブジェクト。</returns>
        public static Parameter GetReference(string name, params string[] children)
        {
            return new Parameter(new Reference(name, children), ParameterType.Reference);
        }

        /// <summary>
        /// ラベル名から <see cref="Lury.Compiling.IR.Parameter"/> オブジェクトを生成します。
        /// </summary>
        /// <param name="value">パラメータとして渡されるラベル名を表す文字列。</param>
        /// <returns>生成された <see cref="Lury.Compiling.IR.Parameter"/> オブジェクト。</returns>
        public static Parameter GetLabel(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");

            return new Parameter(value, ParameterType.Label);
        }

        #endregion

        #region -- Private Static Methods --

        private static string ConvertEscapeSequence(string input)
        {
            return new StringBuilder(input, input.Length * 2)
                .Replace("\\", "\\\\")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t")
                .Replace("\'", "\\'")
                .Replace("\"", "\\\"")
                .Replace("\0", "\\0")
                .Replace("\a", "\\a")
                .Replace("\b", "\\b")
                .Replace("\f", "\\f")
                .Replace("\v", "\\v")
                .ToString();
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
        /// 変数またはレジスタへの参照。
        /// </summary>
        Reference,

        /// <summary>
        /// ラベル。
        /// </summary>
        Label,
    }
}
