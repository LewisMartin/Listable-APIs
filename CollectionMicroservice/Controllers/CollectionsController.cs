﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Listable.CollectionMicroservice.DTO;
using Listable.CollectionMicroservice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Listable.CollectionMicroservice.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class CollectionsController : Controller
    {
        private CollectionStore _collectionStore;

        public CollectionsController(CollectionStore collectionStore)
        {
            _collectionStore = collectionStore;
        }

        // GET collections/retrieve
        [HttpGet]
        public JsonResult Retrieve(string collectionId)
        {
            if (collectionId != null)
                return Json(_collectionStore.GetCollection(collectionId));

            return Json("Collection not found");
        }

        // GET collections/retrieveall
        [HttpGet]
        public JsonResult RetrieveAll(string userId)
        {
            if(userId != null)
                return Json(_collectionStore.GetAllCollectionsForUser(userId).ToList());

            return Json(_collectionStore.GetAllCollections().ToList()); 
        }

        // POST collections/create
        [HttpPost]
        public async Task<string> Create([FromBody] string data)
        {
            var collections = JsonConvert.DeserializeObject<List<Collection>>(data, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy HH:mm:ss" });

            await _collectionStore.InsertCollections(collections);

            return "Created test collections";
        }

        // PUT collections/update/5
        [HttpPut]
        public string Update(string id, [FromBody]string data)
        {
            var collection = JsonConvert.DeserializeObject<Collection>(data, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy HH:mm:ss" });

            _collectionStore.UpdateCollection(id, collection);

            return "Updated collection: " + id;
        }

        // DELETE collections/delete/5
        [HttpDelete]
        public string Delete(string id)
        {
            var success = _collectionStore.DeleteCollection(id);

            if (success)
                return "Deleted collection: " + id;
            else
                return "Deletion failed";
        }
    }
}