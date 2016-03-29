//
// ProgramContext.cs
//
// Author:
//       Tomona Nanase <nanase@users.noreply.github.com>
//
// The MIT License (MIT)
//
// Copyright (c) 2016 Tomona Nanase
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

namespace Lury.Engine
{
    public class ProgramContext
    {
        #region -- Private Fields --

        private readonly ProgramContext parent;
        private readonly Dictionary<string, LuryObject> members = new Dictionary<string, LuryObject>(0);

        #endregion

        #region -- Public Indexers --

        public LuryObject this[string member]
        {
            get
            {
                var context = this;

                while (context != null)
                {
                    if (context.members.ContainsKey(member))
                        return context.members[member];
                    else
                        context = context.parent;
                }

                //throw new LuryException(LuryExceptionType.NameIsNotFound);
                throw new InvalidOperationException();
            }
            set
            {
                var context = this;

                while (context != null)
                {
                    if (context.members.ContainsKey(member))
                    {
                        context.members[member] = value;
                        return;
                    }
                    else
                        context = context.parent;
                }

                this.members.Add(member, value);
            }
        }

        #endregion

        #region -- Public Properties --

        public ProgramContext Parent => this.parent;

        #endregion

        #region -- Constructors --

        public ProgramContext(ProgramContext parent)
        {
            this.parent = parent;
        }

        public ProgramContext()
            : this(null)
        {
        }

        #endregion

        #region -- Public Methods --

        public bool HasMember(string name)
        {
            var context = this;

            while (context != null)
            {
                if (context.members.ContainsKey(name))
                    return true;
                else
                    context = context.parent;
            }

            return this.members.ContainsKey(name);
        }

        //public void SetMemberNoRecursion(string name, LuryObject value)
        //{
        //    this.Members.Add(name, value);
        //}

        #endregion
    }
}
