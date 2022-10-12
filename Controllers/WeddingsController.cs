using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Wedding_Planner.Models;
using Microsoft.EntityFrameworkCore;

namespace Wedding_Planner.Controllers;

public class WeddingsController : Controller
{
    private int? uid
    {
        get{
            return HttpContext.Session.GetInt32("UUID");
        }
    }

    private bool loggedIn
    {
        get{
            return uid != null;
        }
    }
    private MyContext _context;

    public WeddingsController(MyContext context)
    {
        _context = context;
    }
    
    [HttpGet("/weddings")]
    public IActionResult AllWeddings(){
        if(!loggedIn){
            return RedirectToAction("Index", "Users");
        }

        // Get all Weddings
        List<Wedding> All_Weddings = _context.Weddings
        .Include(w => w.Guests)
        .Include(w => w.User)
        .ToList();

        foreach(Wedding date in All_Weddings){
            if(date.Date < DateTime.Now){
                _context.Weddings.Remove(date);
                _context.SaveChanges();
            }
        }

        ViewBag.Wed = All_Weddings;
        return View("AllWeddings");
    }

    [HttpPost("/rsvp/{WeddingId}/guest")]
    public IActionResult RSVP(int WeddingId, Wedding newWed){
        if(!loggedIn || uid == null){
            return RedirectToAction("AllWeddings");
        }

        Planning? un_RSVP = _context.Plannings.FirstOrDefault(p =>p.WeddingId == WeddingId && p.UserId == (int)uid);
        // User? user = _context.Users.FirstOrDefault(u => u.UserId == newWed.UserId);
        // Wedding? guest = _context.Weddings.FirstOrDefault(w => w.UserId == newWed.UserId);

        if(un_RSVP == null){
            Planning RSVP = new Planning(){
                UserId = (int)uid,
                WeddingId = WeddingId
            };

            _context.Plannings.Add(RSVP);
            // user.RSVPs.Add(RSVP);
            // guest.Guests.Add(RSVP);

        }else if(un_RSVP != null){
            _context.Plannings.Remove(un_RSVP);
            // user.RSVPs.Remove(un_RSVP);
            // guest.Guests.Remove(un_RSVP);
        }

        

        _context.SaveChanges();
        return RedirectToAction("AllWeddings");
    }

    [HttpGet("/weddings/new")]
    public IActionResult New(){
        if(!loggedIn){
            return RedirectToAction("Index", "Users");
        }
        return View();
    }

    [HttpPost("/weddings/new")]
    public IActionResult CreateWedding(Wedding newWedding)
    {
        if(!loggedIn || uid == null){
            return RedirectToAction("Index", "Users");
        }

        if(ModelState.IsValid){
            User? AddWedding = _context.Users.Include(p => p.PlannedWeddings).FirstOrDefault(d => d.UserId == (int)uid);
            
            if(AddWedding != null){
                AddWedding.PlannedWeddings.Add(newWedding);
            }

            // We can take the Wedding object created from a form submission
            // And pass this object to the .Add() method
            _context.Weddings.Add(newWedding);
            // OR _context.Weddings.Add(newWedding);
            _context.SaveChanges();
            // Other code
            return RedirectToAction("AllWeddings");
        }
        //return RedirectToAction("New");
        return View("New");
    }

    [HttpGet("/weddings/{WeddingId}")]
    public IActionResult One(int WeddingId)
    {
        if(!loggedIn){
            return RedirectToAction("Index", "Users");
        }
        
        Wedding? oneWedding = _context.Weddings
        .Include(w => w.Guests)
            .ThenInclude(p => p.User)
        .FirstOrDefault(Wedding => Wedding.WeddingId == WeddingId);
        // Other code
        if(oneWedding == null){
            return View("AllWeddings");
        }
        return View("One", oneWedding);
    }

    [HttpGet("delete/{WeddingId}")]
    public IActionResult DeleteWedding(int WeddingId)
    {
        if(!loggedIn){
            return RedirectToAction("Index", "Users");
        }

        // Like Update, we will need to query for a single Wedding from our Context object
        Wedding? RetrievedWedding = _context.Weddings
            .SingleOrDefault(w => w.WeddingId == WeddingId);
        
        // Then pass the object we queried for to .Remove() on Weddings
        if(RetrievedWedding != null){
            _context.Weddings.Remove(RetrievedWedding);
        }
        
        // Finally, .SaveChanges() will remove the corresponding row representing this Wedding from DB 
        _context.SaveChanges();
        // Other code
        return RedirectToAction("AllWeddings");
    }


    // [HttpGet("/weddings/{WeddingId}/edit")]
    // public IActionResult Update(int WeddingId){
    //     if(!loggedIn){
    //         return RedirectToAction("Index", "Users");
    //     }

    //     Wedding? editWedding = _context.Weddings.FirstOrDefault(Wedding => Wedding.WeddingId == WeddingId);
    //     // Other code
    //     if(editWedding == null){
    //         return View("AllWeddings");
    //     }
    //     return View("Update", editWedding);
    // }

    // [HttpPost("/weddings/{WeddingId}/edit")]
    // public IActionResult UpdateWedding(int WeddingId, Wedding editedWedding)
    // {
    //     if(!loggedIn){
    //         return RedirectToAction("Index", "Users");
    //     }

    //     if(ModelState.IsValid){
    //         // We must first Query for a single Wedding from our Context object to track changes.
    //         Wedding RetrievedWedding = _context.Weddings
    //             .FirstOrDefault(Wedding => Wedding.WeddingId == WeddingId);

    //         // Then we may modify properties of this tracked model object
    //         RetrievedWedding.UserName = editedWedding.UserName;
    //         RetrievedWedding.WeddingName = editedWedding.WeddingName;
    //         RetrievedWedding.Calories = editedWedding.Calories;
    //         RetrievedWedding.Tastiness = editedWedding.Tastiness;
    //         //RetrievedWedding.Description = editedWedding.Description;
    //         RetrievedWedding.UpdatedAt = DateTime.Now;
            
    //         // Finally, .SaveChanges() will update the DB with these new values
    //         _context.SaveChanges();
            
    //         // Other code
    //         return RedirectToAction("AllWeddings");
    //     }
    //     return View("Update", WeddingId);
    // }

}