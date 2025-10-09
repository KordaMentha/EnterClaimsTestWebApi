using EnterClaimsTestWebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<DataService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//var dataverseURL = builder.Configuration["Dataverse:EnvironmentUrl"];

string tenantId = Environment.GetEnvironmentVariable("SamTestClaims_tenantID").ToString(); //Azure AD Tenant ID 
string apiScope = Environment.GetEnvironmentVariable("SamTestClaims_apiScope").ToString(); //Api Scope

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";
        options.Audience = "{clientId}"; // or api://{clientId}
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Api.Read", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                (c.Type == "scp" || c.Type == "http://schemas.microsoft.com/identity/claims/scope") &&
                c.Value.Split(' ').Contains(apiScope))));
});

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("Api.Read", policy =>
//        policy.RequireClaim("scope", "https://tectestimportsolution-dev.crm6.dynamics.com/.default"));
//        //policy.RequireClaim("scope", dataverseURL+"/.default"));
//});

var app = builder.Build();

// Use CORS policy
app.UseCors("AllowSpecificOrigin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
