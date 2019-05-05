using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kuazoo.helper
{
    public static class Email
    {
        public static string CreateActivateAccountEmail(string url)
        {
            string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html>
<head>
        
        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
        
        <meta property=""og:title"" content=""*|MC:SUBJECT|*"" />
        
<title>*|MC:SUBJECT|*</title>
	
</head>
    <body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0"" style=""width: 100% !important; -webkit-text-size-adjust: none; margin-top: 0; margin-right: 0; margin-bottom: 0; margin-left: 0; padding-top: 0; padding-right: 0; padding-bottom: 0; padding-left: 0; background-color: #FFFFFF;"" bgcolor=""#FFFFFF"">
    	<center>
        	<table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""backgroundTable"" style=""height: 100% !important; margin-bottom: 0; margin-top: 0; margin-right: 0; margin-left: 0; padding-left: 0; padding-top: 0; padding-bottom: 0; padding-right: 0; width: 100% !important;"">
            	<tr>
                	<td align=""center"" valign=""top"">
                    	<table border=""0"" cellpadding=""20"" cellspacing=""0"" id=""templateContainer"" width=""760"" style=""font-family: Verdana; margin-top: 20px; font-size: 13px; border-top-color: #dddddd; border-right-color: #dddddd; border-bottom-color: #dddddd; border-left-color: #dddddd; border-top-style: solid; border-right-style: solid; border-bottom-style: solid; border-left-style: solid; border-top-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; background: #FAFAFA;"" bgcolor=""#FAFAFA"">
                        	<tr>
								<td colspan=""2"" style=""background:#00466a"">
									<a href=""http://www.kuazoo.com""><img src=""http://www.kuazoo.com/Content/img/logo.png"" width=""151px"" height=""27px""/></a>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:24px; color:#0093c9;font-weight:bold"">
									Thanks for Joining!
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:22px; color:#949494"">
									You're going to love it
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;"">
									Check your inbox every day to discover Kuazoo deals with huge discounts on ....
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;"">
									<div style=""width:100%; border-bottom:1px solid #dddddd""></div>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;"">
									To Complete your registration for Kuazoo, please verify your email
									<br/><br/><br/>
									<a href="""+ url + @""" style=""color: white;background: #0093c9;padding: 10px;text-decoration: none;"">Activate Account</a><br/><br/>
								</td>
							</tr>
                        </table>
                        <br />
                    </td>
                </tr>
				<tr>
					<td>
						<center>
							<div class=""sharedetail"" style=""display:table-cell"">
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-email.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-fb.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-twitter.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-pinterest.png"" style=""width:28px;height:28px""></a>
							</div>
						</center>
					</td>
				</tr>
				<tr>
					<td>
						<center style=""color:#707070;font-size:14px"">
							© 2014 Kuazoo
						</center>
					</td>
				</tr>
            </table>
        </center>
    
<style type=""text/css"">
body { width: 100% !important; }
body { -webkit-text-size-adjust: none !important; }
body { margin: 0 !important; padding: 0 !important; }
a{
	color:blue;
	text-decoration:none;
}
#backgroundTable { height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; }
body { background-color: #FFFFFF !important; }
</style>
</body>
</html>";
            return body;
        }

//        public static string CreatePurchaseEmail(string customer, string merchant, string orderno, DateTime transactiondate, decimal amount, string paymenttype, string url)
//        {
//            string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
//<html>
//<head>
//        
//        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
//        
//        <meta property=""og:title"" content=""*|MC:SUBJECT|*"" />
//        
//<title>*|MC:SUBJECT|*</title>
//	
//</head>
//    <body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0"" style=""width: 100% !important; -webkit-text-size-adjust: none; margin-top: 0; margin-right: 0; margin-bottom: 0; margin-left: 0; padding-top: 0; padding-right: 0; padding-bottom: 0; padding-left: 0; background-color: #FFFFFF;"" bgcolor=""#FFFFFF"">
//    	<center>
//        	<table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""backgroundTable"" style=""height: 100% !important; margin-bottom: 0; margin-top: 0; margin-right: 0; margin-left: 0; padding-left: 0; padding-top: 0; padding-bottom: 0; padding-right: 0; width: 100% !important;"">
//            	<tr>
//                	<td align=""center"" valign=""top"">
//                    	<table border=""0"" cellpadding=""20"" cellspacing=""0"" id=""templateContainer"" width=""760"" style=""font-family: Verdana; margin-top: 20px; font-size: 13px; border-top-color: #dddddd; border-right-color: #dddddd; border-bottom-color: #dddddd; border-left-color: #dddddd; border-top-style: solid; border-right-style: solid; border-bottom-style: solid; border-left-style: solid; border-top-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; background: #FAFAFA;"" bgcolor=""#FAFAFA"">
//                        	<tr>
//								<td colspan=""2"" style=""background:#00466a"">
//									<a href=""http://www.kuazoo.com""><img src=""http://www.kuazoo.com/Content/img/logo.png"" width=""151px"" height=""27px""/></a>
//								</td>
//							</tr>
//							<tr>
//								<td colspan=""2"" style=""font-size:24px; color:#0093c9;font-weight:bold"">
//									Dear "+customer+@"
//								</td>
//							</tr>
//							<tr>
//								<td colspan=""2"" style=""font-size:16px;"">
//									We are pleased to inform you that your online payment via <strong>"+paymenttype+@"</strong> is successful. Your credit card/bank account has been debited with MYR "+amount.ToString("N2") +@".
//								</td>
//							</tr>
//							<tr>
//								<td colspan=""2"" style=""font-size:16px;"">
//									Please note that <strong>"+merchant+@"</strong> will be listed in your credit card/bank statement for this transaction.
//								</td>
//							</tr>
//							<tr>
//								<td colspan=""2"" style=""font-size:16px;"">
//									<u>Transaction Detail</u><br/>
//									<table border=""0"" cellpadding=""5"" cellspacing=""0"">
//										<tr>
//											<td align=""right"" width=""250px"">
//												Order No :
//											</td>
//											<td>"+orderno+@"
//											</td>
//										</tr>
//										<tr>
//											<td align=""right"">
//												Transaction Date :
//											</td>
//											<td>" + string.Format("{0:dd-MMM-yyyy hh:mm:ss tt}",transactiondate) + @"
//											</td>
//										</tr>
//										<tr>
//											<td align=""right"">
//												Transaction Amount :
//											</td>
//											<td>MYR " + amount.ToString("N2") + @"
//											</td>
//										</tr>
//										<tr>
//											<td align=""right"">
//												Payment Type :
//											</td>
//											<td>" + paymenttype + @"
//											</td>
//										</tr>
//										<tr>
//											<td align=""right"" valign=""top"">
//												Product Description :
//											</td>
//											<td>" + merchant + @" <a href="""+url+@""">#"+orderno+ @"</a>
//											</td>
//										</tr>
//									</table>
//								</td>
//							</tr>
//							<tr>
//								<td colspan=""2"" style=""font-size:16px;"">
//									<div style=""width:100%; border-bottom:1px solid #dddddd""></div>
//								</td>
//							</tr>
//							<tr>
//								<td colspan=""2"" style=""font-size:14px; font-weight:bold"">
//									<u>Customer Support</u><br/>
//									If you have any questions about our product and services, <br/>
//									please contact Kuazoo directly at:<br/><br/>
//									Tel No: 04-6308283<br/>
//									Fax No: 04-6308288
//								</td>
//							</tr>
//                        </table>
//                        <br />
//                    </td>
//                </tr>
//				<tr>
//					<td>
//						<center>
//							<div class=""sharedetail"" style=""display:table-cell"">
//								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-email.png"" style=""width:28px;height:28px""></a>
//								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-fb.png"" style=""width:28px;height:28px""></a>
//								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-twitter.png"" style=""width:28px;height:28px""></a>
//								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-pinterest.png"" style=""width:28px;height:28px""></a>
//							</div>
//						</center>
//					</td>
//				</tr>
//				<tr>
//					<td>
//						<center style=""color:#707070;font-size:14px"">
//							© 2014 Kuazoo
//						</center>
//					</td>
//				</tr>
//            </table>
//        </center>
//    
//<style type=""text/css"">
//body { width: 100% !important; }
//body { -webkit-text-size-adjust: none !important; }
//body { margin: 0 !important; padding: 0 !important; }
//a{
//	color:blue;
//	text-decoration:none;
//}
//#backgroundTable { height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; }
//body { background-color: #FFFFFF !important; }
//</style>
//</body>
//</html>";
//            return body;
//        }

        public static string CreatePurchaseEmail(string customer, string merchant, string orderno, DateTime transactiondate, decimal amount, string paymenttype, string url, string dealname, string customeraddress, string sku, decimal price, decimal promo, string promocode, decimal kpoint)
        {
            string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html>
<head>
        
        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
        
        <meta property=""og:title"" content=""*|MC:SUBJECT|*"" />
        
<title>*|MC:SUBJECT|*</title>
	
</head>
    <body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0"" style=""width: 100% !important; -webkit-text-size-adjust: none; margin-top: 0; margin-right: 0; margin-bottom: 0; margin-left: 0; padding-top: 0; padding-right: 0; padding-bottom: 0; padding-left: 0; background-color: #FFFFFF;"" bgcolor=""#FFFFFF"">
    	<center>
        	<table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""backgroundTable"" style=""height: 100% !important; margin-bottom: 0; margin-top: 0; margin-right: 0; margin-left: 0; padding-left: 0; padding-top: 0; padding-bottom: 0; padding-right: 0; width: 100% !important;"">
            	<tr>
                	<td align=""center"" valign=""top"">
                    	<table border=""0"" cellpadding=""20"" cellspacing=""0"" id=""templateContainer"" width=""760"" style=""font-family: Verdana; margin-top: 20px; font-size: 13px; border-top-color: #dddddd; border-right-color: #dddddd; border-bottom-color: #dddddd; border-left-color: #dddddd; border-top-style: solid; border-right-style: solid; border-bottom-style: solid; border-left-style: solid; border-top-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; background: #FAFAFA;"" bgcolor=""#FAFAFA"">
                        	<tr>
								<td colspan=""2"" style=""background:#00466a"">
									<a href=""http://www.kuazoo.com""><img src=""http://www.kuazoo.com/Content/img/logo.png"" width=""151px"" height=""27px""/></a>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:28px; color:white;font-weight:bold;background:#4f81bd;border:2px solid #385e8b;"">
									YOUR ORDER HAS BEEN CONFIRMED!
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:24px; color:#0093c9;font-weight:bold"">
									Dear " + customer + @"
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;"">
									Thank you for shopping at Kuazoo! <br/>
									Your item will be delivered in 3- 5 business days! Here are the details of your order:
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px; color:white;font-weight:bold;background:#4f81bd;border:2px solid #385e8b;text-align:right;padding:10px;"">
									BILLING INFORMATION
								</td>
							</tr>
							<tr style=""vertical-align: top;"">
								<td style=""font-size:16px;width:50%"">
									<strong>Your Order Information:</strong><br/>
									Order Number: "+orderno+@"<br/>
									Order Total: RM " + amount.ToString("N2") + @"
								</td>
								<td style=""font-size:16px;width:50%"">
									<strong>Your Billing Information:</strong><br/>
									" + customeraddress + @"
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px; color:white;font-weight:bold;background:#4f81bd;border:2px solid #385e8b;text-align:right;padding:10px;"">
									ORDER INFORMATION
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;"">
									<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
										<tr style=""vertical-align:top"">
											<td style=""width:33.3%"">
												<strong>Item</strong><br/>
												" +merchant+@"<br/>
												"+dealname+@"<br/>
												"+sku+@"
											</td>
											<td style=""width:33.3%"">
												<strong>Payment method</strong><br/>
												" + paymenttype + @"
											</td>
											<td style=""width:33.3%"" align=""right"">
												<strong>Amount</strong><br/>
												" + price.ToString("N2") + @"
											</td>
										</tr>
										<tr>
											<td colspan=""3"">
												<div style=""width:100%; border-bottom:1px dashed #999""></div>
											</td>
										<tr>
										<tr style=""vertical-align:top;font-size:14px"">
											<td></td>
											<td></td>
											<td align=""right"">
												<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
													<tr style=""vertical-align:top"">
														<td width=""100px"">Shipping:</td>
														<td width=""100px"">FREE!</td>
													</tr>
													<tr style=""vertical-align:top"">
														<td>Promo Code:<br/>("+promocode+@")</td>
														<td>" + promo.ToString("N2") + @"</td>
													</tr>
													<tr style=""vertical-align:top"">
														<td>K-Point:</td>
														<td>" + kpoint.ToString("N2") + @"</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td colspan=""3"" style=""font-size:14px;"">
												<div style=""width:100%; border-bottom:2px solid black""></div>
											</td>
										</tr>
										<tr style=""vertical-align:top;font-size:14px"">
											<td></td>
											<td></td>
											<td align=""right"">
												<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%;font-weight:bold"">
													<tr style=""vertical-align:top"">
														<td width=""100px"">RM Total:</td>
														<td width=""100px"">" + amount.ToString("N2") + @"</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:14px;"">
									To access your account and view your order details, click <a href=""http://www.kuazoo.com"">here</a><br/>
									We hope you enjoy your purchase. Have a great day!

								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:14px;"">
									Sincerely,<br/>
									KUAZOO
								</td>
							</tr>
                        </table>
                        <br />
                    </td>
                </tr>
				<tr>
					<td>
						<center>
							<div class=""sharedetail"" style=""display:table-cell"">
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-email.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-fb.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-twitter.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-pinterest.png"" style=""width:28px;height:28px""></a>
							</div>
						</center>
					</td>
				</tr>
				<tr>
					<td>
						<center style=""color:#707070;font-size:14px"">
							© 2014 Kuazoo
						</center>
					</td>
				</tr>
            </table>
        </center>
    
<style type=""text/css"">
body { width: 100% !important; }
body { -webkit-text-size-adjust: none !important; }
body { margin: 0 !important; padding: 0 !important; }
a{
	color:blue;
	text-decoration:none;
}
#backgroundTable { height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; }
body { background-color: #FFFFFF !important; }
</style>
</body>
</html>";
            return body;
        }
        public static string CreatePurchaseServicesEmail(string customer, string merchant, string orderno, DateTime transactiondate, decimal amount, string paymenttype, string url, string dealname, string fineprint, string sku, decimal price, decimal promo, string promocode, decimal kpoint,string vouchercode)
        {
            string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html>
<head>
        
        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
        
        <meta property=""og:title"" content=""*|MC:SUBJECT|*"" />
        
<title>*|MC:SUBJECT|*</title>
	
</head>
    <body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0"" style=""width: 100% !important; -webkit-text-size-adjust: none; margin-top: 0; margin-right: 0; margin-bottom: 0; margin-left: 0; padding-top: 0; padding-right: 0; padding-bottom: 0; padding-left: 0; background-color: #FFFFFF;"" bgcolor=""#FFFFFF"">
    	<center>
        	<table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""backgroundTable"" style=""height: 100% !important; margin-bottom: 0; margin-top: 0; margin-right: 0; margin-left: 0; padding-left: 0; padding-top: 0; padding-bottom: 0; padding-right: 0; width: 100% !important;"">
            	<tr>
                	<td align=""center"" valign=""top"">
                    	<table border=""0"" cellpadding=""20"" cellspacing=""0"" id=""templateContainer"" width=""760"" style=""font-family: Verdana; margin-top: 20px; font-size: 13px; border-top-color: #dddddd; border-right-color: #dddddd; border-bottom-color: #dddddd; border-left-color: #dddddd; border-top-style: solid; border-right-style: solid; border-bottom-style: solid; border-left-style: solid; border-top-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; background: #FAFAFA;"" bgcolor=""#FAFAFA"">
                        	<tr>
								<td colspan=""2"" style=""background:#00466a"">
									<a href=""http://www.kuazoo.com""><img src=""http://www.kuazoo.com/Content/img/logo.png"" width=""151px"" height=""27px""/></a>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:28px; color:white;font-weight:bold;background:#4f81bd;border:2px solid #385e8b;"">
									YOUR ORDER HAS BEEN CONFIRMED!
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:24px; color:#0093c9;font-weight:bold"">
									Dear " + customer + @"
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;"">
									Thank you for shopping at Kuazoo! <br/>
									Here are the details of your order:
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px; color:white;font-weight:bold;background:#4f81bd;border:2px solid #385e8b;text-align:right;padding:10px;"">
									HOW TO REDEEM YOUR VOUCHER
								</td>
							</tr>
							<tr style=""vertical-align: top;"">
								<td style=""font-size:16px;width:40%"">
									<strong>Voucher code:</strong><br/>
									<ul>
									<li>Print the voucher code</li>
									<li>You just need to follow the instructions</li>
									<li>Enjoy you purchase!</li>
									</ul>
								</td>
								<td style=""font-size:16px;width:60%"">
									" + fineprint + @"
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px; color:white;font-weight:bold;background:#4f81bd;border:2px solid #385e8b;text-align:right;padding:10px;"">
									VOUCHER CODE
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px; text-align:left;"">
									" + vouchercode + @"
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px; color:white;font-weight:bold;background:#4f81bd;border:2px solid #385e8b;text-align:right;padding:10px;"">
									ORDER INFORMATION
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;"">
									<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
										<tr style=""vertical-align:top"">
											<td style=""width:33.3%"">
												<strong>Item</strong><br/>
												" + merchant + @"<br/>
												" + dealname + @"<br/>
												" + sku + @"
											</td>
											<td style=""width:33.3%"">
												<strong>Payment method</strong><br/>
												" + paymenttype + @"
											</td>
											<td style=""width:33.3%"" align=""right"">
												<strong>Amount</strong><br/>
												" + price.ToString("N2") + @"
											</td>
										</tr>
										<tr>
											<td colspan=""3"">
												<div style=""width:100%; border-bottom:1px dashed #999""></div>
											</td>
										<tr>
										<tr style=""vertical-align:top;font-size:14px"">
											<td></td>
											<td></td>
											<td align=""right"">
												<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
													<tr style=""vertical-align:top"">
														<td width=""100px"">Shipping:</td>
														<td width=""100px"">FREE!</td>
													</tr>
													<tr style=""vertical-align:top"">
														<td>Promo Code:<br/>(" + promocode + @")</td>
														<td>" + promo.ToString("N2") + @"</td>
													</tr>
													<tr style=""vertical-align:top"">
														<td>K-Point:</td>
														<td>" + kpoint.ToString("N2") + @"</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td colspan=""3"" style=""font-size:14px;"">
												<div style=""width:100%; border-bottom:2px solid black""></div>
											</td>
										</tr>
										<tr style=""vertical-align:top;font-size:14px"">
											<td></td>
											<td></td>
											<td align=""right"">
												<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%;font-weight:bold"">
													<tr style=""vertical-align:top"">
														<td width=""100px"">RM Total:</td>
														<td width=""100px"">" + amount.ToString("N2") + @"</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:14px;"">
									To access your account and view your order details, click <a href=""http://www.kuazoo.com"">here</a><br/>
									We hope you enjoy your purchase. Have a great day!

								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:14px;"">
									Sincerely,<br/>
									KUAZOO
								</td>
							</tr>
                        </table>
                        <br />
                    </td>
                </tr>
				<tr>
					<td>
						<center>
							<div class=""sharedetail"" style=""display:table-cell"">
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-email.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-fb.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-twitter.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-pinterest.png"" style=""width:28px;height:28px""></a>
							</div>
						</center>
					</td>
				</tr>
				<tr>
					<td>
						<center style=""color:#707070;font-size:14px"">
							© 2014 Kuazoo
						</center>
					</td>
				</tr>
            </table>
        </center>
    
<style type=""text/css"">
body { width: 100% !important; }
body { -webkit-text-size-adjust: none !important; }
body { margin: 0 !important; padding: 0 !important; }
a{
	color:blue;
	text-decoration:none;
}
#backgroundTable { height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; }
body { background-color: #FFFFFF !important; }
</style>
</body>
</html>";
            return body;
        }

        public static string CreatePurchaseEmailV2(string customer, string merchant, string orderno, DateTime transactiondate, decimal amount, string paymenttype, string url, string dealname, string customeraddress, string sku, decimal price, decimal promo, string promocode, decimal kpoint, string dealinterest, string mailto, string imageurl, string variname)
        {
            string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html>
<head>
        
        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
        
        <meta property=""og:title"" content=""*|MC:SUBJECT|*"" />
        
<title>*|MC:SUBJECT|*</title>
	
</head>
    <body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0"" style=""width: 100% !important; -webkit-text-size-adjust: none; margin-top: 0; margin-right: 0; margin-bottom: 0; margin-left: 0; padding-top: 0; padding-right: 0; padding-bottom: 0; padding-left: 0; background-color: #FFFFFF;font-family:Arial;"" bgcolor=""#FFFFFF"">
    	<center>
        	<table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""backgroundTable"" style=""height: 100% !important; margin-bottom: 0; margin-top: 0; margin-right: 0; margin-left: 0; padding-left: 0; padding-top: 0; padding-bottom: 0; padding-right: 0; width: 100% !important;"">
            	<tr>
                	<td align=""center"" valign=""top"">
                    	<table border=""0"" cellpadding=""20"" cellspacing=""0"" id=""templateContainer"" width=""760"" style=""font-family: Verdana; margin-top: 20px; font-size: 13px; border-top-color: #dddddd; border-right-color: #dddddd; border-bottom-color: #dddddd; border-left-color: #dddddd; border-top-style: solid; border-right-style: solid; border-bottom-style: solid; border-left-style: solid; border-top-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; background: #FAFAFA;"" bgcolor=""#FAFAFA"">
                        	<tr>
								<td colspan=""2"" style=""background:#00466a"">
									<a href=""http://www.kuazoo.com""><img src=""http://www.kuazoo.com/Content/img/logo.png"" width=""151px"" height=""27px""/></a>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:30px;color:#666666;font-family:Arial; "">
									Thank you for shopping at Kuazoo! <br/>
									<span style=""font-size:13px;"">Your item will be delivered in 3- 5 business days! Here are the details of your order:</span>
								</td>
							</tr>
							<tr style=""vertical-align: top;"">
								<td style=""font-size:13px;width:70%;color:#333333"">
									<span style=""color:#00466a;font-weight:bold"">Your Order Information</span><br/>
									<strong>Order Number:</strong> " + orderno + @"<br/>
									<strong>Order Date:</strong> " + string.Format("{0:dd MMM yyyy}", transactiondate) + @"<br/>
									<strong>Order Total:</strong> RM " + amount.ToString("N2") + @"<br/>
									<strong>Payment Method:</strong> " + paymenttype + @"
								</td>
								<td style=""font-size:13px;width:30%;border-left:1px solid #dddddd;"">
									<span style=""color:#00466a;font-weight:bold"">Your Billing Information</span><br/>
									" +customeraddress+@"
								</td>
							</tr>
							<tr style=""vertical-align: top;"">
								<td colspan=""2"" style=""font-size:13px;"">
									<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
										<tr style=""vertical-align:top"">
											<td style=""font-size:13px;width:160px;border-bottom:1px solid #dddddd;"">
												<img src=""" + imageurl + @""" alt=""Deal Image"" style=""width:160px"">
											</td>
											<td style=""font-size:13px;border-bottom:1px solid #dddddd;"">
										<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
											<tr>
												<td style=""font-size:14px; text-align:left; font-weight:bold;color:#333333;background:#dddddd"">
													Deal Name 
												</td>
												<td style=""font-size:14px; text-align:left;font-weight:bold;color:#333333;background:#dddddd"">
													Brand
												</td>
												<td style=""font-size:14px;text-align:right; font-weight:bold;color:#333333;background:#dddddd"">
													Deal Amount (RM)
												</td>
											</tr>
											<tr style=""border-bottom:1px solid #dddddd"">
												<td style=""font-size:13px;color:#333333;text-align:left;border-bottom:1px solid #dddddd"">
													" +dealname+@" <br/>"
                                                    + variname + @" <br/>
													SKU : "+sku+@"
												</td>
												<td style=""font-size:13px;color:#333333;text-align:left;border-bottom:1px solid #dddddd;vertical-align:top"">
													"+merchant+ @"
												</td>
												<td style=""font-size:13px;color:#333333;text-align:right;border-bottom:1px solid #dddddd;vertical-align:top"">
												    " + price.ToString("N2")+@"
												</td>
											</tr>
											<tr>
												<td style=""font-size:13px;color:#333333;text-align:left;border-bottom:1px solid #dddddd"">
												</td>
												<td style=""font-size:13px;color:#333333;text-align:left;border-bottom:1px solid #dddddd"">
												</td>
												<td style=""font-size:13px;color:#333333;text-align:right;border-bottom:1px solid #dddddd"">
													<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
														<tr>
															<td style=""text-align:left"">Shipping</td>
															<td style=""text-align:right"">FREE</td>
														</tr>
														<tr>
															<td style=""text-align:left"">Voucher<br/>("+promocode+@")</td>
															<td style=""text-align:right;vertical-align:top"">"+promo.ToString("N2")+@"</td>
														</tr>
														<tr>
															<td style=""text-align:left"">K-Point</td>
															<td style=""text-align:right"">"+kpoint.ToString("N2")+@"</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td style=""font-size:13px;color:#333333;text-align:left;"">
												</td>
												<td style=""font-size:13px;color:#333333;text-align:left;"">
												</td>
												<td style=""font-size:13px;font-weight:bold;color:#333333;text-align:right;"">
													<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
														<tr>
															<td style=""text-align:left"">Total (RM)</td>
															<td style=""text-align:right"">"+amount.ToString("N2")+@"</td>
														</tr>
													</table>
												</td>
											</tr>
										</table>
												
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;"">
									To access your account and view your order details, click <a href=""http://www.kuazoo.com"">here</a><br/>
									We hope you enjoy your purchase. Have a great day!
									<br/><br/>
									<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
										<tr>
											<td style=""width:100px"">
												<img src=""http://www.kuazoo.com.my/Content/img/kuazoo-monkey.png"" style=""width:100px"">
											</td>
											<td style=""font-size:13px;color:#0091c7;vertical-align:middle;"">
												<span style=""font-size:16px;font-weight:bold;color:#00466a"">KUAZOO</span><br/><br/>
												customerservice@kuazoo.com<br/>
												+603-20274708<br/>
												www.kuazoo.com
											</td>
										</tr>	
									</table>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:30px;color:#666666;font-family:Arial;padding-bottom:0px "">
								<br/>
									You may be interested in these
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;color:#333333;font-family:Arial; "">
									<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
										"+dealinterest+ @"
									</table>
								</td>
							</tr>
                        </table>
                        <br />
                    </td>
                </tr>
				<tr>
					<td>
						<center>
							<div class=""sharedetail"" style=""display:table-cell"">
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-email.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-fb.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-twitter.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-pinterest.png"" style=""width:28px;height:28px""></a>
							</div>
						</center>
					</td>
				</tr>
				<tr>
					<td>
						<center style=""color:#666666;font-size:11px"">
							You are receiving this with your email address <span style=""color:#0080b1"">" + mailto + @"</span><br/>
							To ensure our emails don't end up in your spam, add our email to your address book.
							<br/><br/><br/>
						</center>
					</td>
				</tr>
            </table>
        </center>
    
<style type=""text/css"">
body { width: 100% !important; }
body { -webkit-text-size-adjust: none !important; }
body { margin: 0 !important; padding: 0 !important; }
a{
	color:blue;
	text-decoration:none;
}
#backgroundTable { height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; }
body { background-color: #FFFFFF !important; }
</style>
</body>
</html>";
            return body;
        }
        public static string CreatePurchaseEmailFree(string customer, string merchant, string orderno, DateTime transactiondate, string url, string dealname, string customeraddress, string sku, string dealinterest, string mailto, string imageurl, string variancename)
        {
            string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html>
<head>
        
        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
        
        <meta property=""og:title"" content=""*|MC:SUBJECT|*"" />
        
<title>*|MC:SUBJECT|*</title>
	
</head>
    <body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0"" style=""width: 100% !important; -webkit-text-size-adjust: none; margin-top: 0; margin-right: 0; margin-bottom: 0; margin-left: 0; padding-top: 0; padding-right: 0; padding-bottom: 0; padding-left: 0; background-color: #FFFFFF;font-family:Arial;"" bgcolor=""#FFFFFF"">
    	<center>
        	<table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""backgroundTable"" style=""height: 100% !important; margin-bottom: 0; margin-top: 0; margin-right: 0; margin-left: 0; padding-left: 0; padding-top: 0; padding-bottom: 0; padding-right: 0; width: 100% !important;"">
            	<tr>
                	<td align=""center"" valign=""top"">
                    	<table border=""0"" cellpadding=""20"" cellspacing=""0"" id=""templateContainer"" width=""760"" style=""font-family: Verdana; margin-top: 20px; font-size: 13px; border-top-color: #dddddd; border-right-color: #dddddd; border-bottom-color: #dddddd; border-left-color: #dddddd; border-top-style: solid; border-right-style: solid; border-bottom-style: solid; border-left-style: solid; border-top-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; background: #FAFAFA;"" bgcolor=""#FAFAFA"">
                        	<tr>
								<td colspan=""2"" style=""background:#00466a"">
									<a href=""http://www.kuazoo.com""><img src=""http://www.kuazoo.com/Content/img/logo.png"" width=""151px"" height=""27px""/></a>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:30px;color:#666666;font-family:Arial; "">
									Thank you for shopping at Kuazoo! <br/>
									<span style=""font-size:13px;"">Your item will be delivered in 3- 5 business days! Here are the details of your order:</span>
								</td>
							</tr>
							<tr style=""vertical-align: top;"">
								<td style=""font-size:13px;width:70%;color:#333333"">
									<span style=""color:#00466a;font-weight:bold"">Your Order Information</span><br/>
									<strong>Order Number:</strong> " + orderno + @"<br/>
									<strong>Order Date:</strong> " + string.Format("{0:dd MMM yyyy}", transactiondate) + @"<br/>
									<strong>Order Total:</strong> FREE
								</td>
								<td style=""font-size:13px;width:30%;border-left:1px solid #dddddd;"">
									<span style=""color:#00466a;font-weight:bold"">Your Billing Information</span><br/>
									" + customeraddress + @"
								</td>
							</tr>
							<tr style=""vertical-align: top;"">
								<td colspan=""2"" style=""font-size:13px;"">
									<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
										<tr style=""vertical-align:top"">
											<td style=""font-size:13px;width:160px;border-bottom:1px solid #dddddd;"">
												<img src=""" + imageurl + @""" alt=""Deal Image"" style=""width:160px"">
											</td>
											<td style=""font-size:13px;border-bottom:1px solid #dddddd;"">
										<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
											<tr>
												<td style=""font-size:14px; text-align:left; font-weight:bold;color:#333333;background:#dddddd"">
													Deal Name 
												</td>
												<td style=""font-size:14px; text-align:left;font-weight:bold;color:#333333;background:#dddddd"">
													Brand
												</td>
												<td style=""font-size:14px;text-align:right; font-weight:bold;color:#333333;background:#dddddd"">
													Deal Amount (RM)
												</td>
											</tr>
											<tr style=""border-bottom:1px solid #dddddd"">
												<td style=""font-size:13px;color:#333333;text-align:left;border-bottom:1px solid #dddddd"">
													" + dealname + @" <br/>"
                                                    + variancename + @" <br/>
													SKU : " + sku + @"
												</td>
												<td style=""font-size:13px;color:#333333;text-align:left;border-bottom:1px solid #dddddd;vertical-align:top"">
													" + merchant + @"
												</td>
												<td style=""font-size:13px;color:#333333;text-align:right;border-bottom:1px solid #dddddd;vertical-align:top"">
												    FREE
												</td>
											</tr>
										</table>
												
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;"">
									To access your account and view your order details, click <a href=""http://www.kuazoo.com"">here</a><br/>
									We hope you enjoy your purchase. Have a great day!
									<br/><br/>
									<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
										<tr>
											<td style=""width:100px"">
												<img src=""http://www.kuazoo.com.my/Content/img/kuazoo-monkey.png"" style=""width:100px"">
											</td>
											<td style=""font-size:13px;color:#0091c7;vertical-align:middle;"">
												<span style=""font-size:16px;font-weight:bold;color:#00466a"">KUAZOO</span><br/><br/>
												customerservice@kuazoo.com<br/>
												+603-20274708<br/>
												www.kuazoo.com
											</td>
										</tr>	
									</table>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:30px;color:#666666;font-family:Arial;padding-bottom:0px "">
								<br/>
									You may be interested in these
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;color:#333333;font-family:Arial; "">
									<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
										" + dealinterest + @"
									</table>
								</td>
							</tr>
                        </table>
                        <br />
                    </td>
                </tr>
				<tr>
					<td>
						<center>
							<div class=""sharedetail"" style=""display:table-cell"">
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-email.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-fb.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-twitter.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-pinterest.png"" style=""width:28px;height:28px""></a>
							</div>
						</center>
					</td>
				</tr>
				<tr>
					<td>
						<center style=""color:#666666;font-size:11px"">
							You are receiving this with your email address <span style=""color:#0080b1"">" + mailto + @"</span><br/>
							To ensure our emails don't end up in your spam, add our email to your address book.
							<br/><br/><br/>
						</center>
					</td>
				</tr>
            </table>
        </center>
    
<style type=""text/css"">
body { width: 100% !important; }
body { -webkit-text-size-adjust: none !important; }
body { margin: 0 !important; padding: 0 !important; }
a{
	color:blue;
	text-decoration:none;
}
#backgroundTable { height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; }
body { background-color: #FFFFFF !important; }
</style>
</body>
</html>";
            return body;
        }
        public static string CreatePurchaseServicesEmailV2(string customer, string merchant, string orderno, DateTime transactiondate, decimal amount, string paymenttype, string url, string dealname, string fineprint, string sku, decimal price, decimal promo, string promocode, decimal kpoint, string vouchercode, string dealinterest, string mailto, string imageurl, string variancename)
        {
            string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html>
<head>
        
        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
        
        <meta property=""og:title"" content=""*|MC:SUBJECT|*"" />
        
<title>*|MC:SUBJECT|*</title>
	
</head>
    <body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0"" style=""width: 100% !important; -webkit-text-size-adjust: none; margin-top: 0; margin-right: 0; margin-bottom: 0; margin-left: 0; padding-top: 0; padding-right: 0; padding-bottom: 0; padding-left: 0; background-color: #FFFFFF;font-family:Arial;"" bgcolor=""#FFFFFF"">
    	<center>
        	<table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""backgroundTable"" style=""height: 100% !important; margin-bottom: 0; margin-top: 0; margin-right: 0; margin-left: 0; padding-left: 0; padding-top: 0; padding-bottom: 0; padding-right: 0; width: 100% !important;"">
            	<tr>
                	<td align=""center"" valign=""top"">
                    	<table border=""0"" cellpadding=""20"" cellspacing=""0"" id=""templateContainer"" width=""760"" style=""font-family: Verdana; margin-top: 20px; font-size: 13px; border-top-color: #dddddd; border-right-color: #dddddd; border-bottom-color: #dddddd; border-left-color: #dddddd; border-top-style: solid; border-right-style: solid; border-bottom-style: solid; border-left-style: solid; border-top-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; background: #FAFAFA;"" bgcolor=""#FAFAFA"">
                        	<tr>
								<td colspan=""2"" style=""background:#00466a"">
									<a href=""http://www.kuazoo.com""><img src=""http://www.kuazoo.com/Content/img/logo.png"" width=""151px"" height=""27px""/></a>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:30px;color:#666666;font-family:Arial; "">
									Thank you for shopping at Kuazoo! <br/>
									<span style=""font-size:13px;"">Here are the details of your order:</span>
								</td>
							</tr>
							<tr style=""vertical-align: top;"">
								<td style=""font-size:13px;width:35%;color:#333333"">
									<span style=""color:#00466a;font-weight:bold"">Your Order Information</span><br/>
									<strong>Order Number:</strong> " + orderno + @"<br/>
									<strong>Order Date:</strong> " + string.Format("{0:dd MMM yyyy}", transactiondate) + @"<br/>
									<strong>Order Total:</strong> RM " + amount.ToString("N2") + @"<br/>
									<strong>Payment Method:</strong> " + paymenttype + @"
								</td>
								<td style=""font-size:13px;width:65%;border-left:1px solid #dddddd;"">
									<span style=""color:#00466a;font-weight:bold"">Instructions</span><br/>
									All you need to do is print the voucher code and follow the instructions. Enjoy your purchase!
									<br/><br/>
									<span style=""color:#00466a;font-weight:bold"">Voucher Fine print</span><br/>
									" +fineprint+ @"
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""background:#eeeeee;"">
									<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%; color:#333333;font-size;13px;"">
										<tr>
											<td colspan=""3"" style=""color:#00466a;font-size:13px;font-weight:bold;"">
												Your Voucher Code(s)
											</td>
										</tr>
										" + vouchercode+@"
									</table>
								</td>
							</tr>
							<tr style=""vertical-align: top;"">
								<td colspan=""2"" style=""font-size:13px;"">
									<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
										<tr style=""vertical-align:top"">
											<td style=""font-size:13px;width:160px;border-bottom:1px solid #dddddd;"">
												<img src="""+imageurl+@""" alt=""Deal Image"" style=""width:160px"">
											</td>
											<td style=""font-size:13px;border-bottom:1px solid #dddddd;"">
										<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
											<tr>
												<td style=""font-size:14px; text-align:left; font-weight:bold;color:#333333;background:#dddddd"">
													Deal Name 
												</td>
												<td style=""font-size:14px; text-align:left;font-weight:bold;color:#333333;background:#dddddd"">
													Brand
												</td>
												<td style=""font-size:14px;text-align:right; font-weight:bold;color:#333333;background:#dddddd"">
													Deal Amount (RM)
												</td>
											</tr>
											<tr style=""border-bottom:1px solid #dddddd"">
												<td style=""font-size:13px;color:#333333;text-align:left;border-bottom:1px solid #dddddd"">
													" + dealname + @" <br/>"
                                                    + variancename + @" <br/>
													SKU : " + sku + @"
												</td>
												<td style=""font-size:13px;color:#333333;text-align:left;border-bottom:1px solid #dddddd;vertical-align:top"">
													" + merchant + @"
												</td>
												<td style=""font-size:13px;color:#333333;text-align:right;border-bottom:1px solid #dddddd;vertical-align:top"">
												    " + price.ToString("N2") + @"
												</td>
											</tr>
											<tr>
												<td style=""font-size:13px;color:#333333;text-align:left;border-bottom:1px solid #dddddd"">
												</td>
												<td style=""font-size:13px;color:#333333;text-align:left;border-bottom:1px solid #dddddd"">
												</td>
												<td style=""font-size:13px;color:#333333;text-align:right;border-bottom:1px solid #dddddd"">
													<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
														<tr>
															<td style=""text-align:left"">Shipping</td>
															<td style=""text-align:right"">FREE</td>
														</tr>
														<tr>
															<td style=""text-align:left"">Voucher<br/>(" + promocode + @")</td>
															<td style=""text-align:right;vertical-align:top"">" + promo.ToString("N2") + @"</td>
														</tr>
														<tr>
															<td style=""text-align:left"">K-Point</td>
															<td style=""text-align:right"">" + kpoint.ToString("N2") + @"</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td style=""font-size:13px;color:#333333;text-align:left;"">
												</td>
												<td style=""font-size:13px;color:#333333;text-align:left;"">
												</td>
												<td style=""font-size:13px;font-weight:bold;color:#333333;text-align:right;"">
													<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
														<tr>
															<td style=""text-align:left"">Total (RM)</td>
															<td style=""text-align:right"">" + amount.ToString("N2") + @"</td>
														</tr>
													</table>
												</td>
											</tr>
										</table>
												
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;"">
									To access your account and view your order details, click <a href=""http://www.kuazoo.com"">here</a><br/>
									We hope you enjoy your purchase. Have a great day!
									<br/><br/>
									<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
										<tr>
											<td style=""width:100px"">
												<img src=""http://www.kuazoo.com.my/Content/img/kuazoo-monkey.png"" style=""width:100px"">
											</td>
											<td style=""font-size:13px;color:#0091c7;vertical-align:middle;"">
												<span style=""font-size:16px;font-weight:bold;color:#00466a"">KUAZOO</span><br/><br/>
												customerservice@kuazoo.com<br/>
												+603-20274708<br/>
												www.kuazoo.com
											</td>
										</tr>	
									</table>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:30px;color:#666666;font-family:Arial;padding-bottom:0px "">
								<br/>
									You may be interested in these
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;color:#333333;font-family:Arial; "">
									<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
										" + dealinterest + @"
									</table>
								</td>
							</tr>
                        </table>
                        <br />
                    </td>
                </tr>
				<tr>
					<td>
						<center>
							<div class=""sharedetail"" style=""display:table-cell"">
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-email.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-fb.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-twitter.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-pinterest.png"" style=""width:28px;height:28px""></a>
							</div>
						</center>
					</td>
				</tr>
				<tr>
					<td>
						<center style=""color:#666666;font-size:11px"">
							You are receiving this with your email address <span style=""color:#0080b1"">" + mailto + @"</span><br/>
							To ensure our emails don't end up in your spam, add our email to your address book.
							<br/><br/><br/>
						</center>
					</td>
				</tr>
            </table>
        </center>
    
<style type=""text/css"">
body { width: 100% !important; }
body { -webkit-text-size-adjust: none !important; }
body { margin: 0 !important; padding: 0 !important; }
a{
	color:blue;
	text-decoration:none;
}
#backgroundTable { height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; }
body { background-color: #FFFFFF !important; }
</style>
</body>
</html>";
            return body;
        }
        public static string CreatePurchaseServicesEmailFREE(string customer, string merchant, string orderno, DateTime transactiondate, string url, string dealname, string fineprint, string sku, string vouchercode, string dealinterest, string mailto, string imageurl, string variancename)
        {
            string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html>
<head>
        
        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
        
        <meta property=""og:title"" content=""*|MC:SUBJECT|*"" />
        
<title>*|MC:SUBJECT|*</title>
	
</head>
    <body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0"" style=""width: 100% !important; -webkit-text-size-adjust: none; margin-top: 0; margin-right: 0; margin-bottom: 0; margin-left: 0; padding-top: 0; padding-right: 0; padding-bottom: 0; padding-left: 0; background-color: #FFFFFF;font-family:Arial;"" bgcolor=""#FFFFFF"">
    	<center>
        	<table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""backgroundTable"" style=""height: 100% !important; margin-bottom: 0; margin-top: 0; margin-right: 0; margin-left: 0; padding-left: 0; padding-top: 0; padding-bottom: 0; padding-right: 0; width: 100% !important;"">
            	<tr>
                	<td align=""center"" valign=""top"">
                    	<table border=""0"" cellpadding=""20"" cellspacing=""0"" id=""templateContainer"" width=""760"" style=""font-family: Verdana; margin-top: 20px; font-size: 13px; border-top-color: #dddddd; border-right-color: #dddddd; border-bottom-color: #dddddd; border-left-color: #dddddd; border-top-style: solid; border-right-style: solid; border-bottom-style: solid; border-left-style: solid; border-top-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; background: #FAFAFA;"" bgcolor=""#FAFAFA"">
                        	<tr>
								<td colspan=""2"" style=""background:#00466a"">
									<a href=""http://www.kuazoo.com""><img src=""http://www.kuazoo.com/Content/img/logo.png"" width=""151px"" height=""27px""/></a>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:30px;color:#666666;font-family:Arial; "">
									Thank you for shopping at Kuazoo! <br/>
									<span style=""font-size:13px;"">Here are the details of your order:</span>
								</td>
							</tr>
							<tr style=""vertical-align: top;"">
								<td style=""font-size:13px;width:35%;color:#333333"">
									<span style=""color:#00466a;font-weight:bold"">Your Order Information</span><br/>
									<strong>Order Number:</strong> " + orderno + @"<br/>
									<strong>Order Date:</strong> " + string.Format("{0:dd MMM yyyy}", transactiondate) + @"<br/>
									<strong>Order Total:</strong> FREE
								</td>
								<td style=""font-size:13px;width:65%;border-left:1px solid #dddddd;"">
									<span style=""color:#00466a;font-weight:bold"">Instructions</span><br/>
									All you need to do is print the voucher code and follow the instructions. Enjoy your purchase!
									<br/><br/>
									<span style=""color:#00466a;font-weight:bold"">Voucher Fine print</span><br/>
									" + fineprint + @"
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""background:#eeeeee;"">
									<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%; color:#333333;font-size;13px;"">
										<tr>
											<td colspan=""3"" style=""color:#00466a;font-size:13px;font-weight:bold;"">
												Your Voucher Code(s)
											</td>
										</tr>
										" + vouchercode + @"
									</table>
								</td>
							</tr>
							<tr style=""vertical-align: top;"">
								<td colspan=""2"" style=""font-size:13px;"">
									<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
										<tr style=""vertical-align:top"">
											<td style=""font-size:13px;width:160px;border-bottom:1px solid #dddddd;"">
												<img src=""" + imageurl + @""" alt=""Deal Image"" style=""width:160px"">
											</td>
											<td style=""font-size:13px;border-bottom:1px solid #dddddd;"">
										<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
											<tr>
												<td style=""font-size:14px; text-align:left; font-weight:bold;color:#333333;background:#dddddd"">
													Deal Name 
												</td>
												<td style=""font-size:14px; text-align:left;font-weight:bold;color:#333333;background:#dddddd"">
													Brand
												</td>
												<td style=""font-size:14px;text-align:right; font-weight:bold;color:#333333;background:#dddddd"">
													Deal Amount (RM)
												</td>
											</tr>
											<tr style=""border-bottom:1px solid #dddddd"">
												<td style=""font-size:13px;color:#333333;text-align:left;border-bottom:1px solid #dddddd"">
													" + dealname + @" <br/>"
                                                    + variancename + @" <br/>
													SKU : " + sku + @"
												</td>
												<td style=""font-size:13px;color:#333333;text-align:left;border-bottom:1px solid #dddddd;vertical-align:top"">
													" + merchant + @"
												</td>
												<td style=""font-size:13px;color:#333333;text-align:right;border-bottom:1px solid #dddddd;vertical-align:top"">
												    FREE
												</td>
											</tr>
										</table>
												
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;"">
									To access your account and view your order details, click <a href=""http://www.kuazoo.com"">here</a><br/>
									We hope you enjoy your purchase. Have a great day!
									<br/><br/>
									<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
										<tr>
											<td style=""width:100px"">
												<img src=""http://www.kuazoo.com.my/Content/img/kuazoo-monkey.png"" style=""width:100px"">
											</td>
											<td style=""font-size:13px;color:#0091c7;vertical-align:middle;"">
												<span style=""font-size:16px;font-weight:bold;color:#00466a"">KUAZOO</span><br/><br/>
												customerservice@kuazoo.com<br/>
												+603-20274708<br/>
												www.kuazoo.com
											</td>
										</tr>	
									</table>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:30px;color:#666666;font-family:Arial;padding-bottom:0px "">
								<br/>
									You may be interested in these
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;color:#333333;font-family:Arial; "">
									<table border=""0"" cellpadding=""5"" cellspacing=""0"" style=""width:100%"">
										" + dealinterest + @"
									</table>
								</td>
							</tr>
                        </table>
                        <br />
                    </td>
                </tr>
				<tr>
					<td>
						<center>
							<div class=""sharedetail"" style=""display:table-cell"">
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-email.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-fb.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-twitter.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-pinterest.png"" style=""width:28px;height:28px""></a>
							</div>
						</center>
					</td>
				</tr>
				<tr>
					<td>
						<center style=""color:#666666;font-size:11px"">
							You are receiving this with your email address <span style=""color:#0080b1"">" + mailto + @"</span><br/>
							To ensure our emails don't end up in your spam, add our email to your address book.
							<br/><br/><br/>
						</center>
					</td>
				</tr>
            </table>
        </center>
    
<style type=""text/css"">
body { width: 100% !important; }
body { -webkit-text-size-adjust: none !important; }
body { margin: 0 !important; padding: 0 !important; }
a{
	color:blue;
	text-decoration:none;
}
#backgroundTable { height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; }
body { background-color: #FFFFFF !important; }
</style>
</body>
</html>";
            return body;
        }

        public static string CreateResetPasswordEmail(string name, string url)
        {
            string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html>
<head>
        
        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
        
        <meta property=""og:title"" content=""*|MC:SUBJECT|*"" />
        
<title>*|MC:SUBJECT|*</title>
	
</head>
    <body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0"" style=""width: 100% !important; -webkit-text-size-adjust: none; margin-top: 0; margin-right: 0; margin-bottom: 0; margin-left: 0; padding-top: 0; padding-right: 0; padding-bottom: 0; padding-left: 0; background-color: #FFFFFF;"" bgcolor=""#FFFFFF"">
    	<center>
        	<table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""backgroundTable"" style=""height: 100% !important; margin-bottom: 0; margin-top: 0; margin-right: 0; margin-left: 0; padding-left: 0; padding-top: 0; padding-bottom: 0; padding-right: 0; width: 100% !important;"">
            	<tr>
                	<td align=""center"" valign=""top"">
                    	<table border=""0"" cellpadding=""20"" cellspacing=""0"" id=""templateContainer"" width=""760"" style=""font-family: Verdana; margin-top: 20px; font-size: 13px; border-top-color: #dddddd; border-right-color: #dddddd; border-bottom-color: #dddddd; border-left-color: #dddddd; border-top-style: solid; border-right-style: solid; border-bottom-style: solid; border-left-style: solid; border-top-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; background: #FAFAFA;"" bgcolor=""#FAFAFA"">
                        	<tr>
								<td colspan=""2"" style=""background:#00466a"">
									<a href=""http://www.kuazoo.com""><img src=""http://www.kuazoo.com/Content/img/logo.png"" width=""151px"" height=""27px""/></a>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:24px; color:#0093c9;font-weight:bold"">
									Password Reset Request Received
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;"">
									Dear "+name+@",<br/><br/>
									Kuazoo received a request to reset the password for your account.<br/>
									To reset your password, click on the button below.
									<br/><br/><br/>
									<a href="""+ url + @""" style=""color: white;background: #0093c9;padding: 10px;text-decoration: none;"">Reset Password</a><br/><br/>
								</td>
							</tr>
                        </table>
                        <br />
                    </td>
                </tr>
				<tr>
					<td>
						<center>
							<div class=""sharedetail"" style=""display:table-cell"">
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-email.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-fb.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-twitter.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-pinterest.png"" style=""width:28px;height:28px""></a>
							</div>
						</center>
					</td>
				</tr>
				<tr>
					<td>
						<center style=""color:#707070;font-size:14px"">
							© 2014 Kuazoo
						</center>
					</td>
				</tr>
            </table>
        </center>
    
<style type=""text/css"">
body { width: 100% !important; }
body { -webkit-text-size-adjust: none !important; }
body { margin: 0 !important; padding: 0 !important; }
a{
	color:blue;
	text-decoration:none;
}
#backgroundTable { height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; }
body { background-color: #FFFFFF !important; }
</style>
</body>
</html>";
            return body;
        }

        public static string CreateShareEmail(string feature, string flashdeal, string deal, string urlflash, string urldeal)
        {
            string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html>
<head>
        
        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
        
        <meta property=""og:title"" content=""*|MC:SUBJECT|*"" />
        
<title>*|MC:SUBJECT|*</title>
    
	
</head>
    <body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0"" style=""width: 100% !important; -webkit-text-size-adjust: none; margin-top: 0; margin-right: 0; margin-bottom: 0; margin-left: 0; padding-top: 0; padding-right: 0; padding-bottom: 0; padding-left: 0; background-color: #FFFFFF;"" bgcolor=""#FFFFFF"">
    	<center>
        	<table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""backgroundTable"" style=""height: 100% !important; margin-bottom: 0; margin-top: 0; margin-right: 0; margin-left: 0; padding-left: 0; padding-top: 0; padding-bottom: 0; padding-right: 0; width: 100% !important;"">
            	<tr>
                	<td align=""center"" valign=""top"">
                    	<table border=""0"" cellpadding=""20"" cellspacing=""0"" id=""templateContainer"" width=""780"" style=""font-family: Verdana; margin-top: 20px; font-size: 13px; border-top-color: #dddddd; border-right-color: #dddddd; border-bottom-color: #dddddd; border-left-color: #dddddd; border-top-style: solid; border-right-style: solid; border-bottom-style: solid; border-left-style: solid; border-top-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; background: #FAFAFA;"" bgcolor=""#FAFAFA"">
                        	<tr>
								<td colspan=""2"" style=""background:#00466a"">
									<a href=""http://www.kuazoo.com""><img src=""http://www.kuazoo.com/Content/img/logo.png"" width=""151px"" height=""27px""/></a>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:24px; color:#0093c9;font-weight:bold"">
									Featured Deals
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:24px; color:#0093c9;font-weight:bold"">
								<div class=""content"">
									" + feature+ @"
								</div>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;"">
									<div style=""width:100%; border-bottom:1px solid #dddddd""></div>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:24px; color:#0093c9;font-weight:bold"">
									More Great Deals
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:24px; color:#0093c9;font-weight:bold"">
								<div class=""content"">
									" + deal+@"
								</div>
								</td>
							</tr>
							<tr>
								<td colspan=""2""  align=""right"" style=""font-size:16px;"">
									<a href="""+urldeal+ @""" style=""color: white;background: #0093c9;padding: 10px;text-decoration: none;"">SEE ALL DEALS</a><br/><br/>
								</td>
							</tr>
                        </table>
                        <br />
                    </td>
                </tr>
				<tr>
					<td>
						<center>
							<div class=""sharedetail"" style=""display:table-cell"">
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-email.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-fb.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-twitter.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-pinterest.png"" style=""width:28px;height:28px""></a>
							</div>
						</center>
					</td>
				</tr>
				<tr>
					<td>
						<center style=""color:#707070;font-size:14px"">
							© 2014 Kuazoo
						</center>
					</td>
				</tr>
            </table>
        </center>
<style type=""text/css"">
body { width: 100% !important; }
body { -webkit-text-size-adjust: none !important; }
body { margin: 0 !important; padding: 0 !important; }
a{
	color:blue;
	text-decoration:none;
}
#backgroundTable { height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; }
body { background-color: #FFFFFF !important; }
*{
-webkit-box-sizing: border-box;
-moz-box-sizing: border-box;
box-sizing: border-box;
}
</style>
</body>
</html>";
            return body;
        }

        public static string CreateFlashDeal(string deal, string urldeal, decimal discountfrom, decimal discountto)
        {
            string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html>
<head>
        
        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
        
        <meta property=""og:title"" content=""*|MC:SUBJECT|*"" />
        
<title>*|MC:SUBJECT|*</title>
	
</head>
    <body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0"" style=""width: 100% !important; -webkit-text-size-adjust: none; margin-top: 0; margin-right: 0; margin-bottom: 0; margin-left: 0; padding-top: 0; padding-right: 0; padding-bottom: 0; padding-left: 0; background-color: #FFFFFF;"" bgcolor=""#FFFFFF"">
    	<center>
        	<table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""backgroundTable"" style=""height: 100% !important; margin-bottom: 0; margin-top: 0; margin-right: 0; margin-left: 0; padding-left: 0; padding-top: 0; padding-bottom: 0; padding-right: 0; width: 100% !important;"">
            	<tr>
                	<td align=""center"" valign=""top"">
                    	<table border=""0"" cellpadding=""20"" cellspacing=""0"" id=""templateContainer"" width=""760"" style=""font-family: Verdana; margin-top: 20px; font-size: 13px; border-top-color: #dddddd; border-right-color: #dddddd; border-bottom-color: #dddddd; border-left-color: #dddddd; border-top-style: solid; border-right-style: solid; border-bottom-style: solid; border-left-style: solid; border-top-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; background: #FAFAFA;"" bgcolor=""#FAFAFA"">
                        	<tr>
								<td colspan=""2"" style=""background:#00466a"">
									<div style=""background: url('http://www.kuazoo.com/Content/img/logo.png')no-repeat;
background-size: 100% 100%;
width: 151px;
height: 27px;
display: inline-table;""></div>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:24px; color:#0093c9;font-weight:bold"">
									Dear customer
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;"">
									We are pleased to inform you that Deal <a href="""+urldeal+@""">"+deal+@"</a> Hit Flash Deal
									<br/>
									Discount from " + discountfrom + @"% to " + discountto + @"%
								</td>
							</tr>
                        </table>
                        <br />
                    </td>
                </tr>
				<tr>
					<td>
						<center>
							<div class=""sharedetail"" style=""display:table-cell"">
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-email.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-fb.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-twitter.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-pinterest.png"" style=""width:28px;height:28px""></a>
							</div>
						</center>
					</td>
				</tr>
				<tr>
					<td>
						<center style=""color:#707070;font-size:14px"">
							&copy 2014 Kuazoo
						</center>
					</td>
				</tr>
            </table>
        </center>
    
<style type=""text/css"">
body { width: 100% !important; }
body { -webkit-text-size-adjust: none !important; }
body { margin: 0 !important; padding: 0 !important; }
a{
	color:blue;
	text-decoration:none;
}
#backgroundTable { height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; }
body { background-color: #FFFFFF !important; }
</style>
</body>
</html>";
            return body;
        }

        public static string CreateKuazooDeal(string deal, string urldeal)
        {
            string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html>
<head>
        
        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
        
        <meta property=""og:title"" content=""*|MC:SUBJECT|*"" />
        
<title>*|MC:SUBJECT|*</title>
	
</head>
    <body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0"" style=""width: 100% !important; -webkit-text-size-adjust: none; margin-top: 0; margin-right: 0; margin-bottom: 0; margin-left: 0; padding-top: 0; padding-right: 0; padding-bottom: 0; padding-left: 0; background-color: #FFFFFF;"" bgcolor=""#FFFFFF"">
    	<center>
        	<table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""backgroundTable"" style=""height: 100% !important; margin-bottom: 0; margin-top: 0; margin-right: 0; margin-left: 0; padding-left: 0; padding-top: 0; padding-bottom: 0; padding-right: 0; width: 100% !important;"">
            	<tr>
                	<td align=""center"" valign=""top"">
                    	<table border=""0"" cellpadding=""20"" cellspacing=""0"" id=""templateContainer"" width=""760"" style=""font-family: Verdana; margin-top: 20px; font-size: 13px; border-top-color: #dddddd; border-right-color: #dddddd; border-bottom-color: #dddddd; border-left-color: #dddddd; border-top-style: solid; border-right-style: solid; border-bottom-style: solid; border-left-style: solid; border-top-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; background: #FAFAFA;"" bgcolor=""#FAFAFA"">
                        	<tr>
								<td colspan=""2"" style=""background:#00466a"">
									<div style=""background: url('http://www.kuazoo.com/Content/img/logo.png')no-repeat;
background-size: 100% 100%;
width: 151px;
height: 27px;
display: inline-table;""></div>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:24px; color:#0093c9;font-weight:bold"">
									Dear customer
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;"">
									We are pleased to inform you that Deal <a href=""" + urldeal + @""">" + deal + @"</a> has been Kuazoo!
								</td>
							</tr>
                        </table>
                        <br />
                    </td>
                </tr>
				<tr>
					<td>
						<center>
							<div class=""sharedetail"" style=""display:table-cell"">
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-email.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-fb.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-twitter.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-pinterest.png"" style=""width:28px;height:28px""></a>
							</div>
						</center>
					</td>
				</tr>
				<tr>
					<td>
						<center style=""color:#707070;font-size:14px"">
							&copy 2014 Kuazoo
						</center>
					</td>
				</tr>
            </table>
        </center>
    
<style type=""text/css"">
body { width: 100% !important; }
body { -webkit-text-size-adjust: none !important; }
body { margin: 0 !important; padding: 0 !important; }
a{
	color:blue;
	text-decoration:none;
}
#backgroundTable { height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; }
body { background-color: #FFFFFF !important; }
</style>
</body>
</html>";
            return body;
        }

        public static string CreateMinimumTarget(string deal, string merchantname)
        {
            string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html>
<head>
        
        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
        
        <meta property=""og:title"" content=""*|MC:SUBJECT|*"" />
        
<title>*|MC:SUBJECT|*</title>
	
</head>
    <body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0"" style=""width: 100% !important; -webkit-text-size-adjust: none; margin-top: 0; margin-right: 0; margin-bottom: 0; margin-left: 0; padding-top: 0; padding-right: 0; padding-bottom: 0; padding-left: 0; background-color: #FFFFFF;"" bgcolor=""#FFFFFF"">
    	<center>
        	<table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""backgroundTable"" style=""height: 100% !important; margin-bottom: 0; margin-top: 0; margin-right: 0; margin-left: 0; padding-left: 0; padding-top: 0; padding-bottom: 0; padding-right: 0; width: 100% !important;"">
            	<tr>
                	<td align=""center"" valign=""top"">
                    	<table border=""0"" cellpadding=""20"" cellspacing=""0"" id=""templateContainer"" width=""760"" style=""font-family: Verdana; margin-top: 20px; font-size: 13px; border-top-color: #dddddd; border-right-color: #dddddd; border-bottom-color: #dddddd; border-left-color: #dddddd; border-top-style: solid; border-right-style: solid; border-bottom-style: solid; border-left-style: solid; border-top-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; background: #FAFAFA;"" bgcolor=""#FAFAFA"">
                        	<tr>
								<td colspan=""2"" style=""background:#00466a"">
									<div style=""background: url('http://www.kuazoo.com/Content/img/logo.png')no-repeat;
background-size: 100% 100%;
width: 151px;
height: 27px;
display: inline-table;""></div>
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:24px; color:#0093c9;font-weight:bold"">
									Dear Admin
								</td>
							</tr>
							<tr>
								<td colspan=""2"" style=""font-size:16px;"">
									Deal " + deal + @"</a> from merchant " + merchantname +@" hit Minimum Target
								</td>
							</tr>
                        </table>
                        <br />
                    </td>
                </tr>
				<tr>
					<td>
						<center>
							<div class=""sharedetail"" style=""display:table-cell"">
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-email.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-fb.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-twitter.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-pinterest.png"" style=""width:28px;height:28px""></a>
							</div>
						</center>
					</td>
				</tr>
				<tr>
					<td>
						<center style=""color:#707070;font-size:14px"">
							&copy 2014 Kuazoo
						</center>
					</td>
				</tr>
            </table>
        </center>
    
<style type=""text/css"">
body { width: 100% !important; }
body { -webkit-text-size-adjust: none !important; }
body { margin: 0 !important; padding: 0 !important; }
a{
	color:blue;
	text-decoration:none;
}
#backgroundTable { height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; }
body { background-color: #FFFFFF !important; }
</style>
</body>
</html>";
            return body;
        }



        public static string CreateGiftEmail(string dealname, string imageurl, string nameto, string namefrom, string note, string variname, string fineprint, string orderno, DateTime expirationdate)
        {
            string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html>
<head>
        
        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
        
        <meta property=""og:title"" content=""*|MC:SUBJECT|*"" />
        
<title>*|MC:SUBJECT|*</title>
	
</head>
    <body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0"" style=""width: 100% !important; -webkit-text-size-adjust: none; margin-top: 0; margin-right: 0; margin-bottom: 0; margin-left: 0; padding-top: 0; padding-right: 0; padding-bottom: 0; padding-left: 0; background-color: #FFFFFF;font-family:Arial;"" bgcolor=""#FFFFFF"">
    	<center>
        	<table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""backgroundTable"" style=""height: 100% !important; margin-bottom: 0; margin-top: 0; margin-right: 0; margin-left: 0; padding-left: 0; padding-top: 0; padding-bottom: 0; padding-right: 0; width: 100% !important;"">
            	<tr>
                	<td align=""center"" valign=""top"">
                    	<table border=""0"" cellpadding=""20"" cellspacing=""0"" id=""templateContainer"" width=""760"" style=""font-family: Verdana; margin-top: 20px; font-size: 13px; border-top-color: #dddddd; border-right-color: #dddddd; border-bottom-color: #dddddd; border-left-color: #dddddd; border-top-style: solid; border-right-style: solid; border-bottom-style: solid; border-left-style: solid; border-top-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; background: #FAFAFA;"" bgcolor=""#FAFAFA"">
                        	<tr>
								<td colspan=""2"" height=""20px"" style=""background:#00466a;"">
									<a href=""http://www.kuazoo.com""><img src=""http://www.kuazoo.com/Content/img/logo.png"" width=""151px"" height=""27px""/></a>
								</td>
							</tr>
                            
                            <tr>
                            	<td width=""450px"" style=""padding-right: 20px;"">
                                	<div style=""border: 1px solid #CCCCCC; font-size:14px; font-weight:bold; box-shadow: 0px 0px 5px 3px rgba(0,0,0,0.1);"">
                                        <div style=""padding: 8px; 5px; border-bottom: 1px solid #CCCCCC; background-color:white;"">
                                            " + dealname + @"
                                        </div>
                                        <div style=""overflow:hidden; min-width: 450px; max-width: 450px; min-height:250px; max-height:250px; position: relative;"">
                                            <img src=""" + imageurl + @""" style=""display: block; width:100%; position: absolute; top:-100%; left:0; right: 0; bottom:-100%; margin: auto;"" />
                                        </div>
                                    </div>
                                </td>
                                
                                <td valign=""middle"" style=""padding:0px;"">
                                	<img src=""http://www.kuazoo.com/Content/img/gift4u.png"" width=""200px"" />
                                </td>
                            
                            </tr>
                            
                            <tr>
                            	<td colspan=""2"" >
                                	<div>
                                		<span class=""bigtext greytext"">to:</span> <span class=""bigtext bluetext"">" + nameto + @"</span>
                                	</div>
                                    <div>
                                		<span class=""bigtext greytext"">from:</span> <span class=""bigtext bluetext"">" + namefrom + @"</span>
                                	</div>
                                </td>
                            </tr>                            
                            
                            <tr>
                            	<td colspan=""2"" >
                                	" + note + @"
                                </td>
                            </tr>
                            
                            <tr>
                            	<td colspan=""2"" >
                                	<div>
                                		<span class=""bigtext greytext"">gift:</span>
                                	</div>
                                    <div>
                                		<span class=""bigtext"">" + dealname + @"</span>
                                	</div>
                                    <div>
                                		<span style=""font-size: 18px;"">" + variname + @"</span>
                                    </div>
                                </td>
                            </tr>                                  
                            
                            <tr class=""smalltext"">
                            	<td colspan=""2"">
                                	<div>
                                	<div style=""border-bottom: 1px solid #CCCCCC;""></div>
                                    </div>
                                    <div style=""padding: 10px 0 5px 0;"">
                                		<b>Order Number:</b> " + orderno + @"
                                    </div>
                                    <div style=""padding: 0 0 10px 0;"">
                                		<b>Expiration Date:</b> " + expirationdate.ToString("dd MMMM yyyy") + @"
                                    </div>
                                    <div>
                                	<div style=""border-bottom: 1px solid #CCCCCC;""></div>
                                    </div>
                                </td>
                            </tr>
                            
                            <tr class=""smalltext greytext"">
                            	<td colspan=""2"" >
                                	<div>
                                		<b>Terms and Conditions</b>
                                    </div>
                                    <div>
                            			" + fineprint + @"
                                	</div>
                                </td>
                            </tr>
                        </table>
                        <br />
                    </td>
                </tr>
				<tr>
					<td>
						<center>
							<div class=""sharedetail"" style=""display:table-cell; text-align:center;"">
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-email.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-fb.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-twitter.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-pinterest.png"" style=""width:28px;height:28px""></a>
							</div>
						</center>
					</td>
				</tr>
            </table>
        </center>
    
<style type=""text/css"">
body { width: 100% !important; }
body { -webkit-text-size-adjust: none !important; }
body { margin: 0 !important; padding: 0 !important; }
a{
	color:blue;
	text-decoration:none;
}
#backgroundTable { height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; }
body { background-color: #FFFFFF !important; }

.bigtext{
	font-size: 26px;
}
.greytext{
	color: #999999;
}
.bluetext{
	color: #0080B1;
}
.smalltext{
	font-size: 11px;
}

#templateContainer tr td{
	padding: 20px 40px;	
}

</style>
</body>
</html>";
            return body;
        }


        public static string CreateGiftServicesEmail(string dealname, string imageurl, string nameto, string namefrom, string note, string variname, string fineprint, string orderno, DateTime expirationdate, string vouchercode)
        {
            string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html>
<head>
        
        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
        
        <meta property=""og:title"" content=""*|MC:SUBJECT|*"" />
        
<title>*|MC:SUBJECT|*</title>
	
</head>
    <body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0"" style=""width: 100% !important; -webkit-text-size-adjust: none; margin-top: 0; margin-right: 0; margin-bottom: 0; margin-left: 0; padding-top: 0; padding-right: 0; padding-bottom: 0; padding-left: 0; background-color: #FFFFFF;font-family:Arial;"" bgcolor=""#FFFFFF"">
    	<center>
        	<table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""backgroundTable"" style=""height: 100% !important; margin-bottom: 0; margin-top: 0; margin-right: 0; margin-left: 0; padding-left: 0; padding-top: 0; padding-bottom: 0; padding-right: 0; width: 100% !important;"">
            	<tr>
                	<td align=""center"" valign=""top"">
                    	<table border=""0"" cellpadding=""20"" cellspacing=""0"" id=""templateContainer"" width=""760"" style=""font-family: Verdana; margin-top: 20px; font-size: 13px; border-top-color: #dddddd; border-right-color: #dddddd; border-bottom-color: #dddddd; border-left-color: #dddddd; border-top-style: solid; border-right-style: solid; border-bottom-style: solid; border-left-style: solid; border-top-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; background: #FAFAFA;"" bgcolor=""#FAFAFA"">
                        	<tr>
								<td colspan=""2"" height=""20px"" style=""background:#00466a;"">
									<a href=""http://www.kuazoo.com""><img src=""http://www.kuazoo.com/Content/img/logo.png"" width=""151px"" height=""27px""/></a>
								</td>
							</tr>
                            
                            <tr>
                            	<td width=""450px"" style=""padding-right: 20px;"">
                                	<div style=""border: 1px solid #CCCCCC; font-size:14px; font-weight:bold; box-shadow: 0px 0px 5px 3px rgba(0,0,0,0.1);"">
                                        <div style=""padding: 8px; 5px; border-bottom: 1px solid #CCCCCC; background-color:white;"">
                                            " + dealname + @"
                                        </div>
                                        <div style=""overflow:hidden; min-width: 450px; max-width: 450px; min-height:250px; max-height:250px; position: relative;"">
                                            <img src=""" + imageurl + @""" style=""display: block; width:100%; position: absolute; top:-100%; left:0; right: 0; bottom:-100%; margin: auto;"" />
                                        </div>
                                    </div>
                                </td>
                                
                                <td valign=""middle"" style=""padding:0px;"">
                                	<img src=""http://www.kuazoo.com/Content/img/gift4u.png"" width=""200px"" />
                                </td>
                            
                            </tr>
                            
                            <tr>
                            	<td colspan=""2"" >
                                	<div>
                                		<span class=""bigtext greytext"">to:</span> <span class=""bigtext bluetext"">" + nameto + @"</span>
                                	</div>
                                    <div>
                                		<span class=""bigtext greytext"">from:</span> <span class=""bigtext bluetext"">" + namefrom + @"</span>
                                	</div>
                                </td>
                            </tr>                            
                            
                            <tr>
                            	<td colspan=""2"" >
                                	" + note + @"
                                </td>
                            </tr>
                            
                            <tr>
                            	<td colspan=""2"" >
                                	<div>
                                		<span class=""bigtext greytext"">gift:</span>
                                	</div>
                                    <div>
                                		<span class=""bigtext"">" + dealname + @"</span>
                                	</div>
                                    <div>
                                		<span style=""font-size: 18px;"">" + variname + @"</span>
                                    </div>
                                </td>
                            </tr>                            
                            
                            <tr>
                            	<td colspan=""2"" class=""bigtext"">
                                	<div>
                                		<span class=""greytext"">gift code:</span>
                                	</div>
                                    " + vouchercode + @"
                                </td>
                            </tr>
                            
                            
                            <tr class=""smalltext"">
                            	<td colspan=""2"">
                                	<div>
                                	<div style=""border-bottom: 1px solid #CCCCCC;""></div>
                                    </div>
                                    <div style=""padding: 10px 0 5px 0;"">
                                		<b>Order Number:</b> " + orderno + @"
                                    </div>
                                    <div style=""padding: 0 0 10px 0;"">
                                		<b>Expiration Date:</b> " + expirationdate.ToString("dd MMMM yyyy") + @"
                                    </div>
                                    <div>
                                	<div style=""border-bottom: 1px solid #CCCCCC;""></div>
                                    </div>
                                </td>
                            </tr>
                            
                            <tr class=""smalltext greytext"">
                            	<td colspan=""2"" >
                                	<div>
                                		<b>Terms and Conditions</b>
                                    </div>
                                    <div>
                            			" + fineprint + @"
                                	</div>
                                </td>
                            </tr>
                        </table>
                        <br />
                    </td>
                </tr>
				<tr>
					<td>
						<center>
							<div class=""sharedetail"" style=""display:table-cell; text-align:center;"">
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-email.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-fb.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-twitter.png"" style=""width:28px;height:28px""></a>
								<a href=""#""><img src=""http://www.kuazoo.com.my/Content/img/icon/icon-pinterest.png"" style=""width:28px;height:28px""></a>
							</div>
						</center>
					</td>
				</tr>
            </table>
        </center>
    
<style type=""text/css"">
body { width: 100% !important; }
body { -webkit-text-size-adjust: none !important; }
body { margin: 0 !important; padding: 0 !important; }
a{
	color:blue;
	text-decoration:none;
}
#backgroundTable { height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; }
body { background-color: #FFFFFF !important; }

.bigtext{
	font-size: 26px;
}
.greytext{
	color: #999999;
}
.bluetext{
	color: #0080B1;
}
.smalltext{
	font-size: 11px;
}

#templateContainer tr td{
	padding: 20px 40px;	
}

</style>
</body>
</html>";
            return body;
        }




//          public static string CreateShareEmail(string feature, string flashdeal, string deal, string urlflash, string urldeal)
//        {
//            string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
//<html>
//<head>
//        
//        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
//        
//        <meta property=""og:title"" content=""*|MC:SUBJECT|*"" />
//        
//<title>*|MC:SUBJECT|*</title>
//    
//	
//</head>
//    <body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0"" style=""width: 100% !important; -webkit-text-size-adjust: none; margin-top: 0; margin-right: 0; margin-bottom: 0; margin-left: 0; padding-top: 0; padding-right: 0; padding-bottom: 0; padding-left: 0; background-color: #FFFFFF;"" bgcolor=""#FFFFFF"">
//    	<center>
//        	<table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""backgroundTable"" style=""height: 100% !important; margin-bottom: 0; margin-top: 0; margin-right: 0; margin-left: 0; padding-left: 0; padding-top: 0; padding-bottom: 0; padding-right: 0; width: 100% !important;"">
//            	<tr>
//                	<td align=""center"" valign=""top"">
//                    	<table border=""0"" cellpadding=""20"" cellspacing=""0"" id=""templateContainer"" width=""780"" style=""font-family: Verdana; margin-top: 20px; font-size: 13px; border-top-color: #dddddd; border-right-color: #dddddd; border-bottom-color: #dddddd; border-left-color: #dddddd; border-top-style: solid; border-right-style: solid; border-bottom-style: solid; border-left-style: solid; border-top-width: 1px; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; background: #FAFAFA;"" bgcolor=""#FAFAFA"">
//                        	<tr>
//								<td colspan=""2"" style=""background:#00466a"">
//									<a href=""http://www.kuazoo.com""><img src=""http://www.kuazoo.com/Content/img/logo.png"" width=""151px"" height=""27px""/></a>
//								</td>
//							</tr>
//							<tr>
//								<td colspan=""2"" style=""font-size:24px; color:#0093c9;font-weight:bold"">
//									Featured Deals
//								</td>
//							</tr>
//							<tr>
//								<td colspan=""2"" style=""font-size:24px; color:#0093c9;font-weight:bold"">
//								<div class=""content"">
//									" + feature+@"
//								</div>
//								</td>
//							</tr>
//							<tr>
//								<td colspan=""2"" style=""font-size:24px; color:#0093c9;font-weight:bold"">
//									Flash Deals
//								</td>
//							</tr>
//							<tr>
//								<td colspan=""2"" style=""font-size:24px; color:#0093c9;font-weight:bold"">
//								<div class=""content"">
//									"+flashdeal+@"
//								</div>
//								</td>
//							</tr>
//							<tr>
//								<td colspan=""2"" align=""right"" style=""font-size:16px;"">
//									<a href="""+urlflash+@""" style=""color: white;background: #0093c9;padding: 10px;text-decoration: none;"">SEE ALL FLASH DEALS</a><br/><br/>
//								</td>
//							</tr>
//							<tr>
//								<td colspan=""2"" style=""font-size:24px; color:#0093c9;font-weight:bold"">
//									More Great Deals
//								</td>
//							</tr>
//							<tr>
//								<td colspan=""2"" style=""font-size:24px; color:#0093c9;font-weight:bold"">
//								<div class=""content"">
//									"+deal+@"
//								</div>
//								</td>
//							</tr>
//							<tr>
//								<td colspan=""2""  align=""right"" style=""font-size:16px;"">
//									<a href="""+urldeal+ @""" style=""color: white;background: #0093c9;padding: 10px;text-decoration: none;"">SEE ALL DEALS</a><br/><br/>
//								</td>
//							</tr>
//                        </table>
//                        <br />
//                    </td>
//                </tr>
//				<tr>
//					<td>
//						<center>
//							<div class=""sharedetail"" style=""display:table-cell"">
//								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-email.png"" style=""width:28px;height:28px""></a>
//								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-fb.png"" style=""width:28px;height:28px""></a>
//								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-twitter.png"" style=""width:28px;height:28px""></a>
//								<a href=""#""><img src=""http://www.kuazoo.com/Content/img/icon/icon-pinterest.png"" style=""width:28px;height:28px""></a>
//							</div>
//						</center>
//					</td>
//				</tr>
//				<tr>
//					<td>
//						<center style=""color:#707070;font-size:14px"">
//							© 2014 Kuazoo
//						</center>
//					</td>
//				</tr>
//            </table>
//        </center>
//<style type=""text/css"">
//body { width: 100% !important; }
//body { -webkit-text-size-adjust: none !important; }
//body { margin: 0 !important; padding: 0 !important; }
//a{
//	color:blue;
//	text-decoration:none;
//}
//#backgroundTable { height: 100% !important; margin: 0 !important; padding: 0 !important; width: 100% !important; }
//body { background-color: #FFFFFF !important; }
//*{
//-webkit-box-sizing: border-box;
//-moz-box-sizing: border-box;
//box-sizing: border-box;
//}
//</style>
//</body>
//</html>";
//            return body;
//        }
//    }
    }
}
