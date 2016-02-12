//
// Instruction.cs
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

namespace Lury.Compiling.IR
{
    /// <summary>
    /// 中間表現の一つの命令を表現するためのクラスです。
    /// </summary>
    public class Instruction
    {
        #region -- Public Fields --

        /// <summary>
        /// 操作の返り値をレジスタに格納しないことを表す値です。
        /// </summary>
        public const int NoAssign = -1;

        #endregion

        #region -- Public Properties --

        /// <summary>
        /// 操作の返り値が格納されるレジスタの番号を取得します。
        /// </summary>
        public int Destination { get; private set; }

        /// <summary>
        /// 実行される操作 <see cref="Lury.Compiling.IR.Operation"/> の値を取得します。
        /// </summary>
        public Operation Operation { get; private set; }

        /// <summary>
        /// 実行される操作に用いるパラメータのリストを取得します。
        /// </summary>
        public IReadOnlyList<Parameter> Parameters { get; private set; }

        #endregion

        #region -- Constructors --

        /// <summary>
        /// レジスタ番号、操作とパラメータを指定して
        /// 新しい <see cref="Lury.Compiling.IR.Instruction"/> クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="destination">操作の返り値が格納されるレジスタの番号。</param>
        /// <param name="operation">実行される操作を表す <see cref="Lury.Compiling.IR.Operation"/> の値。</param>
        /// <param name="parameters">パラメータの配列。</param>
        public Instruction(int destination, Operation operation, params Parameter[] parameters)
        {
            if (!Enum.IsDefined(typeof(Operation), operation))
                throw new ArgumentOutOfRangeException(nameof(operation));

            this.Destination = destination;
            this.Operation = operation;
            this.Parameters = parameters ?? new Parameter[0];
        }

        /// <summary>
        /// レジスタ番号と操作を指定して
        /// 新しい <see cref="Lury.Compiling.IR.Instruction"/> クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="destination">操作の返り値が格納されるレジスタの番号。</param>
        /// <param name="operation">実行される操作を表す <see cref="Lury.Compiling.IR.Operation"/> の値。</param>
        public Instruction(int destination, Operation operation)
            : this(destination, operation, null)
        {
        }

        /// <summary>
        /// 操作とパラメータを指定して
        /// 新しい <see cref="Lury.Compiling.IR.Instruction"/> クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="operation">実行される操作を表す <see cref="Lury.Compiling.IR.Operation"/> の値。</param>
        /// <param name="parameters">パラメータの配列。</param>
        public Instruction(Operation operation, params Parameter[] parameters)
            : this(NoAssign, operation, parameters)
        {
        }

        /// <summary>
        /// 操作を指定して新しい <see cref="Lury.Compiling.IR.Instruction"/> クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="operation">実行される操作を表す <see cref="Lury.Compiling.IR.Operation"/> の値。</param>
        public Instruction(Operation operation)
            : this(NoAssign, operation, null)
        {
        }

        #endregion
    }
}
