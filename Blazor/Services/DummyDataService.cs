using Bogus;
using DomainModels;

namespace Blazor.Services
{
    public class DummyDataService
    {
        private readonly Faker _faker;
        private readonly Random _random;

        public DummyDataService(string locale = "da")
        {
            _faker = new Faker(locale);
            _random = new Random();
        }

        public DummyDataService(int seed, string locale = "da")
        {
            _faker = new Faker(locale) { Random = new Random(seed) };
            _random = new Random(seed);
        }

        /// <summary>
        /// Genererer en liste af dummy users med realistiske data
        /// </summary>
        public List<User> GenerateUsers(int count, int? maxBookingsPerUser = null)
        {
            var userFaker = new Faker<User>()
                .RuleFor(u => u.Id, f => Guid.NewGuid().ToString())
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Name))
                .RuleFor(u => u.PasswordHash, f => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("password123")))
                .RuleFor(u => u.CreatedAt, f => f.Date.Past(2))
                .RuleFor(u => u.UpdatedAt, (f, u) => u.CreatedAt.AddDays(f.Random.Int(0, 365)))
                .RuleFor(u => u.BookingUsers, f => new List<BookingUser>());

            var users = userFaker.Generate(count);

            // Tilføj bookings hvis ønsket
            if (maxBookingsPerUser.HasValue && maxBookingsPerUser.Value > 0)
            {
                foreach (var user in users)
                {
                    var bookingCount = _random.Next(0, maxBookingsPerUser.Value + 1);
                    user.BookingUsers = GenerateBookingsForUser(user.Id, bookingCount);
                }
            }

            return users;
        }

        /// <summary>
        /// Genererer bookings for en specifik user
        /// </summary>
        public List<BookingUser> GenerateBookingsForUser(string userId, int count)
        {
            var bookingUsers = new List<BookingUser>();

            for (int i = 0; i < count; i++)
            {
                var checkInDate = _faker.Date.Between(DateTime.Now.AddDays(-365), DateTime.Now.AddDays(365));
                var checkOutDate = checkInDate.AddDays(_random.Next(1, 15));
                var totalPrice = _random.Next(500, 5000);

                var booking = new Booking
                {
                    Id = Guid.NewGuid().ToString(),
                    CheckInDate = checkInDate,
                    CheckOutDate = checkOutDate,
                    TotalPrice = totalPrice,
                    CreatedAt = checkInDate.AddDays(-_random.Next(1, 30)),
                    UpdatedAt = checkInDate.AddDays(-_random.Next(0, 7))
                };

                var bookingUser = new BookingUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    BookingId = booking.Id,
                    Booking = booking
                };

                bookingUsers.Add(bookingUser);
            }

            return bookingUsers;
        }

        /// <summary>
        /// Genererer rooms med realistiske data
        /// </summary>
        public List<Room> GenerateRooms(int count)
        {
            var roomTypes = new[] { "Single", "Double", "Suite", "Family", "Deluxe" };
            var roomFaker = new Faker<Room>()
                .RuleFor(r => r.Id, f => Guid.NewGuid().ToString())
                .RuleFor(r => r.Number, f => f.Random.Int(100, 999).ToString())
                .RuleFor(r => r.Type, f => f.PickRandom(roomTypes))
                .RuleFor(r => r.Capacity, f => f.Random.Int(1, 6))
                .RuleFor(r => r.PricePerNight, f => f.Random.Decimal(500, 2500))
                .RuleFor(r => r.IsAvailable, f => f.Random.Bool(0.7f)) // 70% chance for at være ledig
                .RuleFor(r => r.CreatedAt, f => f.Date.Past(1))
                .RuleFor(r => r.UpdatedAt, (f, r) => r.CreatedAt.AddDays(f.Random.Int(0, 365)));

            return roomFaker.Generate(count);
        }

        /// <summary>
        /// Genererer kompleks data med users, rooms og bookings
        /// </summary>
        public DummyDataSet GenerateCompleteDataSet(int userCount, int roomCount, int? maxBookingsPerUser = null)
        {
            var users = GenerateUsers(userCount, maxBookingsPerUser);
            var rooms = GenerateRooms(roomCount);

            return new DummyDataSet
            {
                Users = users,
                Rooms = rooms,
                GeneratedAt = DateTime.Now,
                TotalUsers = users.Count,
                TotalRooms = rooms.Count,
                TotalBookings = users.Sum(u => u.BookingUsers?.Count ?? 0)
            };
        }

        /// <summary>
        /// Genererer performance test data
        /// </summary>
        public async Task<List<PerformanceTestResult>> RunPerformanceTests(int iterations, int delayMs = 0)
        {
            var results = new List<PerformanceTestResult>();
            var stopwatch = new System.Diagnostics.Stopwatch();

            for (int i = 0; i < iterations; i++)
            {
                // Test 1: User generation
                stopwatch.Restart();
                var users = GenerateUsers(100);
                stopwatch.Stop();
                
                results.Add(new PerformanceTestResult
                {
                    TestName = $"User Generation #{i + 1}",
                    DurationMs = stopwatch.ElapsedMilliseconds,
                    DataSize = users.Count,
                    Success = true
                });

                // Test 2: Complex data generation
                stopwatch.Restart();
                var complexData = GenerateCompleteDataSet(50, 25, 3);
                stopwatch.Stop();
                
                results.Add(new PerformanceTestResult
                {
                    TestName = $"Complex Data #{i + 1}",
                    DurationMs = stopwatch.ElapsedMilliseconds,
                    DataSize = complexData.TotalUsers + complexData.TotalRooms,
                    Success = true
                });

                if (delayMs > 0 && i < iterations - 1)
                {
                    await Task.Delay(delayMs);
                }
            }

            return results;
        }
    }

    /// <summary>
    /// Komplet dataset med alle genererede data
    /// </summary>
    public class DummyDataSet
    {
        public List<User> Users { get; set; } = new();
        public List<Room> Rooms { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
        public int TotalUsers { get; set; }
        public int TotalRooms { get; set; }
        public int TotalBookings { get; set; }
    }

    /// <summary>
    /// Resultat af en performance test
    /// </summary>
    public class PerformanceTestResult
    {
        public string TestName { get; set; } = "";
        public double DurationMs { get; set; }
        public int DataSize { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
