using Bringpro.Data;
using Bringpro.Web.Domain;
using Bringpro.Web.Enums;
using Bringpro.Web.Models.Requests;
using Bringpro.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Bringpro.Web.Services
{
    public class WebsiteSettingsServices : BaseService // extending from base service class
    {

        // get website settings by website slug
        // main service used for multi-tenant platform
        public List<WebsiteSettings> GetSettingsBySlug(string Slug)
        {
            List<WebsiteSettings> list = null; //create empty list of website settings

            DataProvider.ExecuteCmd(GetConnection, "dbo.WebsiteSettings_GetByWebsiteSlug" // connection to sql stored proc
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@Slug", Slug); // input the slug into sql stored proc

                  //the return from sql
              }, map: delegate (IDataReader reader, short set)
              {
                  int startingIndex = 0; // starting ordinal

                  WebsiteSettings ws = new WebsiteSettings(); // instantiate new website settings object

                  ws.Id = reader.GetSafeInt32(startingIndex++); //assign website settings vales to website object
                  ws.SettingsId = reader.GetSafeInt32(startingIndex++);
                  ws.WebsiteId = reader.GetSafeInt32(startingIndex++);
                  ws.SettingsValue = reader.GetSafeString(startingIndex++);
                  ws.UserId = reader.GetSafeString(startingIndex++);
                  ws.MediaId = reader.GetSafeInt32(startingIndex++);
                  ws.DateAdded = reader.GetSafeDateTime(startingIndex++);
                  ws.DateModified = reader.GetSafeDateTime(startingIndex++);

                  Website c = new Website(); // instantiate a new website object

                  c.Id = reader.GetSafeInt32(startingIndex++); // assign website values to website object
                  c.Name = reader.GetSafeString(startingIndex++);
                  c.Slug = reader.GetSafeString(startingIndex++);
                  c.Description = reader.GetSafeString(startingIndex++);
                  c.Url = reader.GetSafeString(startingIndex++);
                  c.MediaId = reader.GetSafeInt32(startingIndex++);
                  c.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  c.DateModified = reader.GetSafeDateTime(startingIndex++);
                  c.Phone = reader.GetSafeString(startingIndex++);


                  if (c.Id != 0) // null check to see if website has values
                  {
                      ws.Website = c; // if website has values, assign to website settings object
                  }

                  Settings s = new Settings(); // instantiate a new instance of settings object

                  s.Id = reader.GetSafeInt32(startingIndex++); // assign settings values to settings object
                  s.Category = reader.GetSafeEnum<SettingsCategory>(startingIndex++);
                  s.Name = reader.GetSafeString(startingIndex++);
                  s.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  s.DateModified = reader.GetSafeDateTime(startingIndex++);
                  s.SettingType = reader.GetSafeEnum<SettingsType>(startingIndex++);
                  s.Description = reader.GetSafeString(startingIndex++);
                  s.SettingSlug = reader.GetSafeString(startingIndex++);
                  s.SettingSection = reader.GetSafeEnum<SettingsSection>(startingIndex++);

                  if (s.Id != 0) // null check to see if settings return values
                  {
                      ws.Setting = s; // assign returned values to website settings object
                  }

                  Media m = new Media(); // instantiate a new media object

                  m.Id = reader.GetSafeInt32(startingIndex++); //assign media values to media object
                  m.Url = reader.GetSafeString(startingIndex++);
                  m.MediaType = reader.GetSafeInt32(startingIndex++);
                  m.UserId = reader.GetSafeString(startingIndex++);
                  m.Title = reader.GetSafeString(startingIndex++);
                  m.Description = reader.GetSafeString(startingIndex++);
                  m.ExternalMediaId = reader.GetSafeInt32(startingIndex++);
                  m.FileType = reader.GetSafeString(startingIndex++);
                  m.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  m.DateModified = reader.GetSafeDateTime(startingIndex++);


                  if (m.Id != 0) // null check to see if media returned values
                  {
                      ws.Media = m; // assign returned media values to website settings object
                  }


                  if (list == null) // null check to see if list has any alues
                  {
                      list = new List<WebsiteSettings>(); // instantiate a new website settings object list
                  }

                  list.Add(ws); // add website settings object to list
              }

           );

            return list; // return list of website settings objects
        }


        public static List<WebsiteSettings> GetWebsiteSettingsBySlug(int WebsiteId, List<string> Slugs)
        {
            List<WebsiteSettings> list = new List<WebsiteSettings>();


            {
                DataProvider.ExecuteCmd(GetConnection, "dbo.WebsiteSettings_GetSettingsBySlug"
                  , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                  {
                      paramCollection.AddWithValue("@WebsiteId", WebsiteId);

                      SqlParameter s = new SqlParameter("@Slug", SqlDbType.Structured);
                      if (Slugs != null && Slugs.Any())
                      {
                          s.Value = new NVarcharTable(Slugs);
                      }
                      paramCollection.Add(s);

                  }, map: delegate (IDataReader reader, short set)
                  {
                      WebsiteSettings ws = new WebsiteSettings();

                      int startingIndex = 0;
                      ws.Id = reader.GetSafeInt32(startingIndex++);
                      ws.SettingsId = reader.GetSafeInt32(startingIndex++);
                      ws.WebsiteId = reader.GetSafeInt32(startingIndex++);
                      ws.SettingsValue = reader.GetSafeString(startingIndex++);
                      ws.UserId = reader.GetSafeString(startingIndex++);
                      ws.MediaId = reader.GetSafeInt32(startingIndex++);
                      ws.DateAdded = reader.GetSafeDateTime(startingIndex++);
                      ws.DateModified = reader.GetSafeDateTime(startingIndex++);

                      Website w = new Website();

                      w.Id = reader.GetSafeInt32(startingIndex++);
                      w.Name = reader.GetSafeString(startingIndex++);
                      w.Slug = reader.GetSafeString(startingIndex++);
                      w.Description = reader.GetSafeString(startingIndex++);
                      w.Url = reader.GetSafeString(startingIndex++);
                      w.MediaId = reader.GetSafeInt32(startingIndex++);
                      w.DateCreated = reader.GetSafeDateTime(startingIndex++);
                      w.DateModified = reader.GetSafeDateTime(startingIndex++);

                      ws.Website = w;

                      Settings s = new Settings();

                      s.Id = reader.GetSafeInt32(startingIndex++);
                      s.Category = reader.GetSafeEnum<SettingsCategory>(startingIndex++);
                      s.Name = reader.GetSafeString(startingIndex++);
                      s.DateCreated = reader.GetSafeDateTime(startingIndex++);
                      s.DateModified = reader.GetSafeDateTime(startingIndex++);
                      s.SettingType = reader.GetSafeEnum<SettingsType>(startingIndex++);
                      s.Description = reader.GetSafeString(startingIndex++);
                      s.SettingSlug = reader.GetSafeString(startingIndex++);
                      s.SettingSection = reader.GetSafeEnum<SettingsSection>(startingIndex++);

                      ws.Setting = s;

                      Media m = new Media();
                      m.Id = reader.GetSafeInt32(startingIndex++);
                      m.Url = reader.GetSafeString(startingIndex++);
                      m.MediaType = reader.GetSafeInt32(startingIndex++);
                      m.UserId = reader.GetSafeString(startingIndex++);
                      m.Title = reader.GetSafeString(startingIndex++);
                      m.Description = reader.GetSafeString(startingIndex++);
                      m.ExternalMediaId = reader.GetSafeInt32(startingIndex++);
                      m.FileType = reader.GetSafeString(startingIndex++);
                      m.DateCreated = reader.GetSafeDateTime(startingIndex++);
                      m.DateModified = reader.GetSafeDateTime(startingIndex++);

                      if (m.Id != 0)
                      {
                          ws.Media = m;
                      }

                      if (list == null)
                      {
                          list = new List<WebsiteSettings>();
                      }

                      list.Add(ws);
                  }
                  );

                return list;
            }
        }

        public static Dictionary<string, WebsiteSettings> getWebsiteSettingsDictionaryBySlug(int websiteId, List<string> Slugs)
        {
            Dictionary<string, WebsiteSettings> dict = null;
            List<WebsiteSettings> list = GetWebsiteSettingsBySlug(websiteId, Slugs);

            if (list != null)
            {
                dict = new Dictionary<string, WebsiteSettings>();

                foreach (var setting in list)
                {
                    dict.Add(setting.Setting.SettingSlug, setting);
                    
                }
            }

            return dict;

        }

        // Get by website slug - website, media, address
        public static PaginatedItemsResponse<WebsiteSettings> GetSettingsByQuery(PaginatedRequest model)
        {
            List<WebsiteSettings> list = null;
            PaginatedItemsResponse<WebsiteSettings> response = null;


            DataProvider.ExecuteCmd(GetConnection, "dbo.WebsiteSettings_GetByWebsiteSlug_Query"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@Slug", model.Slug);
                  paramCollection.AddWithValue("@CurrentPage", model.CurrentPage);
                  paramCollection.AddWithValue("@ItemsPerPage", model.ItemsPerPage);
                  paramCollection.AddWithValue("@Query", model.Query);
                  paramCollection.AddWithValue("@QueryCategory", model.QueryCategory);
                  paramCollection.AddWithValue("@QuerySettingSection", model.QuerySettingSection);
                  paramCollection.AddWithValue("@QuerySettingType", model.QuerySettingType);


              }, map: delegate (IDataReader reader, short set)
              {
                  if (set == 0)
                  {
                      int startingIndex = 0;

                      WebsiteSettings ws = new WebsiteSettings();

                      ws.Id = reader.GetSafeInt32(startingIndex++);
                      ws.SettingsId = reader.GetSafeInt32(startingIndex++);
                      ws.WebsiteId = reader.GetSafeInt32(startingIndex++);
                      ws.SettingsValue = reader.GetSafeString(startingIndex++);
                      ws.UserId = reader.GetSafeString(startingIndex++);
                      ws.MediaId = reader.GetSafeInt32(startingIndex++);
                      ws.DateAdded = reader.GetSafeDateTime(startingIndex++);
                      ws.DateModified = reader.GetSafeDateTime(startingIndex++);


                      Website c = new Website();

                      c.Id = reader.GetSafeInt32(startingIndex++);
                      c.Name = reader.GetSafeString(startingIndex++);
                      c.Slug = reader.GetSafeString(startingIndex++);
                      c.Description = reader.GetSafeString(startingIndex++);
                      c.Url = reader.GetSafeString(startingIndex++);
                      c.MediaId = reader.GetSafeInt32(startingIndex++);
                      c.DateCreated = reader.GetSafeDateTime(startingIndex++);
                      c.DateModified = reader.GetSafeDateTime(startingIndex++);
                      c.Phone = reader.GetSafeString(startingIndex++);


                      if (c.Id != 0)
                      {
                          ws.Website = c;
                      }

                      Settings s = new Settings();

                      s.Id = reader.GetSafeInt32(startingIndex++);
                      s.Category = reader.GetSafeEnum<SettingsCategory>(startingIndex++);
                      s.Name = reader.GetSafeString(startingIndex++);
                      s.DateCreated = reader.GetSafeDateTime(startingIndex++);
                      s.DateModified = reader.GetSafeDateTime(startingIndex++);
                      s.SettingType = reader.GetSafeEnum<SettingsType>(startingIndex++);
                      s.Description = reader.GetSafeString(startingIndex++);
                      s.SettingSlug = reader.GetSafeString(startingIndex++);
                      s.SettingSection = reader.GetSafeEnum<SettingsSection>(startingIndex++);

                      if (s.Id != 0)
                      {
                          ws.Setting = s;
                      }

                      Media m = new Media();

                      m.Id = reader.GetSafeInt32(startingIndex++);
                      m.Url = reader.GetSafeString(startingIndex++);
                      m.MediaType = reader.GetSafeInt32(startingIndex++);
                      m.UserId = reader.GetSafeString(startingIndex++);
                      m.Title = reader.GetSafeString(startingIndex++);
                      m.Description = reader.GetSafeString(startingIndex++);
                      m.ExternalMediaId = reader.GetSafeInt32(startingIndex++);
                      m.FileType = reader.GetSafeString(startingIndex++);
                      m.DateCreated = reader.GetSafeDateTime(startingIndex++);
                      m.DateModified = reader.GetSafeDateTime(startingIndex++);


                      if (m.Id != 0)
                      {
                          ws.Media = m;
                      }


                      if (list == null)
                      {
                          list = new List<WebsiteSettings>();
                      }

                      list.Add(ws);
                  }
                  else if (set == 1)
                  {
                      response = new PaginatedItemsResponse<WebsiteSettings>();
                      response.TotalItems = reader.GetSafeInt32(0);
                  }
              }

           );

            response.Items = list;
            response.CurrentPage = model.CurrentPage;
            response.ItemsPerPage = model.ItemsPerPage;
            response.Query = model.Query;

            return response;
        }


        public static List<WebsiteSettings> Get()
        {
            List<WebsiteSettings> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.WebsiteSettings_SelectAll"
                , inputParamMapper: null
                , map: delegate (IDataReader reader, short set)
                 {
                     WebsiteSettings ws = new WebsiteSettings();
                     int startingIndex = 0;

                     ws.Id = reader.GetSafeInt32(startingIndex++);
                     ws.SettingsId = reader.GetSafeInt32(startingIndex++);
                     ws.WebsiteId = reader.GetSafeInt32(startingIndex++);
                     ws.SettingsValue = reader.GetSafeString(startingIndex++);
                     ws.UserId = reader.GetSafeString(startingIndex++);
                     ws.MediaId = reader.GetSafeInt32(startingIndex++);
                     ws.DateAdded = reader.GetSafeDateTime(startingIndex++);
                     ws.DateModified = reader.GetSafeDateTime(startingIndex++);

                     if (list == null)
                     {
                         list = new List<WebsiteSettings>();
                     }
                     list.Add(ws);
                 }
        );
            return list;
        }

        public static WebsiteSettings GetById(int Id)
        {
            WebsiteSettings ws = new WebsiteSettings();

            DataProvider.ExecuteCmd(GetConnection, "dbo.WebsiteSettings_SelectById"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@Id", Id);

              }
              , map: delegate (IDataReader reader, short set)
               {
                   int startingIndex = 0;

                   ws.Id = reader.GetSafeInt32(startingIndex++);
                   ws.SettingsId = reader.GetSafeInt32(startingIndex++);
                   ws.WebsiteId = reader.GetSafeInt32(startingIndex++);
                   ws.SettingsValue = reader.GetSafeString(startingIndex++);
                   ws.UserId = reader.GetSafeString(startingIndex++);
                   ws.MediaId = reader.GetSafeInt32(startingIndex++);
                   ws.DateAdded = reader.GetSafeDateTime(startingIndex++);
                   ws.DateModified = reader.GetSafeDateTime(startingIndex++);

               }
              );
            return ws;
        }

        public static int Insert(WebsiteSettingsAddRequest model)
        {
            int id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.WebsiteSettings_Insert",
                inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@SettingsId", model.SettingsId);
                    paramCollection.AddWithValue("@WebsiteId", model.WebsiteId);
                    paramCollection.AddWithValue("@SettingsValue", model.SettingsValue);
                    paramCollection.AddWithValue("@UserId", model.UserId);
                    paramCollection.AddWithValue("@MediaId", model.MediaId);



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

        public static int Update(int Id, WebsiteSettingsUpdateRequest model)
        {
            int id = 0;
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.WebsiteSettings_Update",
                inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Id", Id);
                    paramCollection.AddWithValue("@SettingsValue", model.SettingsValue);
                    paramCollection.AddWithValue("@UserId", model.UserId);
                    paramCollection.AddWithValue("@MediaId", model.MediaId);


                },
                returnParameters: delegate (SqlParameterCollection param)
                {
                    id = Id;
                }
                );
            return id;
        }

        public static void Delete(int Id)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.WebsiteSettings_Delete",
              inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@Id", Id);
              });
        }


        public static List<WebsiteSettings> GetByWebId(int Id)
        {
            List<WebsiteSettings> WsList = null;

            DataProvider.ExecuteCmd(GetConnection, "WebsiteSettings_GetByWebsiteId"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@WebsiteId", Id);

              }
              , map: delegate (IDataReader reader, short set)
              {
                  int startingIndex = 0;
                  WebsiteSettings ws = new WebsiteSettings();

                  ws.Id = reader.GetSafeInt32(startingIndex++);
                  ws.SettingsId = reader.GetSafeInt32(startingIndex++);
                  ws.WebsiteId = reader.GetSafeInt32(startingIndex++);
                  ws.SettingsValue = reader.GetSafeString(startingIndex++);
                  ws.UserId = reader.GetSafeString(startingIndex++);
                  ws.MediaId = reader.GetSafeInt32(startingIndex++);
                  ws.DateAdded = reader.GetSafeDateTime(startingIndex++);
                  ws.DateModified = reader.GetSafeDateTime(startingIndex++);

                  Website w = new Website();

                  w.Id = reader.GetSafeInt32(startingIndex++);
                  w.Name = reader.GetSafeString(startingIndex++);
                  w.Slug = reader.GetSafeString(startingIndex++);
                  w.Description = reader.GetSafeString(startingIndex++);
                  w.Url = reader.GetSafeString(startingIndex++);
                  w.MediaId = reader.GetSafeInt32(startingIndex++);
                  w.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  w.DateModified = reader.GetSafeDateTime(startingIndex++);

                  ws.Website = w;

                  Settings s = new Settings();

                  s.Id = reader.GetSafeInt32(startingIndex++);
                  s.Category = reader.GetSafeEnum<SettingsCategory>(startingIndex++);
                  s.Name = reader.GetSafeString(startingIndex++);
                  s.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  s.DateModified = reader.GetSafeDateTime(startingIndex++);
                  s.SettingType = reader.GetSafeEnum<SettingsType>(startingIndex++);
                  s.Description = reader.GetSafeString(startingIndex++);
                  s.SettingSlug = reader.GetSafeString(startingIndex++);
                  s.SettingSection = reader.GetSafeEnum<SettingsSection>(startingIndex++);

                  ws.Setting = s;

                  Media m = new Media();
                  m.Id = reader.GetSafeInt32(startingIndex++);
                  m.Url = reader.GetSafeString(startingIndex++);
                  m.MediaType = reader.GetSafeInt32(startingIndex++);
                  m.UserId = reader.GetSafeString(startingIndex++);
                  m.Title = reader.GetSafeString(startingIndex++);
                  m.Description = reader.GetSafeString(startingIndex++);
                  m.ExternalMediaId = reader.GetSafeInt32(startingIndex++);
                  m.FileType = reader.GetSafeString(startingIndex++);
                  m.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  m.DateModified = reader.GetSafeDateTime(startingIndex++);

                  if (m.Id != 0)
                  {
                      ws.Media = m;
                  }

                  if (WsList == null)
                  {
                      //     new list
                      WsList = new List<WebsiteSettings>();
                  }
                  // add ws to list
                  WsList.Add(ws);
              }
              );
            return WsList;
        }
    }

}