namespace Central_Server;

public class Program
{
	public static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

		// Add services to the container.

		builder.Services.AddControllers();
		FileAccess acc = new FileAccess();
		builder.Services.AddSingleton(acc);
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
		app.Run("http://0.0.0.0:80");
	}
}