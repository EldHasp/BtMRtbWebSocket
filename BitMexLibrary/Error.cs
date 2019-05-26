using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BitMexLibrary
{

    [DataContract]
    public class RootError
    {
        [DataMember(Name = "error")]
        public Error Error { get; set; }
    }


    [DataContract]
    public class Error
    {
        public static Error LastError;

        [DataMember(Name = "message", IsRequired = false)]
        public string Message { get; set; }
        [DataMember(Name = "name", IsRequired = false)]
        public string Name { get; set; }

        public Error() { }

        public Error(XElement xElement)
        {
            Name = xElement.Element("html").Element("head").Element("title").Value;
            Message = xElement.Element("html").Element("body").Element("center").Element("h1").Value;
        }
    }


    // Примечание. Для запуска созданного кода может потребоваться NET Framework версии 4.5 или более поздней версии и .NET Core или Standard версии 2.0 или более поздней.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class html
    {

        public htmlHead head { get; set; }

        public htmlBody body { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class htmlHead
    {


        public string title { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class htmlBody
    {
        public htmlBodyCenter center { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string bgcolor { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class htmlBodyCenter
    {

        public string h1 { get; set; }
    }


}
