using System;
using System.Reflection;
using Autofac;
using Grassroots.Application.Commands;
using Grassroots.Application.Dispatchers;
using Grassroots.Application.Events;
using Grassroots.Application.Queries;
using Grassroots.Domain.Events;
using Grassroots.Domain.Repositories;
using Grassroots.Infrastructure.Commands;
using Grassroots.Infrastructure.Data;
using Grassroots.Infrastructure.Events;
using Grassroots.Infrastructure.Mapping;
using Grassroots.Infrastructure.Queries;
using Grassroots.Infrastructure.Repositories;
using Grassroots.Model.Mapping;
using Microsoft.Extensions.Configuration;
using IMapperInterface = Grassroots.Model.Mapping.IMapper;

namespace Grassroots.Infrastructure.DependencyInjection.AutofacModules
{
    public class InfrastructureModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;

        public InfrastructureModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // 注册命令和查询分发器
            builder.RegisterType<CommandDispatcher>().As<ICommandDispatcher>().InstancePerLifetimeScope();
            builder.RegisterType<QueryDispatcher>().As<IQueryDispatcher>().InstancePerLifetimeScope();

            // 注册AutoMapper适配器
            builder.RegisterType<AutoMapperAdapter>().As<IMapperInterface>().SingleInstance();

            // 注册事件相关服务
            builder.RegisterType<DomainEventBus>().As<IDomainEventBus>().SingleInstance();
            builder.RegisterType<IntegrationEventBus>().As<IIntegrationEventBus>().SingleInstance();
            builder.RegisterType<EventStore>().As<IEventStore>().InstancePerLifetimeScope();
            builder.RegisterType<EventMediator>().As<IEventMediator>().InstancePerLifetimeScope();

            // 注册仓储
            builder.RegisterGeneric(typeof(Repository<>))
                .As(typeof(IRepository<>))
                .InstancePerLifetimeScope();
        }
    }
} 