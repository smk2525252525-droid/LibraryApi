using LibraryApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;


//entry point of the application, Brain
//configures services and middleware, and runs the application.(cors, swagger, authentication, database seeding),connects appsettings.json (data) to the Controllers (logic).
//Program.cs starts, reads the DB string from appsettings.json, and checks libraryapi.csproj to make sure all tools are installed
var builder = WebApplication.CreateBuilder(args);


// Database
// Configure the DbContext to use SQL Server with the connection string from appsettings.json
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);


// JWT Authentication

var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "SuperSecretKeyForJWT123!";
var keyBytes = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>//tells app how to reach jwt tokens
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "MyLibraryApi",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "MyLibraryApi",
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
});


// Controllers & JSON options

builder.Services.AddControllers()//enables use of controller classes
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
    );

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//cross origin resource sharing: so javascript front end can access api
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

var app = builder.Build();

// Middleware: sequence of filters every request passes through
//swagger
app.UseSwagger();
app.UseSwaggerUI();


app.UseCors("AllowAll");//cross application allows to talk to frontend

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();//sends request to the specific controller based on the route



//to run and add seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<LibraryDbContext>();
        // This calls the static Initialize method we fixed earlier
        DbInitializer.Initialize(context); 
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}



app.Run();
