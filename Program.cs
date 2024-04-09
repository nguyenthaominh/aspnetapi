using backendaspnet.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Cấu hình thực hiện việc enable Cors
builder.Services.AddCors(o =>
{
	o.AddPolicy("AllowOrigin", p =>
	{
		p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
	});
});

// Cấu hình dịch vụ kết nối tới SQL Server
builder.Services.AddDbContext<BackendContext>(
		options =>
			options.UseSqlServer(
				builder.Configuration.GetConnectionString("DefaultConnection"),
				sqlServerOptionsAction: options =>
				{
					options.EnableRetryOnFailure();
				})
	);
builder.Services.AddDbContext<BackendContext>(ServiceLifetime.Transient);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.DD
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseCors(
		options => options.WithOrigins("http://localhost:3000").AllowAnyMethod()
	);

app.UseAuthorization();

app.MapControllers();
app.Run();
