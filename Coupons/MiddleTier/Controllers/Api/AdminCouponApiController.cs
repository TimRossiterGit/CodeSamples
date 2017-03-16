using Microsoft.Practices.Unity;
using Sabio.Web.Domain;
using Sabio.Web.Models;
using Sabio.Web.Models.Requests;
using Sabio.Web.Models.Responses;
using Sabio.Web.Services;
using Sabio.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sabio.Web.Controllers.Api
{
    [RoutePrefix("api/coupons")]
    [Authorize(Roles = "Administrator, CustomerService")]

    public class AdminCouponApiController : ApiController
    {
        [Dependency]
        public ICouponService _CouponService { get; set; }
        [Route(), HttpPost]
        public HttpResponseMessage Coupons(CouponInsertRequest model)
        {

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<int> couponResponse = new ItemResponse<int>();

            couponResponse.Item = _CouponService.InsertCoupon(model);

            return Request.CreateResponse(couponResponse);
        }

        [Route(), HttpGet]
        public HttpResponseMessage GetAllCopons()
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemsResponse<CouponsDomain> response = new ItemsResponse<CouponsDomain>();

            //CouponService couponService = new CouponService();

            List<CouponsDomain> CouponList = _CouponService.GetAllCoupons();

            response.Items = CouponList;

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [Route("{id:int}"), HttpDelete]
        public HttpResponseMessage CouponDelete(int Id)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            CouponDeleteRequest model = new CouponDeleteRequest();

            model.Id = Id;

            //CouponService couponService = new CouponService();

            bool isSuccessful = _CouponService.DeleteCoupon(model);

            ItemResponse<bool> response = new ItemResponse<bool>();

            response.Item = isSuccessful;

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        //get by coupon Id
        [Route("{Id:int}"), HttpGet]
        public HttpResponseMessage GetCouponById(int Id)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<CouponsDomain> response = new ItemResponse<CouponsDomain>();

            //CouponService couponService = new CouponService();

            CouponsDomain CouponList = _CouponService.GetCouponById(Id);

            response.Item = CouponList;

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [Route("{id:int}"), HttpPut]
        public HttpResponseMessage CouponEdit(CouponsUpdateRequest model, int id)
        {

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            model.Id = id;

            //CouponService couponService = new CouponService();

            bool isSuccessful = _CouponService.UpdateCoupon(model);

            ItemResponse<bool> response = new ItemResponse<bool>();

            response.Item = isSuccessful;

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        //get by website Id
        [Route("websiteid/{WebsiteId:int}"), HttpGet]
        public HttpResponseMessage GetAllCouponsByWebsiteId(int WebsiteId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemsResponse<CouponsDomain> response = new ItemsResponse<CouponsDomain>();

            //CouponService couponService = new CouponService();

            List<CouponsDomain> CouponsListByWebsiteId = _CouponService.GetAllCouponsByWebsiteId(WebsiteId);

            response.Items = CouponsListByWebsiteId;

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }
    }
}
