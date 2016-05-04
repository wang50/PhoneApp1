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
    class PostNode
    {
        String nodeurl;
        HttpClient httpClient;
        public PostNode() {
            nodeurl = "http://OpenTechTest.chinacloudapp.cn:3000";
            httpClient = new HttpClient();
        }

        public async Task<string> sender(String msg)
        {
            String str = "Post Success!";
            try
            {
                HttpResponseMessage wcfResponse = await httpClient.PostAsync(nodeurl, new StringContent(msg));
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
