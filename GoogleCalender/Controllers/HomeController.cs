using Elfie.Serialization;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using GoogleCalender.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace GoogleCalender.Controllers
{
    public class HomeController : Controller
    {
        private readonly string CalendarId = "primary";
        private readonly UserCredential credential;
        private readonly CalendarService Service;
        private readonly string[] Scopes = { CalendarService.Scope.Calendar, CalendarService.Scope.CalendarEvents };
        private readonly string ApplicationName = "CalenderApi";

        public HomeController()
        {
            using (var stream =
                new FileStream("Credential.json", FileMode.Open, FileAccess.Read))
            {
                string CredPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                                Scopes,
                                "user",
                                CancellationToken.None,
                                new FileDataStore(CredPath, true)).Result;
            }
            Service = new (new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName

            });
        }

        public IActionResult Index()
        {
            try
            {
                ViewBag.EventList = CalenderEvents();
                return View();
            }
            catch (Exception e)
            {
                ViewBag.Exception = e.Message;
                return View();
            }

        }
        public IActionResult Delete(string Id)
        {
            try
            {
                var updatedInstance = Service.Events.Delete(CalendarId, Id).Execute();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.Exception = e.Message;
                return View(nameof(Index));
            }

        }
        public IActionResult Create() { return View(); }
        [HttpPost]
        public IActionResult Create(SetCalenderEvent model)
        {
            try
            { 
                Event instance = new()
                {
                    Summary = model.Summary,
                    Description = model.Description,
                    Location = model.Location,
                    Start = new EventDateTime { DateTime = model.Start},
                    End = new EventDateTime { DateTime = model.End }
                };
                var Event = Service.Events.Insert(instance, CalendarId).Execute();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {

                ViewBag.Exception = e.Message;
                return View(nameof(Index));
            }
            
        }
        private List<CalenderModel> CalenderEvents()
        {
            List<CalenderModel> calendars = new();
            EventsResource.ListRequest request = Service.Events.List(CalendarId);
            request.TimeMin= DateTime.Now;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            Events events = Service.Events.List(CalendarId).Execute();
            if (events.Items is not null && events.Items.Count > 0)
            {
                foreach (var item in events.Items)
                {
                    var GoogleEvent = new CalenderModel
                    {
                        Id = item.Id,
                        Summary = item.Summary,
                        Description = item.Description,
                        Location = item.Location,
                        Start = item.Start.DateTime.ToString(),
                        End = item.End.DateTime.ToString()
                    };
                    calendars.Add(GoogleEvent);
                }
                return calendars;
            }
            else
            {
                return null;
            }
        }

    }
}