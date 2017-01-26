using System;

namespace CellarIQ.Data
{
    [Serializable]
    public class CellarConfiguation
    {
        public string DatabaseId { get; set; }
        public string CellarDatabaseConnectionUri { get; set; }

        public string CellarDatabaseKey { get; set; }
        public string SearchServiceName { get; set; }
        public string SearchServiceKey { get; set; }
    }
}
