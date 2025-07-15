using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using OfficeNet.Exceptions;
using OfficeNet.Extensions;
using OfficeNet.Filters;
using OfficeNet.Infrastructure.Context;
using OfficeNet.Infrastructure.Mapping;
using OfficeNet.Permissons;
using OfficeNet.Service;
using OfficeNet.Service.Department;
using OfficeNet.Service.DiscussionForum;
using OfficeNet.Service.OpinionPoll;
using OfficeNet.Service.PlantService;
using OfficeNet.Service.Roles;
using OfficeNet.Service.Survey;
using OfficeNet.Service.Thought;
using OfficeNet.Service.TokenService;
using OfficeNet.Service.UserService;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Auth", Version = "v1", Description = "Services to Authenticate user" });


    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter a valid token in the following format: {your token here} do not add the word 'Bearer' before it."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});



// Adding Database context 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlDbConnection"));
});

// Adding Identity

//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()  this commented because i changed Identitu role to applicatioin role
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();
//RegisteringService here
builder.Services.AddServicesByConvention(typeof(Program).Assembly);

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


///Auto Register Claims
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    //foreach (var permission in PolicyRegistrar.GetAllPermissions())
    //{
    //    options.AddPolicy(permission, policy =>
    //        policy.RequireClaim("Permission", permission));
    //}
});

//builder.Services.AddControllers(options =>
//{
//    options.Filters.Add<SetUserContextFilter>();  // Apply globally
//});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DdMmYyyyDateConverter());
    });

// Regsitering AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);




// Adding Jwt from extension method
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.ConfigureCors();



var app = builder.Build();

//Configuring dynamic policies from database
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var authorizationOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<AuthorizationOptions>>().Value;
    OfficeNet.Permissons.DynamicPolicyRegister.RegisterPermissionsFromDatabase(authorizationOptions, serviceProvider);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();


app.UseExceptionHandler();

app.MapControllers();

app.Run();
