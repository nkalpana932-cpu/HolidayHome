using HolidayHome.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HolidayHome.Controllers
{
    public class RegisterController : Controller
    {
        private readonly string _connStr;
        public RegisterController(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection")!;
        }
        public IActionResult CreateUser()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUser(RegisterModel model)
        {
            if (ModelState.IsValid)
            {

                using (SqlConnection conn = new SqlConnection(_connStr))
                {
                    string query = "InsertRegister";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                    cmd.Parameters.AddWithValue("@AddressLine1", model.AddressLine1 ?? "");
                    cmd.Parameters.AddWithValue("@AddressLine2", model.AddressLine2 ?? "");
                    cmd.Parameters.AddWithValue("@State", model.State);
                    cmd.Parameters.AddWithValue("@ZipCode", model.ZipCode);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                ViewBag.Message = "Registration saved to SQL!";
                return View();
            }

            return View(model);
        }

        public IActionResult UserList()
        {
            List<RegisterModel> users = new List<RegisterModel>();

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                string query = "GetUserList";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new RegisterModel
                    {
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Email = reader["Email"].ToString(),
                        PhoneNumber = reader["PhoneNumber"].ToString(),
                        AddressLine1 = reader["AddressLine1"].ToString(),
                        AddressLine2 = reader["AddressLine2"].ToString(),
                        State = reader["State"].ToString(),
                        ZipCode = reader["ZipCode"].ToString()
                    });
                }
                conn.Close();
            }

            return View(users);
        }
        // EDIT (GET) — Email is PK
        public IActionResult Edit(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return NotFound();

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(@"GetUserList", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", email);
            conn.Open();
            using var rdr = cmd.ExecuteReader();
            if (!rdr.Read()) return NotFound();

            var model = new RegisterModel
            {
                Email = rdr.GetString(0),
                FirstName = rdr.GetString(1),
                LastName = rdr.GetString(2),
                PhoneNumber = rdr.GetString(3),
                AddressLine1 = rdr.IsDBNull(4) ? null : rdr.GetString(4),
                AddressLine2 = rdr.IsDBNull(5) ? null : rdr.GetString(5),
                State = rdr.GetString(6),
                ZipCode = rdr.GetString(7)
            };
            return View(model);
        }
        // GET: Register/Details/email
        public IActionResult Details(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return NotFound();

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand("GetUserList", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", email);

            conn.Open();
            using var rdr = cmd.ExecuteReader();
            if (!rdr.Read()) return NotFound();

            var model = new RegisterModel
            {
                Email = rdr.GetString(0),
                FirstName = rdr.GetString(1),
                LastName = rdr.GetString(2),
                PhoneNumber = rdr.GetString(3),
                AddressLine1 = rdr.IsDBNull(4) ? null : rdr.GetString(4),
                AddressLine2 = rdr.IsDBNull(5) ? null : rdr.GetString(5),
                State = rdr.GetString(6),
                ZipCode = rdr.GetString(7)
            };
            return View(model);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return NotFound();

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand("dbo.DeleteRegisterByEmail", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", email);

            conn.Open();
            cmd.ExecuteNonQuery();

            return RedirectToAction("UserList");
        }

        // GET: /Register/Delete?email=someone@example.com
        public IActionResult Delete(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return NotFound();

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand("GetUserList", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", email);

            conn.Open();
            using var rdr = cmd.ExecuteReader();
            if (!rdr.Read()) return NotFound();

            var model = new RegisterModel
            {
                Email = rdr.GetString(0),
                FirstName = rdr.GetString(1),
                LastName = rdr.GetString(2),
                PhoneNumber = rdr.GetString(3),
                AddressLine1 = rdr.IsDBNull(4) ? null : rdr.GetString(4),
                AddressLine2 = rdr.IsDBNull(5) ? null : rdr.GetString(5),
                State = rdr.GetString(6),
                ZipCode = rdr.GetString(7)
            };

            return View(model); // looks for Views/Register/Delete.cshtml
        }

        // EDIT (POST)
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(string email, RegisterModel model)
        {
            // Email is the key; keep it readonly in the form and trust route value
            if (email != model.Email) ModelState.AddModelError("Email", "Email mismatch.");
            if (!ModelState.IsValid) return View(model);

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand("dbo.UpdateRegister", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", model.Email!);

            cmd.Parameters.AddWithValue("@FirstName", model.FirstName!);
            cmd.Parameters.AddWithValue("@LastName", model.LastName!);
            cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber!);
            cmd.Parameters.AddWithValue("@AddressLine1", (object?)model.AddressLine1 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AddressLine2", (object?)model.AddressLine2 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@State", model.State!);
            cmd.Parameters.AddWithValue("@ZipCode", model.ZipCode!);

            conn.Open();
            var rows = cmd.ExecuteNonQuery();
            if (rows == 0) return NotFound(); // email not found
            return View(model);
        }
    }
}
