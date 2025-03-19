using Central_Server.Data;
using Central_Server.Extensions;
using FileAccess = Central_Server.Data.FileAccess;

namespace Central_Server;

public class Program
{
	public static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

		builder.Services.AddControllers(options =>
		{
			options.Filters.Add<ProtectedController>();
		});

		FileAccess acc = new FileAccess();
		builder.Services.AddSingleton(acc);
		builder.Services.AddSingleton<MacAddrAccess>();
		builder.Services.AddSingleton<DeviceDataAccess>();
		builder.Services.AddSingleton<KeyDataAccess>();
		
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();
		
		WebApplication app = builder.Build();

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();

		app.UseAuthorization();

		app.UseDefaultFiles();
		app.UseStaticFiles();
		
		app.MapControllers();

		Console.WriteLine("Starting for EVU: " + acc.GetEvuName());
		app.Run();
	}
}