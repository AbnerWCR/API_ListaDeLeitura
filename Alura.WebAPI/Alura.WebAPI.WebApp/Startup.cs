using Alura.ListaLeitura.HttpClients;
using Alura.WebAPI.WebApp.Formatters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Alura.ListaLeitura.WebApp
{
    public class Startup
    {
        private string _baseAddressLivroApi = "http://localhost:6000/api/";
        private string _baseAddressAuthApi = "http://localhost:5000/api/";
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<LeituraContext>(options => {
            //    options.UseSqlServer(Configuration.GetConnectionString("ListaLeitura"));
            //});

            //services.AddDbContext<AuthDbContext>(options => {
            //    options.UseSqlServer(Configuration.GetConnectionString("AuthDB"));
            //});

            //services.AddIdentity<Usuario, IdentityRole>(options =>
            //{
            //    options.Password.RequiredLength = 3;
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequireUppercase = false;
            //    options.Password.RequireLowercase = false;
            //}).AddEntityFrameworkStores<AuthDbContext>();

            services.AddHttpContextAccessor();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options  => 
                    options.LoginPath = "/Usuario/Login");

            services.AddHttpClient<LivroApiClient>(client => {
                client.BaseAddress = new Uri(_baseAddressLivroApi);
            });

            services.AddHttpClient<AuthApiClient>(client => {
                client.BaseAddress = new Uri(_baseAddressAuthApi);
            });

            //services.ConfigureApplicationCookie(options => {
            //    options.LoginPath = "/Usuario/Login";
            //});

            //services.AddTransient<IRepository<Livro>, RepositorioBaseEF<Livro>>();

            services.AddMvc(options => {
                options.OutputFormatters.Add(new LivroCsvFormatter());
            }).AddXmlSerializerFormatters();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
