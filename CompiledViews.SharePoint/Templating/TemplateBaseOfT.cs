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
        /// <summary>
        /// Initialises a new instance of <see cref="TemplateBase{T}"/>
        /// </summary>
        protected TemplateBase() : base()
        {
            var type = GetType();
        }


        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public virtual T Model {get;set;}

      
    }
}