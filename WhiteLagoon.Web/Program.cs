using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Infrastructure.Repository;

namespace WhiteLagoon.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
            builder.Services.AddAutoMapper(typeof(MappingConfig));
            builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();

            //builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>();


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                   .AddEntityFrameworkStores<ApplicationDbContext>();
            // .AddDefaultTokenProviders();
            builder.Services.AddScoped<SignInManager<ApplicationUser>>();


            builder.Services.ConfigureApplicationCookie(option =>
            {
                option.AccessDeniedPath = "/Account/AccessDenied";
                option.LoginPath = "/Account/Login";

            });

            builder.Services.Configure<IdentityOptions>(option =>
            {
                option.Password.RequiredLength = 9;
                option.Password.RequireUppercase = true;
                option.Password.RequireDigit = true;
                option.Password.RequireNonAlphanumeric = true;
            });

            builder.Services.AddMvc().AddNToastNotifyToastr(new ToastrOptions
            {
                ProgressBar = false,
                PositionClass = ToastPositions.TopRight,
                TimeOut = 3000,
                HideMethod = "fadeOut"

            });

            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseNToastNotify();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
