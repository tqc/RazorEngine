namespace RazorEngine.Templating
{
    using System;
     using System.Linq;
    using System.Reflection;



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
            HasDynamicModel = false;
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
                                 model = value;
            }
        }
        #endregion

      
    }
}