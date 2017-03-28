using Microsoft.Practices.Unity;
using bringpro.Web.Classes.SignalR;
using bringpro.Web.Classes.Tasks.Bringg;
using bringpro.Web.Classes.Tasks.Bringg.Interfaces;
using bringpro.Web.Core.Injection;
using bringpro.Web.Services;
using bringpro.Web.Services.Interfaces;
using System;
using System.Web.Http;
using System.Web.Mvc;
using Unity.Mvc5;

// Lines 44 & 45 Show Dependency Injection of Mandrill/Sendgrid Web Api
namespace bringpro.Web
{
    public static class UnityConfig
    {

        private static readonly Lazy<UnityContainer> _container = new Lazy<UnityContainer>(() => new UnityContainer());

        public static UnityContainer GetContainer()
        {
            return _container.Value;
        }

        public static void RegisterComponents(HttpConfiguration config)
        {
            UnityContainer container = new UnityContainer();
            container.RegisterType<IAdminUserService, AdminUserService>();
            container.RegisterType<IUserProfileService, UserProfileService>();
            container.RegisterType<IAdminReportsService, AdminReportsService>();
            container.RegisterType<IJobItemOptionsService, JobItemOptionsServices>();
            container.RegisterType<IAddressService, AddressService>();
            container.RegisterType<IUserAddressService, UserAddressService>();
            container.RegisterType<IDashboardService, DashboardService>();
            container.RegisterType<IMediaService, MediaService>();
            container.RegisterType<ICreditCardService, CreditCardService>();
            container.RegisterType<ICouponService, CouponService>();
            container.RegisterType<ICustomerRatingService, CustomerRatingService>();
            container.RegisterType<IEmailCampaignsService, EmailCampaignsService>();
            container.RegisterType<IHelpService, HelpService>();
            container.RegisterType<IUserCreditsService, UserCreditsService>();
            //To easily switch between Mandrill or Sendgrid Email service - just uncomment the line you want to utilize 
            //container.RegisterType<IUserEmailService, SendGridService>(); 
            container.RegisterType<IUserEmailService, MandrillService>();

            container.RegisterType<IUserRegistrationReport, UserRegistrationReport>();
            container.RegisterType<IJobsService, JobsService>();
            container.RegisterType<IJobsWaypointService, JobsWaypointService>();
            container.RegisterType<ITransactionLogService, TransactionLogService>();
            container.RegisterType<IWebsiteService, WebsiteService>();
            container.RegisterType<IContactRequestService, ContactRequestServices>();
            container.RegisterType<IBrainTreeService, BrainTreeService>();
            container.RegisterType<IAdminHub, AdminHub>();
            container.RegisterType<IReferralService, ReferralService>();
            container.RegisterType<IActivityLogService, ActivityLogService>();
            container.RegisterType<IBringgUserService, BringgUserService>();

            container.RegisterType(typeof(IBringgTask<>), typeof(CreateUser<>), "CreateUser");
            container.RegisterType(typeof(IBringgTask<>), typeof(UpdateUser<>), "UpdateUser");
            container.RegisterType(typeof(IBringgTask<>), typeof(DeleteUser<>), "DeleteUser");


            /*the next two lines are possible solutions to make Bringg dependency injection working
             * the error was occuring when a jobId was being generated before getting to adding waypoints.
             * The error message included: 
             * exceptionMessage: "Object reference not set to an instance of an object."
             * stackTrace: "at bringpro.Web.Controllers.Api.JobsApiController.JobInsert(JobInsertRequest model)" */
            //container.RegisterType(typeof(IBringgTask<>), new InjectionConstructor(new GenericParameter("T", "CreateTask")));//typeof(CreateTask<>), "CreateTask");
            //container.RegisterType(typeof(IBringgTask<Job>), typeof(CreateTask<>), "CreateJobTask");


            container.RegisterType(typeof(IBringgTask<>), typeof(CreateTask<>), "CreateTask");
            container.RegisterType(typeof(IBringgTask<>), typeof(CreateTaskWithWaypoints<>), "CreateTaskWithWaypoints");
            container.RegisterType(typeof(IBringgTask<>), typeof(CreateWaypoint<>), "CreateWaypoint");
            container.RegisterType(typeof(IBringgTask<>), typeof(UpdateCustomerTask<>), "UpdateCustomerTask");
            container.RegisterType(typeof(IBringgTask<>), typeof(UpdateTeamTask<>), "UpdateTeamTask");
            container.RegisterType(typeof(IBringgTask<>), typeof(DeleteTeamTask<>), "DeleteTeamTask");

            container.RegisterType<IWebsiteTeamService, WebsiteTeamService>();

            container.RegisterType(typeof(IBringgTask<>), typeof(TestingTask<>), "TestingTask");
            container.RegisterType(typeof(IBringgTask<>), typeof(CreateCustomerTask<>), "CreateCustomerTask");
            container.RegisterType(typeof(IBringgTask<>), typeof(CreateTeamTask<>), "CreateTeamTask");

            container.RegisterType<IAdminJobScheduleService, AdminJobScheduleService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            //  this line is needed so that the resolver can be used by api controllers 
            config.DependencyResolver = new bringpro.Web.Core.Injection.UnityResolver(container);

            var resolver = new UnityDependencyResolver(container);
            //container.RegisterType<IOutputService, OutputService>();
            DependencyResolver.SetResolver(resolver);
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
          
            //  this line is needed so that the resolver can be used by api controllers 
                        config.DependencyResolver = new UnityResolver(container);

            //  we have to make a custom filter injector to provide DI to our custom action filters.
            //  see http://michael-mckenna.com/blog/dependency-injection-for-asp-net-web-api-action-filters-in-3-easy-steps
            //var providers = config.Services.GetFilterProviders().ToList();

            //var defaultprovider = providers.Single(i => i is ActionDescriptorFilterProvider);
            //config.Services.Remove(typeof(System.Web.Http.Filters.IFilterProvider), defaultprovider);

            config.Services.Add(typeof(System.Web.Http.Filters.IFilterProvider), new UnityActionFilterProvider(container));


        }
    }
}

