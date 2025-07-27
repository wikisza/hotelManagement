using hotelASP.Data;
using hotelASP.Entities;
using hotelASP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BCrypt.Net;

namespace hotelASP.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
            .Include(u => u.Role)
            .ToListAsync();

            ViewBag.Roles = _context.Roles.ToList();

            return View(users);
        }
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Roles = _context.Roles.ToList();
            return View(user);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, FirstName, LastName, Email, Username, RoleId")] UserAccount account)
        {

            if (id != account.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.FindAsync(id);

                    if (existingUser == null)
                    {
                        return NotFound();
                    }
                    existingUser.FirstName = account.FirstName;
                    existingUser.LastName = account.LastName;
                    existingUser.Email = account.Email;
                    existingUser.Username = account.Username;
                    existingUser.RoleId = account.RoleId;


                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Edit), new { id });
        }

        private bool AccountExists(int Id)
        {
            return _context.Users.Any(e => e.Id == Id);
        }
        [Authorize]
        public async Task<IActionResult> Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var account = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == Id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            ViewBag.Roles = _context.Roles.ToList();
            return View(account);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Users.FindAsync(id);
            if (account != null)
            {
                _context.Users.Remove(account);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult Home()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Registration()
        {
            ViewBag.Roles = _context.Roles.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Registration(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.Users.Any(u => u.Email == model.Email || u.Username == model.Username);
                if (existingUser)
                {
                    ModelState.AddModelError("", "Email lub nazwa użytkownika już istnieje.");
                    ViewBag.Roles = _context.Roles.ToList();
                    return View(model);
                }
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                DateOnly date = DateOnly.FromDateTime(DateTime.Now);

                UserAccount account = new UserAccount
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Password = hashedPassword,
                    Username = model.Username,
                    CreateDate = date,
                    RoleId = model.RoleId
                };

                _context.Users.Add(account);
                _context.SaveChanges();

                ModelState.Clear();
                ViewBag.Message = $"Użytkownika {account.FirstName} {account.LastName} zarejestrowano pomyślnie. Proszę zaloguj się.";
                ViewBag.Roles = _context.Roles.ToList();
            }

            return View(model);

        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users
                    .Include(u => u.Role)
                    .Where(x => (x.Username == model.UsernameOrEmail || x.Email == model.UsernameOrEmail)).FirstOrDefault();
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    //success
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim("Name", user.FirstName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role.Name),
                        new Claim("CreateAccount", user.CreateDate.ToString()),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Home");
                }
                else
                {
                    ModelState.AddModelError("", "Username/Email lub hasło jest niepoprawne.");
                }

            }

            return View(model);
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

    }
}
