namespace RazorEngine.Templating
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web.Razor.Parser;

    using Compilation;
    using Configuration;

    /// <summary>
    /// Provides factory methods for creating instances of <see cref="TemplateService"/>.
    /// </summary>
    public static class TemplateServiceFactory
    {
        #region Methods
        /// <summary>
        /// Creates an instance of a <see cref="TemplateService"/>.
        /// </summary>
        /// <param name="provider">[Optional] The language provider to use.</param>
        /// <param name="templateBaseType">[Optional] The base template type.</param>
        /// <param name="parser">[Optional] The markup parser to use.</param>
        /// <returns>A new instance of <see cref="TemplateService"/>.</returns>
        public static TemplateService CreateTemplateService(ILanguageProvider provider = null, Type templateBaseType = null, MarkupParser parser = null)
        {
            return new TemplateService(provider, templateBaseType, parser);
        }

        /// <summary>
        /// Creates an instance of a <see cref="TemplateService"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="TemplateServiceConfigurationElement"/> that represents the configuration.</param>
        /// <param name="defaultNamespaces">The enumerable of namespaces to add as default.</param>
        /// <returns>A new instance of <see cref="TemplateService"/>.</returns>
        public static TemplateService CreateTemplateService(TemplateServiceConfigurationElement configuration, IEnumerable<string> defaultNamespaces = null)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            ILanguageProvider provider = null;
            MarkupParser parser = null;
            Type templateBaseType = null;

            if (!string.IsNullOrEmpty(configuration.LanguageProvider))
                provider = (ILanguageProvider)GetInstance(configuration.LanguageProvider);

            if (!string.IsNullOrEmpty(configuration.MarkupParser))
                parser = (MarkupParser)GetInstance(configuration.MarkupParser);

            if (!string.IsNullOrEmpty(configuration.TemplateBase))
                templateBaseType = GetType(configuration.TemplateBase);

            var namespaces = configuration.Namespaces
                .Cast<NamespaceConfigurationElement>()
                .Select(n => n.Namespace);

            if (defaultNamespaces != null)
            {
                namespaces = defaultNamespaces
                    .Concat(namespaces)
                    .Distinct();
            }

            var service = new TemplateService(provider, templateBaseType, parser);
            foreach (string ns in namespaces)
                service.Namespaces.Add(ns);

            return service;
        }

        /// <summary>
        /// Creates an instance of a <see cref="TemplateService"/>.
        /// </summary>
        /// <param name="configuration">The named configuration.</param>
        /// <returns>A new instance of <see cref="TemplateService"/>.</returns>
        public static TemplateService CreateTemplateService(string configuration)
        {
            if (string.IsNullOrEmpty(configuration))
                throw new ArgumentException("The argument 'configuration' cannot be null or empty.");

            var config = RazorEngineConfigurationSection.GetConfiguration();
            if (config == null)
                throw new ConfigurationErrorsException("The configuration section <razorEngine /> is not defined.");

            var templateServiceConfig = config.TemplateServices
                .Cast<TemplateServiceConfigurationElement>()
                .Where(t => t.Name.Equals(configuration, StringComparison.InvariantCultureIgnoreCase))
                .SingleOrDefault();

            if (templateServiceConfig == null)
                throw new ConfigurationErrorsException(string.Format("No configuration for template service '{0}' is defined", configuration));

            return CreateTemplateService(templateServiceConfig, config.Namespaces.Cast<NamespaceConfigurationElement>().Select(n => n.Namespace));
        }

        /// <summary>
        /// Gets an instance of the specified type.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <returns>An instance of the specified type.</returns>
        internal static object GetInstance(string typeName)
        {
            var type = GetType(typeName);
            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// Gets the <see cref="Type"/>
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        internal static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type == null)
                throw new ConfigurationErrorsException(
                    string.Format("The type '{0}' is invalid and could not be loaded.", typeName));

            return type;
        }
        #endregion
    }
}