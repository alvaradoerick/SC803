using AseIsthmusAPI.Data;
using AseIsthmusAPI.Repositories;
using AseIsthmusAPI.Repositories.Interfaces;
using AseIsthmusAPI.Services;
using AseIsthmusAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(c => {
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}); //allow origin

// JSON Serializer
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
    .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// DB Context
builder.Services.AddSqlServer<AseItshmusContext>(builder.Configuration.GetConnectionString("AseIsthmusConn"));

builder.Services.AddSqlServer<AseItshmusContext>(builder.Configuration.GetConnectionString("AseIsthmusConnDev"));

builder.Services.AddHttpContextAccessor();

#region Service Layer


builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<BeneficiaryService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ICategoryAgreementsService, CategoryAgreementService>();
builder.Services.AddScoped<ICategoryAgreementsRepository, CategoryAgreementsRepository>();
builder.Services.AddScoped<AgreementService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<SavingsTypeService>();
builder.Services.AddScoped<LoansTypeService>();
builder.Services.AddScoped<ContributionUsageService>();
builder.Services.AddScoped<SavingsRequestService>();
builder.Services.AddScoped<LoanRequestService>();
builder.Services.AddScoped<DocumentsService>(); 
builder.Services.AddScoped<ContributionBalanceService>(); 
builder.Services.AddScoped<SavingsBalanceService>();
builder.Services.AddScoped<LoanBalanceService>();
builder.Services.AddScoped<ExcelService>();

#endregion

// Authentication and Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrator", policy => policy.RequireClaim("RoleType", "Administrador"));
    options.AddPolicy("Loan-Approvers", policy => policy.RequireClaim("RoleType", "Administrador", "Presidente", "Tesorero"));
});


var app = builder.Build();


app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
