﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.237
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.237.
// 
#pragma warning disable 1591

namespace AuditWizardv8.AuditWizardWebService {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="CustomerWebServiceSoap", Namespace="http://laytontechnology.com/")]
    public partial class CustomerWebService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetCustomerExpiryDateOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetLatestVersionNumberOperationCompleted;
        
        private System.Threading.SendOrPostCallback CheckCustomerPaidOperationCompleted;
        
        private System.Threading.SendOrPostCallback TestConnectionOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public CustomerWebService() {
            this.Url = "http://www.laytontechnology.com.au/AW_Web_Services/CustomerWebService.asmx";
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event GetCustomerExpiryDateCompletedEventHandler GetCustomerExpiryDateCompleted;
        
        /// <remarks/>
        public event GetLatestVersionNumberCompletedEventHandler GetLatestVersionNumberCompleted;
        
        /// <remarks/>
        public event CheckCustomerPaidCompletedEventHandler CheckCustomerPaidCompleted;
        
        /// <remarks/>
        public event TestConnectionCompletedEventHandler TestConnectionCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://laytontechnology.com/GetCustomerExpiryDate", RequestNamespace="http://laytontechnology.com/", ResponseNamespace="http://laytontechnology.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.DateTime GetCustomerExpiryDate(string aCompanyID) {
            object[] results = this.Invoke("GetCustomerExpiryDate", new object[] {
                        aCompanyID});
            return ((System.DateTime)(results[0]));
        }
        
        /// <remarks/>
        public void GetCustomerExpiryDateAsync(string aCompanyID) {
            this.GetCustomerExpiryDateAsync(aCompanyID, null);
        }
        
        /// <remarks/>
        public void GetCustomerExpiryDateAsync(string aCompanyID, object userState) {
            if ((this.GetCustomerExpiryDateOperationCompleted == null)) {
                this.GetCustomerExpiryDateOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetCustomerExpiryDateOperationCompleted);
            }
            this.InvokeAsync("GetCustomerExpiryDate", new object[] {
                        aCompanyID}, this.GetCustomerExpiryDateOperationCompleted, userState);
        }
        
        private void OnGetCustomerExpiryDateOperationCompleted(object arg) {
            if ((this.GetCustomerExpiryDateCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetCustomerExpiryDateCompleted(this, new GetCustomerExpiryDateCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://laytontechnology.com/GetLatestVersionNumber", RequestNamespace="http://laytontechnology.com/", ResponseNamespace="http://laytontechnology.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetLatestVersionNumber() {
            object[] results = this.Invoke("GetLatestVersionNumber", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetLatestVersionNumberAsync() {
            this.GetLatestVersionNumberAsync(null);
        }
        
        /// <remarks/>
        public void GetLatestVersionNumberAsync(object userState) {
            if ((this.GetLatestVersionNumberOperationCompleted == null)) {
                this.GetLatestVersionNumberOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetLatestVersionNumberOperationCompleted);
            }
            this.InvokeAsync("GetLatestVersionNumber", new object[0], this.GetLatestVersionNumberOperationCompleted, userState);
        }
        
        private void OnGetLatestVersionNumberOperationCompleted(object arg) {
            if ((this.GetLatestVersionNumberCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetLatestVersionNumberCompleted(this, new GetLatestVersionNumberCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://laytontechnology.com/CheckCustomerPaid", RequestNamespace="http://laytontechnology.com/", ResponseNamespace="http://laytontechnology.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool CheckCustomerPaid(string aCompanyID) {
            object[] results = this.Invoke("CheckCustomerPaid", new object[] {
                        aCompanyID});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void CheckCustomerPaidAsync(string aCompanyID) {
            this.CheckCustomerPaidAsync(aCompanyID, null);
        }
        
        /// <remarks/>
        public void CheckCustomerPaidAsync(string aCompanyID, object userState) {
            if ((this.CheckCustomerPaidOperationCompleted == null)) {
                this.CheckCustomerPaidOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCheckCustomerPaidOperationCompleted);
            }
            this.InvokeAsync("CheckCustomerPaid", new object[] {
                        aCompanyID}, this.CheckCustomerPaidOperationCompleted, userState);
        }
        
        private void OnCheckCustomerPaidOperationCompleted(object arg) {
            if ((this.CheckCustomerPaidCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CheckCustomerPaidCompleted(this, new CheckCustomerPaidCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://laytontechnology.com/TestConnection", RequestNamespace="http://laytontechnology.com/", ResponseNamespace="http://laytontechnology.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string TestConnection() {
            object[] results = this.Invoke("TestConnection", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void TestConnectionAsync() {
            this.TestConnectionAsync(null);
        }
        
        /// <remarks/>
        public void TestConnectionAsync(object userState) {
            if ((this.TestConnectionOperationCompleted == null)) {
                this.TestConnectionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnTestConnectionOperationCompleted);
            }
            this.InvokeAsync("TestConnection", new object[0], this.TestConnectionOperationCompleted, userState);
        }
        
        private void OnTestConnectionOperationCompleted(object arg) {
            if ((this.TestConnectionCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.TestConnectionCompleted(this, new TestConnectionCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetCustomerExpiryDateCompletedEventHandler(object sender, GetCustomerExpiryDateCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetCustomerExpiryDateCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetCustomerExpiryDateCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public System.DateTime Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.DateTime)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void GetLatestVersionNumberCompletedEventHandler(object sender, GetLatestVersionNumberCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetLatestVersionNumberCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetLatestVersionNumberCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void CheckCustomerPaidCompletedEventHandler(object sender, CheckCustomerPaidCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CheckCustomerPaidCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CheckCustomerPaidCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void TestConnectionCompletedEventHandler(object sender, TestConnectionCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class TestConnectionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal TestConnectionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591