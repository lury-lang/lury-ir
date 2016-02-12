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
            this.CheckParameterCount(this.TargetRoutine);
            this.CheckJumpLabel(this.TargetRoutine);

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

        private void CheckParameterCount(Routine routine)
        {
            foreach (var inst in routine.Instructions)
            {
                if (!JudgeForParameterCount(inst))
                    this.Logger.ReportError(VerifyError.IllegalParameterCount, appendix: "at " + routine.Name);
            }

            foreach (var child in routine.Children)
                this.CheckParameterCount(child);
        }

        private void CheckJumpLabel(Routine routine)
        {
            var labels = routine.Instructions.SelectMany(i => i.Parameters)
                                             .Where(p => p.Type == ParameterType.Label)
                                             .Select(p => p.Value)
                                             .Cast<string>()
                                             .Distinct()
                                             .ToArray();

            if (!labels.All(l => routine.JumpLabels.ContainsKey(l) || routine.Children.Any(r => r.Name == l)))
                this.Logger.ReportError(VerifyError.UndefinedLabelIsReferred, appendix: "at " + routine.Name);

            foreach (var child in routine.Children)
                this.CheckJumpLabel(child);
        }

        #endregion

        #region -- Private Static Methods --

        private static bool JudgeForParameterCount(Instruction inst)
        {
            switch (inst.Operation)
            {
                // 0 params
                case Operation.Nop:
                case Operation.Scope:
                case Operation.Break:
                case Operation.Ovlok:
                    return (inst.Parameters.Count == 0);

                // 1 param
                case Operation.Load:
                case Operation.Inc:
                case Operation.Dec:
                case Operation.Pos:
                case Operation.Neg:
                case Operation.Inv:
                case Operation.Not:
                case Operation.Throw:
                case Operation.Eval:
                case Operation.Jmp:
                case Operation.Catch:
                case Operation.Func:
                    return (inst.Parameters.Count == 1);

                // 2 params
                case Operation.Store:
                case Operation.Pow:
                case Operation.Mul:
                case Operation.Div:
                case Operation.Idiv:
                case Operation.Mod:
                case Operation.Add:
                case Operation.Sub:
                case Operation.Con:
                case Operation.Shl:
                case Operation.Shr:
                case Operation.And:
                case Operation.Xor:
                case Operation.Or:
                case Operation.Lt:
                case Operation.Gt:
                case Operation.Ltq:
                case Operation.Gtq:
                case Operation.Eq:
                case Operation.Neq:
                case Operation.Is:
                case Operation.Isn:
                case Operation.Land:
                case Operation.Lor:
                case Operation.Jmpt:
                case Operation.Jmpf:
                case Operation.Jmpn:
                case Operation.Annot:
                    return (inst.Parameters.Count == 2);

                // 0 or 1 params
                case Operation.Ret:
                case Operation.Yield:
                    return (inst.Parameters.Count == 0 || inst.Parameters.Count == 1);

                // 1 or more
                case Operation.Call:
                case Operation.Class:
                    return (inst.Parameters.Count >= 1);

                default:
                    throw new ArgumentOutOfRangeException("inst");
            }
        }

        #endregion
    }
}
