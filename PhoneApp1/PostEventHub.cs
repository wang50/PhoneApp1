using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using System.IO;
using System.Net.Http;
using System.Net;

namespace PhoneApp1
{
    class PostEventHub
    {

        String sastoken;
        String host;
        String huburl;
        HttpClient httpClient;

        public PostEventHub()
        {
            sastoken = "SharedAccessSignature sr=wanderhub-ns.servicebus.chinacloudapi.cn&sig=UJyDmc4eTAw3NKbTrsjk13UqM0h5MRQSZNgd31tkL60%3d&se=1492073270&skn=ReceiveRule";
            host = "wanderhub-ns.servicebus.chinacloudapi.cn";
            huburl = "https://wanderhub-ns.servicebus.chinacloudapi.cn/wanderhub/messages?timeout=60&api-version=2014-01";

            httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization", sastoken);
            //  httpClient.DefaultRequestHeaders.Add("Content-Type", "application/atom+xml;type=entry;charset=utf-8");
            httpClient.DefaultRequestHeaders.Add("Host", host);
            httpClient.DefaultRequestHeaders.Add("Alert", "Strong Wind");
        }

        public async Task<string> sender(String msg)
        {
            /**
            var response = await httpClient.PostAsync(huburl, new StringContent(msg, Encoding.UTF8, "application/json"));
            return await response.Content.ReadAsStringAsync();
            **/
            String str = "Post Success!";
            try
            {
                HttpResponseMessage wcfResponse = await httpClient.PostAsync(huburl, new StringContent(msg, Encoding.UTF8, "application/json"));
                str = wcfResponse.ToString();
            }
            catch (HttpRequestException hre)
            {
                str = "Error:" + hre.Message;
                MessageBox.Show(str);
            }
            catch (TaskCanceledException)
            {
                str = "Request canceled.";
                MessageBox.Show(str);
            }
            catch (Exception ex)
            {
                str = ex.Message;
                MessageBox.Show(str);
            }
            finally
            {
            }
            return str;
        }

        public void discon()
        {
            if (httpClient != null)
            {
                httpClient.Dispose();
                httpClient = null;
            }
        }
       
    }
}
