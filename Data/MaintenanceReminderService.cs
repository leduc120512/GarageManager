using AutoGarageManager.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoGarageManager.Data
{
    public class MaintenanceReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MaintenanceReminderService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); // Check daily

        public MaintenanceReminderService(IServiceProvider serviceProvider, ILogger<MaintenanceReminderService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Maintenance Reminder Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndCreateRemindersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking maintenance reminders.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Maintenance Reminder Service is stopping.");
        }

        private async Task CheckAndCreateRemindersAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Get all vehicles
                var vehicles = await context.Vehicles.ToListAsync();

                foreach (var vehicle in vehicles)
                {
                    // Check for oil change (every 5000 miles or 6 months)
                    if (vehicle.Mileage > 0 && vehicle.Mileage % 5000 == 0)
                    {
                        var existingReminder = await context.MaintenanceReminders
                            .FirstOrDefaultAsync(r => r.VehicleId == vehicle.Id &&
                                                     r.Title == "Oil Change" &&
                                                     !r.IsCompleted &&
                                                     r.ReminderDate > DateTime.Now);

                        if (existingReminder == null)
                        {
                            var reminder = new MaintenanceReminder
                            {
                                VehicleId = vehicle.Id,
                                Title = "Oil Change",
                                Message = $"Oil change due for {vehicle.Make} {vehicle.Model} ({vehicle.LicensePlate}) at {vehicle.Mileage} miles",
                                ReminderDate = DateTime.Now.AddDays(7) // Due in 7 days
                            };

                            context.MaintenanceReminders.Add(reminder);
                            _logger.LogInformation($"Created oil change reminder for vehicle {vehicle.Id}");
                        }
                    }

                    // Check for tire rotation (every 8000 miles)
                    if (vehicle.Mileage > 0 && vehicle.Mileage % 8000 == 0)
                    {
                        var existingReminder = await context.MaintenanceReminders
                            .FirstOrDefaultAsync(r => r.VehicleId == vehicle.Id &&
                                                     r.Title == "Tire Rotation" &&
                                                     !r.IsCompleted &&
                                                     r.ReminderDate > DateTime.Now);

                        if (existingReminder == null)
                        {
                            var reminder = new MaintenanceReminder
                            {
                                VehicleId = vehicle.Id,
                                Title = "Tire Rotation",
                                Message = $"Tire rotation due for {vehicle.Make} {vehicle.Model} ({vehicle.LicensePlate}) at {vehicle.Mileage} miles",
                                ReminderDate = DateTime.Now.AddDays(7)
                            };

                            context.MaintenanceReminders.Add(reminder);
                            _logger.LogInformation($"Created tire rotation reminder for vehicle {vehicle.Id}");
                        }
                    }

                    // Check for brake inspection (every 12000 miles)
                    if (vehicle.Mileage > 0 && vehicle.Mileage % 12000 == 0)
                    {
                        var existingReminder = await context.MaintenanceReminders
                            .FirstOrDefaultAsync(r => r.VehicleId == vehicle.Id &&
                                                     r.Title == "Brake Inspection" &&
                                                     !r.IsCompleted &&
                                                     r.ReminderDate > DateTime.Now);

                        if (existingReminder == null)
                        {
                            var reminder = new MaintenanceReminder
                            {
                                VehicleId = vehicle.Id,
                                Title = "Brake Inspection",
                                Message = $"Brake inspection due for {vehicle.Make} {vehicle.Model} ({vehicle.LicensePlate}) at {vehicle.Mileage} miles",
                                ReminderDate = DateTime.Now.AddDays(7)
                            };

                            context.MaintenanceReminders.Add(reminder);
                            _logger.LogInformation($"Created brake inspection reminder for vehicle {vehicle.Id}");
                        }
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}