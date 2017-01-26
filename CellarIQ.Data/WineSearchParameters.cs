using System;
using System.Collections.Generic;

namespace CellarIQ.Data
{

    [Serializable]
    public class WineSearchParameters
    {
        
        public WineSearchParameters()
        {
            
        }
        public WineSearchParameters(Dictionary<string, string> parameterDictionary)
        {
            SetParameters(parameterDictionary);
        }

        public string WineLabel { get; set; }

        public string Vintner { get; set; }

        public string Color { get; set; }
        public string Vintage { get; set; }
        public string WineDescription { get; set; }

        public string Varietal { get; set; }

        public string StorageUnit { get; set; }

        public string Shelf { get; set; }

        public void SetParameter(string key, string value)
        {
            // ReSharper disable once StringIndexOfIsCultureSpecific.1
            int separator = key.IndexOf("::");
            if (separator > 0)
            {

                key = key.Substring(separator + 2);
            }

            typeof(WineSearchParameters).GetProperty(key)?.SetValue(this, value);
        }

        public void SetParameters(Dictionary<string, string> parameterDictionary)
        {
            foreach (KeyValuePair<string, string> kvp in parameterDictionary)
            {
                string val = kvp.Value.ToUpper();
                if (!(val.Equals("ANY") || val.Equals("ALL")))
                {
                    SetParameter(kvp.Key, kvp.Value);
                }

            }
        }
    }
}