using Signal.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Windows.Storage;

namespace Signal.Resources
{
    public class CountryCollection : Collection<Country>
    {
        public CountryCollection()
        {
            Load();
        }

        private void Load()
        {
            var uri = new Uri("ms-appx:///Resources/CountryCodes.xml");

            var doc = XDocument.Load("Resources/CountryCodes.xml");
            var countries = from item in doc.Descendants("country")
                            select new Country(item.Attribute("name").Value,item.Attribute("phoneCode").Value);

            foreach(var c in countries)
            {
                Debug.WriteLine($"{c.Name} : {c.Code}");
                Add(c);
            }
        }
    }
}
