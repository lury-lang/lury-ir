//
// Operation.cs
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

namespace Lury.Compiling.IR
{
    /// <summary>
    /// 中間表現に使われるオペレーション (操作) を表す列挙体です。
    /// </summary>
    public enum Operation
    {
        /// <summary>
        /// 何も実行しません。返り値はありません。
        /// </summary>
        Nop,

        /// <summary>
        /// 変数から一時メモリに参照をコピーします。返り値はコピー先の一時メモリ番号です。
        /// </summary>
        Load,
        /// <summary>
        /// 一時メモリから変数に参照をコピーします。変数が存在しない場合は新たに生成されます。返り値はありません。
        /// </summary>
        Store,
        /// <summary>
        /// 変数を削除します。返り値はありません。
        /// </summary>
        Remv,
        /// <summary>
        /// 一時メモリを指定された番号にコピーします。返り値はコピー先の一時メモリ番号です。
        /// </summary>
        Copy,
        /// <summary>
        /// オブジェクトのメンバを解決します。返り値は解決先オブジェクトが格納された一時メモリ番号です。
        /// </summary>
        Rslv,
        /// <summary>
        /// オブジェクトのメンバ解決ができるか判断します。返り値は判定結果が格納された一時メモリ番号です。
        /// </summary>
        Has,

        /// <summary>
        /// 新たなスコープを開始します。返り値はありません。
        /// </summary>
        Scope,
        /// <summary>
        /// これまでのスコープを一つ終了します。返り値はありません。
        /// </summary>
        Break,

        /// <summary>
        /// オペランドの値を 1 つ加算、または前進させます。返り値はありません。
        /// </summary>
        Inc,
        /// <summary>
        /// オペランドの値を 1 つ減算、または後退させます。返り値はありません。
        /// </summary>
        Dec,
        /// <summary>
        /// オペランドの値の符号を変更せず、そのまま返します。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Pos,
        /// <summary>
        /// オペランドの値の符号を逆転します。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Neg,
        /// <summary>
        /// オペランドの値をビット反転します。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Inv,

        /// <summary>
        /// オペランド x, y に対して x の y 乗の値を求めます。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Pow,

        /// <summary>
        /// オペランド x, y に対する乗算の値を求めます。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Mul,
        /// <summary>
        /// オペランド x, y に対する除算の値を求めます。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Div,
        /// <summary>
        /// オペランド x, y に対する整数除算の値を求めます。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Idiv,
        /// <summary>
        /// オペランド x, y に対する余剰の値を求めます。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Mod,

        /// <summary>
        /// オペランド x, y に対する加算の値を求めます。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Add,
        /// <summary>
        /// オペランド x, y に対する減算の値を求めます。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Sub,
        /// <summary>
        /// オペランド x, y に対する結合の値を求めます。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Con,

        /// <summary>
        /// オペランド x を左に y だけ算術左シフトした値を求めます。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Shl,
        /// <summary>
        /// オペランド x を右に y だけ算術右シフトした値を求めます。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Shr,

        /// <summary>
        /// オペランド x, y に対する AND 演算の値を求めます。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        And,
        /// <summary>
        /// オペランド x, y に対する EX-OR の値を求めます。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Xor,
        /// <summary>
        /// オペランド x, y に対する OR の値を求めます。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Or,

        /// <summary>
        /// オペランド x が y よりも小さいかを判定します。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Lt,
        /// <summary>
        /// オペランド x が y よりも大きいかを判定します。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Gt,
        /// <summary>
        /// オペランド x が y 以下であるかを判定します。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Ltq,
        /// <summary>
        /// オペランド x が y 以上であるかを判定します。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Gtq,
        /// <summary>
        /// オペランド x と y が同値であるかを判定します。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Eq,
        /// <summary>
        /// オペランド x と y が同値ではないかを判定します。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Neq,
        /// <summary>
        /// オペランド x が y と同一であるかを判定します。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Is,
        /// <summary>
        /// オペランド x が y と同一ではないかを判定します。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Isn,

        /// <summary>
        /// オペランドを論理反転します。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Not,
        /// <summary>
        /// オペランド x, y に対する論理積を求めます。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Land,
        /// <summary>
        /// オペランド x, y に対する論理和を求めます。返り値は演算結果が格納された先の一時メモリ番号です。
        /// </summary>
        Lor,

        /// <summary>
        /// 値を伴ってルーチンを脱出します。返り値はありません。
        /// </summary>
        Ret,
        /// <summary>
        /// 値を伴ってルーチンを脱出しますが、次回実行時は次のインストラクションから再開します。返り値はありません。
        /// </summary>
        Yield,
        /// <summary>
        /// Catch 命令が出された親ルーチンまで、値を伴って再帰的にルーチンを脱出します。返り値はありません。
        /// </summary>
        Throw,

        /// <summary>
        /// 関数を呼び出します。返り値は関数の返却値が格納された一時メモリ番号です。
        /// </summary>
        Call,
        /// <summary>
        /// オペランドが呼び出し可能な型であれば関数として呼び出します。返り値は関数の返却値が格納された一時メモリ番号です。
        /// </summary>
        Eval,

        /// <summary>
        /// 無条件に指定されたラベルにジャンプします。返り値はありません。
        /// </summary>
        Jmp,
        /// <summary>
        /// オペランドが true である場合は指定されたラベルにジャンプします。返り値はありません。
        /// </summary>
        Jmpt,
        /// <summary>
        /// オペランドが false である場合は指定されたラベルにジャンプします。返り値はありません。
        /// </summary>
        Jmpf,
        /// <summary>
        /// オペランドが nil である場合は指定されたラベルにジャンプします。返り値はありません。
        /// </summary>
        Jmpn,

        /// <summary>
        /// 例外がスローされた場合にジャンプするラベルと、例外を格納する一時メモリを指定します。返り値はありません。
        /// </summary>
        Catch,
        /// <summary>
        /// Catch 命令での例外捕捉を取り消します。返り値はありません。
        /// </summary>
        Ovlok,

        /// <summary>
        /// 指定されたルーチンの関数を生成します。返り値は関数オブジェクトが格納された一時メモリ番号です。
        /// </summary>
        Func,
        /// <summary>
        /// 指定されたルーチンを初期化関数としてクラスを生成します。返り値はクラスオブジェクトが格納された一時メモリ番号です。
        /// </summary>
        Class,
        /// <summary>
        /// 指定されたオペランドにアノテーションを付加します。返り値はありません。
        /// </summary>
        Annot,
    }
}
