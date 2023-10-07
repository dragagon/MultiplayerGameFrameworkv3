using Autofac;
using Microsoft.Extensions.Hosting;

namespace MGF_Lidgren
{
    public class LidgrenModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<LidgrenServer>().As<IHostedService>();
        }
    }
}
