namespace RazorEngine.Compilation
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a requirement for a namespace imports.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class RequireNamespacesAttribute : Attribute
    {
        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="RequireNamespacesAttribute"/>.
        /// </summary>
        /// <param name="namespaces">The required namespaces</param>
        public RequireNamespacesAttribute(params string[] namespaces)
        {
            Namespaces = namespaces ?? new string[0];
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the set of required namespace imports.
        /// </summary>
        public IEnumerable<string> Namespaces { get; private set; }
        #endregion
    }
}