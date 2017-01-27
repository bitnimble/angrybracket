using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AngryBracket
{
	public static class ReflectionHelper
	{
		/// <summary>
		/// Determines if the current type inherits (directly or indirectly from the given type or interface)
		/// </summary>
		public static bool InheritsFrom(this Type type, Type ancestor)
		{
			if (type.BaseType == ancestor)
				return true;
			if (type.GetInterfaces().Contains(ancestor))
				return true;

			return type.BaseType?.InheritsFrom(ancestor) ?? false;
		}

		static Type Nullable = typeof(System.Nullable<>);

		public static bool IsNullable(this Type type)
		{
			//Classes are nullable.
			if (type.IsClass)
				return true;

			//Nullables are nullable.
			if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == Nullable)
				return true;

			return false;
		}

		public static Type MakeNullable(this Type type)
		{
			//Check to see whether it is already nullable.
			if (type.IsNullable())
				return type;

			return Nullable.MakeGenericType(type);
		}

		public static Type[] GetParameterTypes(this MethodInfo method)
		{
			ParameterInfo[] parameters = method.GetParameters();
			Type[] types = new Type[parameters.Length];

			for (int i = 0; i < parameters.Length; i++)
				types[i] = parameters[i].ParameterType;

			return types;
		}
		
		/// <summary>
		/// Returns an array of Types representing the parameters to a System.Predicate, System.Action, or System.Func.
		/// </summary>
		/// <typeparam name="Delegate">An Action, Func, or Predicate type.</typeparam>
		public static Type[] GetParameterTypesFromDelegate<Delegate>()
		{
			return GetParameterTypesFromDelegate(typeof(Delegate));
		}

		/// <summary>
		/// Returns an array of Types representing the parameters to a System.Predicate, System.Action, or System.Func.
		/// </summary>
		/// <param name="del">An Action, Func, or Predicate instance.</param>
		public static Type[] GetParameterTypesFromDelegateInstance(object del)
		{
			return GetParameterTypesFromDelegate(del.GetType());
		}

		/// <summary>
		/// Returns an array of Types representing the parameters to a System.Predicate, System.Action, or System.Func.
		/// </summary>
		/// <param name="delegateType">An Action, Func, or Predicate type.</param>
		public static Type[] GetParameterTypesFromDelegate(Type delegateType)
		{
			if (delegateType.FullName.StartsWith("System.Action") || delegateType.FullName.StartsWith("System.Predicate"))
			{
				if (!delegateType.IsGenericType)
					return new Type[0];
				var genericTypes = delegateType.GetGenericArguments();
				return genericTypes;
			}
			else if (delegateType.FullName.StartsWith("System.Func"))
			{
				var genericTypes = delegateType.GetGenericArguments();
				return genericTypes.Subarray(0, genericTypes.Length - 1);
			}
			else
			{
				var method = delegateType.GetMethod("Invoke");
				return method.GetParameterTypes();
			}
			throw new ArgumentException("delegateType must be one of Action, Func or Predicate", "delegateType");
		}

		/// <summary>
		/// Returns a Type representing the return type of a System.Predicate, System.Action, or System.Func.
		/// </summary>
		/// <typeparam name="Delegate">An Action, Func, or Predicate type.</typeparam>
		public static Type GetReturnTypeFromDelegate<Delegate>()
		{
			return GetReturnTypeFromDelegate(typeof(Delegate));
		}

		/// <summary>
		/// Returns a Type representing the return type of a System.Predicate, System.Action, or System.Func.
		/// </summary>
		/// <param name="del">An Action, Func, or Predicate instance.</param>
		public static Type GetReturnTypeFromDelegateInstance(object del)
		{
			return GetReturnTypeFromDelegate(del.GetType());
		}

		/// <summary>
		/// Returns a Type representing the return type of a System.Predicate, System.Action, or System.Func.
		/// </summary>
		/// <param name="delegateType">An Action, Func, or Predicate type.</param>
		public static Type GetReturnTypeFromDelegate(Type delegateType)
		{
			if (delegateType.FullName.StartsWith("System.Action"))
				return typeof(void);
			else if (delegateType.FullName.StartsWith("System.Predicate"))
				return typeof(bool);
			else if (delegateType.FullName.StartsWith("System.Func"))
			{
				var genericTypes = delegateType.GetGenericArguments();
				return genericTypes[genericTypes.Length - 1];
			}
			else
			{
				var method = delegateType.GetMethod("Invoke");
				return method.ReturnType;
			}
		}
		
		/// <summary>
		/// Creates an Action&lt;...&gt; or a Func&lt;...&gt; based on the given arguments.
		/// If specified, an attempt is made to cast the return type of the method if its type does not match that exepcted.
		/// </summary>
		/// <typeparam name="FuncTy">Either an Action&lt;...&gt; or a Func&lt;...&gt; type suitable for the given method</typeparam>
		/// <returns>A bound delegate</returns>
		public static FuncTy BindToDelegate<FuncTy>(MethodInfo method, object instance = null, bool coerceTypes = true)
		{
			Type[] parameterTypes = GetParameterTypes(method);
			ParameterExpression[] lambdaParameters = new ParameterExpression[parameterTypes.Length];
			Expression[] methodParameters = new Expression[parameterTypes.Length];

			Type[] lambdaParameterTypes = GetParameterTypesFromDelegate<FuncTy>();
			for (int i = 0; i < parameterTypes.Length; i++)
			{
				lambdaParameters[i] = Expression.Parameter(lambdaParameterTypes[i]);
				methodParameters[i] = coerceTypes ? Expression.Convert(lambdaParameters[i], parameterTypes[i]) : (Expression)lambdaParameters[i];
			}

			var functionCall = instance == null
				? Expression.Call(method, methodParameters)
				: Expression.Call(Expression.Constant(instance, method.DeclaringType), method, methodParameters);

			Expression returnCast = coerceTypes ? Expression.Convert(functionCall, GetReturnTypeFromDelegate<FuncTy>()) : (Expression)functionCall;

			Expression<FuncTy> expr = Expression.Lambda<FuncTy>(returnCast, lambdaParameters);

			return expr.Compile();
		}

		/// <summary>See BindToDelegate&lt;Action&gt;</summary>
		public static Action CreateAction(MethodInfo method, object instance, bool coerceTypes = true) { return BindToDelegate<Action>(method, instance, coerceTypes); }
		/// <summary>See BindToDelegate&lt;Action&lt;T1&gt;&gt;</summary>
		public static Action<T1> CreateAction<T1>(MethodInfo method, object instance, bool coerceTypes = true) { return BindToDelegate<Action<T1>>(method, instance, coerceTypes); }
		/// <summary>See BindToDelegate&lt;Action&lt;T1, T2&gt;&gt;</summary>
		public static Action<T1, T2> CreateAction<T1, T2>(MethodInfo method, object instance, bool coerceTypes = true) { return BindToDelegate<Action<T1, T2>>(method, instance, coerceTypes); }
		/// <summary>See BindToDelegate&lt;Action&lt;T1, T2, T3&gt;&gt;</summary>
		public static Action<T1, T2, T3> CreateAction<T1, T2, T3>(MethodInfo method, object instance, bool coerceTypes = true) { return BindToDelegate<Action<T1, T2, T3>>(method, instance, coerceTypes); }
		/// <summary>See BindToDelegate&lt;Action&lt;T1, T2, T3, T4&gt;&gt;</summary>
		public static Action<T1, T2, T3, T4> CreateAction<T1, T2, T3, T4>(MethodInfo method, object instance, bool coerceTypes = true) { return BindToDelegate<Action<T1, T2, T3, T4>>(method, instance, coerceTypes); }

		/// <summary>See BindToDelegate&lt;Func&lt;R1&gt;&gt;</summary>
		public static Func<R1> CreateFunc<R1>(MethodInfo method, object instance, bool coerceTypes = true) { return BindToDelegate<Func<R1>>(method, instance, coerceTypes); }
		/// <summary>See BindToDelegate&lt;Func&lt;T1, R1&gt;&gt;</summary>
		public static Func<T1, R1> CreateFunc<T1, R1>(MethodInfo method, object instance, bool coerceTypes = true) { return BindToDelegate<Func<T1, R1>>(method, instance, coerceTypes); }
		/// <summary>See BindToDelegate&lt;Func&lt;T1, T2, R1&gt;&gt;</summary>
		public static Func<T1, T2, R1> CreateFunc<T1, T2, R1>(MethodInfo method, object instance, bool coerceTypes = true) { return BindToDelegate<Func<T1, T2, R1>>(method, instance, coerceTypes); }
		/// <summary>See BindToDelegate&lt;Func&lt;T1, T2, T3, R1&gt;&gt;</summary>
		public static Func<T1, T2, T3, R1> CreateFunc<T1, T2, T3, R1>(MethodInfo method, object instance, bool coerceTypes = true) { return BindToDelegate<Func<T1, T2, T3, R1>>(method, instance, coerceTypes); }
		/// <summary>See BindToDelegate&lt;Func&lt;T1, T2, T3, T4, R1&gt;&gt;</summary>
		public static Func<T1, T2, T3, T4, R1> CreateFunc<T1, T2, T3, T4, R1>(MethodInfo method, object instance, bool coerceTypes = true) { return BindToDelegate<Func<T1, T2, T3, T4, R1>>(method, instance, coerceTypes); }
	}
}
