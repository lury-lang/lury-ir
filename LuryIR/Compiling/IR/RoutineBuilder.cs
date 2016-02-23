//
// RoutineBuilder.cs
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
using Lury.Compiling.Utils;

namespace Lury.Compiling.IR
{
    public class RoutineBuilder
    {
        #region -- Public Fields --

        public const int NoAssign = Instruction.NoAssign;
        public const int NewAssign = -2;

        #endregion

        #region -- Private Fields --

        private readonly IList<Routine> children;
        private readonly IList<Instruction> instructions;
        private readonly IDictionary<string, int> jumpLabels;
        private readonly IDictionary<int, CodePosition> codePosition;

        private int registerCount = 0;

        #endregion

        #region -- Public Properties --

        public string Name { get; private set; }

        public string SourceName { get; private set; }

        public IList<Routine> Children => this.children;

        public IList<Instruction> Instructions => this.instructions;

        public IDictionary<string, int> JumpLabels => this.jumpLabels;

        public IDictionary<int, CodePosition> CodePosition => this.codePosition;

        #endregion

        #region -- Constructors --

        public RoutineBuilder(string name, string sourceName)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (sourceName == null)
                throw new ArgumentNullException(nameof(sourceName));

            this.Name = name;
            this.SourceName = sourceName;
            this.children = new List<Routine>();
            this.instructions = new List<Instruction>();
            this.jumpLabels = new Dictionary<string, int>();
            this.codePosition = new SortedDictionary<int, CodePosition>();
        }

        #endregion

        #region -- Public Methods --

        public void NoOperation()
            => this.instructions.Add(new Instruction(Operation.Nop));

        public int Load(Parameter from, int dest)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));
            
            if (dest == NoAssign)
                throw new ArgumentOutOfRangeException(nameof(dest));

            if (from.Type == ParameterType.Label)
                throw new ArgumentOutOfRangeException(nameof(from));

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Load, from));
            return dest;
        }

        public void Store(int from, Parameter dest)
        {
            if (dest == null)
                throw new ArgumentNullException(nameof(from));

            if (from < 0)
                throw new ArgumentOutOfRangeException(nameof(dest));
            
            if (dest.Type != ParameterType.Reference)
                throw new ArgumentOutOfRangeException(nameof(dest));

            this.instructions.Add(new Instruction(Operation.Store, Parameter.GetRegister(from), dest));
        }
        
        public void Remove(Parameter x)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            if (x.Type != ParameterType.Reference)
                throw new ArgumentOutOfRangeException(nameof(x));

            this.instructions.Add(new Instruction(Operation.Remv, x));
        }

        public int Copy(int from, int dest)
        {
            if (from < 0)
                throw new ArgumentOutOfRangeException(nameof(from));

            if (dest < 0)
                throw new ArgumentOutOfRangeException(nameof(dest));

            if (from == dest)
                throw new ArgumentOutOfRangeException(nameof(dest));

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(Operation.Copy, Parameter.GetRegister(from), Parameter.GetRegister(dest)));
            return dest;
        }

        public int Resolve(Parameter x, int dest)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            if (x.Type != ParameterType.Reference)
                throw new ArgumentOutOfRangeException(nameof(x));

            if (dest == NoAssign)
                throw new ArgumentOutOfRangeException(nameof(dest));

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(Operation.Rslv, x));
            return dest;
        }

        public int Has(Parameter x, int dest)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            if (x.Type != ParameterType.Reference)
                throw new ArgumentOutOfRangeException(nameof(x));

            if (dest == NoAssign)
                throw new ArgumentOutOfRangeException(nameof(dest));

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(Operation.Has, x));
            return dest;
        }

        public void BeginScope()
            => this.instructions.Add(new Instruction(Operation.Scope));

        public void EndScope()
            => this.instructions.Add(new Instruction(Operation.Break));

        public int Increment(Parameter x, int dest = NoAssign)
            => this.AddUnary(Operation.Inc, x, dest);

        public int Decrement(Parameter x, int dest = NoAssign)
            => this.AddUnary(Operation.Dec, x, dest);

        public int Positive(Parameter x, int dest = NoAssign)
            => this.AddUnary(Operation.Pos, x, dest);

        public int Negative(Parameter x, int dest = NoAssign)
            => this.AddUnary(Operation.Neg, x, dest);

        public int Invert(Parameter x, int dest = NoAssign)
            => this.AddUnary(Operation.Inv, x, dest);

        public int Power(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Pow, x, y, dest);

        public int Multiply(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Mul, x, y, dest);

        public int Divide(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Div, x, y, dest);

        public int IntDivide(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Idiv, x, y, dest);

        public int Modulo(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Mod, x, y, dest);

        public int Add(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Add, x, y, dest);

        public int Subtract(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Sub, x, y, dest);

        public int Concat(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Con, x, y, dest);

        public int ShiftLeft(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Shl, x, y, dest);

        public int ShiftRight(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Shr, x, y, dest);

        public int BitwiseAnd(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.And, x, y, dest);

        public int BitwiseXor(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Xor, x, y, dest);

        public int BitwiseOr(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Or, x, y, dest);

        public int LessThan(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Lt, x, y, dest);

        public int GreaterThan(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Gt, x, y, dest);

        public int LessThanOrEquals(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Ltq, x, y, dest);

        public int GreaterThanOrEquals(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Gtq, x, y, dest);

        public int Equals(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Eq, x, y, dest);

        public int NotEquals(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Neq, x, y, dest);

        public int Is(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Is, x, y, dest);

        public int IsNot(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Isn, x, y, dest);

        public int Not(Parameter x, int dest = NoAssign)
            => this.AddUnary(Operation.Not, x, dest);

        public int LogicalAnd(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Land, x, y, dest);

        public int LogicalOr(Parameter x, Parameter y, int dest = NoAssign)
            => this.AddBinary(Operation.Lor, x, y, dest);

        public void Return(Parameter x)
        {
            this.CheckUnaryParameter(x);
            this.instructions.Add(new Instruction(Operation.Ret, x));
        }

        public void Return()
            => this.instructions.Add(new Instruction(Operation.Ret, Parameter.Nil));

        public void Yield(Parameter x)
        {
            this.CheckUnaryParameter(x);
            this.instructions.Add(new Instruction(Operation.Yield, x));
        }

        public void Yield()
            => this.instructions.Add(new Instruction(Operation.Yield, Parameter.Nil));


        public void Throw(Parameter x)
        {
            this.CheckUnaryParameter(x);
            this.instructions.Add(new Instruction(Operation.Throw, x));
        }

        public int Call(Parameter x, int dest, params Parameter[] parameters)
        {
            this.CheckUnaryParameter(x);

            if (dest == NoAssign)
                throw new ArgumentOutOfRangeException(nameof(dest));

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (parameters.Any(p => p == null))
                throw new ArgumentNullException(nameof(parameters));

            if (parameters.Any(p => p.Type == ParameterType.Reference ||
                                    p.Type == ParameterType.Label))
                throw new ArgumentOutOfRangeException(nameof(parameters));

            this.CheckAndAssignRegister(ref dest);

            var callParams = new Parameter[parameters.Length + 1];
            callParams[0] = x;
            Array.Copy(parameters, 0, callParams, 1, parameters.Length);
            this.instructions.Add(new Instruction(dest, Operation.Call, callParams));
            return dest;
        }

        public void CallWithoutReturn(Parameter x, params Parameter[] parameters)
        {
            this.CheckUnaryParameter(x);

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (parameters.Any(p => p == null))
                throw new ArgumentNullException(nameof(parameters));

            if (parameters.Any(p => p.Type == ParameterType.Reference ||
                                    p.Type == ParameterType.Label))
                throw new ArgumentOutOfRangeException(nameof(parameters));

            var callParams = new Parameter[parameters.Length + 1];
            callParams[0] = x;
            Array.Copy(parameters, 0, callParams, 1, parameters.Length);
            this.instructions.Add(new Instruction(Operation.Call, callParams));
        }

        public int Eval(Parameter x, int dest = NoAssign)
        {
            this.CheckUnaryParameter(x);
            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Eval, x));
            return dest;
        }

        public void Jump(string label)
            => this.instructions.Add(new Instruction(Operation.Jmp, Parameter.GetLabel(label)));

        public void JumpIfTrue(string label, Parameter x)
        {
            this.CheckUnaryParameter(x);
            this.instructions.Add(new Instruction(Operation.Jmpt, Parameter.GetLabel(label), x));
        }

        public void JumpIfFalse(string label, Parameter x)
        {
            this.CheckUnaryParameter(x);
            this.instructions.Add(new Instruction(Operation.Jmpf, Parameter.GetLabel(label), x));
        }

        public void JumpIfNil(string label, Parameter x)
        {
            this.CheckUnaryParameter(x);
            this.instructions.Add(new Instruction(Operation.Jmpn, Parameter.GetLabel(label), x));
        }

        public int Catch(string label, int dest = NoAssign)
        {
            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Catch, Parameter.GetLabel(label)));
            return dest;
        }

        public void Overlook()
            => this.instructions.Add(new Instruction(Operation.Ovlok));

        public int CreateFunction(Routine routine, int dest)
        {
            if (routine == null)
                throw new ArgumentNullException(nameof(routine));

            if (dest == NoAssign)
                throw new ArgumentOutOfRangeException(nameof(dest));

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Func, Parameter.GetLabel(routine.Name)));
            return dest;
        }

        public int CreateClass(Routine initializeRoutine, int dest, params Parameter[] derivation)
        {
            if (initializeRoutine == null)
                throw new ArgumentNullException(nameof(initializeRoutine));
            
            if (dest == NoAssign)
                throw new ArgumentOutOfRangeException(nameof(dest));

            if (derivation == null)
                throw new ArgumentNullException(nameof(derivation));

            if (derivation.Any(p => p == null))
                throw new ArgumentNullException(nameof(derivation));

            if (derivation.Any(p => p.Type == ParameterType.Reference ||
                                    p.Type == ParameterType.Label))
                throw new ArgumentOutOfRangeException(nameof(derivation));

            this.CheckAndAssignRegister(ref dest);

            var classParams = new Parameter[derivation.Length + 1];
            classParams[0] = Parameter.GetLabel(initializeRoutine.Name);
            Array.Copy(derivation, 0, classParams, 1, derivation.Length);
            this.instructions.Add(new Instruction(dest, Operation.Class, classParams));
            return dest;
        }

        public void SetAnnotation(Parameter x, Parameter target)
        {
            this.CheckBinaryParameters(x, target);
            this.instructions.Add(new Instruction(Operation.Annot, target, x));
        }

        public Routine Build()
            => new Routine(
                this.Name,
                this.registerCount,
                this.children,
                this.instructions,
                this.jumpLabels,
                this.codePosition);

        public string GetLabelName(string name)
        {
            if (!this.jumpLabels.ContainsKey(name))
                return name;

            int i = 0;
            string res;

            while (this.jumpLabels.ContainsKey(res = name + i.ToString()))
                i++;

            return res;
        }

        public void SetLabelAtCurrent(string name)
            => this.jumpLabels.Add(name, this.instructions.Count);

        public void SetRoutinePosition(int line, int column, int length = 0)
        {
            if (line < 1)
                throw new ArgumentOutOfRangeException(nameof(line));

            if (column < 1)
                throw new ArgumentOutOfRangeException(nameof(column));
            
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            this.codePosition.Add(
                -1,
                new Utils.CodePosition(this.SourceName, new CharPosition(line, column), length));
        }

        public void SetPositionAtCurrent(int line, int column, int length = 0)
        {
            if (line < 1)
                throw new ArgumentOutOfRangeException(nameof(line));

            if (column < 1)
                throw new ArgumentOutOfRangeException(nameof(column));

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            this.codePosition.Add(
                this.instructions.Count - 1,
                new Utils.CodePosition(this.SourceName, new CharPosition(line, column), length));
        }

        #endregion

        #region -- Private Methods --

        private int AddUnary(Operation op, Parameter x, int dest)
        {
            this.CheckUnaryParameter(x);
            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, op, x));
            return dest;
        }

        private int AddBinary(Operation op, Parameter x, Parameter y, int dest)
        {
            this.CheckBinaryParameters(x, y);
            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, op, x, y));
            return dest;
        }

        private void CheckUnaryParameter(Parameter x)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            if (x.Type == ParameterType.Reference ||
                x.Type == ParameterType.Label)
                throw new ArgumentOutOfRangeException(nameof(x));
        }

        private void CheckBinaryParameters(Parameter x, Parameter y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            if (y == null)
                throw new ArgumentNullException(nameof(y));

            if (x.Type == ParameterType.Reference ||
                x.Type == ParameterType.Label)
                throw new ArgumentOutOfRangeException(nameof(x));

            if (y.Type == ParameterType.Reference ||
                y.Type == ParameterType.Label)
                throw new ArgumentOutOfRangeException(nameof(y));
        }

        private void CheckAndAssignRegister(ref int register)
        {
            if (register == NoAssign || register >= 0)
                return;

            if (register == NewAssign)
            {
                register = this.registerCount;
                registerCount++;
                return;
            }
            else
                throw new ArgumentOutOfRangeException(nameof(register));
        }

        #endregion
    }
}
