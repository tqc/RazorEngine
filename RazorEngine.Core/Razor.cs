namespace RazorEngine
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web.Razor.Parser;

    using Compilation;
    using Configuration;
    using Templating;

    /// <summary>
    /// Provides quick access to template services
    /// </summary>
    public static class Razor
    {
        #region Fields
        private static bool createdService = false;
        private static IEnumerable<string> defaultNamespaces = null;
        private static ILanguageProvider languageProvider = null;
        private static MarkupParser markupParser = null;
        private static TemplateService service;
        private static readonly object sync = new object();
        private static Type templateBaseType = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Initialises the <see cref="Razor"/> type.
        /// </summary>
        static Razor()
        {
            Services = new ConcurrentDictionary<string, TemplateService>();
            Configure();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the collection of configured <see cref="TemplateService"/> instances.
        /// </summary>
        public static IDictionary<string, TemplateService> Services { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Pre-compiles the specified template and caches it using the specified name.
        /// </summary>
        /// <param name="template">The template to precompile.</param>
        /// <param name="name">The cache name for the template.</param>
        public static void Compile(string template, string name)
        {
            Compile(template, null, name);
        }

        /// <summary>
        /// Pre-compiles the specified template and caches it using the specified name.
        /// </summary>
        /// <param name="template">The template to precompile.</param>
        /// <param name="modelType">The type of model used in the template.</param>
        /// <param name="name">The cache name for the template.</param>
        public static void Compile(string template, Type modelType, string name)
        {
            EnsureTemplateService();
            service.Compile(template, modelType, name);
        }

        /// <summary>
        /// Pre-compiles the specified template and caches it using the specified name.
        /// This method should be used when an anonymous model is used in the template.
        /// </summary>
        /// <param name="template">The template to precompile.</param>
        /// <param name="name">The cache name for the template.</param>
        public static void CompileWithAnonymous(string template, string name)
        {
            Compile(template, new { }.GetType(), name);
        }

        /// <summary>
        /// Configures the engine.
        /// </summary>
        private static void Configure()
        {
            var config = RazorEngineConfigurationSection.GetConfiguration();
            if (config == null)
                return;

            defaultNamespaces = config.Namespaces
                .Cast<NamespaceConfigurationElement>()
                .Select(n => n.Namespace);

            config.TemplateServices
                .Cast<TemplateServiceConfigurationElement>()
                .ToList()
                .ForEach(t => Services.Add(t.Name, TemplateServiceFactory.CreateTemplateService(t, defaultNamespaces)));

            if (!string.IsNullOrEmpty(config.TemplateServices.Default))
            {
                TemplateService templateService = null;
                if (!Services.TryGetValue(config.TemplateServices.Default, out templateService))
                    throw new ConfigurationErrorsException(
                        string.Format(
                            "No template service is configured with name '{0}' and could not be set as default.",
                            config.TemplateServices.Default));

                service = templateService;
                createdService = true;
            }
        }

        /// <summary>
        /// Ensures the template service has been created or re-created.
        /// </summary>
        private static void EnsureTemplateService()
        {
            if (!createdService)
            {
                lock (sync)
                {
                    service = new TemplateService(languageProvider, templateBaseType, markupParser);
                    createdService = true;
                }
            }
        }

        /// <summary>
        /// Parses the specified template.
        /// </summary>
        /// <param name="template">The template to parse.</param>
        /// <param name="name">[Optional] The name of the template.</param>
        /// <returns>The parsed template.</returns>
        public static string Parse(string template, string name = null)
        {
            EnsureTemplateService();
            return service.Parse(template, name);
        }

        /// <summary>
        /// Parses the specified template.
        /// </summary>
        /// <param name="template">The template to parse.</param>
        /// <param name="model">The model to merge with the template.</param>
        /// <param name="name">[Optional] The name of the template.</param>
        /// <returns>The parsed template.</returns>
        public static string Parse<T>(string template, T model, string name = null)
        {
            EnsureTemplateService();
            return service.Parse(template, model, name);
        }

        /// <summary>
        /// Convert a cshtml template to cs code
        /// </summary>
        /// <param name="template">The template to parse.</param>
        /// <param name="model">[Optional] Type of the model for the template.</param>
        /// <param name="name">[Optional] The name of the template.</param>
        /// <returns>The parsed template in code form.</returns>
        public static string ParseToCode(string template, Type modelType = null, string name = null)
        {
            EnsureTemplateService();
            return service.ParseToCode(template, modelType, name);
        }


        /// <summary>
        /// Runs the cached template with the specified name.
        /// </summary>
        /// <param name="name">The name of the cached template.</param>
        /// <returns>The parsed template.</returns>
        public static string Run(string name)
        {
            EnsureTemplateService();
            return service.Run(name);
        }

        /// <summary>
        /// Runs the cached template with the specified name.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="name">The name of the cached template.</param>
        /// <returns>The parsed template.</returns>
        public static string Run<T>(T model, string name)
        {
            EnsureTemplateService();
            return service.Run(model, name);
        }

        /// <summary>
        /// Sets the language provider.
        /// </summary>
        /// <param name="provider">The language provider.</param>
        public static void SetLanguageProvider(ILanguageProvider provider)
        {
            if (provider == null)
                throw new ArgumentException("provider");

            languageProvider = provider;
            createdService = false;
        }

        /// <summary>
        /// Sets the markup parser.
        /// </summary>
        /// <param name="parser">The markup parser to use.</param>
        public static void SetMarkupParser(MarkupParser parser)
        {
            markupParser = parser;
            createdService = false;
        }

        /// <summary>
        /// Sets the template base type.
        /// </summary>
        /// <param name="type">The template base type.</param>
        public static void SetTemplateBaseType(Type type)
        {
            templateBaseType = type;
            createdService = false;
        }
        #endregion
    }
}