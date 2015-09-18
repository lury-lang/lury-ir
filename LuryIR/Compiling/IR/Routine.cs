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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lury.Compiling.Utils;

namespace Lury.Compiling.IR
{
    public class Routine
    {
        #region -- Private Fields --

        private readonly IList<Routine> children;
        private readonly IList<Instruction> instructions;
        private readonly IDictionary<string, int> jumpLabels;
        private readonly IDictionary<int, CodePosition> codePosition;

        #endregion

        #region -- Public Properties --

        public string Name { get; private set; }

        public IReadOnlyList<Routine> Children { get { return (IReadOnlyList<Routine>)this.children; } }

        public IReadOnlyList<Instruction> Instructions { get { return (IReadOnlyList<Instruction>)this.instructions; } }

        public IReadOnlyDictionary<string, int> JumpLabels { get { return (IReadOnlyDictionary<string, int>)this.jumpLabels; } }

        public IReadOnlyDictionary<int, CodePosition> CodePosition { get { return (IReadOnlyDictionary<int, CodePosition>)this.codePosition; } }

        #endregion

        #region -- Constructors --

        public Routine(string name,
                       IList<Routine> children,
                       IList<Instruction> instructions,
                       IDictionary<string, int> jumpLabels,
                       IDictionary<int, CodePosition> codePosition)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.Name = name;
            this.children = children ?? new Routine[0];
            this.instructions = instructions ?? new Instruction[0];
            this.jumpLabels = jumpLabels ?? new Dictionary<string, int>();

            this.codePosition =
                (codePosition == null) ? new SortedDictionary<int, CodePosition>() :
                (codePosition is SortedDictionary<int, CodePosition>) ? codePosition :
                new SortedDictionary<int, CodePosition>(codePosition);
        }

        public Routine(string name)
            : this(name, null, null, null, null)
        {
        }

        #endregion

        #region -- Public Methods --

        

        #endregion
    }
}
