using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Castle.DynamicProxy;

namespace NoWoL.TestingUtilities.ObjectCreators
{
    /// <summary>
    /// Provides a way to create instances of an interface using a Mock&lt;&gt; object
    /// </summary>
    public class ProxyInterfaceCreator : IObjectCreator
	{
        private static readonly ProxyGenerator InterfaceInstanceGenerator = new();

        /// <summary>
        /// Determines whether this instance can create the specified object type.
        /// </summary>
        /// <param name="type">Type of the object.</param>
        /// <returns><c>true</c> if this instance can create the specified object type; otherwise, <c>false</c>.</returns>
        public bool CanHandle(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IsInterface;
        }

        /// <summary>
        /// Create an instance of the specified object type.
        /// </summary>
        /// <param name="type">Type of the object.</param>
        /// <param name="objectCreators">A list of <see cref="IObjectCreator"/> to handle creation of sub objects.</param>
        /// <returns>The created object.</returns>
        public object Create(Type type, ICollection<IObjectCreator> objectCreators)
		{
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (CanHandle(type))
            {
                return InterfaceInstanceGenerator.CreateInterfaceProxyWithoutTarget(type,
                                                                                    NoopInterceptor.Instance);
			}

            throw new UnsupportedTypeException("Expecting an interface however received " + type.FullName);
        }

        /// <summary>
        /// Interface interceptor that does nothing
        /// </summary>
        [Serializable]
        [ExcludeFromCodeCoverage]
        private class NoopInterceptor : IInterceptor
        {
            /// <summary>
            /// Thread safe instance of <see cref="NoopInterceptor"/>
            /// </summary>
            public static readonly NoopInterceptor Instance = new();

            /// <summary>
            /// Does nothing
            /// </summary>
            /// <param name="invocation"></param>
            public void Intercept(IInvocation invocation)
            {
            }
        }
    }
}
