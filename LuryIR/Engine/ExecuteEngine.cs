//
// ExecuteEngine.cs
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
using System.Linq;
using System.Numerics;
using System.Reflection;
using Lury.Compiling.IR;
using Lury.Engine.Intrinsic;
using static Lury.Engine.Intrinsic.IntrinsicIntrinsic;

namespace Lury.Engine
{
    public class ExecuteEngine
    {
        #region -- Private Fields --

        private readonly Routine mainRoutine;
        private readonly Dictionary<string, MethodInfo> intrinsicMap;
        private readonly ProgramContext globalContext;

        #endregion

        #region -- Public Properties --

        public Routine MainRoutine => this.mainRoutine;

        #endregion

        #region -- Constructors --

        public ExecuteEngine(Routine mainRoutine)
        {
            if (mainRoutine == null)
                throw new ArgumentNullException(nameof(mainRoutine));

            this.mainRoutine = mainRoutine;
            this.intrinsicMap = new Dictionary<string, MethodInfo>();
            this.globalContext = new ProgramContext();
            this.InitializeGlobalContext();
        }

        #endregion

        #region -- Public Methods --

        public LuryObject Execute()
        {
            //this.mainRoutine.re



            return this.CallRoutine(this.globalContext, this.mainRoutine);
        }

        #endregion

        #region -- Private Methods --

        private void InitializeGlobalContext()
        {
            // 一時的なメソッド

            var intrinsic = new LuryObject(IntrinsicIntrinsic.TypeName, null, freeze: true);
            this.globalContext["Intrinsic"] = intrinsic;
            this.globalContext[intrinsic.LuryTypeName] = intrinsic;

            Action<string, string, IEnumerable<string>> setFunctionMember = (t, n, f) =>
            {
                var type = new LuryObject(t, null);

                foreach (var item in f)
                    type.SetMember(item, new LuryObject(IntrinsicFunction.TypeName, t + "." + item, annotations: new[] { intrinsic }, freeze: true));
                type.Freeze();
                this.globalContext[n] = type;
                this.globalContext[type.LuryTypeName] = type;
            };

            setFunctionMember(IntrinsicFunction.TypeName, "Function", new[] { "opCall" });

            var intrinsicTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t =>
                    t.IsClass &&
                    t.Namespace == "Lury.Engine.Intrinsic" &&
                    Attribute.GetCustomAttribute(t, typeof(IntrinsicClassAttribute)) != null);

            foreach (var type in intrinsicTypes)
            {
                var attr = type.GetCustomAttribute<IntrinsicClassAttribute>();
                var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
                var funcnames = new List<string>();

                for (int j = 0; j < methods.Length; j++)
                {
                    var mattr = methods[j].GetCustomAttributes<IntrinsicAttribute>().Select(a => a.TargetFunction).ToArray();

                    foreach (var funcName in mattr)
                    {
                        funcnames.Add(funcName);
                        this.intrinsicMap.Add(attr.FullName + "." + funcName, methods[j]);
                    }
                }

                if (funcnames.Count > 0)
                    setFunctionMember(attr.FullName, attr.TypeName, funcnames);
            }
        }

        private LuryObject Load(LuryObject[] register, Parameter param, ProgramContext context)
        {
            switch (param.Type)
            {
                case ParameterType.Nil:
                    return LuryObject.Nil;

                case ParameterType.Boolean:
                    return (bool)param.Value ? IntrinsicBoolean.True : IntrinsicBoolean.False;

                case ParameterType.Register:
                    return register[(int)param.Value];

                case ParameterType.Reference:
                    return context[((Reference)param.Value).Name];

                case ParameterType.String:
                    return IntrinsicString.GetObject(param.Value);

                case ParameterType.Integer:
                    return IntrinsicInteger.GetObject((BigInteger)param.Value);

                case ParameterType.Real:
                    return IntrinsicReal.GetObject((double)param.Value);

                case ParameterType.Complex:
                    return IntrinsicComplex.GetObject((Complex)param.Value);

                default:
                    throw new ArgumentException();
            }
        }

        private LuryObject CallRoutine(ProgramContext parentContext, Routine routine)
        {
            LuryObject[] register = new LuryObject[routine.RegisterCount];
            ProgramContext currentContext = new ProgramContext(parentContext);

            for (int i = 0; i < routine.RegisterCount; i++)
                register[i] = LuryObject.Nil;

            int instCount = routine.Instructions.Count;

            for (int i = 0; i < instCount; i++)
            {
                Instruction instruction = routine.Instructions[i];
                LuryObject temporaryObject = null;

                LuryObject x; //, y, op, f;

                switch (instruction.Operation)
                {
                    case Operation.Nop:
                        break;

                    case Operation.Load:
                        if (instruction.Parameters[0].Type == ParameterType.Label)
                            throw new InvalidOperationException();

                        if (instruction.Destination < 0)
                            throw new InvalidOperationException();

                        temporaryObject = this.Load(register, instruction.Parameters[0], currentContext);
                        break;

                    case Operation.Store:
                        if (instruction.Parameters[0].Type != ParameterType.Register)
                            throw new InvalidOperationException();

                        if (instruction.Parameters[1].Type != ParameterType.Reference)
                            throw new InvalidOperationException();

                        x = this.Load(register, instruction.Parameters[0], currentContext);
                        Store(register, x, (Reference)instruction.Parameters[1].Value, currentContext);
                        break;

                    case Operation.Scope:
                        currentContext = new ProgramContext(currentContext);
                        break;

                    case Operation.Break:
                        if (currentContext.Parent == null)
                            throw new InvalidOperationException();

                        currentContext = currentContext.Parent;
                        break;

                    case Operation.Inc:
                        if (instruction.Parameters[0].Type != ParameterType.Register)
                            throw new InvalidOperationException();

                        temporaryObject = this.CallUnaryOperator(OperatorInc, register, instruction.Parameters[0], currentContext);
                        break;

                    case Operation.Dec:
                        if (instruction.Parameters[0].Type != ParameterType.Register)
                            throw new InvalidOperationException();

                        temporaryObject = this.CallUnaryOperator(OperatorDec, register, instruction.Parameters[0], currentContext);
                        break;

                    case Operation.Pos:
                        temporaryObject = this.CallUnaryOperator(OperatorPos, register, instruction.Parameters[0], currentContext);
                        break;

                    case Operation.Neg:
                        temporaryObject = this.CallUnaryOperator(OperatorNeg, register, instruction.Parameters[0], currentContext);
                        break;

                    case Operation.Inv:
                        temporaryObject = this.CallUnaryOperator(OperatorInv, register, instruction.Parameters[0], currentContext);
                        break;

                    case Operation.Pow:
                        temporaryObject = this.CallBinaryOperator(OperatorPow, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Mul:
                        temporaryObject = this.CallBinaryOperator(OperatorMul, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Div:
                        temporaryObject = this.CallBinaryOperator(OperatorDiv, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Idiv:
                        temporaryObject = this.CallBinaryOperator(OperatorIDiv, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Mod:
                        temporaryObject = this.CallBinaryOperator(OperatorMod, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Add:
                        temporaryObject = this.CallBinaryOperator(OperatorAdd, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Sub:
                        temporaryObject = this.CallBinaryOperator(OperatorSub, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Con:
                        temporaryObject = this.CallBinaryOperator(OperatorCon, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Shl:
                        temporaryObject = this.CallBinaryOperator(OperatorLShift, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Shr:
                        temporaryObject = this.CallBinaryOperator(OperatorRShift, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.And:
                        temporaryObject = this.CallBinaryOperator(OperatorAnd, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Xor:
                        temporaryObject = this.CallBinaryOperator(OperatorXor, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Or:
                        temporaryObject = this.CallBinaryOperator(OperatorOr, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Lt:
                        temporaryObject = this.CallBinaryOperator(OperatorLt, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Gt:
                        temporaryObject = this.CallBinaryOperator(OperatorGt, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Ltq:
                        temporaryObject = this.CallBinaryOperator(OperatorLtq, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Gtq:
                        temporaryObject = this.CallBinaryOperator(OperatorGtq, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Eq:
                        temporaryObject = this.CallBinaryOperator(OperatorEq, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Neq:
                        temporaryObject = this.CallBinaryOperator(OperatorNe, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Is:
                        throw new NotImplementedException();

                    case Operation.Isn:
                        throw new NotImplementedException();

                    case Operation.In:
                        temporaryObject = this.CallBinaryOperator(OperatorIn, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.NotIn:
                        temporaryObject = this.CallBinaryOperator(OperatorNotIn, register, instruction.Parameters[0], instruction.Parameters[1], currentContext);
                        break;

                    case Operation.Not:
                        temporaryObject = this.CallUnaryOperator(OperatorNot, register, instruction.Parameters[0], currentContext);
                        break;

                    case Operation.Ret:
                        if (instruction.Parameters.Count == 0)
                            return LuryObject.Nil;
                        else
                            return this.Load(register, instruction.Parameters[0], currentContext);

                    case Operation.Yield:
                        throw new NotImplementedException();

                    case Operation.Throw:
                        throw new NotImplementedException();

                    case Operation.Call:
                        throw new NotImplementedException();

                    case Operation.Eval:
                        throw new NotImplementedException();

                    case Operation.Jmp:
                        i = routine.JumpLabels[(string)instruction.Parameters[0].Value] - 1;
                        break;

                    case Operation.Jmpt:
                        if (this.Load(register, instruction.Parameters[0], currentContext) == IntrinsicBoolean.True)
                            i = routine.JumpLabels[(string)instruction.Parameters[1].Value] - 1;
                        break;

                    case Operation.Jmpf:
                        if (this.Load(register, instruction.Parameters[0], currentContext) == IntrinsicBoolean.False)
                            i = routine.JumpLabels[(string)instruction.Parameters[1].Value] - 1;
                        break;

                    case Operation.Jmpn:
                        if (this.Load(register, instruction.Parameters[0], currentContext) == LuryObject.Nil)
                            i = routine.JumpLabels[(string)instruction.Parameters[1].Value] - 1;
                        break;

                    case Operation.Catch:
                        throw new NotImplementedException();

                    case Operation.Ovlok:
                        throw new NotImplementedException();

                    case Operation.Func:
                        throw new NotImplementedException();

                    case Operation.Class:
                        throw new NotImplementedException();
                        
                    case Operation.Annot:
                        throw new NotImplementedException();

                    default:
                        throw new ArgumentException();
                }

                if (instruction.Destination >= 0)
                {
                    if (temporaryObject == null)
                        throw new ArgumentException();

                    register[instruction.Destination] = temporaryObject;
                }
            }

            return LuryObject.Nil;
        }
        
        private static void Store(LuryObject[] register, LuryObject obj, Reference toRef, ProgramContext context)
        {
            context[toRef.Name] = obj;
        }

        private LuryObject CallUnaryOperator(string funcName, LuryObject[] register, Parameter param, ProgramContext context)
        {
            var x = this.Load(register, param, context);

            var op = x.GetMember(funcName, context);
            var f = op.GetMember("opCall", context);

            if (f.LuryTypeName == IntrinsicFunction.TypeName)
                return this.Call(op, x);
            else
                throw new InvalidOperationException();
        }

        private LuryObject CallBinaryOperator(string funcName, LuryObject[] register, Parameter paramX, Parameter paramY, ProgramContext context)
        {
            var x = this.Load(register, paramX, context);
            var y = this.Load(register, paramY, context);

            var op = x.GetMember(funcName, context);
            var f = op.GetMember("opCall", context);

            if (f.LuryTypeName == IntrinsicFunction.TypeName)
                return this.Call(op, x, y);
            else
                throw new InvalidOperationException();
        }

        #region Function Intrinsic

        private LuryObject Call(LuryObject self, params LuryObject[] parameters)
        {
            if (self.Annotations.Any(o => o.LuryTypeName == IntrinsicIntrinsic.TypeName))
            {
                var intrinsicMethod = intrinsicMap[(string)self.Value];
                return (LuryObject)intrinsicMethod.Invoke(null, parameters);
            }
            else
                throw new NotImplementedException();
        }

        private LuryObject CallKeyword(LuryObject self, LuryObject kwParam)
        {
            throw new NotImplementedException();
        }

        private LuryObject Eval(LuryObject self)
        {
            if (self.Annotations.Any(o => o.LuryTypeName == IntrinsicIntrinsic.TypeName))
            {
                var intrinsicMethod = intrinsicMap[(string)self.Value];
                return (LuryObject)intrinsicMethod.Invoke(null, null);
            }
            else
                throw new NotImplementedException();
        }

        #endregion
        #endregion
    }
}
