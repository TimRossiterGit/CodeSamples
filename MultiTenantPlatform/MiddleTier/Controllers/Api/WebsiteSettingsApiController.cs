﻿using Bringpro.Web.Domain;
using Bringpro.Web.Models.Requests;
using Bringpro.Web.Models.Responses;
using Bringpro.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Bringpro.Web.Controllers.Api
{
    [RoutePrefix("api/websitesettings")]
    public class WebsiteSettingsApiController : ApiController
    {

        [Route("get"), HttpGet]
        public HttpResponseMessage List()
        {
            ItemsResponse<WebsiteSettings> response = new ItemsResponse<WebsiteSettings>();
            List<WebsiteSettings> wsList = WebsiteSettingsServices.Get();
            response.Items = WebsiteSettingsServices.Get();
            return Request.CreateResponse(response);
        }

        [Route("add") HttpPost]
        public HttpResponseMessage AddSettings(WebsiteSettingsAddRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ItemResponse<int> response = new ItemResponse<int>();
            response.Item = WebsiteSettingsServices.Insert(model);
            return Request.CreateResponse(response);
        }

        [Route("update/{Id:int}"), HttpPut]
        public HttpResponseMessage UpdateSettings(int Id, WebsiteSettingsUpdateRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<int> response = new ItemResponse<int>();
            response.Item = WebsiteSettingsServices.Update(Id, model);
            return Request.CreateResponse(response);
        }

        [Route("get/{Id:int}"), HttpGet]
        public HttpResponseMessage GetById(int Id)
        {
            ItemResponse<WebsiteSettings> response = new ItemResponse<WebsiteSettings>();
            response.Item = WebsiteSettingsServices.GetById(Id);
            return Request.CreateResponse(response);
        }

        [Route("delete/{Id:int}"), HttpDelete]
        public HttpResponseMessage DeleteSettings(int Id)
        {
            ItemResponse<int> response = new ItemResponse<int>();
            WebsiteSettingsServices.Delete(Id);
            return Request.CreateResponse(response);

        }

        [Route("getByWebId/{Id:int}"), HttpGet]
        public HttpResponseMessage GetByWebId(int Id)
        {
            ItemsResponse<WebsiteSettings> response = new ItemsResponse<WebsiteSettings>();
            response.Items = WebsiteSettingsServices.GetByWebId(Id);
            return Request.CreateResponse(response);
        }

        //Get Website Settings By Slug w/Pagination and Filtering
        [Route("getBySlug/{Slug}"), HttpGet]
        public HttpResponseMessage GetBySlug([FromUri] PaginatedRequest model)
        {
            
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            //Pagination
            PaginatedItemsResponse<WebsiteSettings> response = WebsiteSettingsServices.GetSettingsByQuery(model);
       
            response.CurrentPage = model.CurrentPage;
            response.ItemsPerPage = model.ItemsPerPage;

            return Request.CreateResponse(response);
        }
    }
}
