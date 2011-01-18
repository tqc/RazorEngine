namespace RazorEngine.Templating
{
    using System;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;

    using Compilation;

    /// <summary>
    /// Provides a base implementation of a template with a model.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    public abstract class TemplateBase<T> : TemplateBase, ITemplate<T>
    {
        #region Fields
        private object model;
        #endregion

        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="TemplateBase{T}"/>
        /// </summary>
        protected TemplateBase()
        {
            var type = GetType();
            HasDynamicModel = type.GetCustomAttributes(typeof(HasDynamicModelAttribute), true).Any();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets whether this template has a dynamic model.
        /// </summary>
        protected bool HasDynamicModel { get; private set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public virtual T Model
        {
            get { return (T)model; }
            set
            {
                if (HasDynamicModel)
                    model = new RazorDynamicObject { Model = value };
                else
                    model = value;
            }
        }
        #endregion

        #region Types
        /// <summary>
        /// Defines a dynamic object.
        /// </summary>
        class RazorDynamicObject : DynamicObject
        {
            #region Properties
            /// <summary>
            /// Gets or sets the model.
            /// </summary>
            public object Model { get; set; }
            #endregion

            #region Methods
            /// <summary>
            /// Gets the value of the specified member.
            /// </summary>
            /// <param name="binder">The current binder.</param>
            /// <param name="result">The member result.</param>
            /// <returns>True.</returns>
            [DebuggerStepperBoundary]
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                Type modelType = Model.GetType();
                var prop = modelType.GetProperty(binder.Name);
                if (prop == null)
                {
                    result = null;
                    return false;
                }

                object value = prop.GetValue(Model, null);
                if (value == null)
                {
                    result = value;
                    return true;
                }

                Type valueType = value.GetType();
                result = (RazorCompiler.IsAnonymousType(valueType))
                             ? new RazorDynamicObject { Model = value }
                             : value;
                return true;
            }
            #endregion
        }
        #endregion
    }
}