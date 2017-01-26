using System.Web.Http;
using CellarIQ.Common.TypeMapping;
using CellarIQ.Web.Common;

namespace CellarIQ.Web.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            new AutoMapperConfigurator().Configure(WebContainerManager.GetAll<IAutoMapperTypeConfigurator>());
        }
    }
}
