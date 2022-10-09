using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using farm2plate.Models;

namespace farm2plate.Controllers
{
    public class ChangeOrderStatusSQSController : Controller
    {

        private const string queueName = "farm2plateQueue";

        public IActionResult Index()
        {
            return View();
        }
    }
}
