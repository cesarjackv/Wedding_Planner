using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Wedding_Planner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Wedding_Planner.Controllers;

public class UsersController : Controller
{
    private MyContext _context;

    public UsersController(MyContext context)
    {
        _context = context;
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
        return View("Index");
    }

    // [HttpGet("/")]
    // public IActionResult AllUsers(){
    //     // Get all Users
    //     List<User> Users_Stats = _context.Users.Include(p => p.PlannedWeddings).ToList();
    //     return View("AllUsers", Users_Stats);
    // }

    [HttpPost("/register")]
    public IActionResult Register(User newUser)
    {
        if(ModelState.IsValid){
            if(_context.Users.Any(u => u.Email == newUser.Email)){
                ModelState.AddModelError("Email", "is taken");
            }
        }else{
            return Index();
        }

        PasswordHasher<User> hashSlingingSlasher = new PasswordHasher<User>();
        newUser.Password = hashSlingingSlasher.HashPassword(newUser, newUser.Password);

        _context.Users.Add(newUser);
        _context.SaveChanges();

        HttpContext.Session.SetInt32("UUID", newUser.UserId);
        HttpContext.Session.SetString("FirstName", newUser.FirstName);
        HttpContext.Session.SetString("LastName", newUser.LastName);
        HttpContext.Session.SetString("FullName", $"{newUser.FirstName} {newUser.LastName}");
        return RedirectToAction("AllWeddings", "Weddings");
    }

    [HttpPost("/login")]
    public IActionResult Login(LoginUser loginUser)
    {
        if(!ModelState.IsValid){
            return Index();
        }

        User? dbUser = _context.Users.FirstOrDefault(u => u.Email == loginUser.LoginEmail); 

        if(dbUser == null){
            ModelState.AddModelError("LoginEmail", "invalid.");
            return Index();
        }

        PasswordHasher<LoginUser> hashSlingingSlasher = new PasswordHasher<LoginUser>();
        PasswordVerificationResult pwCompareResult = hashSlingingSlasher.VerifyHashedPassword(loginUser, dbUser.Password, loginUser.LoginPassword);

        
        if(pwCompareResult == 0){
            ModelState.AddModelError("LoginPassword", "invalid.");
            return Index();
        }

        HttpContext.Session.SetInt32("UUID", dbUser.UserId);
        HttpContext.Session.SetString("FirstName", dbUser.FirstName);
        HttpContext.Session.SetString("LastName", dbUser.LastName);
        HttpContext.Session.SetString("FullName", $"{dbUser.FirstName} {dbUser.LastName}");
        return RedirectToAction("AllWeddings", "Weddings");
    }

    [HttpPost("/logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
}