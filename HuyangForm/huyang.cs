using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using MSScriptControl;

namespace HuyangForm
{
    class Huyang
    {
        //static CookieContainer cookie = new CookieContainer();
        CookieContainer cookie = new CookieContainer();
        public int elementPerPage = 50;
        public string rsrvtQntt = "1";              // 기간
        public int availDay = 0;                    // 날짜
        public int useBgnDtm = 0;                   // 날짜
        public string strUseBgnDtm = string.Empty;  // 날짜
        public string fcltId = string.Empty;        // 예약할 방 Id
        public string paramDprtmId = string.Empty;  // 예약할 휴양림 Id
        public string waitState = string.Empty;
        public int areaCode = 3000001;              // 지역
        public string dprtmId = "0000";             // 휴양림
        public string fcltMdclsCd = "00000";        // 시설
        public string accst = "00000000";           // 인원
        public string _dprtmId = "on";
        public string _fcltMdclsCd = "on";
        public string _accst = "on";
        public bool reserSearch = true;
        public string _resercheck = "on";
        public string _waitCheck = "on";

        public enum 기간
        {
            일박이일 = 1,
            이박삼일 = 2,
            삼박사일 = 3
        }
        public enum 지역
        {
            서울 = 3000001,
            강원 = 3000002,
            충북 = 3000003,
            충남 = 3000004,
            전북 = 3000005,
            전남 = 3000006,
            경북 = 3000007,
            경남 = 3000008
        }
        public enum 시설
        {
            전체 = 0,
            숲속의집 = 3001,
            휴양관 = 3002,
            연립동 = 3003,
            숲속수련장 = 3004,
            한옥동 = 3005,
            야영데크 = 4002,
            황토온돌데크 = 4003,
            몽골텐트 = 4004,
            오토캠핑장 = 4005,
            캐빈 = 4006,
            캠핑카야영장 = 4007,
            데크텐트 = 4009
        }
        public enum 휴양림
        {
            전체 = 0,
            /*---------서울----------*/
            산음 = 103, //양평
            운악산 = 224, //포천
            유명산 = 101, //가평
            중미산 = 108, //양평
            /*---------강원----------*/
            가리왕산 = 113,
            검봉산 = 224,
            대관령 = 111,
            두타산 = 243,
            미천골 = 112,
            방태산 = 109,
            백운산 = 223,
            복주산 = 110,
            삼봉 = 107,
            용대 = 102,
            용화산 = 222,
            청태산 = 106,
            /*---------충북----------*/
            상당산성 = 300,
            속리산 = 115,
            황정산 = 242,
            /*---------충남----------*/
            오서산 = 191,
            용현 = 220,
            희리산 = 187,
            /*---------전북----------*/
            덕유산 = 141,
            변산 = 189,
            운장산 = 194,
            회문산 = 188,
            /*---------전남----------*/
            낙안민속 = 200,
            방장산 = 181,
            천관산 = 196,
            /*---------경북----------*/
            검마산 = 184,
            대야산 = 245,
            운문산 = 195,
            청옥산 = 183,
            칠보산 = 182,
            통고산 = 193,
            /*---------경남----------*/
            남해 = 192,
            신불산 = 105,
            지리산 = 190
        }
        public enum 인원
        {
            전체 = 0,
            삼인실 = 3001001,
            삼사인실 = 3001002,
            사인실 = 3001003,
            오육인실 = 3001004,
            육팔인실 = 3001005,
            팔구인실 = 3001006,
            십십일인실 = 3001007,
            십이인실 = 3001008
        }

        public void SetParams(string _rsrvtQntt, string YYYY, string MM, string DD, string _dprtmId, string _fcltMdclsCd)
        {
            rsrvtQntt = _rsrvtQntt;
            dprtmId = _dprtmId;
            fcltMdclsCd = _fcltMdclsCd;

            useBgnDtm = Convert.ToInt32(YYYY + MM + DD);
            strUseBgnDtm = string.Format("{0}%EB%85%84+{1}%EC%9B%94+{2}%EC%9D%BC", YYYY, MM, DD);
            availDay = useBgnDtm;
        }
        private string makeSendData()
        {
            string sendData = string.Format("elementPerPage={0}&rsrvtQntt={1}&useBgnDtm={2}&fcltId={3}&paramDprtmId={4}&waitState={5}&strUseBgnDtm={6}&availDay={7}&areaCode={8}&dprtmId={9}&_dprtmId={10}&fcltMdclsCd={11}&_fcltMdclsCd={12}&accst={13}&_accst={14}&_reserCheck={15}&_waitCheck={16}",
                /*  0 */    elementPerPage,
                /*  1 */    rsrvtQntt,
                /*  2 */    useBgnDtm,
                /*  3 */    fcltId,
                /*  4 */    paramDprtmId,
                /*  5 */    waitState,
                /*  6 */    strUseBgnDtm,
                /*  7 */    availDay,
                /*  8 */    areaCode,
                /*  9 */    dprtmId,
                /* 10 */    _dprtmId,
                /* 11 */    fcltMdclsCd,
                /* 12 */    _fcltMdclsCd,
                /* 13 */    accst,
                /* 14 */    _accst,
                /* 15 */    _resercheck,
                /* 16 */    _waitCheck
            );

            if (reserSearch == true)
                sendData += "&reserCheck=Y";

            return sendData;
        }

        private void Debug(string filepath, string text)
        {
#if false
            System.IO.StreamWriter file = new System.IO.StreamWriter(filepath);
            file.WriteLine(text);
            file.Close();
#endif
        }
        private string RSAEncrypt(string fileName, string functionName, params object[] pams)
        {
            StreamReader reader = new StreamReader(Environment.CurrentDirectory + @"\" + fileName);
            string scriptCode = reader.ReadToEnd();

            ScriptControlClass sc = new ScriptControlClass();
            sc.Language = "javascript";
            sc.AddCode(scriptCode);

            object obj = sc.Run(functionName, pams);
            return obj.ToString();
        }
        private int IdPwEncrypt(string mmberId, string mmberPassword, ref string encId, ref string encPw)
        {
            string html = RequestWebPage("http://huyang.go.kr/main.action",
                                         "http://huyang.go.kr/main.action", "", cookie);

            string rsaPublicKeyModulus = null;
            string rsaPublicKeyExponent = null;

            Regex regex = new Regex("<input type=\"hidden\" id=\"rsaPublicKeyModulus\" value=\"(.*?)\"");
            Match match = regex.Match(html);
            if (match.Success)
                rsaPublicKeyModulus = match.Groups[1].Value;

            regex = new Regex("<input type=\"hidden\" id=\"rsaPublicKeyExponent\" value=\"(.*?)\"");
            match = regex.Match(html);
            if (match.Success)
                rsaPublicKeyExponent = match.Groups[1].Value;

            if (rsaPublicKeyModulus == null || rsaPublicKeyExponent == null)
                return -1;

            /*
             huyang.go.kr/member/memberLogin.action?RSA=RSA
             &mmberId=90218a26b8983dccd97f70b52643bda1f8d98ea0531fa32205ef58d29af83db816a3e0167a0832c2ba328c06a61e86f5d82686041a70b07a314abe59d173a382d864de4310a5677075cc7af46eb1481ffa948a62d51c7c45f38c6695874ea9810615df2320cd9f10969b1654f95c18667c76e2cad8aabe34bd3b2f12df3db047
             &mmberPassword=7313c5d9b4b85b835f18828b567e961173e947c148582e7c759df1c584c9063704588c5381f8c7f97123a978f79d00364c4b3988c99dd825e097057871c033fd6aa8ef22b9066cb4427af6fe057546e223918e415da8fbbc38ffd4a24189e76fe525bdd71c5993b4d73cd843188fa5fc5d6f3eecee2872073d06c1bbcc9c9de8
             - www.cnblogs.com/qingci/archive/2012/10/18/2729246.html
             - blog.csdn.net/blacksource/article/details/17265607
             */
            encId = RSAEncrypt("rsa.js", "Test", rsaPublicKeyModulus, rsaPublicKeyExponent, mmberId);
            encPw = RSAEncrypt("rsa.js", "Test", rsaPublicKeyModulus, rsaPublicKeyExponent, mmberPassword);

            return 0;
        }

        public string RequestWebPage(string referer, string url, string sendData, CookieContainer cookie)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url); // 웹 요청 객체 생성

            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:37.0) Gecko/20100101 Firefox/37.0";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.Method = "POST";
            req.ContentLength = sendData.Length;
            req.ContentType = "application/x-www-form-urlencoded";
            req.Referer = referer;
            req.KeepAlive = true;
            req.CookieContainer = cookie;

            StreamWriter writer = new StreamWriter(req.GetRequestStream());
            writer.Write(sendData);
            writer.Close();

            HttpWebResponse result = (HttpWebResponse)req.GetResponse();
            if (result.StatusCode == HttpStatusCode.OK)
            {
                Encoding encode = Encoding.GetEncoding("utf-8");
                Stream strReceiveStream = result.GetResponseStream();
                StreamReader reqStreamReader = new StreamReader(strReceiveStream, encode);

                string strResult = reqStreamReader.ReadToEnd();

                req.Abort();
                strReceiveStream.Close();
                reqStreamReader.Close();

                return strResult;
            }
            else
            {
                Console.WriteLine("Error");
                return "ERR";
            }
        }
        public int Login(string mmberId, string mmberPassword)
        {
            string encId = null;
            string encPw = null;

            if (IdPwEncrypt(mmberId, mmberPassword, ref encId, ref encPw) < 0)
            {
                Console.WriteLine("Login Fail : There is no \"rsaPublicKey\"");
                return -1;
            }

            string sendData = string.Format("RSA=RSA&mmberId={0}&mmberPassword={1}", encId, encPw);
            string html = RequestWebPage("http://huyang.go.kr/main.action",
                                         "http://huyang.go.kr/member/memberLogin.action", sendData, cookie);
            if (!html.Contains("my_reser"))
            {
                Console.WriteLine("Login Fail");
                return -1;
            }

            Console.WriteLine("Login Success");
            return 0;
        }
        public int Search()
        {
            /* init blank value */
            paramDprtmId = string.Empty;
            fcltId = string.Empty;
            waitState = string.Empty;

            //string sendData = "elementPerPage=10&rsrvtQntt=1&useBgnDtm=20150520&fcltId=&paramDprtmId=&waitState=&strUseBgnDtm=2015%EB%85%84+05%EC%9B%94+20%EC%9D%BC&availDay=20150526&areaCode=3000001&dprtmId=0000&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&fcltMdclsCd=00000&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&accst=00000000&_accst=on&_accst=on&_accst=on&_accst=on&_accst=on&_accst=on&_accst=on&_accst=on&_accst=on&reserCheck=Y&_reserCheck=on&_waitCheck=on";
            string html = RequestWebPage("http://huyang.go.kr/reservation/reservationSearch.action",
                                         "http://huyang.go.kr/reservation/reservationSearch.action", makeSendData(), cookie);

            Debug("c:\\Search.txt", html);
            if (!html.Contains("javascript:jsReservation"))
            {
                Console.WriteLine("Search Fail : There is no avail rooms.");
                return -1;
            }
            Console.WriteLine("Search Success");

            // TODO
            //  1. make reserve list as array
            //  2. reserve from backwards


            return Parse(html);
        }
        public int Parse(string html)
        {
            //www.rubular.com/
            //huyang.go.kr/reservation/reservationSelect.action?elementPerPage=10&rsrvtQntt=1&useBgnDtm=20150520&fcltId=F01010300200046&paramDprtmId=0101&waitState=0&strUseBgnDtm=2015%EB%85%84+05%EC%9B%94+20%EC%9D%BC&availDay=20150526&areaCode=3000001&dprtmId=0000&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&fcltMdclsCd=00000&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&accst=00000000&_accst=on&_accst=on&_accst=on&_accst=on&_accst=on&_accst=on&_accst=on&_accst=on&_accst=on&reserCheck=Y&_reserCheck=on&_waitCheck=on
            //<img src="/user/images/sub/btn_checkout.gif" onClick="javascript:jsReservation('0101','F01010300100051','20150520','1','0')" class="hand" alt="예약가능" />

            string pattern = "<img src=\"/user/images/sub/btn_checkout.gif\" onClick=\"javascript:jsReservation" + Regex.Escape("(") + "\'(?<paramDprtmId>.*?)\',\'(?<fcltId>.*?)\'";
            //string pattern = "<img src=\"/user/images/sub/btn_checkout.gif\" onClick=\"javascript:jsReservation" + Regex.Escape("(") + "\'(.*?)\',\'(.*?)\'";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(html);
            if (match.Success == false)
            {
                Console.WriteLine("Parse fail : There is no avail rooms.");
                return -1;
            }
#if true
            for (int cnt = 0; match.Success; match = match.NextMatch())
            {
                paramDprtmId = match.Groups["paramDprtmId"].Value;
                fcltId = match.Groups["fcltId"].Value;

                waitState = "0";

                Console.WriteLine(string.Format("Select List : {0}, {1}", paramDprtmId, fcltId));
                Select();

                if (++cnt >= 3)
                    break;
            }

            return 0;
#else
            if (match.Success)
            {
               fcltId = match.Groups[2].Value; //0101 
                paramDprtmId = match.Groups[1].Value; //F01010300200038
                
                waitState = "0";
            }
            else
                return -1;
            


            Console.WriteLine(string.Format("Select List : {0}, {1}", fcltId, paramDprtmId));

            return Select();
#endif

        }
        public int Select(string _paramDprtmId, string _fcltId)
        {
            paramDprtmId = _paramDprtmId;
            fcltId = _fcltId;
            waitState = "0";

            Console.WriteLine(string.Format("Select List : {0}, {1}", fcltId, paramDprtmId));

            return Select();
        }
        public int Select()
        {
            //string sendData = string.Format("elementPerPage=10&rsrvtQntt=1&useBgnDtm=20150520&fcltId={0}&paramDprtmId={1}&waitState=0&strUseBgnDtm=2015%EB%85%84+05%EC%9B%94+20%EC%9D%BC&availDay=20150526&areaCode=3000001&dprtmId=0000&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&_dprtmId=on&fcltMdclsCd=00000&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&_fcltMdclsCd=on&accst=00000000&_accst=on&_accst=on&_accst=on&_accst=on&_accst=on&_accst=on&_accst=on&_accst=on&_accst=on&reserCheck=Y&_reserCheck=on&_waitCheck=on", fcltId, paramDprtmId);
            string html = RequestWebPage("http://huyang.go.kr/reservation/reservationSearch.action",
                                         "http://huyang.go.kr/reservation/reservationSelect.action", makeSendData(), cookie);
            Debug("c:\\Select.txt", html);
            return Reserve();
        }
        public int Reserve()
        {
            //huyang.go.kr/reservation/reservationSelectProc.action?fcltId=F01010300100051&useBgnDtm=20150520&rsrvtQntt=1&paramDprtmId=0101&fcltDfuseTpcd=01&fcltRsrvtTpcd=01&agreeCheck=Y&setCk=Y&bucksCk=Y
            //string sendData = string.Format("fcltId={0}&useBgnDtm=20150520&rsrvtQntt=1&paramDprtmId={1}&fcltDfuseTpcd=01&fcltRsrvtTpcd=01&agreeCheck=Y&setCk=Y&bucksCk=Y", fcltId, paramDprtmId);
            string html = RequestWebPage("http://huyang.go.kr/reservation/reservationSelect.action",
                                         "http://huyang.go.kr/reservation/reservationSelectProc.action", makeSendData(), cookie);
            Debug("c:\\Reserve.txt", html);

            if (!html.Contains("예약내역"))
            {
                Console.WriteLine("Reservation Fail");
                return -1;
            }
            Console.WriteLine("Reservation Success");

            return 0;
        }


        public string GetAvailMonth(string html)
        {
            string pattern = "<option value=\"(?<availMonth>.[0-9][0-9][0-9][0-9][0-9][0-9]?)\"(?<unused>.*?)>(?<name>.*?)</option>";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(html);
            if (match.Success == false)
                return string.Empty;

            return match.Groups["availMonth"].Value;
        }
        public int GetSelectableDprtm(ref List<Object> items, ref string availMonth)
        {
            string html = RequestWebPage("http://huyang.go.kr/reservation/reservationMonthSearch.action",
                                         "http://huyang.go.kr/reservation/reservationMonthSearch.action", "", cookie);

            string pattern = "<option value=\"(?<dprtm>.[0-9][0-9][0-9][0-9]?)\"(?<unused>.*?)>(?<name>.*?)</option>";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(html);
            if (match.Success == false)
                return -1;

            for (; match.Success; match = match.NextMatch())
            {
                items.Add(new { Text = match.Groups["name"].Value, Value = match.Groups["dprtm"].Value });
            }

            availMonth = GetAvailMonth(html);
            return 0;
        }
        public int GetSelectablefcltMdcls(ref List<Object> items, string dprtm)
        {
            string html = RequestWebPage("http://huyang.go.kr/common/ajax/selectFcltMdCls.action",
                                         "http://huyang.go.kr/common/ajax/selectFcltMdCls.action", "dprtm=" + dprtm, cookie);

            string pattern = "\"DEFAULT_CODE\":\"(?<fcltMdcls>.*?)\", \"CODE_NM\":\"(?<name>.*?)\"";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(html);
            if (match.Success == false)
                return -1;

            for (; match.Success; match = match.NextMatch())
            {
                items.Add(new { Text = match.Groups["name"].Value, Value = match.Groups["fcltMdcls"].Value });
            }

            return 0;
        }
        public int GetSelectableRooms(ref List<Object> items, string dprtm, string fcltMdcls, string availMonth)
        {
            string param = string.Format("fcltId=&rsrvtQntt=&useBgnDtm=20130601&paramDprtmId=&waitState=&dprtm={0}&fcltMdcls={1}&availMonth={2}", dprtm, fcltMdcls, availMonth);
            string html = RequestWebPage("http://huyang.go.kr/reservation/reservationMonthSearch.action",
                                         "http://huyang.go.kr/reservation/reservationMonthSearch.action", param, cookie);

            // <a href=\"javascript:win_roomInfo('/common/popup/productInfoPop.action?dprtm_id=0113&fclt_id=F01130300100038');\">\r\n                                            \r\n                                              꾀꼬리(5인실)(33.0㎡)\r\n                                            \r\n                                            \r\n\r\n                                            \r\n                                            </a>\r\n       
            // fclt_id=(?<fclt_id>.*?)\'(.*?)\\r\\n(.*?)\\r\\n(?<name>.*\(?)\(
            string pattern = "fclt_id=(?<fclt_id>.*?)\'(.*?)\\r\\n(.*?)\\r\\n(.*?)(?<name>.*[0-9]?)" + Regex.Escape("(");

            Regex regex = new Regex(pattern);
            Match match = regex.Match(html);
            if (match.Success == false)
                return -1;

            for (; match.Success; match = match.NextMatch())
            {
                items.Add(new { Text = match.Groups["name"].Value.Trim(), Value = match.Groups["fclt_id"].Value });
            }
            return 0;
        }
    }
}
