using System;
using Opc.Ua;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.IO;
using System.Formats.Asn1;
using Opc.Ua.Configuration;
using System.Collections.Generic;
using Opc.Ua.Client;
using Opc.Ua.Bindings;
using System.Threading;
using System.Linq;
using System.Text;
using Opc.Ua.Security.Certificates;
using System.Threading.Tasks;
using System.Xml;
using System.Globalization;


namespace opctest3
{
    class Program
    {
        
        static void Main(string[] args)
        {
            
            var cert = CertificateFactory.CreateCertificate("cn=anonimo")
            .SetNotBefore(new(2021,11,19))
            .SetNotAfter(new(2021,11,26))  
            .SetRSAKeySize(2048)
            .CreateForRSA()
            ;
            cert.GetRSAPrivateKey().ImportRSAPrivateKey(RSA.Create().ExportRSAPrivateKey(),out _);
              
            
            var tl = new CertificateTrustList { TrustedCertificates = new(){ new CertificateIdentifier { StoreType = "X509Store", StorePath= "CurrentUser\\CertificateAuthority" ,Certificate = new ( "C:\\ProgramData\\OPC Foundation\\CertificateStores\\MachineDefault\\certs\\SampleServer.der" )}}};
            var app = new ApplicationInstance(
                new ApplicationConfiguration
                {
                    ApplicationName = "Name",
                    ApplicationUri = "localhost:5000",
                    ApplicationType = ApplicationType.Client,
                    ClientConfiguration = new ClientConfiguration
                    {
                        DefaultSessionTimeout = 2 << 19,
                    },
                    SecurityConfiguration = new SecurityConfiguration
                    {
                        ApplicationCertificate = new() { StoreType = "X509Store", StorePath = "CurrentUser\\TrustedPeople", SubjectName = "cn=anonimo", Certificate = cert },
                        TrustedIssuerCertificates = new(){  StorePath = tl.TrustedCertificates[0].StorePath, StoreType= "X509Store" ,TrustedCertificates= tl.TrustedCertificates},
                        TrustedPeerCertificates = new(){  StorePath = tl.TrustedCertificates[0].StorePath, StoreType= "X509Store" ,TrustedCertificates= tl.TrustedCertificates},

                        AddAppCertToTrustedStore = true,
                    },
                    TransportConfigurations = new TransportConfigurationCollection(),
                    TransportQuotas = new TransportQuotas
                    {
                        MaxArrayLength = 2<<16,
                        MaxBufferSize = 2<<16,
                        MaxStringLength = 2<<20,
                        MaxByteStringLength = 2<<22,
                        MaxMessageSize = 2<<22,
                        OperationTimeout = 2<<16,
                        ChannelLifetime = 2<<16,
                        SecurityTokenLifetime = 2<<20,
                    },
                    
                })
            {
                ApplicationType = ApplicationType.Client,
                ApplicationName = "Name",
            };
            
            var l = app.ApplicationConfiguration.CreateMessageContext();
            app.ApplicationConfiguration.Validate(ApplicationType.Client);
            var id = new UserIdentity(cert);             
            
            Do(app,"test1",id,cert).Wait();
             
        }


        static async Task Do(ApplicationInstance inst,string name,UserIdentity id,X509Certificate2 cert){
            
            var s = new X509Store(StoreName.CertificateAuthority, StoreLocation.CurrentUser);
            s.Open(OpenFlags.ReadOnly);
            var c3 =s.Certificates.Find(X509FindType.FindBySubjectName,"SampleServer",false);
            s.Close();
            
           
            var desc = new EndpointDescription
            {
                EndpointUrl = "opc.tcp://laptop-q6bdecj5:26543/SampleServer",      
                SecurityMode = MessageSecurityMode.None,
                SecurityPolicyUri = "http://opcfoundation.org/UA/SecurityPolicy#None",
                UserIdentityTokens = new UserTokenPolicyCollection { new UserTokenPolicy(UserTokenType.Anonymous), new UserTokenPolicy(UserTokenType.Certificate), new UserTokenPolicy(UserTokenType.IssuedToken)},
            };
            var appdesc = new ApplicationDescription 
            {   
                ApplicationType = ApplicationType.Server, 
                ApplicationName = @"Name",
                ApplicationUri = "urn:localhost:ITSOPCCourseCode.OPCUA.SampleServer",
            };
            
            
            var setting = new TransportChannelSettings{
                ClientCertificate = cert,         
                Description = desc,
                Configuration  = new ()
                {
                        MaxArrayLength = 2<<16,
                        MaxBufferSize = 2<<16,
                        MaxStringLength = 2<<20,
                        MaxByteStringLength = 2<<22,
                        MaxMessageSize = 2<<22,
                        OperationTimeout = 2<<16,
                        ChannelLifetime = 2<<16,
                        UseBinaryEncoding = true,                       
                },
            };
            
            var ce = new ConfiguredEndpoint(appdesc,setting.Configuration)
            {
                EndpointUrl = new Uri("opc.tcp://laptop-q6bdecj5:26543/SampleServer"),
                UserIdentity = id.GetIdentityToken(),
            };
            ce.Description.UserIdentityTokens = desc.UserIdentityTokens;
            ce.Description.ServerCertificate = c3[0].RawData ;
            ce.Description.SecurityMode = MessageSecurityMode.SignAndEncrypt;
            ce.Description.SecurityLevel = 6;
            ce.SelectedUserTokenPolicyIndex = 1;
            ce.SelectedUserTokenPolicy = new UserTokenPolicy(UserTokenType.Certificate);
            ce.UpdateBeforeConnect = true;
            
            var ss = await Session.Create(inst.ApplicationConfiguration,ce,true,true,name,2<<8,id,new List<string>{CultureInfo.CurrentCulture.Name});
                                    
            return;
        }        
    }
}
