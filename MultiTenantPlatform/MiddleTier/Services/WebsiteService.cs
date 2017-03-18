using Microsoft.Practices.Unity;
using Bringpro.Data;
using Bringpro.Web.Classes.Tasks.Bringg.Interfaces;
using Bringpro.Web.Domain;
using Bringpro.Web.Models.Requests;
using Bringpro.Web.Models.Requests.Bringg;
using Bringpro.Web.Services.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;


namespace Bringpro.Web.Services
{
    public class WebsiteService : BaseService, IWebsiteService
    {

        [Dependency]
        public IAddressService _AddressService { get; set; }
        [Dependency]
        public IEmailCampaignsService _CampaignsService { get; set; }


        public static List<Website> websiteGetAll()
        {

            List<Website> list = null;
            DataProvider.ExecuteCmd(GetConnection, "dbo.Website_SelectAll"
             , inputParamMapper: null
             , map: delegate (IDataReader reader, short set)
             {
                 Website c = new Website();
                 int startingIndex = 0; //startingOrdinal

                 c.Id = reader.GetSafeInt32(startingIndex++);
                 c.Name = reader.GetSafeString(startingIndex++);
                 c.Slug = reader.GetSafeString(startingIndex++);
                 c.Description = reader.GetSafeString(startingIndex++);
                 c.Url = reader.GetSafeString(startingIndex++);
                 c.MediaId = reader.GetSafeInt32(startingIndex++);
                 c.DateCreated = reader.GetSafeDateTime(startingIndex++);
                 c.DateModified = reader.GetSafeDateTime(startingIndex++);
                 c.Phone = reader.GetSafeString(startingIndex++);
                 c.AddressId = reader.GetSafeInt32(startingIndex++);

                 if (list == null)
                 {
                     list = new List<Website>();
                 }
                 list.Add(c);
             }
             );
            return list;
        }

        public async Task<int> websiteInsert(WebsiteAddRequest model)
        {
            // ---- Address Service Call ----
            var AddressResult = _AddressService.SaveWebsiteAddress(model);

            // ---- Create New MailChimp List for THIS website ----
            var MailChimpResult = await _CampaignsService.InsertList(model);

            int id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Website_Insert"
            , inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Name", model.Name);
                paramCollection.AddWithValue("@Slug", model.Slug);
                paramCollection.AddWithValue("@Description", model.Description);
                paramCollection.AddWithValue("@Url", model.Url);
                paramCollection.AddWithValue("@MediaId", model.MediaId);
                paramCollection.AddWithValue("@Phone", model.Phone);
                paramCollection.AddWithValue("@AddressId", AddressResult);
                paramCollection.AddWithValue("@ListId", MailChimpResult);
                paramCollection.AddWithValue("@FromEmail", model.FromEmail);
                paramCollection.AddWithValue("@FromName", model.FromName);
                paramCollection.AddWithValue("@Subject", model.Subject);
                paramCollection.AddWithValue("@PermissionReminder", model.PermissionReminder);

                SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                p.Direction = System.Data.ParameterDirection.Output;

                paramCollection.Add(p);
            },
            returnParameters: delegate (SqlParameterCollection param)
            {
                int.TryParse(param["@Id"].Value.ToString(), out id);
            }
            );

            return id;
        }

        // Get by website id - website, media, address
        public Website websiteGetById(int websiteId)
        {
            Website c = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Website_SelectById"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@Id", websiteId);


              }, map: delegate (IDataReader reader, short set)
              {
                  int startingIndex = 0;

                  c = new Website();

                  c.Id = reader.GetSafeInt32(startingIndex++);
                  c.Name = reader.GetSafeString(startingIndex++);
                  c.Slug = reader.GetSafeString(startingIndex++);
                  c.Description = reader.GetSafeString(startingIndex++);
                  c.Url = reader.GetSafeString(startingIndex++);
                  c.MediaId = reader.GetSafeInt32(startingIndex++);
                  c.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  c.DateModified = reader.GetSafeDateTime(startingIndex++);
                  c.Phone = reader.GetSafeString(startingIndex++);
                  c.AddressId = reader.GetSafeInt32(startingIndex++);
                  c.ListId = reader.GetSafeString(startingIndex++);
                  c.FromEmail = reader.GetSafeString(startingIndex++);
                  c.FromName = reader.GetSafeString(startingIndex++);
                  c.Subject = reader.GetSafeString(startingIndex++);
                  c.PermissionReminder = reader.GetSafeString(startingIndex++);

                  Media m = new Media();

                  m.Id = reader.GetSafeInt32(startingIndex++);
                  m.DateModified = reader.GetSafeDateTime(startingIndex++);
                  m.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  m.Url = reader.GetSafeString(startingIndex++);
                  m.MediaType = reader.GetSafeInt32(startingIndex++);
                  m.UserId = reader.GetSafeString(startingIndex++);
                  m.Title = reader.GetSafeString(startingIndex++);
                  m.Description = reader.GetSafeString(startingIndex++);
                  m.ExternalMediaId = reader.GetSafeInt32(startingIndex++);
                  m.FileType = reader.GetSafeString(startingIndex++);

                  if (m.Id != 0)
                  {
                      c.Media = m;
                  }

                  Bringpro.Web.Domain.Address a = new Bringpro.Web.Domain.Address();

                  a.AddressId = reader.GetSafeInt32(startingIndex++);
                  a.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  a.DateModified = reader.GetSafeDateTime(startingIndex++);
                  a.UserId = reader.GetSafeString(startingIndex++);
                  a.Name = reader.GetSafeString(startingIndex++);
                  a.ExternalPlaceId = reader.GetSafeString(startingIndex++);
                  a.Line1 = reader.GetSafeString(startingIndex++);
                  a.Line2 = reader.GetSafeString(startingIndex++);
                  a.City = reader.GetSafeString(startingIndex++);
                  a.State = reader.GetSafeString(startingIndex++);
                  a.StateId = reader.GetSafeInt32(startingIndex++);
                  a.ZipCode = reader.GetSafeInt32(startingIndex++);
                  a.Country = reader.GetSafeString(startingIndex++);
                  a.Latitude = reader.GetSafeDecimal(startingIndex++);
                  a.Longitude = reader.GetSafeDecimal(startingIndex++);

                  if (a.AddressId != 0)
                  {
                      c.Address = a;
                  }

              }

           );
            return c;
        }

        public static Website websiteGetBySlug(string Slug)
        {
            Website c = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Website_GetAllBySlug"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@Slug", Slug);

              }, map: delegate (IDataReader reader, short set)
              {
                  int startingIndex = 0;
                  c = new Website();

                  c.Id = reader.GetSafeInt32(startingIndex++);
                  c.Name = reader.GetSafeString(startingIndex++);
                  c.Slug = reader.GetSafeString(startingIndex++);
                  c.Description = reader.GetSafeString(startingIndex++);
                  c.Url = reader.GetSafeString(startingIndex++);
                  c.MediaId = reader.GetSafeInt32(startingIndex++);
                  c.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  c.DateModified = reader.GetSafeDateTime(startingIndex++);
                  c.Phone = reader.GetSafeString(startingIndex++);
                  c.AddressId = reader.GetSafeInt32(startingIndex++);

                  Media m = new Media();

                  m.Id = reader.GetSafeInt32(startingIndex++);
                  m.DateModified = reader.GetSafeDateTime(startingIndex++);
                  m.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  m.Url = reader.GetSafeString(startingIndex++);
                  m.MediaType = reader.GetSafeInt32(startingIndex++);
                  m.UserId = reader.GetSafeString(startingIndex++);
                  m.Title = reader.GetSafeString(startingIndex++);
                  m.Description = reader.GetSafeString(startingIndex++);
                  m.ExternalMediaId = reader.GetSafeInt32(startingIndex++);
                  m.FileType = reader.GetSafeString(startingIndex++);

                  if (m.Id != 0)
                  {
                      c.Media = m;
                  }

                  Bringpro.Web.Domain.Address a = new Bringpro.Web.Domain.Address();

                  a.AddressId = reader.GetSafeInt32(startingIndex++);
                  a.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  a.DateModified = reader.GetSafeDateTime(startingIndex++);
                  a.UserId = reader.GetSafeString(startingIndex++);
                  a.Name = reader.GetSafeString(startingIndex++);
                  a.ExternalPlaceId = reader.GetSafeString(startingIndex++);
                  a.Line1 = reader.GetSafeString(startingIndex++);
                  a.Line2 = reader.GetSafeString(startingIndex++);
                  a.City = reader.GetSafeString(startingIndex++);
                  a.StateId = reader.GetSafeInt32(startingIndex++);
                  a.ZipCode = reader.GetSafeInt32(startingIndex++);
                  a.Latitude = reader.GetSafeDecimal(startingIndex++);
                  a.Longitude = reader.GetSafeDecimal(startingIndex++);

                  if (a.AddressId != 0)
                  {
                      c.Address = a;
                  }
              }
              );
            return c;
        }

        public async Task<bool> websiteUpdate(WebsiteUpdateRequest model)
        {
            // ---- Address Update Service Call ----
            var AddressResult = _AddressService.SaveWebsiteAddress(model);

            // ---- MailChimp List Update Service Call ----
            var MailChimpResult = await _CampaignsService.InsertList(model);

            bool success = false;
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Website_Update"
                      , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                      {
                          paramCollection.AddWithValue("@Id", model.Id);
                          paramCollection.AddWithValue("@Name", model.Name);
                          paramCollection.AddWithValue("@Slug", model.Slug);
                          paramCollection.AddWithValue("@Description", model.Description);
                          paramCollection.AddWithValue("@Url", model.Url);
                          paramCollection.AddWithValue("@MediaId", model.MediaId);
                          paramCollection.AddWithValue("@Phone", model.Phone);
                          paramCollection.AddWithValue("@AddressId", model.AddressId);
                          paramCollection.AddWithValue("@ListId", MailChimpResult);
                          paramCollection.AddWithValue("@FromEmail", model.FromEmail);
                          paramCollection.AddWithValue("@FromName", model.FromName);
                          paramCollection.AddWithValue("@Subject", model.Subject);
                          paramCollection.AddWithValue("@PermissionReminder", model.PermissionReminder);

                      }, returnParameters: delegate (SqlParameterCollection param)
                      {
                          success = true;
                      });

            return success;
        }

        public static void websiteDelete(int id)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Website_Delete"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Id", id);

                });
        }

        //WebsiteUser Table services
        //add a websiteId and UserId to one another

        public static void AddUserToWebsite(UserWebsite model)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.UserWebsite_Insert"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@UserId", model.UserId);
                    //paramCollection.AddWithValue("@WebsitesId", websiteId);

                    SqlParameter s = new SqlParameter("@WebsiteIds", SqlDbType.Structured);
                    if (model.WebsiteIds != null && model.WebsiteIds.Any())
                    {
                        s.Value = new IntIdTable(model.WebsiteIds);
                    }
                    paramCollection.Add(s);
                });
        }
        //End of WebsiteUserServices

        public static Website GetWebsiteByUrl(string Url)
        {
            Website w = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Website_GetByUrl"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@Url", Url);

              }, map: delegate (IDataReader reader, short set)
              {
                  int startingIndex = 0;
                  w = new Website();

                  w.Id = reader.GetSafeInt32(startingIndex++);
                  w.Name = reader.GetSafeString(startingIndex++);
                  w.Slug = reader.GetSafeString(startingIndex++);
                  w.Description = reader.GetSafeString(startingIndex++);
                  w.Url = reader.GetSafeString(startingIndex++);
                  w.MediaId = reader.GetSafeInt32(startingIndex++);
                  w.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  w.DateModified = reader.GetSafeDateTime(startingIndex++);
                  w.Phone = reader.GetSafeString(startingIndex++);
                  w.AddressId = reader.GetSafeInt32(startingIndex++);

              }

              );

            return w;

        }

        public static Website GetWebsiteIdBySlug(string Slug)
        {
            Website w = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Website_GetIdBySlug"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@Slug", Slug);

              }, map: delegate (IDataReader reader, short set)
              {
                  int startingIndex = 0;
                  w = new Website();

                  w.Id = reader.GetSafeInt32(startingIndex++);

              }

              );

            return w;
        }

        public List<int> GetWebsiteIdByUserId(string UserId)
        {
            List<int> WebsiteIds = new List<int>();
            DataProvider.ExecuteCmd(GetConnection, "dbo.UserWebsite_SelectByUserId"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@UserId", UserId);
                }, map: delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;
                    int WebsiteId;

                    WebsiteId = reader.GetSafeInt32(startingIndex++);

                    WebsiteIds.Add(WebsiteId);
                }
           );
            return WebsiteIds;
        }

    }
}