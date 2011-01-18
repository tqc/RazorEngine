namespace RazorEngine.Configuration
{
    using System.Configuration;

    /// <summary>
    /// Defines the <see cref="ConfigurationElement"/> that represents a template service element.
    /// </summary>
    public class TemplateServiceConfigurationElement : ConfigurationElement
    {
        #region Fields
        private const string LanguageProviderElement = "languageProvider";
        private const string MarkupParserElement = "markupParser";
        private const string NameElement = "name";
        private const string NamespacesElement = "namespaces";
        private const string TemplateBaseElement = "templateBase";
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the language provider.
        /// </summary>
        [ConfigurationProperty(LanguageProviderElement)]
        public string LanguageProvider
        {
            get { return (string)this[LanguageProviderElement]; }
            set { this[LanguageProviderElement] = value; }
        }

        /// <summary>
        /// Gets or sets the markup parser.
        /// </summary>
        [ConfigurationProperty(MarkupParserElement)]
        public string MarkupParser
        {
            get { return (string)this[MarkupParserElement]; }
            set { this[MarkupParserElement] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the template service.
        /// </summary>
        [ConfigurationProperty(NameElement, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this[NameElement]; }
            set { this[NameElement] = value; }
        }

        /// <summary>
        /// Gets or sets the collection of namespaces.
        /// </summary>
        [ConfigurationProperty(NamespacesElement)]
        public NamespaceConfigurationElementConfiguration Namespaces
        {
            get { return (NamespaceConfigurationElementConfiguration)this[NamespacesElement]; }
            set { this[NamespacesElement] = value; }
        }

        /// <summary>
        /// Gets or sets the template base
        /// </summary>
        [ConfigurationProperty(TemplateBaseElement)]
        public string TemplateBase
        {
            get { return (string)this[TemplateBaseElement]; }
            set { this[TemplateBaseElement] = value; }
        }
        #endregion
    }
}