using Azure;
using Core.Enums;
using Core.Services;
using Core.Services.Interfaces;
using Core.ViewModel;
using Datalayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Numerics;
using Zarinpal.AspNetCore.DTOs;
using Zarinpal.AspNetCore.Extensions;
using Zarinpal.AspNetCore.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace dastgire.Controllers
{
    public class ShopController : Controller
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;
        private readonly ICommentService _commentService;
        private readonly IZarinpalService _zarinpalService;
        private readonly IMemoryCache _cache;

        public ShopController(IProductService productService, IUserService userService, ICategoryService categoryService, ICommentService commentService, IZarinpalService zarinpalService, IMemoryCache cache)
        {
            _productService = productService;
            _userService = userService;
            _categoryService = categoryService;
            _commentService = commentService;
            _zarinpalService = zarinpalService;
            _cache = cache;
        }

        [Route("Product/{Id}")]
        public IActionResult Product(string Id)
        {
            return View(_productService.GetProduct(Id));
        }

        public IActionResult GetVariable(int varId)
        {
            var variable = _productService.GetSelectedVariable(varId);
            if (variable != null)
            {
                var variableVM = new PriceShowerViewModel
                {
                    Variable = variable,
                    CurrentNumber = 1,
                };
                return PartialView("_PriceShower", variableVM);
            }
            else
            {
                return Content("false");
            }

        }

        public IActionResult ChangeNumber(int selectedVariableId, int value)
        {
            var variable = _productService.GetSelectedVariable(selectedVariableId);
            if (variable != null)
            {
                if (value > variable.NumberInStock)
                {
                    value = variable.NumberInStock.Value;
                }
                else if (value < 1)
                {
                    value = 1;
                }
                var variableVM = new PriceShowerViewModel
                {
                    Variable = variable,
                    CurrentNumber = value,
                };
                return PartialView("_PriceShower", variableVM);
            }
            else
            {
                return Content("false");
            }
        }

        public IActionResult AddVariableProductToOrder(int productId, int varId, int number, int currentPrice)
        {
            var OpenOrder = _userService.IsOpenOrder(User.Identity.Name);
            Variable? variable = _productService.GetSelectedVariable(varId);
            Product product = _productService.GetProduct(productId);

            if (product.IsSimple)
            {
                if (product.FinalPrice == currentPrice && product.BaseStockNumber > 0)
                {
                    if (OpenOrder > 0)
                    {
                        var isExistProductInOrder = _productService.isVarProductExistInOrder(product, varId, OpenOrder);
                        if (!isExistProductInOrder)
                        {
                            _productService.AddVariableProductToOrder(productId, varId, OpenOrder, number);
                        }
                    }
                    else
                    {
                        var newOrder = _userService.AddNewOrderToUser(User.Identity.Name);
                        _productService.AddVariableProductToOrder(productId, varId, newOrder, number);
                    }
                    var variableVM = new PriceShowerViewModel
                    {
                        Product = product,
                        Variable = variable,
                        CurrentNumber = 1,
                    };
                    return PartialView("_PriceShower", variableVM);
                }
                else
                {
                    return Content("NotValid");
                }

            }
            else
            {
                if (variable != null)
                {
                    if (variable.FinalPrice == currentPrice && variable.NumberInStock > 0)
                    {
                        if (OpenOrder > 0)
                        {
                            var isExistProductInOrder = _productService.isVarProductExistInOrder(product, varId, OpenOrder);
                            if (!isExistProductInOrder)
                            {
                                _productService.AddVariableProductToOrder(productId, varId, OpenOrder, number);
                            }
                        }
                        else
                        {
                            var newOrder = _userService.AddNewOrderToUser(User.Identity.Name);
                            _productService.AddVariableProductToOrder(productId, varId, newOrder, number);
                        }
                        var variableVM = new PriceShowerViewModel
                        {
                            Product = product,
                            Variable = variable,
                            CurrentNumber = 1,
                        };
                        return PartialView("_PriceShower", variableVM);
                    }
                    else
                    {
                        return Content("NotValid");
                    }

                }
                else
                {
                    return Content("NotValid");
                }
            }

        }

        public IActionResult DeleteProductFromCart(int productId, int varId)
        {
            var OpenOrder = _userService.IsOpenOrder(User.Identity.Name);
            var Product = _productService.GetProduct(productId);


            if (Product.IsSimple)
            {
                var orderProduct = _productService.GetProductsInOrder(OpenOrder).Where(x => x.ProductId == productId).FirstOrDefault();

                _productService.DeleteProductFromOrder(orderProduct.Id);

                var variableVM = new PriceShowerViewModel
                {
                    Variable = null,
                    Product = Product,
                    CurrentNumber = 1,
                };
                return PartialView("_PriceShower", variableVM);
            }
            else
            {
                var variable = _productService.GetSelectedVariable(varId);
                if (variable != null)
                {
                    var orderProduct = _productService.GetProductsInOrder(OpenOrder).Where(x => x.VariableId == varId).FirstOrDefault();

                    _productService.DeleteProductFromOrder(orderProduct.Id);


                    var variableVM = new PriceShowerViewModel
                    {
                        Variable = variable,
                        Product = Product,
                        CurrentNumber = 1,
                    };
                    return PartialView("_PriceShower", variableVM);
                }
                else
                {
                    return Content("false");
                }
            }

        }

        public IActionResult insertComment(int productId, string message, string fullName, string email)
        {
            if (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(fullName) && !string.IsNullOrEmpty(email))
            {
                _commentService.insertComment(fullName, email, message, productId);
            }

            var comments = _commentService.GetAllComments(productId);

            return PartialView("_commentList", comments);

        }

        [Route("/Category/{slug}/{Page}")]
        [Route("/Category/{slug}")]
        public IActionResult Category(string slug, int Page = 1)
        {
            var category = _categoryService.GetCategory(slug);
            var products = _categoryService.GetAllProdcutInCategory(category.Id);
            var numberItemInPage = 12;
            ProductsCategoryViewModel ProductsViewModel = new ProductsCategoryViewModel()
            {
                Products = _categoryService.GetSpecificProductsInCategory(category.Id, Page, numberItemInPage),
                CurrentPage = Page,
                CountPerPage = numberItemInPage,
                AllPage = (products.Count() % numberItemInPage == 0) ? (products.Count() / numberItemInPage) : (products.Count() / numberItemInPage) + 1,
                Cat = category,
            };

            return View(ProductsViewModel);
        }

        public int UpdateNumberOrderCard()
        {
            int numberProductInCard = 0;
            if (User.Identity.IsAuthenticated)
            {
                int orderId = _userService.IsOpenOrder(User.Identity.Name);
                if (orderId > 0)
                {
                    numberProductInCard = _productService.GetTotalNumberInOrder(orderId);
                }
            }

            return numberProductInCard;
        }

        public CartViewModel GetCartVM()
        {
            CartViewModel CartVM = new CartViewModel();

            if (!User.Identity.IsAuthenticated)
            {
                CartVM.CartStatus = CartStatus.NotLoggedIn;
                return CartVM;
            }


            var openPackage = _userService.IsOpenOrder(User.Identity.Name);
            if (openPackage < 0)
            {
                CartVM.CartStatus = CartStatus.NotOpenOrder;
                return CartVM;
            }

            var allProductOrders = _productService.GetProductsInOrder(openPackage);

            if (allProductOrders.Count() == 0)
            {
                _productService.DeleteOrder(openPackage);
                CartVM.CartStatus = CartStatus.NotOpenOrder;
                return CartVM;
            }


            var order = _productService.GetOrderById(openPackage);
            CartVM.Order = order;
            CartVM.CartStatus = CartStatus.OK;

            if (order.CouponId != null)
            {
                var result = _productService.VerifyCouponInCart(order);
                if (result != "OK")
                {
                    CartVM.ErrorMessages.Add(result);
                }
            }

            var deletedVariables = allProductOrders.Where(o => o.IsInvalid);

            foreach (var deletedVar in deletedVariables)
            {
                CartVM.ErrorMessages.Add("محصول " + deletedVar.Product.Title + " مدل " + deletedVar.VariableTitle + " ناموجود شده است، این محصول از سبد شما حذف شد");
                _productService.DeleteProductFromOrder(deletedVar.Id);
            }

            foreach (var orderProduct in allProductOrders.Where(o => !o.IsInvalid))
            {
                var result = _productService.VerifyProductCountInCart(orderProduct);
                if (result != "1")
                {
                    CartVM.ErrorMessages.Add(result);
                }
            }


            return CartVM;
        }


        [Route("/Cart")]
        public IActionResult Cart()
        {
            var userName = User.Identity.Name;
            string cacheKey = $"CartVM_{userName}";

            CartViewModel? CartVM = null;

            if (_cache.TryGetValue(cacheKey, out CartVM))
            {
                _cache.Remove(cacheKey);
            }
            else
            {
                CartVM = GetCartVM();
            }

            ViewBag.NOK = TempData["NOK"];

            return View(CartVM);

        }

        [Route("/Checkout")]
        public IActionResult Checkout()
        {
            var userName = User.Identity.Name;
            CartViewModel CartVM = GetCartVM();
            if (CartVM.ErrorMessages.Count > 0)
            {
                string cacheKey = $"CartVM_{userName}";
                _cache.Set(cacheKey, CartVM, TimeSpan.FromMinutes(5));

                return RedirectToAction("Cart");
            }
            else
            {
                return View(CartVM.Order);
            }
        }


        [HttpPost]
        [Route("/Checkout")]
        public async Task<IActionResult> Checkout(string UserDeliverName, string UserDeliverKodeMelli, string UserPhoneNumber, string UserHomePhone, int City, string Address, string PostalCode, int? ShippMethod, string submittype, string OtherUserMobile, string Description)
        {
            bool IsHaveError = false;

            var openPackage = _userService.IsOpenOrder(User.Identity.Name);
            var order = _productService.GetOrderById(openPackage);

            if (string.IsNullOrEmpty(UserDeliverName) || string.IsNullOrEmpty(UserPhoneNumber) || string.IsNullOrEmpty(Address) || string.IsNullOrEmpty(PostalCode) || City == 0)
            {
                ModelState.AddModelError("", "پر کردن تمامی فیلدها اجباری می باشد");
                IsHaveError = true;
            }

            if (ShippMethod == null)
            {
                ModelState.AddModelError("", "لطفا روش ارسال رو مشخص کنید");
                IsHaveError = true;
            }

            if (submittype == "SubmitForUser" && !string.IsNullOrEmpty(OtherUserMobile))
            {
                var otherUser = _userService.isExistUser(OtherUserMobile);
                if (otherUser == null)
                {
                    ModelState.AddModelError("", "کاربری با این شماره موبایل پیدا نشد");
                    IsHaveError = true;
                }
            }




            var userName = User.Identity.Name;
            CartViewModel CartVM = GetCartVM();

            if (CartVM.ErrorMessages.Count > 0)
            {
                string cacheKey = $"CartVM_{userName}";
                _cache.Set(cacheKey, CartVM, TimeSpan.FromMinutes(5));

                return RedirectToAction("Cart");
            }
            else
            {
                if (!IsHaveError)
                {

                    _productService.SetCheckout(order, User.Identity.Name, UserDeliverName, UserDeliverKodeMelli, UserPhoneNumber, UserHomePhone, City, Address, PostalCode, Description);
                    _productService.PrepareOrderBeforePayment(order);

                    if (submittype == "Submit")
                    {
                        return await RequestPayment(order);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(OtherUserMobile))
                        {
                            var otherUser = _userService.isExistUser(OtherUserMobile);
                            order.UserId = otherUser.Id;
                        }

                        _productService.CloseOrder(order, "سفارش توسط مدیریت");

                        order.Status = "NoPayment";
                        _productService.SaveDatabse();

                        return View("ReportPayment", order);
                    }

                }

                ViewBag.UserDeliverName = UserDeliverName;
                ViewBag.UserDeliverKodeMelli = UserDeliverKodeMelli;
                ViewBag.UserPhoneNumber = UserPhoneNumber;
                ViewBag.UserHomePhone = UserHomePhone;
                ViewBag.Address = Address;
                ViewBag.City = City;
                ViewBag.PostalCode = PostalCode;
                ViewBag.Description = Description;
                return View(CartVM.Order);
            }


           
        }


        public IActionResult DeleteProductFromOrder(int OrderProductId)
        {
            _productService.DeleteProductFromOrder(OrderProductId);
            return RedirectToAction("Cart");
        }

        public IActionResult ChangeProductNumberInCart(int OrderProductId, int number)
        {
            var openPackage = _productService.ChangeProductNumberInCart(OrderProductId, number);


            return PartialView("_CartItem", openPackage);
        }

        public IActionResult RefreshShopCartWidget(int OrderId)
        {
            var currentOrder = _productService.GetOrderById(OrderId);

            return PartialView("_ShopCartDetails", currentOrder);
        }



        public IActionResult LoadCityForState(int stateId)
        {
            var cityViewModel = new CityViewModel()
            {
                Cities = _productService.getCityOfState(stateId),
            };

            return PartialView("_CitySelect", cityViewModel);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestPayment(Order order)
        {
            int price = _productService.GetTotalPriceInOrder(order.Id);
            int offValue = 0;
            if (order.CouponId != null)
            {
                offValue = _productService.CalculateOffValue(order, price);
            }
            price = price - offValue;
            price = price * 10; //rial
            //localhost:7040
            //nishcoffee.ir

            var request = new ZarinpalRequestDTO(price, "خرید از فروشگاه [نام فروشگاه]",
            "https://localhost:7040/Shop/VerifyPayment/" + order.Id,
    "test@test.com", User.Identity.Name);

            var result = await _zarinpalService.RequestAsync(request);

            if (result.Data != null)
            {
                // You can store or log zarinpal data in database
                string authority = result.Data.Authority;
                int code = result.Data.Code;
                int fee = result.Data.Fee;
                string feeType = result.Data.FeeType;
                string message = result.Data.Message;
            }

            if (result.IsSuccessStatusCode)
                return Redirect(result.RedirectUrl);

            //var usedPackage = _packageService.GetUserPackage(UserPackageId);
            //_packageService.SavePayDetails(usedPackage,result.StatusCode.Value.GetStatusCodeMessage(), result.StatusCode.ToString());
            // if you want see status message
            //var message1 = result.StatusCode.Value.GetStatusCodeMessage();
            // Do Something


            TempData["NOK"] = "متاسفانه پرداخت انجام نشد";
            return RedirectToAction("Cart");
        }

        [HttpGet]
        public async Task<IActionResult> VerifyPayment(int id)
        {

            var order = _productService.GetOrderById(id);
            if (order.IsFinal)
            {
                return RedirectToAction("MyOrder", "Home");
            }
            // Check 'Status' and 'Authority' query param so zarinpal sent for us
            if (HttpContext.IsValidZarinpalVerifyQueries())
            {
                // If store your price in toman you can use TomanToRial extension
                int price = _productService.GetTotalPriceWhenVerify(id);
                price = price - order.CouponOffValue;
                price = price * 10; // rial

                var verify = new ZarinpalVerifyDTO(price,
                    HttpContext.GetZarinpalAuthorityQuery());

                var response = await _zarinpalService.VerifyAsync(verify);

                if (response.Data != null)
                {
                    // You can store or log zarinpal data in database
                    ulong refId = response.Data.RefId;
                    int fee = response.Data.Fee;
                    string feeType = response.Data.FeeType;
                    int code = response.Data.Code;
                    string cardHash = response.Data.CardHash;
                    string cardPan = response.Data.CardPan;
                }

                if (response.IsSuccessStatusCode)
                {
                    _productService.CloseOrder(order, response.RefId.ToString());

                    return View("ReportPayment", order);
                }
            }

            _productService.RefundOrderAfterFailedPayment(order);
            TempData["NOK"] = "متاسفانه پرداخت انجام نشد";
            return RedirectToAction("Cart");
        }

        public void SetShippingMethod(int OrderId, int shipMethodId)
        {
            var order = _productService.GetOrderById(OrderId);
            _productService.SetShippingPrice(order, shipMethodId);
        }

        public IActionResult SubmitCoupon(int OrderId, string CouponCode)
        {
            if (string.IsNullOrEmpty(CouponCode))
            {
                ViewBag.error = "true";
                ViewBag.message = "لطفا یک کد تخفیف وارد کنید";
                return PartialView("_couponForm", OrderId);
            }
            else
            {
                var Coupon = _productService.IsExistCoupon(CouponCode.Trim());
                if (Coupon == null)
                {
                    ViewBag.error = "true";
                    ViewBag.message = "کد تخفیف معتبر نیست";
                    return PartialView("_couponForm", OrderId);
                }
                else
                {
                    var user = _userService.isExistUser(User.Identity.Name);
                    var IsUserUsedCoupon = _productService.IsUserUsedCoupon(user.Id, Coupon.Id);

                    if (IsUserUsedCoupon && Coupon.AnyUserOneTime == true)
                    {
                        ViewBag.error = "true";
                        ViewBag.message = "از این کد تخفیف فقط یک بار میتوانید استفاده کنید";
                        return PartialView("_couponForm", OrderId);
                    }
                    else
                    {
                        if (Coupon.OnlyFirstOrder)
                        {
                            bool isHaveFinalOrder = _userService.IsUserHaveFinalOrder(user.Id);
                            if (isHaveFinalOrder)
                            {
                                ViewBag.error = "true";
                                ViewBag.message = "از این کد تخفیف فقط برای سفارش اول می توانید استفاده کنید";
                                return PartialView("_couponForm", OrderId);
                            }
                        }
                        if (Coupon.StartDate.HasValue)
                        {
                            PersianCalendar pc = new PersianCalendar();
                            var savedDate = Coupon.StartDate.Value;
                            DateTime dt = new DateTime(savedDate.Year, savedDate.Month, savedDate.Day, savedDate.Hour, savedDate.Minute, savedDate.Second, pc);

                            if (DateTime.Now < dt)
                            {
                                ViewBag.error = "true";
                                ViewBag.message = "این کد تخفیف فعال نیست";
                                return PartialView("_couponForm", OrderId);
                            }
                        }

                        if (Coupon.EndDate.HasValue)
                        {
                            PersianCalendar pc = new PersianCalendar();
                            var savedDate = Coupon.EndDate.Value;
                            DateTime dt = new DateTime(savedDate.Year, savedDate.Month, savedDate.Day, savedDate.Hour, savedDate.Minute, savedDate.Second, pc);

                            if (DateTime.Now > dt)
                            {
                                ViewBag.error = "true";
                                ViewBag.message = "کد تخفیف منقضی شده است";
                                return PartialView("_couponForm", OrderId);
                            }
                        }

                        if (Coupon.MinTotalOrder.HasValue)
                        {
                            if (_productService.GetTotalPriceInOrder(OrderId) < Coupon.MinTotalOrder.Value)
                            {
                                ViewBag.error = "true";
                                ViewBag.message = "برای استفاده از این کد تخفیف مجموع سبد خرید شما باید بالای " + Coupon.MinTotalOrder.Value.ToString("#,0") + " تومان باشد";
                                return PartialView("_couponForm", OrderId);
                            }
                        }
                    }
                }

                var order = _productService.GetOrderById(OrderId);

                order.CouponId = Coupon.Id;

                _productService.SaveDatabse();

                ViewBag.error = "false";
                ViewBag.message = "کد تخفیف اعمال شد :)";
                return PartialView("_couponForm", OrderId);
            }


        }

        public IActionResult Search(string search, int page = 1)
        {
            var products = _productService.GetAllPublishedProduct(search);
            var numberItemInPage = 12;
            ProductsSearchViewModel ProductsViewModel = new ProductsSearchViewModel()
            {
                Products = products.Skip(numberItemInPage * (page - 1)).Take(numberItemInPage).ToList(),
                CurrentPage = page,
                CountPerPage = numberItemInPage,
                AllPage = (products.Count() % numberItemInPage == 0) ? (products.Count() / numberItemInPage) : (products.Count() / numberItemInPage) + 1,
                SearchText = search,
            };

            return View(ProductsViewModel);
        }

        public IActionResult RefreshShippingMethodes(int totalPrice, int cityId)
        {
            ShippingMethodesViewModel ShVM = new ShippingMethodesViewModel()
            {
                CityId = cityId,
                TotalPrice = totalPrice,
            };

            return PartialView("_ShippingMethode", ShVM);
        }

        [Route("WonderProducts")]
        public IActionResult WonderProducts()
        {
            var Products = _productService.GetWonderProducts();
            return View(Products);
        }

        public IActionResult OpenSubCatMenu(int CatId)
        {
            return PartialView("_subCatMenu", CatId);
        }

        public IActionResult SelectVariable(int produtid, List<int> selectedAttributeValueId)
        {
            var Product = _productService.GetProduct(produtid);
            var AllVariables = _productService.GetProductVariableAttributes(produtid);

            //var filteredVariables = AllVariables.Where(p => p.ProductId == produtid && selectedAttributeValueId.Contains(p.ProductAttributeValueId.Value)).Select(x => x.VariableId);
            var SelectedVariableId = AllVariables
                .Where(p => selectedAttributeValueId.Contains(p.ProductAttributeValueId.Value))
                .GroupBy(p => p.VariableId)
                .Where(g => g.Count() == selectedAttributeValueId.Count)
                .Select(g => g.Key)
                .FirstOrDefault();

            var AllVariations = _productService.GetAllProductVariations(produtid);
            var selectedVariations = AllVariations.Where(v => v.VariableId == SelectedVariableId).FirstOrDefault();

            return PartialView("_VariablesShower", new VariablesShowerViewModel { AllVariations = AllVariations, SelectedVariable = selectedVariations, Product = Product });
        }

        public IActionResult RefreshPriceShower(int produtid, int selectedVariable)
        {
            var variableVM = new PriceShowerViewModel
            {
                Variable = _productService.GetSelectedVariable(selectedVariable),
                Product = _productService.GetProduct(produtid),
                CurrentNumber = 1,
            };
            return PartialView("_PriceShower", variableVM);
        }
    }
}
