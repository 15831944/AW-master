using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.Overview
{
    internal class LaytonLicesnseType
    {

    }

    internal class LaytonLicenseVerification
    {
        public static bool VerifyLicense(string key)
        {
      //      bool isRegistered = false;
      //      bool isTrial = false;
      //      bool trialPeriodDays = 14;
        
      //  connectionDB = New SQLConnection(ConnectionDBstr)
      //  connectionDB.open()
      //  sql = "select register_install_date, register_product_key, register_company_id, register_company_name, " & _
      //        "CAST(datepart(""mm"", register_install_date) As binary(1)) + " & _
      //        "CAST(datepart(""dd"", register_install_date) As binary(1)) +  " & _
      //        "CAST(datepart(""yyyy"", register_install_date) As binary(2)) As datekey1, " & _
      //        "CAST(register_install_datekey As binary(4)) As datekey2 " & _
      //        "from register"
      //  cmd = New SQLCommand(sql,ConnectionDB)
      //  dtr = cmd.ExecuteReader()
      //  If dtr.hasrows = true then
      //      While dtr.read()
      //         Dim keyGen As Object
      //         Dim installDate As DateTime
      //         Dim productKey As String
      //         Dim companyId As String
      //         Dim companyName As String
      //         Dim datekey1 As String
      //         Dim datekey2 As String
      //         Dim enc As New System.Text.ASCIIEncoding()
               
      //         installDate = dtr("register_install_date")
      //         datekey1 = enc.GetString(dtr("datekey1"))
      //         datekey2 = enc.GetString(dtr("datekey2"))
               
      //         If (IsDBNull(dtr("register_product_key")) = False) And _
      //            (IsDBNull(dtr("register_company_id")) = False) And _
      //            (IsDBNull(dtr("register_company_name")) = False) Then
                  
      //            productKey = dtr("register_product_key")
      //            companyId = dtr("register_company_id")
      //            companyName = dtr("register_company_name")
                  
      //            ' Check if the key is valid
      //            keyGen = CreateObject("LaytonLicenseGenerator.LaytonLicenseKey")
      //            ' sucessfully opened key generator...check if the key is valid
                  
      //            If KeyGen.IsValidKey (productKey, companyId, companyName) = 1 then
      //               ' a valid key...check if it is for ClickReport
      //               If KeyGen.ProductCode(productKey,256) = 1 Then
      //                  lbl_login.Text = "ClickReport Login For:<BR>" & companyName
      //                  lbl_registration.Text = ""
      //                  isRegistered = True
      //               Else
      //                  lbl_registration.Text = "The registration key you have entered is not for ClickReport.<BR><a href=""register.aspx"">Click Here</a> to register ClickReport"
      //               End If
      //            Else ' Invalid key
      //               lbl_registration.Text = "Your registration information is invalid.<BR><a href=""register.aspx"">Click Here</a> to register ClickReport"
      //            End If
                  
      //         Else ' Product is not registered
      //            ' Check if the product is still in the evaluation period
      //            Dim Now As DateTime = DateTime.Now
      //            Dim timeDiff As TimeSpan = Now.Subtract(installDate)
      //            If timeDiff.Days > trialPeriodDays Then
      //               ' Trial period is over 
      //               lbl_login.Text = "Your Trial Has Expired"
      //               lbl_registration.Text =  "<a href=""register.aspx"">Click Here</a> to register ClickReport"                    
      //            Else ' Still in trial period
      //               ' Check if the date matches the datekey
      //               If datekey1 = datekey2 Then
      //                  Dim daysRemaining = trialPeriodDays - timeDiff.Days
      //                  lbl_registration.Text = "You have " & daysRemaining & " days remaining in your trial period.<BR><a href=""register.aspx"">Click Here</a> to register ClickReport"
      //                  isTrial = True
      //               Else   
      //                  lbl_registration.Text = "Your registration information is invalid.<BR><a href=""register.aspx"">Click Here</a> to register ClickReport"
      //               End If
      //            End If
      //         End If
                    
      //      End while
      //      dtr.close
      //  Else
      //      lbl_registration.text = "ClickReport is missing registration information.<BR>Please contact support <BR>or<BR><a href=""register.aspx"">Click Here</a> to register ClickReport"
      //  End if
            
      //If (isRegistered = True) Or (isTrial = True) Then


            // Get the key from the settings
            return true;
        }
    }
}
