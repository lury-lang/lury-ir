//
// Routine.cs
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
using System.Text;
using System.Linq;
using Lury.Compiling.Utils;

namespace Lury.Compiling.IR
{
    public class Routine
    {
        #region -- Private Fields --

        private readonly IReadOnlyList<Routine> children;
        private readonly IReadOnlyList<Instruction> instructions;
        private readonly IReadOnlyDictionary<string, int> jumpLabels;
        private readonly IReadOnlyDictionary<int, CodePosition> codePosition;

        #endregion

        #region -- Public Properties --

        public string Name { get; private set; }

        public int AllocSize { get; private set; }

        public IReadOnlyList<Routine> Children { get { return this.children; } }

        public IReadOnlyList<Instruction> Instructions { get { return this.instructions; } }

        public IReadOnlyDictionary<string, int> JumpLabels { get { return this.jumpLabels; } }

        public IReadOnlyDictionary<int, CodePosition> CodePosition { get { return this.codePosition; } }

        #endregion

        #region -- Constructors --

        public Routine(string name,
                       int allocSize,
                       IList<Routine> children,
                       IList<Instruction> instructions,
                       IDictionary<string, int> jumpLabels,
                       IDictionary<int, CodePosition> codePosition)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (allocSize < 0)
                throw new ArgumentOutOfRangeException("allocSize");

            this.Name = name;
            this.AllocSize = allocSize;
            this.children = (IReadOnlyList<Routine>)children ?? new Routine[0];
            this.instructions = (IReadOnlyList<Instruction>)instructions ?? new Instruction[0];
            this.jumpLabels = (IReadOnlyDictionary<string, int>)jumpLabels ?? new Dictionary<string, int>();

            this.codePosition =
                (IReadOnlyDictionary<int, CodePosition>)(
                (codePosition == null) ? new SortedDictionary<int, CodePosition>() :
                (codePosition is SortedDictionary<int, CodePosition>) ? codePosition :
                new SortedDictionary<int, CodePosition>(codePosition));
        }

        public Routine(string name)
            : this(name, 0, null, null, null, null)
        {
        }

        #endregion

        #region -- Public Methods --

        public string Dump()
        {
            StringBuilder sb = new StringBuilder();
            int positionWidth = GetPositionWidth(this);

            this.DumpPrivate(sb, 0, positionWidth);

            return sb.ToString();
        }

        #endregion

        #region -- Private Methods --

        private void DumpPrivate(StringBuilder sb, int indent, int positionWidth)
        {
            const int indentWidth = 4;

            this.DumpPosition(sb, positionWidth, -1);
            sb.Append(' ', indentWidth * indent);
            sb.Append("::");
            sb.AppendLine(this.Name);
            indent++;

            sb.Append(' ', positionWidth + indentWidth * indent);
            sb.AppendFormat("# alloc {0} register", this.AllocSize);
            sb.AppendLine();

            foreach (var child in this.children)
                child.DumpPrivate(sb, indent, positionWidth);

            var labels = this.jumpLabels.ToDictionary(k => k.Value, v => v.Key);

            for (int i = 0; i < this.instructions.Count; i++)
            {
                if (labels.ContainsKey(i))
                {
                    sb.Append(' ', positionWidth + indentWidth * (indent - 1));
                    sb.Append(":");
                    sb.AppendLine(labels[i]);
                }

                this.DumpPosition(sb, positionWidth, i);
                
                var inst = this.instructions[i];

                sb.Append(' ', indentWidth * indent);
                if (inst.Destination != Instruction.NoAssign)
                {
                    sb.Append('%');
                    sb.Append(inst.Destination);
                    sb.Append(" = ");
                }
                sb.Append(inst.Operation.ToString().ToLower());
                
                if (inst.Parameters.Count > 0)
                {
                    sb.Append(' ');
                    sb.Append(string.Join(" ", inst.Parameters));
                }
                sb.AppendLine();
            }
        }

        private void DumpPosition(StringBuilder sb, int positionWidth, int line)
        {
            if (this.codePosition.ContainsKey(line))
            {
                var pos = this.codePosition[line];
                var str = string.Format(
                    "L{0},{1}{2}",
                    pos.Position.Line,
                    pos.Position.Column,
                    pos.Length > 0 ? "(" + pos.Length + ")" : "");

                sb.Append(str);
                sb.Append(' ', Math.Max(positionWidth - str.Length, 1));
            }
            else
            {
                sb.Append(' ', positionWidth);
            }
        }

        #endregion

        #region -- Private Static Methods --

        private static int GetPositionWidth(Routine routine)
        {
            int res;
            if (routine.codePosition.Count > 0)
            {
                var p = routine.codePosition.Select(k => k.Value).ToArray();
                int lmax = Math.Max((int)Math.Log10(p.Max(c => c.Length)), 0);
                int cmax = (int)Math.Log10(p.Max(c => c.Position.Column));
                int rmax = (int)Math.Log10(p.Max(c => c.Position.Line));
                res = lmax + cmax + rmax + 3 + 6;
            }
            else
                res = 0;

            foreach (var child in routine.children)
            {
                int cres = GetPositionWidth(child);

                if (res < cres)
                    res = cres;
            }

            return res;
        }


        #endregion
    }
}
