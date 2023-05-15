using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Security.Requirements;
using App.Services;
using ex.models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
namespace ex
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddOptions();
            var mailSetting = Configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailSetting);
            services.AddSingleton<IEmailSender, SendMailService>();

            services.AddRazorPages();
            services.AddDbContext<MyBlogContext>(options =>
            {
                string connection = Configuration.GetConnectionString("MyBlogContext");
                options.UseSqlServer(connection);
            });

            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<MyBlogContext>()
                    .AddDefaultTokenProviders();
            // services.AddDefaultIdentity<AppUser>()
            //         .AddEntityFrameworkStores<MyBlogContext>()
            //         .AddDefaultTokenProviders();

            // Truy cập IdentityOptions
            services.Configure<IdentityOptions>(options =>
            {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 3 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
                options.SignIn.RequireConfirmedAccount = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login/";
                options.LogoutPath = "/logout/";
                options.AccessDeniedPath = "/access-denied.html";
            });
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    var gconfig = Configuration.GetSection("Authentication:Google");
                    options.ClientId = gconfig["ClientId"];
                    options.ClientSecret = gconfig["ClientSecret"];
                    // Default CallBackPath = https://localhost:5001/signin-google
                    options.CallbackPath = "/dang-nhap-tu-google";
                })
                // .AddFacebook()
                // .AddTwitter()
                // .AddMicrosoftAccount()
                ;
            services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();
            // services.Configure<SecurityStampValidatorOptions>(options => {
            //     options.ValidationInterval = TimeSpan.FromSeconds(30);
            // });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AllowEditRole", policyBuilder =>
                {
                    // Dieu kien Policy
                    policyBuilder.RequireAuthenticatedUser();

                    policyBuilder.RequireClaim("canedit", "user");
                    // policyBuilder.RequireRole("Admin");
                    // policyBuilder.RequireRole("Editor");
                    // policyBuilder.RequireClaim("TEn claim", "gia tri 1", "gia tri 2");
                    // policyBuilder.RequireClaim();

                    // IdentityRoleClaim<string> claim1;
                    // IdentityUserClaim<string> claim2;
                });
                options.AddPolicy("InGenZ", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.Requirements.Add(new GenZRequirement());
                });
                options.AddPolicy("ShowAdminMenu", pb =>
                {
                    pb.RequireRole("Admin");
                });
                options.AddPolicy("CanUpdateArticle", pb =>
                {
                    pb.Requirements.Add(new ArticleUpdateRequirement());
                });
            });
            // AuthorizationHandler phai la AddTransient (Tao ra doi tuong AuthorizationHandler cho moi truy van)
            services.AddTransient<IAuthorizationHandler, AppAuthorizationHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });


        }
    }
}

/*
    CREATE, READ, UPDATE, DELETE (CRUD)
    dotnet aspnet-codegenerator razorpage -m razorweb.models.Article -dc razorweb.models.MyBlogContext -outDir Pages/Blog -udl --referenceScriptLibraries

    dotnet aspnet-codegenerator razorpage -m ex.models.Article -dc ex.models.MyBlogContext -outDir Pages/Blog -udl --referenceScriptLibraries

    Identity
        - Authentication: Xac dinh danh tinh -> Login, Logout ...
        - Authorization: Xac thuc quyen truy cap
            Role-base authorization - xac thuc quyen theo vai tro
            -Role (vai tro): (Admin, Editor, Manager, Member, ...)
            
            Index
            Create
            Edit
            Delete

            * Policy-based authorization
            * Claims-based authorization
                Claims -> Dac tinh tinh chat cua 1 doi tuong

                Ví dụ Bằng lái E2 (Role) -> Được lái xe 4 chỗ
                - Ngày sinh -> claim
                - Nơi sinh -> claim

                Mua rượu (> 18 tuổi)
                    - Kiểm tra ngày sinh là Claims-based

            dotnet new page -n Index -o Areas/Admin/Pages/Role -p:n App.Admin.Role

            dotnet new page -n Create -o Areas/Admin/Pages/Role -p:n App.Admin.Role
            dotnet new page -n EditUserRoleClaim -o Areas/Admin/Pages/User -p:n App.Admin.User

            [Authorize] - Controller, Action, PageModel -> Dang nhap

        - Quan ly User

    /Identity/Account/Login
    /Identity/Account/Manage

    Phat sinh code thay vi mac dinh cho dang nhap

    dotnet aspnet-codegenerator identity -dc ex.models.MyBlogContext
    CallbackPath
    https://localhost:5001/dang-nhap-tu-google

*/