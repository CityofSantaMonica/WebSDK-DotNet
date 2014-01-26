﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accela.Web.SDK;
using Accela.Web.SDK.Models;
using System.Net.Http;
using System.Net;
using System.Web;
using System.IO;
using Newtonsoft.Json;

namespace Accela.Web.SDK
{
    public class RecordHandler : BaseHandler, IRecord
    {
        public RecordHandler(string appId, string appSecret, ApplicationType appType) : base(appId, appSecret, appType) { } 

        #region Related Records
        public List<Record> GetRelatedRecords(string recordId, string token)
        {
            try
            {
                // Validate
                RequestValidator.ValidateToken(token);
                if (String.IsNullOrWhiteSpace(recordId))
                {
                    throw new Exception("Null Record Id provided");
                }

                // get related record
                string url = apiUrl + ConfigurationReader.GetValue("GetRelatedRecords").Replace("{recordId}", recordId);
                RESTResponse response = HttpHelper.SendGetRequest(url, token, this.appId);

                // create response
                List<Record> records = new List<Record>();
                records = (List<Record>)HttpHelper.ConvertToSDKResponse(records, response);
                return records;
            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Get Related Record :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Get Related Record :"));
            }
        }
        #endregion

        #region Record Fees
        public List<RecordFees> GetRecordFees(string recordId, string token) // TODO
        {
            try
            {
                // Validate
                RequestValidator.ValidateToken(token);
                if (String.IsNullOrWhiteSpace(recordId))
                {
                    throw new Exception("Null Record Id provided");
                }

                // get related record
                string url = apiUrl + ConfigurationReader.GetValue("GetRecordFees").Replace("{recordId}", recordId);
                RESTResponse response = HttpHelper.SendGetRequest(url, token, this.appId);

                // create response
                List<RecordFees> recordFees = new List<RecordFees>();
                recordFees = (List<RecordFees>)HttpHelper.ConvertToSDKResponse(recordFees, response);
                return recordFees;
            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Get Record Fees :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Get Record Fees :"));
            }
        }
        #endregion

        #region Records
        public Record GetRecord(string recordId, string token)
        {
            try
            {
                // Validate
                RequestValidator.ValidateToken(token);
                if (String.IsNullOrWhiteSpace(recordId))
                {
                    throw new Exception("Null Record Id provided");
                }

                // get record summary
                string url = apiUrl + ConfigurationReader.GetValue("GetRecord").Replace("{recordIds}", recordId);
                RESTResponse response = HttpHelper.SendGetRequest(url, token, this.appId);

                // create response
                List<Record> records = new List<Record>();
                records = (List<Record>)HttpHelper.ConvertToSDKResponse(records, response);
                if (records != null && records.Count > 0)
                    return records[0];
                return null;
            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Get Record :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Get Record :"));
            }
        }

        public List<Record> GetRecords(string token, string filter, ref PaginationInfo paginationInfo, int offset = -1, int limit = -1)
        {
            try
            {
                // validate
                RequestValidator.ValidateToken(token);

                // create url
                StringBuilder url = new StringBuilder(apiUrl);
                if (this.appType == ApplicationType.Agency)
                {
                    url = url.Append(ConfigurationReader.GetValue("GetRecords")).Replace("{limit}", limit.ToString()).Replace("{offset}", offset.ToString()); 
                    if (!string.IsNullOrEmpty(filter))
                       url.Append("&").Append(filter);
                }
                else if (this.appType == ApplicationType.Citizen)
                {
                    url = url.Append(ConfigurationReader.GetValue("GetMyRecords"));
                }

                // get records
                RESTResponse response = HttpHelper.SendGetRequest(url.ToString(), token, this.appId);

                // create response
                List<Record> records = new List<Record>();
                records = (List<Record>)HttpHelper.ConvertToSDKResponse(records, response, ref paginationInfo);
                return records;
            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Get Records :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Get Records :"));
            }
        }

        public RecordId CreateRecordFinalize(Record record, string token) // Doesn't work
        {
            return CreateRecordInternal(record, token, "final");
        }

        public RecordId CreateRecordInitialize(Record record, string token) // Doesn't work bug raised
        {
            return CreateRecordInternal(record, token, "initial");
        }

        public RecordId CreateRecord(Record record, string token)
        {
            return CreateRecordInternal(record, token, "");
        }

        public Record UpdateRecordDetail(Record record, string token) // Doesn't work 400 
        {
            try
            {
                // Validate
                RequestValidator.ValidateToken(token);
                if (record == null)
                {
                    throw new Exception("Null request provided");
                }

                // Update 
                string url = apiUrl + ConfigurationReader.GetValue("UpdateRecord");
                RESTResponse response = HttpHelper.SendPutRequest(url, record, token, this.appId);
                return (Record)HttpHelper.ConvertToSDKResponse(record, response);
            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Update Record :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Update Record :"));
            }
        }

        public void DeleteRecord(string recordId, string token) { }

        #endregion

        #region Record Contacts
        public List<Contact> SearchRecordContacts(string token, string filter, ref PaginationInfo paginationInfo, int offset = -1, int limit = -1) // TODO
        { 
            try
            {
                // Validate
                RequestValidator.ValidateToken(token);

                StringBuilder url = new StringBuilder(apiUrl).Append(ConfigurationReader.GetValue("SearchContact")).Replace("{limit}", limit.ToString()).Replace("{offset}", offset.ToString()); ;
                if (!String.IsNullOrWhiteSpace(filter))
                {
                    url.Append("&amp;");
                    url.Append(filter);
                }

                // get contacts
                List<Contact> responseGetRecordContacts = new List<Contact>();
                RESTResponse response = HttpHelper.SendGetRequest(url.ToString(), token, this.appId);

                // create response
                responseGetRecordContacts = (List<Contact>)HttpHelper.ConvertToSDKResponse(responseGetRecordContacts, response);
                return responseGetRecordContacts;
            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Search Record Contacts :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Search Record Contacts :"));
            }
        }

        public List<ContactType> GetContactTypes(string token) // 404 BUg opened
        {
            try
            {
                // Validate
                RequestValidator.ValidateToken(token);

                // get contacts
                List<ContactType> contactTypes = new List<ContactType>();
                string url = apiUrl + ConfigurationReader.GetValue("GetContactTypes");
                RESTResponse response = HttpHelper.SendGetRequest(url, token, this.appId);

                // create response
                contactTypes = (List<ContactType>)HttpHelper.ConvertToSDKResponse(contactTypes, response);
                return contactTypes;
            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Get Contact Types :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Get Contact Types :"));
            }
        }

        public List<Contact> GetRecordContacts(string recordId, string token, ref PaginationInfo paginationInfo, int offset = -1, int limit = -1) // TODO add offset & limit
        {
            try
            {
                // Validate
                if (String.IsNullOrWhiteSpace(recordId))
                {
                    throw new Exception("Null Record Id provided");
                }
                RequestValidator.ValidateToken(token);

                // get contacts
                string url = apiUrl + ConfigurationReader.GetValue("GetRecordContacts").Replace("{recordIds}", recordId);
                RESTResponse response = HttpHelper.SendGetRequest(url, token, this.appId);

                // create response
                List<Contact> contacts = new List<Contact>();
                return (List<Contact>)HttpHelper.ConvertToSDKResponse(contacts, response);
            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Get Record Contacts :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Get Record Contacts :"));
            }
        }

        public Result CreateRecordContact(List<Contact> contacts, string recordId, string token) // TODO
        {
            try
            {
                // Validate
                RequestValidator.ValidateToken(token);
                if (String.IsNullOrWhiteSpace(recordId))
                {
                    throw new Exception("Null Record Id provided");
                }
                if (contacts == null)
                {
                    throw new Exception("Null request provided");
                }

                // Update 
                string url = apiUrl + ConfigurationReader.GetValue("CreateRecordContact").Replace("recordId", recordId);
                RESTResponse response = HttpHelper.SendPostRequest(url, contacts, token, this.appId);

                // create response
                Result result = new Result();
                return (Result)HttpHelper.ConvertToSDKResponse(result, response);

            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Update Record Contact :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Update Record Contact :"));
            }
        }

        public Contact UpdateRecordContact(Contact contact, string recordId, string token) // TODO
        {
            try
            {
                // Validate
                RequestValidator.ValidateToken(token);
                if (String.IsNullOrWhiteSpace(recordId))
                {
                    throw new Exception("Null Record Id provided");
                }
                if (contact == null)
                {
                    throw new Exception("Null request provided");
                }

                // Update 
                string url = apiUrl + ConfigurationReader.GetValue("UpdateRecordContact").Replace("recordId", recordId).Replace("{id}", contact.id);
                RESTResponse response = HttpHelper.SendPutRequest(url, contact, token, this.appId);

                // create response
                return (Contact)HttpHelper.ConvertToSDKResponse(contact, response);
            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Update Record Contact :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Update Record Contact :"));
            }
        }

        public void DeleteRecordContact(string contactId, string recordId, string token) 
        {
            try
            {
                // Validate
                RequestValidator.ValidateToken(token);
                if (String.IsNullOrWhiteSpace(recordId))
                {
                    throw new Exception("Null Record Id provided");
                }
                if (String.IsNullOrWhiteSpace(contactId))
                {
                    throw new Exception("Null Contact Id provided");
                }

                // Update 
                string url = apiUrl + ConfigurationReader.GetValue("UpdateRecordContact").Replace("recordId", recordId).Replace("{id}", contactId);
                RESTResponse response = HttpHelper.SendDeleteRequest(url, token, appId);
                HttpHelper.ConvertToSDKResponse(null, response);
            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Delete Record Contact :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Delete Record Contact :"));
            }
        }
        #endregion

        #region Record Customfields
        public List<Dictionary<string, string>> GetRecordCustomFields(string recordId, string token)
        {
            try
            {
                // Validate
                if (String.IsNullOrWhiteSpace(recordId))
                {
                    throw new Exception("Null Record Id provided");
                }
                RequestValidator.ValidateToken(token);

                // get Custom Fields
                string url = apiUrl + ConfigurationReader.GetValue("GetRecordCustomFields").Replace("{recordIds}", recordId);
                RESTResponse response = HttpHelper.SendGetRequest(url, token, this.appId);

                // create response
                List<Dictionary<string, string>> customFieldList = new List<Dictionary<string, string>>();
                customFieldList = (List<Dictionary<string, string>>)HttpHelper.ConvertToSDKResponse(customFieldList, response);
                return customFieldList; 
            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Get Record Custom Fields :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Get Record Custom Fields :"));
            }
        }

        // Describe Custom Fields
        public Response DescribeRecordCustomFields(string recordTypeId, string token) // TODO
        {
            try
            {
                // Validate
                if (String.IsNullOrWhiteSpace(recordTypeId))
                {
                    throw new Exception("Null Record Type Id provided");
                }
                RequestValidator.ValidateToken(token);

                // get Custom Field Desc
                Response asis = new Response();
                string url = apiUrl + ConfigurationReader.GetValue("DescribeASI").Replace("{recordTypeId}", recordTypeId);
                //asis = (ResponseGetRecordASIs)HttpHelper.SendGetRequest(url, asis, token, appInfo);
                return asis;
            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Describe Record Custom Fields :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Describe Record Custom Fields :"));
            }
        }

        public List<Dictionary<string, string>> UpdateRecordCustomFields(string recordId, List<Dictionary<string, string>> customFieldList, string token) // Doesn't work bug raised
        {
            try
            {
                // Validate
                if (String.IsNullOrWhiteSpace(recordId))
                {
                    throw new Exception("Null Record Id provided");
                }
                if (customFieldList == null || customFieldList.Count == 0)
                {
                    throw new Exception("Null Custom Field List provided");
                }
                RequestValidator.ValidateToken(token);


                // update Custom Fields
                string url = apiUrl + ConfigurationReader.GetValue("UpdateRecordCustomFields").Replace("{recordId}", recordId);
                RESTResponse response = HttpHelper.SendPutRequest(url, customFieldList, token, this.appId);

                // create response
                customFieldList = (List<Dictionary<string, string>>)HttpHelper.ConvertToSDKResponse(customFieldList, response);
                return customFieldList; 

            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Update Record Custom Fields :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Update Record Custom Fields :"));
            }
        }
        #endregion

        #region Record Documents
        public List<Document> GetRecordDocuments(string recordId, string token)
        {
            try
            {
                // Validate
                if (String.IsNullOrWhiteSpace(recordId))
                {
                    throw new Exception("Null Record Id provided");
                }
                RequestValidator.ValidateToken(token);

                // get record documents
                string url = apiUrl + ConfigurationReader.GetValue("GetRecordDocuments").Replace("{recordId}", recordId);
                RESTResponse response = HttpHelper.SendGetRequest(url, token, this.appId);

                // create response
                List<Document> docList = new List<Document>();
                docList = (List<Document>)HttpHelper.ConvertToSDKResponse(docList, response);
                return docList; 
            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Get Record Documents :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Get Record Documents :"));
            }
        }

        public Document CreateRecordDocument(string documentPath, string documentDescription, string recordId, string token) // TODO does not work
        {
            try
            {
                // Validate
                if (String.IsNullOrWhiteSpace(recordId))
                {
                    throw new Exception("Null Record Id provided");
                }
                if (String.IsNullOrWhiteSpace(documentPath))
                {
                    throw new Exception("Please provide a valid path to upload your document");
                }
                RequestValidator.ValidateToken(token);

                string url = apiUrl + ConfigurationReader.GetValue("CreateRecordDocument").Replace("{recordId}", recordId);
                RESTResponse response = HttpHelper.SendUploadRequest(documentPath, documentDescription, url, token, appId);

                // create response
                Document doc = new Document();
                doc = (Document)HttpHelper.ConvertToSDKResponse(doc, response);
                return doc;
            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Create Record Documents :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Create Record Documents :"));
            }
        }

        public void DeleteRecordDocuments(string recordId, string token) // TODO test
        {
            try
            {
                // Validate
                if (String.IsNullOrWhiteSpace(recordId))
                {
                    throw new Exception("Null Record Id provided");
                }
                RequestValidator.ValidateToken(token);

                // delete documents
                string url = apiUrl + ConfigurationReader.GetValue("DeleteRecordDocuments").Replace("{recordIds}", recordId);
                RESTResponse response = HttpHelper.SendDeleteRequest(url, token, this.appId);
                HttpHelper.ConvertToSDKResponse(null, response);
            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Delete Record Documents :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Delete Record Documents :"));
            }
        }

        public void DeleteRecordDocument(string documentId, string recordId, string token) // TODO test
        {
            try
            {
                // Validate
                if (String.IsNullOrWhiteSpace(recordId))
                {
                    throw new Exception("Null Record Id provided");
                }
                if (String.IsNullOrWhiteSpace(documentId))
                {
                    throw new Exception("Null Document Id provided");
                }
                RequestValidator.ValidateToken(token);

                // delete document
                string url = apiUrl + ConfigurationReader.GetValue("DeleteRecordDocuments").Replace("{recordIds}", recordId).Replace("{documentId}", documentId);
                RESTResponse response = HttpHelper.SendDeleteRequest(url, token, this.appId);
                HttpHelper.ConvertToSDKResponse(null, response);
            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Delete Record Documents :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Delete Record Documents :"));
            }
        }
        #endregion

        #region private methods
        private RecordId CreateRecordInternal(Record record, string token, string type)
        {
            try
            {
                // Validate
                RequestValidator.ValidateToken(token);
                if (record == null)
                {
                    throw new Exception("Null request provided");
                }

                // Create
                string url = null;
                switch (type)
                {
                    case "initial":
                        url = apiUrl + ConfigurationReader.GetValue("CreatePartialRecord");
                        break;
                    case "final":
                        url = apiUrl + ConfigurationReader.GetValue("CreateFinalRecord");
                        break;
                    case "":
                        url = apiUrl + ConfigurationReader.GetValue("CreateRecord");
                        break;
                }

                RESTResponse response = HttpHelper.SendPostRequest(url, record, token, this.appId);

                // Response
                Record responseRecord = new Record();
                responseRecord = (Record)HttpHelper.ConvertToSDKResponse(responseRecord, response);
                if (responseRecord != null)
                {
                    return new RecordId { id = responseRecord.id, customId = responseRecord.customId, serviceProviderCode = responseRecord.serviceProviderCode, trackingId = responseRecord.trackingId };
                }
                return null;
            }
            catch (WebException webException)
            {
                throw new Exception(HttpHelper.HandleWebException(webException, "Error in Create Record :"));
            }
            catch (Exception exception)
            {
                throw new Exception(HttpHelper.HandleException(exception, "Error in Create Record :"));
            }
        }
        #endregion
    }
}