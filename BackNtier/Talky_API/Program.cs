using BLL.Abstrack;
using BLL.Concret;
using BLL.Mapper;
using Core.Helpers;
using DAL.Abstrack;
using DAL.Concret;
using DAL.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Talky_API.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(Map));

// Helpers
builder.Services.AddScoped<JwtHelper>();

// DAL Services
builder.Services.AddScoped<IUserDAL, UserDal>();
builder.Services.AddScoped<IMessageDal, MessageDal>();
builder.Services.AddScoped<ICallDAL, CallDAL>();
builder.Services.AddScoped<IGroupDAL, GroupDAL>();
builder.Services.AddScoped<IGroupmemberDAL, GroupMemberDAL>();
builder.Services.AddScoped<IGroupMessageDAL, GroupMessageDAL>();
builder.Services.AddScoped<IBlockedUserDAL, BlockedUserDAL>();
builder.Services.AddScoped<IContactDAL, ContactDAL>();
builder.Services.AddScoped<IStoryDAL, StoryDAL>();
builder.Services.AddScoped<IStoryViewDAL, StoryViewDAL>();
builder.Services.AddScoped<IMovieRoomDAL, MovieRoomDAL>();
builder.Services.AddScoped<IMovieRoomParticipantDAL, MovieRoomParticipantDAL>();
builder.Services.AddScoped<IMovieRoomMessageDAL, MovieRoomMessageDAL>();

// BLL Services
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IMessageService, MessageManager>();
builder.Services.AddScoped<ICallService, CallManager>();
builder.Services.AddScoped<IGroupService, GroupManager>();
builder.Services.AddScoped<IGroupMemberService, GroupMemberManager>();
builder.Services.AddScoped<IGroupMessageService, GroupMessageManager>();
builder.Services.AddScoped<IBlockedUserService, BlockedUserManager>();
builder.Services.AddScoped<IContactService, ContactManager>();
builder.Services.AddScoped<IStoryService, StoryManager>();
builder.Services.AddScoped<IStoryViewService, StoryViewManager>();
builder.Services.AddScoped<IMovieRoomService, MovieRoomManager>();
builder.Services.AddScoped<IMovieRoomMessageService, MovieRoomMessageManager>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    // Configure JWT for SignalR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && 
                (path.StartsWithSegments("/chatHub") || path.StartsWithSegments("/movieRoomHub")))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.SetIsOriginAllowed(_ => true)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials());
});

// SignalR
builder.Services.AddSignalR();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Talky API",
        Version = "v1",
        Description = "Talky Messaging Application API with N-Tier Architecture",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Talky Team"
        }
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Talky API v1");
        c.RoutePrefix = string.Empty; // Swagger UI at root
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
app.MapHub<MovieRoomHub>("/movieRoomHub");

app.Run();
