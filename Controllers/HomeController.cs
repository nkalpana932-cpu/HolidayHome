using System.Data;
using System.Diagnostics;
using HolidayHome.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace HolidayHome.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _connStr;
        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _connStr = config.GetConnectionString("DefaultConnection")!;

        }
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        // GET: /HolidayMaster
        [HttpGet]
        public async Task<IActionResult> StartBooking()
        {
            var items = new List<HolidayMaster>();

            await using var conn = new SqlConnection(_connStr);
            await using var cmd = new SqlCommand("dbo.GetHolidayMasters", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            await using var rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync())
            {
                items.Add(new HolidayMaster
                {
                    HolidayMasterId = rdr.GetInt32(rdr.GetOrdinal("HolidayMasterId")),
                    Location = rdr.IsDBNull(rdr.GetOrdinal("Location")) ? null : rdr.GetString(rdr.GetOrdinal("Location")),
                    Details = rdr.IsDBNull(rdr.GetOrdinal("Details")) ? null : rdr.GetString(rdr.GetOrdinal("Details")),
                    PricePerDay = rdr.IsDBNull(rdr.GetOrdinal("PricePerDay")) ? (decimal?)null : rdr.GetDecimal(rdr.GetOrdinal("PricePerDay"))
                });
            }

            return View(items); // expects a view to list items
        }

        // GET: /HolidayMaster/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            HolidayMaster? item = null;

            await using var conn = new SqlConnection(_connStr);
            await using var cmd = new SqlCommand("dbo.GetHolidayMasterById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            await using var rdr = await cmd.ExecuteReaderAsync();

            if (await rdr.ReadAsync())
            {
                item = new HolidayMaster
                {
                    HolidayMasterId = rdr.GetInt32(rdr.GetOrdinal("HolidayMasterId")),
                    Location = rdr.IsDBNull(rdr.GetOrdinal("Location")) ? null : rdr.GetString(rdr.GetOrdinal("Location")),
                    Details = rdr.IsDBNull(rdr.GetOrdinal("Details")) ? null : rdr.GetString(rdr.GetOrdinal("Details")),
                    PricePerDay = rdr.IsDBNull(rdr.GetOrdinal("PricePerDay")) ? (decimal?)null : rdr.GetDecimal(rdr.GetOrdinal("PricePerDay"))
                };
            }

            if (item == null) return NotFound();
            return View(item); // expects a details view
        }

        // GET: /HolidayMaster/Create
        [HttpGet]
        public IActionResult Create() => View(); // returns a form bound to HolidayMaster

        // POST: /HolidayMaster/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HolidayMaster model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await using var conn = new SqlConnection(_connStr);
            await using var cmd = new SqlCommand("dbo.InsertHolidayMaster", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@HolidayMasterId", SqlDbType.Int).Value = model.HolidayMasterId;
            cmd.Parameters.Add("@Location", SqlDbType.VarChar, 100).Value = (object?)model.Location ?? DBNull.Value;
            cmd.Parameters.Add("@Details", SqlDbType.VarChar, 255).Value = (object?)model.Details ?? DBNull.Value;
            cmd.Parameters.Add("@PricePerDay", SqlDbType.Decimal).Value = (object?)model.PricePerDay ?? DBNull.Value;
            // important to set precision/scale for decimals:
            cmd.Parameters["@PricePerDay"].Precision = 10;
            cmd.Parameters["@PricePerDay"].Scale = 2;

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            // after successful insert, go back to list
            return RedirectToAction(nameof(Index));
        }
    }
}
