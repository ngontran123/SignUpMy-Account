using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Drawing;

namespace GSMMASTER
{
    public class AppSettingElement : ConfigurationElement
    {
        [ConfigurationProperty("com", IsRequired = true)]
        public string Com
        {
            get
            {
                return (string)base["com"];
            }
            set
            {
                base["com"] = value;
            }
        }
    }
    public class PortSettingElement:ConfigurationElement
    {
        [ConfigurationProperty("port_custom",IsKey =true,IsRequired = true)]
        public string Port_Custom
        {
            get
            {
                return (string)this["port_custom"];
            }
            set
            {
                this["port_custom"] = value;
            }
        }
    }

    public class AppSettingCollection : ConfigurationElementCollection
    {
        // Create a property that lets us access an element in the
        // collection with the int index of the element
        public AppSettingElement this[int index]
        {
            get
            {
                // Gets the SageCRMInstanceElement at the specified
                // index in the collection
                return (AppSettingElement)BaseGet(index);
            }
            set
            {
                // Check if a SageCRMInstanceElement exists at the
                // specified index and delete it if it does
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                // Add the new SageCRMInstanceElement at the specified
                // index
                BaseAdd(index, value);
            }
        }

        // Create a property that lets us access an element in the
        // colleciton with the name of the element
        public new AppSettingElement this[string key]
        {
            get
            {
                // Gets the SageCRMInstanceElement where the name
                // matches the string key specified
                return (AppSettingElement)BaseGet(key);
            }
            set
            {
                // Checks if a SageCRMInstanceElement exists with
                // the specified name and deletes it if it does
                if (BaseGet(key) != null)
                    BaseRemoveAt(BaseIndexOf(BaseGet(key)));

                // Adds the new SageCRMInstanceElement
                BaseAdd(value);
            }
        }

        // Method that must be overriden to create a new element
        // that can be stored in the collection
        protected override ConfigurationElement CreateNewElement()
        {
            return new AppSettingElement();
        }

        // Method that must be overriden to get the key of a
        // specified element
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AppSettingElement)element).Com;
        }
    }
    public class PortSettingCollection: ConfigurationElementCollection
    {
        protected override void BaseAdd(ConfigurationElement element)
        {
            // Remove the validation for duplicate values
            BaseAdd(element, false);
        }
        public PortSettingElement this[int index]
        {
            get
            {
                // Gets the SageCRMInstanceElement at the specified
                // index in the collection
                return (PortSettingElement)BaseGet(index);
            }
            set
            {
                // Check if a SageCRMInstanceElement exists at the
                // specified index and delete it if it does
               

                // Add the new SageCRMInstanceElement at the specified
                // index
                BaseAdd(index, value);
            }
        }
       
        // Create a property that lets us access an element in the
        // colleciton with the name of the element
        public new PortSettingElement this[string key]
        {
            get
            {
                // Gets the SageCRMInstanceElement where the name
                // matches the string key specified
                return (PortSettingElement)BaseGet(key);
            }
            set
            {
                // Checks if a SageCRMInstanceElement exists with
                // the specified name and deletes it if it does
                // Adds the new SageCRMInstanceElement
                BaseAdd(value);
            }
        }
        protected override bool IsElementName(string elementName)
        {
            return true;
        }
        public override bool IsReadOnly()
        {
            return false;
        }
        // Method that must be overriden to create a new element
        // that can be stored in the collection
        protected override ConfigurationElement CreateNewElement()
        {
            return new PortSettingElement();
        }

        // Method that must be overriden to get the key of a
        // specified element
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PortSettingElement)element).Port_Custom;
        }
    }
   
    public class AppSettingSection : ConfigurationSection
    {
        [ConfigurationProperty("setting")]
        [ConfigurationCollection(typeof(AppSettingCollection))]
        public AppSettingCollection collect
        {
            get
            {
                return (AppSettingCollection)this["setting"];
            }
            set
            {
                this["setting"] = value;
            }
        }
    }
    public class PortSettingSection:ConfigurationSection
    {
        [ConfigurationProperty("port",IsDefaultCollection =true)]
        [ConfigurationCollection(typeof(PortSettingCollection))]
        public PortSettingCollection collect
        {
            get
            {
                return (PortSettingCollection)this["port"];
            }
            set
            {
                this["port"] = value;
            }
        }
    }
  
    public class InforSettingSection:ConfigurationSection
    {
        [ConfigurationProperty("recheck_port",IsRequired =true)]
        public string Recheck_Port
        {
            get {return (string)this["recheck_port"];}
            set
            {
                this["recheck_port"] = value;
            }
        }
        [ConfigurationProperty("baud_rate",IsRequired =true)]
        public string Baudrate
        {
            get { return (string)this["baud_rate"]; }
            set
            {
                this["baud_rate"] = value;
            }
        }
        [ConfigurationProperty("is_remember",IsRequired =true)]
        public bool Is_Remember
        {
            get { return (bool)this["is_remember"]; }
            set
            {
                this["is_remember"] = value;
            }
        }
        [ConfigurationProperty("username",IsRequired =true)]
        public string UserName
        {
            get{ return (string)this["username"]; }
            set
            {
                this["username"] = value;
            }
        }
        [ConfigurationProperty("password",IsRequired =true)]
        public string Password
        {
            get { return (string)this["password"]; }
            set
            {
                this["password"] = value;
            }
        }
    }
    public class MyPasswordSettingSection:ConfigurationSection
    {
        [ConfigurationProperty("mypassword",IsRequired =true)]
        public string MyPassword
        {
            get { return (string)this["mypassword"]; }
            set
            {
                this["mypassword"] = value;
            }
        }
    }
}