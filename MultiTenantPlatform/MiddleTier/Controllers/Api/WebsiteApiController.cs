using Microsoft.Practices.Unity;
using Bringpro.Web.Domain;
using Bringpro.Web.Domain.Tests;
using Bringpro.Web.Models.Requests;
using Bringpro.Web.Models.Responses;
using Bringpro.Web.Services;
using Bringpro.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bringpro.Web.Controllers.Api
{
    [RoutePrefix("api/website")]
    public class WebsiteAPIController : ApiController
    {
        [Dependency]
        public IWebsiteService _WebsiteService { get; set; } //Dependency Injection for Website Service

        [Route(), HttpGet]
        public HttpResponseMessage List()
        {
            ItemsResponse<Website> response = new ItemsResponse<Website>();
            response.Items = WebsiteService.websiteGetAll();
            return Request.CreateResponse(response);
        }

        [Route(), HttpPost]
        public async Task<HttpResponseMessage> Add(WebsiteAddRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<int> response = new ItemResponse<int>();
            response.Item = await _WebsiteService.websiteInsert(model);
            return Request.CreateResponse(response);
        }

        [Route("{id:int}"), HttpGet]
        public HttpResponseMessage GetById(int id)
        {
            ItemResponse<Website> response = new ItemResponse<Website>();
            response.Item = _WebsiteService.websiteGetById(id);
            return Request.CreateResponse(response);
        }

        [Route("{id:int}"), HttpPut]
        public async Task<HttpResponseMessage> Edit(WebsiteUpdateRequest model, int id)
        {
            // - Validate incoming payload model
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            model.Id = id;

            bool isSuccessful = await _WebsiteService.websiteUpdate(model);

            // - Instantiate an ItemResponse Model to send back to the browser
            ItemResponse<bool> response = new ItemResponse<bool>();

            // - Load the response Item with the success boolean (true)
            response.Item = isSuccessful;

            return Request.CreateResponse(response);

        }
        [Route("{id:int}"), HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            SuccessResponse response = new SuccessResponse();
            WebsiteService.websiteDelete(id);
            return Request.CreateResponse(response);
        }

        //get website by slug
        [Route("{Slug}"), HttpGet]
        public HttpResponseMessage GetSettingsBySlug(string Slug)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemsResponse<WebsiteSettings> response = new ItemsResponse<WebsiteSettings>();

            WebsiteSettingsServices websiteService = new WebsiteSettingsServices();
            
            //generate a list of website settings
            List<WebsiteSettings> WebsiteBySlug = websiteService.GetSettingsBySlug(Slug);

            response.Items = WebsiteBySlug;

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [Route("selected/{Slug}"), HttpGet]
        public HttpResponseMessage GetWebsiteBySlug(string Slug)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<Website> response = new ItemResponse<Website>();
            response.Item = WebsiteService.websiteGetBySlug(Slug);
            return Request.CreateResponse(response);


        }
    }
}