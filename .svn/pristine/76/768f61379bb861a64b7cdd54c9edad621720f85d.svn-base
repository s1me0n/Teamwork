using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SecretCommunicator.Data.Interfaces;
using SecretCommunicator.Models.Interfaces;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Formatting;
namespace SecretCommunicator.Data.Helpers
{
    public class BaseApiController<T> : ApiController where T : class, IIdentifier
    {
        protected IRepository DataStore { get; set; }
        protected string[] Includes { get; set; }

        //public BaseApiController()
        //{
        //    //TODO: USE DEPENDENCY INJECTION FOR DECOUPLING
        //    this.DataStore = new EfRepository();
        //}

        public BaseApiController(IRepository albumRepository)
        {
            DataStore = albumRepository;
        }

        // GET api/<controller>
        public virtual IEnumerable<T> Get()
        {
            return DataStore.All<T>(Includes);
        }

        // GET api/<controller>/5
        public virtual T Get(string id)
        {
            var result = DataStore.Find<T>(t => t.Id == id, Includes);
            return result;
        }

        // POST api/<controller>
        public virtual HttpResponseMessage Post([FromBody]T value)
        {
            try
            {
                DataStore.Create<T>(value);

                var response =
                Request.CreateResponse(HttpStatusCode.Created, value);

                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = value.Id }));
                return response;
            }
            catch (Exception ex)
            {
                var errResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                throw new HttpResponseException(errResponse);
            }
        }

        // PUT api/<controller>
        public virtual HttpResponseMessage Put([FromBody]T value)
        {
            try
            {
                DataStore.Update<T>(value);
                var response =
                    Request.CreateResponse(HttpStatusCode.Created, value);

                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = value.Id }));
                return response;
            }
            catch (Exception ex)
            {
                var errResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                throw new HttpResponseException(errResponse);
            }
        }

        // DELETE api/<controller>/5
        public virtual void Delete(string id)
        {
            DataStore.Delete<T>(id);
        }

        public virtual void Delete([FromBody]T value)
        {
            Delete(value.Id);
        }

        protected IEnumerable GetModelErrors()
        {
            return this.ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        }
    }
}
