using MongoDB.Driver;
using MongoDB.Entities;
using Polly;
using Polly.Extensions.Http;
using SearchService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());
builder.Services.AddAuthentication();

var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async ()=>{
    try{
        await DbInitializer.InitDb(app);
    }
    catch(Exception e){
        Console.WriteLine(e);
    }
});

// try{
//     await DbInitializer.InitDb(app);
// }
// catch(Exception e){
//     Console.WriteLine(e);
// }


app.Run();

static IAsyncPolicy<HttpResponseMessage> GetPolicy()
     => HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_=>TimeSpan.FromSeconds(3));

