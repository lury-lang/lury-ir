//
// LuryObject.cs
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
using Lury.Engine.Intrinsic;

namespace Lury.Engine
{
    public class LuryObject
    {
        #region -- Public Fields --

        public static readonly LuryObject Nil = new LuryObject(null, null, freeze: true);

        #endregion

        #region -- Private Fields --

        private readonly Dictionary<string, LuryObject> Members = new Dictionary<string, LuryObject>(0);
        private readonly List<LuryObject> annotations;
        private readonly string luryTypeName;
        private readonly object value;

        private bool isFrozen;

        #endregion

        #region -- Public Properties --

        public string LuryTypeName { get; }

        public object Value { get; }

        public IEnumerable<LuryObject> Annotations => this.annotations;

        public bool IsFrozen { get; private set; }

        #endregion

        #region -- Constructors --

        public LuryObject(string luryTypeName, object value, bool freeze = false, IEnumerable<LuryObject> annotations = null)
        {
            this.LuryTypeName = luryTypeName;
            this.Value = value;
            this.annotations = new List<LuryObject>(0);

            if (annotations != null)
                this.annotations.AddRange(annotations);

            if (freeze)
                this.Freeze();
        }

        #endregion

        #region -- Public Methods --

        public void SetMember(string name, LuryObject value)
        {
            if (this.IsFrozen)
                throw new InvalidOperationException();

            if (this.Members.ContainsKey(name))
                this.Members[name] = value;
            else
                this.Members.Add(name, value);
        }

        public LuryObject GetMember(string name, ProgramContext context)
        {
            if (this.Members.ContainsKey(name))
                return this.Members[name];

            else if (this.LuryTypeName != null && context.HasMember(this.LuryTypeName))
            {
                return context[this.LuryTypeName].GetMemberNoRecursion(name);
            }
            else
                //throw new LuryException(LuryExceptionType.NameIsNotFound);
                throw new InvalidOperationException();
        }

        public void AddAnnotation(LuryObject obj)
        {
            if (this.IsFrozen)
                throw new InvalidOperationException();

            this.annotations.Add(obj);
        }

        public bool HasMember(string member)
        {
            return this.Members.ContainsKey(member);
        }

        public void Freeze()
        {
            this.IsFrozen = true;
        }

        #endregion

        #region -- Private Methods --

        private LuryObject GetMemberNoRecursion(string name)
        {
            if (this.Members.ContainsKey(name))
                return this.Members[name];
            else
                //throw new LuryException(LuryExceptionType.NameIsNotFound);
                throw new InvalidOperationException();
        }

        #endregion
    }
}
