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

        public IList<Routine> Children { get { return this.children; } }

        public IList<Instruction> Instructions { get { return this.instructions; } }

        public IDictionary<string, int> JumpLabels { get { return this.jumpLabels; } }

        public IDictionary<int, CodePosition> CodePosition { get { return this.codePosition; } }

        #endregion

        #region -- Constructors --

        public RoutineBuilder(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.Name = name;
            this.children = new List<Routine>();
            this.instructions = new List<Instruction>();
            this.jumpLabels = new Dictionary<string, int>();
            this.codePosition = new SortedDictionary<int, CodePosition>();
        }

        #endregion

        #region -- Public Methods --

        public void NoOperation()
        {
            this.instructions.Add(new Instruction(Operation.Nop));
        }

        public int Load(Parameter x, int dest)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (dest == NoAssign)
                throw new ArgumentOutOfRangeException("dest");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Load, x));
            return dest;
        }

        public void Store(int dest, Parameter x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (dest < 0)
                throw new ArgumentOutOfRangeException("dest");
            
            this.instructions.Add(new Instruction(Operation.Store, Parameter.GetReference(new Reference(dest)), x));
        }

        public void BeginScope()
        {
            this.instructions.Add(new Instruction(Operation.Scope));
        }

        public void EndScope()
        {
            this.instructions.Add(new Instruction(Operation.Break));
        }

        public int Increment(Parameter x, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Inc, x));
            return dest;
        }

        public int Decrement(Parameter x, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Dec, x));
            return dest;
        }

        public int Positive(Parameter x, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Pos, x));
            return dest;
        }

        public int Negative(Parameter x, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Neg, x));
            return dest;
        }

        public int BitwiseNot(Parameter x, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Bnot, x));
            return dest;
        }

        public int Power(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Pow, x, y));
            return dest;
        }

        public int Multiply(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Mul, x, y));
            return dest;
        }

        public int Divide(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Div, x, y));
            return dest;
        }

        public int IntDivide(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Idiv, x, y));
            return dest;
        }

        public int Modulo(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Mod, x, y));
            return dest;
        }

        public int Add(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Add, x, y));
            return dest;
        }

        public int Subtract(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Sub, x, y));
            return dest;
        }

        public int Concat(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Con, x, y));
            return dest;
        }

        public int ShiftLeft(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Shl, x, y));
            return dest;
        }

        public int ShiftRight(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Shr, x, y));
            return dest;
        }

        public int BitwiseAnd(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.And, x, y));
            return dest;
        }

        public int BitwiseXor(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Xor, x, y));
            return dest;
        }

        public int BitwiseOr(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Or, x, y));
            return dest;
        }

        public int CompareLessThan(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Lt, x, y));
            return dest;
        }

        public int CompareGreaterThan(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Gt, x, y));
            return dest;
        }

        public int CompareLessThanOrEquals(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Ltq, x, y));
            return dest;
        }

        public int CompareGreaterThanOrEquals(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Gtq, x, y));
            return dest;
        }

        public int CompareEquals(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Eq, x, y));
            return dest;
        }

        public int CompareNotEquals(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Neq, x, y));
            return dest;
        }

        public int CompareIs(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Is, x, y));
            return dest;
        }

        public int CompareIsNot(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Isn, x, y));
            return dest;
        }

        public int LogicalNot(Parameter x, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");
            
            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Lnot, x));
            return dest;
        }

        public int LogicalAnd(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Land, x, y));
            return dest;
        }

        public int LogicalOr(Parameter x, Parameter y, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (y == null)
                throw new ArgumentNullException("y");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Lor, x, y));
            return dest;
        }

        public void Return(Parameter x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            this.instructions.Add(new Instruction(Operation.Ret, x));
        }

        public void Return()
        {
            this.instructions.Add(new Instruction(Operation.Ret, Parameter.Nil));
        }

        public void Yield(Parameter x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            this.instructions.Add(new Instruction(Operation.Yield, x));
        }

        public void Yield()
        {
            this.instructions.Add(new Instruction(Operation.Yield, Parameter.Nil));
        }

        public void Throw(Parameter x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            this.instructions.Add(new Instruction(Operation.Throw, x));
        }

        public int Call(Parameter x, int dest, params Parameter[] parameters)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (dest == NoAssign)
                throw new ArgumentOutOfRangeException("dest");

            if (parameters == null)
                throw new ArgumentNullException("parameters");

            if (parameters.Any(p => p == null))
                throw new ArgumentNullException("parameters");

            this.CheckAndAssignRegister(ref dest);

            var callParams = new Parameter[parameters.Length + 1];
            callParams[0] = x;
            Array.Copy(parameters, 0, callParams, 1, parameters.Length);
            this.instructions.Add(new Instruction(dest, Operation.Call, callParams));
            return dest;
        }

        public void CallWithoutReturn(Parameter x, params Parameter[] parameters)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (parameters == null)
                throw new ArgumentNullException("parameters");

            if (parameters.Any(p => p == null))
                throw new ArgumentNullException("parameters");

            var callParams = new Parameter[parameters.Length + 1];
            callParams[0] = x;
            Array.Copy(parameters, 0, callParams, 1, parameters.Length);
            this.instructions.Add(new Instruction(Operation.Call, callParams));
        }

        public int Eval(Parameter x, int dest = NoAssign)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Eval, x));
            return dest;
        }

        public void Jump(string label)
        {
            this.instructions.Add(new Instruction(Operation.Jmp, Parameter.GetLabel(label)));
        }

        public void JumpIfTrue(string label, Parameter x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            this.instructions.Add(new Instruction(Operation.Jmpt, Parameter.GetLabel(label), x));
        }

        public void JumpIfFalse(string label, Parameter x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            this.instructions.Add(new Instruction(Operation.Jmpf, Parameter.GetLabel(label), x));
        }

        public void JumpIfNil(string label, Parameter x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            this.instructions.Add(new Instruction(Operation.Jmpn, Parameter.GetLabel(label), x));
        }

        public int Catch(string label, int dest = NoAssign)
        {
            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Catch, Parameter.GetLabel(label)));
            return dest;
        }

        public void Overlook()
        {
            this.instructions.Add(new Instruction(Operation.Ovlok));
        }

        public int CreateFunction(Routine routine, int dest)
        {
            if (routine == null)
                throw new ArgumentNullException("routine");

            if (dest == NoAssign)
                throw new ArgumentOutOfRangeException("dest");

            this.CheckAndAssignRegister(ref dest);
            this.instructions.Add(new Instruction(dest, Operation.Func, Parameter.GetLabel(routine.Name)));
            return dest;
        }

        public int CreateClass(Routine initializeRoutine, int dest, params Parameter[] derivation)
        {
            if (initializeRoutine == null)
                throw new ArgumentNullException("initializeRoutine");
            
            if (dest == NoAssign)
                throw new ArgumentOutOfRangeException("dest");

            if (derivation == null)
                throw new ArgumentNullException("derivation");

            if (derivation.Any(p => p == null))
                throw new ArgumentNullException("derivation");

            this.CheckAndAssignRegister(ref dest);

            var classParams = new Parameter[derivation.Length + 1];
            classParams[0] = Parameter.GetLabel(initializeRoutine.Name);
            Array.Copy(derivation, 0, classParams, 1, derivation.Length);
            this.instructions.Add(new Instruction(dest, Operation.Class, classParams));
            return dest;
        }

        public void SetAnnotation(Parameter x, Parameter target)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (target == null)
                throw new ArgumentNullException("target");

            this.instructions.Add(new Instruction(Operation.Annot, target, x));
        }

        public Routine Build()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region -- Private Methods --

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

            throw new ArgumentOutOfRangeException("register");
        }

        #endregion
    }
}
