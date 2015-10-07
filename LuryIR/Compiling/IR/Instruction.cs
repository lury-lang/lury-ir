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
    public class Instruction
    {
        #region -- Public Properties --

        public string Destination { get; private set; }

        public Operation Operation { get; private set; }

        public IReadOnlyList<Parameter> Parameters { get; private set; }

        #endregion

        #region -- Constructors --

        public Instruction(string destination, Operation operation, params Parameter[] parameters)
        {
            if (!Enum.IsDefined(typeof(Operation), operation))
                throw new ArgumentOutOfRangeException("operation");

            this.Destination = destination;
            this.Operation = operation;
            this.Parameters = parameters ?? new Parameter[0];
        }

        public Instruction(string destination, Operation operation)
            : this(destination, operation, null)
        {
        }

        public Instruction(Operation operation, params Parameter[] parameters)
            : this(null, operation, parameters)
        {
        }

        public Instruction(Operation operation)
            : this(null, operation, null)
        {
        }

        #endregion
    }
}
