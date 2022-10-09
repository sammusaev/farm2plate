using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using farm2plate.Areas.Identity.Data;
using farm2plate.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System;
using Amazon.SQS.Model;
using Amazon.SQS;
using System.Text.Json;

namespace farm2plate.Controllers
{
    [Authorize(Roles="Customer")]
    public class CustomerController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        // private ApplicationUser _user;
        private readonly ApplicationDbContext _context;
        private const string queueName = "farm2plateQueue";

        public CustomerController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, ApplicationDbContext context) {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> ViewShop(int? ShopID)
        {
            //System.Diagnostics.Debug.WriteLine($"Shop ID {ShopID}");
            var shop = await _context.Shops.FindAsync(ShopID);
            //System.Diagnostics.Debug.WriteLine($"Just Shop {shop}");
            await _context.Entry(shop).Collection(x => x.Products).LoadAsync();
            //System.Diagnostics.Debug.WriteLine($"Shop Name {shop.Name}");
            ViewBag.BucketName = Environment.GetEnvironmentVariable("AWS_IMAGE_BUCKET_NAME");
            return View(shop);
        }

        public async Task<IActionResult> ViewOrders()
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());

            var productQuantitiesList = new List<double>();
            var orderStatusesList = new List<Status>();
            var orderIdsList = new List<int>();

            System.Diagnostics.Debug.WriteLine($"USER ID {user.Id}");
            await _context.Entry(user).Collection(x => x.SOrders).LoadAsync();

            foreach (var order in user.SOrders)
            {
                productQuantitiesList.Add(order.ProductQuantity);
                orderStatusesList.Add(order.SOrderStatus);
                orderIdsList.Add(order.SOrderID);
            }

            ViewBag.orders = user.SOrders;
            var ordersCount = user.SOrders.Count;
            //System.Diagnostics.Debug.WriteLine($"!!!! SOrders {user.SOrders} SOrders.Count {user.SOrders.Count}");

            var productNamesList = new List<string>();
            var productPricesList = new List<double>();
            var productImagesList = new List<string>();
            var shopNamesList = new List<string>();
            var totalPricesList = new List<double>();

            foreach (var order in user.SOrders) {
                var product = await _context.Products.FindAsync(order.ProductID);
                var shop = await _context.Shops.FindAsync(order.ShopID);
                productNamesList.Add(product.ProductName);
                productImagesList.Add(product.ProductImage);
                productPricesList.Add(product.ProductPrice);
                shopNamesList.Add(shop.ShopName);
                totalPricesList.Add(order.ProductQuantity * product.ProductPrice);
            }

            var productNames = productNamesList.ToArray();
            var productPrices = productPricesList.ToArray();
            var productImages = productImagesList.ToArray();
            var shopNames = shopNamesList.ToArray();
            var totalPrices = totalPricesList.ToArray();
            var productQuantities = productQuantitiesList.ToArray();
            var orderStatuses = orderStatusesList.ToArray();
            var orderIdS = orderIdsList.ToArray();

            ViewBag.productNames = productNames;
            ViewBag.productPrices = productPrices;
            ViewBag.productImages = productImages;
            ViewBag.shopNames = shopNames;
            ViewBag.totalPrices = totalPrices;
            ViewBag.productQuantities = productQuantities;
            ViewBag.orderStatuses = orderStatuses;
            ViewBag.orderIdS = orderIdS;

            ViewBag.BucketName = Environment.GetEnvironmentVariable("AWS_IMAGE_BUCKET_NAME");

            if (ordersCount > 0)
            {
                ViewBag.hasOrders = true;
            } else ViewBag.hasOrders = false;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChangeOrderStatus([Bind("SOrderStatus")] SOrder sorder, int? OrderID, Status SOrderStatus)
        {

            System.Diagnostics.Debug.WriteLine($"!!! sorder {sorder} SOrderStatus {SOrderStatus} OrderID {OrderID}");

            var order = await _context.SOrders.FindAsync(OrderID);
            order.SOrderStatus = SOrderStatus;
            await _context.SaveChangesAsync();

            ViewBag.OrderID = OrderID;
            ViewBag.NewOrderStatus = SOrderStatus;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Order([Bind("ProductQuantity")] SOrder sorder, double? ProductQuantity, int? ProductID, int? ShopID, string ShopName, string ProductImage, string ProductName, double? ProductPrice)
        {

            System.Diagnostics.Debug.WriteLine($"!!! ProductQuantity {ProductQuantity} ProductID {ProductID} ShopID {ShopID} sorderUserID {sorder.UserID}");

            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            var product = await _context.Products.FindAsync(ProductID);
            sorder.ProductID = (int)ProductID;
            sorder.ProductQuantity = (double)ProductQuantity;
            sorder.ShopID = (int)ShopID;
            sorder.UserID = user.Id;
            sorder.SOrderStatus = Status.IN_PROGRESS;

            ViewBag.ShopName = ShopName;
            ViewBag.ShopName = ShopName;
            ViewBag.BucketName = Environment.GetEnvironmentVariable("AWS_IMAGE_BUCKET_NAME");
            ViewBag.ProductImage = ProductImage;
            ViewBag.ProductPrice = ProductPrice;
            ViewBag.TotalPrice = ProductQuantity * ProductPrice;    

            _context.SOrders.Add(sorder);

            product.ProductQuantity = (double)(product.ProductQuantity - ProductQuantity);

             await _context.SaveChangesAsync();

            // SQS

            string AWS_ACCESS_KEY = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY");
            string AWS_SECRET_ACCESS_KEY = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
            string AWS_SESSION_TOKEN = Environment.GetEnvironmentVariable("AWS_SESSION_TOKEN");

            AmazonSQSClient sqsClient = new AmazonSQSClient(AWS_ACCESS_KEY, AWS_SECRET_ACCESS_KEY, AWS_SESSION_TOKEN, Amazon.RegionEndpoint.USEast1);

            try {
                var res = await sqsClient.GetQueueUrlAsync(new GetQueueUrlRequest { QueueName = queueName });
                GetQueueAttributesRequest attReq = new GetQueueAttributesRequest();
                attReq.QueueUrl = res.QueueUrl;
                attReq.AttributeNames.Add("ApproximateNumberOfMessages");
                GetQueueAttributesResponse attRes = await sqsClient.GetQueueAttributesAsync(attReq);
                ViewBag.messageCount = attRes.ApproximateNumberOfMessages;
            } catch (AmazonSQSException ex)
            {
                ViewBag.message = ex.Message;
            }

            return View(sorder);
        }

        // sendmessage and order can be combined (httppost and validateantiforgerytoken tags both apply)

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> sendMessage(string ShopName, int OrderID, Status newOrderStatus)
        {
            string AWS_ACCESS_KEY = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY");
            string AWS_SECRET_ACCESS_KEY = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
            string AWS_SESSION_TOKEN = Environment.GetEnvironmentVariable("AWS_SESSION_TOKEN");

            AmazonSQSClient sqsClient = new AmazonSQSClient(AWS_ACCESS_KEY, AWS_SECRET_ACCESS_KEY, AWS_SESSION_TOKEN, Amazon.RegionEndpoint.USEast1);

            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());

            OrderStatusChange orderStatusChange = new OrderStatusChange
            {
                CustomerID = user.Id,
                CustomerName = user.UserName,
                ShopName = ShopName,
                SOrderID = OrderID,
                OldSOrderStatus = Status.IN_PROGRESS,
                NewSOrderStatus = newOrderStatus,
                SOrderDateTime = DateTime.Now
            };

            try
            {
                SendMessageRequest sendReq = new SendMessageRequest { MessageBody = JsonSerializer.Serialize(orderStatusChange)};
                var res = await sqsClient.GetQueueUrlAsync(new GetQueueUrlRequest { QueueName = queueName });
                sendReq.QueueUrl = res.QueueUrl;
                await sqsClient.SendMessageAsync(sendReq);
            }
            catch (AmazonSQSException ex)
            {
                return RedirectToAction("Index", "SQSExample", new { msg = "Reservation error! Error: " + ex.Message });
            }
            return RedirectToAction("Index", "SQSExample", new { msg = "Reservation done! Our representative will contact you soon!" });
        }

        public async Task<IActionResult> Index()
        {
            var shops = _context.Shops;
            ViewBag.Shops = shops;
            return View();
        }

        /*public LocalRedirectResult LocalRedirect()
        {
            return LocalRedirect("/customer");
        }*/ // same as RedirectCustomerHome

        public RedirectToActionResult RedirectCustomerHome()
        {
            return RedirectToAction("Index", "Customer");
        }

        public LocalRedirectResult RedirectViewOrders()
        {
            return LocalRedirect("/Customer/ViewOrders");
        }
    }
}
