using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace WAWSDeploy
{
    public class PublishSettings
    {
        private const string ProfileRootNode = "publishData";
        private const string ProfileNode = "publishProfile";
        private const string PublishMethod = "publishMethod";
        private const string MSDeployHandler = "msdeploy.axd";
        private const string DefaultPort = ":8172";

        private string _publishUrlRaw = string.Empty;
        private string _computerName = string.Empty;
        private string _siteName = string.Empty;
        private string _userName = string.Empty;
        private string _password = string.Empty;
        private string _destinationAppUrl = string.Empty;
        private string _sqlDbConnectionString = string.Empty;
        private string _mysqlDbConnectionString = string.Empty;
        private bool _allowUntrusted;
        private bool? _useNTLM = null;
        private IDictionary<string, PublishSettingsDatabase> _databases;
        private PublishSettingsRemoteAgent _agentType = PublishSettingsRemoteAgent.None;
        private NameValueCollection _otherAttributes;
        
        public PublishSettings(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            Load(doc.CreateNavigator());
        }

        /// <summary>
        /// Used for unit testing
        /// </summary>
        internal PublishSettings()
        {
        }

        internal void Load(XPathNavigator nav)
        {
            Debug.Assert(nav != null, "nav should not be null");
            bool foundPublishSettings = false;
            string name = string.Empty;
            string value = string.Empty;

            nav.MoveToFirstChild();

            if (nav.Name != ProfileRootNode)
            {
                throw new XmlException(string.Format("Expected root node to be '{0}'", ProfileRootNode)); 
            }

            bool fContinue = nav.MoveToFirstChild();

            if (nav.Name != ProfileNode)
            {
                throw new XmlException(string.Format("Expected first child node to be '{0}'", ProfileNode));
            }

            while (fContinue)
            {
                string publishMethod = nav.GetAttribute(PublishMethod, string.Empty);

                if (string.Equals(publishMethod, "MSDeploy", StringComparison.OrdinalIgnoreCase))
                {
                    bool hasMoreAttributes = nav.MoveToFirstAttribute();

                    while (hasMoreAttributes)
                    {
                        if (string.Equals(nav.Name, "publishUrl", StringComparison.OrdinalIgnoreCase))
                        {
                            _publishUrlRaw = nav.Value;
                        }
                        else if (string.Equals(nav.Name, "msdeploySite", StringComparison.OrdinalIgnoreCase))
                        {
                            _siteName = nav.Value;
                        }
                        else if (string.Equals(nav.Name, "userName", StringComparison.OrdinalIgnoreCase))
                        {
                            _userName = nav.Value;
                        }
                        else if (string.Equals(nav.Name, "userPWD", StringComparison.OrdinalIgnoreCase))
                        {
                            _password = nav.Value;
                        }
                        else if (string.Equals(nav.Name, "destinationAppUrl", StringComparison.OrdinalIgnoreCase))
                        {
                            _destinationAppUrl = nav.Value;
                        }
                        else if (string.Equals(nav.Name, "agentType", StringComparison.OrdinalIgnoreCase))
                        {
                            string agentType = nav.Value;
                            try
                            {
                                _agentType = (PublishSettingsRemoteAgent)Enum.Parse(typeof(PublishSettingsRemoteAgent), agentType, true);
                            }
                            catch (ArgumentException ex)
                            {
                                throw new XmlException(
                                    string.Format("Invalid agent type.  Valid options are '{0}'",
                                                  string.Join(", ", Enum.GetNames(typeof(PublishSettingsRemoteAgent)))),
                                    ex);
                            }
                        }
                        else if (string.Equals(nav.Name, "SQLServerDBConnectionString", StringComparison.OrdinalIgnoreCase))
                        {
                            _sqlDbConnectionString = nav.Value;
                        }
                        else if (string.Equals(nav.Name, "mySQLDBConnectionString", StringComparison.OrdinalIgnoreCase))
                        {
                            _mysqlDbConnectionString = nav.Value;
                        }
                        else if (string.Equals(nav.Name, "msdeployAllowUntrustedCertificate", StringComparison.OrdinalIgnoreCase))
                        {
                            string allowUntrustedValue = nav.Value;
                            _allowUntrusted = string.Equals(allowUntrustedValue, "true", StringComparison.OrdinalIgnoreCase) ? true : false;
                        }
                        else if (string.Equals(nav.Name, "useNTLM", StringComparison.OrdinalIgnoreCase))
                        {
                            string useNTLM = nav.Value;
                            if (!string.IsNullOrEmpty(useNTLM))
                            {
                                _useNTLM = Convert.ToBoolean(useNTLM);
                            }

                            // User didn't specify a value so we'll automatically figure it out a little later based on agent type.
                        }
                        else if (!string.Equals(nav.Name, PublishMethod, StringComparison.OrdinalIgnoreCase))
                        {
                            OtherAttributes.Add(nav.Name, nav.Value);
                        }

                        hasMoreAttributes = nav.MoveToNextAttribute();
                    }

                    // Move to the publishProfile node
                    nav.MoveToParent();
                    if (nav.MoveToFirstChild())
                    {
                        do
                        {
                            if (string.Equals(nav.Name, "databases", StringComparison.OrdinalIgnoreCase))
                            {
                                AddDatabases(Databases, nav);
                            }
                        } while (nav.MoveToNext());

                        // Move to publishProfile node
                        nav.MoveToParent();
                    }

                    foundPublishSettings = true;
                    break;
                }

                //move to next publishprofile node
                fContinue = nav.MoveToNext();
            }

            if (!foundPublishSettings)
            {
                throw new XmlException("Could not find MSDeploy publish settings");
            }
        }

        internal static void AddDatabases(
            IDictionary<string, PublishSettingsDatabase> databases,
            XPathNavigator nav)
        {
            if (nav.MoveToFirstChild())
            {
                do
                {
                    if (string.Equals(nav.Name, "add", StringComparison.OrdinalIgnoreCase))
                    {
                        PublishSettingsDatabase database = new PublishSettingsDatabase()
                        {
                            Name = nav.GetAttribute("name", string.Empty),
                            ConnectionString = nav.GetAttribute("connectionString", string.Empty),
                            ProviderName = nav.GetAttribute("providerName", string.Empty),
                            Type = nav.GetAttribute("type", string.Empty),
                            TargetDatabase = nav.GetAttribute("targetDatabaseEngineType", string.Empty),
                            TargetServerVersion = nav.GetAttribute("targetServerVersion", string.Empty)
                        };

                        if (string.IsNullOrEmpty(database.Name))
                        {
                            throw new XmlException("Database 'add' element must contain a 'Name' attribute");
                        }

                        databases.Add(database.Name, database);
                    }
                }
                while (nav.MoveToNext());

                // Move back to the databases node
                nav.MoveToParent();
            }
        }

        public string ComputerName
        {
            get
            {
                if (string.IsNullOrEmpty(_computerName) && !string.IsNullOrEmpty(_publishUrlRaw))
                {
                    if (AgentType == PublishSettingsRemoteAgent.WMSvc ||
                       AgentType == PublishSettingsRemoteAgent.None)
                    {
                        _computerName = GetWmsvcUrl(_publishUrlRaw, _siteName);
                    }
                    else
                    {
                        _computerName = _publishUrlRaw;
                    }
                }

                return _computerName;
            }

            internal set
            {
                _computerName = value;
            }
        }

        public string PublishUrlRaw
        {
            get
            {
                return _publishUrlRaw;
            }

            internal set
            {
                _publishUrlRaw = value;
            }
        }

        public bool AllowUntrusted
        {
            get
            {
                return _allowUntrusted;
            }

            internal set
            {
                _allowUntrusted = value;
            }
        }

        public string SiteName
        {
            get
            {
                return _siteName;
            }

            internal set
            {
                _siteName = value;
            }
        }

        public string DestinationAppUrl
        {
            get
            {
                return _destinationAppUrl;
            }

            internal set
            {
                _destinationAppUrl = value;
            }
        }

        public string Username
        {
            get
            {
                return _userName;
            }

            internal set
            {
                _userName = value;
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }

            internal set
            {
                _password = value;
            }
       }

        public string MySqlDBConnectionString
        {
            get
            {
                return _mysqlDbConnectionString;
            }

            internal set
            {
                _mysqlDbConnectionString = value;
            }
        }

        public string SqlDBConnectionString
        {
            get
            {
                return _sqlDbConnectionString;
            }

            internal set
            {
                _sqlDbConnectionString = value;
            }
        }

        public IDictionary<string, PublishSettingsDatabase> Databases
        {
            get
            {
                if (_databases == null)
                {
                    _databases = 
                        new Dictionary<string, PublishSettingsDatabase>(StringComparer.OrdinalIgnoreCase);
                }

                return _databases;
            }

            internal set
            {
                _databases = value;
            }
        }

        public PublishSettingsRemoteAgent AgentType
        {
            get
            {
                return _agentType;
            }

            internal set
            {
                _agentType = value;
            }
        }

        public bool UseNTLM
        {
            get
            {

                if (!_useNTLM.HasValue)
                {
                    if (_agentType == PublishSettingsRemoteAgent.WMSvc ||
                        _agentType == PublishSettingsRemoteAgent.None)
                    {
                        _useNTLM = false;
                    }
                    else
                    {
                        _useNTLM = true;
                    }
                }

                return _useNTLM.Value;
            }

            internal set
            {
                _useNTLM = value;
            }
        }

        public NameValueCollection OtherAttributes
        {
            get
            {
                if (_otherAttributes == null)
                {
                    _otherAttributes = new NameValueCollection();
                }

                return _otherAttributes;
            }

            internal set
            {
                _otherAttributes = value;
            }
        }

        internal static string GetWmsvcUrl(string publishUrl, string siteName)
        {
            string computerName = publishUrl;

            if (!computerName.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                // Some examples of what we might expect here:
                // foo.com:443/MSDeploy/msdeploy.axd
                // foo.com/MSDeploy/msdeploy.axd
                // foo.com:443
                // foo.com

                computerName = InsertPortIfNotSpecified(computerName);
                computerName = AppendHandlerIfNotSpecified(computerName);

                if (!string.IsNullOrEmpty(siteName))
                {
                    computerName = string.Format("https://{0}?site={1}", computerName, siteName);
                }
                else
                {
                    computerName = string.Format("https://{0}", computerName);
                }
            }

            return computerName;
        }

        internal static string AppendHandlerIfNotSpecified(string publishUrl)
        {
            if (!publishUrl.EndsWith(MSDeployHandler, StringComparison.OrdinalIgnoreCase))
            {
                if (publishUrl.EndsWith("/"))
                {
                    publishUrl = publishUrl + MSDeployHandler;
                }
                else
                {
                    publishUrl = publishUrl + "/" + MSDeployHandler;
                }
            }

            return publishUrl;
        }

        internal static string InsertPortIfNotSpecified(string publishUrl)
        {
            string[] colonParts = publishUrl.Split(new char[] { ':' });

            if (colonParts.Length == 1)
            {
                // No port was specified so we need to add it in
                int slashIndex = publishUrl.IndexOf('/');
                if (slashIndex > -1)
                {
                    //publishUrl = InsertPortBeforeSlash(publishUrl, slashIndex);
                    publishUrl = publishUrl.Insert(slashIndex, DefaultPort);
                }
                else
                {
                    publishUrl = publishUrl + DefaultPort;
                }
            }

            if (colonParts.Length > 1)
            {
                // It's possible that a port was specified, but we're not sure.  Apps like Monaco do weird
                // things like put colon characters in the path and who knows what might happen in the future.
                // We're being extra careful here to make sure that we only look for ports after the hostname.
                // This means right after a colon, but never following ANY '/' characters.

                // Examples of colonParts[0] might be
                // test.com
                // foo.com/bar
                int slashIndex = colonParts[0].IndexOf('/');
                if (slashIndex > -1)
                {
                    // Since a slash was found before the first colon, we know that the first colon was
                    // not used for the port.  Therefore we need to inject the default port before the first slash
                    colonParts[0] = colonParts[0].Insert(slashIndex, DefaultPort);
                    publishUrl = string.Join(":", colonParts);
                }
            }

            return publishUrl;
        }
    }

    public enum PublishSettingsRemoteAgent : int
    {
        WMSvc = 0,
        MSDepSvc,
        TempAgent,
        None,
    }

    public class PublishSettingsDatabase
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string ProviderName { get; set; }
        public string Type { get; set; }
        public string TargetDatabase { get; set; }
        public string TargetServerVersion { get; set; }
    }

}
