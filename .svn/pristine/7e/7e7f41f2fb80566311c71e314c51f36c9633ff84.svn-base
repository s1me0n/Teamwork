using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SecretCommunicator.Models.Library
{
    [DataContract]
    public class ServiceData
    {
        private bool _IsError = false;
        [DataMember(Name = "IsError")]
        public bool IsError
        {
            get { return _IsError; }
            set { _IsError = value; }
        }

        private string _ErrorMessage = string.Empty;
        [DataMember(Name = "ErrorMessage")]
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }

        private string _ResultType = string.Empty;
        [DataMember(Name = "ResultType")]
        public string ResultType
        {
            get { return _ResultType; }
            set { _ResultType = value; }
        }

        private dynamic _Result;
        [DataMember(Name = "Result")]
        public dynamic Result
        {
            get { return _Result; }
            set { _Result = value; }
        }
    }
}
