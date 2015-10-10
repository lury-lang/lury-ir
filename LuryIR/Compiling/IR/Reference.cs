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
using System.Text;
using System.Threading.Tasks;

namespace Lury.Compiling.IR
{
    public class Reference
    {
        #region -- Public Properties --

        public bool IsRegister { get; private set; }

        public int Register { get; private set; }

        public string Name { get; private set; }

        public IReadOnlyList<string> Children { get; private set; }

        public bool HasChildren { get; private set; }

        #endregion

        #region -- Constructors --

        public Reference(int register, params string[] children)
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

        public Reference(string name, params string[] children)
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
    }
}
