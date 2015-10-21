//
// RoutineVerifier.cs
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
using Lury.Compiling.Logger;

namespace Lury.Compiling.IR
{
    public class RoutineVerifier
    {
        #region -- Public Properties --

        public OutputLogger Logger { get; private set; }

        public Routine TargetRoutine { get; private set; }

        #endregion

        #region -- Constructors --

        public RoutineVerifier(Routine routine, OutputLogger logger)
        {
            this.TargetRoutine = routine;
            this.Logger = logger;
        }

        public RoutineVerifier(Routine routine)
            : this(routine, new OutputLogger())
        {
        }

        #endregion

        #region -- Public Methods --

        public bool Verify()
        {
            this.CheckNullList(this.TargetRoutine);
            this.CheckRegisterCount(this.TargetRoutine);
            this.CheckUnusedRegister(this.TargetRoutine);

            return !this.Logger.ErrorOutputs.Any();
        }

        #endregion

        #region -- Private Methods --

        private void CheckNullList(Routine routine)
        {
            if (routine.Children.Any(_ => _ == null))
                this.Logger.ReportError(VerifyError.ChildrenHasNull, appendix: "at " + routine.Name);

            if (routine.Instructions.Any(_ => _ == null))
                this.Logger.ReportError(VerifyError.InstructionsHasNull, appendix: "at " + routine.Name);

            foreach (var child in routine.Children)
                this.CheckNullList(child);
        }

        private void CheckRegisterCount(Routine routine)
        {
            int maxDestNum = routine.Instructions.Max(i => i.Destination);
            int maxParamNum = routine.Instructions.SelectMany(i => i.Parameters.Select(p => p.Value)
                                                                               .OfType<Reference>()
                                                                               .Where(r => r.IsRegister)
                                                                               .Select(r => r.Register)).Max();

            int requireCount = Math.Max(maxDestNum, maxParamNum) + 1;

            if (routine.RegisterCount < requireCount)
                this.Logger.ReportError(VerifyError.RegisterNotEnough, appendix: "at " + routine.Name);
            else if (routine.RegisterCount > requireCount)
                this.Logger.ReportWarn(VerifyWarn.RegisterTooEnough, appendix: "at " + routine.Name);

            foreach (var child in routine.Children)
                this.CheckRegisterCount(child);
        }

        private void CheckUnusedRegister(Routine routine)
        {
            var array = routine.Instructions.SelectMany(i => i.Parameters.Select(p => p.Value)
                                                                         .OfType<Reference>()
                                                                         .Where(r => r.IsRegister)
                                                                         .Select(r => r.Register))
                                            .OrderBy(i => i)
                                            .Distinct()
                                            .ToArray();

            if (array.Length < routine.RegisterCount)
                this.Logger.ReportWarn(VerifyWarn.UnusedRegisterExists, appendix: "at " + routine.Name);


            foreach (var child in routine.Children)
                this.CheckUnusedRegister(child);
        }

        #endregion
    }
}
