using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Model;
using BLL;
using LukuangWeb.LoginService;
using System.Net;
using System.IO;
using LukuangWeb.jiankong;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Data;

namespace LukuangWeb
{
    public partial class GzcxApi : System.Web.UI.Page
    {
        string key = "haiwei602";     //APP验证码
        string Ckey = "";
        string str = "";
        Model.Variablecs var = new Variablecs();
        protected void Page_Load(object sender, EventArgs e)
        {
            //解决https请求异常：基础连接已经关闭: 未能为 SSL/TLS 安全通道建立信任关系。  
            ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(ValidateServerCertificate));
            ServicePointManager.Expect100Continue = false;
            Ckey = Request.QueryString["key"];
            if (key != Ckey)
            {
                str = "{\"status\":\"wrongkey\"}";
                Response.Write(str);
            }
            else
            {
                #region 获取变量
                #region 费率
                if (Request.QueryString["Fl_State"] != null)
                {
                    var.Fl_State = int.Parse(Request.QueryString["Fl_State"]);    //是否费率
                }
                if (Request.QueryString["fromStationNo"] != null)
                {
                    var.fromStationNo = Request.QueryString["fromStationNo"];    //入口编号
                }
                if (Request.QueryString["toStationNo"] != null)
                {
                    var.toStationNo = Request.QueryString["toStationNo"];    //出口编号
                }
                if (Request.QueryString["vehicleType"] != null)
                {
                    var.vehicleType = int.Parse(Request.QueryString["vehicleType"]);    //轴组
                }
                if (Request.QueryString["weight"] != null && Request.QueryString["weight"] != "")
                {
                    var.weight = Request.QueryString["weight"];    //重量(客车不填，只对货车) – 单位:KG
                }
                if (Request.QueryString["vehicleClass"] != null)
                {
                    var.vehicleClass = Request.QueryString["vehicleClass"];    //车型
                }
                #endregion
                 
                #region 飞机
                if (Request.QueryString["Fj_state"] != null)
                {
                    var.Fj_state = int.Parse(Request.QueryString["Fj_state"]);    //是否飞机
                }
                if (Request.QueryString["FightType"] != null)
                {
                    var.FightType = int.Parse(Request.QueryString["FightType"]);    //飞机出入港状态 1为入港，0为出港
                }
                #endregion
                 
            }
        }

        #region 路线
         
        private void Get_Lx()
        {
            string appkey = "f4d34764307099273839c79e5b10fc66"; //配置您申请的appkey
            DateTime nowdate = DateTime.Now;


            //1.航班可售舱位及价格查询
            string url1 = "http://192.168.100.118:7076/mapdata/rest/mapdata/search";

            var parameters1 = new Dictionary<string, string>();

            parameters1.Add("layerName", "GIS_LX");//查询名称
            parameters1.Add("where", "lxdm='" + var.Zc_tj + "'");//查询条件
            parameters1.Add("page", "0");//页数
            parameters1.Add("rows", "99");//每页显示
            string result1 = sendPost(url1, parameters1, "get");


            result1 = "{\"status\":\"success\",\"reason\":{\"data\":" + result1 + "},\"error_code\": 0}";
            Response.Write(result1);
        }
        #endregion

        
 
        #endregion

        


        #region json处理


        /// <summary>
        /// Http (GET/POST)
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="parameters">请求参数</param>
        /// <param name="method">请求方法</param>
        /// <returns>响应内容</returns>
        static string sendPost(string url, IDictionary<string, string> parameters, string method)
        {
            if (method.ToLower() == "post")
            {
                HttpWebRequest req = null;
                HttpWebResponse rsp = null;
                System.IO.Stream reqStream = null;
                try
                {
                    req = (HttpWebRequest)WebRequest.Create(url);
                    req.Method = method;
                    req.KeepAlive = false;
                    req.ProtocolVersion = HttpVersion.Version10;
                    req.Timeout = 5000;
                    req.ContentType = "application/json;charset=utf-8";
                    byte[] postData = Encoding.UTF8.GetBytes(BuildQuery(parameters, "utf8"));
                    reqStream = req.GetRequestStream();
                    reqStream.Write(postData, 0, postData.Length);
                    rsp = (HttpWebResponse)req.GetResponse();
                    Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                    return GetResponseAsString(rsp, encoding);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                finally
                {
                    if (reqStream != null) reqStream.Close();
                    if (rsp != null) rsp.Close();
                }
            }
            else
            {
                //创建请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?" + BuildQuery(parameters, "utf8"));

                //GET请求
                request.Method = "GET";
                request.ReadWriteTimeout = 5000;
                request.ContentType = "text/html;charset=UTF-8";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));

                //返回内容
                string retString = myStreamReader.ReadToEnd();
                return retString;
            }
        }

        /// <summary>
        /// Http (GET/POST)
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="parameters">请求参数</param>
        /// <param name="method">请求方法</param>
        /// <returns>响应内容</returns>
        static string sendPost1(string url, IDictionary<string, string> parameters, string method)
        {
            if (method.ToLower() == "post")
            {
                HttpWebRequest req = null;
                HttpWebResponse rsp = null;
                System.IO.Stream reqStream = null;
                try
                {
                    req = (HttpWebRequest)WebRequest.Create(url);
                    req.Method = method;
                    req.KeepAlive = false;
                    req.ProtocolVersion = HttpVersion.Version10;
                    req.Timeout = 5000;
                    req.ContentType = "application/json;charset=utf-8";
                    byte[] postData = Encoding.UTF8.GetBytes(BuildQuery(parameters, "utf8"));
                    reqStream = req.GetRequestStream();
                    reqStream.Write(postData, 0, postData.Length);
                    rsp = (HttpWebResponse)req.GetResponse();
                    Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                    return GetResponseAsString(rsp, encoding);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                finally
                {
                    if (reqStream != null) reqStream.Close();
                    if (rsp != null) rsp.Close();
                }
            }
            else
            {
                //创建请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?" + BuildQuery(parameters, "gb2312"));

                //GET请求
                request.Method = "GET";
                request.ReadWriteTimeout = 5000;
                request.ContentType = "text/xml";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("gb2312"));

                //返回内容
                string retString = myStreamReader.ReadToEnd();
                return retString;
            }
        }

        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典</param>
        /// <returns>URL编码后的请求数据</returns>
        static string BuildQuery(IDictionary<string, string> parameters, string encode)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;
            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name))//&& !string.IsNullOrEmpty(value)
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }
                    postData.Append(name);
                    postData.Append("=");
                    if (encode == "gb2312")
                    {
                        postData.Append(HttpUtility.UrlEncode(value, Encoding.GetEncoding("gb2312")));
                    }
                    else if (encode == "utf8")
                    {
                        postData.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
                    }
                    else
                    {
                        postData.Append(value);
                    }
                    hasParam = true;
                }
            }
            return postData.ToString();
        }

        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            System.IO.Stream stream = null;
            StreamReader reader = null;
            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);
                return reader.ReadToEnd();
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }
        }
        private static bool ValidateServerCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            //为了通过证书验证，总是返回true  
            return true;
        }
        #endregion
    }
}
