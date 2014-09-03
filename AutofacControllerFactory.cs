using Autofac;
using System;
using System.Web.Mvc;

namespace Name
{
    public class AutofacControllerFactory : DefaultControllerFactory
    {
        private IContainer _container;
        public AutofacControllerFactory(IContainer container)
        {
            _container = container;
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return _container.ResolveKeyed<IController>(controllerType.Name);
        }

    }
}