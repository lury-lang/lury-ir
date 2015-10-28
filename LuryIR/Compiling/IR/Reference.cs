//
// Reference.cs
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
using System.Collections.Generic;
using System.Linq;

namespace Lury.Compiling.IR
{
    /// <summary>
    /// 参照パラメータに必要な値をカプセル化したクラスです。
    /// </summary>
    /// <remarks>
    /// <see cref="Reference"/> クラスはインストラクションの実行に必要なパラメータの一つの値として振る舞います。
    /// レジスタや変数などのオブジェクトに対する参照を表し、さらにそのオブジェクトが持つ属性への参照も同時に表します。
    /// 後者を子参照とここでは呼びます。レジスタの番号は 0 以上の整数値を持ち、参照や子参照の名前は 1 文字以上の文字列です。
    /// </remarks>
    public class Reference
    {
        #region -- Public Properties --

        /// <summary>
        /// この <see cref="Reference"/> オブジェクトがレジスタへの参照を表しているかの真偽値を取得します。
        /// </summary>
        /// <value>レジスタへの参照を表すとき true、それ以外のとき false。</value>
        public bool IsRegister { get; private set; }

        /// <summary>
        /// 参照するレジスタの番号を表す数値を取得します。
        /// </summary>
        /// <value>レジスタ番号を表す 0 以上の <see cref="System.Int32"/> 型の値。</value>
        public int Register { get; private set; }

        /// <summary>
        /// 参照する変数の名前を表す文字列を取得します。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 次の参照となる子の変数名の階層リストを取得します。
        /// </summary>
        public IReadOnlyList<string> Children { get; private set; }

        /// <summary>
        /// 次の参照となる子の参照を持つかの真偽値を取得します。
        /// </summary>
        public bool HasChildren { get; private set; }

        #endregion

        #region -- Constructors --

        /// <summary>
        /// レジスタ番号と子参照の配列を指定して
        /// 新しい <see cref="Lury.Compiling.IR.Reference"/> クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="register">参照されるレジスタ番号。</param>
        /// <param name="children">子として参照される変数名の配列。</param>
        internal Reference(int register, params string[] children)
        {
            if (register < 0)
                throw new ArgumentOutOfRangeException("register");

            if (children == null)
                throw new ArgumentNullException("children");

            if (children.Any(string.IsNullOrEmpty))
                throw new ArgumentNullException("children");

            this.IsRegister = true;
            this.Register = register;
            this.Name = null;
            this.Children = children;
            this.HasChildren = (children.Length > 0);
        }

        /// <summary>
        /// 変数名と子参照の配列を指定して
        /// 新しい <see cref="Lury.Compiling.IR.Reference"/> クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="name">参照される変数名。</param>
        /// <param name="children">子として参照される変数名の配列。</param>
        internal Reference(string name, params string[] children)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentOutOfRangeException("name");

            if (children == null)
                throw new ArgumentNullException("children");

            if (children.Any(s => s == null))
                throw new ArgumentNullException("children");

            this.IsRegister = false;
            this.Register = 0;
            this.Name = name;
            this.Children = children;
            this.HasChildren = (children.Length > 0);
        }

        #endregion

        #region -- Public Methods --

        /// <summary>
        /// このインスタンスの状態を表す文字列を取得します。
        /// </summary>
        /// <returns>このインスタンスの状態を表す、レジスタ番号または変数名と子参照が含まれる文字列。</returns>
        public override string ToString()
        {
            return string.Format(
                    "{0}{1}{2}{3}",
                    this.IsRegister ? '%' : '*',
                    this.IsRegister ? this.Register.ToString() : this.Name,
                    this.HasChildren ? "." : "",
                    string.Join(".", this.Children));
        }

        #endregion
    }
}
