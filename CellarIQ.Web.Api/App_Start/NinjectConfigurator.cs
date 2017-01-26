using System;
using System.Web;
using Ninject;
using Ninject.Activation;

namespace CellarIQ.Web.Api
{
    /// <summary>
    ///     Class used to set up the Ninject DI container.
    /// </summary>
    public class NinjectConfigurator
    {
        /// <summary>
        ///     Entry method used by caller to configure the given
        ///     container with all of this application's
        ///     dependencies.
        /// </summary>
        public void Configure(IKernel container)
        {
            AddBindings(container);
        }

        private void AddBindings(IKernel container)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            // IMPORTANT NOTE! *Never* configure a type as singleton if it depends upon ISession!!! This is because
            // ISession is disposed of at the end of every request. For the same reason, make sure that types
            // dependent upon such types aren't configured as singleton.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////

            //ConfigureLog4net(container);
            ConfigureUserSession(container);
            ConfigureNHibernate(container);
            //ConfigureDependenciesOnlyUsedForLegacyProcessing(container);
            ConfigureAutoMapper(container);

            //container.Bind<IBasicSecurityService>().To<BasicSecurityService>().InSingletonScope();
            //container.Bind<IDateTime>().To<DateTimeAdapter>().InSingletonScope();

            //container.Bind<ICommonLinkService>().To<CommonLinkService>().InRequestScope();
            //container.Bind<IPagedDataRequestFactory>().To<PagedDataRequestFactory>().InSingletonScope();

            //container.Bind<IAllStatusesInquiryProcessor>().To<AllStatusesInquiryProcessor>().InRequestScope();
            //container.Bind<IAllStatusesQueryProcessor>().To<AllStatusesQueryProcessor>().InRequestScope();
            //container.Bind<IStatusLinkService>().To<StatusLinkService>().InRequestScope();

            //container.Bind<IAllUsersInquiryProcessor>().To<AllUsersInquiryProcessor>().InRequestScope();
            //container.Bind<IAllUsersQueryProcessor>().To<AllUsersQueryProcessor>().InRequestScope();
            //container.Bind<IUserByIdInquiryProcessor>().To<UserByIdInquiryProcessor>().InRequestScope();
            //container.Bind<IUserByIdQueryProcessor>().To<UserByIdQueryProcessor>().InRequestScope();
            //container.Bind<IUserLinkService>().To<UserLinkService>().InRequestScope();

            //container.Bind<ITasksControllerDependencyBlock>().To<TasksControllerDependencyBlock>().InRequestScope();
            //container.Bind<IAddTaskMaintenanceProcessor>().To<AddTaskMaintenanceProcessor>().InRequestScope();
            //container.Bind<IAddTaskQueryProcessor>().To<AddTaskQueryProcessor>().InRequestScope();
            //container.Bind<IUpdateTaskMaintenanceProcessor>().To<UpdateTaskMaintenanceProcessor>().InRequestScope();
            //container.Bind<IUpdateablePropertyDetector>().To<JObjectUpdateablePropertyDetector>().InSingletonScope();
            //container.Bind<IUpdateTaskQueryProcessor>().To<UpdateTaskQueryProcessor>().InRequestScope();
            //container.Bind<IDeleteTaskQueryProcessor>().To<DeleteTaskQueryProcessor>().InRequestScope();
            //container.Bind<IStartTaskWorkflowProcessor>().To<StartTaskWorkflowProcessor>().InRequestScope();
            //container.Bind<ICompleteTaskWorkflowProcessor>().To<CompleteTaskWorkflowProcessor>().InRequestScope();
            //container.Bind<IReactivateTaskWorkflowProcessor>().To<ReactivateTaskWorkflowProcessor>().InRequestScope();
            //container.Bind<IUpdateTaskStatusQueryProcessor>().To<UpdateTaskStatusQueryProcessor>().InRequestScope();
            //container.Bind<IAllTasksInquiryProcessor>().To<AllTasksInquiryProcessor>().InRequestScope();
            //container.Bind<IAllTasksQueryProcessor>().To<AllTasksQueryProcessor>().InRequestScope();
            //container.Bind<ITaskByIdInquiryProcessor>().To<TaskByIdInquiryProcessor>().InRequestScope();
            //container.Bind<ITaskByIdQueryProcessor>().To<TaskByIdQueryProcessor>().InRequestScope();
            //container.Bind<ITaskLinkService>().To<TaskLinkService>().InRequestScope();

            //container.Bind<ITaskUsersInquiryProcessor>().To<TaskUsersInquiryProcessor>().InRequestScope();
            //container.Bind<ITaskUsersMaintenanceProcessor>().To<TaskUsersMaintenanceProcessor>().InRequestScope();
            //container.Bind<ITaskUsersLinkService>().To<TaskUsersLinkService>().InRequestScope();

            //container.Bind<IAddTaskMaintenanceProcessorV2>().To<AddTaskMaintenanceProcessorV2>().InRequestScope();
        }

        private void ConfigureAutoMapper(IKernel container)
        {
            //container.Bind<IAutoMapper>().To<AutoMapperAdapter>().InSingletonScope();
            //container.Bind<IAutoMapperTypeConfigurator>()
            //    .To<StatusEntityToStatusAutoMapperTypeConfigurator>()
            //    .InSingletonScope();
            //container.Bind<IAutoMapperTypeConfigurator>()
            //    .To<StatusToStatusEntityAutoMapperTypeConfigurator>()
            //    .InSingletonScope();
            //container.Bind<IAutoMapperTypeConfigurator>()
            //    .To<UserEntityToUserAutoMapperTypeConfigurator>()
            //    .InSingletonScope();
            //container.Bind<IAutoMapperTypeConfigurator>()
            //    .To<UserToUserEntityAutoMapperTypeConfigurator>()
            //    .InSingletonScope();
            //container.Bind<IAutoMapperTypeConfigurator>()
            //    .To<NewTaskToTaskEntityAutoMapperTypeConfigurator>()
            //    .InSingletonScope();
            //container.Bind<IAutoMapperTypeConfigurator>()
            //    .To<TaskEntityToTaskAutoMapperTypeConfigurator>()
            //    .InSingletonScope();
            //container.Bind<IAutoMapperTypeConfigurator>()
            //    .To<TaskToTaskEntityAutoMapperTypeConfigurator>()
            //    .InSingletonScope();

            //container.Bind<IAutoMapperTypeConfigurator>()
            //    .To<NewTaskV2ToTaskEntityAutoMapperTypeConfigurator>()
            //    .InRequestScope();
        }

        private void ConfigureUserSession(IKernel container)
        {
            //var userSession = new UserSession();
            //container.Bind<IUserSession>().ToConstant(userSession).InSingletonScope();
            //container.Bind<IWebUserSession>().ToConstant(userSession).InSingletonScope();
        }

        private void ConfigureNHibernate(IKernel container)
        {
            //var sessionFactory = Fluently.Configure()
            //    .Database(
            //        MsSqlConfiguration.MsSql2008.ConnectionString(
            //            c => c.FromConnectionStringWithKey("WebApi2BookDb")))
            //    .CurrentSessionContext("web")
            //    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<TaskMap>())
            //    .BuildSessionFactory();

            //container.Bind<ISessionFactory>().ToConstant(sessionFactory);
            //container.Bind<ISession>().ToMethod(CreateSession);
            //container.Bind<IActionTransactionHelper>().To<ActionTransactionHelper>().InRequestScope();
        }

        //private ISession CreateSession(IContext context)
        //{
        //    var sessionFactory = context.Kernel.Get<ISessionFactory>();
        //    if (!CurrentSessionContext.HasBind(sessionFactory))
        //    {
        //        var session = sessionFactory.OpenSession();
        //        CurrentSessionContext.Bind(session);
        //    }

        //    return sessionFactory.GetCurrentSession();
        //}

       
        //private void ConfigureLog4net(IKernel container)
        //{
        //    XmlConfigurator.Configure();

        //    var logManager = new LogManagerAdapter();
        //    container.Bind<ILogManager>().ToConstant(logManager);
        //}
    }
}